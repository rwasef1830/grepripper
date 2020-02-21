using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
                    oldViewModel.ErrorsChanged -= HandleModelPropertyChanged;
                }

                if (e.NewValue != null && e.NewValue is MainWindowViewModel newViewModel)
                {
                    BindingOperations.EnableCollectionSynchronization(
                        newViewModel.SearchResults,
                        newViewModel.SearchResultsLocker);
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
                if (Directory.Exists(directoryName ?? text))
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
    }
}
