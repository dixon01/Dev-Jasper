namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    partial class PopupControl
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
            this.panelContent = new System.Windows.Forms.Panel();
            this.labelCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.panelContent.Location = new System.Drawing.Point(3, 39);
            this.panelContent.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.panelContent.Name = "panelContent";
            this.panelContent.Padding = new System.Windows.Forms.Padding(5);
            this.panelContent.Size = new System.Drawing.Size(524, 198);
            this.panelContent.TabIndex = 0;
            // 
            // labelCaption
            // 
            this.labelCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCaption.BackColor = System.Drawing.Color.Blue;
            this.labelCaption.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold);
            this.labelCaption.ForeColor = System.Drawing.Color.White;
            this.labelCaption.Location = new System.Drawing.Point(3, 3);
            this.labelCaption.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelCaption.Size = new System.Drawing.Size(524, 36);
            this.labelCaption.TabIndex = 1;
            this.labelCaption.Text = "Caption";
            this.labelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PopupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.labelCaption);
            this.Controls.Add(this.panelContent);
            this.Name = "PopupControl";
            this.Size = new System.Drawing.Size(530, 240);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelCaption;
        protected System.Windows.Forms.Panel panelContent;
    }
}
