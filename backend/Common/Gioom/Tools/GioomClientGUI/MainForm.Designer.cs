namespace Gorba.Common.Gioom.Tools.ClientGUI
{
    using Gorba.Common.Gioom.Tools.Controls;

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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxNodeSearch = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.treeViewPorts = new Aga.Controls.Tree.TreeViewAdv();
            this.treeColumnPort = new Aga.Controls.Tree.TreeColumn();
            this.treeColumnValue = new Aga.Controls.Tree.TreeColumn();
            this.nodeStateIcon = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeTextBoxPort = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBoxValue = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.portInfoControl1 = new Gorba.Common.Gioom.Tools.Controls.PortInfoControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.localPortsControl1 = new Gorba.Common.Gioom.Tools.Controls.LocalPortsControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.portInfoSearch = new Gorba.Common.Gioom.Tools.Controls.PortInfoControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonSearchPort = new System.Windows.Forms.Button();
            this.textBoxSearchName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSearchApp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSearchUnit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Node:";
            // 
            // textBoxNodeSearch
            // 
            this.textBoxNodeSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNodeSearch.Location = new System.Drawing.Point(45, 14);
            this.textBoxNodeSearch.Name = "textBoxNodeSearch";
            this.textBoxNodeSearch.Size = new System.Drawing.Size(132, 20);
            this.textBoxNodeSearch.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonSearch);
            this.splitContainer1.Panel1.Controls.Add(this.treeViewPorts);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxNodeSearch);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.portInfoControl1);
            this.splitContainer1.Size = new System.Drawing.Size(787, 420);
            this.splitContainer1.SplitterDistance = 261;
            this.splitContainer1.TabIndex = 3;
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.Location = new System.Drawing.Point(183, 12);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonSearch.TabIndex = 3;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.ButtonSearchOnClick);
            // 
            // treeViewPorts
            // 
            this.treeViewPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewPorts.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewPorts.Columns.Add(this.treeColumnPort);
            this.treeViewPorts.Columns.Add(this.treeColumnValue);
            this.treeViewPorts.DefaultToolTipProvider = null;
            this.treeViewPorts.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewPorts.FullRowSelect = true;
            this.treeViewPorts.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.treeViewPorts.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewPorts.Location = new System.Drawing.Point(3, 41);
            this.treeViewPorts.Model = null;
            this.treeViewPorts.Name = "treeViewPorts";
            this.treeViewPorts.NodeControls.Add(this.nodeStateIcon);
            this.treeViewPorts.NodeControls.Add(this.nodeTextBoxPort);
            this.treeViewPorts.NodeControls.Add(this.nodeTextBoxValue);
            this.treeViewPorts.SelectedNode = null;
            this.treeViewPorts.Size = new System.Drawing.Size(255, 376);
            this.treeViewPorts.TabIndex = 0;
            this.treeViewPorts.Text = "treeViewAdv1";
            this.treeViewPorts.UseColumns = true;
            this.treeViewPorts.SelectionChanged += new System.EventHandler(this.TreeViewPortsOnSelectionChanged);
            // 
            // treeColumnPort
            // 
            this.treeColumnPort.Header = "Port";
            this.treeColumnPort.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumnPort.TooltipText = null;
            this.treeColumnPort.Width = 150;
            // 
            // treeColumnValue
            // 
            this.treeColumnValue.Header = "Value";
            this.treeColumnValue.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumnValue.TooltipText = null;
            this.treeColumnValue.Width = 100;
            // 
            // nodeStateIcon
            // 
            this.nodeStateIcon.LeftMargin = 1;
            this.nodeStateIcon.ParentColumn = this.treeColumnPort;
            this.nodeStateIcon.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBoxPort
            // 
            this.nodeTextBoxPort.DataPropertyName = "Name";
            this.nodeTextBoxPort.IncrementalSearchEnabled = true;
            this.nodeTextBoxPort.LeftMargin = 3;
            this.nodeTextBoxPort.ParentColumn = this.treeColumnPort;
            // 
            // nodeTextBoxValue
            // 
            this.nodeTextBoxValue.DataPropertyName = "Value";
            this.nodeTextBoxValue.IncrementalSearchEnabled = true;
            this.nodeTextBoxValue.LeftMargin = 3;
            this.nodeTextBoxValue.ParentColumn = this.treeColumnValue;
            // 
            // portInfoControl1
            // 
            this.portInfoControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portInfoControl1.Location = new System.Drawing.Point(0, 0);
            this.portInfoControl1.Name = "portInfoControl1";
            this.portInfoControl1.Size = new System.Drawing.Size(522, 420);
            this.portInfoControl1.TabIndex = 0;
            this.portInfoControl1.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(801, 452);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(793, 426);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Query Ports";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.localPortsControl1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(793, 426);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Local Ports";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // localPortsControl1
            // 
            this.localPortsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.localPortsControl1.Location = new System.Drawing.Point(3, 3);
            this.localPortsControl1.Name = "localPortsControl1";
            this.localPortsControl1.Size = new System.Drawing.Size(787, 420);
            this.localPortsControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.portInfoSearch);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(793, 426);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Search Port";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // portInfoSearch
            // 
            this.portInfoSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.portInfoSearch.Location = new System.Drawing.Point(6, 109);
            this.portInfoSearch.Name = "portInfoSearch";
            this.portInfoSearch.Size = new System.Drawing.Size(781, 311);
            this.portInfoSearch.TabIndex = 1;
            this.portInfoSearch.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonSearchPort);
            this.groupBox1.Controls.Add(this.textBoxSearchName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxSearchApp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxSearchUnit);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(781, 97);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Query";
            // 
            // buttonSearchPort
            // 
            this.buttonSearchPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearchPort.Location = new System.Drawing.Point(700, 68);
            this.buttonSearchPort.Name = "buttonSearchPort";
            this.buttonSearchPort.Size = new System.Drawing.Size(75, 23);
            this.buttonSearchPort.TabIndex = 6;
            this.buttonSearchPort.Text = "Search";
            this.buttonSearchPort.UseVisualStyleBackColor = true;
            this.buttonSearchPort.Click += new System.EventHandler(this.ButtonSearchPortClick);
            // 
            // textBoxSearchName
            // 
            this.textBoxSearchName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearchName.Location = new System.Drawing.Point(69, 71);
            this.textBoxSearchName.Name = "textBoxSearchName";
            this.textBoxSearchName.Size = new System.Drawing.Size(625, 20);
            this.textBoxSearchName.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "I/O Name:";
            // 
            // textBoxSearchApp
            // 
            this.textBoxSearchApp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearchApp.Location = new System.Drawing.Point(69, 45);
            this.textBoxSearchApp.Name = "textBoxSearchApp";
            this.textBoxSearchApp.Size = new System.Drawing.Size(625, 20);
            this.textBoxSearchApp.TabIndex = 5;
            this.textBoxSearchApp.Text = "*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Application:";
            // 
            // textBoxSearchUnit
            // 
            this.textBoxSearchUnit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearchUnit.Location = new System.Drawing.Point(69, 19);
            this.textBoxSearchUnit.Name = "textBoxSearchUnit";
            this.textBoxSearchUnit.Size = new System.Drawing.Size(625, 20);
            this.textBoxSearchUnit.TabIndex = 5;
            this.textBoxSearchUnit.Text = "*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Unit:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 452);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "GIOoM Test GUI";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv treeViewPorts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNodeSearch;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonSearch;
        private Aga.Controls.Tree.TreeColumn treeColumnPort;
        private Aga.Controls.Tree.TreeColumn treeColumnValue;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxPort;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxValue;
        private PortInfoControl portInfoControl1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private LocalPortsControl localPortsControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonSearchPort;
        private System.Windows.Forms.TextBox textBoxSearchName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSearchApp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSearchUnit;
        private System.Windows.Forms.Label label2;
        private PortInfoControl portInfoSearch;
    }
}

