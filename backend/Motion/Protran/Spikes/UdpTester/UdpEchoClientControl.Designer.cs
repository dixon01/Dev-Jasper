namespace Gorba.Motion.Protran.Spikes.UdpTester
{
    partial class UdpEchoClientControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudLocalPort = new System.Windows.Forms.NumericUpDown();
            this.nudRemotePort = new System.Windows.Forms.NumericUpDown();
            this.textBoxRemoteAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxPayload = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxTotalTime = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxRxdTimeStamp = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxSentTimeStamp = new System.Windows.Forms.TextBox();
            this.textBoxRxdMessages = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxSentMessages = new System.Windows.Forms.TextBox();
            this.buttonRun = new System.Windows.Forms.CheckBox();
            this.nudSendInterval = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRemotePort)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSendInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.nudLocalPort);
            this.groupBox1.Controls.Add(this.nudRemotePort);
            this.groupBox1.Controls.Add(this.textBoxRemoteAddress);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(426, 100);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // nudLocalPort
            // 
            this.nudLocalPort.Location = new System.Drawing.Point(113, 19);
            this.nudLocalPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudLocalPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLocalPort.Name = "nudLocalPort";
            this.nudLocalPort.Size = new System.Drawing.Size(66, 20);
            this.nudLocalPort.TabIndex = 3;
            this.nudLocalPort.Value = new decimal(new int[] {
            13500,
            0,
            0,
            0});
            // 
            // nudRemotePort
            // 
            this.nudRemotePort.Location = new System.Drawing.Point(113, 71);
            this.nudRemotePort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudRemotePort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRemotePort.Name = "nudRemotePort";
            this.nudRemotePort.Size = new System.Drawing.Size(66, 20);
            this.nudRemotePort.TabIndex = 0;
            this.nudRemotePort.Value = new decimal(new int[] {
            13501,
            0,
            0,
            0});
            // 
            // textBoxRemoteAddress
            // 
            this.textBoxRemoteAddress.Location = new System.Drawing.Point(113, 45);
            this.textBoxRemoteAddress.Name = "textBoxRemoteAddress";
            this.textBoxRemoteAddress.Size = new System.Drawing.Size(171, 20);
            this.textBoxRemoteAddress.TabIndex = 2;
            this.textBoxRemoteAddress.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Local Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Remote Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Remote Address";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.textBoxPayload);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(3, 109);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(426, 45);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Payload";
            // 
            // textBoxPayload
            // 
            this.textBoxPayload.Location = new System.Drawing.Point(113, 14);
            this.textBoxPayload.Name = "textBoxPayload";
            this.textBoxPayload.Size = new System.Drawing.Size(171, 20);
            this.textBoxPayload.TabIndex = 3;
            this.textBoxPayload.Text = "ABCDEFGH";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(62, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Payload";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.textBoxTotalTime);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.textBoxRxdTimeStamp);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.textBoxSentTimeStamp);
            this.groupBox3.Controls.Add(this.textBoxRxdMessages);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.textBoxSentMessages);
            this.groupBox3.Controls.Add(this.buttonRun);
            this.groupBox3.Controls.Add(this.nudSendInterval);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(3, 160);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(426, 161);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sending";
            // 
            // textBoxTotalTime
            // 
            this.textBoxTotalTime.Location = new System.Drawing.Point(113, 103);
            this.textBoxTotalTime.Name = "textBoxTotalTime";
            this.textBoxTotalTime.ReadOnly = true;
            this.textBoxTotalTime.Size = new System.Drawing.Size(107, 20);
            this.textBoxTotalTime.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(11, 106);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Total time for Echo";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(209, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(109, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Received TimeStamp";
            // 
            // textBoxRxdTimeStamp
            // 
            this.textBoxRxdTimeStamp.Location = new System.Drawing.Point(329, 74);
            this.textBoxRxdTimeStamp.Name = "textBoxRxdTimeStamp";
            this.textBoxRxdTimeStamp.ReadOnly = true;
            this.textBoxRxdTimeStamp.Size = new System.Drawing.Size(66, 20);
            this.textBoxRxdTimeStamp.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(233, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Sent TimeStamp";
            // 
            // textBoxSentTimeStamp
            // 
            this.textBoxSentTimeStamp.Location = new System.Drawing.Point(329, 45);
            this.textBoxSentTimeStamp.Name = "textBoxSentTimeStamp";
            this.textBoxSentTimeStamp.ReadOnly = true;
            this.textBoxSentTimeStamp.Size = new System.Drawing.Size(66, 20);
            this.textBoxSentTimeStamp.TabIndex = 8;
            // 
            // textBoxRxdMessages
            // 
            this.textBoxRxdMessages.Location = new System.Drawing.Point(113, 74);
            this.textBoxRxdMessages.Name = "textBoxRxdMessages";
            this.textBoxRxdMessages.ReadOnly = true;
            this.textBoxRxdMessages.Size = new System.Drawing.Size(66, 20);
            this.textBoxRxdMessages.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Received Message";
            // 
            // textBoxSentMessages
            // 
            this.textBoxSentMessages.Location = new System.Drawing.Point(113, 45);
            this.textBoxSentMessages.Name = "textBoxSentMessages";
            this.textBoxSentMessages.ReadOnly = true;
            this.textBoxSentMessages.Size = new System.Drawing.Size(66, 20);
            this.textBoxSentMessages.TabIndex = 5;
            // 
            // buttonRun
            // 
            this.buttonRun.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonRun.Location = new System.Drawing.Point(113, 131);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(92, 24);
            this.buttonRun.TabIndex = 4;
            this.buttonRun.Text = "Run";
            this.buttonRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.CheckedChanged += new System.EventHandler(this.ButtonRunCheckedChanged);
            // 
            // nudSendInterval
            // 
            this.nudSendInterval.Location = new System.Drawing.Point(113, 19);
            this.nudSendInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudSendInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSendInterval.Name = "nudSendInterval";
            this.nudSendInterval.Size = new System.Drawing.Size(66, 20);
            this.nudSendInterval.TabIndex = 3;
            this.nudSendInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(185, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "ms";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Sent Message";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Send Interval";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(226, 106);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(20, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "ms";
            // 
            // UdpEchoClientControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "UdpEchoClientControl";
            this.Size = new System.Drawing.Size(433, 322);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRemotePort)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSendInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudLocalPort;
        private System.Windows.Forms.NumericUpDown nudRemotePort;
        private System.Windows.Forms.TextBox textBoxRemoteAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPayload;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxSentMessages;
        private System.Windows.Forms.CheckBox buttonRun;
        private System.Windows.Forms.NumericUpDown nudSendInterval;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxSentTimeStamp;
        private System.Windows.Forms.TextBox textBoxRxdMessages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxRxdTimeStamp;
        private System.Windows.Forms.TextBox textBoxTotalTime;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;

    }
}
