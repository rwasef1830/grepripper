namespace FastGrep.UI
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._labelFolderPath = new System.Windows.Forms.Label();
            this._textBoxFolderPath = new System.Windows.Forms.TextBox();
            this._checkBoxSearchSubfolders = new System.Windows.Forms.CheckBox();
            this._textBoxFilePattern = new System.Windows.Forms.TextBox();
            this._labelFilePattern = new System.Windows.Forms.Label();
            this._textBoxText = new System.Windows.Forms.TextBox();
            this._labelText = new System.Windows.Forms.Label();
            this._dataGridViewResults = new System.Windows.Forms.DataGridView();
            this._buttonBrowseFolder = new System.Windows.Forms.Button();
            this._buttonSearch = new System.Windows.Forms.Button();
            this._buttonStop = new System.Windows.Forms.Button();
            this._checkBoxIgnoreCase = new System.Windows.Forms.CheckBox();
            this._checkBoxRegex = new System.Windows.Forms.CheckBox();
            this._textBoxStatus = new System.Windows.Forms.TextBox();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._columnFilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._columnLineNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._columnText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewResults)).BeginInit();
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
            this._textBoxFolderPath.Location = new System.Drawing.Point(61, 3);
            this._textBoxFolderPath.Name = "_textBoxFolderPath";
            this._textBoxFolderPath.Size = new System.Drawing.Size(659, 22);
            this._textBoxFolderPath.TabIndex = 1;
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
            this._textBoxFilePattern.Location = new System.Drawing.Point(61, 25);
            this._textBoxFilePattern.Name = "_textBoxFilePattern";
            this._textBoxFilePattern.Size = new System.Drawing.Size(659, 22);
            this._textBoxFilePattern.TabIndex = 4;
            // 
            // _labelFilePattern
            // 
            this._labelFilePattern.AutoSize = true;
            this._labelFilePattern.Location = new System.Drawing.Point(4, 28);
            this._labelFilePattern.Name = "_labelFilePattern";
            this._labelFilePattern.Size = new System.Drawing.Size(28, 13);
            this._labelFilePattern.TabIndex = 3;
            this._labelFilePattern.Text = "File&s";
            // 
            // _textBoxText
            // 
            this._textBoxText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxText.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textBoxText.Location = new System.Drawing.Point(61, 49);
            this._textBoxText.Name = "_textBoxText";
            this._textBoxText.Size = new System.Drawing.Size(659, 22);
            this._textBoxText.TabIndex = 6;
            this._textBoxText.TextChanged += new System.EventHandler(this.TextBoxText_TextChanged);
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGridViewResults.DefaultCellStyle = dataGridViewCellStyle2;
            this._dataGridViewResults.GridColor = System.Drawing.SystemColors.ControlLight;
            this._dataGridViewResults.Location = new System.Drawing.Point(1, 100);
            this._dataGridViewResults.MultiSelect = false;
            this._dataGridViewResults.Name = "_dataGridViewResults";
            this._dataGridViewResults.ReadOnly = true;
            this._dataGridViewResults.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this._dataGridViewResults.RowHeadersVisible = false;
            this._dataGridViewResults.RowTemplate.Height = 16;
            this._dataGridViewResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridViewResults.Size = new System.Drawing.Size(882, 452);
            this._dataGridViewResults.TabIndex = 7;
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
            // _buttonSearch
            // 
            this._buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonSearch.Location = new System.Drawing.Point(754, 71);
            this._buttonSearch.Name = "_buttonSearch";
            this._buttonSearch.Size = new System.Drawing.Size(56, 23);
            this._buttonSearch.TabIndex = 10;
            this._buttonSearch.Text = "Go";
            this._buttonSearch.UseVisualStyleBackColor = true;
            this._buttonSearch.Click += new System.EventHandler(this.ButtonSearch_Click);
            // 
            // _buttonStop
            // 
            this._buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonStop.Location = new System.Drawing.Point(816, 71);
            this._buttonStop.Name = "_buttonStop";
            this._buttonStop.Size = new System.Drawing.Size(56, 23);
            this._buttonStop.TabIndex = 11;
            this._buttonStop.Text = "&Stop";
            this._buttonStop.UseVisualStyleBackColor = true;
            this._buttonStop.Click += new System.EventHandler(this.ButtonStop_Click);
            // 
            // _checkBoxIgnoreCase
            // 
            this._checkBoxIgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._checkBoxIgnoreCase.AutoSize = true;
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
            // _textBoxStatus
            // 
            this._textBoxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._textBoxStatus.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._textBoxStatus.Location = new System.Drawing.Point(7, 74);
            this._textBoxStatus.Name = "_textBoxStatus";
            this._textBoxStatus.ReadOnly = true;
            this._textBoxStatus.Size = new System.Drawing.Size(741, 20);
            this._textBoxStatus.TabIndex = 14;
            this._textBoxStatus.TabStop = false;
            this._textBoxStatus.WordWrap = false;
            // 
            // _folderBrowserDialog
            // 
            this._folderBrowserDialog.Description = "Choose directory to search";
            this._folderBrowserDialog.ShowNewFolderButton = false;
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this._columnLineNumber.DefaultCellStyle = dataGridViewCellStyle1;
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
            // MainForm
            // 
            this.AcceptButton = this._buttonSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(884, 552);
            this.Controls.Add(this._textBoxStatus);
            this.Controls.Add(this._checkBoxRegex);
            this.Controls.Add(this._checkBoxIgnoreCase);
            this.Controls.Add(this._buttonStop);
            this.Controls.Add(this._buttonSearch);
            this.Controls.Add(this._buttonBrowseFolder);
            this.Controls.Add(this._dataGridViewResults);
            this.Controls.Add(this._labelText);
            this.Controls.Add(this._textBoxText);
            this.Controls.Add(this._labelFilePattern);
            this.Controls.Add(this._textBoxFilePattern);
            this.Controls.Add(this._checkBoxSearchSubfolders);
            this.Controls.Add(this._textBoxFolderPath);
            this.Controls.Add(this._labelFolderPath);
            this.Name = "MainForm";
            this.Text = "FastGrep";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewResults)).EndInit();
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
        private System.Windows.Forms.Button _buttonSearch;
        private System.Windows.Forms.Button _buttonStop;
        private System.Windows.Forms.CheckBox _checkBoxIgnoreCase;
        private System.Windows.Forms.CheckBox _checkBoxRegex;
        private System.Windows.Forms.TextBox _textBoxStatus;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn _columnFilePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn _columnLineNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn _columnText;

    }
}

