namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    partial class TelegramControl
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
            System.Windows.Forms.GroupBox groupBox1;
            this.buttonResend = new System.Windows.Forms.Button();
            this.textBoxTelegram = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.telegramCreationControl1 = new Gorba.Motion.Protran.Controls.TelegramCreationControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonStep = new System.Windows.Forms.Button();
            this.checkBoxStepTime = new System.Windows.Forms.CheckBox();
            this.numericUpDownStepTime = new System.Windows.Forms.NumericUpDown();
            this.checkBoxIgnoreUnknown = new System.Windows.Forms.CheckBox();
            this.listBoxFileTelegrams = new System.Windows.Forms.ListBox();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonStopIbisBus = new System.Windows.Forms.Button();
            this.buttonStartIbisBus = new System.Windows.Forms.Button();
            this.listBoxIbisBusTelegrams = new System.Windows.Forms.ListBox();
            this.openFileDialogRecordings = new System.Windows.Forms.OpenFileDialog();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStepTime)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(this.buttonResend);
            groupBox1.Controls.Add(this.textBoxTelegram);
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(536, 48);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Last Telegram";
            // 
            // buttonResend
            // 
            this.buttonResend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonResend.Location = new System.Drawing.Point(455, 19);
            this.buttonResend.Name = "buttonResend";
            this.buttonResend.Size = new System.Drawing.Size(75, 23);
            this.buttonResend.TabIndex = 1;
            this.buttonResend.Text = "Resend";
            this.buttonResend.UseVisualStyleBackColor = true;
            this.buttonResend.Click += new System.EventHandler(this.OnButtonResendClick);
            // 
            // textBoxTelegram
            // 
            this.textBoxTelegram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTelegram.Location = new System.Drawing.Point(6, 21);
            this.textBoxTelegram.Name = "textBoxTelegram";
            this.textBoxTelegram.Size = new System.Drawing.Size(443, 20);
            this.textBoxTelegram.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, 57);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(536, 328);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.telegramCreationControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(528, 302);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Manual";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // telegramCreationControl1
            // 
            this.telegramCreationControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.telegramCreationControl1.Location = new System.Drawing.Point(3, 3);
            this.telegramCreationControl1.Name = "telegramCreationControl1";
            this.telegramCreationControl1.Size = new System.Drawing.Size(522, 296);
            this.telegramCreationControl1.TabIndex = 0;
            this.telegramCreationControl1.IbisTelegramCreated += new System.EventHandler<Gorba.Motion.Protran.Controls.DataEventArgs>(this.OnIbisTelegramCreated);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonStep);
            this.tabPage2.Controls.Add(this.checkBoxStepTime);
            this.tabPage2.Controls.Add(this.numericUpDownStepTime);
            this.tabPage2.Controls.Add(this.checkBoxIgnoreUnknown);
            this.tabPage2.Controls.Add(this.listBoxFileTelegrams);
            this.tabPage2.Controls.Add(this.buttonReset);
            this.tabPage2.Controls.Add(this.buttonPause);
            this.tabPage2.Controls.Add(this.buttonPlay);
            this.tabPage2.Controls.Add(this.buttonOpenFile);
            this.tabPage2.Controls.Add(this.textBoxFileName);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(528, 302);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "File";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonStep
            // 
            this.buttonStep.Enabled = false;
            this.buttonStep.Location = new System.Drawing.Point(7, 145);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 23);
            this.buttonStep.TabIndex = 7;
            this.buttonStep.Text = "Step";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.Click += new System.EventHandler(this.ButtonStepClick);
            // 
            // checkBoxStepTime
            // 
            this.checkBoxStepTime.AutoSize = true;
            this.checkBoxStepTime.Location = new System.Drawing.Point(173, 34);
            this.checkBoxStepTime.Name = "checkBoxStepTime";
            this.checkBoxStepTime.Size = new System.Drawing.Size(123, 17);
            this.checkBoxStepTime.TabIndex = 6;
            this.checkBoxStepTime.Text = "Use this step time (s)";
            this.checkBoxStepTime.UseVisualStyleBackColor = true;
            this.checkBoxStepTime.CheckedChanged += new System.EventHandler(this.CheckBoxStepTimeCheckedChanged);
            // 
            // numericUpDownStepTime
            // 
            this.numericUpDownStepTime.DecimalPlaces = 1;
            this.numericUpDownStepTime.Enabled = false;
            this.numericUpDownStepTime.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownStepTime.Location = new System.Drawing.Point(299, 34);
            this.numericUpDownStepTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownStepTime.Name = "numericUpDownStepTime";
            this.numericUpDownStepTime.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownStepTime.TabIndex = 5;
            this.numericUpDownStepTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // checkBoxIgnoreUnknown
            // 
            this.checkBoxIgnoreUnknown.AutoSize = true;
            this.checkBoxIgnoreUnknown.Checked = true;
            this.checkBoxIgnoreUnknown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreUnknown.Location = new System.Drawing.Point(6, 34);
            this.checkBoxIgnoreUnknown.Name = "checkBoxIgnoreUnknown";
            this.checkBoxIgnoreUnknown.Size = new System.Drawing.Size(151, 17);
            this.checkBoxIgnoreUnknown.TabIndex = 4;
            this.checkBoxIgnoreUnknown.Text = "Ignore unknown telegrams";
            this.checkBoxIgnoreUnknown.UseVisualStyleBackColor = true;
            // 
            // listBoxFileTelegrams
            // 
            this.listBoxFileTelegrams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxFileTelegrams.FormattingEnabled = true;
            this.listBoxFileTelegrams.IntegralHeight = false;
            this.listBoxFileTelegrams.Location = new System.Drawing.Point(87, 57);
            this.listBoxFileTelegrams.Name = "listBoxFileTelegrams";
            this.listBoxFileTelegrams.Size = new System.Drawing.Size(435, 239);
            this.listBoxFileTelegrams.TabIndex = 3;
            this.listBoxFileTelegrams.DoubleClick += new System.EventHandler(this.ListBoxFileTelegramsDoubleClick);
            // 
            // buttonReset
            // 
            this.buttonReset.Enabled = false;
            this.buttonReset.Location = new System.Drawing.Point(6, 115);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 2;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.ButtonResetClick);
            // 
            // buttonPause
            // 
            this.buttonPause.Enabled = false;
            this.buttonPause.Location = new System.Drawing.Point(6, 86);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(75, 23);
            this.buttonPause.TabIndex = 2;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.ButtonPauseClick);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Location = new System.Drawing.Point(6, 57);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(75, 23);
            this.buttonPlay.TabIndex = 2;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.ButtonPlayClick);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenFile.Location = new System.Drawing.Point(447, 6);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenFile.TabIndex = 1;
            this.buttonOpenFile.Text = "Open...";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.ButtonOpenFileClick);
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFileName.Location = new System.Drawing.Point(6, 8);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.ReadOnly = true;
            this.textBoxFileName.Size = new System.Drawing.Size(435, 20);
            this.textBoxFileName.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buttonClear);
            this.tabPage3.Controls.Add(this.buttonStopIbisBus);
            this.tabPage3.Controls.Add(this.buttonStartIbisBus);
            this.tabPage3.Controls.Add(this.listBoxIbisBusTelegrams);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(528, 302);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "IBIS Wagenbus";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(4, 94);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 7;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClearClick);
            // 
            // buttonStopIbisBus
            // 
            this.buttonStopIbisBus.Location = new System.Drawing.Point(3, 53);
            this.buttonStopIbisBus.Name = "buttonStopIbisBus";
            this.buttonStopIbisBus.Size = new System.Drawing.Size(75, 23);
            this.buttonStopIbisBus.TabIndex = 6;
            this.buttonStopIbisBus.Text = "Stop IbisBus";
            this.buttonStopIbisBus.UseVisualStyleBackColor = true;
            this.buttonStopIbisBus.Click += new System.EventHandler(this.ButtonStopIbisBusClick);
            // 
            // buttonStartIbisBus
            // 
            this.buttonStartIbisBus.Location = new System.Drawing.Point(3, 15);
            this.buttonStartIbisBus.Name = "buttonStartIbisBus";
            this.buttonStartIbisBus.Size = new System.Drawing.Size(75, 23);
            this.buttonStartIbisBus.TabIndex = 5;
            this.buttonStartIbisBus.Text = "Start IbisBus";
            this.buttonStartIbisBus.UseVisualStyleBackColor = true;
            this.buttonStartIbisBus.Click += new System.EventHandler(this.ButtonStartIbisBusClick);
            // 
            // listBoxIbisBusTelegrams
            // 
            this.listBoxIbisBusTelegrams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxIbisBusTelegrams.FormattingEnabled = true;
            this.listBoxIbisBusTelegrams.IntegralHeight = false;
            this.listBoxIbisBusTelegrams.Location = new System.Drawing.Point(84, 15);
            this.listBoxIbisBusTelegrams.Name = "listBoxIbisBusTelegrams";
            this.listBoxIbisBusTelegrams.Size = new System.Drawing.Size(430, 272);
            this.listBoxIbisBusTelegrams.TabIndex = 4;
            this.listBoxIbisBusTelegrams.DoubleClick += new System.EventHandler(this.ListBoxIbisBusTelegramsDoubleClick);
            // 
            // openFileDialogRecordings
            // 
            this.openFileDialogRecordings.Filter = "All supported files (*.ism;*.pro.csv;*.log;*.txt)|*.ism;*.pro.csv;*.log;*.txt|ISM" +
    " Logs (*.ism)|*.ism|Wagenbus Monitor Logs (*.pro.csv)|*.pro.csv|Protran Logs (*." +
    "log;*.txt)|*.log;*.txt";
            this.openFileDialogRecordings.Title = "Open IBIS Recording";
            // 
            // TelegramControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(groupBox1);
            this.Name = "TelegramControl";
            this.Size = new System.Drawing.Size(542, 388);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStepTime)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxTelegram;
        private System.Windows.Forms.Button buttonResend;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Protran.Controls.TelegramCreationControl telegramCreationControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox listBoxFileTelegrams;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialogRecordings;
        private System.Windows.Forms.CheckBox checkBoxIgnoreUnknown;
        private System.Windows.Forms.CheckBox checkBoxStepTime;
        private System.Windows.Forms.NumericUpDown numericUpDownStepTime;
        private System.Windows.Forms.Button buttonStep;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox listBoxIbisBusTelegrams;
        private System.Windows.Forms.Button buttonStartIbisBus;
        private System.Windows.Forms.Button buttonStopIbisBus;
        private System.Windows.Forms.Button buttonClear;

    }
}
