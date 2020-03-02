using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using FunkyGrep.UI.Util;
using FunkyGrep.UI.ViewModels;
using Prism.Validation;

namespace FunkyGrep.UI.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DataContextProperty)
            {
                if (e.OldValue != null && e.OldValue is MainWindowViewModel oldViewModel)
                {
                    BindingOperations.DisableCollectionSynchronization(oldViewModel.SearchResults);
                    BindingOperations.DisableCollectionSynchronization(oldViewModel.SearchErrors);
                    oldViewModel.ErrorsChanged -= HandleModelPropertyChanged;
                }

                if (e.NewValue != null && e.NewValue is MainWindowViewModel newViewModel)
                {
                    BindingOperations.EnableCollectionSynchronization(
                        newViewModel.SearchResults,
                        newViewModel.SearchResultsLocker);
                    BindingOperations.EnableCollectionSynchronization(
                        newViewModel.SearchErrors,
                        newViewModel.SearchErrorsLocker);
                    newViewModel.ErrorsChanged += HandleModelPropertyChanged;
                }
            }

            base.OnPropertyChanged(e);
        }

        static void HandleModelPropertyChanged(object sender, DataErrorsChangedEventArgs e)
        {
            var viewModel = (BindableValidator)sender;

            if (e.PropertyName.Length == 0 && viewModel.Errors[string.Empty].Count > 0)
            {
                MessageBox.Show(
                    viewModel.Errors[string.Empty][0],
                    "Error during search",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        void HandleDirectoryAutoCompleteBoxPopulating(object sender, PopulatingEventArgs e)
        {
            var autoCompleteBox = (AutoCompleteBox)sender;
            string text = autoCompleteBox.Text;
            string[] subDirectories = null;

            if (!string.IsNullOrWhiteSpace(text))
            {
                string directoryName = Path.GetDirectoryName(text);
                if (DirectoryUtil.ExistsOrNullIfTimeout(directoryName ?? text, TimeSpan.FromSeconds(2)) ?? false)
                {
                    subDirectories = Directory.GetDirectories(
                        directoryName ?? text,
                        "*.*",
                        SearchOption.TopDirectoryOnly);
                }
            }

            autoCompleteBox.ItemsSource = subDirectories;
            autoCompleteBox.PopulateComplete();
        }

        void HandleDataGridPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var depObj = (DependencyObject)e.OriginalSource;

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

                if (depObj is DataGridColumnHeader)
                {
                    e.Handled = true;
                    return;
                }

                if (depObj is DataGridRow dataGridRow)
                {
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
    }
}
