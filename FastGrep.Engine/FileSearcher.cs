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
                if (this._searchTask != null)
                {
                    throw new InvalidOperationException("Searcher task is already running.");
                }

                this._stopwatch.Start();

                this._searchTask = Task.Factory.StartNew(
                    () => this.DoSearch(this._cancelSrc.Token),
                    this._cancelSrc.Token);

                this._searchTask.ContinueWith(
                    antecedent =>
                    {
                        try
                        {
                            this.Stop();
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch (Exception) { }
                        // ReSharper restore EmptyGeneralCatchClause
                    });
            }
        }

        void DoSearch(CancellationToken token)
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

                    token.ThrowIfCancellationRequested();

                    string fileContents;
                    using (fileDataSource.Reader)
                    {
                        fileContents = fileDataSource.Reader.ReadToEnd();
                    }

                    var regexMatches = this._expression.Matches(fileContents);
                    if (regexMatches.Count == 0) return;

                    var matches = regexMatches.OfType<Match>().Select(x => GetMatchLine(x, fileContents));
                    this.OnMatchFound(new MatchFoundEventArgs(fileDataSource.Identifier, matches));
                });

            if (parallelResult.IsCompleted)
            {
                this._stopwatch.Stop();
                this.OnCompleted(new CompletedEventArgs(this._stopwatch.Elapsed));
            }
        }

        static MatchedLine GetMatchLine(Capture match, string fileContents)
        {
            int contextStartIndex = match.Index;
            int contextLength = match.Length;

            foreach (var newLineChar in "\r\n")
            {
                int index = fileContents.LastIndexOf(newLineChar, contextStartIndex);
                if (index < 0) continue;

                contextStartIndex = index;
                break;
            }

            foreach (var newLineChar in "\n\r")
            {
                int index = fileContents.IndexOf(newLineChar, contextStartIndex + contextLength);
                if (index < 0) continue;

                contextLength = index - contextStartIndex;
                break;
            }

            if (contextStartIndex == match.Index)
            {
                contextStartIndex = 0;
            }

            if (contextLength == match.Length)
            {
                contextLength = fileContents.Length;
            }

            int lineNumber = GetLineNumber(fileContents, match.Index);

            return new MatchedLine(lineNumber, fileContents.Substring(contextStartIndex, contextLength));
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

        public void Stop()
        {
            lock (this._locker)
            {
                this._cancelSrc.Cancel();

                try
                {
                    this.Wait();
                }
                catch (OperationCanceledException) {}

                this._searchTask.Dispose();
                this._searchTask = null;
            }
        }

        public void Wait()
        {
            this._searchTask.Wait();
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