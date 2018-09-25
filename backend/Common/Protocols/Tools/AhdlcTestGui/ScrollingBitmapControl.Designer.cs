namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    partial class ScrollingBitmapControl
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // bitmapCreatorControl
            // 
            this.bitmapCreatorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bitmapCreatorControl.BitmapSize = new System.Drawing.Size(16, 112);
            this.bitmapCreatorControl.HasColor = false;
            this.bitmapCreatorControl.Location = new System.Drawing.Point(3, 29);
            this.bitmapCreatorControl.Name = "bitmapCreatorControl";
            this.bitmapCreatorControl.Size = new System.Drawing.Size(494, 339);
            this.bitmapCreatorControl.TabIndex = 2;
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numericUpDownWidth.Location = new System.Drawing.Point(128, 3);
            this.numericUpDownWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(68, 20);
            this.numericUpDownWidth.TabIndex = 3;
            this.numericUpDownWidth.ValueChanged += new System.EventHandler(this.NumericUpDownWidthValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Width of Scrolling Text:";
            // 
            // ScrollingBitmapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownWidth);
            this.Controls.Add(this.bitmapCreatorControl);
            this.Name = "ScrollingBitmapControl";
            this.Controls.SetChildIndex(this.buttonSend, 0);
            this.Controls.SetChildIndex(this.bitmapCreatorControl, 0);
            this.Controls.SetChildIndex(this.numericUpDownWidth, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BitmapCreatorControl bitmapCreatorControl;
        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.Label label1;

    }
}
