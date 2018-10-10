namespace Gorba.Common.Medi.TestGui
{
    partial class FileSystemView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbxSource = new System.Windows.Forms.ComboBox();
            this.treeFileSystem = new System.Windows.Forms.TreeView();
            this.pgridInfo = new System.Windows.Forms.PropertyGrid();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.downloadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbxSource);
            this.splitContainer1.Panel1.Controls.Add(this.treeFileSystem);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pgridInfo);
            this.splitContainer1.Size = new System.Drawing.Size(570, 350);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.TabIndex = 0;
            // 
            // cbxSource
            // 
            this.cbxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSource.FormattingEnabled = true;
            this.cbxSource.Items.AddRange(new object[] {
            "Local",
            "Browse..."});
            this.cbxSource.Location = new System.Drawing.Point(3, 3);
            this.cbxSource.Name = "cbxSource";
            this.cbxSource.Size = new System.Drawing.Size(193, 21);
            this.cbxSource.TabIndex = 1;
            this.cbxSource.SelectedIndexChanged += new System.EventHandler(this.CbxSourceSelectedIndexChanged);
            // 
            // treeFileSystem
            // 
            this.treeFileSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeFileSystem.ContextMenuStrip = this.contextMenuStrip;
            this.treeFileSystem.Location = new System.Drawing.Point(0, 30);
            this.treeFileSystem.Name = "treeFileSystem";
            this.treeFileSystem.Size = new System.Drawing.Size(199, 320);
            this.treeFileSystem.TabIndex = 0;
            this.treeFileSystem.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeManagementBeforeExpand);
            this.treeFileSystem.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeManagementAfterSelect);
            // 
            // pgridInfo
            // 
            this.pgridInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgridInfo.Location = new System.Drawing.Point(0, 0);
            this.pgridInfo.Name = "pgridInfo";
            this.pgridInfo.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.pgridInfo.Size = new System.Drawing.Size(367, 350);
            this.pgridInfo.TabIndex = 0;
            this.pgridInfo.ToolbarVisible = false;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadFileToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(159, 48);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripOpening);
            // 
            // downloadFileToolStripMenuItem
            // 
            this.downloadFileToolStripMenuItem.Name = "downloadFileToolStripMenuItem";
            this.downloadFileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.downloadFileToolStripMenuItem.Text = "Download File...";
            this.downloadFileToolStripMenuItem.Click += new System.EventHandler(this.DownloadFileToolStripMenuItemClick);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "All files (*.*)|*.*";
            // 
            // FileSystemView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "FileSystemView";
            this.Size = new System.Drawing.Size(570, 350);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox cbxSource;
        private System.Windows.Forms.TreeView treeFileSystem;
        private System.Windows.Forms.PropertyGrid pgridInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem downloadFileToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}
