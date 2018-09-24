namespace Gorba.Common.Medi.TestGui
{
    partial class NetworkList
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
            this.lbxAddresses = new System.Windows.Forms.ListBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.nudTimeout = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // lbxAddresses
            // 
            this.lbxAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxAddresses.FormattingEnabled = true;
            this.lbxAddresses.IntegralHeight = false;
            this.lbxAddresses.Location = new System.Drawing.Point(3, 32);
            this.lbxAddresses.Name = "lbxAddresses";
            this.lbxAddresses.Size = new System.Drawing.Size(297, 256);
            this.lbxAddresses.TabIndex = 0;
            this.lbxAddresses.SelectedIndexChanged += new System.EventHandler(this.LbxAddressesSelectedIndexChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(247, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(53, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefreshClick);
            // 
            // nudTimeout
            // 
            this.nudTimeout.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudTimeout.Location = new System.Drawing.Point(76, 5);
            this.nudTimeout.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.nudTimeout.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudTimeout.Name = "nudTimeout";
            this.nudTimeout.Size = new System.Drawing.Size(76, 20);
            this.nudTimeout.TabIndex = 2;
            this.nudTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Timeout [ms]";
            // 
            // NetworkList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudTimeout);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lbxAddresses);
            this.Name = "NetworkList";
            this.Size = new System.Drawing.Size(303, 291);
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxAddresses;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.NumericUpDown nudTimeout;
        private System.Windows.Forms.Label label1;
    }
}
