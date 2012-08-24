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

        readonly CancellationTokenSource _cancelSrc;

        readonly Regex _expression;
        readonly IEnumerable<DataSource> _dataSources;
        readonly Stopwatch _stopwatch;

        readonly object _locker = new object();
        Task _searchTask;

        public FileSearcher(
            Regex expression,
            IEnumerable<DataSource> dataSources)
        {
            Ensure.That(() => expression).IsNotNull();
            Ensure.That(() => dataSources).IsNotNull();

            this._dataSources = dataSources;
            this._expression = expression;

            this._cancelSrc = new CancellationTokenSource();
            this._stopwatch = new Stopwatch();
        }

        public event EventHandler<MatchFoundEventArgs> MatchFound;
        public event EventHandler<CompletedEventArgs> Completed;

        public void Start()
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
                        this._stopwatch.Start();
                        this.DoSearch();
                    },
                    this._cancelSrc.Token);
            }
        }

        public void Cancel()
        {
            lock (this._locker)
            {
                if (this._searchTask == null) return;

                try
                {
                    this._cancelSrc.Cancel();
                    this.Wait();
                }
                catch (AggregateException aex)
                {
                    aex.Handle(ex => ex is OperationCanceledException);
                }
                finally
                {
                    this._searchTask = null;
                }
            }
        }

        public void Wait()
        {
            this._searchTask.Wait();
        }

        void DoSearch()
        {
            var parallelResult = Parallel.ForEach(
                this._dataSources,
                new ParallelOptions
                {
                    CancellationToken = this._cancelSrc.Token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                },
                (fileDataSource, loopState, i) =>
                {
                    if (fileDataSource.Length > MaxFileSize) return;

                    this._cancelSrc.Token.ThrowIfCancellationRequested();

                    string fileContents;
                    using (fileDataSource.Reader)
                    {
                        fileContents = fileDataSource.Reader.ReadToEnd();
                    }

                    var regexMatches = this._expression.Matches(fileContents);
                    if (regexMatches.Count == 0) return;

                    var matches = regexMatches
                        .OfType<Match>()
                        .Select(match =>
                                {
                                    var lineText = GetMatchFullLineText(match, fileContents);
                                    int lineNumber = GetLineNumber(fileContents, match.Index);
                                    return new MatchedLine(lineNumber, lineText);
                                });

                    this.OnMatchFound(new MatchFoundEventArgs(fileDataSource.Identifier, matches));
                });

            if (parallelResult.IsCompleted)
            {
                this._stopwatch.Stop();
                this.OnCompleted(new CompletedEventArgs(this._stopwatch.Elapsed));
            }
        }

        static string GetMatchFullLineText(Capture match, string fileContents)
        {
            int contextStartIndex = -1;
            int contextLength = -1;

            foreach (var newLineChar in "\r\n")
            {
                int index = fileContents.LastIndexOf(newLineChar, match.Index);
                if (index < 0) continue;

                contextStartIndex = index;
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

                contextLength = index - contextStartIndex;
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
    }
}