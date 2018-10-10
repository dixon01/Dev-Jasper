namespace Gorba.Motion.Protran.Visualizer.Controls
{
    partial class LogsControl
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
            this.checkBoxTelegramsEnabled = new System.Windows.Forms.CheckBox();
            this.listBoxTelegramLog = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // checkBoxTelegramsEnabled
            // 
            this.checkBoxTelegramsEnabled.AutoSize = true;
            this.checkBoxTelegramsEnabled.Checked = true;
            this.checkBoxTelegramsEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTelegramsEnabled.Location = new System.Drawing.Point(3, 3);
            this.checkBoxTelegramsEnabled.Name = "checkBoxTelegramsEnabled";
            this.checkBoxTelegramsEnabled.Size = new System.Drawing.Size(65, 17);
            this.checkBoxTelegramsEnabled.TabIndex = 1;
            this.checkBoxTelegramsEnabled.Text = "Enabled";
            this.checkBoxTelegramsEnabled.UseVisualStyleBackColor = true;
            this.checkBoxTelegramsEnabled.CheckedChanged += new System.EventHandler(this.CheckBoxTelegramsEnabledCheckedChanged);
            // 
            // listBoxTelegramLog
            // 
            this.listBoxTelegramLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxTelegramLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxTelegramLog.FormattingEnabled = true;
            this.listBoxTelegramLog.IntegralHeight = false;
            this.listBoxTelegramLog.ItemHeight = 18;
            this.listBoxTelegramLog.Location = new System.Drawing.Point(3, 26);
            this.listBoxTelegramLog.Name = "listBoxTelegramLog";
            this.listBoxTelegramLog.ScrollAlwaysVisible = true;
            this.listBoxTelegramLog.Size = new System.Drawing.Size(599, 403);
            this.listBoxTelegramLog.TabIndex = 0;
            this.listBoxTelegramLog.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBoxTelegramLogDrawItem);
            this.listBoxTelegramLog.DoubleClick += new System.EventHandler(this.ListBoxTelegramLogDoubleClick);
            // 
            // LogsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBoxTelegramLog);
            this.Controls.Add(this.checkBoxTelegramsEnabled);
            this.Name = "LogsControl";
            this.Size = new System.Drawing.Size(605, 432);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxTelegramLog;
        private System.Windows.Forms.CheckBox checkBoxTelegramsEnabled;
    }
}
