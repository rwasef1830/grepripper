#region License
// Copyright (c) 2020 Raif Atef Wasef
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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using FunkyGrep.Engine;
using FunkyGrep.Engine.Specifications;
using FunkyGrep.UI.Services;
using FunkyGrep.UI.Validation.DataAnnotations;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using Prism.Commands;
using Prism.Validation;

namespace FunkyGrep.UI.ViewModels
{
    public class MainWindowViewModel : ValidatableBindableBase
    {
        readonly IDialogService _dialogService;
        readonly IClipboardService _clipboardService;
        readonly IProcessService _processService;

        string _directory;
        bool _includeSubDirectories;
        string _filePatternsSpaceSeparated;
        IReadOnlyList<string> _filePatterns;
        string _searchText;
        bool _searchTextIsRegex;
        bool _ignoreCase;
        bool _searchIsRunning;
        SearchProgressViewModel _searchProgress;
        bool? _lastSearchCompleted;
        TimeSpan? _lastSearchDuration;
        FileSearcher _searcher;
        string _lastSearchDirectory;

        [Required]
        [DirectoryExists]
        public string Directory
        {
            get => this._directory;
            set => this.SetProperty(ref this._directory, value);
        }

        public bool IncludeSubDirectories
        {
            get => this._includeSubDirectories;
            set => this.SetProperty(ref this._includeSubDirectories, value);
        }

