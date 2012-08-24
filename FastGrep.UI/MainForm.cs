using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using FastGrep.Engine;
using FastGrep.Engine.Specifications;

namespace FastGrep.UI
{
    public partial class MainForm : Form
    {
        FileSearcher _searcher;

        public MainForm(string initialFolder)
        {
            this.InitializeComponent();
            this.InitializeDefaults(initialFolder);
        }

        void InitializeDefaults(string initialFolder)
        {
            this._checkBoxSearchSubfolders.Checked = true;

            // ReSharper disable LocalizableElement
            this._textBoxFilePattern.Text = "*.*";
            // ReSharper restore LocalizableElement

            this._textBoxFolderPath.Text = initialFolder != null
                                               ? new DirectoryInfo(initialFolder).FullName
                                               : Environment.CurrentDirectory;

            this.InitializeGridColumnWidths();
        }

        void InitializeGridColumnWidths()
        {
            int baseWidth = this._dataGridViewResults.Width;

            this._columnFilePath.Width = (int)(baseWidth * 0.35);
            this._columnLineNumber.Width = (int)(baseWidth * 0.05);
            this._columnText.Width = (int)(baseWidth * 0.55);
        }

        void MainForm_Shown(object sender, EventArgs e)
        {
            this._textBoxText.Focus();
        }

        void ButtonBrowseFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = this._folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                this._textBoxFolderPath.Text = this._folderBrowserDialog.SelectedPath;
            }
        }

        void CheckBoxRegex_CheckedChanged(object sender, EventArgs e)
        {
            this.FocusTextPatternBox();
            this.ValidateTextPattern();
        }

        void CheckBoxIgnoreCase_CheckedChanged(object sender, EventArgs e)
        {
            this.FocusTextPatternBox();
            this.ValidateTextPattern();
        }

        void FocusTextPatternBox()
        {
            this._textBoxText.Focus();
            this._textBoxText.SelectionLength = 0;
            this._textBoxText.SelectionStart = this._textBoxText.Text.Length;
        }

        void TextBoxText_TextChanged(object sender, EventArgs e)
        {
            this.ValidateTextPattern();
        }

        void ValidateTextPattern()
        {
            try
            {
                if (this._checkBoxRegex.Checked)
                {
                    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                    Regex.IsMatch(string.Empty, this._textBoxText.Text);
                    // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                }

                this.ClearStatusMessage();
            }
            catch (Exception ex)
            {
                this.UpdateStatusMessage(ex.Message, true);
            }
        }

        void ClearStatusMessage()
        {
            this.UpdateStatusMessage(String.Empty, false);
        }

        void UpdateStatusMessage(string message, bool isError)
        {
            if (isError)
            {
                this._textBoxStatus.BackColor = Color.IndianRed;
                this._textBoxStatus.ForeColor = Color.White;
            }
            else
            {
                this._textBoxStatus.ResetBackColor();
                this._textBoxStatus.ResetForeColor();
            }

            this._textBoxStatus.Text = message;
        }

        void ButtonSearch_Click(object sender, EventArgs e)
        {
            this.DoSearch();
        }

        void Searcher_Completed(object sender, CompletedEventArgs e)
        {
            this.UnregisterSearcherEvents();

            this.DoCrossThreadUiAction(
                () =>
                {
                    string completedMessage = String.Format("Search completed. Elapsed: {0}", e.Duration);
                    this.UpdateStatusMessage(completedMessage, false);
                    this._buttonSearch.Enabled = true;
                });
        }

        void Searcher_MatchFound(object o, MatchFoundEventArgs args)
        {
            foreach (MatchedLine line in args.Matches)
            {
                MatchedLine localLine = line;
                string relativePath = args.FilePath.Substring(this._textBoxFolderPath.Text.Length + 1);
                this.DoCrossThreadUiAction(() => this._dataGridViewResults.Rows.Add(relativePath, localLine.Number, localLine.Text));
            }
        }

        void ButtonStop_Click(object sender, EventArgs e)
        {
            this.AbortSearch();
        }

        void DoSearch()
        {
            try
            {
                this.ClearStatusMessage();
                this._dataGridViewResults.Rows.Clear();

                var patternSpec = new PatternSpecification(
                    this._textBoxText.Text, this._checkBoxRegex.Checked,
                    this._checkBoxIgnoreCase.Checked);

                var fileSpec = new FileSpecification(
                    this._textBoxFolderPath.Text, this._checkBoxSearchSubfolders.Checked,
                    this._textBoxFilePattern.Text, new string[0]);

                var searcher = new FileSearcher(patternSpec.Expression, fileSpec.EnumerateFiles());
                searcher.MatchFound += this.Searcher_MatchFound;
                searcher.Completed += this.Searcher_Completed;

                this._buttonSearch.Enabled = false;
                this._searcher = searcher;
                searcher.Start();
            }
            catch (Exception ex)
            {
                this.UpdateStatusMessage(ex.Message, true);
            }
        }

        void AbortSearch()
        {
            if (this._searcher == null) return;

            this.UpdateStatusMessage("Cancelling...", false);

            // Explicitly using a high priority thread here
            new Thread(
                () =>
                {
                    try
                    {
                        this._searcher.Cancel();
                        this.UnregisterSearcherEvents();
                        this.DoCrossThreadUiAction(
                            () =>
                            {
                                this.UpdateStatusMessage("Search cancelled.", false);
                                this._buttonSearch.Enabled = true;
                            });
                    }
                    catch (Exception ex)
                    {
                        this.DoCrossThreadUiAction(() => this.UpdateStatusMessage(ex.Message, true));
                    }
                }) { Priority = ThreadPriority.AboveNormal }.Start();
        }

        void DoCrossThreadUiAction(Action uiAction)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(uiAction);
            }
            else
            {
                uiAction();
            }
        }

        void UnregisterSearcherEvents()
        {
            this._searcher.MatchFound -= this.Searcher_MatchFound;
            this._searcher.Completed -= this.Searcher_Completed;
        }
    }
}
