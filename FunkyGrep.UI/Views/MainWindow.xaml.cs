using System.IO;
using System.Windows.Controls;

namespace FunkyGrep.UI.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
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
