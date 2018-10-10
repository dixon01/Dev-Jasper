namespace Gorba.Motion.Protran.Visualizer.Controls
{
    partial class GenericDataControl
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
            this.genericDataTabControl1 = new Gorba.Motion.Protran.Controls.GenericDataTabControl();
            this.buttonClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // genericDataTabControl1
            // 
            this.genericDataTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.genericDataTabControl1.Dictionary = null;
            this.genericDataTabControl1.Location = new System.Drawing.Point(0, 32);
            this.genericDataTabControl1.Name = "genericDataTabControl1";
            this.genericDataTabControl1.SelectedIndex = 0;
            this.genericDataTabControl1.Size = new System.Drawing.Size(195, 158);
            this.genericDataTabControl1.TabIndex = 0;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(3, 3);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 1;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClearClick);
            // 
            // GenericDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.genericDataTabControl1);
            this.Name = "GenericDataControl";
            this.Size = new System.Drawing.Size(195, 190);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClear;
        protected Protran.Controls.GenericDataTabControl genericDataTabControl1;
    }
}
