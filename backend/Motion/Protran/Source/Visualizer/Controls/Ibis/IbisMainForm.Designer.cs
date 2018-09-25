namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    partial class IbisMainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ibisTabControl1 = new Gorba.Motion.Protran.Visualizer.Controls.Ibis.IbisTabControl();
            this.SuspendLayout();
            // 
            // ibisTabControl1
            // 
            this.ibisTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibisTabControl1.Location = new System.Drawing.Point(0, 0);
            this.ibisTabControl1.Name = "ibisTabControl1";
            this.ibisTabControl1.Size = new System.Drawing.Size(943, 550);
            this.ibisTabControl1.TabIndex = 0;
            // 
            // IbisMainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(943, 550);
            this.Controls.Add(this.ibisTabControl1);
            this.Name = "IbisMainForm";
            this.Text = "IBIS Protocol";
            this.ResumeLayout(false);

        }

        #endregion

        private IbisTabControl ibisTabControl1;

    }
}

