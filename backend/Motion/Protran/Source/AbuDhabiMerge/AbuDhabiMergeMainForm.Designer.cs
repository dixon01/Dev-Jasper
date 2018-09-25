namespace Gorba.Motion.Protran.AbuDhabiMerge
{
    using System.Windows.Forms;

    partial class AbuDhabiMergeMainForm
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new Gorba.Motion.Protran.AbuDhabiMerge.TabControl();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Location = new System.Drawing.Point(9, 5);
            this.tabControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            this.tabControl.Name = "tabControl";
            this.tabControl.Size = new System.Drawing.Size(1002, 734);
            this.tabControl.TabIndex = 0;
            // 
            // AbuDhabiMergeMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 743);
            this.Controls.Add(this.tabControl);
            this.Name = "AbuDhabiMergeMainForm";
            this.Text = "AbuDhabiMerge";
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl tabControl;
    }
}