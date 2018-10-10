namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Forms
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
            this.label_listeningPort = new System.Windows.Forms.Label();
            this.numericUpDown_port = new System.Windows.Forms.NumericUpDown();
            this.button_startStop = new System.Windows.Forms.Button();
            this.label_listening = new System.Windows.Forms.Label();
            this.textBox_receivedText = new System.Windows.Forms.TextBox();
            this.button_clear = new System.Windows.Forms.Button();
            this.textBox_internalMessages = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label_internalMessages = new System.Windows.Forms.Label();
            this.label_periodicSend = new System.Windows.Forms.Label();
            this.numericUpDown_period = new System.Windows.Forms.NumericUpDown();
            this.label_ms = new System.Windows.Forms.Label();
            this.label_lineNumber = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox_tripInfo = new System.Windows.Forms.GroupBox();
            this.label_destArab = new System.Windows.Forms.Label();
            this.label_destination = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadStateMachineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxLineInfo = new System.Windows.Forms.GroupBox();
            this.labelLineNumber = new System.Windows.Forms.Label();
            this.labelCurrentDirectionNo = new System.Windows.Forms.Label();
            this.labelDestinationNo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelExi_DestinationArabic = new System.Windows.Forms.Label();
            this.labelExi_Destination = new System.Windows.Forms.Label();
            this.labelExi_LineNo = new System.Windows.Forms.Label();
            this.labelExi_CurrentDirectionNo = new System.Windows.Forms.Label();
            this.labelExi_DestinationNo = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelCountdownNumber = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelSpecialInputState = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxStatus = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxDisplayStateFront = new System.Windows.Forms.ComboBox();
            this.comboBoxDisplayStateLeft = new System.Windows.Forms.ComboBox();
            this.comboBoxDisplayStateRight = new System.Windows.Forms.ComboBox();
            this.comboBoxDisplayStateRear = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_port)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_period)).BeginInit();
            this.groupBox_tripInfo.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBoxLineInfo.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_listeningPort
            // 
            this.label_listeningPort.AutoSize = true;
            this.label_listeningPort.Location = new System.Drawing.Point(12, 29);
            this.label_listeningPort.Name = "label_listeningPort";
            this.label_listeningPort.Size = new System.Drawing.Size(29, 13);
            this.label_listeningPort.TabIndex = 0;
            this.label_listeningPort.Text = "Port:";
            // 
            // numericUpDown_port
            // 
            this.numericUpDown_port.Location = new System.Drawing.Point(58, 27);
            this.numericUpDown_port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDown_port.Name = "numericUpDown_port";
            this.numericUpDown_port.Size = new System.Drawing.Size(79, 20);
            this.numericUpDown_port.TabIndex = 1;
            this.numericUpDown_port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_port.Value = new decimal(new int[] {
            32021,
            0,
            0,
            0});
            // 
            // button_startStop
            // 
            this.button_startStop.Location = new System.Drawing.Point(143, 27);
            this.button_startStop.Name = "button_startStop";
            this.button_startStop.Size = new System.Drawing.Size(49, 20);
            this.button_startStop.TabIndex = 2;
            this.button_startStop.Text = "Start";
            this.button_startStop.UseVisualStyleBackColor = true;
            this.button_startStop.Click += new System.EventHandler(this.ButtonStartStopClick);
            // 
            // label_listening
            // 
            this.label_listening.AutoSize = true;
            this.label_listening.Location = new System.Drawing.Point(198, 31);
            this.label_listening.Name = "label_listening";
            this.label_listening.Size = new System.Drawing.Size(69, 13);
            this.label_listening.TabIndex = 3;
            this.label_listening.Text = "(not listening)";
            // 
            // textBox_receivedText
            // 
            this.textBox_receivedText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_receivedText.Location = new System.Drawing.Point(460, 47);
            this.textBox_receivedText.Multiline = true;
            this.textBox_receivedText.Name = "textBox_receivedText";
            this.textBox_receivedText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_receivedText.Size = new System.Drawing.Size(511, 325);
            this.textBox_receivedText.TabIndex = 4;
            // 
            // button_clear
            // 
            this.button_clear.Location = new System.Drawing.Point(410, 53);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(44, 20);
            this.button_clear.TabIndex = 5;
            this.button_clear.Text = "Clear";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.ButtonClearTextAreaClick);
            // 
            // textBox_internalMessages
            // 
            this.textBox_internalMessages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_internalMessages.Location = new System.Drawing.Point(460, 404);
            this.textBox_internalMessages.Multiline = true;
            this.textBox_internalMessages.Name = "textBox_internalMessages";
            this.textBox_internalMessages.Size = new System.Drawing.Size(511, 68);
            this.textBox_internalMessages.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(927, 378);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(44, 20);
            this.button1.TabIndex = 7;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ButtonClearInternalMsgClick);
            // 
            // label_internalMessages
            // 
            this.label_internalMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_internalMessages.AutoSize = true;
            this.label_internalMessages.Location = new System.Drawing.Point(460, 388);
            this.label_internalMessages.Name = "label_internalMessages";
            this.label_internalMessages.Size = new System.Drawing.Size(95, 13);
            this.label_internalMessages.TabIndex = 8;
            this.label_internalMessages.Text = "Internal messages:";
            // 
            // label_periodicSend
            // 
            this.label_periodicSend.AutoSize = true;
            this.label_periodicSend.Location = new System.Drawing.Point(12, 57);
            this.label_periodicSend.Name = "label_periodicSend";
            this.label_periodicSend.Size = new System.Drawing.Size(40, 13);
            this.label_periodicSend.TabIndex = 9;
            this.label_periodicSend.Text = "Period:";
            // 
            // numericUpDown_period
            // 
            this.numericUpDown_period.Location = new System.Drawing.Point(58, 55);
            this.numericUpDown_period.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDown_period.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_period.Name = "numericUpDown_period";
            this.numericUpDown_period.Size = new System.Drawing.Size(79, 20);
            this.numericUpDown_period.TabIndex = 10;
            this.numericUpDown_period.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_period.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // label_ms
            // 
            this.label_ms.AutoSize = true;
            this.label_ms.Location = new System.Drawing.Point(143, 57);
            this.label_ms.Name = "label_ms";
            this.label_ms.Size = new System.Drawing.Size(20, 13);
            this.label_ms.TabIndex = 11;
            this.label_ms.Text = "ms";
            // 
            // label_lineNumber
            // 
            this.label_lineNumber.AutoSize = true;
            this.label_lineNumber.Location = new System.Drawing.Point(6, 16);
            this.label_lineNumber.Name = "label_lineNumber";
            this.label_lineNumber.Size = new System.Drawing.Size(86, 13);
            this.label_lineNumber.TabIndex = 12;
            this.label_lineNumber.Text = "Line number: xxx";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(460, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Data received:";
            // 
            // groupBox_tripInfo
            // 
            this.groupBox_tripInfo.Controls.Add(this.label_destArab);
            this.groupBox_tripInfo.Controls.Add(this.label_destination);
            this.groupBox_tripInfo.Controls.Add(this.label_lineNumber);
            this.groupBox_tripInfo.Location = new System.Drawing.Point(12, 212);
            this.groupBox_tripInfo.Name = "groupBox_tripInfo";
            this.groupBox_tripInfo.Size = new System.Drawing.Size(442, 41);
            this.groupBox_tripInfo.TabIndex = 14;
            this.groupBox_tripInfo.TabStop = false;
            this.groupBox_tripInfo.Text = "Trip info (100) - Obsolete";
            // 
            // label_destArab
            // 
            this.label_destArab.AutoSize = true;
            this.label_destArab.Location = new System.Drawing.Point(287, 16);
            this.label_destArab.Name = "label_destArab";
            this.label_destArab.Size = new System.Drawing.Size(77, 13);
            this.label_destArab.TabIndex = 14;
            this.label_destArab.Text = "Dest. arab: xxx";
            // 
            // label_destination
            // 
            this.label_destination.AutoSize = true;
            this.label_destination.Location = new System.Drawing.Point(136, 16);
            this.label_destination.Name = "label_destination";
            this.label_destination.Size = new System.Drawing.Size(81, 13);
            this.label_destination.TabIndex = 13;
            this.label_destination.Text = "Destination: xxx";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(983, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
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
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadStateMachineToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolToolStripMenuItem.Text = "Tools";
            // 
            // downloadStateMachineToolStripMenuItem
            // 
            this.downloadStateMachineToolStripMenuItem.Name = "downloadStateMachineToolStripMenuItem";
            this.downloadStateMachineToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.downloadStateMachineToolStripMenuItem.Text = "Download State Machine";
            this.downloadStateMachineToolStripMenuItem.Click += new System.EventHandler(this.DownloadStateMachineToolStripMenuItemClick);
            // 
            // groupBoxLineInfo
            // 
            this.groupBoxLineInfo.Controls.Add(this.labelLineNumber);
            this.groupBoxLineInfo.Controls.Add(this.labelCurrentDirectionNo);
            this.groupBoxLineInfo.Controls.Add(this.labelDestinationNo);
            this.groupBoxLineInfo.Location = new System.Drawing.Point(12, 259);
            this.groupBoxLineInfo.Name = "groupBoxLineInfo";
            this.groupBoxLineInfo.Size = new System.Drawing.Size(442, 36);
            this.groupBoxLineInfo.TabIndex = 16;
            this.groupBoxLineInfo.TabStop = false;
            this.groupBoxLineInfo.Text = "Line Info (103) - Obsolete";
            // 
            // labelLineNumber
            // 
            this.labelLineNumber.AutoSize = true;
            this.labelLineNumber.Location = new System.Drawing.Point(289, 17);
            this.labelLineNumber.Name = "labelLineNumber";
            this.labelLineNumber.Size = new System.Drawing.Size(86, 13);
            this.labelLineNumber.TabIndex = 2;
            this.labelLineNumber.Text = "Line number: xxx";
            // 
            // labelCurrentDirectionNo
            // 
            this.labelCurrentDirectionNo.AutoSize = true;
            this.labelCurrentDirectionNo.Location = new System.Drawing.Point(138, 17);
            this.labelCurrentDirectionNo.Name = "labelCurrentDirectionNo";
            this.labelCurrentDirectionNo.Size = new System.Drawing.Size(124, 13);
            this.labelCurrentDirectionNo.TabIndex = 1;
            this.labelCurrentDirectionNo.Text = "Current Direction No: xxx";
            // 
            // labelDestinationNo
            // 
            this.labelDestinationNo.AutoSize = true;
            this.labelDestinationNo.Location = new System.Drawing.Point(6, 17);
            this.labelDestinationNo.Name = "labelDestinationNo";
            this.labelDestinationNo.Size = new System.Drawing.Size(98, 13);
            this.labelDestinationNo.TabIndex = 0;
            this.labelDestinationNo.Text = "Destination No: xxx";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelExi_DestinationArabic);
            this.groupBox1.Controls.Add(this.labelExi_Destination);
            this.groupBox1.Controls.Add(this.labelExi_LineNo);
            this.groupBox1.Controls.Add(this.labelExi_CurrentDirectionNo);
            this.groupBox1.Controls.Add(this.labelExi_DestinationNo);
            this.groupBox1.Location = new System.Drawing.Point(12, 301);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(442, 77);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Extended Line Info (104)";
            // 
            // labelExi_DestinationArabic
            // 
            this.labelExi_DestinationArabic.AutoSize = true;
            this.labelExi_DestinationArabic.Location = new System.Drawing.Point(175, 49);
            this.labelExi_DestinationArabic.Name = "labelExi_DestinationArabic";
            this.labelExi_DestinationArabic.Size = new System.Drawing.Size(77, 13);
            this.labelExi_DestinationArabic.TabIndex = 17;
            this.labelExi_DestinationArabic.Text = "Dest. arab: xxx";
            // 
            // labelExi_Destination
            // 
            this.labelExi_Destination.AutoSize = true;
            this.labelExi_Destination.Location = new System.Drawing.Point(6, 49);
            this.labelExi_Destination.Name = "labelExi_Destination";
            this.labelExi_Destination.Size = new System.Drawing.Size(81, 13);
            this.labelExi_Destination.TabIndex = 16;
            this.labelExi_Destination.Text = "Destination: xxx";
            // 
            // labelExi_LineNo
            // 
            this.labelExi_LineNo.AutoSize = true;
            this.labelExi_LineNo.Location = new System.Drawing.Point(334, 25);
            this.labelExi_LineNo.Name = "labelExi_LineNo";
            this.labelExi_LineNo.Size = new System.Drawing.Size(86, 13);
            this.labelExi_LineNo.TabIndex = 5;
            this.labelExi_LineNo.Text = "Line number: xxx";
            // 
            // labelExi_CurrentDirectionNo
            // 
            this.labelExi_CurrentDirectionNo.AutoSize = true;
            this.labelExi_CurrentDirectionNo.Location = new System.Drawing.Point(175, 25);
            this.labelExi_CurrentDirectionNo.Name = "labelExi_CurrentDirectionNo";
            this.labelExi_CurrentDirectionNo.Size = new System.Drawing.Size(124, 13);
            this.labelExi_CurrentDirectionNo.TabIndex = 4;
            this.labelExi_CurrentDirectionNo.Text = "Current Direction No: xxx";
            // 
            // labelExi_DestinationNo
            // 
            this.labelExi_DestinationNo.AutoSize = true;
            this.labelExi_DestinationNo.Location = new System.Drawing.Point(6, 25);
            this.labelExi_DestinationNo.Name = "labelExi_DestinationNo";
            this.labelExi_DestinationNo.Size = new System.Drawing.Size(98, 13);
            this.labelExi_DestinationNo.TabIndex = 3;
            this.labelExi_DestinationNo.Text = "Destination No: xxx";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelCountdownNumber);
            this.groupBox2.Location = new System.Drawing.Point(12, 384);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(442, 41);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Countdown Number (105)";
            // 
            // labelCountdownNumber
            // 
            this.labelCountdownNumber.AutoSize = true;
            this.labelCountdownNumber.Location = new System.Drawing.Point(6, 16);
            this.labelCountdownNumber.Name = "labelCountdownNumber";
            this.labelCountdownNumber.Size = new System.Drawing.Size(105, 13);
            this.labelCountdownNumber.TabIndex = 12;
            this.labelCountdownNumber.Text = "Countdown number: ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelSpecialInputState);
            this.groupBox3.Location = new System.Drawing.Point(12, 431);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(442, 41);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Special Input Info (106)";
            // 
            // labelSpecialInputState
            // 
            this.labelSpecialInputState.AutoSize = true;
            this.labelSpecialInputState.Location = new System.Drawing.Point(6, 16);
            this.labelSpecialInputState.Name = "labelSpecialInputState";
            this.labelSpecialInputState.Size = new System.Drawing.Size(103, 13);
            this.labelSpecialInputState.TabIndex = 12;
            this.labelSpecialInputState.Text = "Special Input State: ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBoxStatus);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(12, 81);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(442, 46);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Status (1)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Status:";
            // 
            // comboBoxStatus
            // 
            this.comboBoxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStatus.FormattingEnabled = true;
            this.comboBoxStatus.Location = new System.Drawing.Point(52, 19);
            this.comboBoxStatus.Name = "comboBoxStatus";
            this.comboBoxStatus.Size = new System.Drawing.Size(121, 21);
            this.comboBoxStatus.TabIndex = 13;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.comboBoxDisplayStateRear);
            this.groupBox5.Controls.Add(this.comboBoxDisplayStateRight);
            this.groupBox5.Controls.Add(this.comboBoxDisplayStateLeft);
            this.groupBox5.Controls.Add(this.comboBoxDisplayStateFront);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Location = new System.Drawing.Point(12, 133);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(442, 73);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Display Status (3)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Front:";
            // 
            // comboBoxDisplayStateFront
            // 
            this.comboBoxDisplayStateFront.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisplayStateFront.FormattingEnabled = true;
            this.comboBoxDisplayStateFront.Location = new System.Drawing.Point(52, 19);
            this.comboBoxDisplayStateFront.Name = "comboBoxDisplayStateFront";
            this.comboBoxDisplayStateFront.Size = new System.Drawing.Size(121, 21);
            this.comboBoxDisplayStateFront.TabIndex = 13;
            // 
            // comboBoxDisplayStateLeft
            // 
            this.comboBoxDisplayStateLeft.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisplayStateLeft.FormattingEnabled = true;
            this.comboBoxDisplayStateLeft.Location = new System.Drawing.Point(52, 46);
            this.comboBoxDisplayStateLeft.Name = "comboBoxDisplayStateLeft";
            this.comboBoxDisplayStateLeft.Size = new System.Drawing.Size(121, 21);
            this.comboBoxDisplayStateLeft.TabIndex = 13;
            // 
            // comboBoxDisplayStateRight
            // 
            this.comboBoxDisplayStateRight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisplayStateRight.FormattingEnabled = true;
            this.comboBoxDisplayStateRight.Location = new System.Drawing.Point(254, 46);
            this.comboBoxDisplayStateRight.Name = "comboBoxDisplayStateRight";
            this.comboBoxDisplayStateRight.Size = new System.Drawing.Size(121, 21);
            this.comboBoxDisplayStateRight.TabIndex = 13;
            // 
            // comboBoxDisplayStateRear
            // 
            this.comboBoxDisplayStateRear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisplayStateRear.FormattingEnabled = true;
            this.comboBoxDisplayStateRear.Location = new System.Drawing.Point(254, 19);
            this.comboBoxDisplayStateRear.Name = "comboBoxDisplayStateRear";
            this.comboBoxDisplayStateRear.Size = new System.Drawing.Size(121, 21);
            this.comboBoxDisplayStateRear.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Left:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(208, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Right:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(208, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Rear:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(983, 484);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxLineInfo);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox_tripInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_ms);
            this.Controls.Add(this.numericUpDown_period);
            this.Controls.Add(this.label_periodicSend);
            this.Controls.Add(this.label_internalMessages);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_internalMessages);
            this.Controls.Add(this.button_clear);
            this.Controls.Add(this.textBox_receivedText);
            this.Controls.Add(this.label_listening);
            this.Controls.Add(this.button_startStop);
            this.Controls.Add(this.numericUpDown_port);
            this.Controls.Add(this.label_listeningPort);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(650, 522);
            this.Name = "MainForm";
            this.Text = "CU5 Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_port)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_period)).EndInit();
            this.groupBox_tripInfo.ResumeLayout(false);
            this.groupBox_tripInfo.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBoxLineInfo.ResumeLayout(false);
            this.groupBoxLineInfo.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_listeningPort;
        private System.Windows.Forms.NumericUpDown numericUpDown_port;
        private System.Windows.Forms.Button button_startStop;
        private System.Windows.Forms.Label label_listening;
        private System.Windows.Forms.TextBox textBox_receivedText;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.TextBox textBox_internalMessages;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label_internalMessages;
        private System.Windows.Forms.Label label_periodicSend;
        private System.Windows.Forms.NumericUpDown numericUpDown_period;
        private System.Windows.Forms.Label label_ms;
        private System.Windows.Forms.Label label_lineNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox_tripInfo;
        private System.Windows.Forms.Label label_destArab;
        private System.Windows.Forms.Label label_destination;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadStateMachineToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxLineInfo;
        private System.Windows.Forms.Label labelLineNumber;
        private System.Windows.Forms.Label labelCurrentDirectionNo;
        private System.Windows.Forms.Label labelDestinationNo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelExi_DestinationArabic;
        private System.Windows.Forms.Label labelExi_Destination;
        private System.Windows.Forms.Label labelExi_LineNo;
        private System.Windows.Forms.Label labelExi_CurrentDirectionNo;
        private System.Windows.Forms.Label labelExi_DestinationNo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelCountdownNumber;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelSpecialInputState;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBoxStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox comboBoxDisplayStateRear;
        private System.Windows.Forms.ComboBox comboBoxDisplayStateRight;
        private System.Windows.Forms.ComboBox comboBoxDisplayStateLeft;
        private System.Windows.Forms.ComboBox comboBoxDisplayStateFront;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
    }
}

