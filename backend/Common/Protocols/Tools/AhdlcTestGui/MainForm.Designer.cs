namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    partial class MainForm
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.checkBoxHighSpeed = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(225, 12);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start Master";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.ButtonStartClick);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Location = new System.Drawing.Point(12, 41);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(846, 476);
            this.tabControl.TabIndex = 1;
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPorts.FormattingEnabled = true;
            this.comboBoxPorts.Location = new System.Drawing.Point(12, 13);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(79, 21);
            this.comboBoxPorts.TabIndex = 2;
            this.comboBoxPorts.SelectedIndexChanged += new System.EventHandler(this.ComboBoxPortsSelectedIndexChanged);
            // 
            // checkBoxHighSpeed
            // 
            this.checkBoxHighSpeed.AutoSize = true;
            this.checkBoxHighSpeed.Checked = true;
            this.checkBoxHighSpeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHighSpeed.Location = new System.Drawing.Point(97, 16);
            this.checkBoxHighSpeed.Name = "checkBoxHighSpeed";
            this.checkBoxHighSpeed.Size = new System.Drawing.Size(122, 17);
            this.checkBoxHighSpeed.TabIndex = 3;
            this.checkBoxHighSpeed.Text = "Highspeed (RS-485)";
            this.checkBoxHighSpeed.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 529);
            this.Controls.Add(this.checkBoxHighSpeed);
            this.Controls.Add(this.comboBoxPorts);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonStart);
            this.Name = "MainForm";
            this.Text = "AHDLC Test GUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.CheckBox checkBoxHighSpeed;
    }
}

