namespace Gorba.Motion.SystemManager.TestApps.WinFormsTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.trackBarRamUsage = new System.Windows.Forms.TrackBar();
            this.trackBarCpuUsage = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.propertyGridInfo = new System.Windows.Forms.PropertyGrid();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxCommandLine = new System.Windows.Forms.TextBox();
            this.buttonProcessKill = new System.Windows.Forms.Button();
            this.buttonApplicationExit = new System.Windows.Forms.Button();
            this.buttonOutOfMemory = new System.Windows.Forms.Button();
            this.buttonUnhandledException = new System.Windows.Forms.Button();
            this.buttonEnvironmentExitNormal = new System.Windows.Forms.Button();
            this.buttonEnvironmentExit = new System.Windows.Forms.Button();
            this.checkBoxRegistered = new System.Windows.Forms.CheckBox();
            this.checkBoxWatchdog = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAutoRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonRelaunch = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonReboot = new System.Windows.Forms.ToolStripButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.logsTextBox = new System.Windows.Forms.RichTextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.buttonRefreshSystemInfo = new System.Windows.Forms.Button();
            this.propertyGridSystemInfo = new System.Windows.Forms.PropertyGrid();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRamUsage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCpuUsage)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(628, 479);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.trackBarRamUsage);
            this.tabPage1.Controls.Add(this.trackBarCpuUsage);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.textBoxCommandLine);
            this.tabPage1.Controls.Add(this.buttonProcessKill);
            this.tabPage1.Controls.Add(this.buttonApplicationExit);
            this.tabPage1.Controls.Add(this.buttonOutOfMemory);
            this.tabPage1.Controls.Add(this.buttonUnhandledException);
            this.tabPage1.Controls.Add(this.buttonEnvironmentExitNormal);
            this.tabPage1.Controls.Add(this.buttonEnvironmentExit);
            this.tabPage1.Controls.Add(this.checkBoxRegistered);
            this.tabPage1.Controls.Add(this.checkBoxWatchdog);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(620, 453);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Application";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // trackBarRamUsage
            // 
            this.trackBarRamUsage.AutoSize = false;
            this.trackBarRamUsage.Location = new System.Drawing.Point(103, 171);
            this.trackBarRamUsage.Maximum = 100;
            this.trackBarRamUsage.Name = "trackBarRamUsage";
            this.trackBarRamUsage.Size = new System.Drawing.Size(248, 31);
            this.trackBarRamUsage.TabIndex = 8;
            this.trackBarRamUsage.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarRamUsage.ValueChanged += new System.EventHandler(this.TrackBarRamUsageValueChanged);
            // 
            // trackBarCpuUsage
            // 
            this.trackBarCpuUsage.AutoSize = false;
            this.trackBarCpuUsage.Location = new System.Drawing.Point(103, 134);
            this.trackBarCpuUsage.Maximum = 100;
            this.trackBarCpuUsage.Name = "trackBarCpuUsage";
            this.trackBarCpuUsage.Size = new System.Drawing.Size(248, 31);
            this.trackBarCpuUsage.TabIndex = 8;
            this.trackBarCpuUsage.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarCpuUsage.ValueChanged += new System.EventHandler(this.TrackBarCpuUsageValueChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.propertyGridInfo);
            this.panel1.Location = new System.Drawing.Point(103, 208);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(511, 239);
            this.panel1.TabIndex = 7;
            // 
            // propertyGridInfo
            // 
            this.propertyGridInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridInfo.HelpVisible = false;
            this.propertyGridInfo.Location = new System.Drawing.Point(0, 0);
            this.propertyGridInfo.Name = "propertyGridInfo";
            this.propertyGridInfo.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGridInfo.Size = new System.Drawing.Size(507, 235);
            this.propertyGridInfo.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "RAM Usage";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "CPU Usage";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 215);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Information";
            // 
            // textBoxCommandLine
            // 
            this.textBoxCommandLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCommandLine.Location = new System.Drawing.Point(103, 6);
            this.textBoxCommandLine.Name = "textBoxCommandLine";
            this.textBoxCommandLine.ReadOnly = true;
            this.textBoxCommandLine.Size = new System.Drawing.Size(511, 20);
            this.textBoxCommandLine.TabIndex = 4;
            // 
            // buttonProcessKill
            // 
            this.buttonProcessKill.Location = new System.Drawing.Point(484, 76);
            this.buttonProcessKill.Name = "buttonProcessKill";
            this.buttonProcessKill.Size = new System.Drawing.Size(121, 23);
            this.buttonProcessKill.TabIndex = 3;
            this.buttonProcessKill.Text = "Process.Kill()";
            this.buttonProcessKill.UseVisualStyleBackColor = true;
            this.buttonProcessKill.Click += new System.EventHandler(this.ButtonProcessKillClick);
            // 
            // buttonApplicationExit
            // 
            this.buttonApplicationExit.Location = new System.Drawing.Point(357, 76);
            this.buttonApplicationExit.Name = "buttonApplicationExit";
            this.buttonApplicationExit.Size = new System.Drawing.Size(121, 23);
            this.buttonApplicationExit.TabIndex = 3;
            this.buttonApplicationExit.Text = "Application.Exit()";
            this.buttonApplicationExit.UseVisualStyleBackColor = true;
            this.buttonApplicationExit.Click += new System.EventHandler(this.ButtonApplicationExitClick);
            // 
            // buttonOutOfMemory
            // 
            this.buttonOutOfMemory.Location = new System.Drawing.Point(230, 105);
            this.buttonOutOfMemory.Name = "buttonOutOfMemory";
            this.buttonOutOfMemory.Size = new System.Drawing.Size(121, 23);
            this.buttonOutOfMemory.TabIndex = 3;
            this.buttonOutOfMemory.Text = "Out of Memory";
            this.buttonOutOfMemory.UseVisualStyleBackColor = true;
            this.buttonOutOfMemory.Click += new System.EventHandler(this.ButtonOutOfMemoryClick);
            // 
            // buttonUnhandledException
            // 
            this.buttonUnhandledException.Location = new System.Drawing.Point(103, 105);
            this.buttonUnhandledException.Name = "buttonUnhandledException";
            this.buttonUnhandledException.Size = new System.Drawing.Size(121, 23);
            this.buttonUnhandledException.TabIndex = 3;
            this.buttonUnhandledException.Text = "Unhandled Exception";
            this.buttonUnhandledException.UseVisualStyleBackColor = true;
            this.buttonUnhandledException.Click += new System.EventHandler(this.ButtonUnhandledExceptionClick);
            // 
            // buttonEnvironmentExitNormal
            // 
            this.buttonEnvironmentExitNormal.Location = new System.Drawing.Point(103, 76);
            this.buttonEnvironmentExitNormal.Name = "buttonEnvironmentExitNormal";
            this.buttonEnvironmentExitNormal.Size = new System.Drawing.Size(121, 23);
            this.buttonEnvironmentExitNormal.TabIndex = 3;
            this.buttonEnvironmentExitNormal.Text = "Environment.Exit(0)";
            this.buttonEnvironmentExitNormal.UseVisualStyleBackColor = true;
            this.buttonEnvironmentExitNormal.Click += new System.EventHandler(this.ButtonEnvironmentExitNormalClick);
            // 
            // buttonEnvironmentExit
            // 
            this.buttonEnvironmentExit.Location = new System.Drawing.Point(230, 76);
            this.buttonEnvironmentExit.Name = "buttonEnvironmentExit";
            this.buttonEnvironmentExit.Size = new System.Drawing.Size(121, 23);
            this.buttonEnvironmentExit.TabIndex = 3;
            this.buttonEnvironmentExit.Text = "Environment.Exit(-1)";
            this.buttonEnvironmentExit.UseVisualStyleBackColor = true;
            this.buttonEnvironmentExit.Click += new System.EventHandler(this.ButtonEnvironmentExitClick);
            // 
            // checkBoxRegistered
            // 
            this.checkBoxRegistered.AutoSize = true;
            this.checkBoxRegistered.Enabled = false;
            this.checkBoxRegistered.Location = new System.Drawing.Point(103, 32);
            this.checkBoxRegistered.Name = "checkBoxRegistered";
            this.checkBoxRegistered.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRegistered.TabIndex = 2;
            this.checkBoxRegistered.UseVisualStyleBackColor = true;
            // 
            // checkBoxWatchdog
            // 
            this.checkBoxWatchdog.AutoSize = true;
            this.checkBoxWatchdog.Checked = true;
            this.checkBoxWatchdog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWatchdog.Location = new System.Drawing.Point(103, 56);
            this.checkBoxWatchdog.Name = "checkBoxWatchdog";
            this.checkBoxWatchdog.Size = new System.Drawing.Size(15, 14);
            this.checkBoxWatchdog.TabIndex = 2;
            this.checkBoxWatchdog.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Fail Application";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Registered";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Exit Application";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Satisfy Watchdog";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Command Line";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.toolStripContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(620, 453);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "System Manager";
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dataGridView1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(614, 422);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(3, 3);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(614, 447);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.Size = new System.Drawing.Size(614, 422);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1SelectionChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAutoRefresh,
            this.toolStripButtonRefresh,
            this.toolStripLabel1,
            this.toolStripButtonRelaunch,
            this.toolStripButtonExit,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.toolStripButtonReboot});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(614, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripButtonAutoRefresh
            // 
            this.toolStripButtonAutoRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonAutoRefresh.Checked = true;
            this.toolStripButtonAutoRefresh.CheckOnClick = true;
            this.toolStripButtonAutoRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonAutoRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonAutoRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAutoRefresh.Image")));
            this.toolStripButtonAutoRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAutoRefresh.Name = "toolStripButtonAutoRefresh";
            this.toolStripButtonAutoRefresh.Size = new System.Drawing.Size(115, 22);
            this.toolStripButtonAutoRefresh.Text = "Auto-Refresh States";
            this.toolStripButtonAutoRefresh.CheckedChanged += new System.EventHandler(this.ToolStripButtonAutoRefreshCheckedChanged);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRefresh.Image")));
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(50, 22);
            this.toolStripButtonRefresh.Text = "Refresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.ToolStripButtonRefreshClick);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(71, 22);
            this.toolStripLabel1.Text = "Application:";
            // 
            // toolStripButtonRelaunch
            // 
            this.toolStripButtonRelaunch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonRelaunch.Enabled = false;
            this.toolStripButtonRelaunch.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRelaunch.Image")));
            this.toolStripButtonRelaunch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRelaunch.Name = "toolStripButtonRelaunch";
            this.toolStripButtonRelaunch.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonRelaunch.Text = "Relaunch";
            this.toolStripButtonRelaunch.Click += new System.EventHandler(this.ToolStripButtonRelaunchClick);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonExit.Enabled = false;
            this.toolStripButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExit.Image")));
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(29, 22);
            this.toolStripButtonExit.Text = "Exit";
            this.toolStripButtonExit.Click += new System.EventHandler(this.ToolStripButtonExitClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(48, 22);
            this.toolStripLabel2.Text = "System:";
            // 
            // toolStripButtonReboot
            // 
            this.toolStripButtonReboot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonReboot.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReboot.Image")));
            this.toolStripButtonReboot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReboot.Name = "toolStripButtonReboot";
            this.toolStripButtonReboot.Size = new System.Drawing.Size(49, 22);
            this.toolStripButtonReboot.Text = "Reboot";
            this.toolStripButtonReboot.Click += new System.EventHandler(this.ToolStripButtonRebootClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.logsTextBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(620, 453);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Logs";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // logsTextBox
            // 
            this.logsTextBox.BackColor = System.Drawing.Color.Black;
            this.logsTextBox.DetectUrls = false;
            this.logsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logsTextBox.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logsTextBox.ForeColor = System.Drawing.Color.White;
            this.logsTextBox.Location = new System.Drawing.Point(3, 3);
            this.logsTextBox.Name = "logsTextBox";
            this.logsTextBox.ReadOnly = true;
            this.logsTextBox.Size = new System.Drawing.Size(614, 447);
            this.logsTextBox.TabIndex = 0;
            this.logsTextBox.Text = "";
            this.logsTextBox.WordWrap = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.propertyGridSystemInfo);
            this.tabPage4.Controls.Add(this.buttonRefreshSystemInfo);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(620, 453);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "System Info";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // buttonRefreshSystemInfo
            // 
            this.buttonRefreshSystemInfo.Location = new System.Drawing.Point(6, 6);
            this.buttonRefreshSystemInfo.Name = "buttonRefreshSystemInfo";
            this.buttonRefreshSystemInfo.Size = new System.Drawing.Size(75, 23);
            this.buttonRefreshSystemInfo.TabIndex = 0;
            this.buttonRefreshSystemInfo.Text = "Refresh";
            this.buttonRefreshSystemInfo.UseVisualStyleBackColor = true;
            this.buttonRefreshSystemInfo.Click += new System.EventHandler(this.ButtonRefreshSystemInfoClick);
            // 
            // propertyGridSystemInfo
            // 
            this.propertyGridSystemInfo.HelpVisible = false;
            this.propertyGridSystemInfo.Location = new System.Drawing.Point(6, 35);
            this.propertyGridSystemInfo.Name = "propertyGridSystemInfo";
            this.propertyGridSystemInfo.Size = new System.Drawing.Size(608, 412);
            this.propertyGridSystemInfo.TabIndex = 1;
            this.propertyGridSystemInfo.ToolbarVisible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 479);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "System Manager Test GUI";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRamUsage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCpuUsage)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox logsTextBox;
        private System.Windows.Forms.CheckBox checkBoxWatchdog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAutoRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.Button buttonEnvironmentExit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonProcessKill;
        private System.Windows.Forms.Button buttonApplicationExit;
        private System.Windows.Forms.Button buttonUnhandledException;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonEnvironmentExitNormal;
        private System.Windows.Forms.Button buttonOutOfMemory;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.ToolStripButton toolStripButtonRelaunch;
        private System.Windows.Forms.TextBox textBoxCommandLine;
        private System.Windows.Forms.CheckBox checkBoxRegistered;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PropertyGrid propertyGridInfo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackBarCpuUsage;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar trackBarRamUsage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripButton toolStripButtonReboot;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.PropertyGrid propertyGridSystemInfo;
        private System.Windows.Forms.Button buttonRefreshSystemInfo;
    }
}

