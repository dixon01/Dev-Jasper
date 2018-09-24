namespace Gorba.Common.Gioom.Tools.Controls
{
    partial class LocalPortsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalPortsControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxPorts = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItemAddFlag = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddFlagReadWrite = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddFlagReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddInt = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddIntReadWrite = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddIntReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddEnum = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddEnumReadWrite = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddEnumReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddEnumFlags = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddEnumFlagsReadWrite = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddEnumFlagsReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonRemove = new System.Windows.Forms.ToolStripButton();
            this.portInfoControl = new PortInfoControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxPorts);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.portInfoControl);
            this.splitContainer1.Size = new System.Drawing.Size(641, 450);
            this.splitContainer1.SplitterDistance = 213;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBoxPorts
            // 
            this.listBoxPorts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxPorts.FormattingEnabled = true;
            this.listBoxPorts.Location = new System.Drawing.Point(0, 25);
            this.listBoxPorts.Name = "listBoxPorts";
            this.listBoxPorts.Size = new System.Drawing.Size(213, 425);
            this.listBoxPorts.TabIndex = 1;
            this.listBoxPorts.SelectedIndexChanged += new System.EventHandler(this.ListBoxPortsOnSelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripButtonRemove});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(213, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddFlag,
            this.toolStripMenuItemAddInt,
            this.toolStripMenuItemAddEnum,
            this.toolStripMenuItemAddEnumFlags});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(28, 22);
            this.toolStripDropDownButton1.Text = "+";
            // 
            // toolStripMenuItemAddFlag
            // 
            this.toolStripMenuItemAddFlag.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddFlagReadWrite,
            this.toolStripMenuItemAddFlagReadOnly});
            this.toolStripMenuItemAddFlag.Name = "toolStripMenuItemAddFlag";
            this.toolStripMenuItemAddFlag.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemAddFlag.Text = "Flag";
            // 
            // toolStripMenuItemAddFlagReadWrite
            // 
            this.toolStripMenuItemAddFlagReadWrite.Name = "toolStripMenuItemAddFlagReadWrite";
            this.toolStripMenuItemAddFlagReadWrite.Size = new System.Drawing.Size(133, 22);
            this.toolStripMenuItemAddFlagReadWrite.Text = "Read/Write";
            this.toolStripMenuItemAddFlagReadWrite.Click += new System.EventHandler(this.ToolStripMenuItemAddFlagReadWriteOnClick);
            // 
            // toolStripMenuItemAddFlagReadOnly
            // 
            this.toolStripMenuItemAddFlagReadOnly.Name = "toolStripMenuItemAddFlagReadOnly";
            this.toolStripMenuItemAddFlagReadOnly.Size = new System.Drawing.Size(133, 22);
            this.toolStripMenuItemAddFlagReadOnly.Text = "Read-Only";
            this.toolStripMenuItemAddFlagReadOnly.Click += new System.EventHandler(this.ToolStripMenuItemAddFlagReadOnlyOnClick);
            // 
            // toolStripMenuItemAddInt
            // 
            this.toolStripMenuItemAddInt.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddIntReadWrite,
            this.toolStripMenuItemAddIntReadOnly});
            this.toolStripMenuItemAddInt.Name = "toolStripMenuItemAddInt";
            this.toolStripMenuItemAddInt.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemAddInt.Text = "Integer";
            // 
            // toolStripMenuItemAddIntReadWrite
            // 
            this.toolStripMenuItemAddIntReadWrite.Name = "toolStripMenuItemAddIntReadWrite";
            this.toolStripMenuItemAddIntReadWrite.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItemAddIntReadWrite.Text = "0..20 (r/w)";
            this.toolStripMenuItemAddIntReadWrite.Click += new System.EventHandler(this.ToolStripMenuItemAddIntReadWriteOnClick);
            // 
            // toolStripMenuItemAddIntReadOnly
            // 
            this.toolStripMenuItemAddIntReadOnly.Name = "toolStripMenuItemAddIntReadOnly";
            this.toolStripMenuItemAddIntReadOnly.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItemAddIntReadOnly.Text = "-100..100 (ro)";
            this.toolStripMenuItemAddIntReadOnly.Click += new System.EventHandler(this.ToolStripMenuItemAddIntReadOnlyOnClick);
            // 
            // toolStripMenuItemAddEnum
            // 
            this.toolStripMenuItemAddEnum.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddEnumReadWrite,
            this.toolStripMenuItemAddEnumReadOnly});
            this.toolStripMenuItemAddEnum.Name = "toolStripMenuItemAddEnum";
            this.toolStripMenuItemAddEnum.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemAddEnum.Text = "Enum";
            // 
            // toolStripMenuItemAddEnumReadWrite
            // 
            this.toolStripMenuItemAddEnumReadWrite.Name = "toolStripMenuItemAddEnumReadWrite";
            this.toolStripMenuItemAddEnumReadWrite.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItemAddEnumReadWrite.Text = "ProcessPriority (r/w)";
            this.toolStripMenuItemAddEnumReadWrite.Click += new System.EventHandler(this.ToolStripMenuItemAddEnumReadWriteOnClick);
            // 
            // toolStripMenuItemAddEnumReadOnly
            // 
            this.toolStripMenuItemAddEnumReadOnly.Name = "toolStripMenuItemAddEnumReadOnly";
            this.toolStripMenuItemAddEnumReadOnly.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItemAddEnumReadOnly.Text = "DayOfWeek (ro)";
            this.toolStripMenuItemAddEnumReadOnly.Click += new System.EventHandler(this.ToolStripMenuItemAddEnumReadOnlyOnClick);
            // 
            // toolStripMenuItemAddEnumFlags
            // 
            this.toolStripMenuItemAddEnumFlags.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddEnumFlagsReadWrite,
            this.toolStripMenuItemAddEnumFlagsReadOnly});
            this.toolStripMenuItemAddEnumFlags.Name = "toolStripMenuItemAddEnumFlags";
            this.toolStripMenuItemAddEnumFlags.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemAddEnumFlags.Text = "Enum Flags";
            // 
            // toolStripMenuItemAddEnumFlagsReadWrite
            // 
            this.toolStripMenuItemAddEnumFlagsReadWrite.Name = "toolStripMenuItemAddEnumFlagsReadWrite";
            this.toolStripMenuItemAddEnumFlagsReadWrite.Size = new System.Drawing.Size(166, 22);
            this.toolStripMenuItemAddEnumFlagsReadWrite.Text = "FontStyle (r/w)";
            this.toolStripMenuItemAddEnumFlagsReadWrite.Click += new System.EventHandler(this.ToolStripMenuItemAddEnumFlagsReadWriteOnClick);
            // 
            // toolStripMenuItemAddEnumFlagsReadOnly
            // 
            this.toolStripMenuItemAddEnumFlagsReadOnly.Name = "toolStripMenuItemAddEnumFlagsReadOnly";
            this.toolStripMenuItemAddEnumFlagsReadOnly.Size = new System.Drawing.Size(166, 22);
            this.toolStripMenuItemAddEnumFlagsReadOnly.Text = "FileAttributes (ro)";
            this.toolStripMenuItemAddEnumFlagsReadOnly.Click += new System.EventHandler(this.ToolStripMenuItemAddEnumFlagsReadOnlyOnClick);
            // 
            // toolStripButtonRemove
            // 
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonRemove.Enabled = false;
            this.toolStripButtonRemove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemove.Image")));
            this.toolStripButtonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemove.Text = "-";
            this.toolStripButtonRemove.Click += new System.EventHandler(this.ToolStripButtonRemoveOnClick);
            // 
            // portInfoControl
            // 
            this.portInfoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portInfoControl.IsLocal = true;
            this.portInfoControl.Location = new System.Drawing.Point(0, 0);
            this.portInfoControl.Name = "portInfoControl";
            this.portInfoControl.Size = new System.Drawing.Size(424, 450);
            this.portInfoControl.TabIndex = 0;
            this.portInfoControl.Visible = false;
            // 
            // LocalPortsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "LocalPortsControl";
            this.Size = new System.Drawing.Size(641, 450);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddFlag;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemove;
        private System.Windows.Forms.ListBox listBoxPorts;
        private PortInfoControl portInfoControl;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddFlagReadOnly;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddFlagReadWrite;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddInt;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddIntReadWrite;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddIntReadOnly;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddEnum;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddEnumReadWrite;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddEnumReadOnly;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddEnumFlags;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddEnumFlagsReadOnly;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddEnumFlagsReadWrite;
    }
}
