namespace Gorba.Common.Medi.TestGui
{
    partial class ManagementView
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
            this.treeManagement = new System.Windows.Forms.TreeView();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pgridManagement = new System.Windows.Forms.PropertyGrid();
            this.gridTable = new System.Windows.Forms.DataGridView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTable)).BeginInit();
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
            this.splitContainer1.Panel1.Controls.Add(this.treeManagement);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gridTable);
            this.splitContainer1.Panel2.Controls.Add(this.pgridManagement);
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
            // treeManagement
            // 
            this.treeManagement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeManagement.ContextMenuStrip = this.contextMenu;
            this.treeManagement.Location = new System.Drawing.Point(0, 30);
            this.treeManagement.Name = "treeManagement";
            this.treeManagement.Size = new System.Drawing.Size(199, 320);
            this.treeManagement.TabIndex = 0;
            this.treeManagement.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeManagementBeforeExpand);
            this.treeManagement.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeManagementAfterSelect);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(114, 26);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuOpening);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.RefreshToolStripMenuItemClick);
            // 
            // pgridManagement
            // 
            this.pgridManagement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgridManagement.Location = new System.Drawing.Point(0, 0);
            this.pgridManagement.Name = "pgridManagement";
            this.pgridManagement.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.pgridManagement.Size = new System.Drawing.Size(367, 350);
            this.pgridManagement.TabIndex = 0;
            this.pgridManagement.ToolbarVisible = false;
            this.pgridManagement.Visible = false;
            // 
            // gridTable
            // 
            this.gridTable.AllowUserToAddRows = false;
            this.gridTable.AllowUserToDeleteRows = false;
            this.gridTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTable.Location = new System.Drawing.Point(0, 0);
            this.gridTable.Name = "gridTable";
            this.gridTable.Size = new System.Drawing.Size(367, 350);
            this.gridTable.TabIndex = 1;
            this.gridTable.Visible = false;
            // 
            // ManagementView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ManagementView";
            this.Size = new System.Drawing.Size(570, 350);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox cbxSource;
        private System.Windows.Forms.TreeView treeManagement;
        private System.Windows.Forms.PropertyGrid pgridManagement;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.DataGridView gridTable;
    }
}
