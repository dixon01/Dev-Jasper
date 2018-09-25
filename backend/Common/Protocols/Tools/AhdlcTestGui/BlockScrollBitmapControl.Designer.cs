namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    partial class BlockScrollBitmapControl
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
            this.tabPageBackground = new System.Windows.Forms.TabPage();
            this.backgroundCreatorControl = new Gorba.Common.Protocols.Tools.AhdlcTestGui.BitmapCreatorControl();
            this.tabPageScrollBlock1 = new System.Windows.Forms.TabPage();
            this.scrollBlockEditor1 = new Gorba.Common.Protocols.Tools.AhdlcTestGui.ScrollBlockEditorControl();
            this.tabPageScrollBlock2 = new System.Windows.Forms.TabPage();
            this.scrollBlockEditor2 = new Gorba.Common.Protocols.Tools.AhdlcTestGui.ScrollBlockEditorControl();
            this.tabControl1.SuspendLayout();
            this.tabPageBackground.SuspendLayout();
            this.tabPageScrollBlock1.SuspendLayout();
            this.tabPageScrollBlock2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPageBackground);
            this.tabControl1.Controls.Add(this.tabPageScrollBlock1);
            this.tabControl1.Controls.Add(this.tabPageScrollBlock2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(500, 371);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPageBackground
            // 
            this.tabPageBackground.Controls.Add(this.backgroundCreatorControl);
            this.tabPageBackground.Location = new System.Drawing.Point(4, 25);
            this.tabPageBackground.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageBackground.Name = "tabPageBackground";
            this.tabPageBackground.Size = new System.Drawing.Size(492, 342);
            this.tabPageBackground.TabIndex = 0;
            this.tabPageBackground.Text = "Background";
            this.tabPageBackground.UseVisualStyleBackColor = true;
            // 
            // backgroundCreatorControl
            // 
            this.backgroundCreatorControl.BitmapSize = new System.Drawing.Size(16, 112);
            this.backgroundCreatorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundCreatorControl.HasColor = false;
            this.backgroundCreatorControl.Location = new System.Drawing.Point(0, 0);
            this.backgroundCreatorControl.Margin = new System.Windows.Forms.Padding(0);
            this.backgroundCreatorControl.Name = "backgroundCreatorControl";
            this.backgroundCreatorControl.Size = new System.Drawing.Size(492, 342);
            this.backgroundCreatorControl.TabIndex = 0;
            // 
            // tabPageScrollBlock1
            // 
            this.tabPageScrollBlock1.Controls.Add(this.scrollBlockEditor1);
            this.tabPageScrollBlock1.Location = new System.Drawing.Point(4, 25);
            this.tabPageScrollBlock1.Name = "tabPageScrollBlock1";
            this.tabPageScrollBlock1.Size = new System.Drawing.Size(492, 342);
            this.tabPageScrollBlock1.TabIndex = 1;
            this.tabPageScrollBlock1.Text = "Scroll Block 1";
            this.tabPageScrollBlock1.UseVisualStyleBackColor = true;
            // 
            // scrollBlockEditor1
            // 
            this.scrollBlockEditor1.BlockEnabled = true;
            this.scrollBlockEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollBlockEditor1.LargeWidthSupported = false;
            this.scrollBlockEditor1.Location = new System.Drawing.Point(0, 0);
            this.scrollBlockEditor1.Margin = new System.Windows.Forms.Padding(0);
            this.scrollBlockEditor1.Name = "scrollBlockEditor1";
            this.scrollBlockEditor1.ScrollSpeedSupported = false;
            this.scrollBlockEditor1.Size = new System.Drawing.Size(492, 342);
            this.scrollBlockEditor1.TabIndex = 0;
            // 
            // tabPageScrollBlock2
            // 
            this.tabPageScrollBlock2.Controls.Add(this.scrollBlockEditor2);
            this.tabPageScrollBlock2.Location = new System.Drawing.Point(4, 25);
            this.tabPageScrollBlock2.Name = "tabPageScrollBlock2";
            this.tabPageScrollBlock2.Size = new System.Drawing.Size(492, 342);
            this.tabPageScrollBlock2.TabIndex = 2;
            this.tabPageScrollBlock2.Text = "Scroll Block 2";
            this.tabPageScrollBlock2.UseVisualStyleBackColor = true;
            // 
            // scrollBlockEditor2
            // 
            this.scrollBlockEditor2.BlockEnabled = false;
            this.scrollBlockEditor2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollBlockEditor2.LargeWidthSupported = false;
            this.scrollBlockEditor2.Location = new System.Drawing.Point(0, 0);
            this.scrollBlockEditor2.Margin = new System.Windows.Forms.Padding(0);
            this.scrollBlockEditor2.Name = "scrollBlockEditor2";
            this.scrollBlockEditor2.ScrollSpeedSupported = false;
            this.scrollBlockEditor2.Size = new System.Drawing.Size(492, 342);
            this.scrollBlockEditor2.TabIndex = 1;
            // 
            // BlockScrollBitmapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.tabControl1);
            this.Name = "BlockScrollBitmapControl";
            this.Controls.SetChildIndex(this.buttonSend, 0);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            this.tabControl1.ResumeLayout(false);
            this.tabPageBackground.ResumeLayout(false);
            this.tabPageScrollBlock1.ResumeLayout(false);
            this.tabPageScrollBlock2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageBackground;
        private BitmapCreatorControl backgroundCreatorControl;
        private System.Windows.Forms.TabPage tabPageScrollBlock1;
        private ScrollBlockEditorControl scrollBlockEditor1;
        private System.Windows.Forms.TabPage tabPageScrollBlock2;
        private ScrollBlockEditorControl scrollBlockEditor2;

    }
}
