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
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using FunkyGrep.UI.Services;
using FunkyGrep.UI.Validation;
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
        readonly IAppSettingsService _appSettingsService;
        readonly IEditorFinderService _editorFinderService;

        SettingsViewModel _settings;
        bool _scanForEditorsCanExecute;

        public ICommand ShowSelectFolderDialogCommand { get; }

        public ICommand CopyAbsoluteFilePathToClipboardCommand { get; }

        public ICommand CopyRelativeFilePathToClipboardCommand { get; }

        public ICommand CopyFileToClipboardCommand { get; }

        public ICommand CopyLineNumberToClipboardCommand { get; }

        public ICommand OpenFileInEditorCommand { get; }

        public SearchViewModel Search { get; }

        public SettingsViewModel Settings
        {
            get => this._settings;
            set => this.SetProperty(ref this._settings, value);
        }

        public ICommand ScanForEditorsCommand { get; }

        public bool ScanForEditorsCanExecute
        {
            get => this._scanForEditorsCanExecute;
            set => this.SetProperty(ref this._scanForEditorsCanExecute, value);
        }

        public MainWindowViewModel(
            IDialogService dialogService,
            IClipboardService clipboardService,
            IProcessService processService,
            IAppSettingsService appSettingsService,
            IEditorFinderService editorFinderService)
        {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._clipboardService = clipboardService ?? throw new ArgumentNullException(nameof(clipboardService));
            this._processService = processService ?? throw new ArgumentNullException(nameof(processService));
            this._appSettingsService = appSettingsService
                                       ?? throw new ArgumentNullException(nameof(appSettingsService));
            this._editorFinderService = editorFinderService ?? throw new ArgumentNullException(nameof(editorFinderService));

            this.ShowSelectFolderDialogCommand = new DelegateCommand(
                this.ShowSelectFolderDialog,
                () => !(this.Search.Operation?.IsRunning ?? false));


            this.CopyAbsoluteFilePathToClipboardCommand =
                new DelegateCommand<IFileItem>(this.CopyAbsoluteFilePathToClipboard);
            this.CopyRelativeFilePathToClipboardCommand =
                new DelegateCommand<IFileItem>(this.CopyRelativeFilePathToClipboard);
            this.CopyFileToClipboardCommand = new DelegateCommand<IFileItem>(this.CopyFileToClipboard);
            this.CopyLineNumberToClipboardCommand =
                new DelegateCommand<SearchResultItem>(this.CopyLineNumberToClipboard);
            this.OpenFileInEditorCommand = new DelegateCommand<OpenFileInEditorParameters>(this.OpenFileInEditor);

            this.Search = new SearchViewModel();
            this.Search.BubbleFutureGeneralError(this);
            this._settings = this._appSettingsService.LoadOrCreate<SettingsViewModel>();

            if (this._settings.Editors.Count <= 1)
            {
                this.ScanForEditors();
            }

            this._scanForEditorsCanExecute = true;
            this.ScanForEditorsCommand = new DelegateCommand(this.ScanForEditors)
                .ObservesCanExecute(() => this.ScanForEditorsCanExecute);
        }

        void ShowSelectFolderDialog()
        {
            var settings = new FolderBrowserDialogSettings
            {
                SelectedPath = this.Search.Directory,
                ShowNewFolderButton = false
            };

            // ReSharper disable once ConstantNullCoalescingCondition
            if (this._dialogService.ShowFolderBrowserDialog(
                this,
                settings) ?? false)
            {
                this.Search.Directory = settings.SelectedPath;
            }
        }

        void CopyAbsoluteFilePathToClipboard(IFileItem item)
        {
            if (item == null)
            {
                return;
            }

            var itemFilePath = item.AbsoluteFilePath;
            this._clipboardService.SetText(itemFilePath);
        }

        void CopyRelativeFilePathToClipboard(IFileItem item)
        {
            if (item == null)
            {
                return;
            }

            var itemFilePath = item.RelativeFilePath;
            this._clipboardService.SetText(itemFilePath);
        }

        void CopyFileToClipboard(IFileItem item)
        {
            if (item == null)
            {
                return;
            }

            var itemFilePath = item.AbsoluteFilePath;
            this._clipboardService.SetFileDropList(new[] { itemFilePath });
        }

        void CopyLineNumberToClipboard(SearchResultItem item)
        {
            if (item?.Match == null)
            {
                return;
            }

            this._clipboardService.SetText(item.Match.LineNumber.ToString());
        }

        void OpenFileInEditor(OpenFileInEditorParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            try
            {
                var item = parameters.FileItem;
                var editor = parameters.Editor;

                var itemFilePath = item.AbsoluteFilePath;
                var lineNumber = 0;

                if (item is SearchResultItem resultItem)
                {
                    lineNumber = resultItem.Match.LineNumber;
                }

                var executablePath = editor.ExecutablePath;
                var arguments = string.Format(editor.ArgumentsTemplate, itemFilePath, lineNumber);

                var pi = new ProcessStartInfo(executablePath, arguments)
                {
                    UseShellExecute = true
                };

                this._processService.Start(pi);
            }
            catch (Exception ex)
            {
                this.SetGeneralError(ex);
            }
        }

        void ScanForEditors()
        {
            this.ScanForEditorsCanExecute = false;

            try
            {
                var newEditors = this._editorFinderService.FindInstalledSupportedEditors();
                var mergedEditorList = new List<EditorInfo>();

                foreach (var existingEditor in this.Settings.Editors)
                {
                    var matchingNewEditor = newEditors.FirstOrDefault(x => x.DisplayName == existingEditor.DisplayName);
                    if (matchingNewEditor != null)
                    {
                        mergedEditorList.Add(matchingNewEditor);
                        continue;
                    }

                    mergedEditorList.Add(existingEditor);
                }

                foreach (var newEditor in newEditors)
                {
                    if (mergedEditorList.All(x => x.DisplayName != newEditor.DisplayName))
                    {
                        mergedEditorList.Add(newEditor);
                    }
                }

                var mergedEditors = new ObservableCollection<EditorInfo>(mergedEditorList.OrderBy(x => x.DisplayName));
                this.Settings.Editors = mergedEditors;
                this.Settings.DefaultEditorIndex = 0;
            }
            catch (Exception ex)
            {
                this.SetGeneralError(ex);
            }
            finally
            {
                this.ScanForEditorsCanExecute = true;
            }
        }

        public void SaveSettings()
        {
            try
            {
                if (this._settings.HasErrors)
                {
                    this._settings = new SettingsViewModel();
                }

                this._appSettingsService.Save(this._settings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.SetGeneralError(ex);
            }
        }
    }
}
