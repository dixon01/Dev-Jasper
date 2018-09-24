namespace Gorba.Motion.Obc.Terminal.C74
{
    using Gorba.Motion.Obc.Terminal.C74.Controls;

    partial class RootForm
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
            this.mainFieldPanel = new System.Windows.Forms.Panel();
            this.messageField = new Gorba.Motion.Obc.Terminal.C74.Controls.MessageField();
            this.statusField = new Gorba.Motion.Obc.Terminal.C74.Controls.StatusField();
            this.iconBar = new Gorba.Motion.Obc.Terminal.C74.Controls.IconBar();
            this.messageListMainField = new Gorba.Motion.Obc.Terminal.C74.Controls.MessageListMainField();
            this.blockDriveMainField = new Gorba.Motion.Obc.Terminal.C74.Controls.BlockDriveMainField();
            this.statusMainField = new Gorba.Motion.Obc.Terminal.C74.Controls.StatusMainField();
            this.driveWaitMainField = new Gorba.Motion.Obc.Terminal.C74.Controls.DriveWaitMainField();
            this.listMainField = new Gorba.Motion.Obc.Terminal.C74.Controls.ListMainField();
            this.driveSelectMainField = new Gorba.Motion.Obc.Terminal.C74.Controls.DriveSelectMainField();
            this.numberInputMainField = new Gorba.Motion.Obc.Terminal.C74.Controls.NumberInputMainField();
            this.mainFieldPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainFieldPanel
            // 
            this.mainFieldPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainFieldPanel.Controls.Add(this.messageListMainField);
            this.mainFieldPanel.Controls.Add(this.blockDriveMainField);
            this.mainFieldPanel.Controls.Add(this.statusMainField);
            this.mainFieldPanel.Controls.Add(this.driveWaitMainField);
            this.mainFieldPanel.Controls.Add(this.listMainField);
            this.mainFieldPanel.Controls.Add(this.driveSelectMainField);
            this.mainFieldPanel.Controls.Add(this.numberInputMainField);
            this.mainFieldPanel.Location = new System.Drawing.Point(0, 96);
            this.mainFieldPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainFieldPanel.Name = "mainFieldPanel";
            this.mainFieldPanel.Size = new System.Drawing.Size(549, 384);
            this.mainFieldPanel.TabIndex = 11;
            // 
            // messageField
            // 
            this.messageField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.messageField.IsSelected = false;
            this.messageField.Location = new System.Drawing.Point(0, 47);
            this.messageField.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.messageField.Name = "messageField";
            this.messageField.Size = new System.Drawing.Size(549, 48);
            this.messageField.TabIndex = 14;
            // 
            // statusField
            // 
            this.statusField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(32)))), ((int)(((byte)(134)))));
            this.statusField.Font = new System.Drawing.Font("Arial", 22F);
            this.statusField.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.statusField.Location = new System.Drawing.Point(0, 0);
            this.statusField.Margin = new System.Windows.Forms.Padding(0);
            this.statusField.Name = "statusField";
            this.statusField.Size = new System.Drawing.Size(640, 48);
            this.statusField.TabIndex = 13;
            // 
            // iconBar
            // 
            this.iconBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.iconBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.iconBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.iconBar.Location = new System.Drawing.Point(550, 47);
            this.iconBar.Margin = new System.Windows.Forms.Padding(0);
            this.iconBar.Name = "iconBar";
            this.iconBar.Size = new System.Drawing.Size(90, 433);
            this.iconBar.TabIndex = 12;
            // 
            // messageListMainField
            // 
            this.messageListMainField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageListMainField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.messageListMainField.Location = new System.Drawing.Point(0, 0);
            this.messageListMainField.Margin = new System.Windows.Forms.Padding(0);
            this.messageListMainField.Name = "messageListMainField";
            this.messageListMainField.Padding = new System.Windows.Forms.Padding(10);
            this.messageListMainField.Size = new System.Drawing.Size(549, 384);
            this.messageListMainField.TabIndex = 10;
            this.messageListMainField.Visible = false;
            // 
            // blockDriveMainField
            // 
            this.blockDriveMainField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blockDriveMainField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.blockDriveMainField.Location = new System.Drawing.Point(0, 0);
            this.blockDriveMainField.Margin = new System.Windows.Forms.Padding(0);
            this.blockDriveMainField.Name = "blockDriveMainField";
            this.blockDriveMainField.Padding = new System.Windows.Forms.Padding(10);
            this.blockDriveMainField.Size = new System.Drawing.Size(549, 384);
            this.blockDriveMainField.TabIndex = 9;
            this.blockDriveMainField.Visible = false;
            // 
            // statusMainField
            // 
            this.statusMainField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusMainField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.statusMainField.Location = new System.Drawing.Point(0, 0);
            this.statusMainField.Margin = new System.Windows.Forms.Padding(0);
            this.statusMainField.Name = "statusMainField";
            this.statusMainField.Padding = new System.Windows.Forms.Padding(10);
            this.statusMainField.Size = new System.Drawing.Size(549, 384);
            this.statusMainField.TabIndex = 1;
            this.statusMainField.Visible = false;
            // 
            // driveWaitMainField
            // 
            this.driveWaitMainField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.driveWaitMainField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.driveWaitMainField.Location = new System.Drawing.Point(0, 0);
            this.driveWaitMainField.Margin = new System.Windows.Forms.Padding(0);
            this.driveWaitMainField.Name = "driveWaitMainField";
            this.driveWaitMainField.Padding = new System.Windows.Forms.Padding(10);
            this.driveWaitMainField.Size = new System.Drawing.Size(549, 384);
            this.driveWaitMainField.TabIndex = 8;
            this.driveWaitMainField.Visible = false;
            // 
            // listMainField
            // 
            this.listMainField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listMainField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.listMainField.Location = new System.Drawing.Point(0, 0);
            this.listMainField.Margin = new System.Windows.Forms.Padding(0);
            this.listMainField.Name = "listMainField";
            this.listMainField.Padding = new System.Windows.Forms.Padding(10);
            this.listMainField.Size = new System.Drawing.Size(549, 384);
            this.listMainField.TabIndex = 7;
            this.listMainField.Visible = false;
            // 
            // driveSelectMainField
            // 
            this.driveSelectMainField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.driveSelectMainField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.driveSelectMainField.Location = new System.Drawing.Point(0, 0);
            this.driveSelectMainField.Margin = new System.Windows.Forms.Padding(0);
            this.driveSelectMainField.Name = "driveSelectMainField";
            this.driveSelectMainField.Padding = new System.Windows.Forms.Padding(10);
            this.driveSelectMainField.Size = new System.Drawing.Size(549, 384);
            this.driveSelectMainField.TabIndex = 6;
            this.driveSelectMainField.Visible = false;
            // 
            // numberInputMainField
            // 
            this.numberInputMainField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numberInputMainField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.numberInputMainField.HintText = "";
            this.numberInputMainField.Location = new System.Drawing.Point(0, 0);
            this.numberInputMainField.Margin = new System.Windows.Forms.Padding(0);
            this.numberInputMainField.Name = "numberInputMainField";
            this.numberInputMainField.Padding = new System.Windows.Forms.Padding(10);
            this.numberInputMainField.Size = new System.Drawing.Size(549, 384);
            this.numberInputMainField.TabIndex = 4;
            this.numberInputMainField.Visible = false;
            // 
            // RootForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.ControlBox = false;
            this.Controls.Add(this.messageField);
            this.Controls.Add(this.statusField);
            this.Controls.Add(this.iconBar);
            this.Controls.Add(this.mainFieldPanel);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RootForm";
            this.mainFieldPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private StatusMainField statusMainField;
        private NumberInputMainField numberInputMainField;
        private DriveSelectMainField driveSelectMainField;
        private ListMainField listMainField;
        private DriveWaitMainField driveWaitMainField;
        private BlockDriveMainField blockDriveMainField;
        private MessageListMainField messageListMainField;
        private System.Windows.Forms.Panel mainFieldPanel;
        private IconBar iconBar;
        private StatusField statusField;
        private MessageField messageField;
    }
}