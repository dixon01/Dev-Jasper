namespace Gorba.Motion.Protran.Visualizer.Controls.IO
{
    partial class IOMainForm
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
            this.ioTabControl1 = new Gorba.Motion.Protran.Visualizer.Controls.IO.IOTabControl();
            this.SuspendLayout();
            // 
            // ioTabControl1
            // 
            this.ioTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ioTabControl1.Location = new System.Drawing.Point(0, 0);
            this.ioTabControl1.Name = "ioTabControl1";
            this.ioTabControl1.Size = new System.Drawing.Size(560, 469);
            this.ioTabControl1.TabIndex = 0;
            // 
            // IOMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 469);
            this.Controls.Add(this.ioTabControl1);
            this.Name = "IOMainForm";
            this.Text = "IO Protocol";
            this.ResumeLayout(false);

        }

        #endregion

        private IOTabControl ioTabControl1;




    }
}