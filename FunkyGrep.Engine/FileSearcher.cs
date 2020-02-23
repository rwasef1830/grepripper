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
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        readonly bool _skipBinaryFiles;
        readonly ThreadLocal<Regex> _threadLocalRegex;

        readonly object _locker = new object();
        readonly int _maxContextLength;
        long _doneCount;
        long _failedCount;
        long _skippedCount;
        Task _progressReportTask;
        Task _searchTask;

        // Progress tracking
        long _totalCount;

        public FileSearcher(
            Regex expression,
            IEnumerable<IDataSource> dataSources,
            bool skipBinaryFiles,
            int maxContextLength = DefaultMaxContextLength)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            this._dataSources = dataSources ?? throw new ArgumentNullException(nameof(dataSources));
            this._skipBinaryFiles = skipBinaryFiles;
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
                        var stopwatch = Stopwatch.StartNew();
                        Exception error = null;

                        try
                        {
                            _ = this.DoSearch(this._cancelSrc.Token);
                        }
                        catch (OperationCanceledException) { }
                        catch (Exception ex)
                        {
                            error = ex;
                        }
                        finally
                        {
                            this.Completed?.Invoke(this, new CompletedEventArgs(stopwatch.Elapsed, error));
                        }
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
                        try
                        {
                            // Enter report loop
                            do
                            {
                                this.ProgressChanged?.Invoke(
                                    this,
                                    new ProgressEventArgs(
                                        Interlocked.Read(ref this._doneCount),
                                        Interlocked.Read(ref this._totalCount),
                                        Interlocked.Read(ref this._failedCount),
                                        Interlocked.Read(ref this._skippedCount)));
                            }
                            while (!this._cancelSrc.Token.WaitHandle.WaitOne(100)
                                   && !this._searchTask.IsCompleted);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
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
            var byteArrayPool = ArrayPool<byte>.Shared;
            var sByteArrayPool = ArrayPool<sbyte>.Shared;

            try
            {
                return Parallel.ForEach(
                    this._dataSources,
                    new ParallelOptions { CancellationToken = token },
                    (dataSource, loopState, i) =>
                    {
                        try
                        {
                            var length = dataSource.GetLength();

                            if (length > MaxFileSize || length == 0)
                            {
                                return;
                            }

                            token.ThrowIfCancellationRequested();

                            var matches = new List<MatchedLine>();

                            using (var stream = dataSource.OpenRead())
                            {
                                var byteBuffer = byteArrayPool.Rent(4096);
                                try
                                {
                                    var bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length);
                                    if (IsLikelyToBeBinary(byteBuffer, bytesRead))
                                    {
                                        Interlocked.Increment(ref this._skippedCount);
                                        return;
                                    }
                                }
                                finally
                                {
                                    byteArrayPool.Return(byteBuffer);
                                }

                                stream.Seek(0, SeekOrigin.Begin);

                                using var reader = new StreamReader(stream, true);

                                int lineNumber = 1;
                                for (string line = reader.ReadLine();
                                    line != null;
                                    line = reader.ReadLine(), lineNumber++)
                                {
                                    // ReSharper disable once AccessToDisposedClosure - Dispose happens after lambda is called.
                                    var regexMatches = this._threadLocalRegex.Value.Matches(line);
                                    if (regexMatches.Count == 0)
                                    {
                                        continue;
                                    }

                                    foreach (Match match in regexMatches)
                                    {
                                        if (!match.Success)
                                        {
                                            continue;
                                        }

                                        var lastMatchedLine = this.GetMatchedLine(match, line, lineNumber);
                                        matches.Add(lastMatchedLine);
                                    }

                                    token.ThrowIfCancellationRequested();
                                }
                            }

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

        static bool IsLikelyToBeBinary(byte[] byteBuffer, int bytesRead)
        {
            var nullCount = 0;
            var twoConsecutiveNullsFound = false;

            for (var j = 0; j < bytesRead; j++)
            {
                if (byteBuffer[j] != 0)
                {
                    continue;
                }

                nullCount++;
                if (bytesRead <= j + 1)
                {
                    continue;
                }

                if (byteBuffer[j + 1] != 0)
                {
                    continue;
                }

                nullCount++;
                twoConsecutiveNullsFound = true;
                break;
            }

            return twoConsecutiveNullsFound && nullCount > 2;
        }

        MatchedLine GetMatchedLine(Capture match, string line, int lineNumber)
        {
            int maxLeftChars = this._maxContextLength - Math.Max(1, match.Length / 2);
            int minLeftIndex = match.Index - maxLeftChars;

            if (minLeftIndex < 0)
            {
                minLeftIndex = 0;
            }

            int contextStartIndex = Math.Max(0, minLeftIndex);
            int contextLength = Math.Min(line.Length - contextStartIndex, this._maxContextLength);

            var clampedLine = line;
            var adjustedMatchIndex = match.Index;

            if (contextStartIndex != 0 || contextLength != line.Length)
            {
                clampedLine = line.Substring(contextStartIndex, contextLength);
                adjustedMatchIndex -= contextStartIndex;
            }

            return new MatchedLine(
                lineNumber,
                clampedLine,
                adjustedMatchIndex,
                match.Length);
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
