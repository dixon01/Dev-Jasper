namespace Gorba.Motion.Protran.Controls
{
    partial class GO004RowControl
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
            this.textBoxIndex = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBoxType = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBoxStartTime = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBoxEndTime = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.applySendButton1 = new Gorba.Motion.Protran.Controls.ApplySendButton();
            this.SuspendLayout();
            // 
            // textBoxIndex
            // 
            this.textBoxIndex.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxIndex.Location = new System.Drawing.Point(3, 3);
            this.textBoxIndex.Name = "textBoxIndex";
            this.textBoxIndex.Size = new System.Drawing.Size(57, 20);
            this.textBoxIndex.Suggestion = "ZZ";
            this.textBoxIndex.TabIndex = 0;
            this.textBoxIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxType
            // 
            this.textBoxType.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxType.Location = new System.Drawing.Point(66, 3);
            this.textBoxType.Name = "textBoxType";
            this.textBoxType.Size = new System.Drawing.Size(57, 20);
            this.textBoxType.Suggestion = "ZZ";
            this.textBoxType.TabIndex = 1;
            this.textBoxType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxStartTime
            // 
            this.textBoxStartTime.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxStartTime.Location = new System.Drawing.Point(129, 3);
            this.textBoxStartTime.Name = "textBoxStartTime";
            this.textBoxStartTime.Size = new System.Drawing.Size(57, 20);
            this.textBoxStartTime.Suggestion = "HHMM";
            this.textBoxStartTime.TabIndex = 2;
            this.textBoxStartTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxEndTime
            // 
            this.textBoxEndTime.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxEndTime.Location = new System.Drawing.Point(192, 3);
            this.textBoxEndTime.Name = "textBoxEndTime";
            this.textBoxEndTime.Size = new System.Drawing.Size(57, 20);
            this.textBoxEndTime.Suggestion = "HHMM";
            this.textBoxEndTime.TabIndex = 3;
            this.textBoxEndTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMessage.Location = new System.Drawing.Point(255, 3);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(220, 20);
            this.textBoxMessage.TabIndex = 4;
            // 
            // applySendButton1
            // 
            this.applySendButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applySendButton1.Location = new System.Drawing.Point(481, 1);
            this.applySendButton1.Name = "applySendButton1";
            this.applySendButton1.Size = new System.Drawing.Size(75, 23);
            this.applySendButton1.TabIndex = 5;
            this.applySendButton1.Text = "Apply";
            this.applySendButton1.UseVisualStyleBackColor = true;
            this.applySendButton1.SendClick += new System.EventHandler(this.ButtonSendClick);
            this.applySendButton1.Click += new System.EventHandler(this.ButtonApplyClick);
            // 
            // GO004Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.applySendButton1);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.textBoxEndTime);
            this.Controls.Add(this.textBoxStartTime);
            this.Controls.Add(this.textBoxType);
            this.Controls.Add(this.textBoxIndex);
            this.Name = "GO004Control";
            this.Size = new System.Drawing.Size(559, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SuggestTextBox textBoxIndex;
        private SuggestTextBox textBoxType;
        private SuggestTextBox textBoxStartTime;
        private SuggestTextBox textBoxEndTime;
        private System.Windows.Forms.TextBox textBoxMessage;
        private ApplySendButton applySendButton1;
    }
}
