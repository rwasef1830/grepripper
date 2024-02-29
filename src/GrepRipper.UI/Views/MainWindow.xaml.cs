using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using GrepRipper.UI.Util;
using GrepRipper.UI.ViewModels;
using Prism.Validation;

namespace GrepRipper.UI.Views;

public partial class MainWindow
{
    SearchOperationViewModel? _lastSearchOperation;

    public MainWindow()
    {
        this.InitializeComponent();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == DataContextProperty)
        {
            if (e.OldValue is MainWindowViewModel oldViewModel)
            {
                oldViewModel.Search.PropertyChanged -= this.HandleSearchPropertyChanged;
                oldViewModel.ErrorsChanged -= this.HandleModelPropertyChanged;
            }

            if (e.NewValue is MainWindowViewModel newViewModel)
            {
                this.HandleSearchPropertyChanged(
                    newViewModel.Search,
                    new PropertyChangedEventArgs(nameof(newViewModel.Search.Operation)));
                newViewModel.Search.PropertyChanged += this.HandleSearchPropertyChanged;
                newViewModel.ErrorsChanged += this.HandleModelPropertyChanged;
            }
        }

        base.OnPropertyChanged(e);
    }

    void HandleSearchPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not SearchViewModel searchViewModel)
        {
            return;
        }

        if (e.PropertyName != nameof(searchViewModel.Operation))
        {
            return;
        }

        if (this._lastSearchOperation != null)
        {
            BindingOperations.DisableCollectionSynchronization(this._lastSearchOperation.Results);
            BindingOperations.DisableCollectionSynchronization(this._lastSearchOperation.SearchErrors);
        }

        BindingOperations.EnableCollectionSynchronization(
            searchViewModel.Operation.Results,
            searchViewModel.Operation.ResultsLocker);
        BindingOperations.EnableCollectionSynchronization(
            searchViewModel.Operation.SearchErrors,
            searchViewModel.Operation.SearchErrorsLocker);

        this._lastSearchOperation = searchViewModel.Operation;
    }

    void HandleModelPropertyChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        if (sender is not BindableValidator viewModel)
        {
            return;
        }

        if (e.PropertyName?.Length == 0 && viewModel.Errors[string.Empty].Count > 0)
        {
            this.Dispatcher.Invoke(
                (Window self, BindableValidator bindableValidator) =>
                    MessageBox.Show(
                        self,
                        bindableValidator.Errors[string.Empty][0],
                        "Error during search",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error),
                this,
                viewModel);
        }
    }

    void HandleDirectoryAutoCompleteBoxPopulating(object sender, PopulatingEventArgs e)
    {
        var autoCompleteBox = (AutoCompleteBox)sender;
        string text = autoCompleteBox.Text;
        string[] subDirectories = Array.Empty<string>();

        if (!string.IsNullOrWhiteSpace(text))
        {
            string? directoryName = Path.GetDirectoryName(text);
            if (DirectoryUtil.ExistsOrNullIfTimeout(directoryName ?? text, TimeSpan.FromSeconds(2)) ?? false)
            {
                try
                {
                    subDirectories = Directory.GetDirectories(
                        directoryName ?? text,
                        "*",
                        SearchOption.TopDirectoryOnly);
                }
                catch
                {
                    // ignore
                }
            }
        }

        autoCompleteBox.ItemsSource = subDirectories;
        autoCompleteBox.PopulateComplete();
    }

    void HandleDataGridPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        var depObj = (DependencyObject?)e.OriginalSource;

        if (depObj is Inline inline)
        {
            depObj = inline.Parent;
            if (depObj == null)
            {
                return;
            }
        }

        while (depObj != null)
        {
            depObj = VisualTreeHelper.GetParent(depObj);

            switch (depObj)
            {
                case DataGridColumnHeader:
                    e.Handled = true;
                    return;
                
                case DataGridRow dataGridRow:
                    dataGridRow.IsSelected = true;
                    return;
            }
        }
    }

    void HandleDataGridRowPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var row = (DataGridRow)sender;
        DragDrop.DoDragDrop(
            row,
            new DataObject(DataFormats.FileDrop, new[] { ((IFileItem)row.Item).AbsoluteFilePath }),
            DragDropEffects.All);
    }

    void HandleDataGridRowPreviewMouseDoubleClick(object? sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGridRow row)
        {
            return;
        }
        
        var fileItem = (IFileItem)row.Item;
        var viewModel = (MainWindowViewModel)this.DataContext;

        var parameters = new OpenFileInEditorParameters(
            viewModel.Settings.Editors[viewModel.Settings.DefaultEditorIndex],
            fileItem);

        if (viewModel.OpenFileInEditorCommand.CanExecute(parameters))
        {
            viewModel.OpenFileInEditorCommand.Execute(parameters);
        }
    }

    void HandleMainWindowClosing(object sender, CancelEventArgs e)
    {
        var viewModel = (MainWindowViewModel)this.DataContext;
        viewModel.SaveSettings();
    }

    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    void HandleTextBoxSearchPatternPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var viewModel = (MainWindowViewModel)this.DataContext;

        switch (e.Key)
        {
            case Key.Down when viewModel.Search.ContextLineCount == 0:
                return;
            case Key.Down:
                viewModel.Search.ContextLineCount--;
                break;
            case Key.Up when viewModel.Search.ContextLineCount == SearchViewModel.MaxContextLineCount:
                return;
            case Key.Up:
                viewModel.Search.ContextLineCount++;
                break;
            default:
                return;
        }

        e.Handled = true;
    }
}
