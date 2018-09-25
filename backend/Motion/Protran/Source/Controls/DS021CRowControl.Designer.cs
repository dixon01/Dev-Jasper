namespace Gorba.Motion.Protran.Controls
{
    partial class DS021CRowControl
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
            this.button_apply = new Gorba.Motion.Protran.Controls.ApplySendButton();
            this.textBox_TransferSymbols = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBox_Transfers = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBox_StopName = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBox_StopIndex = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.textBoxStatus = new Gorba.Motion.Protran.Controls.SuggestTextBox();
            this.SuspendLayout();
            // 
            // button_apply
            // 
            this.button_apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_apply.Location = new System.Drawing.Point(620, 0);
            this.button_apply.Name = "button_apply";
            this.button_apply.Size = new System.Drawing.Size(43, 23);
            this.button_apply.TabIndex = 38;
            this.button_apply.Text = "Apply";
            this.button_apply.UseVisualStyleBackColor = true;
            this.button_apply.SendClick += new System.EventHandler(this.ButtonSendClick);
            this.button_apply.Click += new System.EventHandler(this.ButtonApplyClick);
            // 
            // textBox_TransferSymbols
            // 
            this.textBox_TransferSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TransferSymbols.ForeColor = System.Drawing.Color.LightGray;
            this.textBox_TransferSymbols.Location = new System.Drawing.Point(479, 3);
            this.textBox_TransferSymbols.Name = "textBox_TransferSymbols";
            this.textBox_TransferSymbols.Size = new System.Drawing.Size(125, 20);
            this.textBox_TransferSymbols.Suggestion = "Max 30 characters";
            this.textBox_TransferSymbols.TabIndex = 37;
            this.textBox_TransferSymbols.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_Transfers
            // 
            this.textBox_Transfers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Transfers.ForeColor = System.Drawing.Color.LightGray;
            this.textBox_Transfers.Location = new System.Drawing.Point(349, 3);
            this.textBox_Transfers.Name = "textBox_Transfers";
            this.textBox_Transfers.Size = new System.Drawing.Size(124, 20);
            this.textBox_Transfers.Suggestion = "Max 30 characters";
            this.textBox_Transfers.TabIndex = 36;
            this.textBox_Transfers.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_StopName
            // 
            this.textBox_StopName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_StopName.ForeColor = System.Drawing.Color.LightGray;
            this.textBox_StopName.Location = new System.Drawing.Point(125, 3);
            this.textBox_StopName.Name = "textBox_StopName";
            this.textBox_StopName.Size = new System.Drawing.Size(218, 20);
            this.textBox_StopName.Suggestion = "Max 30 characters";
            this.textBox_StopName.TabIndex = 35;
            this.textBox_StopName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_StopIndex
            // 
            this.textBox_StopIndex.ForeColor = System.Drawing.Color.LightGray;
            this.textBox_StopIndex.Location = new System.Drawing.Point(53, 3);
            this.textBox_StopIndex.Name = "textBox_StopIndex";
            this.textBox_StopIndex.Size = new System.Drawing.Size(66, 20);
            this.textBox_StopIndex.Suggestion = "XXX";
            this.textBox_StopIndex.TabIndex = 34;
            this.textBox_StopIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxStatus.Location = new System.Drawing.Point(4, 3);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.Size = new System.Drawing.Size(43, 20);
            this.textBoxStatus.Suggestion = "X";
            this.textBoxStatus.TabIndex = 1;
            this.textBoxStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DS021CRowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_apply);
            this.Controls.Add(this.textBox_TransferSymbols);
            this.Controls.Add(this.textBox_Transfers);
            this.Controls.Add(this.textBox_StopName);
            this.Controls.Add(this.textBox_StopIndex);
            this.Controls.Add(this.textBoxStatus);
            this.Name = "DS021CRowControl";
            this.Size = new System.Drawing.Size(689, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SuggestTextBox textBoxStatus;
        private SuggestTextBox textBox_StopIndex;
        private SuggestTextBox textBox_StopName;
        private SuggestTextBox textBox_Transfers;
        private SuggestTextBox textBox_TransferSymbols;
        private ApplySendButton button_apply;
    }
}
