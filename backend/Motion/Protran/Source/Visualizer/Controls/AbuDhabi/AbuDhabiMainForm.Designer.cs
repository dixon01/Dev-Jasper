namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    partial class AbuDhabiMainForm
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
            this.abuDhabiTabControl1 = new Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi.AbuDhabiTabControl();
            this.SuspendLayout();
            // 
            // abuDhabiTabControl1
            // 
            this.abuDhabiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.abuDhabiTabControl1.Location = new System.Drawing.Point(0, 0);
            this.abuDhabiTabControl1.Name = "abuDhabiTabControl1";
            this.abuDhabiTabControl1.Size = new System.Drawing.Size(802, 509);
            this.abuDhabiTabControl1.TabIndex = 0;
            // 
            // AbuDhabiMainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(802, 509);
            this.Controls.Add(this.abuDhabiTabControl1);
            this.Name = "AbuDhabiMainForm";
            this.Text = "Abu Dhabi Protocol";
            this.ResumeLayout(false);

        }

        #endregion

        private AbuDhabiTabControl abuDhabiTabControl1;
    }
}