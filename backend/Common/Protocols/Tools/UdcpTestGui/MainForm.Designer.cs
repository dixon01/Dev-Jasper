namespace Gorba.Common.Protocols.UdcpTestGui
{
    sealed partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.toolStripContainer2 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxDiscovery = new System.Windows.Forms.ListBox();
            this.propertyGridDiscovery = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSerach = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.propertyGridLocal = new System.Windows.Forms.PropertyGrid();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonEnabled = new System.Windows.Forms.ToolStripButton();
            this.announceTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAnnounce = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonConfigure = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReboot = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(435, 364);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.toolStripContainer2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(427, 338);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Discovery";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // toolStripContainer2
            // 
            // 
            // toolStripContainer2.ContentPanel
            // 
            this.toolStripContainer2.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(421, 307);
            this.toolStripContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer2.Location = new System.Drawing.Point(3, 3);
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.Size = new System.Drawing.Size(421, 332);
            this.toolStripContainer2.TabIndex = 1;
            this.toolStripContainer2.Text = "toolStripContainer2";
            // 
            // toolStripContainer2.TopToolStripPanel
            // 
            this.toolStripContainer2.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxDiscovery);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGridDiscovery);
            this.splitContainer1.Size = new System.Drawing.Size(421, 307);
            this.splitContainer1.SplitterDistance = 140;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBoxDiscovery
            // 
            this.listBoxDiscovery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxDiscovery.FormattingEnabled = true;
            this.listBoxDiscovery.IntegralHeight = false;
            this.listBoxDiscovery.Location = new System.Drawing.Point(0, 0);
            this.listBoxDiscovery.Name = "listBoxDiscovery";
            this.listBoxDiscovery.Size = new System.Drawing.Size(140, 307);
            this.listBoxDiscovery.TabIndex = 0;
            this.listBoxDiscovery.SelectedIndexChanged += new System.EventHandler(this.ListBoxDiscoverySelectedIndexChanged);
            // 
            // propertyGridDiscovery
            // 
            this.propertyGridDiscovery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridDiscovery.Location = new System.Drawing.Point(0, 0);
            this.propertyGridDiscovery.Name = "propertyGridDiscovery";
            this.propertyGridDiscovery.Size = new System.Drawing.Size(277, 307);
            this.propertyGridDiscovery.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSerach,
            this.toolStripSeparator1,
            this.toolStripButtonAnnounce,
            this.toolStripButtonConfigure,
            this.toolStripButtonReboot});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(132, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripButtonSerach
            // 
            this.toolStripButtonSerach.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSerach.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSerach.Image")));
            this.toolStripButtonSerach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSerach.Name = "toolStripButtonSerach";
            this.toolStripButtonSerach.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSerach.Click += new System.EventHandler(this.ToolStripButtonSerachClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.propertyGridLocal);
            this.tabPage2.Controls.Add(this.toolStrip2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(427, 338);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Local";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // propertyGridLocal
            // 
            this.propertyGridLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridLocal.Enabled = false;
            this.propertyGridLocal.Location = new System.Drawing.Point(3, 28);
            this.propertyGridLocal.Name = "propertyGridLocal";
            this.propertyGridLocal.Size = new System.Drawing.Size(421, 307);
            this.propertyGridLocal.TabIndex = 2;
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEnabled});
            this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(421, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButtonEnabled
            // 
            this.toolStripButtonEnabled.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonEnabled.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEnabled.Image")));
            this.toolStripButtonEnabled.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEnabled.Name = "toolStripButtonEnabled";
            this.toolStripButtonEnabled.Size = new System.Drawing.Size(53, 22);
            this.toolStripButtonEnabled.Text = "Enabled";
            this.toolStripButtonEnabled.Click += new System.EventHandler(this.ToolStripButtonEnabledClick);
            // 
            // announceTimer
            // 
            this.announceTimer.Interval = 200;
            this.announceTimer.Tick += new System.EventHandler(this.AnnounceTimerTick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonAnnounce
            // 
            this.toolStripButtonAnnounce.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAnnounce.Enabled = false;
            this.toolStripButtonAnnounce.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAnnounce.Image")));
            this.toolStripButtonAnnounce.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAnnounce.Name = "toolStripButtonAnnounce";
            this.toolStripButtonAnnounce.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAnnounce.Text = "Announce";
            this.toolStripButtonAnnounce.Click += new System.EventHandler(this.ToolStripButtonAnnounceClick);
            // 
            // toolStripButtonConfigure
            // 
            this.toolStripButtonConfigure.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConfigure.Enabled = false;
            this.toolStripButtonConfigure.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonConfigure.Image")));
            this.toolStripButtonConfigure.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConfigure.Name = "toolStripButtonConfigure";
            this.toolStripButtonConfigure.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonConfigure.Text = "Send Configuration";
            this.toolStripButtonConfigure.Click += new System.EventHandler(this.ToolStripButtonConfigureClick);
            // 
            // toolStripButtonReboot
            // 
            this.toolStripButtonReboot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReboot.Enabled = false;
            this.toolStripButtonReboot.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReboot.Image")));
            this.toolStripButtonReboot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReboot.Name = "toolStripButtonReboot";
            this.toolStripButtonReboot.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonReboot.Text = "Reboot";
            this.toolStripButtonReboot.Click += new System.EventHandler(this.ToolStripButtonRebootClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 364);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "UDCP Tester";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.PerformLayout();
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBoxDiscovery;
        private System.Windows.Forms.PropertyGrid propertyGridDiscovery;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer2;
        private System.Windows.Forms.ToolStripButton toolStripButtonSerach;
        private System.Windows.Forms.PropertyGrid propertyGridLocal;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonEnabled;
        private System.Windows.Forms.Timer announceTimer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAnnounce;
        private System.Windows.Forms.ToolStripButton toolStripButtonConfigure;
        private System.Windows.Forms.ToolStripButton toolStripButtonReboot;
    }
}

