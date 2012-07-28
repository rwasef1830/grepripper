using System;
using System.Collections.Generic;
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
        }

        public event EventHandler<MatchEventArgs> MatchFound;
        public event EventHandler<EventArgs> Completed;

        public void Start()
        {
            lock (this._locker)
            {
                if (this._searchTask != null)
                {
                    throw new InvalidOperationException("Searcher task is already running.");
                }

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

                    MatchCollection matches = this._expression.Matches(fileContents);

                    if (matches.Count == 0) return;

                    this.OnMatchFound(
                        new MatchEventArgs(fileDataSource.Identifier, fileContents, matches));
                });

            if (parallelResult.IsCompleted)
            {
                this.OnCompleted(EventArgs.Empty);
            }
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

        void OnMatchFound(MatchEventArgs e)
        {
            EventHandler<MatchEventArgs> handler = this.MatchFound;
            if (handler != null) handler(this, e);
        }

        void OnCompleted(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.Completed;
            if (handler != null) handler(this, e);
        }
    }
}