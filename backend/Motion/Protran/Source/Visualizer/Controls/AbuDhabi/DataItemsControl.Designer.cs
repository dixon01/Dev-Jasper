namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    partial class DataItemsControl
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageIsiGet = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkBoxAll = new System.Windows.Forms.CheckBox();
            this.buttonRequest = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPageIsiGet.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageIsiGet);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(645, 432);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageIsiGet
            // 
            this.tabPageIsiGet.Controls.Add(this.splitContainer1);
            this.tabPageIsiGet.Location = new System.Drawing.Point(4, 22);
            this.tabPageIsiGet.Name = "tabPageIsiGet";
            this.tabPageIsiGet.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIsiGet.Size = new System.Drawing.Size(637, 406);
            this.tabPageIsiGet.TabIndex = 0;
            this.tabPageIsiGet.Text = "<IsiGet>";
            this.tabPageIsiGet.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxAll);
            this.splitContainer1.Panel1.Controls.Add(this.buttonRequest);
            this.splitContainer1.Panel1.Controls.Add(this.checkedListBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(631, 400);
            this.splitContainer1.SplitterDistance = 203;
            this.splitContainer1.TabIndex = 1;
            // 
            // checkBoxAll
            // 
            this.checkBoxAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAll.AutoSize = true;
            this.checkBoxAll.Location = new System.Drawing.Point(3, 381);
            this.checkBoxAll.Name = "checkBoxAll";
            this.checkBoxAll.Size = new System.Drawing.Size(69, 17);
            this.checkBoxAll.TabIndex = 1;
            this.checkBoxAll.Text = "Select all";
            this.checkBoxAll.UseVisualStyleBackColor = true;
            this.checkBoxAll.CheckStateChanged += new System.EventHandler(this.CheckBoxAllCheckStateChanged);
            // 
            // buttonRequest
            // 
            this.buttonRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRequest.Enabled = false;
            this.buttonRequest.Location = new System.Drawing.Point(128, 377);
            this.buttonRequest.Name = "buttonRequest";
            this.buttonRequest.Size = new System.Drawing.Size(75, 23);
            this.buttonRequest.TabIndex = 0;
            this.buttonRequest.Text = "Request";
            this.buttonRequest.UseVisualStyleBackColor = true;
            this.buttonRequest.Click += new System.EventHandler(this.ButtonRequestClick);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.IntegralHeight = false;
            this.checkedListBox1.Items.AddRange(new object[] {
            "AppName",
            "CurrentSoftwareVersion",
            "CurrentStop",
            "CurrentStopArabic",
            "Destination",
            "DestinationArabic",
            "DeviceState",
            "LineNo",
            "LineNameForDisplay",
            "StopDepartureCountdown",
            "NumberOfCameras",
            "Stop-1",
            "Stop2",
            "Stop3",
            "Stop4",
            "Stop5",
            "Stop-1Arabic",
            "Stop2Arabic",
            "Stop3Arabic",
            "Stop4Arabic",
            "Stop5Arabic",
            "IntDispFreeTextInfo",
            "Time_ISO8601",
            "VideoPictureRate",
            "TicketingCicosRaid1",
            "TicketingCicosRaid2",
            "TicketingCicosRaid3",
            "TicketingCicosRaid4",
            "TicketingCicosRaid5",
            "CurrentDirectionNo",
            "CurrentStopNumber",
            "StopPosition",
            "VehicleNo",
            "CourseNo",
            "IgnitionState",
            "GPGGA",
            "GPRMC",
            "BlockNo",
            "DoorState",
            "CurrentStopConnectionInfo",
            "Stop2ConnectionInfo",
            "Stop3ConnectionInfo",
            "Stop4ConnectionInfo",
            "Stop5ConnectionInfo",
            "IsiClientRunsFtpTransfers",
            "TickerTextArabic",
            "CurrentStopConnectionInfoArabic",
            "Stop2ConnectionInfoArabic",
            "Stop3ConnectionInfoArabic",
            "Stop4ConnectionInfoArabic",
            "Stop5ConnectionInfoArabic",
            "LastStop",
            "LastStopArabic",
            "PositionNearStop",
            "DestinationNo",
            "DriverId",
            "PatternNo",
            "DutyNumber",
            "TicketingCicosNumberOfTransactionsOnLastStop",
            "TripMode",
            "SerialNumber",
            "CurrentParameterVersion",
            "IsVehicle100mBeforeStopOrAtStop",
            "GorbaSystemFallbackActive",
            "BlockDayType",
            "CurrentStopPointPositionNumber",
            "VdvBaseVersion",
            "CurrentTripNo",
            "TicketRejection"});
            this.checkedListBox1.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox1.Margin = new System.Windows.Forms.Padding(0);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(203, 374);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox1ItemCheck);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(424, 400);
            this.dataGridView1.TabIndex = 0;
            // 
            // DataItemsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "DataItemsControl";
            this.Size = new System.Drawing.Size(645, 432);
            this.tabControl1.ResumeLayout(false);
            this.tabPageIsiGet.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageIsiGet;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonRequest;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox checkBoxAll;
    }
}
