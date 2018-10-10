namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Forms
{
    partial class StateMachineUi
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
            this.groupBox_events = new System.Windows.Forms.GroupBox();
            this.button_clearAllFiles = new System.Windows.Forms.Button();
            this.button_addFileToDownload = new System.Windows.Forms.Button();
            this.button_downloadStart = new System.Windows.Forms.Button();
            this.label_downloadStart = new System.Windows.Forms.Label();
            this.comboBox_downloadStart = new System.Windows.Forms.ComboBox();
            this.label_downloadProgressRequest = new System.Windows.Forms.Label();
            this.button_downloadProgressRequest = new System.Windows.Forms.Button();
            this.label_downloadAbort = new System.Windows.Forms.Label();
            this.button_sendDownloadAbort = new System.Windows.Forms.Button();
            this.groupBox_errorEvents = new System.Windows.Forms.GroupBox();
            this.numericUpDown_fileIndexErrorCode = new System.Windows.Forms.NumericUpDown();
            this.label_fileIndexErrorCode = new System.Windows.Forms.Label();
            this.button_sendErrorCode = new System.Windows.Forms.Button();
            this.comboBox_errorCode = new System.Windows.Forms.ComboBox();
            this.label_errorCode = new System.Windows.Forms.Label();
            this.groupBox_downloadProgress = new System.Windows.Forms.GroupBox();
            this.button_clearFiles = new System.Windows.Forms.Button();
            this.groupBox_file4 = new System.Windows.Forms.GroupBox();
            this.label_file4_name = new System.Windows.Forms.Label();
            this.groupBox_file3 = new System.Windows.Forms.GroupBox();
            this.label_file3_name = new System.Windows.Forms.Label();
            this.groupBox_file2 = new System.Windows.Forms.GroupBox();
            this.label_file2_name = new System.Windows.Forms.Label();
            this.groupBox_file1 = new System.Windows.Forms.GroupBox();
            this.label_file1_name = new System.Windows.Forms.Label();
            this.groupBox_file0 = new System.Windows.Forms.GroupBox();
            this.label_file0_name = new System.Windows.Forms.Label();
            this.textBox_tripletsSent = new System.Windows.Forms.TextBox();
            this.label_tripletsSent = new System.Windows.Forms.Label();
            this.textBox_lastState = new System.Windows.Forms.TextBox();
            this.textBox_currentState = new System.Windows.Forms.TextBox();
            this.textBox_nextStates = new System.Windows.Forms.TextBox();
            this.label_previousState = new System.Windows.Forms.Label();
            this.label_currentState = new System.Windows.Forms.Label();
            this.label_nextStates = new System.Windows.Forms.Label();
            this.textBox_messages = new System.Windows.Forms.TextBox();
            this.button_clearTripletsSent = new System.Windows.Forms.Button();
            this.textBox_crc = new System.Windows.Forms.TextBox();
            this.label_fileNameForCrc = new System.Windows.Forms.Label();
            this.label_crc = new System.Windows.Forms.Label();
            this.label_crcValue = new System.Windows.Forms.Label();
            this.button_calculateCrc = new System.Windows.Forms.Button();
            this.groupBox_events.SuspendLayout();
            this.groupBox_errorEvents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_fileIndexErrorCode)).BeginInit();
            this.groupBox_downloadProgress.SuspendLayout();
            this.groupBox_file4.SuspendLayout();
            this.groupBox_file3.SuspendLayout();
            this.groupBox_file2.SuspendLayout();
            this.groupBox_file1.SuspendLayout();
            this.groupBox_file0.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_events
            // 
            this.groupBox_events.Controls.Add(this.button_clearAllFiles);
            this.groupBox_events.Controls.Add(this.button_addFileToDownload);
            this.groupBox_events.Controls.Add(this.button_downloadStart);
            this.groupBox_events.Controls.Add(this.label_downloadStart);
            this.groupBox_events.Controls.Add(this.comboBox_downloadStart);
            this.groupBox_events.Controls.Add(this.label_downloadProgressRequest);
            this.groupBox_events.Controls.Add(this.button_downloadProgressRequest);
            this.groupBox_events.Controls.Add(this.label_downloadAbort);
            this.groupBox_events.Controls.Add(this.button_sendDownloadAbort);
            this.groupBox_events.Location = new System.Drawing.Point(9, 460);
            this.groupBox_events.Name = "groupBox_events";
            this.groupBox_events.Size = new System.Drawing.Size(388, 128);
            this.groupBox_events.TabIndex = 0;
            this.groupBox_events.TabStop = false;
            this.groupBox_events.Text = "CTU Datagrams:";
            // 
            // button_clearAllFiles
            // 
            this.button_clearAllFiles.Location = new System.Drawing.Point(183, 97);
            this.button_clearAllFiles.Name = "button_clearAllFiles";
            this.button_clearAllFiles.Size = new System.Drawing.Size(49, 23);
            this.button_clearAllFiles.TabIndex = 8;
            this.button_clearAllFiles.Text = "Clear";
            this.button_clearAllFiles.UseVisualStyleBackColor = true;
            this.button_clearAllFiles.Click += new System.EventHandler(this.ButtonClearAllFilesClick);
            // 
            // button_addFileToDownload
            // 
            this.button_addFileToDownload.Location = new System.Drawing.Point(128, 97);
            this.button_addFileToDownload.Name = "button_addFileToDownload";
            this.button_addFileToDownload.Size = new System.Drawing.Size(49, 23);
            this.button_addFileToDownload.TabIndex = 7;
            this.button_addFileToDownload.Text = "Add";
            this.button_addFileToDownload.UseVisualStyleBackColor = true;
            this.button_addFileToDownload.Click += new System.EventHandler(this.ButtonAddFileToDownloadClick);
            // 
            // button_downloadStart
            // 
            this.button_downloadStart.Location = new System.Drawing.Point(333, 68);
            this.button_downloadStart.Name = "button_downloadStart";
            this.button_downloadStart.Size = new System.Drawing.Size(49, 23);
            this.button_downloadStart.TabIndex = 6;
            this.button_downloadStart.Text = "Send";
            this.button_downloadStart.UseVisualStyleBackColor = true;
            this.button_downloadStart.Click += new System.EventHandler(this.SendCtuDatagramClick);
            // 
            // label_downloadStart
            // 
            this.label_downloadStart.AutoSize = true;
            this.label_downloadStart.Location = new System.Drawing.Point(7, 73);
            this.label_downloadStart.Name = "label_downloadStart";
            this.label_downloadStart.Size = new System.Drawing.Size(80, 13);
            this.label_downloadStart.TabIndex = 5;
            this.label_downloadStart.Text = "Download Start";
            // 
            // comboBox_downloadStart
            // 
            this.comboBox_downloadStart.FormattingEnabled = true;
            this.comboBox_downloadStart.Items.AddRange(new object[] {
            "7981754,0x3E2D3B02,File1.zip",
            "21073936,0xBC8A3150,File2.exe",
            "17815040,0xCE1D9E8D,File3.msi"});
            this.comboBox_downloadStart.Location = new System.Drawing.Point(93, 70);
            this.comboBox_downloadStart.Name = "comboBox_downloadStart";
            this.comboBox_downloadStart.Size = new System.Drawing.Size(180, 21);
            this.comboBox_downloadStart.TabIndex = 4;
            // 
            // label_downloadProgressRequest
            // 
            this.label_downloadProgressRequest.AutoSize = true;
            this.label_downloadProgressRequest.Location = new System.Drawing.Point(7, 46);
            this.label_downloadProgressRequest.Name = "label_downloadProgressRequest";
            this.label_downloadProgressRequest.Size = new System.Drawing.Size(142, 13);
            this.label_downloadProgressRequest.TabIndex = 3;
            this.label_downloadProgressRequest.Text = "Download Progress Request";
            // 
            // button_downloadProgressRequest
            // 
            this.button_downloadProgressRequest.Location = new System.Drawing.Point(333, 41);
            this.button_downloadProgressRequest.Name = "button_downloadProgressRequest";
            this.button_downloadProgressRequest.Size = new System.Drawing.Size(49, 23);
            this.button_downloadProgressRequest.TabIndex = 2;
            this.button_downloadProgressRequest.Text = "Send";
            this.button_downloadProgressRequest.UseVisualStyleBackColor = true;
            this.button_downloadProgressRequest.Click += new System.EventHandler(this.SendCtuDatagramClick);
            // 
            // label_downloadAbort
            // 
            this.label_downloadAbort.AutoSize = true;
            this.label_downloadAbort.Location = new System.Drawing.Point(7, 20);
            this.label_downloadAbort.Name = "label_downloadAbort";
            this.label_downloadAbort.Size = new System.Drawing.Size(83, 13);
            this.label_downloadAbort.TabIndex = 1;
            this.label_downloadAbort.Text = "Download Abort";
            // 
            // button_sendDownloadAbort
            // 
            this.button_sendDownloadAbort.Location = new System.Drawing.Point(333, 15);
            this.button_sendDownloadAbort.Name = "button_sendDownloadAbort";
            this.button_sendDownloadAbort.Size = new System.Drawing.Size(49, 23);
            this.button_sendDownloadAbort.TabIndex = 0;
            this.button_sendDownloadAbort.Text = "Send";
            this.button_sendDownloadAbort.UseVisualStyleBackColor = true;
            this.button_sendDownloadAbort.Click += new System.EventHandler(this.SendCtuDatagramClick);
            // 
            // groupBox_errorEvents
            // 
            this.groupBox_errorEvents.Controls.Add(this.numericUpDown_fileIndexErrorCode);
            this.groupBox_errorEvents.Controls.Add(this.label_fileIndexErrorCode);
            this.groupBox_errorEvents.Controls.Add(this.button_sendErrorCode);
            this.groupBox_errorEvents.Controls.Add(this.comboBox_errorCode);
            this.groupBox_errorEvents.Controls.Add(this.label_errorCode);
            this.groupBox_errorEvents.Location = new System.Drawing.Point(9, 408);
            this.groupBox_errorEvents.Name = "groupBox_errorEvents";
            this.groupBox_errorEvents.Size = new System.Drawing.Size(382, 46);
            this.groupBox_errorEvents.TabIndex = 1;
            this.groupBox_errorEvents.TabStop = false;
            this.groupBox_errorEvents.Text = "Other Events:";
            // 
            // numericUpDown_fileIndexErrorCode
            // 
            this.numericUpDown_fileIndexErrorCode.Location = new System.Drawing.Point(64, 15);
            this.numericUpDown_fileIndexErrorCode.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_fileIndexErrorCode.Name = "numericUpDown_fileIndexErrorCode";
            this.numericUpDown_fileIndexErrorCode.Size = new System.Drawing.Size(55, 20);
            this.numericUpDown_fileIndexErrorCode.TabIndex = 9;
            this.numericUpDown_fileIndexErrorCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_fileIndexErrorCode
            // 
            this.label_fileIndexErrorCode.AutoSize = true;
            this.label_fileIndexErrorCode.Location = new System.Drawing.Point(6, 18);
            this.label_fileIndexErrorCode.Name = "label_fileIndexErrorCode";
            this.label_fileIndexErrorCode.Size = new System.Drawing.Size(55, 13);
            this.label_fileIndexErrorCode.TabIndex = 8;
            this.label_fileIndexErrorCode.Text = "File Index:";
            // 
            // button_sendErrorCode
            // 
            this.button_sendErrorCode.Location = new System.Drawing.Point(327, 12);
            this.button_sendErrorCode.Name = "button_sendErrorCode";
            this.button_sendErrorCode.Size = new System.Drawing.Size(49, 23);
            this.button_sendErrorCode.TabIndex = 7;
            this.button_sendErrorCode.Text = "Send";
            this.button_sendErrorCode.UseVisualStyleBackColor = true;
            this.button_sendErrorCode.Click += new System.EventHandler(this.ButtonSendErrorCodeClick);
            // 
            // comboBox_errorCode
            // 
            this.comboBox_errorCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_errorCode.FormattingEnabled = true;
            this.comboBox_errorCode.Items.AddRange(new object[] {
            "GeneralError",
            "LowMemory",
            "ConnectionError"});
            this.comboBox_errorCode.Location = new System.Drawing.Point(191, 14);
            this.comboBox_errorCode.Name = "comboBox_errorCode";
            this.comboBox_errorCode.Size = new System.Drawing.Size(82, 21);
            this.comboBox_errorCode.TabIndex = 3;
            // 
            // label_errorCode
            // 
            this.label_errorCode.AutoSize = true;
            this.label_errorCode.Location = new System.Drawing.Point(125, 18);
            this.label_errorCode.Name = "label_errorCode";
            this.label_errorCode.Size = new System.Drawing.Size(60, 13);
            this.label_errorCode.TabIndex = 2;
            this.label_errorCode.Text = "Error Code:";
            // 
            // groupBox_downloadProgress
            // 
            this.groupBox_downloadProgress.Controls.Add(this.button_clearFiles);
            this.groupBox_downloadProgress.Controls.Add(this.groupBox_file4);
            this.groupBox_downloadProgress.Controls.Add(this.groupBox_file3);
            this.groupBox_downloadProgress.Controls.Add(this.groupBox_file2);
            this.groupBox_downloadProgress.Controls.Add(this.groupBox_file1);
            this.groupBox_downloadProgress.Controls.Add(this.groupBox_file0);
            this.groupBox_downloadProgress.Location = new System.Drawing.Point(426, 129);
            this.groupBox_downloadProgress.Name = "groupBox_downloadProgress";
            this.groupBox_downloadProgress.Size = new System.Drawing.Size(411, 260);
            this.groupBox_downloadProgress.TabIndex = 2;
            this.groupBox_downloadProgress.TabStop = false;
            this.groupBox_downloadProgress.Text = "Downloads Progress:";
            // 
            // button_clearFiles
            // 
            this.button_clearFiles.Location = new System.Drawing.Point(358, 231);
            this.button_clearFiles.Name = "button_clearFiles";
            this.button_clearFiles.Size = new System.Drawing.Size(47, 23);
            this.button_clearFiles.TabIndex = 8;
            this.button_clearFiles.Text = "Clear";
            this.button_clearFiles.UseVisualStyleBackColor = true;
            this.button_clearFiles.Click += new System.EventHandler(this.ButtonClearFilesClick);
            // 
            // groupBox_file4
            // 
            this.groupBox_file4.Controls.Add(this.label_file4_name);
            this.groupBox_file4.Location = new System.Drawing.Point(9, 179);
            this.groupBox_file4.Name = "groupBox_file4";
            this.groupBox_file4.Size = new System.Drawing.Size(396, 34);
            this.groupBox_file4.TabIndex = 7;
            this.groupBox_file4.TabStop = false;
            this.groupBox_file4.Text = "File 4th:";
            // 
            // label_file4_name
            // 
            this.label_file4_name.AutoSize = true;
            this.label_file4_name.Location = new System.Drawing.Point(6, 16);
            this.label_file4_name.Name = "label_file4_name";
            this.label_file4_name.Size = new System.Drawing.Size(35, 13);
            this.label_file4_name.TabIndex = 0;
            this.label_file4_name.Text = "Name";
            // 
            // groupBox_file3
            // 
            this.groupBox_file3.Controls.Add(this.label_file3_name);
            this.groupBox_file3.Location = new System.Drawing.Point(9, 139);
            this.groupBox_file3.Name = "groupBox_file3";
            this.groupBox_file3.Size = new System.Drawing.Size(396, 34);
            this.groupBox_file3.TabIndex = 6;
            this.groupBox_file3.TabStop = false;
            this.groupBox_file3.Text = "File 3rd:";
            // 
            // label_file3_name
            // 
            this.label_file3_name.AutoSize = true;
            this.label_file3_name.Location = new System.Drawing.Point(6, 16);
            this.label_file3_name.Name = "label_file3_name";
            this.label_file3_name.Size = new System.Drawing.Size(35, 13);
            this.label_file3_name.TabIndex = 0;
            this.label_file3_name.Text = "Name";
            // 
            // groupBox_file2
            // 
            this.groupBox_file2.Controls.Add(this.label_file2_name);
            this.groupBox_file2.Location = new System.Drawing.Point(9, 99);
            this.groupBox_file2.Name = "groupBox_file2";
            this.groupBox_file2.Size = new System.Drawing.Size(396, 34);
            this.groupBox_file2.TabIndex = 5;
            this.groupBox_file2.TabStop = false;
            this.groupBox_file2.Text = "File 2nd:";
            // 
            // label_file2_name
            // 
            this.label_file2_name.AutoSize = true;
            this.label_file2_name.Location = new System.Drawing.Point(6, 16);
            this.label_file2_name.Name = "label_file2_name";
            this.label_file2_name.Size = new System.Drawing.Size(35, 13);
            this.label_file2_name.TabIndex = 0;
            this.label_file2_name.Text = "Name";
            // 
            // groupBox_file1
            // 
            this.groupBox_file1.Controls.Add(this.label_file1_name);
            this.groupBox_file1.Location = new System.Drawing.Point(10, 59);
            this.groupBox_file1.Name = "groupBox_file1";
            this.groupBox_file1.Size = new System.Drawing.Size(395, 34);
            this.groupBox_file1.TabIndex = 4;
            this.groupBox_file1.TabStop = false;
            this.groupBox_file1.Text = "File 1st:";
            // 
            // label_file1_name
            // 
            this.label_file1_name.AutoSize = true;
            this.label_file1_name.Location = new System.Drawing.Point(6, 16);
            this.label_file1_name.Name = "label_file1_name";
            this.label_file1_name.Size = new System.Drawing.Size(35, 13);
            this.label_file1_name.TabIndex = 0;
            this.label_file1_name.Text = "Name";
            // 
            // groupBox_file0
            // 
            this.groupBox_file0.Controls.Add(this.label_file0_name);
            this.groupBox_file0.Location = new System.Drawing.Point(10, 19);
            this.groupBox_file0.Name = "groupBox_file0";
            this.groupBox_file0.Size = new System.Drawing.Size(395, 34);
            this.groupBox_file0.TabIndex = 3;
            this.groupBox_file0.TabStop = false;
            this.groupBox_file0.Text = "File 0th:";
            // 
            // label_file0_name
            // 
            this.label_file0_name.AutoSize = true;
            this.label_file0_name.Location = new System.Drawing.Point(6, 16);
            this.label_file0_name.Name = "label_file0_name";
            this.label_file0_name.Size = new System.Drawing.Size(35, 13);
            this.label_file0_name.TabIndex = 0;
            this.label_file0_name.Text = "Name";
            // 
            // textBox_tripletsSent
            // 
            this.textBox_tripletsSent.Location = new System.Drawing.Point(9, 145);
            this.textBox_tripletsSent.Multiline = true;
            this.textBox_tripletsSent.Name = "textBox_tripletsSent";
            this.textBox_tripletsSent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_tripletsSent.Size = new System.Drawing.Size(411, 209);
            this.textBox_tripletsSent.TabIndex = 4;
            // 
            // label_tripletsSent
            // 
            this.label_tripletsSent.AutoSize = true;
            this.label_tripletsSent.Location = new System.Drawing.Point(9, 129);
            this.label_tripletsSent.Name = "label_tripletsSent";
            this.label_tripletsSent.Size = new System.Drawing.Size(69, 13);
            this.label_tripletsSent.TabIndex = 6;
            this.label_tripletsSent.Text = "Triplets Sent:";
            // 
            // textBox_lastState
            // 
            this.textBox_lastState.Location = new System.Drawing.Point(9, 28);
            this.textBox_lastState.Multiline = true;
            this.textBox_lastState.Name = "textBox_lastState";
            this.textBox_lastState.ReadOnly = true;
            this.textBox_lastState.Size = new System.Drawing.Size(224, 20);
            this.textBox_lastState.TabIndex = 7;
            this.textBox_lastState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_currentState
            // 
            this.textBox_currentState.Location = new System.Drawing.Point(233, 28);
            this.textBox_currentState.Multiline = true;
            this.textBox_currentState.Name = "textBox_currentState";
            this.textBox_currentState.ReadOnly = true;
            this.textBox_currentState.Size = new System.Drawing.Size(224, 20);
            this.textBox_currentState.TabIndex = 8;
            this.textBox_currentState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_nextStates
            // 
            this.textBox_nextStates.Location = new System.Drawing.Point(457, 28);
            this.textBox_nextStates.Multiline = true;
            this.textBox_nextStates.Name = "textBox_nextStates";
            this.textBox_nextStates.ReadOnly = true;
            this.textBox_nextStates.Size = new System.Drawing.Size(224, 100);
            this.textBox_nextStates.TabIndex = 9;
            this.textBox_nextStates.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_previousState
            // 
            this.label_previousState.AutoSize = true;
            this.label_previousState.Location = new System.Drawing.Point(77, 9);
            this.label_previousState.Name = "label_previousState";
            this.label_previousState.Size = new System.Drawing.Size(76, 13);
            this.label_previousState.TabIndex = 10;
            this.label_previousState.Text = "Previous State";
            // 
            // label_currentState
            // 
            this.label_currentState.AutoSize = true;
            this.label_currentState.Location = new System.Drawing.Point(309, 9);
            this.label_currentState.Name = "label_currentState";
            this.label_currentState.Size = new System.Drawing.Size(69, 13);
            this.label_currentState.TabIndex = 11;
            this.label_currentState.Text = "Current State";
            // 
            // label_nextStates
            // 
            this.label_nextStates.AutoSize = true;
            this.label_nextStates.Location = new System.Drawing.Point(537, 9);
            this.label_nextStates.Name = "label_nextStates";
            this.label_nextStates.Size = new System.Drawing.Size(62, 13);
            this.label_nextStates.TabIndex = 12;
            this.label_nextStates.Text = "Next States";
            // 
            // textBox_messages
            // 
            this.textBox_messages.Location = new System.Drawing.Point(9, 55);
            this.textBox_messages.Multiline = true;
            this.textBox_messages.Name = "textBox_messages";
            this.textBox_messages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_messages.Size = new System.Drawing.Size(442, 71);
            this.textBox_messages.TabIndex = 13;
            // 
            // button_clearTripletsSent
            // 
            this.button_clearTripletsSent.Location = new System.Drawing.Point(373, 360);
            this.button_clearTripletsSent.Name = "button_clearTripletsSent";
            this.button_clearTripletsSent.Size = new System.Drawing.Size(47, 23);
            this.button_clearTripletsSent.TabIndex = 14;
            this.button_clearTripletsSent.Text = "Clear";
            this.button_clearTripletsSent.UseVisualStyleBackColor = true;
            this.button_clearTripletsSent.Click += new System.EventHandler(this.ButtonClearTripletsSentClick);
            // 
            // textBox_crc
            // 
            this.textBox_crc.Location = new System.Drawing.Point(688, 28);
            this.textBox_crc.Name = "textBox_crc";
            this.textBox_crc.Size = new System.Drawing.Size(146, 20);
            this.textBox_crc.TabIndex = 15;
            // 
            // label_fileNameForCrc
            // 
            this.label_fileNameForCrc.AutoSize = true;
            this.label_fileNameForCrc.Location = new System.Drawing.Point(685, 9);
            this.label_fileNameForCrc.Name = "label_fileNameForCrc";
            this.label_fileNameForCrc.Size = new System.Drawing.Size(26, 13);
            this.label_fileNameForCrc.TabIndex = 16;
            this.label_fileNameForCrc.Text = "File:";
            // 
            // label_crc
            // 
            this.label_crc.AutoSize = true;
            this.label_crc.Location = new System.Drawing.Point(688, 55);
            this.label_crc.Name = "label_crc";
            this.label_crc.Size = new System.Drawing.Size(32, 13);
            this.label_crc.TabIndex = 17;
            this.label_crc.Text = "CRC:";
            // 
            // label_crcValue
            // 
            this.label_crcValue.AutoSize = true;
            this.label_crcValue.Location = new System.Drawing.Point(726, 55);
            this.label_crcValue.Name = "label_crcValue";
            this.label_crcValue.Size = new System.Drawing.Size(0, 13);
            this.label_crcValue.TabIndex = 18;
            // 
            // button_calculateCrc
            // 
            this.button_calculateCrc.Location = new System.Drawing.Point(688, 71);
            this.button_calculateCrc.Name = "button_calculateCrc";
            this.button_calculateCrc.Size = new System.Drawing.Size(75, 23);
            this.button_calculateCrc.TabIndex = 19;
            this.button_calculateCrc.Text = "Calculate";
            this.button_calculateCrc.UseVisualStyleBackColor = true;
            this.button_calculateCrc.Click += new System.EventHandler(this.ButtonCalculateCrcClick);
            // 
            // StateMachineUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 403);
            this.Controls.Add(this.button_calculateCrc);
            this.Controls.Add(this.label_crcValue);
            this.Controls.Add(this.label_crc);
            this.Controls.Add(this.label_fileNameForCrc);
            this.Controls.Add(this.textBox_crc);
            this.Controls.Add(this.button_clearTripletsSent);
            this.Controls.Add(this.textBox_messages);
            this.Controls.Add(this.label_nextStates);
            this.Controls.Add(this.label_currentState);
            this.Controls.Add(this.label_previousState);
            this.Controls.Add(this.textBox_nextStates);
            this.Controls.Add(this.textBox_currentState);
            this.Controls.Add(this.textBox_lastState);
            this.Controls.Add(this.label_tripletsSent);
            this.Controls.Add(this.textBox_tripletsSent);
            this.Controls.Add(this.groupBox_downloadProgress);
            this.Controls.Add(this.groupBox_errorEvents);
            this.Controls.Add(this.groupBox_events);
            this.Name = "StateMachineUi";
            this.Text = "State Machine Control";
            this.groupBox_events.ResumeLayout(false);
            this.groupBox_events.PerformLayout();
            this.groupBox_errorEvents.ResumeLayout(false);
            this.groupBox_errorEvents.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_fileIndexErrorCode)).EndInit();
            this.groupBox_downloadProgress.ResumeLayout(false);
            this.groupBox_file4.ResumeLayout(false);
            this.groupBox_file4.PerformLayout();
            this.groupBox_file3.ResumeLayout(false);
            this.groupBox_file3.PerformLayout();
            this.groupBox_file2.ResumeLayout(false);
            this.groupBox_file2.PerformLayout();
            this.groupBox_file1.ResumeLayout(false);
            this.groupBox_file1.PerformLayout();
            this.groupBox_file0.ResumeLayout(false);
            this.groupBox_file0.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_events;
        private System.Windows.Forms.Button button_downloadStart;
        private System.Windows.Forms.Label label_downloadStart;
        private System.Windows.Forms.ComboBox comboBox_downloadStart;
        private System.Windows.Forms.Label label_downloadProgressRequest;
        private System.Windows.Forms.Button button_downloadProgressRequest;
        private System.Windows.Forms.Label label_downloadAbort;
        private System.Windows.Forms.Button button_sendDownloadAbort;
        private System.Windows.Forms.GroupBox groupBox_errorEvents;
        private System.Windows.Forms.ComboBox comboBox_errorCode;
        private System.Windows.Forms.Label label_errorCode;
        private System.Windows.Forms.NumericUpDown numericUpDown_fileIndexErrorCode;
        private System.Windows.Forms.Label label_fileIndexErrorCode;
        private System.Windows.Forms.Button button_sendErrorCode;
        private System.Windows.Forms.GroupBox groupBox_downloadProgress;
        private System.Windows.Forms.GroupBox groupBox_file4;
        private System.Windows.Forms.Label label_file4_name;
        private System.Windows.Forms.GroupBox groupBox_file3;
        private System.Windows.Forms.Label label_file3_name;
        private System.Windows.Forms.GroupBox groupBox_file2;
        private System.Windows.Forms.Label label_file2_name;
        private System.Windows.Forms.GroupBox groupBox_file1;
        private System.Windows.Forms.Label label_file1_name;
        private System.Windows.Forms.GroupBox groupBox_file0;
        private System.Windows.Forms.Label label_file0_name;
        private System.Windows.Forms.Button button_addFileToDownload;
        private System.Windows.Forms.Button button_clearAllFiles;
        private System.Windows.Forms.TextBox textBox_tripletsSent;
        private System.Windows.Forms.Label label_tripletsSent;
        private System.Windows.Forms.TextBox textBox_lastState;
        private System.Windows.Forms.TextBox textBox_currentState;
        private System.Windows.Forms.TextBox textBox_nextStates;
        private System.Windows.Forms.Label label_previousState;
        private System.Windows.Forms.Label label_currentState;
        private System.Windows.Forms.Label label_nextStates;
        private System.Windows.Forms.TextBox textBox_messages;
        private System.Windows.Forms.Button button_clearFiles;
        private System.Windows.Forms.Button button_clearTripletsSent;
        private System.Windows.Forms.TextBox textBox_crc;
        private System.Windows.Forms.Label label_fileNameForCrc;
        private System.Windows.Forms.Label label_crc;
        private System.Windows.Forms.Label label_crcValue;
        private System.Windows.Forms.Button button_calculateCrc;
    }
}