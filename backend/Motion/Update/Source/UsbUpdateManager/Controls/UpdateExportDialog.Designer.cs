namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    partial class UpdateExportDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxUnitGroups = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ValidFromDateTime = new System.Windows.Forms.DateTimePicker();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.preInstallationActionsControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.InstallationActionsControl();
            this.updateFolderStructureControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.UpdateFolderStructureControl();
            this.postInstallationActionsControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.InstallationActionsControl();
            this.checkBoxInstallAfterBoot = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(449, 26);
            this.panel1.TabIndex = 6;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(69, 3);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(377, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.TextChanged += new System.EventHandler(this.TextBoxNameTextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Controls.Add(this.buttonOk);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 438);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(449, 29);
            this.panel2.TabIndex = 7;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(372, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(291, 3);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxUnitGroups);
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Location = new System.Drawing.Point(3, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(450, 377);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Unit Groups";
            // 
            // comboBoxUnitGroups
            // 
            this.comboBoxUnitGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxUnitGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUnitGroups.FormattingEnabled = true;
            this.comboBoxUnitGroups.Location = new System.Drawing.Point(6, 19);
            this.comboBoxUnitGroups.Name = "comboBoxUnitGroups";
            this.comboBoxUnitGroups.Size = new System.Drawing.Size(438, 21);
            this.comboBoxUnitGroups.TabIndex = 5;
            this.comboBoxUnitGroups.SelectedIndexChanged += new System.EventHandler(this.ComboBoxUnitGroupsSelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(6, 46);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(438, 325);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.preInstallationActionsControl);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(430, 299);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Pre-Installation Actions";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.updateFolderStructureControl);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(430, 299);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Update Structure";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.postInstallationActionsControl);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(430, 299);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Post-Installation Actions";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ValidFromDateTime
            // 
            this.ValidFromDateTime.CustomFormat = "dd.MM.yyyy HH.mm.ss";
            this.ValidFromDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.ValidFromDateTime.Location = new System.Drawing.Point(69, 6);
            this.ValidFromDateTime.Name = "ValidFromDateTime";
            this.ValidFromDateTime.Size = new System.Drawing.Size(144, 20);
            this.ValidFromDateTime.TabIndex = 9;
            this.ValidFromDateTime.Value = new System.DateTime(2014, 1, 13, 15, 38, 0, 0);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.checkBoxInstallAfterBoot);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.ValidFromDateTime);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 29);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(449, 30);
            this.panel3.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Valid From:";
            // 
            // preInstallationActionsControl
            // 
            this.preInstallationActionsControl.Location = new System.Drawing.Point(3, 3);
            this.preInstallationActionsControl.Name = "preInstallationActionsControl";
            this.preInstallationActionsControl.ReadOnly = false;
            this.preInstallationActionsControl.Size = new System.Drawing.Size(424, 269);
            this.preInstallationActionsControl.TabIndex = 0;
            // 
            // updateFolderStructureControl
            // 
            this.updateFolderStructureControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateFolderStructureControl.Location = new System.Drawing.Point(3, 3);
            this.updateFolderStructureControl.Name = "updateFolderStructureControl";
            this.updateFolderStructureControl.Size = new System.Drawing.Size(424, 293);
            this.updateFolderStructureControl.TabIndex = 0;
            // 
            // postInstallationActionsControl
            // 
            this.postInstallationActionsControl.Location = new System.Drawing.Point(3, 3);
            this.postInstallationActionsControl.Name = "postInstallationActionsControl";
            this.postInstallationActionsControl.ReadOnly = false;
            this.postInstallationActionsControl.Size = new System.Drawing.Size(424, 293);
            this.postInstallationActionsControl.TabIndex = 0;
            // 
            // checkBoxInstallAfterBoot
            // 
            this.checkBoxInstallAfterBoot.AutoSize = true;
            this.checkBoxInstallAfterBoot.Location = new System.Drawing.Point(220, 9);
            this.checkBoxInstallAfterBoot.Name = "checkBoxInstallAfterBoot";
            this.checkBoxInstallAfterBoot.Size = new System.Drawing.Size(102, 17);
            this.checkBoxInstallAfterBoot.TabIndex = 11;
            this.checkBoxInstallAfterBoot.Text = "Install after Boot";
            this.checkBoxInstallAfterBoot.UseVisualStyleBackColor = true;
            // 
            // UpdateExportDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(455, 470);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateExportDialog";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.ShowIcon = false;
            this.Text = "Export Update";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxUnitGroups;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button buttonCancel;
        private UpdateFolderStructureControl updateFolderStructureControl;
        private InstallationActionsControl preInstallationActionsControl;
        private InstallationActionsControl postInstallationActionsControl;
        private System.Windows.Forms.DateTimePicker ValidFromDateTime;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxInstallAfterBoot;
    }
}