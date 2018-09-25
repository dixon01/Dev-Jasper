namespace MyHyperTerminal
{
    partial class IbisUi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IbisUi));
            this.telegramCreationControl1 = new Gorba.Motion.Protran.Controls.TelegramCreationControl();
            this.SuspendLayout();
            // 
            // telegramCreationControl1
            // 
            this.telegramCreationControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.telegramCreationControl1.Location = new System.Drawing.Point(0, 0);
            this.telegramCreationControl1.Name = "telegramCreationControl1";
            this.telegramCreationControl1.Size = new System.Drawing.Size(589, 506);
            this.telegramCreationControl1.TabIndex = 0;
            this.telegramCreationControl1.IbisTelegramCreated += new System.EventHandler<Gorba.Motion.Protran.Controls.DataEventArgs>(this.TelegramCreationControl1IbisTelegramCreated);
            // 
            // IbisUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(589, 506);
            this.Controls.Add(this.telegramCreationControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IbisUi";
            this.Text = "IBIS telegrams";
            this.ResumeLayout(false);

        }

        #endregion

        private Gorba.Motion.Protran.Controls.TelegramCreationControl telegramCreationControl1;

    }
}