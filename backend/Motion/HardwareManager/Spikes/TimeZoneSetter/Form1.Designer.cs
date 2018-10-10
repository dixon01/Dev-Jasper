namespace TimeZoneSetter
{
    partial class Form1
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
            this.listBoxTimeZones = new System.Windows.Forms.ListBox();
            this.buttonChangeTimeZone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxTimeZones
            // 
            this.listBoxTimeZones.FormattingEnabled = true;
            this.listBoxTimeZones.IntegralHeight = false;
            this.listBoxTimeZones.Location = new System.Drawing.Point(12, 12);
            this.listBoxTimeZones.Name = "listBoxTimeZones";
            this.listBoxTimeZones.Size = new System.Drawing.Size(325, 260);
            this.listBoxTimeZones.TabIndex = 0;
            this.listBoxTimeZones.SelectedIndexChanged += new System.EventHandler(this.ListBoxTimeZonesSelectedIndexChanged);
            // 
            // buttonChangeTimeZone
            // 
            this.buttonChangeTimeZone.Enabled = false;
            this.buttonChangeTimeZone.Location = new System.Drawing.Point(207, 278);
            this.buttonChangeTimeZone.Name = "buttonChangeTimeZone";
            this.buttonChangeTimeZone.Size = new System.Drawing.Size(130, 23);
            this.buttonChangeTimeZone.TabIndex = 1;
            this.buttonChangeTimeZone.Text = "Change Time Zone";
            this.buttonChangeTimeZone.UseVisualStyleBackColor = true;
            this.buttonChangeTimeZone.Click += new System.EventHandler(this.ButtonChangeTimeZoneClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 313);
            this.Controls.Add(this.buttonChangeTimeZone);
            this.Controls.Add(this.listBoxTimeZones);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxTimeZones;
        private System.Windows.Forms.Button buttonChangeTimeZone;
    }
}

