namespace Gorba.Motion.Protran.Controls
{
    partial class GO005RowControl
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
            this.textBoxDataLength = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBoxLineNumber = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBoxStopIndex = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBoxDestination = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.applySendButton = new Gorba.Motion.Protran.Controls.ApplySendButton();
            this.SuspendLayout();
            // 
            // textBoxDataLength
            // 
            this.textBoxDataLength.Location = new System.Drawing.Point(3, 3);
            this.textBoxDataLength.Name = "textBoxDataLength";
            this.textBoxDataLength.Size = new System.Drawing.Size(66, 20);
            this.textBoxDataLength.Suggestion = "HH";
            this.textBoxDataLength.TabIndex = 5;
            this.textBoxDataLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxLineNumber
            // 
            this.textBoxLineNumber.Location = new System.Drawing.Point(75, 3);
            this.textBoxLineNumber.Name = "textBoxLineNumber";
            this.textBoxLineNumber.Size = new System.Drawing.Size(78, 20);
            this.textBoxLineNumber.Suggestion = "LLLL";
            this.textBoxLineNumber.TabIndex = 6;
            this.textBoxLineNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxLineNumber.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBoxStopIndex
            // 
            this.textBoxStopIndex.Location = new System.Drawing.Point(159, 3);
            this.textBoxStopIndex.Name = "textBoxStopIndex";
            this.textBoxStopIndex.Size = new System.Drawing.Size(78, 20);
            this.textBoxStopIndex.Suggestion = "XXXX";
            this.textBoxStopIndex.TabIndex = 7;
            this.textBoxStopIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxStopIndex.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBoxDestination
            // 
            this.textBoxDestination.Location = new System.Drawing.Point(243, 3);
            this.textBoxDestination.Name = "textBoxDestination";
            this.textBoxDestination.Size = new System.Drawing.Size(209, 20);
            this.textBoxDestination.Suggestion = "Max 30 Characters";
            this.textBoxDestination.TabIndex = 8;
            this.textBoxDestination.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxDestination.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // applySendButton
            // 
            this.applySendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applySendButton.Location = new System.Drawing.Point(460, 1);
            this.applySendButton.Name = "applySendButton";
            this.applySendButton.Size = new System.Drawing.Size(75, 23);
            this.applySendButton.TabIndex = 9;
            this.applySendButton.Text = "Apply";
            this.applySendButton.UseVisualStyleBackColor = true;
            this.applySendButton.SendClick += new System.EventHandler(this.ButtonSendClick);
            this.applySendButton.Click += new System.EventHandler(this.ButtonApplyClick);
            // 
            // GO005Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.applySendButton);
            this.Controls.Add(this.textBoxDestination);
            this.Controls.Add(this.textBoxStopIndex);
            this.Controls.Add(this.textBoxLineNumber);
            this.Controls.Add(this.textBoxDataLength);
            this.Name = "GO005Control";
            this.Size = new System.Drawing.Size(535, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SuggestTextBox textBoxDataLength;
        private SuggestTextBox textBoxLineNumber;
        private SuggestTextBox textBoxStopIndex;
        private SuggestTextBox textBoxDestination;
        private ApplySendButton applySendButton;
    }
}
