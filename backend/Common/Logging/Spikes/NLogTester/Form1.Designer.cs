namespace NLogTester
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.buttonLog = new System.Windows.Forms.Button();
            this.comboBoxLevel = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxInfinite = new System.Windows.Forms.CheckBox();
            this.buttonStartStop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownInterval = new System.Windows.Forms.NumericUpDown();
            this.timerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.checkedListBoxLevels = new System.Windows.Forms.CheckedListBox();
            this.textBoxBatchMessage = new System.Windows.Forms.TextBox();
            this.timerBatchMessages = new System.Windows.Forms.Timer(this.components);
            this.textBoxCounter = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMessage.Location = new System.Drawing.Point(133, 21);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(381, 20);
            this.textBoxMessage.TabIndex = 0;
            this.textBoxMessage.Text = "Hello World";
            // 
            // buttonLog
            // 
            this.buttonLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLog.Location = new System.Drawing.Point(520, 19);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(75, 23);
            this.buttonLog.TabIndex = 1;
            this.buttonLog.Text = "Log";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.ButtonLogClick);
            // 
            // comboBoxLevel
            // 
            this.comboBoxLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLevel.FormattingEnabled = true;
            this.comboBoxLevel.Location = new System.Drawing.Point(6, 21);
            this.comboBoxLevel.Name = "comboBoxLevel";
            this.comboBoxLevel.Size = new System.Drawing.Size(121, 21);
            this.comboBoxLevel.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxLevel);
            this.groupBox1.Controls.Add(this.buttonLog);
            this.groupBox1.Controls.Add(this.textBoxMessage);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(601, 48);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Single Message";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxCounter);
            this.groupBox2.Controls.Add(this.checkBoxInfinite);
            this.groupBox2.Controls.Add(this.buttonStartStop);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.numericUpDownCount);
            this.groupBox2.Controls.Add(this.numericUpDownInterval);
            this.groupBox2.Controls.Add(this.checkedListBoxLevels);
            this.groupBox2.Controls.Add(this.textBoxBatchMessage);
            this.groupBox2.Location = new System.Drawing.Point(12, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(601, 126);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Batch Messages";
            // 
            // checkBoxInfinite
            // 
            this.checkBoxInfinite.AutoSize = true;
            this.checkBoxInfinite.Location = new System.Drawing.Point(270, 72);
            this.checkBoxInfinite.Name = "checkBoxInfinite";
            this.checkBoxInfinite.Size = new System.Drawing.Size(57, 17);
            this.checkBoxInfinite.TabIndex = 3;
            this.checkBoxInfinite.Text = "Infinite";
            this.checkBoxInfinite.UseVisualStyleBackColor = true;
            this.checkBoxInfinite.CheckedChanged += new System.EventHandler(this.CheckBoxInfiniteCheckedChanged);
            // 
            // buttonStartStop
            // 
            this.buttonStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartStop.Location = new System.Drawing.Point(520, 97);
            this.buttonStartStop.Name = "buttonStartStop";
            this.buttonStartStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStartStop.TabIndex = 1;
            this.buttonStartStop.Text = "Start";
            this.buttonStartStop.UseVisualStyleBackColor = true;
            this.buttonStartStop.Click += new System.EventHandler(this.ButtonStartStopClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "ms";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(133, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Count";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(133, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Interval";
            // 
            // numericUpDownCount
            // 
            this.numericUpDownCount.Location = new System.Drawing.Point(181, 71);
            this.numericUpDownCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownCount.Name = "numericUpDownCount";
            this.numericUpDownCount.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownCount.TabIndex = 1;
            this.numericUpDownCount.ThousandsSeparator = true;
            this.numericUpDownCount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numericUpDownInterval
            // 
            this.numericUpDownInterval.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.timerBindingSource, "Interval", true));
            this.numericUpDownInterval.Location = new System.Drawing.Point(181, 45);
            this.numericUpDownInterval.Maximum = new decimal(new int[] {
            3600000,
            0,
            0,
            0});
            this.numericUpDownInterval.Name = "numericUpDownInterval";
            this.numericUpDownInterval.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownInterval.TabIndex = 1;
            this.numericUpDownInterval.ThousandsSeparator = true;
            this.numericUpDownInterval.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // timerBindingSource
            // 
            this.timerBindingSource.DataSource = typeof(System.Windows.Forms.Timer);
            // 
            // checkedListBoxLevels
            // 
            this.checkedListBoxLevels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxLevels.FormattingEnabled = true;
            this.checkedListBoxLevels.IntegralHeight = false;
            this.checkedListBoxLevels.Location = new System.Drawing.Point(6, 19);
            this.checkedListBoxLevels.Name = "checkedListBoxLevels";
            this.checkedListBoxLevels.Size = new System.Drawing.Size(121, 101);
            this.checkedListBoxLevels.TabIndex = 0;
            this.checkedListBoxLevels.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxLevelsItemCheck);
            // 
            // textBoxBatchMessage
            // 
            this.textBoxBatchMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBatchMessage.Location = new System.Drawing.Point(133, 19);
            this.textBoxBatchMessage.Name = "textBoxBatchMessage";
            this.textBoxBatchMessage.Size = new System.Drawing.Size(381, 20);
            this.textBoxBatchMessage.TabIndex = 0;
            this.textBoxBatchMessage.Text = "Hello World #{0}";
            // 
            // timerBatchMessages
            // 
            this.timerBatchMessages.Tick += new System.EventHandler(this.TimerBatchMessagesTick);
            // 
            // textBoxCounter
            // 
            this.textBoxCounter.Location = new System.Drawing.Point(520, 71);
            this.textBoxCounter.Name = "textBoxCounter";
            this.textBoxCounter.ReadOnly = true;
            this.textBoxCounter.Size = new System.Drawing.Size(75, 20);
            this.textBoxCounter.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 345);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "NLog Tester";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timerBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Button buttonLog;
        private System.Windows.Forms.ComboBox comboBoxLevel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownInterval;
        private System.Windows.Forms.BindingSource timerBindingSource;
        private System.Windows.Forms.CheckedListBox checkedListBoxLevels;
        private System.Windows.Forms.TextBox textBoxBatchMessage;
        private System.Windows.Forms.Timer timerBatchMessages;
        private System.Windows.Forms.CheckBox checkBoxInfinite;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownCount;
        private System.Windows.Forms.Button buttonStartStop;
        private System.Windows.Forms.TextBox textBoxCounter;
    }
}

