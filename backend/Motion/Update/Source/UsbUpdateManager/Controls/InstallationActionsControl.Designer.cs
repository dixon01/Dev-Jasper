namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    partial class InstallationActionsControl
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButtonAdd = new System.Windows.Forms.ToolStripDropDownButton();
            this.addExecutableFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRegularFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.executeLocalApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDown = new System.Windows.Forms.ToolStripButton();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addRegularFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.addExecutableFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonAdd,
            this.toolStripButtonDelete,
            this.toolStripSeparator1,
            this.toolStripButtonUp,
            this.toolStripButtonDown});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(447, 25);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 0;
            // 
            // toolStripDropDownButtonAdd
            // 
            this.toolStripDropDownButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addExecutableFileToolStripMenuItem,
            this.addRegularFileToolStripMenuItem,
            this.toolStripMenuItem1,
            this.executeLocalApplicationToolStripMenuItem});
            this.toolStripDropDownButtonAdd.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.AddFile;
            this.toolStripDropDownButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonAdd.Name = "toolStripDropDownButtonAdd";
            this.toolStripDropDownButtonAdd.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButtonAdd.Text = "Add...";
            // 
            // addExecutableFileToolStripMenuItem
            // 
            this.addExecutableFileToolStripMenuItem.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.ExecutableFile;
            this.addExecutableFileToolStripMenuItem.Name = "addExecutableFileToolStripMenuItem";
            this.addExecutableFileToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.addExecutableFileToolStripMenuItem.Text = "Add Executable File...";
            this.addExecutableFileToolStripMenuItem.Click += new System.EventHandler(this.AddExecutableFileToolStripMenuItemClick);
            // 
            // addRegularFileToolStripMenuItem
            // 
            this.addRegularFileToolStripMenuItem.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.RegularFile;
            this.addRegularFileToolStripMenuItem.Name = "addRegularFileToolStripMenuItem";
            this.addRegularFileToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.addRegularFileToolStripMenuItem.Text = "Add Regular File...";
            this.addRegularFileToolStripMenuItem.Click += new System.EventHandler(this.AddRegularFileToolStripMenuItemClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(215, 6);
            // 
            // executeLocalApplicationToolStripMenuItem
            // 
            this.executeLocalApplicationToolStripMenuItem.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.ExecuteLocal;
            this.executeLocalApplicationToolStripMenuItem.Name = "executeLocalApplicationToolStripMenuItem";
            this.executeLocalApplicationToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.executeLocalApplicationToolStripMenuItem.Text = "Execute Local Application...";
            this.executeLocalApplicationToolStripMenuItem.Click += new System.EventHandler(this.ExecuteLocalApplicationToolStripMenuItemClick);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelete.Enabled = false;
            this.toolStripButtonDelete.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.delete;
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelete.Text = "Delete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.ToolStripButtonDeleteClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonUp
            // 
            this.toolStripButtonUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUp.Enabled = false;
            this.toolStripButtonUp.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.UpArrowShort_Blue;
            this.toolStripButtonUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUp.Name = "toolStripButtonUp";
            this.toolStripButtonUp.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUp.Text = "Up";
            this.toolStripButtonUp.Click += new System.EventHandler(this.ToolStripButtonUpClick);
            // 
            // toolStripButtonDown
            // 
            this.toolStripButtonDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDown.Enabled = false;
            this.toolStripButtonDown.Image = global::Gorba.Motion.Update.UsbUpdateManager.Properties.Resources.DownArrowShort_Blue;
            this.toolStripButtonDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDown.Name = "toolStripButtonDown";
            this.toolStripButtonDown.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDown.Text = "Down";
            this.toolStripButtonDown.Click += new System.EventHandler(this.ToolStripButtonDownClick);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.LargeImageList = this.imageListLarge;
            this.listView.Location = new System.Drawing.Point(0, 25);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(447, 331);
            this.listView.SmallImageList = this.imageListSmall;
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.ListViewSelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Arguments";
            this.columnHeader2.Width = 250;
            // 
            // addRegularFileDialog
            // 
            this.addRegularFileDialog.Filter = "All files (*.*)|*.*";
            this.addRegularFileDialog.Multiselect = true;
            this.addRegularFileDialog.RestoreDirectory = true;
            this.addRegularFileDialog.Title = "Select Regular File(s) to Add";
            // 
            // imageListLarge
            // 
            this.imageListLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListLarge.ImageSize = new System.Drawing.Size(32, 32);
            this.imageListLarge.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // imageListSmall
            // 
            this.imageListSmall.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListSmall.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // addExecutableFileDialog
            // 
            this.addExecutableFileDialog.Filter = "Executable files (*.exe;*.bat)|*.exe;*.bat|All files (*.*)|*.*";
            // 
            // InstallationActionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStrip);
            this.Name = "InstallationActionsControl";
            this.Size = new System.Drawing.Size(447, 356);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonAdd;
        private System.Windows.Forms.ToolStripMenuItem addExecutableFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRegularFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem executeLocalApplicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonDown;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.OpenFileDialog addRegularFileDialog;
        private System.Windows.Forms.ImageList imageListLarge;
        private System.Windows.Forms.ImageList imageListSmall;
        private System.Windows.Forms.OpenFileDialog addExecutableFileDialog;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
