namespace AccessViolation
{
    partial class AccessViolation
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
            this.buttonAccessViolation = new System.Windows.Forms.Button();
            this.buttonSupressMessages = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAccessViolation
            // 
            this.buttonAccessViolation.Location = new System.Drawing.Point(12, 41);
            this.buttonAccessViolation.Name = "buttonAccessViolation";
            this.buttonAccessViolation.Size = new System.Drawing.Size(159, 24);
            this.buttonAccessViolation.TabIndex = 0;
            this.buttonAccessViolation.Text = "Create Access Violation";
            this.buttonAccessViolation.UseVisualStyleBackColor = true;
            this.buttonAccessViolation.Click += new System.EventHandler(this.ButtonAccessViolationClick);
            // 
            // buttonSupressMessages
            // 
            this.buttonSupressMessages.Location = new System.Drawing.Point(12, 12);
            this.buttonSupressMessages.Name = "buttonSupressMessages";
            this.buttonSupressMessages.Size = new System.Drawing.Size(159, 23);
            this.buttonSupressMessages.TabIndex = 1;
            this.buttonSupressMessages.Text = "Supress Messages";
            this.buttonSupressMessages.UseVisualStyleBackColor = true;
            this.buttonSupressMessages.Click += new System.EventHandler(this.ButtonSupressMessagesClick);
            // 
            // AccessViolation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.buttonSupressMessages);
            this.Controls.Add(this.buttonAccessViolation);
            this.Name = "AccessViolation";
            this.Text = "AccessViolation";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAccessViolation;
        private System.Windows.Forms.Button buttonSupressMessages;
    }
}

