namespace FileCompressionTest
{
    partial class SelectCompress
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBoxOutputFilename = new System.Windows.Forms.TextBox();
            this.CompressButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonOpenGz = new System.Windows.Forms.Button();
            this.textBoxOutputBufferSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxInputBufferSize = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxGzOutputFile = new System.Windows.Forms.TextBox();
            this.buttonGzCompress = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxBuffer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ZipfolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(2, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(478, 452);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBoxOutputFilename);
            this.tabPage1.Controls.Add(this.CompressButton);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.BrowseButton);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(470, 426);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Zip Compress";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBoxOutputFilename
            // 
            this.textBoxOutputFilename.Location = new System.Drawing.Point(129, 46);
            this.textBoxOutputFilename.Name = "textBoxOutputFilename";
            this.textBoxOutputFilename.Size = new System.Drawing.Size(249, 20);
            this.textBoxOutputFilename.TabIndex = 5;
            // 
            // CompressButton
            // 
            this.CompressButton.Location = new System.Drawing.Point(384, 43);
            this.CompressButton.Name = "CompressButton";
            this.CompressButton.Size = new System.Drawing.Size(75, 23);
            this.CompressButton.TabIndex = 4;
            this.CompressButton.Text = "Compress";
            this.CompressButton.UseVisualStyleBackColor = true;
            this.CompressButton.Click += new System.EventHandler(this.CompressButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Output File Path:";
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(129, 11);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 2;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Open File to compress:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonOpenGz);
            this.tabPage2.Controls.Add(this.textBoxOutputBufferSize);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.textBoxInputBufferSize);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.textBoxGzOutputFile);
            this.tabPage2.Controls.Add(this.buttonGzCompress);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.textBoxBuffer);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(470, 426);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Gz Compress";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonOpenGz
            // 
            this.buttonOpenGz.Location = new System.Drawing.Point(190, 89);
            this.buttonOpenGz.Name = "buttonOpenGz";
            this.buttonOpenGz.Size = new System.Drawing.Size(107, 23);
            this.buttonOpenGz.TabIndex = 13;
            this.buttonOpenGz.Text = "Open Gz Stream";
            this.buttonOpenGz.UseVisualStyleBackColor = true;
            this.buttonOpenGz.Click += new System.EventHandler(this.ButtonOpenGzClick);
            // 
            // textBoxOutputBufferSize
            // 
            this.textBoxOutputBufferSize.Location = new System.Drawing.Point(330, 131);
            this.textBoxOutputBufferSize.Name = "textBoxOutputBufferSize";
            this.textBoxOutputBufferSize.Size = new System.Drawing.Size(100, 20);
            this.textBoxOutputBufferSize.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(231, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Output buffer size:";
            // 
            // textBoxInputBufferSize
            // 
            this.textBoxInputBufferSize.Location = new System.Drawing.Point(109, 130);
            this.textBoxInputBufferSize.Name = "textBoxInputBufferSize";
            this.textBoxInputBufferSize.Size = new System.Drawing.Size(100, 20);
            this.textBoxInputBufferSize.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Input buffer size:";
            // 
            // textBoxGzOutputFile
            // 
            this.textBoxGzOutputFile.Location = new System.Drawing.Point(109, 48);
            this.textBoxGzOutputFile.Name = "textBoxGzOutputFile";
            this.textBoxGzOutputFile.Size = new System.Drawing.Size(183, 20);
            this.textBoxGzOutputFile.TabIndex = 8;
            // 
            // buttonGzCompress
            // 
            this.buttonGzCompress.Location = new System.Drawing.Point(109, 89);
            this.buttonGzCompress.Name = "buttonGzCompress";
            this.buttonGzCompress.Size = new System.Drawing.Size(75, 23);
            this.buttonGzCompress.TabIndex = 7;
            this.buttonGzCompress.Text = "Compress";
            this.buttonGzCompress.UseVisualStyleBackColor = true;
            this.buttonGzCompress.Click += new System.EventHandler(this.ButtonGzCompressClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Output File Path:";
            // 
            // textBoxBuffer
            // 
            this.textBoxBuffer.Location = new System.Drawing.Point(109, 8);
            this.textBoxBuffer.Name = "textBoxBuffer";
            this.textBoxBuffer.Size = new System.Drawing.Size(183, 20);
            this.textBoxBuffer.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Enter Bytes for Zip:";
            // 
            // SelectCompress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 467);
            this.Controls.Add(this.tabControl1);
            this.Name = "SelectCompress";
            this.Text = "SelectCompress";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button CompressButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxOutputFilename;
        private System.Windows.Forms.FolderBrowserDialog ZipfolderBrowserDialog;
        private System.Windows.Forms.TextBox textBoxBuffer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxGzOutputFile;
        private System.Windows.Forms.Button buttonGzCompress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxOutputBufferSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxInputBufferSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonOpenGz;
    }
}