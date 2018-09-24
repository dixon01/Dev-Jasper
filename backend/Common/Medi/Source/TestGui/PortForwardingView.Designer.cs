namespace Gorba.Common.Medi.TestGui
{
    partial class PortForwardingView
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownServerPort = new System.Windows.Forms.NumericUpDown();
            this.buttonBrowseServer = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxServerPeer = new System.Windows.Forms.TextBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownClientPort = new System.Windows.Forms.NumericUpDown();
            this.textBoxClientAddress = new System.Windows.Forms.TextBox();
            this.buttonBrowseClient = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxClientPeer = new System.Windows.Forms.TextBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownServerPort)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClientPort)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.buttonAdd);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(573, 156);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add TCP Port Forwarding";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.numericUpDownServerPort);
            this.groupBox3.Controls.Add(this.buttonBrowseServer);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.textBoxServerPeer);
            this.groupBox3.Location = new System.Drawing.Point(6, 73);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(561, 48);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Server";
            // 
            // numericUpDownServerPort
            // 
            this.numericUpDownServerPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownServerPort.Location = new System.Drawing.Point(497, 19);
            this.numericUpDownServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownServerPort.Name = "numericUpDownServerPort";
            this.numericUpDownServerPort.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownServerPort.TabIndex = 6;
            this.numericUpDownServerPort.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // buttonBrowseServer
            // 
            this.buttonBrowseServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseServer.Location = new System.Drawing.Point(241, 17);
            this.buttonBrowseServer.Name = "buttonBrowseServer";
            this.buttonBrowseServer.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseServer.TabIndex = 4;
            this.buttonBrowseServer.Text = "Browse...";
            this.buttonBrowseServer.UseVisualStyleBackColor = true;
            this.buttonBrowseServer.Click += new System.EventHandler(this.ButtonBrowseServerClick);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(462, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Port:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Peer:";
            // 
            // textBoxServerPeer
            // 
            this.textBoxServerPeer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxServerPeer.Location = new System.Drawing.Point(44, 19);
            this.textBoxServerPeer.Name = "textBoxServerPeer";
            this.textBoxServerPeer.ReadOnly = true;
            this.textBoxServerPeer.Size = new System.Drawing.Size(191, 20);
            this.textBoxServerPeer.TabIndex = 2;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.Enabled = false;
            this.buttonAdd.Location = new System.Drawing.Point(492, 127);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 4;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAddClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.numericUpDownClientPort);
            this.groupBox2.Controls.Add(this.textBoxClientAddress);
            this.groupBox2.Controls.Add(this.buttonBrowseClient);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxClientPeer);
            this.groupBox2.Location = new System.Drawing.Point(6, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(561, 48);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Client";
            // 
            // numericUpDownClientPort
            // 
            this.numericUpDownClientPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownClientPort.Location = new System.Drawing.Point(497, 19);
            this.numericUpDownClientPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownClientPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownClientPort.Name = "numericUpDownClientPort";
            this.numericUpDownClientPort.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownClientPort.TabIndex = 6;
            this.numericUpDownClientPort.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // textBoxClientAddress
            // 
            this.textBoxClientAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxClientAddress.Location = new System.Drawing.Point(376, 19);
            this.textBoxClientAddress.Name = "textBoxClientAddress";
            this.textBoxClientAddress.Size = new System.Drawing.Size(80, 20);
            this.textBoxClientAddress.TabIndex = 5;
            this.textBoxClientAddress.Text = "127.0.0.1";
            // 
            // buttonBrowseClient
            // 
            this.buttonBrowseClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseClient.Location = new System.Drawing.Point(241, 17);
            this.buttonBrowseClient.Name = "buttonBrowseClient";
            this.buttonBrowseClient.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseClient.TabIndex = 4;
            this.buttonBrowseClient.Text = "Browse...";
            this.buttonBrowseClient.UseVisualStyleBackColor = true;
            this.buttonBrowseClient.Click += new System.EventHandler(this.ButtonBrowseClientClick);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(462, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Port:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(322, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Address:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Peer:";
            // 
            // textBoxClientPeer
            // 
            this.textBoxClientPeer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxClientPeer.Location = new System.Drawing.Point(44, 19);
            this.textBoxClientPeer.Name = "textBoxClientPeer";
            this.textBoxClientPeer.ReadOnly = true;
            this.textBoxClientPeer.Size = new System.Drawing.Size(191, 20);
            this.textBoxClientPeer.TabIndex = 2;
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.IntegralHeight = false;
            this.listBox.Location = new System.Drawing.Point(3, 165);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(573, 184);
            this.listBox.TabIndex = 1;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.ListBoxSelectedIndexChanged);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(501, 355);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 4;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.ButtonRemoveClick);
            // 
            // PortForwardingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.groupBox1);
            this.Name = "PortForwardingView";
            this.Size = new System.Drawing.Size(579, 381);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownServerPort)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClientPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numericUpDownServerPort;
        private System.Windows.Forms.Button buttonBrowseServer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxServerPeer;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericUpDownClientPort;
        private System.Windows.Forms.TextBox textBoxClientAddress;
        private System.Windows.Forms.Button buttonBrowseClient;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxClientPeer;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button buttonRemove;
    }
}
