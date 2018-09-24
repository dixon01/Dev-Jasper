namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    partial class VMxDigitalClock
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMxDigitalClock));
          this.lblTime = new System.Windows.Forms.Label();
          this.timer1 = new System.Windows.Forms.Timer();
          this.SuspendLayout();
          // 
          // lblTime
          // 
          resources.ApplyResources(this.lblTime, "lblTime");
          this.lblTime.ForeColor = System.Drawing.Color.White;
          this.lblTime.Name = "lblTime";
          // 
          // timer1
          // 
          this.timer1.Enabled = true;
          this.timer1.Interval = 1000;
          this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
          // 
          // VMxDigitalClock
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
          this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(32)))), ((int)(((byte)(134)))));
          this.Controls.Add(this.lblTime);
          this.ForeColor = System.Drawing.Color.Black;
          this.Name = "VMxDigitalClock";
          resources.ApplyResources(this, "$this");
          this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Timer timer1;
    }
}
