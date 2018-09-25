namespace MyHyperTerminal
{
    partial class WriteSchedulerUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WriteSchedulerUI));
            this.main_flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.button_addTaskPacket = new System.Windows.Forms.Button();
            this.button_start = new System.Windows.Forms.Button();
            this.checkBox_loopOnPackets = new System.Windows.Forms.CheckBox();
            this.textBox_packetsSent = new System.Windows.Forms.TextBox();
            this.label_packetsSent = new System.Windows.Forms.Label();
            this.checkBox_enableLog = new System.Windows.Forms.CheckBox();
            this.button_resetPacketsSent = new System.Windows.Forms.Button();
            this.textBox_logFileAbsPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // main_flowLayoutPanel
            // 
            this.main_flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.main_flowLayoutPanel.AutoScroll = true;
            this.main_flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.main_flowLayoutPanel.Name = "main_flowLayoutPanel";
            this.main_flowLayoutPanel.Size = new System.Drawing.Size(634, 657);
            this.main_flowLayoutPanel.TabIndex = 0;
            // 
            // button_addTaskPacket
            // 
            this.button_addTaskPacket.Location = new System.Drawing.Point(640, 12);
            this.button_addTaskPacket.Name = "button_addTaskPacket";
            this.button_addTaskPacket.Size = new System.Drawing.Size(97, 23);
            this.button_addTaskPacket.TabIndex = 1;
            this.button_addTaskPacket.Text = "Add packet";
            this.button_addTaskPacket.UseVisualStyleBackColor = true;
            this.button_addTaskPacket.Click += new System.EventHandler(this.button_addTaskPacket_Click);
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(640, 37);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(97, 23);
            this.button_start.TabIndex = 2;
            this.button_start.Text = "Start";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // checkBox_loopOnPackets
            // 
            this.checkBox_loopOnPackets.AutoSize = true;
            this.checkBox_loopOnPackets.Location = new System.Drawing.Point(640, 66);
            this.checkBox_loopOnPackets.Name = "checkBox_loopOnPackets";
            this.checkBox_loopOnPackets.Size = new System.Drawing.Size(46, 17);
            this.checkBox_loopOnPackets.TabIndex = 3;
            this.checkBox_loopOnPackets.Text = "loop";
            this.checkBox_loopOnPackets.UseVisualStyleBackColor = true;
            // 
            // textBox_packetsSent
            // 
            this.textBox_packetsSent.BackColor = System.Drawing.Color.White;
            this.textBox_packetsSent.Location = new System.Drawing.Point(640, 106);
            this.textBox_packetsSent.Name = "textBox_packetsSent";
            this.textBox_packetsSent.ReadOnly = true;
            this.textBox_packetsSent.Size = new System.Drawing.Size(97, 20);
            this.textBox_packetsSent.TabIndex = 4;
            this.textBox_packetsSent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_packetsSent
            // 
            this.label_packetsSent.AutoSize = true;
            this.label_packetsSent.Location = new System.Drawing.Point(637, 86);
            this.label_packetsSent.Name = "label_packetsSent";
            this.label_packetsSent.Size = new System.Drawing.Size(32, 13);
            this.label_packetsSent.TabIndex = 5;
            this.label_packetsSent.Text = "Sent:";
            // 
            // checkBox_enableLog
            // 
            this.checkBox_enableLog.AutoSize = true;
            this.checkBox_enableLog.Location = new System.Drawing.Point(641, 157);
            this.checkBox_enableLog.Name = "checkBox_enableLog";
            this.checkBox_enableLog.Size = new System.Drawing.Size(81, 17);
            this.checkBox_enableLog.TabIndex = 6;
            this.checkBox_enableLog.Text = "log enabled";
            this.checkBox_enableLog.UseVisualStyleBackColor = true;
            // 
            // button_resetPacketsSent
            // 
            this.button_resetPacketsSent.Location = new System.Drawing.Point(692, 81);
            this.button_resetPacketsSent.Name = "button_resetPacketsSent";
            this.button_resetPacketsSent.Size = new System.Drawing.Size(45, 23);
            this.button_resetPacketsSent.TabIndex = 7;
            this.button_resetPacketsSent.Text = "Reset";
            this.button_resetPacketsSent.UseVisualStyleBackColor = true;
            this.button_resetPacketsSent.Click += new System.EventHandler(this.button_resetPacketsSent_Click);
            // 
            // textBox_logFileAbsPath
            // 
            this.textBox_logFileAbsPath.Location = new System.Drawing.Point(641, 181);
            this.textBox_logFileAbsPath.Name = "textBox_logFileAbsPath";
            this.textBox_logFileAbsPath.Size = new System.Drawing.Size(96, 20);
            this.textBox_logFileAbsPath.TabIndex = 8;
            this.textBox_logFileAbsPath.Text = "./packets.log";
            // 
            // WriteSchedulerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.ClientSize = new System.Drawing.Size(740, 656);
            this.Controls.Add(this.textBox_logFileAbsPath);
            this.Controls.Add(this.button_resetPacketsSent);
            this.Controls.Add(this.checkBox_enableLog);
            this.Controls.Add(this.label_packetsSent);
            this.Controls.Add(this.textBox_packetsSent);
            this.Controls.Add(this.checkBox_loopOnPackets);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.button_addTaskPacket);
            this.Controls.Add(this.main_flowLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WriteSchedulerUI";
            this.Text = "Packets Scheduler";
            this.Load += new System.EventHandler(this.WriteSchedulerUI_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WriteSchedulerUI_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel main_flowLayoutPanel;
        private System.Windows.Forms.Button button_addTaskPacket;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.CheckBox checkBox_loopOnPackets;
        private System.Windows.Forms.TextBox textBox_packetsSent;
        private System.Windows.Forms.Label label_packetsSent;
        private System.Windows.Forms.CheckBox checkBox_enableLog;
        private System.Windows.Forms.Button button_resetPacketsSent;
        private System.Windows.Forms.TextBox textBox_logFileAbsPath;
    }
}