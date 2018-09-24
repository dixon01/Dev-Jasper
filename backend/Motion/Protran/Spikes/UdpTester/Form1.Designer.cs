namespace Gorba.Motion.Protran.Spikes.UdpTester
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.udpSenderControl1 = new Gorba.Motion.Protran.Spikes.UdpTester.UdpSenderControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.udpReceiverControl1 = new Gorba.Motion.Protran.Spikes.UdpTester.UdpReceiverControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.udpEchoClientControl1 = new Gorba.Motion.Protran.Spikes.UdpTester.UdpEchoClientControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.udpEchoServerControl1 = new Gorba.Motion.Protran.Spikes.UdpTester.UdpEchoServerControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(496, 353);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.udpSenderControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(488, 327);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Sender";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // udpSenderControl1
            // 
            this.udpSenderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpSenderControl1.Location = new System.Drawing.Point(3, 3);
            this.udpSenderControl1.Name = "udpSenderControl1";
            this.udpSenderControl1.Size = new System.Drawing.Size(482, 321);
            this.udpSenderControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.udpReceiverControl1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(488, 327);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Receiver";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // udpReceiverControl1
            // 
            this.udpReceiverControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpReceiverControl1.Location = new System.Drawing.Point(3, 3);
            this.udpReceiverControl1.Name = "udpReceiverControl1";
            this.udpReceiverControl1.Size = new System.Drawing.Size(482, 321);
            this.udpReceiverControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.udpEchoClientControl1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(488, 327);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Echo Client";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // udpEchoClientControl1
            // 
            this.udpEchoClientControl1.Location = new System.Drawing.Point(4, 4);
            this.udpEchoClientControl1.Name = "udpEchoClientControl1";
            this.udpEchoClientControl1.Size = new System.Drawing.Size(488, 323);
            this.udpEchoClientControl1.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.udpEchoServerControl1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(488, 327);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Echo Server";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // udpEchoServerControl1
            // 
            this.udpEchoServerControl1.Location = new System.Drawing.Point(4, 4);
            this.udpEchoServerControl1.Name = "udpEchoServerControl1";
            this.udpEchoServerControl1.Size = new System.Drawing.Size(481, 190);
            this.udpEchoServerControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 353);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "UDP Tester";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private UdpSenderControl udpSenderControl1;
        private UdpReceiverControl udpReceiverControl1;
        private System.Windows.Forms.TabPage tabPage4;
        private UdpEchoClientControl udpEchoClientControl1;
        private UdpEchoServerControl udpEchoServerControl1;
    }
}

