namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using Gorba.Motion.Obc.Terminal.Gui.Control;

    partial class DriveBlock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DriveBlock));
            this.lblLight = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtStation3 = new CustomTextbox();
            this.txtDelayTime = new CustomTextbox();
            this.txtStation2 = new CustomTextbox();
            this.txtStation1 = new CustomTextbox();
            this.SuspendLayout();
            // 
            // lblLight
            // 
            this.lblLight.BackColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.lblLight, "lblLight");
            this.lblLight.Name = "lblLight";
            // 
            // imageList1
            // 
            resources.ApplyResources(this.imageList1, "imageList1");
            this.imageList1.Images.Clear();
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource2"))));
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource3"))));
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource4"))));
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            // 
            // txtStation3
            // 
            this.txtStation3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtStation3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtStation3.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            resources.ApplyResources(this.txtStation3, "txtStation3");
            this.txtStation3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.txtStation3.Name = "txtStation3";
            // 
            // txtDelayTime
            // 
            this.txtDelayTime.BackColor = System.Drawing.Color.Black;
            this.txtDelayTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.txtDelayTime.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            resources.ApplyResources(this.txtDelayTime, "txtDelayTime");
            this.txtDelayTime.ForeColor = System.Drawing.Color.White;
            this.txtDelayTime.Name = "txtDelayTime";
            // 
            // txtStation2
            // 
            this.txtStation2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtStation2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtStation2.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            resources.ApplyResources(this.txtStation2, "txtStation2");
            this.txtStation2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.txtStation2.Name = "txtStation2";
            // 
            // txtStation1
            // 
            this.txtStation1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtStation1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtStation1.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            resources.ApplyResources(this.txtStation1, "txtStation1");
            this.txtStation1.ForeColor = System.Drawing.Color.White;
            this.txtStation1.Name = "txtStation1";
            // 
            // VMxDriveBlock
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtStation3);
            this.Controls.Add(this.lblLight);
            this.Controls.Add(this.txtDelayTime);
            this.Controls.Add(this.txtStation2);
            this.Controls.Add(this.txtStation1);
            this.Name = "VMxDriveBlock";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.PictureBox lblLight;
        internal CustomTextbox txtDelayTime;
        internal CustomTextbox txtStation2;
        internal CustomTextbox txtStation1;
        internal CustomTextbox txtStation3;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
