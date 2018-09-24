namespace Gorba.Motion.Obc.Terminal.Gui.Form
{
    using Gorba.Motion.Obc.Terminal.Gui.Control;
    using Gorba.Motion.Obc.Terminal.Gui.MainFields;

    public partial class RootForm
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RootForm));
          this.pnlMainField = new System.Windows.Forms.Panel();
          this.ihmiRightButton = new Gorba.Motion.Obc.Terminal.Gui.Control.IhmiRightButton();
          this.vmxMessageField = new Gorba.Motion.Obc.Terminal.Gui.Control.MessageField();
          this.vmxStatusField = new Gorba.Motion.Obc.Terminal.Gui.Control.StatusField();
          this.vmxDigitalClock1 = new Gorba.Motion.Obc.Terminal.Gui.Control.VMxDigitalClock();
          this.vmxIconBar1 = new Gorba.Motion.Obc.Terminal.Gui.MainFields.IconBar();
          this.SuspendLayout();
          // 
          // pnlMainField
          // 
          resources.ApplyResources(this.pnlMainField, "pnlMainField");
          this.pnlMainField.Name = "pnlMainField";
          // 
          // ihmiRightButton
          // 
          resources.ApplyResources(this.ihmiRightButton, "ihmiRightButton");
          this.ihmiRightButton.Name = "ihmiRightButton";
          // 
          // vmxMessageField
          // 
          resources.ApplyResources(this.vmxMessageField, "vmxMessageField");
          this.vmxMessageField.Name = "vmxMessageField";
          // 
          // vmxStatusField
          // 
          resources.ApplyResources(this.vmxStatusField, "vmxStatusField");
          this.vmxStatusField.Name = "vmxStatusField";
          // 
          // vmxDigitalClock1
          // 
          resources.ApplyResources(this.vmxDigitalClock1, "vmxDigitalClock1");
          this.vmxDigitalClock1.Name = "vmxDigitalClock1";
          // 
          // vmxIconBar1
          // 
          this.vmxIconBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
          resources.ApplyResources(this.vmxIconBar1, "vmxIconBar1");
          this.vmxIconBar1.Name = "vmxIconBar1";
          // 
          // RootForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
          this.BackColor = System.Drawing.Color.White;
          resources.ApplyResources(this, "$this");
          this.ControlBox = false;
          this.Controls.Add(this.ihmiRightButton);
          this.Controls.Add(this.vmxMessageField);
          this.Controls.Add(this.pnlMainField);
          this.Controls.Add(this.vmxStatusField);
          this.Controls.Add(this.vmxDigitalClock1);
          this.Controls.Add(this.vmxIconBar1);
          this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "RootForm";
          this.ResumeLayout(false);

        }

        #endregion

        private MessageField vmxMessageField;
        private IconBar vmxIconBar1;
        private StatusField vmxStatusField;

        private System.Windows.Forms.Panel pnlMainField;
        private VMxDigitalClock vmxDigitalClock1;
        private IhmiRightButton ihmiRightButton;

    }
}

