namespace Gorba.Common.Gioom.Tools.Controls
{
    partial class FlagPortStateControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBoxValue = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxValue
            // 
            this.checkBoxValue.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxValue.Location = new System.Drawing.Point(3, 3);
            this.checkBoxValue.Name = "checkBoxValue";
            this.checkBoxValue.Size = new System.Drawing.Size(104, 24);
            this.checkBoxValue.TabIndex = 0;
            this.checkBoxValue.Text = "Off";
            this.checkBoxValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxValue.UseVisualStyleBackColor = true;
            this.checkBoxValue.CheckedChanged += new System.EventHandler(this.CheckBoxValueOnCheckedChanged);
            // 
            // FlagPortStateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxValue);
            this.Name = "FlagPortStateControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxValue;
    }
}
