namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    partial class Vdv301InputControl
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
            this.tabPageCustomerInformationService = new System.Windows.Forms.TabPage();
            this.customerInformationServiceInputControl = new Gorba.Motion.Protran.Visualizer.Controls.Vdv301.CustomerInformationServiceInputControl();
            this.tabControl.SuspendLayout();
            this.tabPageCustomerInformationService.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageCustomerInformationService);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(936, 476);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageCustomerInformationService
            // 
            this.tabPageCustomerInformationService.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageCustomerInformationService.Controls.Add(this.customerInformationServiceInputControl);
            this.tabPageCustomerInformationService.Location = new System.Drawing.Point(4, 22);
            this.tabPageCustomerInformationService.Name = "tabPageCustomerInformationService";
            this.tabPageCustomerInformationService.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCustomerInformationService.Size = new System.Drawing.Size(928, 450);
            this.tabPageCustomerInformationService.TabIndex = 0;
            this.tabPageCustomerInformationService.Text = "CustomerInformationService";
            // 
            // customerInformationServiceInputControl
            // 
            this.customerInformationServiceInputControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customerInformationServiceInputControl.Location = new System.Drawing.Point(3, 3);
            this.customerInformationServiceInputControl.Name = "customerInformationServiceInputControl";
            this.customerInformationServiceInputControl.Size = new System.Drawing.Size(922, 444);
            this.customerInformationServiceInputControl.TabIndex = 0;
            // 
            // Vdv301InputControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "Vdv301InputControl";
            this.Size = new System.Drawing.Size(936, 476);
            this.tabControl.ResumeLayout(false);
            this.tabPageCustomerInformationService.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageCustomerInformationService;
        private CustomerInformationServiceInputControl customerInformationServiceInputControl;
    }
}