        [Required]
        [ValidSpaceSeparatedGlobExpressions]
        public string FilePatternsSpaceSeparated
        {
            get => this._filePatternsSpaceSeparated;
            set
            {
                if (this.SetProperty(ref this._filePatternsSpaceSeparated, value))
                {
                    this.SetProperty(
                        ref this._filePatterns,
                        string.IsNullOrWhiteSpace(value)
                            ? new string[0]
                            : value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }

        public IReadOnlyList<string> FilePatterns => this._filePatterns;

        [PossiblyValidRegex]
        public string SearchText
        {
            get => this._searchText;
            set => this.SetProperty(ref this._searchText, value);
        }

        public bool SearchTextIsRegex
        {
            get => this._searchTextIsRegex;
            set
            {
                this.SetProperty(ref this._searchTextIsRegex, value);
                this.RaisePropertyChanged(nameof(this.SearchText));
            }
        }

        public bool IgnoreCase
        {
            get => this._ignoreCase;
            set => this.SetProperty(ref this._ignoreCase, value);
        }

        public SearchProgressViewModel SearchProgress
        {
            get => this._searchProgress;
            set => this.SetProperty(ref this._searchProgress, value);
        }

        public ObservableCollection<SearchResultItem> SearchResults { get; }

        public object SearchResultsLocker { get; }

        public bool? LastSearchCompleted
        {
            get => this._lastSearchCompleted;
            set => this.SetProperty(ref this._lastSearchCompleted, value);
        }

        public TimeSpan? LastSearchDuration
        {
            get => this._lastSearchDuration;
            set
            {
                if (this.SetProperty(ref this._lastSearchDuration, value))
                {
                    this.RaisePropertyChanged(nameof(this.LastSearchDurationIsSet));
                }
            }
        }

        public bool LastSearchDurationIsSet => this.LastSearchDuration.HasValue;

        public bool SearchIsRunning
        {
            get => this._searchIsRunning;
            set
            {
                if (this.SetProperty(ref this._searchIsRunning, value))
                {
                    this.RaisePropertyChanged(nameof(this.SearchIsNotRunning));
                }
            }
        }

        public bool SearchIsNotRunning => !this.SearchIsRunning;

        public ICommand ShowSelectFolderDialogCommand { get; }

        public ICommand RunSearchCommand { get; }

        public ICommand AbortSearchCommand { get; }

        public ICommand CopyFilePathToClipboardCommand { get; }

        public ICommand CopyFileToClipboardCommand { get; }

        public ICommand CopyLineNumberToClipboardCommand { get; }

        public ICommand OpenFileInEditorCommand { get; }

        public MainWindowViewModel(
            IDialogService dialogService,
            IClipboardService clipboardService,
            IProcessService processService)
        {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._clipboardService = clipboardService ?? throw new ArgumentNullException(nameof(clipboardService));
            this._processService = processService ?? throw new ArgumentNullException(nameof(processService));

            this.ShowSelectFolderDialogCommand = new DelegateCommand(
                this.ShowSelectFolderDialog,
                () => !this.SearchIsRunning);

            this.RunSearchCommand = new DelegateCommand(this.RunSearch);
            this.AbortSearchCommand = new DelegateCommand(this.AbortSearch);
            this.CopyFilePathToClipboardCommand = new DelegateCommand<SearchResultItem>(this.CopyFilePathToClipboard);
            this.CopyFileToClipboardCommand = new DelegateCommand<SearchResultItem>(this.CopyFileToClipboard);
            this.CopyLineNumberToClipboardCommand =
                new DelegateCommand<SearchResultItem>(this.CopyLineNumberToClipboard);
            this.OpenFileInEditorCommand = new DelegateCommand<SearchResultItem>(this.OpenFileInEditor);

            this.Directory = Environment.CurrentDirectory;
            this.IncludeSubDirectories = true;
            this.FilePatternsSpaceSeparated = "*";
            this.SearchTextIsRegex = true;
            this.SearchResults = new ObservableCollection<SearchResultItem>();
            this.SearchResultsLocker = new object();
        }

        void ShowSelectFolderDialog()
        {
            var settings = new FolderBrowserDialogSettings
            {
                SelectedPath = this.Directory,
                ShowNewFolderButton = false
            };

            // ReSharper disable once ConstantNullCoalescingCondition
            if (this._dialogService.ShowFolderBrowserDialog(
                this,
                settings) ?? false)
            {
                this.Directory = settings.SelectedPath;
            }
        }

        void CopyFilePathToClipboard(SearchResultItem item)
        {
            if (item == null)
            {
                return;
            }

            var itemFilePath = Path.Combine(this._lastSearchDirectory, item.FilePath);
            this._clipboardService.SetText(itemFilePath);
        }

        void CopyFileToClipboard(SearchResultItem item)
        {
            if (item == null)
            {
                return;
            }

            var itemFilePath = Path.Combine(this._lastSearchDirectory, item.FilePath);
            this._clipboardService.SetFileDropList(new[] { itemFilePath });
        }

        void CopyLineNumberToClipboard(SearchResultItem item)
        {
            if (item == null)
            {
                return;
            }

            this._clipboardService.SetText(item.LineNumber.ToString());
        }

        void OpenFileInEditor(SearchResultItem item)
        {
            if (item == null)
            {
                return;
            }

            var itemFilePath = Path.Combine(this._lastSearchDirectory, item.FilePath);
            var pi = new ProcessStartInfo(
                "notepad.exe",
                $"\"{itemFilePath}\"")
            {
                UseShellExecute = true
            };

            this._processService.Start(pi);
        }

        void RunSearch()
        {
            if (!this.ValidateProperties())
            {
                return;
            }

            try
            {
                this.LastSearchCompleted = null;
                this.LastSearchDuration = null;
                this._lastSearchDirectory = this.Directory;
                this.SearchProgress = new SearchProgressViewModel();

                this.SearchIsRunning = true;

                var patternSpec = new PatternSpecification(
                    this.SearchText,
                    this.SearchTextIsRegex,
                    this.IgnoreCase);

                var fileSpec = new FileSpecification(
                    this.Directory,
                    this.IncludeSubDirectories,
                    this.FilePatterns,
                    new string[0]);

                lock (this.SearchResultsLocker)
                {
                    this.SearchResults.Clear();
                }

                this._searcher = new FileSearcher(patternSpec.Expression, fileSpec.EnumerateFiles());
                this._searcher.MatchFound += (_, args) =>
                {
                    int basenameLength = this.Directory.Length;

                    if (!this.Directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        basenameLength++;
                    }

                    lock (this.SearchResultsLocker)
                    {
                        foreach (var line in args.Matches)
                        {
                            var localLine = line;
                            string relativePath = args.FilePath.Substring(basenameLength);
                            this.SearchResults.Add(new SearchResultItem(relativePath, localLine));
                        }
                    }
                };

                this._searcher.ProgressChanged += (_, args) =>
                {
                    if (args.TotalNumberOfFiles > 0)
                    {
                        this.SearchProgress.TotalFileCount = args.TotalNumberOfFiles;
                    }

                    this.SearchProgress.SearchedFileCount = args.NumberOfSearchedFiles;
                    this.SearchProgress.FailedFileCount = args.NumberOfFailedFiles;
                };

                this._searcher.Completed += (_, args) =>
                {
                    this.LastSearchDuration = args.Duration;
                    this.LastSearchCompleted = true;
                    this.SearchIsRunning = false;

                    if (args.Error != null)
                    {
                        this.SetAllErrors(
                            new Dictionary<string, ReadOnlyCollection<string>>
                            {
                                [string.Empty] = new ReadOnlyCollection<string>(new[] { args.Error.ToString() })
                            });
                    }
                };

                this._searcher.Begin();
            }
            catch (Exception ex)
            {
                try
                {
                    this.CleanUpSearch();
                }
                catch
                {
                    this.SetAllErrors(
                        new Dictionary<string, ReadOnlyCollection<string>>
                        {
                            [string.Empty] = new ReadOnlyCollection<string>(new[] { ex.ToString() })
                        });
                }
            }
        }

        void AbortSearch()
        {
            if (!this.SearchIsRunning)
            {
                return;
            }

            try
            {
                this.CleanUpSearch();
            }
            catch (Exception ex)
            {
                this.SetAllErrors(
                    new Dictionary<string, ReadOnlyCollection<string>>
                    {
                        [string.Empty] = new ReadOnlyCollection<string>(new[] { ex.ToString() })
                    });
            }
            finally
            {
                this.SearchIsRunning = false;
                this.LastSearchCompleted = false;
            }
        }

        void CleanUpSearch()
        {
            this._searcher?.Cancel();
        }
    }
}
