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
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using FunkyGrep.Engine;
using FunkyGrep.Engine.Specifications;
using FunkyGrep.UI.Validation;
using FunkyGrep.UI.Validation.DataAnnotations;
using Prism.Commands;
using Prism.Validation;

namespace FunkyGrep.UI.ViewModels
{
    public class SearchViewModel : ValidatableBindableBase
    {
        string _directory;
        bool _includeSubDirectories;
        string _filePatternsSpaceSeparated;
        IReadOnlyList<string> _filePatterns;
        bool _skipBinaryFiles;
        string _searchPattern;
        bool _searchPatternIsRegex;
        bool _ignoreCase;
        byte _contextLineCount;
        SearchOperationViewModel _operation;

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

        public bool SkipBinaryFiles
        {
            get => this._skipBinaryFiles;
            set => this.SetProperty(ref this._skipBinaryFiles, value);
        }

        [RequiredAllowWhiteSpace]
        [PossiblyValidRegex(nameof(SearchPatternIsRegex))]
        public string SearchPattern
        {
            get => this._searchPattern;
            set => this.SetProperty(ref this._searchPattern, value);
        }

        public bool SearchPatternIsRegex
        {
            get => this._searchPatternIsRegex;
            set
            {
                this.SetProperty(ref this._searchPatternIsRegex, value);
                this.RaisePropertyChanged(nameof(this.SearchPattern));
            }
        }

        public bool IgnoreCase
        {
            get => this._ignoreCase;
            set => this.SetProperty(ref this._ignoreCase, value);
        }

        [Range(0, 255)]
        public byte ContextLineCount
        {
            get => this._contextLineCount;
            set => this.SetProperty(ref this._contextLineCount, value);
        }

        public SearchOperationViewModel Operation
        {
            get => this._operation;
            private set => this.SetProperty(ref this._operation, value);
        }

        public ICommand RunSearchCommand { get; }

        public ICommand AbortSearchCommand { get; }

        public SearchViewModel()
        {
            this.Directory = Environment.CurrentDirectory;
            this.IncludeSubDirectories = true;
            this.FilePatternsSpaceSeparated = "*";
            this.SkipBinaryFiles = true;
            this.SearchPatternIsRegex = true;
            this.ContextLineCount = 0;
            this.Operation = new SearchOperationViewModel();
            this.RunSearchCommand = new DelegateCommand(this.RunSearchIfValid);
            this.AbortSearchCommand = new DelegateCommand(() => this.Operation.Cancel());
        }

        void RunSearchIfValid()
        {
            if (!this.ValidateProperties())
            {
                return;
            }

            try
            {
                var patternSpec = new PatternSpecification(
                    this.SearchPattern,
                    this.SearchPatternIsRegex,
                    this.IgnoreCase);

                var fileSpec = new FileSpecification(
                    this.Directory,
                    this.IncludeSubDirectories,
                    this.FilePatterns,
                    new string[0]);

                var searcher = new FileSearcher(
                    patternSpec.Expression,
                    fileSpec.EnumerateFiles(),
                    this.SkipBinaryFiles,
                    this.ContextLineCount);

                this.Operation = new SearchOperationViewModel();
                this.Operation.BubbleFutureGeneralError(this);
                this.Operation.Start(this.Directory, searcher);
            }
            catch (Exception ex)
            {
                this.Operation.Cancel();
                this.SetGeneralError(ex);
            }
        }
    }
}
