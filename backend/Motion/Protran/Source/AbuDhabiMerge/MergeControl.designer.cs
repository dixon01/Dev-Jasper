namespace Gorba.Motion.Protran.AbuDhabiMerge
{
    partial class MergeControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.MergedFileTreeView = new System.Windows.Forms.TreeView();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ClearMergeStatusButton = new System.Windows.Forms.Button();
            this.mergeStatusTextbox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.MergePathTextbox = new System.Windows.Forms.TextBox();
            this.MergeButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FilesInFolderTreeView = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonSoftware = new System.Windows.Forms.RadioButton();
            this.radioButtonData = new System.Windows.Forms.RadioButton();
            this.SelectedFilesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.labelSelectBrowse = new System.Windows.Forms.Label();
            this.FileCountTextbox = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.labelCount = new System.Windows.Forms.Label();
            this.labelClear = new System.Windows.Forms.Label();
            this.ClearFilesButton = new System.Windows.Forms.Button();
            this.SelectFilesOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.MergeFilesSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SelectFoldersOpenFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.MergedFileTreeView);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.ClearMergeStatusButton);
            this.groupBox2.Controls.Add(this.mergeStatusTextbox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.ProgressBar);
            this.groupBox2.Controls.Add(this.MergePathTextbox);
            this.groupBox2.Controls.Add(this.MergeButton);
            this.groupBox2.Location = new System.Drawing.Point(534, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(438, 686);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Merge ";
            // 
            // MergedFileTreeView
            // 
            this.MergedFileTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MergedFileTreeView.Location = new System.Drawing.Point(9, 80);
            this.MergedFileTreeView.Name = "MergedFileTreeView";
            this.MergedFileTreeView.Size = new System.Drawing.Size(423, 222);
            this.MergedFileTreeView.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 305);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Merge Status:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Tree view of Destination Merge File:";
            // 
            // ClearMergeStatusButton
            // 
            this.ClearMergeStatusButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearMergeStatusButton.Location = new System.Drawing.Point(357, 657);
            this.ClearMergeStatusButton.Name = "ClearMergeStatusButton";
            this.ClearMergeStatusButton.Size = new System.Drawing.Size(75, 23);
            this.ClearMergeStatusButton.TabIndex = 2;
            this.ClearMergeStatusButton.Text = "Clear";
            this.ClearMergeStatusButton.UseVisualStyleBackColor = true;
            this.ClearMergeStatusButton.Click += new System.EventHandler(this.ClearMergeStatusButtonClick);
            // 
            // mergeStatusTextbox
            // 
            this.mergeStatusTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mergeStatusTextbox.Location = new System.Drawing.Point(9, 361);
            this.mergeStatusTextbox.Multiline = true;
            this.mergeStatusTextbox.Name = "mergeStatusTextbox";
            this.mergeStatusTextbox.ReadOnly = true;
            this.mergeStatusTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mergeStatusTextbox.Size = new System.Drawing.Size(423, 290);
            this.mergeStatusTextbox.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(155, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Select Filename for Merged file:";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(6, 321);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(426, 23);
            this.ProgressBar.Step = 1;
            this.ProgressBar.TabIndex = 0;
            // 
            // MergePathTextbox
            // 
            this.MergePathTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MergePathTextbox.Location = new System.Drawing.Point(6, 35);
            this.MergePathTextbox.Name = "MergePathTextbox";
            this.MergePathTextbox.ReadOnly = true;
            this.MergePathTextbox.Size = new System.Drawing.Size(345, 20);
            this.MergePathTextbox.TabIndex = 10;
            // 
            // MergeButton
            // 
            this.MergeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MergeButton.Location = new System.Drawing.Point(357, 32);
            this.MergeButton.Name = "MergeButton";
            this.MergeButton.Size = new System.Drawing.Size(75, 23);
            this.MergeButton.TabIndex = 9;
            this.MergeButton.Text = "Merge";
            this.MergeButton.UseVisualStyleBackColor = true;
            this.MergeButton.Click += new System.EventHandler(this.MergeButtonClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.FilesInFolderTreeView);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.SelectedFilesCheckedListBox);
            this.groupBox1.Controls.Add(this.labelSelectBrowse);
            this.groupBox1.Controls.Add(this.FileCountTextbox);
            this.groupBox1.Controls.Add(this.BrowseButton);
            this.groupBox1.Controls.Add(this.labelCount);
            this.groupBox1.Controls.Add(this.labelClear);
            this.groupBox1.Controls.Add(this.ClearFilesButton);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(525, 686);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // FilesInFolderTreeView
            // 
            this.FilesInFolderTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.FilesInFolderTreeView.CheckBoxes = true;
            this.FilesInFolderTreeView.Location = new System.Drawing.Point(252, 51);
            this.FilesInFolderTreeView.Name = "FilesInFolderTreeView";
            this.FilesInFolderTreeView.Size = new System.Drawing.Size(267, 484);
            this.FilesInFolderTreeView.TabIndex = 15;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.radioButtonSoftware);
            this.groupBox3.Controls.Add(this.radioButtonData);
            this.groupBox3.Location = new System.Drawing.Point(18, 582);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(311, 88);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select Type of Merged File (Extension)";
            // 
            // radioButtonSoftware
            // 
            this.radioButtonSoftware.AutoSize = true;
            this.radioButtonSoftware.Location = new System.Drawing.Point(18, 52);
            this.radioButtonSoftware.Name = "radioButtonSoftware";
            this.radioButtonSoftware.Size = new System.Drawing.Size(67, 17);
            this.radioButtonSoftware.TabIndex = 1;
            this.radioButtonSoftware.Text = "Software";
            this.radioButtonSoftware.UseVisualStyleBackColor = true;
            this.radioButtonSoftware.CheckedChanged += new System.EventHandler(this.RadioButtonSoftwareCheckedChanged);
            // 
            // radioButtonData
            // 
            this.radioButtonData.AutoSize = true;
            this.radioButtonData.Checked = true;
            this.radioButtonData.Location = new System.Drawing.Point(18, 20);
            this.radioButtonData.Name = "radioButtonData";
            this.radioButtonData.Size = new System.Drawing.Size(48, 17);
            this.radioButtonData.TabIndex = 0;
            this.radioButtonData.TabStop = true;
            this.radioButtonData.Text = "Data";
            this.radioButtonData.UseVisualStyleBackColor = true;
            this.radioButtonData.CheckedChanged += new System.EventHandler(this.RadioButtonDataCheckedChanged);
            // 
            // SelectedFilesCheckedListBox
            // 
            this.SelectedFilesCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SelectedFilesCheckedListBox.CheckOnClick = true;
            this.SelectedFilesCheckedListBox.FormattingEnabled = true;
            this.SelectedFilesCheckedListBox.HorizontalScrollbar = true;
            this.SelectedFilesCheckedListBox.Location = new System.Drawing.Point(18, 51);
            this.SelectedFilesCheckedListBox.Name = "SelectedFilesCheckedListBox";
            this.SelectedFilesCheckedListBox.ScrollAlwaysVisible = true;
            this.SelectedFilesCheckedListBox.Size = new System.Drawing.Size(227, 484);
            this.SelectedFilesCheckedListBox.TabIndex = 12;
            // 
            // labelSelectBrowse
            // 
            this.labelSelectBrowse.AutoSize = true;
            this.labelSelectBrowse.Location = new System.Drawing.Point(15, 19);
            this.labelSelectBrowse.Name = "labelSelectBrowse";
            this.labelSelectBrowse.Size = new System.Drawing.Size(122, 13);
            this.labelSelectBrowse.TabIndex = 0;
            this.labelSelectBrowse.Text = "Select Folders to Merge:";
            // 
            // FileCountTextbox
            // 
            this.FileCountTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FileCountTextbox.Location = new System.Drawing.Point(152, 545);
            this.FileCountTextbox.Name = "FileCountTextbox";
            this.FileCountTextbox.ReadOnly = true;
            this.FileCountTextbox.Size = new System.Drawing.Size(55, 20);
            this.FileCountTextbox.TabIndex = 6;
            this.FileCountTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(444, 19);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 2;
            this.BrowseButton.Text = "Browse...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(15, 548);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(136, 13);
            this.labelCount.TabIndex = 5;
            this.labelCount.Text = "Number of folders selected:";
            // 
            // labelClear
            // 
            this.labelClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelClear.AutoSize = true;
            this.labelClear.Location = new System.Drawing.Point(289, 547);
            this.labelClear.Name = "labelClear";
            this.labelClear.Size = new System.Drawing.Size(124, 13);
            this.labelClear.TabIndex = 3;
            this.labelClear.Text = "Clear all selected folders:";
            // 
            // ClearFilesButton
            // 
            this.ClearFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ClearFilesButton.Location = new System.Drawing.Point(417, 542);
            this.ClearFilesButton.Name = "ClearFilesButton";
            this.ClearFilesButton.Size = new System.Drawing.Size(75, 23);
            this.ClearFilesButton.TabIndex = 4;
            this.ClearFilesButton.Text = "Clear";
            this.ClearFilesButton.UseVisualStyleBackColor = true;
            this.ClearFilesButton.Click += new System.EventHandler(this.ClearFilesButtonClick);
            // 
            // SelectFilesOpenFileDialog
            // 
            this.SelectFilesOpenFileDialog.Multiselect = true;
            // 
            // SelectFoldersOpenFileDialog
            // 
            this.SelectFoldersOpenFileDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.SelectFoldersOpenFileDialog.SelectedPath = "D:\\";
            this.SelectFoldersOpenFileDialog.ShowNewFolderButton = false;
            // 
            // MergeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MergeControl";
            this.Size = new System.Drawing.Size(975, 692);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ClearMergeStatusButton;

        private System.Windows.Forms.TextBox mergeStatusTextbox;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;

        public System.Windows.Forms.TextBox MergePathTextbox;
        private System.Windows.Forms.Button MergeButton;

        public System.Windows.Forms.Label labelSelectBrowse;
        private System.Windows.Forms.TextBox FileCountTextbox;
        private System.Windows.Forms.Button BrowseButton;

        public System.Windows.Forms.Label labelCount;

        public System.Windows.Forms.Label labelClear;
        private System.Windows.Forms.Button ClearFilesButton;

        protected System.Windows.Forms.OpenFileDialog SelectFilesOpenFileDialog;
        private System.Windows.Forms.SaveFileDialog MergeFilesSaveFileDialog;

        public System.Windows.Forms.CheckedListBox SelectedFilesCheckedListBox;
        private System.Windows.Forms.GroupBox groupBox3;

        public System.Windows.Forms.RadioButton radioButtonSoftware;

        public System.Windows.Forms.RadioButton radioButtonData;
        private System.Windows.Forms.FolderBrowserDialog SelectFoldersOpenFileDialog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;

        public System.Windows.Forms.TreeView FilesInFolderTreeView;

        public System.Windows.Forms.TreeView MergedFileTreeView;
    }
}
