namespace Server.GUIS
{
    partial class GenViewEditorUI
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
            this.components = new System.ComponentModel.Container();
            this.numericUpDown_tableNumber = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_rowNumber = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_columnNumber = new System.Windows.Forms.NumericUpDown();
            this.radioButton_addrTypeDirect = new System.Windows.Forms.RadioButton();
            this.radioButton_valueTypeText = new System.Windows.Forms.RadioButton();
            this.radioButton_valueTypeMedia = new System.Windows.Forms.RadioButton();
            this.textBox_value = new System.Windows.Forms.TextBox();
            this.label_tableNumber = new System.Windows.Forms.Label();
            this.label_rowNumber = new System.Windows.Forms.Label();
            this.label_columnNumber = new System.Windows.Forms.Label();
            this.groupBox_valueType = new System.Windows.Forms.GroupBox();
            this.radioButton_addrTypeIndirect = new System.Windows.Forms.RadioButton();
            this.groupBox_addrType = new System.Windows.Forms.GroupBox();
            this.groupBox_coordinates = new System.Windows.Forms.GroupBox();
            this.groupBox_auto = new System.Windows.Forms.GroupBox();
            this.button_startStopAuto = new System.Windows.Forms.Button();
            this.label_period = new System.Windows.Forms.Label();
            this.numericUpDown_period = new System.Windows.Forms.NumericUpDown();
            this.checkBox_enableAuto = new System.Windows.Forms.CheckBox();
            this.groupBox_manual = new System.Windows.Forms.GroupBox();
            this.button_send = new System.Windows.Forms.Button();
            this.groupBox_value = new System.Windows.Forms.GroupBox();
            this.checkBox_randomValues = new System.Windows.Forms.CheckBox();
            this.textBox_ximpleArea = new System.Windows.Forms.TextBox();
            this.timer_logsFlusher = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_tableNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_rowNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_columnNumber)).BeginInit();
            this.groupBox_valueType.SuspendLayout();
            this.groupBox_addrType.SuspendLayout();
            this.groupBox_coordinates.SuspendLayout();
            this.groupBox_auto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_period)).BeginInit();
            this.groupBox_manual.SuspendLayout();
            this.groupBox_value.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericUpDown_tableNumber
            // 
            this.numericUpDown_tableNumber.Location = new System.Drawing.Point(76, 21);
            this.numericUpDown_tableNumber.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_tableNumber.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numericUpDown_tableNumber.Name = "numericUpDown_tableNumber";
            this.numericUpDown_tableNumber.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown_tableNumber.TabIndex = 0;
            this.numericUpDown_tableNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // numericUpDown_rowNumber
            // 
            this.numericUpDown_rowNumber.Location = new System.Drawing.Point(76, 47);
            this.numericUpDown_rowNumber.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_rowNumber.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numericUpDown_rowNumber.Name = "numericUpDown_rowNumber";
            this.numericUpDown_rowNumber.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown_rowNumber.TabIndex = 1;
            this.numericUpDown_rowNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // numericUpDown_columnNumber
            // 
            this.numericUpDown_columnNumber.Location = new System.Drawing.Point(76, 73);
            this.numericUpDown_columnNumber.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_columnNumber.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numericUpDown_columnNumber.Name = "numericUpDown_columnNumber";
            this.numericUpDown_columnNumber.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown_columnNumber.TabIndex = 2;
            this.numericUpDown_columnNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // radioButton_addrTypeDirect
            // 
            this.radioButton_addrTypeDirect.AutoSize = true;
            this.radioButton_addrTypeDirect.Checked = true;
            this.radioButton_addrTypeDirect.Location = new System.Drawing.Point(6, 19);
            this.radioButton_addrTypeDirect.Name = "radioButton_addrTypeDirect";
            this.radioButton_addrTypeDirect.Size = new System.Drawing.Size(53, 17);
            this.radioButton_addrTypeDirect.TabIndex = 3;
            this.radioButton_addrTypeDirect.TabStop = true;
            this.radioButton_addrTypeDirect.Text = "Direct";
            this.radioButton_addrTypeDirect.UseVisualStyleBackColor = true;
            // 
            // radioButton_valueTypeText
            // 
            this.radioButton_valueTypeText.AutoSize = true;
            this.radioButton_valueTypeText.Checked = true;
            this.radioButton_valueTypeText.Location = new System.Drawing.Point(6, 19);
            this.radioButton_valueTypeText.Name = "radioButton_valueTypeText";
            this.radioButton_valueTypeText.Size = new System.Drawing.Size(46, 17);
            this.radioButton_valueTypeText.TabIndex = 4;
            this.radioButton_valueTypeText.TabStop = true;
            this.radioButton_valueTypeText.Text = "Text";
            this.radioButton_valueTypeText.UseVisualStyleBackColor = true;
            // 
            // radioButton_valueTypeMedia
            // 
            this.radioButton_valueTypeMedia.AutoSize = true;
            this.radioButton_valueTypeMedia.Location = new System.Drawing.Point(100, 19);
            this.radioButton_valueTypeMedia.Name = "radioButton_valueTypeMedia";
            this.radioButton_valueTypeMedia.Size = new System.Drawing.Size(54, 17);
            this.radioButton_valueTypeMedia.TabIndex = 5;
            this.radioButton_valueTypeMedia.Text = "Media";
            this.radioButton_valueTypeMedia.UseVisualStyleBackColor = true;
            // 
            // textBox_value
            // 
            this.textBox_value.Location = new System.Drawing.Point(6, 19);
            this.textBox_value.Multiline = true;
            this.textBox_value.Name = "textBox_value";
            this.textBox_value.Size = new System.Drawing.Size(164, 23);
            this.textBox_value.TabIndex = 7;
            // 
            // label_tableNumber
            // 
            this.label_tableNumber.AutoSize = true;
            this.label_tableNumber.Location = new System.Drawing.Point(9, 23);
            this.label_tableNumber.Name = "label_tableNumber";
            this.label_tableNumber.Size = new System.Drawing.Size(34, 13);
            this.label_tableNumber.TabIndex = 8;
            this.label_tableNumber.Text = "Table";
            // 
            // label_rowNumber
            // 
            this.label_rowNumber.AutoSize = true;
            this.label_rowNumber.Location = new System.Drawing.Point(9, 50);
            this.label_rowNumber.Name = "label_rowNumber";
            this.label_rowNumber.Size = new System.Drawing.Size(29, 13);
            this.label_rowNumber.TabIndex = 9;
            this.label_rowNumber.Text = "Row";
            // 
            // label_columnNumber
            // 
            this.label_columnNumber.AutoSize = true;
            this.label_columnNumber.Location = new System.Drawing.Point(9, 76);
            this.label_columnNumber.Name = "label_columnNumber";
            this.label_columnNumber.Size = new System.Drawing.Size(42, 13);
            this.label_columnNumber.TabIndex = 10;
            this.label_columnNumber.Text = "Column";
            // 
            // groupBox_valueType
            // 
            this.groupBox_valueType.Controls.Add(this.radioButton_valueTypeText);
            this.groupBox_valueType.Controls.Add(this.radioButton_valueTypeMedia);
            this.groupBox_valueType.Location = new System.Drawing.Point(3, 157);
            this.groupBox_valueType.Name = "groupBox_valueType";
            this.groupBox_valueType.Size = new System.Drawing.Size(174, 48);
            this.groupBox_valueType.TabIndex = 11;
            this.groupBox_valueType.TabStop = false;
            this.groupBox_valueType.Text = "Value Type:";
            // 
            // radioButton_addrTypeIndirect
            // 
            this.radioButton_addrTypeIndirect.AutoSize = true;
            this.radioButton_addrTypeIndirect.Location = new System.Drawing.Point(100, 16);
            this.radioButton_addrTypeIndirect.Name = "radioButton_addrTypeIndirect";
            this.radioButton_addrTypeIndirect.Size = new System.Drawing.Size(60, 17);
            this.radioButton_addrTypeIndirect.TabIndex = 12;
            this.radioButton_addrTypeIndirect.Text = "Indirect";
            this.radioButton_addrTypeIndirect.UseVisualStyleBackColor = true;
            // 
            // groupBox_addrType
            // 
            this.groupBox_addrType.Controls.Add(this.radioButton_addrTypeIndirect);
            this.groupBox_addrType.Controls.Add(this.radioButton_addrTypeDirect);
            this.groupBox_addrType.Location = new System.Drawing.Point(3, 108);
            this.groupBox_addrType.Name = "groupBox_addrType";
            this.groupBox_addrType.Size = new System.Drawing.Size(174, 43);
            this.groupBox_addrType.TabIndex = 13;
            this.groupBox_addrType.TabStop = false;
            this.groupBox_addrType.Text = "Address Type:";
            // 
            // groupBox_coordinates
            // 
            this.groupBox_coordinates.Controls.Add(this.numericUpDown_rowNumber);
            this.groupBox_coordinates.Controls.Add(this.numericUpDown_tableNumber);
            this.groupBox_coordinates.Controls.Add(this.numericUpDown_columnNumber);
            this.groupBox_coordinates.Controls.Add(this.label_columnNumber);
            this.groupBox_coordinates.Controls.Add(this.label_tableNumber);
            this.groupBox_coordinates.Controls.Add(this.label_rowNumber);
            this.groupBox_coordinates.Location = new System.Drawing.Point(3, 2);
            this.groupBox_coordinates.Name = "groupBox_coordinates";
            this.groupBox_coordinates.Size = new System.Drawing.Size(174, 100);
            this.groupBox_coordinates.TabIndex = 14;
            this.groupBox_coordinates.TabStop = false;
            this.groupBox_coordinates.Text = "Coordinates:";
            // 
            // groupBox_auto
            // 
            this.groupBox_auto.Controls.Add(this.checkBox_randomValues);
            this.groupBox_auto.Controls.Add(this.button_startStopAuto);
            this.groupBox_auto.Controls.Add(this.label_period);
            this.groupBox_auto.Controls.Add(this.numericUpDown_period);
            this.groupBox_auto.Controls.Add(this.checkBox_enableAuto);
            this.groupBox_auto.Location = new System.Drawing.Point(183, 2);
            this.groupBox_auto.Name = "groupBox_auto";
            this.groupBox_auto.Size = new System.Drawing.Size(164, 149);
            this.groupBox_auto.TabIndex = 16;
            this.groupBox_auto.TabStop = false;
            this.groupBox_auto.Text = "Auto:";
            // 
            // button_startStopAuto
            // 
            this.button_startStopAuto.Location = new System.Drawing.Point(54, 119);
            this.button_startStopAuto.Name = "button_startStopAuto";
            this.button_startStopAuto.Size = new System.Drawing.Size(55, 23);
            this.button_startStopAuto.TabIndex = 3;
            this.button_startStopAuto.Text = "Start";
            this.button_startStopAuto.UseVisualStyleBackColor = true;
            this.button_startStopAuto.Click += new System.EventHandler(this.button_startStopAuto_Click);
            // 
            // label_period
            // 
            this.label_period.AutoSize = true;
            this.label_period.Location = new System.Drawing.Point(4, 46);
            this.label_period.Name = "label_period";
            this.label_period.Size = new System.Drawing.Size(62, 13);
            this.label_period.TabIndex = 2;
            this.label_period.Text = "Period (ms):";
            // 
            // numericUpDown_period
            // 
            this.numericUpDown_period.Location = new System.Drawing.Point(74, 44);
            this.numericUpDown_period.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_period.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_period.Name = "numericUpDown_period";
            this.numericUpDown_period.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown_period.TabIndex = 1;
            this.numericUpDown_period.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_period.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // checkBox_enableAuto
            // 
            this.checkBox_enableAuto.AutoSize = true;
            this.checkBox_enableAuto.Location = new System.Drawing.Point(7, 18);
            this.checkBox_enableAuto.Name = "checkBox_enableAuto";
            this.checkBox_enableAuto.Size = new System.Drawing.Size(65, 17);
            this.checkBox_enableAuto.TabIndex = 0;
            this.checkBox_enableAuto.Text = "Enabled";
            this.checkBox_enableAuto.UseVisualStyleBackColor = true;
            this.checkBox_enableAuto.CheckedChanged += new System.EventHandler(this.checkBox_enableAuto_CheckedChanged);
            // 
            // groupBox_manual
            // 
            this.groupBox_manual.Controls.Add(this.button_send);
            this.groupBox_manual.Location = new System.Drawing.Point(183, 157);
            this.groupBox_manual.Name = "groupBox_manual";
            this.groupBox_manual.Size = new System.Drawing.Size(164, 103);
            this.groupBox_manual.TabIndex = 17;
            this.groupBox_manual.TabStop = false;
            this.groupBox_manual.Text = "Manual:";
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(60, 45);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(45, 23);
            this.button_send.TabIndex = 0;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // groupBox_value
            // 
            this.groupBox_value.Controls.Add(this.textBox_value);
            this.groupBox_value.Location = new System.Drawing.Point(3, 211);
            this.groupBox_value.Name = "groupBox_value";
            this.groupBox_value.Size = new System.Drawing.Size(174, 49);
            this.groupBox_value.TabIndex = 18;
            this.groupBox_value.TabStop = false;
            this.groupBox_value.Text = "Value:";
            // 
            // checkBox_randomValues
            // 
            this.checkBox_randomValues.AutoSize = true;
            this.checkBox_randomValues.Location = new System.Drawing.Point(7, 76);
            this.checkBox_randomValues.Name = "checkBox_randomValues";
            this.checkBox_randomValues.Size = new System.Drawing.Size(100, 17);
            this.checkBox_randomValues.TabIndex = 4;
            this.checkBox_randomValues.Text = "Random values";
            this.checkBox_randomValues.UseVisualStyleBackColor = true;
            // 
            // textBox_ximpleArea
            // 
            this.textBox_ximpleArea.BackColor = System.Drawing.Color.White;
            this.textBox_ximpleArea.Location = new System.Drawing.Point(353, 8);
            this.textBox_ximpleArea.Multiline = true;
            this.textBox_ximpleArea.Name = "textBox_ximpleArea";
            this.textBox_ximpleArea.ReadOnly = true;
            this.textBox_ximpleArea.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_ximpleArea.Size = new System.Drawing.Size(337, 252);
            this.textBox_ximpleArea.TabIndex = 19;
            // 
            // timer_logsFlusher
            // 
            this.timer_logsFlusher.Interval = 500;
            this.timer_logsFlusher.Tick += new System.EventHandler(this.timer_logsFlusher_Tick);
            // 
            // GenViewEditorUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 266);
            this.Controls.Add(this.textBox_ximpleArea);
            this.Controls.Add(this.groupBox_value);
            this.Controls.Add(this.groupBox_manual);
            this.Controls.Add(this.groupBox_auto);
            this.Controls.Add(this.groupBox_coordinates);
            this.Controls.Add(this.groupBox_addrType);
            this.Controls.Add(this.groupBox_valueType);
            this.Name = "GenViewEditorUI";
            this.Text = "Generic View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GenViewEditorUI_FormClosing);
            this.Load += new System.EventHandler(this.GenViewEditorUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_tableNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_rowNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_columnNumber)).EndInit();
            this.groupBox_valueType.ResumeLayout(false);
            this.groupBox_valueType.PerformLayout();
            this.groupBox_addrType.ResumeLayout(false);
            this.groupBox_addrType.PerformLayout();
            this.groupBox_coordinates.ResumeLayout(false);
            this.groupBox_coordinates.PerformLayout();
            this.groupBox_auto.ResumeLayout(false);
            this.groupBox_auto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_period)).EndInit();
            this.groupBox_manual.ResumeLayout(false);
            this.groupBox_value.ResumeLayout(false);
            this.groupBox_value.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown_tableNumber;
        private System.Windows.Forms.NumericUpDown numericUpDown_rowNumber;
        private System.Windows.Forms.NumericUpDown numericUpDown_columnNumber;
        private System.Windows.Forms.RadioButton radioButton_addrTypeDirect;
        private System.Windows.Forms.RadioButton radioButton_valueTypeText;
        private System.Windows.Forms.RadioButton radioButton_valueTypeMedia;
        private System.Windows.Forms.TextBox textBox_value;
        private System.Windows.Forms.Label label_tableNumber;
        private System.Windows.Forms.Label label_rowNumber;
        private System.Windows.Forms.Label label_columnNumber;
        private System.Windows.Forms.GroupBox groupBox_valueType;
        private System.Windows.Forms.RadioButton radioButton_addrTypeIndirect;
        private System.Windows.Forms.GroupBox groupBox_addrType;
        private System.Windows.Forms.GroupBox groupBox_coordinates;
        private System.Windows.Forms.GroupBox groupBox_auto;
        private System.Windows.Forms.Button button_startStopAuto;
        private System.Windows.Forms.Label label_period;
        private System.Windows.Forms.NumericUpDown numericUpDown_period;
        private System.Windows.Forms.CheckBox checkBox_enableAuto;
        private System.Windows.Forms.GroupBox groupBox_manual;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.GroupBox groupBox_value;
        private System.Windows.Forms.CheckBox checkBox_randomValues;
        private System.Windows.Forms.TextBox textBox_ximpleArea;
        private System.Windows.Forms.Timer timer_logsFlusher;
    }
}