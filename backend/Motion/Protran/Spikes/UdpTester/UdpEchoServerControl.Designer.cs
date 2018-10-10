namespace Gorba.Motion.Protran.Spikes.UdpTester
{
    partial class UdpEchoServerControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxSentMessages = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxRxMessages = new System.Windows.Forms.TextBox();
            this.buttonRun = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPort)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // nudLocalPort
            // 
            this.nudLocalPort.Location = new System.Drawing.Point(106, 19);
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
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.textBoxSentMessages);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBoxRxMessages);
            this.groupBox3.Controls.Add(this.buttonRun);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(3, 54);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(502, 127);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Receiving";
            // 
            // textBoxSentMessages
            // 
            this.textBoxSentMessages.Location = new System.Drawing.Point(106, 50);
            this.textBoxSentMessages.Name = "textBoxSentMessages";
            this.textBoxSentMessages.ReadOnly = true;
            this.textBoxSentMessages.Size = new System.Drawing.Size(66, 20);
            this.textBoxSentMessages.TabIndex = 7;
            this.textBoxSentMessages.Text = "12345678";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Sent Messages";
            // 
            // textBoxRxMessages
            // 
            this.textBoxRxMessages.Location = new System.Drawing.Point(106, 19);
            this.textBoxRxMessages.Name = "textBoxRxMessages";
            this.textBoxRxMessages.ReadOnly = true;
            this.textBoxRxMessages.Size = new System.Drawing.Size(66, 20);
            this.textBoxRxMessages.TabIndex = 5;
            // 
            // buttonRun
            // 
            this.buttonRun.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonRun.Location = new System.Drawing.Point(106, 97);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(92, 24);
            this.buttonRun.TabIndex = 4;
            this.buttonRun.Text = "Run";
            this.buttonRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.CheckedChanged += new System.EventHandler(this.ButtonRunCheckedChanged);
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
            // UdpEchoServerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "UdpEchoServerControl";
            this.Size = new System.Drawing.Size(508, 185);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalPort)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudLocalPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxSentMessages;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxRxMessages;
        private System.Windows.Forms.CheckBox buttonRun;
        private System.Windows.Forms.Label label8;
    }
}
