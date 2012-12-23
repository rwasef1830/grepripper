using System;

namespace FunkyGrep.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this._labelFolderPath = new System.Windows.Forms.Label();
            this._textBoxFolderPath = new System.Windows.Forms.TextBox();
            this._checkBoxSearchSubfolders = new System.Windows.Forms.CheckBox();
            this._textBoxFilePattern = new System.Windows.Forms.TextBox();
            this._labelFilePattern = new System.Windows.Forms.Label();
            this._textBoxText = new System.Windows.Forms.TextBox();
            this._labelText = new System.Windows.Forms.Label();
            this._dataGridViewResults = new System.Windows.Forms.DataGridView();
            this._columnFilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._columnLineNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._columnText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._buttonBrowseFolder = new System.Windows.Forms.Button();
            this._buttonGo = new System.Windows.Forms.Button();
            this._checkBoxIgnoreCase = new System.Windows.Forms.CheckBox();
            this._checkBoxRegex = new System.Windows.Forms.CheckBox();
            this._progressBarStatus = new System.Windows.Forms.ProgressBar();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._labelStatus = new System.Windows.Forms.Label();
            this._contextMenuStripGridRow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItemRelativePath = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItemAbsPath = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItemLineNum = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItemNotepad = new System.Windows.Forms.ToolStripMenuItem();
            this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewResults)).BeginInit();
            this._contextMenuStripGridRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // _labelFolderPath
            // 
            this._labelFolderPath.AutoSize = true;
            this._labelFolderPath.Location = new System.Drawing.Point(4, 6);
            this._labelFolderPath.Name = "_labelFolderPath";
            this._labelFolderPath.Size = new System.Drawing.Size(49, 13);
            this._labelFolderPath.TabIndex = 0;
            this._labelFolderPath.Text = "&Directory";
            // 
            // _textBoxFolderPath
            // 
            this._textBoxFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxFolderPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._textBoxFolderPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this._textBoxFolderPath.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._errorProvider.SetIconPadding(this._textBoxFolderPath, -18);
            this._textBoxFolderPath.Location = new System.Drawing.Point(61, 3);
            this._textBoxFolderPath.Name = "_textBoxFolderPath";
            this._textBoxFolderPath.Size = new System.Drawing.Size(659, 22);
            this._textBoxFolderPath.TabIndex = 1;
            this._textBoxFolderPath.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxFolderPath_Validating);
            // 
            // _checkBoxSearchSubfolders
            // 
            this._checkBoxSearchSubfolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxSearchSubfolders.AutoSize = true;
            this._checkBoxSearchSubfolders.Checked = true;
            this._checkBoxSearchSubfolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxSearchSubfolders.Location = new System.Drawing.Point(772, 5);
            this._checkBoxSearchSubfolders.Name = "_checkBoxSearchSubfolders";
            this._checkBoxSearchSubfolders.Size = new System.Drawing.Size(99, 17);
            this._checkBoxSearchSubfolders.TabIndex = 2;
            this._checkBoxSearchSubfolders.Text = "&Include Subdirs";
            this._checkBoxSearchSubfolders.UseVisualStyleBackColor = true;
            // 
            // _textBoxFilePattern
            // 
            this._textBoxFilePattern.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxFilePattern.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this._textBoxFilePattern.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._errorProvider.SetIconPadding(this._textBoxFilePattern, -18);
            this._textBoxFilePattern.Location = new System.Drawing.Point(61, 25);
            this._textBoxFilePattern.Name = "_textBoxFilePattern";
            this._textBoxFilePattern.Size = new System.Drawing.Size(659, 22);
            this._textBoxFilePattern.TabIndex = 4;
            this._textBoxFilePattern.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxFilePattern_Validating);
            // 
            // _labelFilePattern
            // 
            this._labelFilePattern.AutoSize = true;
            this._labelFilePattern.Location = new System.Drawing.Point(4, 28);
            this._labelFilePattern.Name = "_labelFilePattern";
            this._labelFilePattern.Size = new System.Drawing.Size(28, 13);
            this._labelFilePattern.TabIndex = 3;
            this._labelFilePattern.Text = "&Files";
            // 
            // _textBoxText
            // 
            this._textBoxText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxText.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._errorProvider.SetIconPadding(this._textBoxText, -18);
            this._textBoxText.Location = new System.Drawing.Point(61, 49);
            this._textBoxText.Name = "_textBoxText";
            this._textBoxText.Size = new System.Drawing.Size(659, 22);
            this._textBoxText.TabIndex = 6;
            this._textBoxText.TextChanged += new System.EventHandler(this.TextBoxText_TextChanged);
            this._textBoxText.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxText_Validating);
            // 
            // _labelText
            // 
            this._labelText.AutoSize = true;
            this._labelText.Location = new System.Drawing.Point(4, 52);
            this._labelText.Name = "_labelText";
            this._labelText.Size = new System.Drawing.Size(28, 13);
            this._labelText.TabIndex = 5;
            this._labelText.Text = "&Text";
            // 
            // _dataGridViewResults
            // 
            this._dataGridViewResults.AllowUserToAddRows = false;
            this._dataGridViewResults.AllowUserToDeleteRows = false;
            this._dataGridViewResults.AllowUserToResizeRows = false;
            this._dataGridViewResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dataGridViewResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._dataGridViewResults.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this._dataGridViewResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridViewResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._columnFilePath,
            this._columnLineNumber,
            this._columnText});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGridViewResults.DefaultCellStyle = dataGridViewCellStyle4;
            this._dataGridViewResults.GridColor = System.Drawing.SystemColors.ControlLight;
            this._dataGridViewResults.Location = new System.Drawing.Point(1, 102);
            this._dataGridViewResults.MultiSelect = false;
            this._dataGridViewResults.Name = "_dataGridViewResults";
            this._dataGridViewResults.ReadOnly = true;
            this._dataGridViewResults.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this._dataGridViewResults.RowHeadersVisible = false;
            this._dataGridViewResults.RowTemplate.Height = 16;
            this._dataGridViewResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridViewResults.Size = new System.Drawing.Size(882, 360);
            this._dataGridViewResults.TabIndex = 7;
            this._dataGridViewResults.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewResults_CellMouseClick);
            this._dataGridViewResults.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewResults_CellMouseDown);
            // 
            // _columnFilePath
            // 
            this._columnFilePath.FillWeight = 80F;
            this._columnFilePath.HeaderText = "File";
            this._columnFilePath.Name = "_columnFilePath";
            this._columnFilePath.ReadOnly = true;
            this._columnFilePath.Width = 48;
            // 
            // _columnLineNumber
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this._columnLineNumber.DefaultCellStyle = dataGridViewCellStyle3;
            this._columnLineNumber.HeaderText = "Line";
            this._columnLineNumber.Name = "_columnLineNumber";
            this._columnLineNumber.ReadOnly = true;
            // 
            // _columnText
            // 
            this._columnText.HeaderText = "Text";
            this._columnText.Name = "_columnText";
            this._columnText.ReadOnly = true;
            this._columnText.Width = 53;
            // 
            // _buttonBrowseFolder
            // 
            this._buttonBrowseFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonBrowseFolder.Location = new System.Drawing.Point(725, 1);
            this._buttonBrowseFolder.Name = "_buttonBrowseFolder";
            this._buttonBrowseFolder.Size = new System.Drawing.Size(27, 23);
            this._buttonBrowseFolder.TabIndex = 9;
            this._buttonBrowseFolder.Text = "...";
            this._buttonBrowseFolder.UseVisualStyleBackColor = true;
            this._buttonBrowseFolder.Click += new System.EventHandler(this.ButtonBrowseFolder_Click);
            // 
            // _buttonGo
            // 
            this._buttonGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonGo.Location = new System.Drawing.Point(824, 73);
            this._buttonGo.Name = "_buttonGo";
            this._buttonGo.Size = new System.Drawing.Size(48, 23);
            this._buttonGo.TabIndex = 10;
            this._buttonGo.Text = "Go";
            this._buttonGo.UseVisualStyleBackColor = true;
            this._buttonGo.Click += new System.EventHandler(this.ButtonGo_Click);
            // 
            // _checkBoxIgnoreCase
            // 
            this._checkBoxIgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxIgnoreCase.AutoSize = true;
            this._checkBoxIgnoreCase.Checked = true;
            this._checkBoxIgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxIgnoreCase.Location = new System.Drawing.Point(788, 51);
            this._checkBoxIgnoreCase.Name = "_checkBoxIgnoreCase";
            this._checkBoxIgnoreCase.Size = new System.Drawing.Size(83, 17);
            this._checkBoxIgnoreCase.TabIndex = 12;
            this._checkBoxIgnoreCase.Text = "Ignore &Case";
            this._checkBoxIgnoreCase.UseVisualStyleBackColor = true;
            this._checkBoxIgnoreCase.CheckedChanged += new System.EventHandler(this.CheckBoxIgnoreCase_CheckedChanged);
            // 
            // _checkBoxRegex
            // 
            this._checkBoxRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxRegex.AutoSize = true;
            this._checkBoxRegex.Checked = true;
            this._checkBoxRegex.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxRegex.Location = new System.Drawing.Point(725, 51);
            this._checkBoxRegex.Name = "_checkBoxRegex";
            this._checkBoxRegex.Size = new System.Drawing.Size(57, 17);
            this._checkBoxRegex.TabIndex = 13;
            this._checkBoxRegex.Text = "&Regex";
            this._checkBoxRegex.UseVisualStyleBackColor = true;
            this._checkBoxRegex.CheckedChanged += new System.EventHandler(this.CheckBoxRegex_CheckedChanged);
            // 
            // _progressBarStatus
            // 
            this._progressBarStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressBarStatus.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._progressBarStatus.Location = new System.Drawing.Point(7, 74);
            this._progressBarStatus.Name = "_progressBarStatus";
            this._progressBarStatus.Size = new System.Drawing.Size(811, 10);
            this._progressBarStatus.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._progressBarStatus.TabIndex = 14;
            this._progressBarStatus.TabStop = false;
            // 
            // _folderBrowserDialog
            // 
            this._folderBrowserDialog.Description = "Choose directory to search";
            this._folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // _labelStatus
            // 
            this._labelStatus.AutoSize = true;
            this._labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelStatus.Location = new System.Drawing.Point(4, 86);
            this._labelStatus.Name = "_labelStatus";
            this._labelStatus.Size = new System.Drawing.Size(100, 13);
            this._labelStatus.TabIndex = 16;
            this._labelStatus.Text = "Search not running.";
            // 
            // _contextMenuStripGridRow
            // 
            this._contextMenuStripGridRow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMenuItemCopy,
            this._toolStripMenuItemOpen});
            this._contextMenuStripGridRow.Name = "contextMenuStrip1";
            this._contextMenuStripGridRow.Size = new System.Drawing.Size(104, 48);
            // 
            // _toolStripMenuItemCopy
            // 
            this._toolStripMenuItemCopy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMenuItemRelativePath,
            this._toolStripMenuItemAbsPath,
            this._toolStripMenuItemFile,
            this._toolStripMenuItemLineNum});
            this._toolStripMenuItemCopy.Name = "_toolStripMenuItemCopy";
            this._toolStripMenuItemCopy.Size = new System.Drawing.Size(103, 22);
            this._toolStripMenuItemCopy.Text = "Copy";
            // 
            // _toolStripMenuItemRelativePath
            // 
            this._toolStripMenuItemRelativePath.Name = "_toolStripMenuItemRelativePath";
            this._toolStripMenuItemRelativePath.Size = new System.Drawing.Size(148, 22);
            this._toolStripMenuItemRelativePath.Text = "Relative path";
            this._toolStripMenuItemRelativePath.Click += new System.EventHandler(this.ToolStripMenuItemRelativePath_Click);
            // 
            // _toolStripMenuItemAbsPath
            // 
            this._toolStripMenuItemAbsPath.Name = "_toolStripMenuItemAbsPath";
            this._toolStripMenuItemAbsPath.Size = new System.Drawing.Size(148, 22);
            this._toolStripMenuItemAbsPath.Text = "Absolute path";
            this._toolStripMenuItemAbsPath.Click += new System.EventHandler(this.ToolStripMenuItemAbsPath_Click);
            // 
            // _toolStripMenuItemFile
            // 
            this._toolStripMenuItemFile.Name = "_toolStripMenuItemFile";
            this._toolStripMenuItemFile.Size = new System.Drawing.Size(148, 22);
            this._toolStripMenuItemFile.Text = "File";
            this._toolStripMenuItemFile.Click += new System.EventHandler(this.ToolStripMenuItemFile_Click);
            // 
            // _toolStripMenuItemLineNum
            // 
            this._toolStripMenuItemLineNum.Name = "_toolStripMenuItemLineNum";
            this._toolStripMenuItemLineNum.Size = new System.Drawing.Size(148, 22);
            this._toolStripMenuItemLineNum.Text = "Line number";
            this._toolStripMenuItemLineNum.Click += new System.EventHandler(this.ToolStripMenuItemLineNumber_Click);
            // 
            // _toolStripMenuItemOpen
            // 
            this._toolStripMenuItemOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMenuItemNotepad});
            this._toolStripMenuItemOpen.Name = "_toolStripMenuItemOpen";
            this._toolStripMenuItemOpen.Size = new System.Drawing.Size(103, 22);
            this._toolStripMenuItemOpen.Text = "Open";
            this._toolStripMenuItemOpen.Click += new System.EventHandler(this.ToolStripMenuItemNotepad_Click);
            // 
            // _toolStripMenuItemNotepad
            // 
            this._toolStripMenuItemNotepad.Name = "_toolStripMenuItemNotepad";
            this._toolStripMenuItemNotepad.Size = new System.Drawing.Size(120, 22);
            this._toolStripMenuItemNotepad.Text = "Notepad";
            this._toolStripMenuItemNotepad.Click += new System.EventHandler(this.ToolStripMenuItemNotepad_Click);
            // 
            // _errorProvider
            // 
            this._errorProvider.ContainerControl = this;
            // 
            // MainForm
            // 
            this.AcceptButton = this._buttonGo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(884, 462);
            this.Controls.Add(this._labelStatus);
            this.Controls.Add(this._progressBarStatus);
            this.Controls.Add(this._checkBoxRegex);
            this.Controls.Add(this._checkBoxIgnoreCase);
            this.Controls.Add(this._buttonGo);
            this.Controls.Add(this._buttonBrowseFolder);
            this.Controls.Add(this._dataGridViewResults);
            this.Controls.Add(this._labelText);
            this.Controls.Add(this._textBoxText);
            this.Controls.Add(this._labelFilePattern);
            this.Controls.Add(this._textBoxFilePattern);
            this.Controls.Add(this._checkBoxSearchSubfolders);
            this.Controls.Add(this._textBoxFolderPath);
            this.Controls.Add(this._labelFolderPath);
            this.MinimumSize = new System.Drawing.Size(450, 250);
            this.Name = "MainForm";
            this.Text = "FunkyGrep";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewResults)).EndInit();
            this._contextMenuStripGridRow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _labelFolderPath;
        private System.Windows.Forms.TextBox _textBoxFolderPath;
        private System.Windows.Forms.CheckBox _checkBoxSearchSubfolders;
        private System.Windows.Forms.TextBox _textBoxFilePattern;
        private System.Windows.Forms.Label _labelFilePattern;
        private System.Windows.Forms.TextBox _textBoxText;
        private System.Windows.Forms.Label _labelText;
        private System.Windows.Forms.DataGridView _dataGridViewResults;
        private System.Windows.Forms.Button _buttonBrowseFolder;
        private System.Windows.Forms.Button _buttonGo;
        private System.Windows.Forms.CheckBox _checkBoxIgnoreCase;
        private System.Windows.Forms.CheckBox _checkBoxRegex;
        private System.Windows.Forms.ProgressBar _progressBarStatus;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn _columnFilePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn _columnLineNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn _columnText;
        private System.Windows.Forms.Label _labelStatus;
        private System.Windows.Forms.ContextMenuStrip _contextMenuStripGridRow;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemOpen;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemCopy;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemRelativePath;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemAbsPath;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemLineNum;
        private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemNotepad;
        private System.Windows.Forms.ErrorProvider _errorProvider;

    }
}

