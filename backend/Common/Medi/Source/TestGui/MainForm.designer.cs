namespace Gorba.Common.Medi.TestGui
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
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.configurationTabPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.messageDispatcherTabPage = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.mediView1 = new Gorba.Common.Medi.TestGui.MediView();
            this.managementView1 = new Gorba.Common.Medi.TestGui.ManagementView();
            this.fileSystemView1 = new Gorba.Common.Medi.TestGui.FileSystemView();
            this.mediAddressEditor1 = new Gorba.Common.Medi.TestGui.MediAddressEditor();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.configurationTabPage.SuspendLayout();
            this.messageDispatcherTabPage.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(0, 520);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Configure";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 37);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Size = new System.Drawing.Size(684, 546);
            this.splitContainer1.SplitterDistance = 136;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(136, 514);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1AfterSelect);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.configurationTabPage);
            this.tabControl1.Controls.Add(this.messageDispatcherTabPage);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(698, 612);
            this.tabControl1.TabIndex = 3;
            // 
            // configurationTabPage
            // 
            this.configurationTabPage.Controls.Add(this.mediAddressEditor1);
            this.configurationTabPage.Controls.Add(this.label1);
            this.configurationTabPage.Controls.Add(this.splitContainer1);
            this.configurationTabPage.Location = new System.Drawing.Point(4, 22);
            this.configurationTabPage.Name = "configurationTabPage";
            this.configurationTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.configurationTabPage.Size = new System.Drawing.Size(690, 586);
            this.configurationTabPage.TabIndex = 0;
            this.configurationTabPage.Text = "Configuration";
            this.configurationTabPage.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Address:";
            // 
            // messageDispatcherTabPage
            // 
            this.messageDispatcherTabPage.Controls.Add(this.mediView1);
            this.messageDispatcherTabPage.Location = new System.Drawing.Point(4, 22);
            this.messageDispatcherTabPage.Name = "messageDispatcherTabPage";
            this.messageDispatcherTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.messageDispatcherTabPage.Size = new System.Drawing.Size(690, 586);
            this.messageDispatcherTabPage.TabIndex = 1;
            this.messageDispatcherTabPage.Text = "Message Dispatcher";
            this.messageDispatcherTabPage.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.managementView1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(690, 586);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Management";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.fileSystemView1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(690, 586);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "File System";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // mediView1
            // 
            this.mediView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mediView1.Location = new System.Drawing.Point(3, 3);
            this.mediView1.Name = "mediView1";
            this.mediView1.Size = new System.Drawing.Size(684, 580);
            this.mediView1.TabIndex = 0;
            // 
            // managementView1
            // 
            this.managementView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.managementView1.Location = new System.Drawing.Point(3, 3);
            this.managementView1.Name = "managementView1";
            this.managementView1.Size = new System.Drawing.Size(684, 580);
            this.managementView1.TabIndex = 0;
            // 
            // fileSystemView1
            // 
            this.fileSystemView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileSystemView1.Location = new System.Drawing.Point(3, 3);
            this.fileSystemView1.Name = "fileSystemView1";
            this.fileSystemView1.Size = new System.Drawing.Size(684, 580);
            this.fileSystemView1.TabIndex = 0;
            // 
            // mediAddressEditor1
            // 
            this.mediAddressEditor1.Location = new System.Drawing.Point(60, 8);
            this.mediAddressEditor1.Name = "mediAddressEditor1";
            this.mediAddressEditor1.Size = new System.Drawing.Size(301, 23);
            this.mediAddressEditor1.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 612);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "Medi Protocol Tester";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.configurationTabPage.ResumeLayout(false);
            this.configurationTabPage.PerformLayout();
            this.messageDispatcherTabPage.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage configurationTabPage;
        private System.Windows.Forms.TabPage messageDispatcherTabPage;
        private MediView mediView1;
        private System.Windows.Forms.TabPage tabPage3;
        private ManagementView managementView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage4;
        private FileSystemView fileSystemView1;
        private MediAddressEditor mediAddressEditor1;
    }
}

