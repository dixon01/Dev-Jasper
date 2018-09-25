namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    partial class BlockDriveMainField
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
            this.components = new System.ComponentModel.Container();
            this.labelStop3 = new System.Windows.Forms.Label();
            this.labelStop2 = new System.Windows.Forms.Label();
            this.labelStop1 = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.textDelay = new System.Windows.Forms.Label();
            this.panelLight = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStop3
            // 
            this.labelStop3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStop3.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.labelStop3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(163)))), ((int)(((byte)(163)))));
            this.labelStop3.Location = new System.Drawing.Point(145, 94);
            this.labelStop3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelStop3.Name = "labelStop3";
            this.labelStop3.Size = new System.Drawing.Size(391, 36);
            this.labelStop3.TabIndex = 10003;
            this.labelStop3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStop2
            // 
            this.labelStop2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStop2.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.labelStop2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(163)))), ((int)(((byte)(163)))));
            this.labelStop2.Location = new System.Drawing.Point(145, 157);
            this.labelStop2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelStop2.Name = "labelStop2";
            this.labelStop2.Size = new System.Drawing.Size(391, 36);
            this.labelStop2.TabIndex = 10003;
            this.labelStop2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStop1
            // 
            this.labelStop1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStop1.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.labelStop1.ForeColor = System.Drawing.Color.White;
            this.labelStop1.Location = new System.Drawing.Point(145, 220);
            this.labelStop1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.labelStop1.Name = "labelStop1";
            this.labelStop1.Size = new System.Drawing.Size(391, 36);
            this.labelStop1.TabIndex = 10003;
            this.labelStop1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerOnTick);
            // 
            // textDelay
            // 
            this.textDelay.BackColor = System.Drawing.Color.Black;
            this.textDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textDelay.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textDelay.ForeColor = System.Drawing.Color.White;
            this.textDelay.Location = new System.Drawing.Point(145, 266);
            this.textDelay.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.textDelay.Name = "textDelay";
            this.textDelay.Size = new System.Drawing.Size(394, 50);
            this.textDelay.TabIndex = 10004;
            this.textDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelLight
            // 
            this.panelLight.BackColor = System.Drawing.Color.Black;
            this.panelLight.Location = new System.Drawing.Point(460, 270);
            this.panelLight.Name = "panelLight";
            this.panelLight.Size = new System.Drawing.Size(75, 42);
            this.panelLight.TabIndex = 10005;
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox.Location = new System.Drawing.Point(10, 20);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(132, 320);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 10002;
            this.pictureBox.TabStop = false;
            // 
            // BlockDriveMainField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelLight);
            this.Controls.Add(this.textDelay);
            this.Controls.Add(this.labelStop1);
            this.Controls.Add(this.labelStop2);
            this.Controls.Add(this.labelStop3);
            this.Controls.Add(this.pictureBox);
            this.Name = "BlockDriveMainField";
            this.Controls.SetChildIndex(this.buttonMenu, 0);
            this.Controls.SetChildIndex(this.pictureBox, 0);
            this.Controls.SetChildIndex(this.labelStop3, 0);
            this.Controls.SetChildIndex(this.labelStop2, 0);
            this.Controls.SetChildIndex(this.labelStop1, 0);
            this.Controls.SetChildIndex(this.textDelay, 0);
            this.Controls.SetChildIndex(this.panelLight, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelStop3;
        private System.Windows.Forms.Label labelStop2;
        private System.Windows.Forms.Label labelStop1;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label textDelay;
        private System.Windows.Forms.Panel panelLight;

    }
}
