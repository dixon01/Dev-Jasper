namespace lightSensor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.com1 = new System.Windows.Forms.RadioButton();
            this.com2 = new System.Windows.Forms.RadioButton();
            this.com3 = new System.Windows.Forms.RadioButton();
            this.com4 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pollRequest = new System.Windows.Forms.Button();
            this.versionRequest = new System.Windows.Forms.Button();
            this.setSensorScale = new System.Windows.Forms.Button();
            this.setBrightnessRequest = new System.Windows.Forms.Button();
            this.setMonitorPower = new System.Windows.Forms.Button();
            this.setMode = new System.Windows.Forms.Button();
            this.queryRequest = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.response = new System.Windows.Forms.TextBox();
            this.power = new System.Windows.Forms.CheckBox();
            this.normal = new System.Windows.Forms.CheckBox();
            this.scale0 = new System.Windows.Forms.RadioButton();
            this.scale1 = new System.Windows.Forms.RadioButton();
            this.scale2 = new System.Windows.Forms.RadioButton();
            this.scale3 = new System.Windows.Forms.RadioButton();
            this.brightness = new System.Windows.Forms.HScrollBar();
            this.brightnessText = new System.Windows.Forms.Label();
            this.com10 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.com10);
            this.groupBox1.Controls.Add(this.com4);
            this.groupBox1.Controls.Add(this.com3);
            this.groupBox1.Controls.Add(this.com2);
            this.groupBox1.Controls.Add(this.com1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(126, 158);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select a COM port";
            // 
            // com1
            // 
            this.com1.AutoSize = true;
            this.com1.Location = new System.Drawing.Point(15, 30);
            this.com1.Name = "com1";
            this.com1.Size = new System.Drawing.Size(55, 17);
            this.com1.TabIndex = 0;
            this.com1.Text = "COM1";
            this.com1.UseVisualStyleBackColor = true;
            // 
            // com2
            // 
            this.com2.AutoSize = true;
            this.com2.Checked = true;
            this.com2.Location = new System.Drawing.Point(15, 54);
            this.com2.Name = "com2";
            this.com2.Size = new System.Drawing.Size(55, 17);
            this.com2.TabIndex = 1;
            this.com2.TabStop = true;
            this.com2.Text = "COM2";
            this.com2.UseVisualStyleBackColor = true;
            // 
            // com3
            // 
            this.com3.AutoSize = true;
            this.com3.Location = new System.Drawing.Point(15, 78);
            this.com3.Name = "com3";
            this.com3.Size = new System.Drawing.Size(55, 17);
            this.com3.TabIndex = 2;
            this.com3.Text = "COM3";
            this.com3.UseVisualStyleBackColor = true;
            // 
            // com4
            // 
            this.com4.AutoSize = true;
            this.com4.Location = new System.Drawing.Point(15, 102);
            this.com4.Name = "com4";
            this.com4.Size = new System.Drawing.Size(55, 17);
            this.com4.TabIndex = 3;
            this.com4.Text = "COM4";
            this.com4.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.brightnessText);
            this.groupBox2.Controls.Add(this.brightness);
            this.groupBox2.Controls.Add(this.scale3);
            this.groupBox2.Controls.Add(this.scale2);
            this.groupBox2.Controls.Add(this.scale1);
            this.groupBox2.Controls.Add(this.scale0);
            this.groupBox2.Controls.Add(this.normal);
            this.groupBox2.Controls.Add(this.power);
            this.groupBox2.Controls.Add(this.queryRequest);
            this.groupBox2.Controls.Add(this.setMode);
            this.groupBox2.Controls.Add(this.setMonitorPower);
            this.groupBox2.Controls.Add(this.setBrightnessRequest);
            this.groupBox2.Controls.Add(this.setSensorScale);
            this.groupBox2.Controls.Add(this.versionRequest);
            this.groupBox2.Controls.Add(this.pollRequest);
            this.groupBox2.Location = new System.Drawing.Point(173, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(401, 236);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Command to Send";
            // 
            // pollRequest
            // 
            this.pollRequest.Location = new System.Drawing.Point(7, 20);
            this.pollRequest.Name = "pollRequest";
            this.pollRequest.Size = new System.Drawing.Size(171, 23);
            this.pollRequest.TabIndex = 0;
            this.pollRequest.Text = "POLL REQUEST";
            this.pollRequest.UseVisualStyleBackColor = true;
            this.pollRequest.Click += new System.EventHandler(this.pollRequest_Click);
            // 
            // versionRequest
            // 
            this.versionRequest.Location = new System.Drawing.Point(7, 50);
            this.versionRequest.Name = "versionRequest";
            this.versionRequest.Size = new System.Drawing.Size(171, 23);
            this.versionRequest.TabIndex = 1;
            this.versionRequest.Text = "VERSION REQUEST";
            this.versionRequest.UseVisualStyleBackColor = true;
            this.versionRequest.Click += new System.EventHandler(this.versionRequest_Click);
            // 
            // setSensorScale
            // 
            this.setSensorScale.Location = new System.Drawing.Point(7, 80);
            this.setSensorScale.Name = "setSensorScale";
            this.setSensorScale.Size = new System.Drawing.Size(171, 23);
            this.setSensorScale.TabIndex = 2;
            this.setSensorScale.Text = "SET SENSOR SCALE";
            this.setSensorScale.UseVisualStyleBackColor = true;
            this.setSensorScale.Click += new System.EventHandler(this.setSensorScale_Click);
            // 
            // setBrightnessRequest
            // 
            this.setBrightnessRequest.Location = new System.Drawing.Point(7, 110);
            this.setBrightnessRequest.Name = "setBrightnessRequest";
            this.setBrightnessRequest.Size = new System.Drawing.Size(171, 23);
            this.setBrightnessRequest.TabIndex = 3;
            this.setBrightnessRequest.Text = "SET BRIGHTNESS REQUEST";
            this.setBrightnessRequest.UseVisualStyleBackColor = true;
            this.setBrightnessRequest.Click += new System.EventHandler(this.setBrightnessRequest_Click);
            // 
            // setMonitorPower
            // 
            this.setMonitorPower.Location = new System.Drawing.Point(7, 140);
            this.setMonitorPower.Name = "setMonitorPower";
            this.setMonitorPower.Size = new System.Drawing.Size(171, 23);
            this.setMonitorPower.TabIndex = 4;
            this.setMonitorPower.Text = "SET MONITOR POWER";
            this.setMonitorPower.UseVisualStyleBackColor = true;
            this.setMonitorPower.Click += new System.EventHandler(this.setMonitorPower_Click);
            // 
            // setMode
            // 
            this.setMode.Location = new System.Drawing.Point(7, 170);
            this.setMode.Name = "setMode";
            this.setMode.Size = new System.Drawing.Size(171, 23);
            this.setMode.TabIndex = 5;
            this.setMode.Text = "SET MODE";
            this.setMode.UseVisualStyleBackColor = true;
            this.setMode.Click += new System.EventHandler(this.setMode_Click);
            // 
            // queryRequest
            // 
            this.queryRequest.Location = new System.Drawing.Point(7, 200);
            this.queryRequest.Name = "queryRequest";
            this.queryRequest.Size = new System.Drawing.Size(171, 23);
            this.queryRequest.TabIndex = 6;
            this.queryRequest.Text = "QUERY REQUEST";
            this.queryRequest.UseVisualStyleBackColor = true;
            this.queryRequest.Click += new System.EventHandler(this.queryRequest_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.response);
            this.groupBox3.Location = new System.Drawing.Point(13, 270);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(561, 182);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Response";
            // 
            // response
            // 
            this.response.Location = new System.Drawing.Point(7, 20);
            this.response.Multiline = true;
            this.response.Name = "response";
            this.response.Size = new System.Drawing.Size(548, 146);
            this.response.TabIndex = 0;
            // 
            // power
            // 
            this.power.AutoSize = true;
            this.power.Location = new System.Drawing.Point(196, 145);
            this.power.Name = "power";
            this.power.Size = new System.Drawing.Size(56, 17);
            this.power.TabIndex = 7;
            this.power.Text = "Power";
            this.power.UseVisualStyleBackColor = true;
            // 
            // normal
            // 
            this.normal.AutoSize = true;
            this.normal.Location = new System.Drawing.Point(196, 175);
            this.normal.Name = "normal";
            this.normal.Size = new System.Drawing.Size(59, 17);
            this.normal.TabIndex = 8;
            this.normal.Text = "Normal";
            this.normal.UseVisualStyleBackColor = true;
            // 
            // scale0
            // 
            this.scale0.AutoSize = true;
            this.scale0.Checked = true;
            this.scale0.Location = new System.Drawing.Point(196, 68);
            this.scale0.Name = "scale0";
            this.scale0.Size = new System.Drawing.Size(85, 17);
            this.scale0.TabIndex = 9;
            this.scale0.TabStop = true;
            this.scale0.Text = "0.015 - 1000";
            this.scale0.UseVisualStyleBackColor = true;
            // 
            // scale1
            // 
            this.scale1.AutoSize = true;
            this.scale1.Location = new System.Drawing.Point(288, 67);
            this.scale1.Name = "scale1";
            this.scale1.Size = new System.Drawing.Size(79, 17);
            this.scale1.TabIndex = 10;
            this.scale1.TabStop = true;
            this.scale1.Text = "0.06 - 4000";
            this.scale1.UseVisualStyleBackColor = true;
            // 
            // scale2
            // 
            this.scale2.AutoSize = true;
            this.scale2.Location = new System.Drawing.Point(196, 92);
            this.scale2.Name = "scale2";
            this.scale2.Size = new System.Drawing.Size(85, 17);
            this.scale2.TabIndex = 11;
            this.scale2.TabStop = true;
            this.scale2.Text = "0.24 - 16000";
            this.scale2.UseVisualStyleBackColor = true;
            // 
            // scale3
            // 
            this.scale3.AutoSize = true;
            this.scale3.Location = new System.Drawing.Point(288, 90);
            this.scale3.Name = "scale3";
            this.scale3.Size = new System.Drawing.Size(85, 17);
            this.scale3.TabIndex = 12;
            this.scale3.TabStop = true;
            this.scale3.Text = "0.96 - 64000";
            this.scale3.UseVisualStyleBackColor = true;
            // 
            // brightness
            // 
            this.brightness.Location = new System.Drawing.Point(196, 115);
            this.brightness.Maximum = 264;
            this.brightness.Name = "brightness";
            this.brightness.Size = new System.Drawing.Size(199, 17);
            this.brightness.TabIndex = 13;
            this.brightness.Scroll += new System.Windows.Forms.ScrollEventHandler(this.brightness_Scroll);
            // 
            // brightnessText
            // 
            this.brightnessText.AutoSize = true;
            this.brightnessText.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.brightnessText.Location = new System.Drawing.Point(359, 145);
            this.brightnessText.Name = "brightnessText";
            this.brightnessText.Size = new System.Drawing.Size(13, 13);
            this.brightnessText.TabIndex = 14;
            this.brightnessText.Text = "0";
            this.brightnessText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // com10
            // 
            this.com10.AutoSize = true;
            this.com10.Location = new System.Drawing.Point(15, 126);
            this.com10.Name = "com10";
            this.com10.Size = new System.Drawing.Size(61, 17);
            this.com10.TabIndex = 4;
            this.com10.TabStop = true;
            this.com10.Text = "COM10";
            this.com10.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 464);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton com4;
        private System.Windows.Forms.RadioButton com3;
        private System.Windows.Forms.RadioButton com2;
        private System.Windows.Forms.RadioButton com1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button setMonitorPower;
        private System.Windows.Forms.Button setBrightnessRequest;
        private System.Windows.Forms.Button setSensorScale;
        private System.Windows.Forms.Button versionRequest;
        private System.Windows.Forms.Button pollRequest;
        private System.Windows.Forms.Button queryRequest;
        private System.Windows.Forms.Button setMode;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox response;
        private System.Windows.Forms.CheckBox power;
        private System.Windows.Forms.CheckBox normal;
        private System.Windows.Forms.RadioButton scale3;
        private System.Windows.Forms.RadioButton scale2;
        private System.Windows.Forms.RadioButton scale1;
        private System.Windows.Forms.RadioButton scale0;
        private System.Windows.Forms.Label brightnessText;
        private System.Windows.Forms.HScrollBar brightness;
        private System.Windows.Forms.RadioButton com10;
    }
}

