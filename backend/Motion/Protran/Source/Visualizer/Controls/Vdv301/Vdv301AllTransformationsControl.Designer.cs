namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    partial class Vdv301AllTransformationsControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxTransformations = new System.Windows.Forms.ListBox();
            this.vdv301TransformationControl = new Gorba.Motion.Protran.Visualizer.Controls.Vdv301.Vdv301TransformationControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxTransformations);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.vdv301TransformationControl);
            this.splitContainer1.Size = new System.Drawing.Size(807, 514);
            this.splitContainer1.SplitterDistance = 269;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBoxTransformations
            // 
            this.listBoxTransformations.DisplayMember = "ChainName";
            this.listBoxTransformations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTransformations.FormattingEnabled = true;
            this.listBoxTransformations.IntegralHeight = false;
            this.listBoxTransformations.Location = new System.Drawing.Point(0, 0);
            this.listBoxTransformations.Name = "listBoxTransformations";
            this.listBoxTransformations.Size = new System.Drawing.Size(269, 514);
            this.listBoxTransformations.TabIndex = 0;
            this.listBoxTransformations.SelectedIndexChanged += new System.EventHandler(this.ListBoxTransformationsOnSelectedIndexChanged);
            // 
            // vdv301TransformationControl
            // 
            this.vdv301TransformationControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vdv301TransformationControl.Location = new System.Drawing.Point(0, 0);
            this.vdv301TransformationControl.Name = "vdv301TransformationControl";
            this.vdv301TransformationControl.Size = new System.Drawing.Size(534, 514);
            this.vdv301TransformationControl.TabIndex = 0;
            // 
            // Vdv301AllTransformationsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Vdv301AllTransformationsControl";
            this.Size = new System.Drawing.Size(807, 514);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBoxTransformations;
        private Gorba.Motion.Protran.Visualizer.Controls.Vdv301.Vdv301TransformationControl vdv301TransformationControl;
    }
}
