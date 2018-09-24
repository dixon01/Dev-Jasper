namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    partial class ProgressBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressBar));
            this.timer1 = new System.Windows.Forms.Timer();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pnlCaption = new System.Windows.Forms.Panel();
            this.lblCaption = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.pnlCaption.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.pnlMain.Controls.Add(this.progressBar1);
            this.pnlMain.Controls.Add(this.pnlCaption);
            resources.ApplyResources(this.pnlMain, "pnlMain");
            this.pnlMain.Name = "pnlMain";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // pnlCaption
            // 
            this.pnlCaption.BackColor = System.Drawing.Color.Blue;
            this.pnlCaption.Controls.Add(this.lblCaption);
            resources.ApplyResources(this.pnlCaption, "pnlCaption");
            this.pnlCaption.Name = "pnlCaption";
            // 
            // lblCaption
            // 
            this.lblCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(32)))), ((int)(((byte)(134)))));
            resources.ApplyResources(this.lblCaption, "lblCaption");
            this.lblCaption.ForeColor = System.Drawing.Color.White;
            this.lblCaption.Name = "lblCaption";
            // 
            // VMxProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlMain);
            resources.ApplyResources(this, "$this");
            this.Name = "VMxProgressBar";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ProgressBarPaint);
            this.EnabledChanged += new System.EventHandler(this.ProgressBarEnabledChanged);
            this.pnlMain.ResumeLayout(false);
            this.pnlCaption.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlCaption;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}
