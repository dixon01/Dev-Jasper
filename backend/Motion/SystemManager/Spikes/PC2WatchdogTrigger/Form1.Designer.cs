namespace PC2WatchdogTrigger
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
            this.timerWatchdog = new System.Windows.Forms.Timer(this.components);
            this.buttonOpen = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxLastTrigger = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonTrigger = new System.Windows.Forms.Button();
            this.checkBoxAutoTrigger = new System.Windows.Forms.CheckBox();
            this.timerUpdateLastTrigger = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerWatchdog
            // 
            this.timerWatchdog.Interval = 10000;
            this.timerWatchdog.Tick += new System.EventHandler(this.TimerWatchdogTick);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(12, 12);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 0;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.ButtonOpenClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBoxLastTrigger);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonTrigger);
            this.groupBox1.Controls.Add(this.checkBoxAutoTrigger);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 102);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trigger";
            // 
            // textBoxLastTrigger
            // 
            this.textBoxLastTrigger.Location = new System.Drawing.Point(81, 71);
            this.textBoxLastTrigger.Name = "textBoxLastTrigger";
            this.textBoxLastTrigger.ReadOnly = true;
            this.textBoxLastTrigger.Size = new System.Drawing.Size(77, 20);
            this.textBoxLastTrigger.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Last Trigger:";
            // 
            // buttonTrigger
            // 
            this.buttonTrigger.Location = new System.Drawing.Point(6, 42);
            this.buttonTrigger.Name = "buttonTrigger";
            this.buttonTrigger.Size = new System.Drawing.Size(125, 23);
            this.buttonTrigger.TabIndex = 1;
            this.buttonTrigger.Text = "Trigger Manually";
            this.buttonTrigger.UseVisualStyleBackColor = true;
            this.buttonTrigger.Click += new System.EventHandler(this.ButtonTriggerClick);
            // 
            // checkBoxAutoTrigger
            // 
            this.checkBoxAutoTrigger.AutoSize = true;
            this.checkBoxAutoTrigger.Location = new System.Drawing.Point(6, 19);
            this.checkBoxAutoTrigger.Name = "checkBoxAutoTrigger";
            this.checkBoxAutoTrigger.Size = new System.Drawing.Size(125, 17);
            this.checkBoxAutoTrigger.TabIndex = 0;
            this.checkBoxAutoTrigger.Text = "Auto-Trigger (10 sec)";
            this.checkBoxAutoTrigger.UseVisualStyleBackColor = true;
            this.checkBoxAutoTrigger.CheckedChanged += new System.EventHandler(this.CheckBoxAutoTriggerCheckedChanged);
            // 
            // timerUpdateLastTrigger
            // 
            this.timerUpdateLastTrigger.Interval = 200;
            this.timerUpdateLastTrigger.Tick += new System.EventHandler(this.TimerUpdateLastTriggerTick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 155);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonOpen);
            this.Name = "Form1";
            this.Text = "PC-2 Watchdog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerWatchdog;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxLastTrigger;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonTrigger;
        private System.Windows.Forms.CheckBox checkBoxAutoTrigger;
        private System.Windows.Forms.Timer timerUpdateLastTrigger;
    }
}

