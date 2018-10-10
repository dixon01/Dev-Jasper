namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using Gorba.Motion.Obc.Terminal.Gui.Control;

    partial class MsgBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MsgBox));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pnlCaption = new System.Windows.Forms.Panel();
            this.lblCaption = new System.Windows.Forms.Label();
            this.btnOK = new VCustomButton();
            this.pnlMain.SuspendLayout();
            this.pnlCaption.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.pnlMain.Controls.Add(this.btnOK);
            this.pnlMain.Controls.Add(this.lblMessage);
            this.pnlMain.Controls.Add(this.pnlCaption);
            resources.ApplyResources(this.pnlMain, "pnlMain");
            this.pnlMain.Name = "pnlMain";
            // 
            // lblMessage
            // 
            resources.ApplyResources(this.lblMessage, "lblMessage");
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Name = "lblMessage";
            // 
            // pnlCaption
            // 
            this.pnlCaption.BackColor = System.Drawing.Color.Blue;
            this.pnlCaption.Controls.Add(this.lblCaption);
            resources.ApplyResources(this.pnlCaption, "pnlCaption");
            this.pnlCaption.Name = "pnlCaption";
            // 
            // lblCaption
            // 
            this.lblCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(32)))), ((int)(((byte)(134)))));
            resources.ApplyResources(this.lblCaption, "lblCaption");
            this.lblCaption.ForeColor = System.Drawing.Color.White;
            this.lblCaption.Name = "lblCaption";
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnOK.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.btnOK.BorderColorClicked = System.Drawing.Color.Black;
            this.btnOK.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.btnOK.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btnOK.ClickedBgImage")));
            this.btnOK.DisabledBgImage = null;
            this.btnOK.FgImage = null;
            this.btnOK.FocusedBgImage = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.btnOK.IsPushed = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btnOK.NormalBgImage")));
            this.btnOK.TabStop = false;
            this.btnOK.ToggleMode = false;
            this.btnOK.Click += new System.EventHandler(this.BtnOkClick);
            // 
            // VMxMsgBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlMain);
            this.ForeColor = System.Drawing.Color.Black;
            resources.ApplyResources(this, "$this");
            this.Name = "VMxMsgBox";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MsgBoxPaint);
            this.EnabledChanged += new System.EventHandler(this.MsgBoxEnabledChanged);
            this.pnlMain.ResumeLayout(false);
            this.pnlCaption.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlCaption;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label lblMessage;
        private VCustomButton btnOK;
    }
}
