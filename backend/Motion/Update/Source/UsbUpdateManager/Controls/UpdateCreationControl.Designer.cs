namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    partial class UpdateCreationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateCreationControl));
            this.comboBoxUnitGroup = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripContainer2 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.addFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonExport = new System.Windows.Forms.Button();
            this.contextMenuStripExport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolStripSeparatorDeveloperMode = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAddFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemoveFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddFiles = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemoveFiles = new System.Windows.Forms.ToolStripButton();
            this.viewToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.largeIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonReplaceFiles = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditFile = new System.Windows.Forms.ToolStripButton();
            this.foldersTreeControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.FoldersTreeControl();
            this.fileExplorerListView = new Gorba.Motion.Update.UsbUpdateManager.Controls.FileExplorerListView();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxUnitGroup
            // 
            this.comboBoxUnitGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUnitGroup.FormattingEnabled = true;
            this.comboBoxUnitGroup.Location = new System.Drawing.Point(70, 3);
            this.comboBoxUnitGroup.Name = "comboBoxUnitGroup";
            this.comboBoxUnitGroup.Size = new System.Drawing.Size(163, 21);
            this.comboBoxUnitGroup.TabIndex = 0;
            this.comboBoxUnitGroup.DropDown += new System.EventHandler(this.ComboBoxUnitGroupDropDown);
            this.comboBoxUnitGroup.DropDownClosed += new System.EventHandler(this.ComboBoxUnitGroupDropDownClosed);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Unit Group:";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 30);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.toolStripContainer1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.toolStripContainer2);
            this.splitContainer.Size = new System.Drawing.Size(560, 378);
            this.splitContainer.SplitterDistance = 186;
            this.splitContainer.TabIndex = 2;
            this.splitContainer.Visible = false;
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.foldersTreeControl);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(186, 353);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(186, 378);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddFolder,
            this.toolStripButtonRemoveFolder});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(186, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripContainer2
            // 
            this.toolStripContainer2.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer2.ContentPanel
            // 
            this.toolStripContainer2.ContentPanel.Controls.Add(this.fileExplorerListView);
            this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(370, 353);
            this.toolStripContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer2.LeftToolStripPanelVisible = false;
            this.toolStripContainer2.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.RightToolStripPanelVisible = false;
            this.toolStripContainer2.Size = new System.Drawing.Size(370, 378);
            this.toolStripContainer2.TabIndex = 3;
            this.toolStripContainer2.Text = "toolStripContainer2";
            // 
            // toolStripContainer2.TopToolStripPanel
            // 
            this.toolStripContainer2.TopToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddFiles,
            this.toolStripButtonRemoveFiles,
            this.viewToolStripDropDownButton,
            this.toolStripSeparatorDeveloperMode,
            this.toolStripButtonReplaceFiles,
            this.toolStripButtonEditFile});
            this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(370, 25);
            this.toolStrip2.Stretch = true;
            this.toolStrip2.TabIndex = 0;
            // 
            // addFileDialog
            // 
            this.addFileDialog.Filter = "All files (*.*)|*.*";
            this.addFileDialog.Multiselect = true;
            this.addFileDialog.RestoreDirectory = true;
            this.addFileDialog.SupportMultiDottedExtensions = true;
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.Location = new System.Drawing.Point(469, 3);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(88, 23);
            this.buttonExport.TabIndex = 3;
            this.buttonExport.Text = "Export all...";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.ButtonExportClick);
            // 
            // contextMenuStripExport
            // 
            this.contextMenuStripExport.Name = "contextMenuStripExport";
            this.contextMenuStripExport.Size = new System.Drawing.Size(61, 4);
            // 
            // toolStripSeparatorDeveloperMode
            // 
            this.toolStripSeparatorDeveloperMode.Name = "toolStripSeparatorDeveloperMode";
            this.toolStripSeparatorDeveloperMode.Size = new System.Drawing.Size(6, 25);
            this.toolStripSeparatorDeveloperMode.Visible = false;
            // 
            // toolStripButtonAddFolder
            // 
            this.toolStripButtonAddFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddFolder.Enabled = false;
            this.toolStripButtonAddFolder.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.newfldr;
            this.toolStripButtonAddFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddFolder.Name = "toolStripButtonAddFolder";
            this.toolStripButtonAddFolder.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddFolder.Text = "Add";
            this.toolStripButtonAddFolder.Click += new System.EventHandler(this.ToolStripButtonAddFolderClick);
            // 
            // toolStripButtonRemoveFolder
            // 
            this.toolStripButtonRemoveFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemoveFolder.Enabled = false;
            this.toolStripButtonRemoveFolder.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.delete;
            this.toolStripButtonRemoveFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveFolder.Name = "toolStripButtonRemoveFolder";
            this.toolStripButtonRemoveFolder.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemoveFolder.Text = "Remove";
            this.toolStripButtonRemoveFolder.Click += new System.EventHandler(this.ToolStripButtonRemoveFolderClick);
            // 
            // toolStripButtonAddFiles
            // 
            this.toolStripButtonAddFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddFiles.Enabled = false;
            this.toolStripButtonAddFiles.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.AddFile;
            this.toolStripButtonAddFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddFiles.Name = "toolStripButtonAddFiles";
            this.toolStripButtonAddFiles.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddFiles.Text = "Add...";
            this.toolStripButtonAddFiles.Click += new System.EventHandler(this.ToolStripButtonAddFilesClick);
            // 
            // toolStripButtonRemoveFiles
            // 
            this.toolStripButtonRemoveFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemoveFiles.Enabled = false;
            this.toolStripButtonRemoveFiles.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.delete;
            this.toolStripButtonRemoveFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveFiles.Name = "toolStripButtonRemoveFiles";
            this.toolStripButtonRemoveFiles.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemoveFiles.Text = "Remove";
            this.toolStripButtonRemoveFiles.Click += new System.EventHandler(this.ToolStripButtonRemoveFilesClick);
            // 
            // viewToolStripDropDownButton
            // 
            this.viewToolStripDropDownButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.viewToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.largeIconToolStripMenuItem,
            this.smallIconToolStripMenuItem,
            this.listToolStripMenuItem,
            this.detailToolStripMenuItem,
            this.tilesToolStripMenuItem});
            this.viewToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("viewToolStripDropDownButton.Image")));
            this.viewToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewToolStripDropDownButton.Name = "viewToolStripDropDownButton";
            this.viewToolStripDropDownButton.Size = new System.Drawing.Size(45, 22);
            this.viewToolStripDropDownButton.Text = "View";
            // 
            // largeIconToolStripMenuItem
            // 
            this.largeIconToolStripMenuItem.Name = "largeIconToolStripMenuItem";
            this.largeIconToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.largeIconToolStripMenuItem.Text = "Large Icon";
            this.largeIconToolStripMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItemClick);
            // 
            // smallIconToolStripMenuItem
            // 
            this.smallIconToolStripMenuItem.Name = "smallIconToolStripMenuItem";
            this.smallIconToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.smallIconToolStripMenuItem.Text = "Small Icon";
            this.smallIconToolStripMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItemClick);
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.listToolStripMenuItem.Text = "List";
            this.listToolStripMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItemClick);
            // 
            // detailToolStripMenuItem
            // 
            this.detailToolStripMenuItem.Name = "detailToolStripMenuItem";
            this.detailToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.detailToolStripMenuItem.Text = "Details";
            this.detailToolStripMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItemClick);
            // 
            // tilesToolStripMenuItem
            // 
            this.tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
            this.tilesToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.tilesToolStripMenuItem.Text = "Tiles";
            this.tilesToolStripMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItemClick);
            // 
            // toolStripButtonReplaceFiles
            // 
            this.toolStripButtonReplaceFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReplaceFiles.Enabled = false;
            this.toolStripButtonReplaceFiles.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.move;
            this.toolStripButtonReplaceFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReplaceFiles.Name = "toolStripButtonReplaceFiles";
            this.toolStripButtonReplaceFiles.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonReplaceFiles.Text = "Replace Files...";
            this.toolStripButtonReplaceFiles.Visible = false;
            this.toolStripButtonReplaceFiles.Click += new System.EventHandler(this.ToolStripButtonReplaceFilesClick);
            // 
            // toolStripButtonEditFile
            // 
            this.toolStripButtonEditFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditFile.Enabled = false;
            this.toolStripButtonEditFile.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.EditDocument;
            this.toolStripButtonEditFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditFile.Name = "toolStripButtonEditFile";
            this.toolStripButtonEditFile.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEditFile.Text = "Edit File...";
            this.toolStripButtonEditFile.Visible = false;
            this.toolStripButtonEditFile.Click += new System.EventHandler(this.ToolStripButtonEditFileClick);
            // 
            // foldersTreeControl
            // 
            this.foldersTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foldersTreeControl.Location = new System.Drawing.Point(0, 0);
            this.foldersTreeControl.Name = "foldersTreeControl";
            this.foldersTreeControl.Size = new System.Drawing.Size(186, 353);
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
            this.fileExplorerListView.Size = new System.Drawing.Size(370, 353);
            this.fileExplorerListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.fileExplorerListView.TabIndex = 0;
            this.fileExplorerListView.UseCompatibleStateImageBehavior = false;
            this.fileExplorerListView.View = System.Windows.Forms.View.Details;
            this.fileExplorerListView.ItemActivate += new System.EventHandler(this.FileExplorerListViewItemActivate);
            this.fileExplorerListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.FileExplorerListViewItemSelectionChanged);
            // 
            // UpdateCreationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxUnitGroup);
            this.Name = "UpdateCreationControl";
            this.Size = new System.Drawing.Size(560, 408);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.PerformLayout();
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxUnitGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer;
        private FoldersTreeControl foldersTreeControl;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFolder;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveFolder;
        private System.Windows.Forms.ToolStripContainer toolStripContainer2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFiles;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveFiles;
        private System.Windows.Forms.OpenFileDialog addFileDialog;
        private System.Windows.Forms.ToolStripDropDownButton viewToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem largeIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripExport;
        private FileExplorerListView fileExplorerListView;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripButton toolStripButtonReplaceFiles;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorDeveloperMode;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditFile;
    }
}
