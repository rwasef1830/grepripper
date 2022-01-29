using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows.Input;
using FunkyGrep.UI.Services;
using FunkyGrep.UI.Validation;
using JetBrains.Annotations;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using Prism.Commands;
using Prism.Validation;

namespace FunkyGrep.UI.ViewModels;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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

    public ICommand CopyTextToClipboardCommand { get; }

    public ICommand CopyFileToClipboardCommand { get; }

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

        this.Search = new SearchViewModel();
        this.ShowSelectFolderDialogCommand = new DelegateCommand(
            this.ShowSelectFolderDialog,
            () => this.Search.Operation.Status != SearchOperationStatus.Running);
        this.CopyTextToClipboardCommand = new DelegateCommand<object>(this.CopyTextToClipboard);
        this.CopyFileToClipboardCommand = new DelegateCommand<string>(this.CopyFileToClipboard);
        this.OpenFileInEditorCommand = new DelegateCommand<OpenFileInEditorParameters>(this.OpenFileInEditor);
        
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

    void CopyTextToClipboard(object? obj)
    {
        var text = obj?.ToString();

        if (text == null)
        {
            return;
        }

        this._clipboardService.SetText(text);
    }

    void CopyFileToClipboard(string absoluteFilePath)
    {
        if (!File.Exists(absoluteFilePath))
        {
            return;
        }

        this._clipboardService.SetFileDropList(new[] { absoluteFilePath });
    }

    void OpenFileInEditor(OpenFileInEditorParameters parameters)
    {
        try
        {
            var (editorInfo, item) = parameters;
            var (_, executablePath, argumentsTemplate) = editorInfo;

            var itemFilePath = item.AbsoluteFilePath;
            var lineNumber = 0;

            if (item is SearchResultItem resultItem)
            {
                lineNumber = resultItem.Match.LineNumber;
            }

            var arguments = string.Format(argumentsTemplate, itemFilePath, lineNumber);

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

    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
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
