using System;
using System.Collections.Specialized;
using System.Diagnostics;
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
                this._labelStatus.BackColor = Color.IndianRed;
                this._labelStatus.ForeColor = Color.White;
            }
            else
            {
                this._labelStatus.ResetBackColor();
                this._labelStatus.ResetForeColor();
            }

            this._labelStatus.Text = message;
        }

        void ButtonSearch_Click(object sender, EventArgs e)
        {
            this.DoSearch();
        }

        void Searcher_ProgressChanged(object sender, ProgressEventArgs e)
        {
            this.DoCrossThreadUiAction(
                () =>
                {
                    if (e.TotalNumberOfFiles > 0)
                    {
                        this._labelStatus.Text = String.Format(
                            "{0:n0} of {1:n0} done. {2:n0} failed.",
                            e.NumberOfSearchedFiles,
                            e.TotalNumberOfFiles,
                            e.NumberOfFailedFiles);

                        var percent = (int)Math.Min(100, e.NumberOfSearchedFiles * 100 / e.TotalNumberOfFiles);

                        this._progressBarStatus.Style = ProgressBarStyle.Continuous;
                        this._progressBarStatus.Value = percent;
                    }
                    else
                    {
                        this._labelStatus.Text = String.Format(
                            "{0:n0} done. {1:n0} failed.",
                            e.NumberOfSearchedFiles,
                            e.NumberOfFailedFiles);

                        this._progressBarStatus.Style = ProgressBarStyle.Marquee;
                    }
                });
        }

        void Searcher_Completed(object sender, CompletedEventArgs e)
        {
            this.UnregisterSearcherEvents();

            this.DoCrossThreadUiAction(
                () =>
                {
                    string completedMessage = String.Format("Last search duration: {0}", e.Duration);
                    this.UpdateStatusMessage(completedMessage, false);
                    this._buttonSearch.Enabled = true;

                    this._progressBarStatus.Style = ProgressBarStyle.Continuous;
                    this._progressBarStatus.Value = 0;
                });
        }

        void Searcher_MatchFound(object o, MatchFoundEventArgs args)
        {
            string parentPath = this._textBoxFolderPath.Text;
            int basenameLength = parentPath.Length;

            if (!parentPath.EndsWith(Path.DirectorySeparatorChar.ToString(Thread.CurrentThread.CurrentCulture)))
            {
                basenameLength++;
            }

            foreach (MatchedLine line in args.Matches)
            {
                MatchedLine localLine = line;
                string relativePath = args.FilePath.Substring(basenameLength);

                this.DoCrossThreadUiAction(
                    () => this._dataGridViewResults.Rows.Add(relativePath, localLine.Number, localLine.Text));
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
                var patternSpec = new PatternSpecification(
                    this._textBoxText.Text,
                    this._checkBoxRegex.Checked,
                    this._checkBoxIgnoreCase.Checked);

                var fileSpec = new FileSpecification(
                    this._textBoxFolderPath.Text,
                    this._checkBoxSearchSubfolders.Checked,
                    this._textBoxFilePattern.Text,
                    new string[0]);

                this.ClearStatusMessage();
                this._dataGridViewResults.Rows.Clear();
                this._progressBarStatus.Style = ProgressBarStyle.Marquee;

                var searcher = new FileSearcher(patternSpec.Expression, fileSpec.EnumerateFiles());
                searcher.MatchFound += this.Searcher_MatchFound;
                searcher.ProgressChanged += this.Searcher_ProgressChanged;
                searcher.Completed += this.Searcher_Completed;

                this._buttonSearch.Enabled = false;
                this._searcher = searcher;
                searcher.Begin();
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

                                this._progressBarStatus.Style = ProgressBarStyle.Continuous;
                                this._progressBarStatus.Value = 0;
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
            this._searcher.ProgressChanged -= this.Searcher_ProgressChanged;
            this._searcher.Completed -= this.Searcher_Completed;
        }

        void DataGridViewResults_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this._dataGridViewResults.CurrentCell = this._dataGridViewResults[e.ColumnIndex, e.RowIndex];

                this._contextMenuStripGridRow.Show(
                    (Control)sender,
                    this._dataGridViewResults.PointToClient(Cursor.Position));
            }
        }

        void ToolStripMenuItemRelativePath_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this._dataGridViewResults.CurrentCell.OwningRow.Cells[0].Value.ToString());
        }

        void ToolStripMenuItemAbsPath_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.GetSelectedMatchFullPath());
        }

        string GetSelectedMatchFullPath()
        {
            return Path.Combine(
                this._textBoxFolderPath.Text,
                this._dataGridViewResults.CurrentCell.OwningRow.Cells[0].Value.ToString());
        }

        void ToolStripMenuItemFile_Click(object sender, EventArgs e)
        {
            var filePath = this.GetSelectedMatchFullPath();
            var collection = new StringCollection { filePath };
            Clipboard.SetFileDropList(collection);
        }

        void ToolStripMenuItemLineNumber_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this._dataGridViewResults.CurrentCell.OwningRow.Cells[1].Value.ToString());
        }

        void ToolStripMenuItemNotepad_Click(object sender, EventArgs e)
        {
            var pi = new ProcessStartInfo("notepad.exe", "\"" + this.GetSelectedMatchFullPath() + "\"")
                     {
                         UseShellExecute = true
                     };

            Process.Start(pi);
        }
    }
}
