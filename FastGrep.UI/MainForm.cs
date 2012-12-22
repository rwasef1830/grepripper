using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        bool _isSearching;

        readonly Control[] _inputControls;

        public MainForm(string initialFolder)
        {
            this.InitializeComponent();
            this.InitializeDefaults(initialFolder);

            this._inputControls = new Control[]
            {
                this._textBoxFolderPath, this._textBoxFilePattern, this._textBoxText,
                this._checkBoxSearchSubfolders, this._checkBoxRegex, this._checkBoxIgnoreCase,
                this._buttonBrowseFolder
            };
        }

        void InitializeDefaults(string initialFolder)
        {
            this._textBoxFilePattern.Text = "*.*";

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

        void ButtonGo_Click(object sender, EventArgs e)
        {
            if (!this._isSearching)
            {
                if (this.HasAnyValidationError())
                {
                    return;
                }

                this.BeginSearch();
            }
            else
            {
                this.AbortSearch();
            }
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
                    this._buttonGo.Text = "&Go";
                    this._buttonGo.Enabled = true;

                    this._progressBarStatus.Style = ProgressBarStyle.Continuous;
                    this._progressBarStatus.Value = 0;

                    foreach (var control in this._inputControls)
                    {
                        control.Enabled = true;
                    }

                    this._isSearching = false;

                    if (this._dataGridViewResults.Rows.Count == 0)
                        this._textBoxText.Focus();
                    else
                        this._dataGridViewResults.Focus();
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

        void BeginSearch()
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
                    this._textBoxFilePattern.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                    new string[0]);

                foreach (var control in this._inputControls)
                {
                    control.Enabled = false;
                }

                this.ClearStatusMessage();
                this._dataGridViewResults.Rows.Clear();
                this._progressBarStatus.Style = ProgressBarStyle.Marquee;

                var searcher = new FileSearcher(patternSpec.Expression, fileSpec.EnumerateFiles());
                searcher.MatchFound += this.Searcher_MatchFound;
                searcher.ProgressChanged += this.Searcher_ProgressChanged;
                searcher.Completed += this.Searcher_Completed;

                this._searcher = searcher;
                searcher.Begin();

                this._buttonGo.Text = "&Stop";
                this._isSearching = true;

                this._buttonGo.Focus();
            }
            catch (Exception ex)
            {
                this.UpdateStatusMessage(ex.Message, true);
            }
        }

        void AbortSearch()
        {
            if (this._searcher == null) return;

            this._buttonGo.Enabled = false;
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

                                this._buttonGo.Text = "&Go";
                                this._isSearching = false;
                                this._buttonGo.Enabled = true;

                                this._progressBarStatus.Style = ProgressBarStyle.Continuous;
                                this._progressBarStatus.Value = 0;

                                foreach (var control in this._inputControls)
                                {
                                    control.Enabled = true;
                                }

                                if (this._dataGridViewResults.Rows.Count == 0)
                                    this._textBoxText.Focus();
                                else
                                    this._dataGridViewResults.Focus();
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
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                this._dataGridViewResults.CurrentCell = this._dataGridViewResults[e.ColumnIndex, e.RowIndex];

                this._contextMenuStripGridRow.Show(
                    (Control)sender,
                    this._dataGridViewResults.PointToClient(Cursor.Position));
            }
        }

        void DataGridViewResults_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                this._dataGridViewResults.CurrentCell = this._dataGridViewResults[e.ColumnIndex, e.RowIndex];

                var selectedMatchFullPath = this.GetSelectedMatchFullPath();

                this._dataGridViewResults.DoDragDrop(
                    new DataObject(DataFormats.FileDrop, new[] { selectedMatchFullPath }),
                    DragDropEffects.All);
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

        bool HasAnyValidationError()
        {
            return
                new[] { this._textBoxFolderPath, this._textBoxFilePattern, this._textBoxText }
                    .Any(x => !String.IsNullOrWhiteSpace(this._errorProvider.GetError(x)));
        }

        void TextBoxFolderPath_Validating(object sender, CancelEventArgs e)
        {
            this.DoTextValidation(
                sender,
                t =>
                {
                    try
                    {
                        if (String.IsNullOrWhiteSpace(t))
                            return "Type or browse for a path to search";

                        if (!Directory.Exists(t))
                            throw new DirectoryNotFoundException();
                    }
                    catch (Exception ex)
                    {
                        return "Invalid path: " + ex.Message;
                    }

                    return null;
                });
        }

        void TextBoxText_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateTextPattern();
        }

        void ValidateTextPattern()
        {
            this.DoTextValidation(
                this._textBoxText,
                t =>
                {
                    try
                    {
                        if (String.IsNullOrWhiteSpace(t))
                        {
                            return "Enter a search pattern";
                        }

                        if (this._checkBoxRegex.Checked)
                        {
                            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                            Regex.IsMatch(string.Empty, t);
                            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                        }

                        return null;
                    }
                    catch (Exception ex)
                    {
                        return "Invalid regex: " + ex.Message;
                    }
                });
        }

        void TextBoxFilePattern_Validating(object sender, CancelEventArgs e)
        {
            this.DoTextValidation(
                sender,
                t =>
                {
                    var failedPatterns =
                        t.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         .Where(x => !GlobExpression.IsValid(x))
                         .ToList();

                    if (failedPatterns.Any())
                    {
                        return "Invalid pattern: " + String.Join(", ", failedPatterns);
                    }

                    return null;
                });
        }

        void DoTextValidation(
            object validatingEventHandlerSenderControl, 
            Func<string, string> validationFunc)
        {
            if (!(validatingEventHandlerSenderControl is TextBoxBase))
                throw new ArgumentException(
                    "object must be an instance of " + typeof(TextBoxBase).Name,
                    "validatingEventHandlerSenderControl");

            if (validationFunc == null) throw new ArgumentNullException("validationFunc");

            var control = (TextBoxBase)validatingEventHandlerSenderControl;
            var errorMessage = validationFunc(control.Text);

            if (String.IsNullOrWhiteSpace(errorMessage))
            {
                this._errorProvider.SetError(control, String.Empty);

                control.BackColor = SystemColors.Window;
                control.ForeColor = SystemColors.WindowText;
            }
            else
            {
                this._errorProvider.SetError(control, errorMessage);

                control.BackColor = Color.MistyRose;
                control.ForeColor = Color.Black;
            }
        }
    }
}
