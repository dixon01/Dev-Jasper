namespace Gorba.Common.Gioom.Tools.Controls
{
    partial class PortStateWrapperControl
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
            this.enumPortStateControl1 = new EnumPortStateControl();
            this.flagPortStateControl1 = new FlagPortStateControl();
            this.integerPortStateControl1 = new IntegerPortStateControl();
            this.enumFlagPortStateControl1 = new EnumFlagPortStateControl();
            this.SuspendLayout();
            // 
            // enumPortStateControl1
            // 
            this.enumPortStateControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.enumPortStateControl1.Location = new System.Drawing.Point(0, 0);
            this.enumPortStateControl1.Name = "enumPortStateControl1";
            this.enumPortStateControl1.Size = new System.Drawing.Size(150, 150);
            this.enumPortStateControl1.TabIndex = 0;
            this.enumPortStateControl1.Visible = false;
            // 
            // flagPortStateControl1
            // 
            this.flagPortStateControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flagPortStateControl1.Location = new System.Drawing.Point(0, 0);
            this.flagPortStateControl1.Name = "flagPortStateControl1";
            this.flagPortStateControl1.Size = new System.Drawing.Size(150, 150);
            this.flagPortStateControl1.TabIndex = 1;
            this.flagPortStateControl1.Visible = false;
            // 
            // integerPortStateControl1
            // 
            this.integerPortStateControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.integerPortStateControl1.Location = new System.Drawing.Point(0, 0);
            this.integerPortStateControl1.Name = "integerPortStateControl1";
            this.integerPortStateControl1.Size = new System.Drawing.Size(150, 150);
            this.integerPortStateControl1.TabIndex = 2;
            this.integerPortStateControl1.Visible = false;
            // 
            // enumFlagPortStateControl1
            // 
            this.enumFlagPortStateControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.enumFlagPortStateControl1.Location = new System.Drawing.Point(0, 0);
            this.enumFlagPortStateControl1.Name = "enumFlagPortStateControl1";
            this.enumFlagPortStateControl1.Size = new System.Drawing.Size(150, 150);
            this.enumFlagPortStateControl1.TabIndex = 3;
            // 
            // PortStateWrapperControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.enumFlagPortStateControl1);
            this.Controls.Add(this.integerPortStateControl1);
            this.Controls.Add(this.flagPortStateControl1);
            this.Controls.Add(this.enumPortStateControl1);
            this.Name = "PortStateWrapperControl";
            this.ResumeLayout(false);

        }

        #endregion

        private EnumPortStateControl enumPortStateControl1;
        private FlagPortStateControl flagPortStateControl1;
        private IntegerPortStateControl integerPortStateControl1;
        private EnumFlagPortStateControl enumFlagPortStateControl1;
    }
}
