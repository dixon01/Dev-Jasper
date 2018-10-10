namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    partial class DriveSelectMainField
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
            this.labelCaption = new System.Windows.Forms.Label();
            this.buttonInput1 = new Gorba.Motion.Obc.Terminal.C74.Controls.ButtonInput();
            this.buttonInput2 = new Gorba.Motion.Obc.Terminal.C74.Controls.ButtonInput();
            this.buttonInput3 = new Gorba.Motion.Obc.Terminal.C74.Controls.ButtonInput();
            this.buttonSchool = new Gorba.Motion.Obc.Terminal.C74.Controls.ToggleButtonInput();
            this.buttonAdditional = new Gorba.Motion.Obc.Terminal.C74.Controls.ToggleButtonInput();
            this.SuspendLayout();
            // 
            // labelCaption
            // 
            this.labelCaption.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.labelCaption.ForeColor = System.Drawing.Color.White;
            this.labelCaption.Location = new System.Drawing.Point(10, 10);
            this.labelCaption.Margin = new System.Windows.Forms.Padding(0);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(300, 36);
            this.labelCaption.TabIndex = 10000;
            // 
            // buttonInput1
            // 
            this.buttonInput1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonInput1.IsSelected = false;
            this.buttonInput1.Location = new System.Drawing.Point(10, 56);
            this.buttonInput1.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonInput1.Name = "buttonInput1";
            this.buttonInput1.Padding = new System.Windows.Forms.Padding(3);
            this.buttonInput1.Size = new System.Drawing.Size(250, 40);
            this.buttonInput1.TabIndex = 1;
            this.buttonInput1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonInput1.Visible = false;
            this.buttonInput1.Pressed += new System.EventHandler(this.ButtonInput1OnPressed);
            // 
            // buttonInput2
            // 
            this.buttonInput2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonInput2.IsSelected = false;
            this.buttonInput2.Location = new System.Drawing.Point(10, 106);
            this.buttonInput2.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonInput2.Name = "buttonInput2";
            this.buttonInput2.Padding = new System.Windows.Forms.Padding(3);
            this.buttonInput2.Size = new System.Drawing.Size(250, 40);
            this.buttonInput2.TabIndex = 2;
            this.buttonInput2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonInput2.Visible = false;
            this.buttonInput2.Pressed += new System.EventHandler(this.ButtonInput2OnPressed);
            // 
            // buttonInput3
            // 
            this.buttonInput3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonInput3.IsSelected = false;
            this.buttonInput3.Location = new System.Drawing.Point(10, 156);
            this.buttonInput3.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonInput3.Name = "buttonInput3";
            this.buttonInput3.Padding = new System.Windows.Forms.Padding(3);
            this.buttonInput3.Size = new System.Drawing.Size(250, 40);
            this.buttonInput3.TabIndex = 3;
            this.buttonInput3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonInput3.Visible = false;
            this.buttonInput3.Pressed += new System.EventHandler(this.ButtonInput3OnPressed);
            // 
            // buttonSchool
            // 
            this.buttonSchool.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonSchool.IsChecked = false;
            this.buttonSchool.IsSelected = false;
            this.buttonSchool.Location = new System.Drawing.Point(10, 331);
            this.buttonSchool.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.buttonSchool.Name = "buttonSchool";
            this.buttonSchool.Padding = new System.Windows.Forms.Padding(3);
            this.buttonSchool.Size = new System.Drawing.Size(80, 40);
            this.buttonSchool.TabIndex = 4;
            this.buttonSchool.Text = "L";
            this.buttonSchool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonAdditional
            // 
            this.buttonAdditional.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonAdditional.IsChecked = false;
            this.buttonAdditional.IsSelected = false;
            this.buttonAdditional.Location = new System.Drawing.Point(100, 331);
            this.buttonAdditional.Margin = new System.Windows.Forms.Padding(0);
            this.buttonAdditional.Name = "buttonAdditional";
            this.buttonAdditional.Padding = new System.Windows.Forms.Padding(3);
            this.buttonAdditional.Size = new System.Drawing.Size(80, 40);
            this.buttonAdditional.TabIndex = 5;
            this.buttonAdditional.Text = "+";
            this.buttonAdditional.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DriveSelectMainField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonAdditional);
            this.Controls.Add(this.buttonSchool);
            this.Controls.Add(this.buttonInput3);
            this.Controls.Add(this.buttonInput2);
            this.Controls.Add(this.buttonInput1);
            this.Controls.Add(this.labelCaption);
            this.Name = "DriveSelectMainField";
            this.Controls.SetChildIndex(this.buttonMenu, 0);
            this.Controls.SetChildIndex(this.labelCaption, 0);
            this.Controls.SetChildIndex(this.buttonInput1, 0);
            this.Controls.SetChildIndex(this.buttonInput2, 0);
            this.Controls.SetChildIndex(this.buttonInput3, 0);
            this.Controls.SetChildIndex(this.buttonSchool, 0);
            this.Controls.SetChildIndex(this.buttonAdditional, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelCaption;
        private ButtonInput buttonInput1;
        private ButtonInput buttonInput2;
        private ButtonInput buttonInput3;
        private ToggleButtonInput buttonSchool;
        private ToggleButtonInput buttonAdditional;
    }
}
