namespace Gorba.Motion.Protran.Tools.TopboxSimulator
{
    partial class TransferStatus
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_clearDatagramsSent = new System.Windows.Forms.Button();
            this.textBox_datagramsSent = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_clearDatagramsReceived = new System.Windows.Forms.Button();
            this.textBox_datagramsReceived = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_clearDatagramsSent);
            this.groupBox2.Controls.Add(this.textBox_datagramsSent);
            this.groupBox2.Location = new System.Drawing.Point(12, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(473, 489);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Datagrams Sent to CU5";
            // 
            // button_clearDatagramsSent
            // 
            this.button_clearDatagramsSent.Location = new System.Drawing.Point(379, 460);
            this.button_clearDatagramsSent.Name = "button_clearDatagramsSent";
            this.button_clearDatagramsSent.Size = new System.Drawing.Size(75, 23);
            this.button_clearDatagramsSent.TabIndex = 5;
            this.button_clearDatagramsSent.Text = "Clear";
            this.button_clearDatagramsSent.UseVisualStyleBackColor = true;
            this.button_clearDatagramsSent.Click += new System.EventHandler(this.ButtonClearDatagramsSentClick);
            // 
            // textBox_datagramsSent
            // 
            this.textBox_datagramsSent.Location = new System.Drawing.Point(16, 19);
            this.textBox_datagramsSent.Multiline = true;
            this.textBox_datagramsSent.Name = "textBox_datagramsSent";
            this.textBox_datagramsSent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_datagramsSent.Size = new System.Drawing.Size(438, 435);
            this.textBox_datagramsSent.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_clearDatagramsReceived);
            this.groupBox3.Controls.Add(this.textBox_datagramsReceived);
            this.groupBox3.Location = new System.Drawing.Point(491, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(437, 492);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Received Datagrams from CU5";
            // 
            // button_clearDatagramsReceived
            // 
            this.button_clearDatagramsReceived.Location = new System.Drawing.Point(346, 463);
            this.button_clearDatagramsReceived.Name = "button_clearDatagramsReceived";
            this.button_clearDatagramsReceived.Size = new System.Drawing.Size(75, 23);
            this.button_clearDatagramsReceived.TabIndex = 1;
            this.button_clearDatagramsReceived.Text = "Clear";
            this.button_clearDatagramsReceived.UseVisualStyleBackColor = true;
            this.button_clearDatagramsReceived.Click += new System.EventHandler(this.ButtonClearDatagramsReceivedClick);
            // 
            // textBox_datagramsReceived
            // 
            this.textBox_datagramsReceived.Location = new System.Drawing.Point(16, 20);
            this.textBox_datagramsReceived.Multiline = true;
            this.textBox_datagramsReceived.Name = "textBox_datagramsReceived";
            this.textBox_datagramsReceived.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_datagramsReceived.Size = new System.Drawing.Size(405, 434);
            this.textBox_datagramsReceived.TabIndex = 0;
            // 
            // TransferStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 514);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "TransferStatus";
            this.Text = "TransferStatus";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox_datagramsSent;
        private System.Windows.Forms.TextBox textBox_datagramsReceived;
        private System.Windows.Forms.Button button_clearDatagramsSent;
        private System.Windows.Forms.Button button_clearDatagramsReceived;
    }
}