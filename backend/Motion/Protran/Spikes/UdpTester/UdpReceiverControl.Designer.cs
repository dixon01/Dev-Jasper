namespace Gorba.Motion.Protran.Spikes.UdpTester
{
    partial class UdpReceiverControl
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxRxMax = new System.Windows.Forms.TextBox();
            this.textBoxRxAvg = new System.Windows.Forms.TextBox();
            this.textBoxRxMin = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxRxMessages = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudLocalPort = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLostMessages = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPort)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.textBoxRxMax);
            this.groupBox3.Controls.Add(this.textBoxRxAvg);
            this.groupBox3.Controls.Add(this.textBoxRxMin);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBoxLostMessages);
            this.groupBox3.Controls.Add(this.textBoxRxMessages);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.buttonRun);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(3, 54);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(502, 140);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Receiving";
            // 
            // textBoxRxMax
            // 
            this.textBoxRxMax.Location = new System.Drawing.Point(241, 84);
            this.textBoxRxMax.Name = "textBoxRxMax";
            this.textBoxRxMax.ReadOnly = true;
            this.textBoxRxMax.Size = new System.Drawing.Size(66, 20);
            this.textBoxRxMax.TabIndex = 5;
            // 
            // textBoxRxAvg
            // 
            this.textBoxRxAvg.Location = new System.Drawing.Point(169, 84);
            this.textBoxRxAvg.Name = "textBoxRxAvg";
            this.textBoxRxAvg.ReadOnly = true;
            this.textBoxRxAvg.Size = new System.Drawing.Size(66, 20);
            this.textBoxRxAvg.TabIndex = 5;
            // 
            // textBoxRxMin
            // 
            this.textBoxRxMin.Location = new System.Drawing.Point(97, 84);
            this.textBoxRxMin.Name = "textBoxRxMin";
            this.textBoxRxMin.ReadOnly = true;
            this.textBoxRxMin.Size = new System.Drawing.Size(66, 20);
            this.textBoxRxMin.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(241, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Maximum";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // textBoxRxMessages
            // 
            this.textBoxRxMessages.Location = new System.Drawing.Point(97, 19);
            this.textBoxRxMessages.Name = "textBoxRxMessages";
            this.textBoxRxMessages.ReadOnly = true;
            this.textBoxRxMessages.Size = new System.Drawing.Size(66, 20);
            this.textBoxRxMessages.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(169, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Average";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // buttonRun
            // 
            this.buttonRun.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonRun.Location = new System.Drawing.Point(97, 110);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(92, 24);
            this.buttonRun.TabIndex = 4;
            this.buttonRun.Text = "Run";
            this.buttonRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.CheckedChanged += new System.EventHandler(this.ButtonRunCheckedChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(97, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Minimum";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Rx Jitter";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Rx Messages";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.nudLocalPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 45);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // nudLocalPort
            // 
            this.nudLocalPort.Location = new System.Drawing.Point(97, 19);
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
            13501,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Local Port";
            // 
            // textBoxLostMessages
            // 
            this.textBoxLostMessages.Location = new System.Drawing.Point(97, 45);
            this.textBoxLostMessages.Name = "textBoxLostMessages";
            this.textBoxLostMessages.ReadOnly = true;
            this.textBoxLostMessages.Size = new System.Drawing.Size(66, 20);
            this.textBoxLostMessages.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Lost Messages";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(313, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "ms";
            // 
            // UdpReceiverControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "UdpReceiverControl";
            this.Size = new System.Drawing.Size(508, 319);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxRxMessages;
        private System.Windows.Forms.CheckBox buttonRun;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudLocalPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxRxMin;
        private System.Windows.Forms.TextBox textBoxRxMax;
        private System.Windows.Forms.TextBox textBoxRxAvg;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLostMessages;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}
