namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    partial class Vdv301MainForm
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
            this.vdv301TabControl1 = new Gorba.Motion.Protran.Visualizer.Controls.Vdv301.Vdv301TabControl();
            this.SuspendLayout();
            // 
            // vdv301TabControl1
            // 
            this.vdv301TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vdv301TabControl1.Location = new System.Drawing.Point(0, 0);
            this.vdv301TabControl1.Name = "vdv301TabControl1";
            this.vdv301TabControl1.Size = new System.Drawing.Size(802, 509);
            this.vdv301TabControl1.TabIndex = 0;
            // 
            // Vdv301MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(802, 509);
            this.Controls.Add(this.vdv301TabControl1);
            this.Name = "Vdv301MainForm";
            this.Text = "VDV 301 Protocol";
            this.ResumeLayout(false);

        }

        #endregion

        private Vdv301TabControl vdv301TabControl1;

    }
}