using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;

namespace FastGrep.Engine
{
    public class FileSearcher
    {
        public const long MaxFileSize = 256 * 1024 * 1024;
        public const int MaxContextLength = 512;

        readonly CancellationTokenSource _cancelSrc;

        readonly Regex _expression;
        readonly IEnumerable<IDataSource> _dataSources;

        readonly object _locker = new object();
        Task _searchTask;

        // Progress tracking
        long _totalCount;
        long _doneCount;
        long _failedCount;
        Task _progressReportTask;

        public FileSearcher(
            Regex expression,
            IEnumerable<IDataSource> dataSources)
        {
            Ensure.That(() => expression).IsNotNull();
            Ensure.That(() => dataSources).IsNotNull();

            this._dataSources = dataSources;
            this._expression = expression;

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

                this._searchTask = Task.Factory.StartNew(
                    () =>
                    {
                        var stopwatch = Stopwatch.StartNew();
                        var loopResult = this.DoSearch(this._cancelSrc.Token);
                        stopwatch.Stop();

                        if (loopResult.IsCompleted)
                        {
                            this.OnCompleted(new CompletedEventArgs(stopwatch.Elapsed));
                        }
                    },
                    this._cancelSrc.Token);

                // Find out the total number of files on a separate thread
                Task.Factory.StartNew(
                    () =>
                    {
                        var totalCount = 0;

                        using (var enumerator = this._dataSources.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                this._cancelSrc.Token.ThrowIfCancellationRequested();
                                totalCount++;
                            }
                        }

                        Interlocked.Exchange(ref this._totalCount, totalCount);
                    });

                this._progressReportTask = Task.Factory.StartNew(
                    () =>
                    {
                        // Enter report loop
                        do
                        {
                            this.OnProgressChanged(
                                new ProgressEventArgs(Interlocked.Read(ref this._doneCount),
                                                      Interlocked.Read(ref this._totalCount),
                                                      Interlocked.Read(ref this._failedCount)));
                        }
                        while (!this._cancelSrc.Token.WaitHandle.WaitOne(100) && !this._searchTask.IsCompleted);
                    },
                    this._cancelSrc.Token);
            }
        }

        public void Cancel()
        {
            lock (this._locker)
            {
                if (this._searchTask == null) return;

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
            return Parallel.ForEach(
                this._dataSources,
                new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                },
                (dataSource, loopState, i) =>
                {
                    try
                    {
                        if (dataSource.GetLength() > MaxFileSize) return;

                        token.ThrowIfCancellationRequested();

                        string fileContents;
                        using (var reader = dataSource.OpenReader())
                        {
                            fileContents = reader.ReadToEnd();
                        }

                        var regexMatches = this._expression.Matches(fileContents);
                        if (regexMatches.Count == 0) return;

                        var matches = regexMatches
                            .OfType<Match>()
                            .Select(match =>
                                    {
                                        var lineText = GetMatchFullLineTextClamped(match, fileContents);
                                        int lineNumber = GetLineNumber(fileContents, match.Index);
                                        return new MatchedLine(lineNumber, lineText);
                                    });

                        this.OnMatchFound(new MatchFoundEventArgs(dataSource.Identifier, matches));
                    }
                    catch (Exception)
                    {
                        // TODO: Log these exceptions and let the user view them somehow.
                        Interlocked.Increment(ref this._failedCount);
                    }
                    finally
                    {
                        // TODO: Log skipped files and let the user view them somehow.
                        Interlocked.Increment(ref this._doneCount);
                    }
                });
        }

        static string GetMatchFullLineTextClamped(Capture match, string fileContents)
        {
            int contextStartIndex = -1;
            int contextLength = -1;

            int maxLeftChars = MaxContextLength - (match.Length / 2);
            int minLeftIndex = match.Index - maxLeftChars;
            if (minLeftIndex < 0) minLeftIndex = 0;

            foreach (var newLineChar in "\r\n")
            {
                int index = fileContents.LastIndexOf(newLineChar, match.Index);
                if (index < 0) continue;

                contextStartIndex = Math.Max(index, minLeftIndex);
                break;
            }

            if (contextStartIndex == -1)
            {
                contextStartIndex = 0;
            }

            foreach (var newLineChar in "\n\r")
            {
                int index = fileContents.IndexOf(newLineChar, match.Index + match.Length);
                if (index < 0) continue;

                contextLength = Math.Min(index - contextStartIndex, MaxContextLength);
                break;
            }

            if (contextLength == -1)
            {
                contextLength = fileContents.Length;
            }

            var result = fileContents.Substring(contextStartIndex, contextLength);

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

        void OnMatchFound(MatchFoundEventArgs e)
        {
            EventHandler<MatchFoundEventArgs> handler = this.MatchFound;
            if (handler != null) handler(this, e);
        }

        void OnCompleted(CompletedEventArgs e)
        {
            EventHandler<CompletedEventArgs> handler = this.Completed;
            if (handler != null) handler(this, e);
        }

        void OnProgressChanged(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> handler = this.ProgressChanged;
            if (handler != null) handler(this, e);
        }
    }
}