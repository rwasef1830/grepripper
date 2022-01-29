using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using FunkyGrep.Engine;
using FunkyGrep.UI.Validation;
using Prism.Validation;

namespace FunkyGrep.UI.ViewModels;

public class SearchOperationViewModel : ValidatableBindableBase
{
    FileSearcher? _searcher;
    string? _directory;
    TimeSpan _lastSearchDuration;
    SearchOperationStatus _status;
    long _searchedCount;
    long _failedCount;
    long _skippedCount;
    long? _totalFileCount;

    public ObservableCollection<SearchResultItem> Results { get; }

    public ObservableCollection<SearchErrorItem> SearchErrors { get; }

    public object ResultsLocker { get; }

    public object SearchErrorsLocker { get; }

    public SearchOperationStatus Status
    {
        get => this._status;
        private set
        {
            if (this.SetProperty(ref this._status, value))
            {
                this.RaisePropertyChanged(nameof(this.IsNotRunning));
            }
        }
    }

    public bool IsNotRunning => this.Status != SearchOperationStatus.Running;

    public TimeSpan LastSearchDuration
    {
        get => this._lastSearchDuration;
        set => this.SetProperty(ref this._lastSearchDuration, value);
    }

    public long SearchedCount
    {
        get => this._searchedCount;
        set => this.SetProperty(ref this._searchedCount, value);
    }

    public long FailedCount
    {
        get => this._failedCount;
        set => this.SetProperty(ref this._failedCount, value);
    }

    public long SkippedCount
    {
        get => this._skippedCount;
        set => this.SetProperty(ref this._skippedCount, value);
    }

    public long? TotalFileCount
    {
        get => this._totalFileCount;
        set
        {
            if (this.SetProperty(ref this._totalFileCount, value))
            {
                this.RaisePropertyChanged(nameof(this.TotalFileCountIsSet));
            }
        }
    }

    public bool TotalFileCountIsSet => this.TotalFileCount.HasValue;

    public SearchOperationViewModel()
    {
        this.Results = new ObservableCollection<SearchResultItem>();
        this.SearchErrors = new ObservableCollection<SearchErrorItem>();
        this.ResultsLocker = new object();
        this.SearchErrorsLocker = new object();
    }

    void Update(object? _, ProgressEventArgs progressEventArgs)
    {
        this.SearchedCount = progressEventArgs.SearchedCount;
        this.SkippedCount = progressEventArgs.SkippedCount;
        this.FailedCount = progressEventArgs.FailedCount;

        if (progressEventArgs.TotalCount > 0)
        {
            this.TotalFileCount = progressEventArgs.TotalCount;
        }
    }

    public void Start(string directory, FileSearcher fileSearcher)
    {
        if (this.Status != SearchOperationStatus.NeverRun)
        {
            throw new InvalidOperationException("Search already started.");
        }

        if (this._searcher != null)
        {
            throw new InvalidOperationException("Instances are not reusable. Please create a new instance.");
        }

        try
        {
            this.Status = SearchOperationStatus.Running;
            this._directory = directory;

            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(directory));
            }

            this._searcher = fileSearcher ?? throw new ArgumentNullException(nameof(fileSearcher));
            this._searcher.MatchFound += this.HandleMatchFound;
            this._searcher.ProgressChanged += this.Update;
            this._searcher.Error += this.HandleError;
            this._searcher.Reset += this.HandleReset;
            this._searcher.Completed += this.HandleCompleted;

            this._searcher.Begin();
        }
        catch (Exception ex)
        {
            try
            {
                this.CleanUpSearch();
                this.SetGeneralError(ex);
            }
            catch
            {
                // ignore
            }
            finally
            {
                this.Status = SearchOperationStatus.Aborted;
            }
        }
    }

    void HandleMatchFound(object? _, MatchFoundEventArgs args)
    {
        if (this._directory == null)
        {
            throw new InvalidOperationException();
        }
        
        int basenameLength = this._directory.Length;

        if (this._directory[^1] != Path.DirectorySeparatorChar)
        {
            basenameLength++;
        }

        lock (this.ResultsLocker)
        {
            string relativePath = args.FilePath[basenameLength..];
            foreach (var match in args.Matches)
            {
                var searchResultItem = new SearchResultItem(args.FilePath, relativePath, match);
                this.Results.Add(searchResultItem);
            }
        }
    }
    
    void HandleError(object? _, SearchErrorEventArgs args)
    {
        if (this._directory == null)
        {
            throw new InvalidOperationException();
        }
        
        int basenameLength = this._directory.Length;

        if (this._directory[^1] != Path.DirectorySeparatorChar)
        {
            basenameLength++;
        }

        lock (this.SearchErrorsLocker)
        {
            string relativePath = args.FilePath;
            if (args.FilePath.Length > basenameLength)
            {
                relativePath = args.FilePath[basenameLength..];
            }

            var errorString = args.Error is IOException or UnauthorizedAccessException
                ? args.Error.Message
                : args.Error.ToStringDemystified();

            var searchErrorItem = new SearchErrorItem(args.FilePath, relativePath, errorString);
            this.SearchErrors.Add(searchErrorItem);
        }
    }
    
    void HandleReset(object? _, EventArgs e)
    {
        lock (this.SearchErrorsLocker)
        {
            this.SearchErrors.Clear();
        }

        lock (this.ResultsLocker)
        {
            this.Results.Clear();
        }
    }
    
    void HandleCompleted(object? _, CompletedEventArgs args)
    {
        this.Update(null, args.FinalProgressUpdate);
        this.LastSearchDuration = args.Duration;

        if (args.FailureReason != null)
        {
            this.Status = SearchOperationStatus.Aborted;
            this.SetGeneralError(args.FailureReason);
        }
        else
        {
            this.Status = SearchOperationStatus.Completed;
        }
    }

    public void Cancel()
    {
        try
        {
            this.CleanUpSearch();
        }
        catch (Exception ex)
        {
            this.SetGeneralError(ex);
        }
        finally
        {
            this.Status = SearchOperationStatus.Aborted;
        }
    }

    void CleanUpSearch()
    {
        this._searcher?.Cancel();
    }
}
