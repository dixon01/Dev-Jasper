namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    partial class ColorBitmapControl
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
            this.bitmapCreatorControl = new Gorba.Common.Protocols.Tools.AhdlcTestGui.BitmapCreatorControl();
            this.SuspendLayout();
            // 
            // bitmapCreatorControl
            // 
            this.bitmapCreatorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bitmapCreatorControl.BitmapSize = new System.Drawing.Size(112, 16);
            this.bitmapCreatorControl.HasColor = true;
            this.bitmapCreatorControl.Location = new System.Drawing.Point(0, 0);
            this.bitmapCreatorControl.Name = "bitmapCreatorControl";
            this.bitmapCreatorControl.Size = new System.Drawing.Size(500, 368);
            this.bitmapCreatorControl.TabIndex = 2;
            // 
            // ColorBitmapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bitmapCreatorControl);
            this.Name = "ColorBitmapControl";
            this.Controls.SetChildIndex(this.buttonSend, 0);
            this.Controls.SetChildIndex(this.bitmapCreatorControl, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapCreatorControl bitmapCreatorControl;
    }
}
