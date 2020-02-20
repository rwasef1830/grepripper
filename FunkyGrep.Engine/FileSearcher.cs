#region License
// Copyright (c) 2012 Raif Atef Wasef
// This source file is licensed under the  MIT license.
// 
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom 
// the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY 
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
// OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FunkyGrep.Engine
{
    public class FileSearcher
    {
        public const long MaxFileSize = 256 * 1024 * 1024;
        public const int DefaultMaxContextLength = 512;

        readonly CancellationTokenSource _cancelSrc;

        readonly IEnumerable<IDataSource> _dataSources;
        readonly ThreadLocal<Regex> _threadLocalRegex;

        readonly object _locker = new object();
        readonly int _maxContextLength;
        long _doneCount;
        long _failedCount;
        Task _progressReportTask;
        Task _searchTask;

        // Progress tracking
        long _totalCount;

        public FileSearcher(
            Regex expression,
            IEnumerable<IDataSource> dataSources,
            int maxContextLength = DefaultMaxContextLength)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            this._dataSources = dataSources ?? throw new ArgumentNullException(nameof(dataSources));
            this._threadLocalRegex = new ThreadLocal<Regex>(() => new Regex(expression.ToString(), expression.Options));
            this._maxContextLength = maxContextLength;

            this._cancelSrc = new CancellationTokenSource();
        }

        public event EventHandler<ProgressEventArgs> ProgressChanged;
        public event EventHandler<MatchFoundEventArgs> MatchFound;
        public event EventHandler<CompletedEventArgs> Completed;

        public void Begin()
        {
            lock (this._locker)
            {
                if (this._searchTask != null && !this._searchTask.IsCompleted)
                {
                    throw new InvalidOperationException("Searcher task is already running.");
                }

                this._searchTask = Task.Run(
                    () =>
                    {
                        try
                        {
                            var stopwatch = Stopwatch.StartNew();
                            var loopResult = this.DoSearch(this._cancelSrc.Token);
                            stopwatch.Stop();

                            if (loopResult.IsCompleted)
                            {
                                this.Completed?.Invoke(this, new CompletedEventArgs(stopwatch.Elapsed));
                            }
                        }
                        catch (OperationCanceledException) { }
                    },
                    this._cancelSrc.Token);

                // Find out the total number of files on a separate thread
                Task.Run(
                    () =>
                    {
                        try
                        {
                            int totalCount = 0;

                            using (IEnumerator<IDataSource> enumerator = this._dataSources.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    if (this._cancelSrc.IsCancellationRequested)
                                    {
                                        return;
                                    }

                                    totalCount++;
                                }
                            }

                            Interlocked.Exchange(ref this._totalCount, totalCount);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    });

                this._progressReportTask = Task.Run(
                    () =>
                    {
                        // Enter report loop
                        do
                        {
                            this.ProgressChanged?.Invoke(
                                this,
                                new ProgressEventArgs(
                                    Interlocked.Read(ref this._doneCount),
                                    Interlocked.Read(ref this._totalCount),
                                    Interlocked.Read(ref this._failedCount)));
                        }
                        while (!this._cancelSrc.Token.WaitHandle.WaitOne(100)
                               && !this._searchTask.IsCompleted);
                    },
                    this._cancelSrc.Token);
            }
        }

        public void Cancel()
        {
            lock (this._locker)
            {
                if (this._searchTask == null)
                {
                    return;
                }

                this._cancelSrc.Cancel();
                this.Wait();
            }
        }

        public void Wait()
        {
            try
            {
                Task.WaitAll(this._progressReportTask, this._searchTask);

                this._progressReportTask = null;
                this._searchTask = null;
            }
            catch (AggregateException aex)
            {
                aex.Handle(ex => ex is OperationCanceledException);
            }
            finally
            {
                this._progressReportTask = null;
                this._searchTask = null;
            }
        }

        ParallelLoopResult DoSearch(CancellationToken token)
        {
            try
            {
                return Parallel.ForEach(
                    this._dataSources,
                    new ParallelOptions { CancellationToken = token },
                    (dataSource, loopState, i) =>
                    {
                        try
                        {
                            if (dataSource.GetLength() > MaxFileSize)
                            {
                                return;
                            }

                            token.ThrowIfCancellationRequested();

                            string fileContents;
                            using (var reader = dataSource.OpenReader())
                            {
                                fileContents = reader.ReadToEnd();
                            }

                            token.ThrowIfCancellationRequested();

                            // ReSharper disable once AccessToDisposedClosure - Dispose happens after lambda is called.
                            var regexMatches = this._threadLocalRegex.Value.Matches(fileContents);
                            if (regexMatches.Count == 0)
                            {
                                return;
                            }

                            var matches = regexMatches
                                .Select(
                                    match =>
                                    {
                                        string lineText = this.GetMatchFullLineTextClamped(
                                            match,
                                            fileContents);
                                        int lineNumber = GetLineNumber(fileContents, match.Index);
                                        return new MatchedLine(lineNumber, lineText);
                                    })
                                .ToList();

                            this.MatchFound?.Invoke(this, new MatchFoundEventArgs(dataSource.Identifier, matches));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                            Interlocked.Increment(ref this._failedCount);
                        }
                        finally
                        {
                            Interlocked.Increment(ref this._doneCount);
                        }
                    });
            }
            finally
            {
                this._threadLocalRegex.Dispose();
            }
        }

        string GetMatchFullLineTextClamped(Capture match, string fileContents)
        {
            int contextStartIndex = -1;
            int contextLength = -1;

            int maxLeftChars = this._maxContextLength - Math.Max(1, match.Length / 2);
            int minLeftIndex = match.Index - maxLeftChars;
            if (minLeftIndex < 0) minLeftIndex = 0;

            foreach (char newLineChar in "\r\n")
            {
                int index = fileContents.LastIndexOf(newLineChar, match.Index);
                if (index < 0) continue;

                // Skip CR or LF
                index++;
                // If we had got a CR and next is LF, skip that too.
                if (newLineChar == '\r' && fileContents[index] == '\n')
                {
                    index++;
                }

                contextStartIndex = Math.Max(index, minLeftIndex);
                break;
            }

            if (contextStartIndex == -1)
            {
                contextStartIndex = 0;
            }

            foreach (char newLineChar in "\n\r")
            {
                int index = fileContents.IndexOf(newLineChar, match.Index + match.Length);
                if (index < 0) continue;

                // Skip CR or LF
                index--;
                // If we had got a LF and prev is CR, skip that too.
                if (newLineChar == '\n' && fileContents[index] == '\r')
                {
                    index--;
                }

                contextLength = Math.Min(index - contextStartIndex + 1, this._maxContextLength);
                break;
            }

            if (contextLength == -1)
            {
                contextLength = Math.Min(fileContents.Length, this._maxContextLength);
            }

            string result = fileContents.Substring(contextStartIndex, contextLength);

            return result;
        }

        static int GetLineNumber(string text, int position)
        {
            int lineNumber = 1;

            for (int i = 0; i <= position - 1; i++)
            {
                if (text[i] == '\r')
                {
                    if (i + 1 <= position - 1 && text[i + 1] == '\n')
                    {
                        i++;
                    }

                    lineNumber++;
                    continue;
                }

                if (text[i] == '\n')
                {
                    lineNumber++;
                }
            }

            return lineNumber;
        }
    }
}
