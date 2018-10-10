namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    partial class CustomerInformationServiceInputControl
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageExpert = new System.Windows.Forms.TabPage();
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonVerify = new System.Windows.Forms.Button();
            this.xmlEditorControl = new Gorba.Motion.Protran.Visualizer.Controls.Vdv301.XmlEditorControl();
            this.tabControl.SuspendLayout();
            this.tabPageExpert.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl.Controls.Add(this.tabPageExpert);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(799, 421);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageExpert
            // 
            this.tabPageExpert.Controls.Add(this.xmlEditorControl);
            this.tabPageExpert.Location = new System.Drawing.Point(4, 25);
            this.tabPageExpert.Name = "tabPageExpert";
            this.tabPageExpert.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExpert.Size = new System.Drawing.Size(791, 392);
            this.tabPageExpert.TabIndex = 0;
            this.tabPageExpert.Text = "Expert Mode";
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Location = new System.Drawing.Point(721, 427);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.ButtonSendOnClick);
            // 
            // buttonVerify
            // 
            this.buttonVerify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonVerify.Location = new System.Drawing.Point(640, 427);
            this.buttonVerify.Name = "buttonVerify";
            this.buttonVerify.Size = new System.Drawing.Size(75, 23);
            this.buttonVerify.TabIndex = 1;
            this.buttonVerify.Text = "Verify XML";
            this.buttonVerify.UseVisualStyleBackColor = true;
            this.buttonVerify.Click += new System.EventHandler(this.ButtonVerifyOnClick);
            // 
            // xmlEditorControl
            // 
            this.xmlEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlEditorControl.Location = new System.Drawing.Point(3, 3);
            this.xmlEditorControl.Name = "xmlEditorControl";
            this.xmlEditorControl.Size = new System.Drawing.Size(785, 386);
            this.xmlEditorControl.TabIndex = 0;
            // 
            // CustomerInformationServiceInputControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonVerify);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.tabControl);
            this.Name = "CustomerInformationServiceInputControl";
            this.Size = new System.Drawing.Size(799, 453);
            this.tabControl.ResumeLayout(false);
            this.tabPageExpert.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageExpert;
        private XmlEditorControl xmlEditorControl;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonVerify;
    }
}
