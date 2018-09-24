namespace MyHyperTerminal
{
    partial class SendFileUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendFileUI));
            this.button_selectAFile = new System.Windows.Forms.Button();
            this.textBox_fileAbsPath = new System.Windows.Forms.TextBox();
            this.button_send = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_selectAFile
            // 
            this.button_selectAFile.Location = new System.Drawing.Point(1, 12);
            this.button_selectAFile.Name = "button_selectAFile";
            this.button_selectAFile.Size = new System.Drawing.Size(51, 23);
            this.button_selectAFile.TabIndex = 0;
            this.button_selectAFile.Text = "Select";
            this.button_selectAFile.UseVisualStyleBackColor = true;
            this.button_selectAFile.Click += new System.EventHandler(this.button_selectAFile_Click);
            // 
            // textBox_fileAbsPath
            // 
            this.textBox_fileAbsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_fileAbsPath.Location = new System.Drawing.Point(55, 14);
            this.textBox_fileAbsPath.Name = "textBox_fileAbsPath";
            this.textBox_fileAbsPath.Size = new System.Drawing.Size(232, 20);
            this.textBox_fileAbsPath.TabIndex = 1;
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(120, 54);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(52, 23);
            this.button_send.TabIndex = 2;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // SendFileUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.textBox_fileAbsPath);
            this.Controls.Add(this.button_selectAFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SendFileUI";
            this.Text = "Send a file";
            this.Load += new System.EventHandler(this.SendFileUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_selectAFile;
        private System.Windows.Forms.TextBox textBox_fileAbsPath;
        private System.Windows.Forms.Button button_send;
    }
}