namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using Gorba.Motion.Obc.Terminal.Gui.Control;

    partial class DriveWaitScreen
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DriveWaitScreen));
          this.lblDestinationStop = new System.Windows.Forms.Label();
          this.lblDestinationStart = new System.Windows.Forms.Label();
          this.lblStartTime = new System.Windows.Forms.Label();
          this.pnlLight = new System.Windows.Forms.Panel();
          this.timer1 = new System.Windows.Forms.Timer();
          this.txtStartTime = new Gorba.Motion.Obc.Terminal.Gui.Control.CustomTextbox();
          this.txtStartStop = new Gorba.Motion.Obc.Terminal.Gui.Control.CustomTextbox();
          this.txtDestinationStop = new Gorba.Motion.Obc.Terminal.Gui.Control.CustomTextbox();
          this.SuspendLayout();
          // 
          // lblDestinationStop
          // 
          resources.ApplyResources(this.lblDestinationStop, "lblDestinationStop");
          this.lblDestinationStop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.lblDestinationStop.Name = "lblDestinationStop";
          // 
          // lblDestinationStart
          // 
          resources.ApplyResources(this.lblDestinationStart, "lblDestinationStart");
          this.lblDestinationStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.lblDestinationStart.Name = "lblDestinationStart";
          // 
          // lblStartTime
          // 
          resources.ApplyResources(this.lblStartTime, "lblStartTime");
          this.lblStartTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.lblStartTime.Name = "lblStartTime";
          // 
          // pnlLight
          // 
          this.pnlLight.BackColor = System.Drawing.Color.Red;
          resources.ApplyResources(this.pnlLight, "pnlLight");
          this.pnlLight.Name = "pnlLight";
          // 
          // timer1
          // 
          this.timer1.Interval = 1000;
          this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
          // 
          // txtStartTime
          // 
          this.txtStartTime.BackColor = System.Drawing.SystemColors.ControlDarkDark;
          this.txtStartTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
          this.txtStartTime.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
          resources.ApplyResources(this.txtStartTime, "txtStartTime");
          this.txtStartTime.ForeColor = System.Drawing.Color.White;
          this.txtStartTime.Name = "txtStartTime";
          // 
          // txtStartStop
          // 
          this.txtStartStop.BackColor = System.Drawing.SystemColors.ControlDarkDark;
          this.txtStartStop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
          this.txtStartStop.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
          resources.ApplyResources(this.txtStartStop, "txtStartStop");
          this.txtStartStop.ForeColor = System.Drawing.Color.White;
          this.txtStartStop.Name = "txtStartStop";
          // 
          // txtDestinationStop
          // 
          this.txtDestinationStop.BackColor = System.Drawing.SystemColors.ControlDarkDark;
          this.txtDestinationStop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
          this.txtDestinationStop.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
          resources.ApplyResources(this.txtDestinationStop, "txtDestinationStop");
          this.txtDestinationStop.ForeColor = System.Drawing.Color.White;
          this.txtDestinationStop.Name = "txtDestinationStop";
          // 
          // DriveWaitScreen
          // 
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
          this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
          this.Controls.Add(this.pnlLight);
          this.Controls.Add(this.txtStartTime);
          this.Controls.Add(this.lblStartTime);
          this.Controls.Add(this.txtStartStop);
          this.Controls.Add(this.lblDestinationStart);
          this.Controls.Add(this.txtDestinationStop);
          this.Controls.Add(this.lblDestinationStop);
          this.Name = "DriveWaitScreen";
          resources.ApplyResources(this, "$this");
          this.ResumeLayout(false);

        }

        #endregion

        internal CustomTextbox txtDestinationStop;
        private System.Windows.Forms.Label lblDestinationStop;
        internal CustomTextbox txtStartStop;
        private System.Windows.Forms.Label lblDestinationStart;
        internal CustomTextbox txtStartTime;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Panel pnlLight;
        private System.Windows.Forms.Timer timer1;

    }
}
