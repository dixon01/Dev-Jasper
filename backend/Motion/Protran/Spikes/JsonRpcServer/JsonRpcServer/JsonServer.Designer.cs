namespace JsonRpcServer
{
    partial class JsonServer
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonSendTelegram = new System.Windows.Forms.Button();
            this.textBoxTelegram = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(13, 13);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(97, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start Server";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.ButtonStartClick);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(13, 43);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(97, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop Server";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.ButtonStopClick);
            // 
            // buttonSendTelegram
            // 
            this.buttonSendTelegram.Location = new System.Drawing.Point(148, 79);
            this.buttonSendTelegram.Name = "buttonSendTelegram";
            this.buttonSendTelegram.Size = new System.Drawing.Size(97, 23);
            this.buttonSendTelegram.TabIndex = 2;
            this.buttonSendTelegram.Text = "Send Telegram";
            this.buttonSendTelegram.UseVisualStyleBackColor = true;
            this.buttonSendTelegram.Click += new System.EventHandler(this.ButtonSendTelegramClick);
            // 
            // textBoxTelegram
            // 
            this.textBoxTelegram.Location = new System.Drawing.Point(13, 79);
            this.textBoxTelegram.Name = "textBoxTelegram";
            this.textBoxTelegram.Size = new System.Drawing.Size(118, 20);
            this.textBoxTelegram.TabIndex = 3;
            // 
            // JsonServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 284);
            this.Controls.Add(this.textBoxTelegram);
            this.Controls.Add(this.buttonSendTelegram);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Name = "JsonServer";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonSendTelegram;
        private System.Windows.Forms.TextBox textBoxTelegram;
    }
}

