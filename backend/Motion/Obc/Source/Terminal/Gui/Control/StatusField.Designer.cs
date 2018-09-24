namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    partial class StatusField
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusField));
          this.lbl1 = new System.Windows.Forms.Label();
          this.SuspendLayout();
          // 
          // lbl1
          // 
          resources.ApplyResources(this.lbl1, "lbl1");
          this.lbl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(32)))), ((int)(((byte)(134)))));
          this.lbl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
          this.lbl1.Name = "lbl1";
          // 
          // StatusField
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
          this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(32)))), ((int)(((byte)(134)))));
          this.Controls.Add(this.lbl1);
          resources.ApplyResources(this, "$this");
          this.Name = "StatusField";
          this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl1;
    }
}
