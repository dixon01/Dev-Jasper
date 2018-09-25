namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    partial class MainField
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
            this.buttonMenu = new Gorba.Motion.Obc.Terminal.C74.Controls.ButtonInput();
            this.messageBox = new Gorba.Motion.Obc.Terminal.C74.Controls.MessageBoxControl();
            this.progressBox = new Gorba.Motion.Obc.Terminal.C74.Controls.ProgressBoxControl();
            this.SuspendLayout();
            // 
            // buttonMenu
            // 
            this.buttonMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMenu.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.buttonMenu.IsSelected = false;
            this.buttonMenu.Location = new System.Drawing.Point(456, 331);
            this.buttonMenu.Name = "buttonMenu";
            this.buttonMenu.Padding = new System.Windows.Forms.Padding(3);
            this.buttonMenu.Size = new System.Drawing.Size(80, 40);
            this.buttonMenu.TabIndex = 9999;
            this.buttonMenu.Text = "Menu";
            this.buttonMenu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonMenu.Pressed += new System.EventHandler(this.ButtonMenuOnPressed);
            // 
            // messageBox
            // 
            this.messageBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.messageBox.BackColor = System.Drawing.Color.White;
            this.messageBox.Caption = "Caption";
            this.messageBox.IsSelected = false;
            this.messageBox.Location = new System.Drawing.Point(9, 50);
            this.messageBox.Message = "Message Text";
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(530, 240);
            this.messageBox.TabIndex = 10000;
            this.messageBox.TitleColor = System.Drawing.Color.Blue;
            this.messageBox.Visible = false;
            this.messageBox.OkPressed += new System.EventHandler(this.MessageBoxOnOkPressed);
            // 
            // progressBox
            // 
            this.progressBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.progressBox.BackColor = System.Drawing.Color.White;
            this.progressBox.Caption = "Caption";
            this.progressBox.Location = new System.Drawing.Point(9, 50);
            this.progressBox.Name = "progressBox";
            this.progressBox.Size = new System.Drawing.Size(530, 240);
            this.progressBox.TabIndex = 10001;
            this.progressBox.TitleColor = System.Drawing.Color.Blue;
            this.progressBox.Visible = false;
            this.progressBox.Stopped += new System.EventHandler(this.ProgressBoxOnStopped);
            // 
            // MainField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.progressBox);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.buttonMenu);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MainField";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(549, 384);
            this.ResumeLayout(false);

        }

        #endregion

        protected ButtonInput buttonMenu;
        private MessageBoxControl messageBox;
        private ProgressBoxControl progressBox;

    }
}
