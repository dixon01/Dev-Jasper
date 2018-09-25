namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    partial class UnitUpdateDetailsControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxUpdates = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.updateFolderStructureControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.UpdateFolderStructureControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.preInstallationActionsControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.InstallationActionsControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.postInstallationActionsControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.InstallationActionsControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridViewHistory = new System.Windows.Forms.DataGridView();
            this.textBoxTimestamp = new System.Windows.Forms.TextBox();
            this.textBoxState = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxValidFrom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxUpdates);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxValidFrom);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxTimestamp);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxState);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(670, 479);
            this.splitContainer1.SplitterDistance = 183;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBoxUpdates
            // 
            this.listBoxUpdates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxUpdates.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxUpdates.FormattingEnabled = true;
            this.listBoxUpdates.IntegralHeight = false;
            this.listBoxUpdates.Location = new System.Drawing.Point(0, 0);
            this.listBoxUpdates.Name = "listBoxUpdates";
            this.listBoxUpdates.Size = new System.Drawing.Size(183, 479);
            this.listBoxUpdates.TabIndex = 0;
            this.listBoxUpdates.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBoxUpdatesDrawItem);
            this.listBoxUpdates.SelectedIndexChanged += new System.EventHandler(this.ListBoxUpdatesSelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 78);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(483, 401);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.updateFolderStructureControl);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(475, 375);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Update Details";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // updateFolderStructureControl
            // 
            this.updateFolderStructureControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateFolderStructureControl.Location = new System.Drawing.Point(3, 3);
            this.updateFolderStructureControl.Name = "updateFolderStructureControl";
            this.updateFolderStructureControl.Size = new System.Drawing.Size(469, 369);
            this.updateFolderStructureControl.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.preInstallationActionsControl);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(475, 398);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Pre-Installation Actions";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // preInstallationActionsControl
            // 
            this.preInstallationActionsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preInstallationActionsControl.Location = new System.Drawing.Point(3, 3);
            this.preInstallationActionsControl.Name = "preInstallationActionsControl";
            this.preInstallationActionsControl.ReadOnly = true;
            this.preInstallationActionsControl.Size = new System.Drawing.Size(469, 392);
            this.preInstallationActionsControl.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.postInstallationActionsControl);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(475, 398);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Post-Installation Actions";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // postInstallationActionsControl
            // 
            this.postInstallationActionsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.postInstallationActionsControl.Location = new System.Drawing.Point(3, 3);
            this.postInstallationActionsControl.Name = "postInstallationActionsControl";
            this.postInstallationActionsControl.ReadOnly = true;
            this.postInstallationActionsControl.Size = new System.Drawing.Size(469, 392);
            this.postInstallationActionsControl.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridViewHistory);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(475, 398);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Update History";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridViewHistory
            // 
            this.dataGridViewHistory.AllowUserToAddRows = false;
            this.dataGridViewHistory.AllowUserToDeleteRows = false;
            this.dataGridViewHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewHistory.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewHistory.Name = "dataGridViewHistory";
            this.dataGridViewHistory.ReadOnly = true;
            this.dataGridViewHistory.Size = new System.Drawing.Size(469, 392);
            this.dataGridViewHistory.TabIndex = 0;
            this.dataGridViewHistory.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DataGridViewHistoryCellFormatting);
            // 
            // textBoxTimestamp
            // 
            this.textBoxTimestamp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTimestamp.Location = new System.Drawing.Point(107, 55);
            this.textBoxTimestamp.Name = "textBoxTimestamp";
            this.textBoxTimestamp.ReadOnly = true;
            this.textBoxTimestamp.Size = new System.Drawing.Size(373, 20);
            this.textBoxTimestamp.TabIndex = 3;
            // 
            // textBoxState
            // 
            this.textBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxState.Location = new System.Drawing.Point(107, 29);
            this.textBoxState.Name = "textBoxState";
            this.textBoxState.ReadOnly = true;
            this.textBoxState.Size = new System.Drawing.Size(373, 20);
            this.textBoxState.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Last State Change:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "State:";
            // 
            // textBoxValidFrom
            // 
            this.textBoxValidFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxValidFrom.Location = new System.Drawing.Point(107, 0);
            this.textBoxValidFrom.Name = "textBoxValidFrom";
            this.textBoxValidFrom.ReadOnly = true;
            this.textBoxValidFrom.Size = new System.Drawing.Size(373, 20);
            this.textBoxValidFrom.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Valid From:";
            // 
            // UnitUpdateDetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UnitUpdateDetailsControl";
            this.Size = new System.Drawing.Size(670, 479);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBoxUpdates;
        private System.Windows.Forms.TextBox textBoxState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTimestamp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridViewHistory;
        private UpdateFolderStructureControl updateFolderStructureControl;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private InstallationActionsControl preInstallationActionsControl;
        private InstallationActionsControl postInstallationActionsControl;
        private System.Windows.Forms.TextBox textBoxValidFrom;
        private System.Windows.Forms.Label label3;
    }
}
