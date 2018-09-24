namespace Gorba.Common.Medi.TestGui
{
    partial class MediAddressEditor
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.textBoxUnitName = new System.Windows.Forms.TextBox();
            this.textBoxAppName = new System.Windows.Forms.TextBox();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.textBoxUnitName);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.textBoxAppName);
            this.splitContainer.Size = new System.Drawing.Size(301, 23);
            this.splitContainer.SplitterDistance = 142;
            this.splitContainer.TabIndex = 8;
            // 
            // textBoxUnitName
            // 
            this.textBoxUnitName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxUnitName.Location = new System.Drawing.Point(0, 0);
            this.textBoxUnitName.Name = "textBoxUnitName";
            this.textBoxUnitName.Size = new System.Drawing.Size(142, 20);
            this.textBoxUnitName.TabIndex = 0;
            this.textBoxUnitName.TextChanged += new System.EventHandler(this.TextBoxOnTextChanged);
            // 
            // textBoxAppName
            // 
            this.textBoxAppName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAppName.Location = new System.Drawing.Point(0, 0);
            this.textBoxAppName.Name = "textBoxAppName";
            this.textBoxAppName.Size = new System.Drawing.Size(155, 20);
            this.textBoxAppName.TabIndex = 1;
            this.textBoxAppName.TextChanged += new System.EventHandler(this.TextBoxOnTextChanged);
            // 
            // MediAddressEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "MediAddressEditor";
            this.Size = new System.Drawing.Size(301, 23);
            this.EnabledChanged += new System.EventHandler(this.OnEnabledChanged);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox textBoxUnitName;
        private System.Windows.Forms.TextBox textBoxAppName;
    }
}
