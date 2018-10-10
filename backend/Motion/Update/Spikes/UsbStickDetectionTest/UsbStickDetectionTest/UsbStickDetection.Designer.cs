namespace UsbStickDetectionTest
{
    partial class UsbStickDetection
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
            this.buttonWmi = new System.Windows.Forms.Button();
            this.richTextBoxWmi = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonWmi
            // 
            this.buttonWmi.Location = new System.Drawing.Point(12, 21);
            this.buttonWmi.Name = "buttonWmi";
            this.buttonWmi.Size = new System.Drawing.Size(107, 23);
            this.buttonWmi.TabIndex = 1;
            this.buttonWmi.Text = "Start Wmi";
            this.buttonWmi.UseVisualStyleBackColor = true;
            this.buttonWmi.Click += new System.EventHandler(this.ButtonWmiClick);
            // 
            // richTextBoxWmi
            // 
            this.richTextBoxWmi.Location = new System.Drawing.Point(12, 64);
            this.richTextBoxWmi.Name = "richTextBoxWmi";
            this.richTextBoxWmi.Size = new System.Drawing.Size(281, 135);
            this.richTextBoxWmi.TabIndex = 3;
            this.richTextBoxWmi.Text = "";
            // 
            // UsbStickDetection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 384);
            this.Controls.Add(this.buttonWmi);
            this.Controls.Add(this.richTextBoxWmi);
            this.Name = "UsbStickDetection";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonWmi;
        private System.Windows.Forms.RichTextBox richTextBoxWmi;
    }
}

