namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    partial class NumberInputMainField
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
            this.numberInput1 = new Gorba.Motion.Obc.Terminal.C74.Controls.NumberInput();
            this.label1 = new System.Windows.Forms.Label();
            this.numberInput2 = new Gorba.Motion.Obc.Terminal.C74.Controls.NumberInput();
            this.label2 = new System.Windows.Forms.Label();
            this.labelHint = new System.Windows.Forms.Label();
            this.labelCaption = new System.Windows.Forms.Label();
            this.buttonOK = new Gorba.Motion.Obc.Terminal.C74.Controls.ButtonInput();
            this.buttonLanguage1 = new Gorba.Motion.Obc.Terminal.C74.Controls.ButtonInput();
            this.buttonLangauge2 = new Gorba.Motion.Obc.Terminal.C74.Controls.ButtonInput();
            this.buttonLanguage3 = new Gorba.Motion.Obc.Terminal.C74.Controls.ButtonInput();
            this.SuspendLayout();
            // 
            // numberInput1
            // 
            this.numberInput1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.numberInput1.IsEditing = false;
            this.numberInput1.IsSelected = false;
            this.numberInput1.Location = new System.Drawing.Point(10, 86);
            this.numberInput1.Margin = new System.Windows.Forms.Padding(0);
            this.numberInput1.Name = "numberInput1";
            this.numberInput1.Padding = new System.Windows.Forms.Padding(3);
            this.numberInput1.Size = new System.Drawing.Size(250, 40);
            this.numberInput1.TabIndex = 0;
            this.numberInput1.Value = 0;
            this.numberInput1.ValueChanged += new System.EventHandler(this.NumberInput1OnValueChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 18.5F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.label1.Location = new System.Drawing.Point(13, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 30);
            this.label1.TabIndex = 1;
            // 
            // numberInput2
            // 
            this.numberInput2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.numberInput2.IsEditing = false;
            this.numberInput2.IsSelected = false;
            this.numberInput2.Location = new System.Drawing.Point(10, 174);
            this.numberInput2.Margin = new System.Windows.Forms.Padding(0);
            this.numberInput2.Name = "numberInput2";
            this.numberInput2.Padding = new System.Windows.Forms.Padding(3);
            this.numberInput2.Size = new System.Drawing.Size(250, 40);
            this.numberInput2.TabIndex = 1;
            this.numberInput2.Value = 0;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 18.5F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.label2.Location = new System.Drawing.Point(13, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(300, 30);
            this.label2.TabIndex = 1;
            // 
            // labelHint
            // 
            this.labelHint.Font = new System.Drawing.Font("Arial", 18.5F);
            this.labelHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.labelHint.Location = new System.Drawing.Point(8, 231);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(300, 30);
            this.labelHint.TabIndex = 1;
            // 
            // labelCaption
            // 
            this.labelCaption.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.labelCaption.ForeColor = System.Drawing.Color.White;
            this.labelCaption.Location = new System.Drawing.Point(10, 10);
            this.labelCaption.Margin = new System.Windows.Forms.Padding(0);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(300, 36);
            this.labelCaption.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.5F);
            this.buttonOK.IsSelected = false;
            this.buttonOK.Location = new System.Drawing.Point(363, 331);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(10);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Padding = new System.Windows.Forms.Padding(3);
            this.buttonOK.Size = new System.Drawing.Size(80, 40);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonOK.Pressed += new System.EventHandler(this.ButtonOkOnPressed);
            // 
            // buttonLanguage1
            // 
            this.buttonLanguage1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonLanguage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.5F);
            this.buttonLanguage1.IsSelected = false;
            this.buttonLanguage1.Location = new System.Drawing.Point(263, 331);
            this.buttonLanguage1.Margin = new System.Windows.Forms.Padding(10);
            this.buttonLanguage1.Name = "buttonLanguage1";
            this.buttonLanguage1.Padding = new System.Windows.Forms.Padding(3);
            this.buttonLanguage1.Size = new System.Drawing.Size(80, 40);
            this.buttonLanguage1.TabIndex = 4;
            this.buttonLanguage1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonLanguage1.Visible = false;
            this.buttonLanguage1.Pressed += new System.EventHandler(this.ButtonLanguageOnPressed);
            // 
            // buttonLangauge2
            // 
            this.buttonLangauge2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonLangauge2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.5F);
            this.buttonLangauge2.IsSelected = false;
            this.buttonLangauge2.Location = new System.Drawing.Point(163, 331);
            this.buttonLangauge2.Margin = new System.Windows.Forms.Padding(10);
            this.buttonLangauge2.Name = "buttonLangauge2";
            this.buttonLangauge2.Padding = new System.Windows.Forms.Padding(3);
            this.buttonLangauge2.Size = new System.Drawing.Size(80, 40);
            this.buttonLangauge2.TabIndex = 3;
            this.buttonLangauge2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonLangauge2.Visible = false;
            this.buttonLangauge2.Pressed += new System.EventHandler(this.ButtonLanguageOnPressed);
            // 
            // buttonLanguage3
            // 
            this.buttonLanguage3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonLanguage3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18.5F);
            this.buttonLanguage3.IsSelected = false;
            this.buttonLanguage3.Location = new System.Drawing.Point(63, 331);
            this.buttonLanguage3.Margin = new System.Windows.Forms.Padding(10);
            this.buttonLanguage3.Name = "buttonLanguage3";
            this.buttonLanguage3.Padding = new System.Windows.Forms.Padding(3);
            this.buttonLanguage3.Size = new System.Drawing.Size(80, 40);
            this.buttonLanguage3.TabIndex = 2;
            this.buttonLanguage3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonLanguage3.Visible = false;
            this.buttonLanguage3.Pressed += new System.EventHandler(this.ButtonLanguageOnPressed);
            // 
            // NumberInputMainField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.buttonLanguage3);
            this.Controls.Add(this.buttonLangauge2);
            this.Controls.Add(this.buttonLanguage1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelHint);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numberInput2);
            this.Controls.Add(this.labelCaption);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numberInput1);
            this.Name = "NumberInputMainField";
            this.Controls.SetChildIndex(this.numberInput1, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.labelCaption, 0);
            this.Controls.SetChildIndex(this.numberInput2, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.labelHint, 0);
            this.Controls.SetChildIndex(this.buttonMenu, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonLanguage1, 0);
            this.Controls.SetChildIndex(this.buttonLangauge2, 0);
            this.Controls.SetChildIndex(this.buttonLanguage3, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private NumberInput numberInput1;
        private System.Windows.Forms.Label label1;
        private NumberInput numberInput2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelHint;
        private System.Windows.Forms.Label labelCaption;
        private ButtonInput buttonOK;
        private ButtonInput buttonLanguage1;
        private ButtonInput buttonLangauge2;
        private ButtonInput buttonLanguage3;
    }
}
