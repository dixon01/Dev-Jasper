namespace MyHyperTerminal
{
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBox_packetToWrite = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_send = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_update = new System.Windows.Forms.Button();
            this.serialPortStatusPictureBox = new System.Windows.Forms.PictureBox();
            this.button_ResetSP = new System.Windows.Forms.Button();
            this.textBox_BufferInputSP = new System.Windows.Forms.TextBox();
            this.comboBox_NameSP = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_BufferOutputSP = new System.Windows.Forms.TextBox();
            this.comboBox_BaudSP = new System.Windows.Forms.ComboBox();
            this.comboBox_FlowControl = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_ParitySP = new System.Windows.Forms.ComboBox();
            this.trackBar_BufferInputSP = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_NBitSP = new System.Windows.Forms.ComboBox();
            this.trackBar_BufferOutputSP = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBox_BitStopSP = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_writeArea = new System.Windows.Forms.TextBox();
            this.button_clearDataRead = new System.Windows.Forms.Button();
            this.checkBox_ascii = new System.Windows.Forms.CheckBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoWriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendAFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iBISCreatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox_readAreaEnable = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serialPortStatusPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_BufferInputSP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_BufferOutputSP)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_packetToWrite
            // 
            this.textBox_packetToWrite.Location = new System.Drawing.Point(212, 50);
            this.textBox_packetToWrite.Name = "textBox_packetToWrite";
            this.textBox_packetToWrite.Size = new System.Drawing.Size(382, 20);
            this.textBox_packetToWrite.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(209, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Write a packet:";
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(519, 76);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(75, 23);
            this.button_send.TabIndex = 2;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.OnButtonSendClick);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.GhostWhite;
            this.groupBox1.Controls.Add(this.button_update);
            this.groupBox1.Controls.Add(this.serialPortStatusPictureBox);
            this.groupBox1.Controls.Add(this.button_ResetSP);
            this.groupBox1.Controls.Add(this.textBox_BufferInputSP);
            this.groupBox1.Controls.Add(this.comboBox_NameSP);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox_BufferOutputSP);
            this.groupBox1.Controls.Add(this.comboBox_BaudSP);
            this.groupBox1.Controls.Add(this.comboBox_FlowControl);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBox_ParitySP);
            this.groupBox1.Controls.Add(this.trackBar_BufferInputSP);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox_NBitSP);
            this.groupBox1.Controls.Add(this.trackBar_BufferOutputSP);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.comboBox_BitStopSP);
            this.groupBox1.Location = new System.Drawing.Point(3, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 336);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial Port Config:";
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(6, 34);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(110, 23);
            this.button_update.TabIndex = 46;
            this.button_update.Text = "Update COM list";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.OnButtonUpdateClick);
            // 
            // serialPortStatusPictureBox
            // 
            this.serialPortStatusPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.serialPortStatusPictureBox.Image = global::MyHyperTerminal.Properties.Resources.PowerOff;
            this.serialPortStatusPictureBox.Location = new System.Drawing.Point(122, 19);
            this.serialPortStatusPictureBox.Name = "serialPortStatusPictureBox";
            this.serialPortStatusPictureBox.Size = new System.Drawing.Size(55, 50);
            this.serialPortStatusPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.serialPortStatusPictureBox.TabIndex = 42;
            this.serialPortStatusPictureBox.TabStop = false;
            this.serialPortStatusPictureBox.Click += new System.EventHandler(this.OnSerialPortStatusPictureBoxClick);
            // 
            // button_ResetSP
            // 
            this.button_ResetSP.Location = new System.Drawing.Point(68, 283);
            this.button_ResetSP.Name = "button_ResetSP";
            this.button_ResetSP.Size = new System.Drawing.Size(75, 23);
            this.button_ResetSP.TabIndex = 45;
            this.button_ResetSP.Text = "Reset";
            this.button_ResetSP.UseVisualStyleBackColor = true;
            this.button_ResetSP.Click += new System.EventHandler(this.OnButtonResetSpClick);
            // 
            // textBox_BufferInputSP
            // 
            this.textBox_BufferInputSP.Location = new System.Drawing.Point(82, 229);
            this.textBox_BufferInputSP.Name = "textBox_BufferInputSP";
            this.textBox_BufferInputSP.Size = new System.Drawing.Size(44, 20);
            this.textBox_BufferInputSP.TabIndex = 33;
            this.textBox_BufferInputSP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // comboBox_NameSP
            // 
            this.comboBox_NameSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_NameSP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_NameSP.DropDownWidth = 200;
            this.comboBox_NameSP.FormattingEnabled = true;
            this.comboBox_NameSP.Location = new System.Drawing.Point(82, 74);
            this.comboBox_NameSP.MinimumSize = new System.Drawing.Size(80, 0);
            this.comboBox_NameSP.Name = "comboBox_NameSP";
            this.comboBox_NameSP.Size = new System.Drawing.Size(95, 21);
            this.comboBox_NameSP.TabIndex = 35;
            this.comboBox_NameSP.SelectedIndexChanged += new System.EventHandler(this.SerialParameterValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 208);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 44;
            this.label8.Text = "Flow Control";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 257);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Output Buffer";
            // 
            // textBox_BufferOutputSP
            // 
            this.textBox_BufferOutputSP.Location = new System.Drawing.Point(82, 254);
            this.textBox_BufferOutputSP.Name = "textBox_BufferOutputSP";
            this.textBox_BufferOutputSP.Size = new System.Drawing.Size(44, 20);
            this.textBox_BufferOutputSP.TabIndex = 34;
            this.textBox_BufferOutputSP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // comboBox_BaudSP
            // 
            this.comboBox_BaudSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_BaudSP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BaudSP.FormattingEnabled = true;
            this.comboBox_BaudSP.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "56000",
            "57600",
            "115200",
            "128000"});
            this.comboBox_BaudSP.Location = new System.Drawing.Point(82, 100);
            this.comboBox_BaudSP.MinimumSize = new System.Drawing.Size(80, 0);
            this.comboBox_BaudSP.Name = "comboBox_BaudSP";
            this.comboBox_BaudSP.Size = new System.Drawing.Size(95, 21);
            this.comboBox_BaudSP.TabIndex = 36;
            this.comboBox_BaudSP.SelectedIndexChanged += new System.EventHandler(this.SerialParameterValueChanged);
            // 
            // comboBox_FlowControl
            // 
            this.comboBox_FlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_FlowControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_FlowControl.DropDownWidth = 125;
            this.comboBox_FlowControl.FormattingEnabled = true;
            this.comboBox_FlowControl.Items.AddRange(new object[] {
            "None",
            "XOnXOff",
            "RequestToSend",
            "RequestToSendXOnXOff"});
            this.comboBox_FlowControl.Location = new System.Drawing.Point(82, 205);
            this.comboBox_FlowControl.MinimumSize = new System.Drawing.Size(80, 0);
            this.comboBox_FlowControl.Name = "comboBox_FlowControl";
            this.comboBox_FlowControl.Size = new System.Drawing.Size(95, 21);
            this.comboBox_FlowControl.TabIndex = 43;
            this.comboBox_FlowControl.SelectedIndexChanged += new System.EventHandler(this.SerialParameterValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 232);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Input Buffer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Port";
            // 
            // comboBox_ParitySP
            // 
            this.comboBox_ParitySP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_ParitySP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ParitySP.FormattingEnabled = true;
            this.comboBox_ParitySP.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.comboBox_ParitySP.Location = new System.Drawing.Point(82, 126);
            this.comboBox_ParitySP.MinimumSize = new System.Drawing.Size(80, 0);
            this.comboBox_ParitySP.Name = "comboBox_ParitySP";
            this.comboBox_ParitySP.Size = new System.Drawing.Size(95, 21);
            this.comboBox_ParitySP.TabIndex = 37;
            this.comboBox_ParitySP.SelectedIndexChanged += new System.EventHandler(this.SerialParameterValueChanged);
            // 
            // trackBar_BufferInputSP
            // 
            this.trackBar_BufferInputSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar_BufferInputSP.AutoSize = false;
            this.trackBar_BufferInputSP.Location = new System.Drawing.Point(126, 226);
            this.trackBar_BufferInputSP.Maximum = 10000;
            this.trackBar_BufferInputSP.Minimum = 1;
            this.trackBar_BufferInputSP.Name = "trackBar_BufferInputSP";
            this.trackBar_BufferInputSP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trackBar_BufferInputSP.Size = new System.Drawing.Size(53, 23);
            this.trackBar_BufferInputSP.TabIndex = 40;
            this.trackBar_BufferInputSP.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_BufferInputSP.Value = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Bit stop";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Baud rate";
            // 
            // comboBox_NBitSP
            // 
            this.comboBox_NBitSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_NBitSP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_NBitSP.FormattingEnabled = true;
            this.comboBox_NBitSP.Items.AddRange(new object[] {
            "8",
            "7",
            "6",
            "5"});
            this.comboBox_NBitSP.Location = new System.Drawing.Point(82, 152);
            this.comboBox_NBitSP.MinimumSize = new System.Drawing.Size(80, 0);
            this.comboBox_NBitSP.Name = "comboBox_NBitSP";
            this.comboBox_NBitSP.Size = new System.Drawing.Size(95, 21);
            this.comboBox_NBitSP.TabIndex = 38;
            this.comboBox_NBitSP.SelectedIndexChanged += new System.EventHandler(this.SerialParameterValueChanged);
            // 
            // trackBar_BufferOutputSP
            // 
            this.trackBar_BufferOutputSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar_BufferOutputSP.AutoSize = false;
            this.trackBar_BufferOutputSP.Location = new System.Drawing.Point(126, 254);
            this.trackBar_BufferOutputSP.Maximum = 10000;
            this.trackBar_BufferOutputSP.Minimum = 1;
            this.trackBar_BufferOutputSP.Name = "trackBar_BufferOutputSP";
            this.trackBar_BufferOutputSP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trackBar_BufferOutputSP.Size = new System.Drawing.Size(53, 23);
            this.trackBar_BufferOutputSP.TabIndex = 41;
            this.trackBar_BufferOutputSP.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_BufferOutputSP.Value = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "N°bit";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(42, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Parity";
            // 
            // comboBox_BitStopSP
            // 
            this.comboBox_BitStopSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_BitStopSP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BitStopSP.FormattingEnabled = true;
            this.comboBox_BitStopSP.Items.AddRange(new object[] {
            "One",
            "Two",
            "OnePointFive"});
            this.comboBox_BitStopSP.Location = new System.Drawing.Point(82, 178);
            this.comboBox_BitStopSP.MinimumSize = new System.Drawing.Size(80, 0);
            this.comboBox_BitStopSP.Name = "comboBox_BitStopSP";
            this.comboBox_BitStopSP.Size = new System.Drawing.Size(95, 21);
            this.comboBox_BitStopSP.TabIndex = 39;
            this.comboBox_BitStopSP.SelectedIndexChanged += new System.EventHandler(this.SerialParameterValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(209, 107);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 48;
            this.label10.Text = "Data read:";
            // 
            // textBox_writeArea
            // 
            this.textBox_writeArea.BackColor = System.Drawing.Color.GhostWhite;
            this.textBox_writeArea.Location = new System.Drawing.Point(212, 123);
            this.textBox_writeArea.Multiline = true;
            this.textBox_writeArea.Name = "textBox_writeArea";
            this.textBox_writeArea.ReadOnly = true;
            this.textBox_writeArea.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_writeArea.Size = new System.Drawing.Size(382, 243);
            this.textBox_writeArea.TabIndex = 49;
            // 
            // button_clearDataRead
            // 
            this.button_clearDataRead.Location = new System.Drawing.Point(519, 372);
            this.button_clearDataRead.Name = "button_clearDataRead";
            this.button_clearDataRead.Size = new System.Drawing.Size(75, 23);
            this.button_clearDataRead.TabIndex = 50;
            this.button_clearDataRead.Text = "Clear";
            this.button_clearDataRead.UseVisualStyleBackColor = true;
            this.button_clearDataRead.Click += new System.EventHandler(this.OnButtonClearDataReadClick);
            // 
            // checkBox_ascii
            // 
            this.checkBox_ascii.AutoSize = true;
            this.checkBox_ascii.Location = new System.Drawing.Point(499, 30);
            this.checkBox_ascii.Name = "checkBox_ascii";
            this.checkBox_ascii.Size = new System.Drawing.Size(95, 17);
            this.checkBox_ascii.TabIndex = 51;
            this.checkBox_ascii.Text = "Ascii encoding";
            this.checkBox_ascii.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(598, 24);
            this.menuStrip.TabIndex = 52;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExitToolStripMenuItemClick);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoWriteToolStripMenuItem,
            this.sendAFileToolStripMenuItem,
            this.iBISCreatorToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // autoWriteToolStripMenuItem
            // 
            this.autoWriteToolStripMenuItem.Name = "autoWriteToolStripMenuItem";
            this.autoWriteToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.autoWriteToolStripMenuItem.Text = "Write Scheduler";
            this.autoWriteToolStripMenuItem.Click += new System.EventHandler(this.OnAutoWriteToolStripMenuItemClick);
            // 
            // sendAFileToolStripMenuItem
            // 
            this.sendAFileToolStripMenuItem.Name = "sendAFileToolStripMenuItem";
            this.sendAFileToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.sendAFileToolStripMenuItem.Text = "Send a file";
            this.sendAFileToolStripMenuItem.Click += new System.EventHandler(this.OnSendAFileToolStripMenuItemClick);
            // 
            // iBISCreatorToolStripMenuItem
            // 
            this.iBISCreatorToolStripMenuItem.Name = "iBISCreatorToolStripMenuItem";
            this.iBISCreatorToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.iBISCreatorToolStripMenuItem.Text = "IBIS creator";
            this.iBISCreatorToolStripMenuItem.Click += new System.EventHandler(this.OnIBisCreatorToolStripMenuItemClick);
            // 
            // checkBox_readAreaEnable
            // 
            this.checkBox_readAreaEnable.AutoSize = true;
            this.checkBox_readAreaEnable.Checked = true;
            this.checkBox_readAreaEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_readAreaEnable.Location = new System.Drawing.Point(266, 106);
            this.checkBox_readAreaEnable.Name = "checkBox_readAreaEnable";
            this.checkBox_readAreaEnable.Size = new System.Drawing.Size(64, 17);
            this.checkBox_readAreaEnable.TabIndex = 53;
            this.checkBox_readAreaEnable.Text = "enabled";
            this.checkBox_readAreaEnable.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.ClientSize = new System.Drawing.Size(598, 399);
            this.Controls.Add(this.checkBox_readAreaEnable);
            this.Controls.Add(this.checkBox_ascii);
            this.Controls.Add(this.button_clearDataRead);
            this.Controls.Add(this.textBox_writeArea);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_packetToWrite);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Hyper Terminal by Cossi Stefano";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnForm1FormClosing);
            this.Load += new System.EventHandler(this.Form1Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serialPortStatusPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_BufferInputSP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_BufferOutputSP)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_packetToWrite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox serialPortStatusPictureBox;
        private System.Windows.Forms.Button button_ResetSP;
        private System.Windows.Forms.TextBox textBox_BufferInputSP;
        private System.Windows.Forms.ComboBox comboBox_NameSP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_BufferOutputSP;
        private System.Windows.Forms.ComboBox comboBox_BaudSP;
        private System.Windows.Forms.ComboBox comboBox_FlowControl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_ParitySP;
        private System.Windows.Forms.TrackBar trackBar_BufferInputSP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_NBitSP;
        private System.Windows.Forms.TrackBar trackBar_BufferOutputSP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox_BitStopSP;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_writeArea;
        private System.Windows.Forms.Button button_clearDataRead;
        private System.Windows.Forms.CheckBox checkBox_ascii;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoWriteToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox_readAreaEnable;
        private System.Windows.Forms.Button button_update;
        private System.Windows.Forms.ToolStripMenuItem sendAFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iBISCreatorToolStripMenuItem;
    }
}

