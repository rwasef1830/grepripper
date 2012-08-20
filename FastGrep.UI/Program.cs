using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NDesk.Options;

namespace FastGrep.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var optionSet = new OptionSet();
                List<string> unprocessed = optionSet.Parse(args);

                string initialFolder = unprocessed.FirstOrDefault();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(initialFolder));
            }
            catch (OptionException ex)
            {
                // ReSharper disable LocalizableElement
                MessageBox.Show(
                    "Invalid arguments passed via command line. Details: " + ex,
                    "Fatal error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                // ReSharper restore LocalizableElement
            }
            catch (Exception ex)
            {
                // ReSharper disable LocalizableElement
                MessageBox.Show(
                    "An unhandled error occurred. Details: " + ex,
                    "Fatal error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                // ReSharper restore LocalizableElement
            }
        }
    }
}
