namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    partial class UpdateFolderStructureControl
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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.foldersTreeControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.FoldersTreeControl();
            this.fileExplorerListView = new Gorba.Motion.Update.UsbUpdateManager.Controls.FileExplorerListView();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.foldersTreeControl);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.fileExplorerListView);
            this.splitContainer2.Size = new System.Drawing.Size(798, 507);
            this.splitContainer2.SplitterDistance = 179;
            this.splitContainer2.TabIndex = 2;
            // 
            // foldersTreeControl
            // 
            this.foldersTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foldersTreeControl.Location = new System.Drawing.Point(0, 0);
            this.foldersTreeControl.Name = "foldersTreeControl";
            this.foldersTreeControl.Size = new System.Drawing.Size(179, 507);
            this.foldersTreeControl.TabIndex = 0;
            this.foldersTreeControl.SelectedFolderChanged += new System.EventHandler(this.FoldersTreeControlSelectedFolderChanged);
            // 
            // fileExplorerListView
            // 
            this.fileExplorerListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileExplorerListView.HideSelection = false;
            this.fileExplorerListView.Location = new System.Drawing.Point(0, 0);
            this.fileExplorerListView.Name = "fileExplorerListView";
            this.fileExplorerListView.ShowGroups = false;
            this.fileExplorerListView.Size = new System.Drawing.Size(615, 507);
            this.fileExplorerListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.fileExplorerListView.TabIndex = 0;
            this.fileExplorerListView.UseCompatibleStateImageBehavior = false;
            this.fileExplorerListView.View = System.Windows.Forms.View.Details;
            this.fileExplorerListView.ItemActivate += new System.EventHandler(this.FileExplorerListViewItemActivate);
            // 
            // UpdateFolderStructureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Name = "UpdateFolderStructureControl";
            this.Size = new System.Drawing.Size(798, 507);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private FoldersTreeControl foldersTreeControl;
        private FileExplorerListView fileExplorerListView;
    }
}
