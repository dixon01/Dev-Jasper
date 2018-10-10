namespace Gorba.Motion.Protran.Visualizer.Controls.Main
{
    partial class ApplicationLogForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxLogLevel = new System.Windows.Forms.ComboBox();
            this.nlogTextBox = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Log Level:";
            // 
            // comboBoxLogLevel
            // 
            this.comboBoxLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLogLevel.FormattingEnabled = true;
            this.comboBoxLogLevel.Location = new System.Drawing.Point(66, 3);
            this.comboBoxLogLevel.Name = "comboBoxLogLevel";
            this.comboBoxLogLevel.Size = new System.Drawing.Size(121, 21);
            this.comboBoxLogLevel.TabIndex = 4;
            this.comboBoxLogLevel.SelectedIndexChanged += new System.EventHandler(this.ComboBoxLogLevelOnSelectedIndexChanged);
            // 
            // nlogTextBox
            // 
            this.nlogTextBox.BackColor = System.Drawing.Color.Black;
            this.nlogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nlogTextBox.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nlogTextBox.Location = new System.Drawing.Point(0, 27);
            this.nlogTextBox.Name = "nlogTextBox";
            this.nlogTextBox.ReadOnly = true;
            this.nlogTextBox.Size = new System.Drawing.Size(540, 287);
            this.nlogTextBox.TabIndex = 3;
            this.nlogTextBox.Text = "";
            this.nlogTextBox.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBoxLogLevel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(540, 27);
            this.panel1.TabIndex = 6;
            // 
            // ApplicationLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 314);
            this.Controls.Add(this.nlogTextBox);
            this.Controls.Add(this.panel1);
            this.Name = "ApplicationLogForm";
            this.Text = "Application Logs";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxLogLevel;
        private System.Windows.Forms.RichTextBox nlogTextBox;
        private System.Windows.Forms.Panel panel1;

    }
}