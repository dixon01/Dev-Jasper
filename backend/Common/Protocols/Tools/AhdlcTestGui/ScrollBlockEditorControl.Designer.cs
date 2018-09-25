namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    partial class ScrollBlockEditorControl
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
            this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownStartX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStartY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownEndX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownEndY = new System.Windows.Forms.NumericUpDown();
            this.bitmapCreatorControl = new Gorba.Common.Protocols.Tools.AhdlcTestGui.BitmapCreatorControl();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.comboBoxSpeed = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxEnabled
            // 
            this.checkBoxEnabled.AutoSize = true;
            this.checkBoxEnabled.Location = new System.Drawing.Point(58, 6);
            this.checkBoxEnabled.Name = "checkBoxEnabled";
            this.checkBoxEnabled.Size = new System.Drawing.Size(15, 14);
            this.checkBoxEnabled.TabIndex = 0;
            this.checkBoxEnabled.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enabled:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(77, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Start X:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(182, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Start Y:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(287, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "End X:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(389, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "End Y:";
            // 
            // numericUpDownStartX
            // 
            this.numericUpDownStartX.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownStartX.Location = new System.Drawing.Point(125, 3);
            this.numericUpDownStartX.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownStartX.Name = "numericUpDownStartX";
            this.numericUpDownStartX.Size = new System.Drawing.Size(51, 20);
            this.numericUpDownStartX.TabIndex = 2;
            this.numericUpDownStartX.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // numericUpDownStartY
            // 
            this.numericUpDownStartY.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownStartY.Location = new System.Drawing.Point(230, 3);
            this.numericUpDownStartY.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDownStartY.Name = "numericUpDownStartY";
            this.numericUpDownStartY.Size = new System.Drawing.Size(51, 20);
            this.numericUpDownStartY.TabIndex = 2;
            this.numericUpDownStartY.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // numericUpDownEndX
            // 
            this.numericUpDownEndX.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownEndX.Location = new System.Drawing.Point(332, 3);
            this.numericUpDownEndX.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownEndX.Name = "numericUpDownEndX";
            this.numericUpDownEndX.Size = new System.Drawing.Size(51, 20);
            this.numericUpDownEndX.TabIndex = 2;
            this.numericUpDownEndX.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // numericUpDownEndY
            // 
            this.numericUpDownEndY.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownEndY.Location = new System.Drawing.Point(434, 3);
            this.numericUpDownEndY.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDownEndY.Name = "numericUpDownEndY";
            this.numericUpDownEndY.Size = new System.Drawing.Size(51, 20);
            this.numericUpDownEndY.TabIndex = 2;
            this.numericUpDownEndY.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // bitmapCreatorControl
            // 
            this.bitmapCreatorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bitmapCreatorControl.BitmapSize = new System.Drawing.Size(16, 112);
            this.bitmapCreatorControl.HasColor = false;
            this.bitmapCreatorControl.Location = new System.Drawing.Point(0, 25);
            this.bitmapCreatorControl.Name = "bitmapCreatorControl";
            this.bitmapCreatorControl.Size = new System.Drawing.Size(763, 367);
            this.bitmapCreatorControl.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(491, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Width:";
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownWidth.Location = new System.Drawing.Point(535, 3);
            this.numericUpDownWidth.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(51, 20);
            this.numericUpDownWidth.TabIndex = 2;
            this.numericUpDownWidth.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // labelSpeed
            // 
            this.labelSpeed.AutoSize = true;
            this.labelSpeed.Location = new System.Drawing.Point(592, 6);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(41, 13);
            this.labelSpeed.TabIndex = 1;
            this.labelSpeed.Text = "Speed:";
            // 
            // comboBoxSpeed
            // 
            this.comboBoxSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpeed.FormattingEnabled = true;
            this.comboBoxSpeed.Location = new System.Drawing.Point(639, 2);
            this.comboBoxSpeed.Name = "comboBoxSpeed";
            this.comboBoxSpeed.Size = new System.Drawing.Size(76, 21);
            this.comboBoxSpeed.TabIndex = 4;
            // 
            // ScrollBlockEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBoxSpeed);
            this.Controls.Add(this.bitmapCreatorControl);
            this.Controls.Add(this.numericUpDownWidth);
            this.Controls.Add(this.numericUpDownEndY);
            this.Controls.Add(this.numericUpDownEndX);
            this.Controls.Add(this.numericUpDownStartY);
            this.Controls.Add(this.numericUpDownStartX);
            this.Controls.Add(this.labelSpeed);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxEnabled);
            this.Name = "ScrollBlockEditorControl";
            this.Size = new System.Drawing.Size(763, 392);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxEnabled;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownStartX;
        private System.Windows.Forms.NumericUpDown numericUpDownStartY;
        private System.Windows.Forms.NumericUpDown numericUpDownEndX;
        private System.Windows.Forms.NumericUpDown numericUpDownEndY;
        private BitmapCreatorControl bitmapCreatorControl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.Label labelSpeed;
        private System.Windows.Forms.ComboBox comboBoxSpeed;
    }
}
