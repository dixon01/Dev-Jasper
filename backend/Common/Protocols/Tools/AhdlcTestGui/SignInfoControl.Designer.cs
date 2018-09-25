    namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    partial class SignInfoControl : IDataSendContext
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
            this.timerStatusRequest = new System.Windows.Forms.Timer(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageStatus = new System.Windows.Forms.TabPage();
            this.propertyGridStatus = new System.Windows.Forms.PropertyGrid();
            this.tabPageStaticBitmap = new System.Windows.Forms.TabPage();
            this.tabPageScrollingBitmap = new System.Windows.Forms.TabPage();
            this.tabPageBlockScroll = new System.Windows.Forms.TabPage();
            this.tabPageAutoText = new System.Windows.Forms.TabPage();
            this.tabPageScrollText = new System.Windows.Forms.TabPage();
            this.tabPageStaticText = new System.Windows.Forms.TabPage();
            this.tabPageColor = new System.Windows.Forms.TabPage();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.buttonSizeFromStatus = new System.Windows.Forms.Button();
            this.checkBoxColor = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.staticBitmapControl = new Gorba.Common.Protocols.Tools.AhdlcTestGui.StaticBitmapControl();
            this.scrollingBitmapControl = new Gorba.Common.Protocols.Tools.AhdlcTestGui.ScrollingBitmapControl();
            this.blockScrollBitmapControl1 = new Gorba.Common.Protocols.Tools.AhdlcTestGui.BlockScrollBitmapControl();
            this.textSendControlAutoText = new Gorba.Common.Protocols.Tools.AhdlcTestGui.TextSendControl();
            this.textSendControlScrollText = new Gorba.Common.Protocols.Tools.AhdlcTestGui.TextSendControl();
            this.textSendControlStaticText = new Gorba.Common.Protocols.Tools.AhdlcTestGui.TextSendControl();
            this.colorBitmapControl1 = new Gorba.Common.Protocols.Tools.AhdlcTestGui.ColorBitmapControl();
            this.tabControl.SuspendLayout();
            this.tabPageStatus.SuspendLayout();
            this.tabPageStaticBitmap.SuspendLayout();
            this.tabPageScrollingBitmap.SuspendLayout();
            this.tabPageBlockScroll.SuspendLayout();
            this.tabPageAutoText.SuspendLayout();
            this.tabPageScrollText.SuspendLayout();
            this.tabPageStaticText.SuspendLayout();
            this.tabPageColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxEnabled
            // 
            this.checkBoxEnabled.AutoSize = true;
            this.checkBoxEnabled.Location = new System.Drawing.Point(3, 6);
            this.checkBoxEnabled.Name = "checkBoxEnabled";
            this.checkBoxEnabled.Size = new System.Drawing.Size(65, 17);
            this.checkBoxEnabled.TabIndex = 0;
            this.checkBoxEnabled.Text = "Enabled";
            this.checkBoxEnabled.UseVisualStyleBackColor = true;
            this.checkBoxEnabled.CheckedChanged += new System.EventHandler(this.CheckBoxEnabledCheckedChanged);
            // 
            // timerStatusRequest
            // 
            this.timerStatusRequest.Interval = 10000;
            this.timerStatusRequest.Tick += new System.EventHandler(this.TimerStatusRequestTick);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl.Controls.Add(this.tabPageStatus);
            this.tabControl.Controls.Add(this.tabPageStaticBitmap);
            this.tabControl.Controls.Add(this.tabPageScrollingBitmap);
            this.tabControl.Controls.Add(this.tabPageBlockScroll);
            this.tabControl.Controls.Add(this.tabPageAutoText);
            this.tabControl.Controls.Add(this.tabPageScrollText);
            this.tabControl.Controls.Add(this.tabPageStaticText);
            this.tabControl.Controls.Add(this.tabPageColor);
            this.tabControl.Location = new System.Drawing.Point(3, 32);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(536, 334);
            this.tabControl.TabIndex = 1;
            // 
            // tabPageStatus
            // 
            this.tabPageStatus.Controls.Add(this.propertyGridStatus);
            this.tabPageStatus.Location = new System.Drawing.Point(4, 25);
            this.tabPageStatus.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageStatus.Name = "tabPageStatus";
            this.tabPageStatus.Size = new System.Drawing.Size(528, 305);
            this.tabPageStatus.TabIndex = 0;
            this.tabPageStatus.Text = "Status";
            this.tabPageStatus.UseVisualStyleBackColor = true;
            // 
            // propertyGridStatus
            // 
            this.propertyGridStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridStatus.HelpVisible = false;
            this.propertyGridStatus.Location = new System.Drawing.Point(0, 0);
            this.propertyGridStatus.Name = "propertyGridStatus";
            this.propertyGridStatus.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridStatus.Size = new System.Drawing.Size(528, 305);
            this.propertyGridStatus.TabIndex = 0;
            this.propertyGridStatus.ToolbarVisible = false;
            // 
            // tabPageStaticBitmap
            // 
            this.tabPageStaticBitmap.Controls.Add(this.staticBitmapControl);
            this.tabPageStaticBitmap.Location = new System.Drawing.Point(4, 25);
            this.tabPageStaticBitmap.Name = "tabPageStaticBitmap";
            this.tabPageStaticBitmap.Size = new System.Drawing.Size(528, 305);
            this.tabPageStaticBitmap.TabIndex = 1;
            this.tabPageStaticBitmap.Text = "0x00";
            this.tabPageStaticBitmap.ToolTipText = "Static Bitmap (0x00)";
            this.tabPageStaticBitmap.UseVisualStyleBackColor = true;
            // 
            // tabPageScrollingBitmap
            // 
            this.tabPageScrollingBitmap.Controls.Add(this.scrollingBitmapControl);
            this.tabPageScrollingBitmap.Location = new System.Drawing.Point(4, 25);
            this.tabPageScrollingBitmap.Name = "tabPageScrollingBitmap";
            this.tabPageScrollingBitmap.Size = new System.Drawing.Size(528, 305);
            this.tabPageScrollingBitmap.TabIndex = 2;
            this.tabPageScrollingBitmap.Text = "0x01";
            this.tabPageScrollingBitmap.ToolTipText = "Scrolling Bitmap (0x01)";
            this.tabPageScrollingBitmap.UseVisualStyleBackColor = true;
            // 
            // tabPageBlockScroll
            // 
            this.tabPageBlockScroll.Controls.Add(this.blockScrollBitmapControl1);
            this.tabPageBlockScroll.Location = new System.Drawing.Point(4, 25);
            this.tabPageBlockScroll.Name = "tabPageBlockScroll";
            this.tabPageBlockScroll.Size = new System.Drawing.Size(528, 305);
            this.tabPageBlockScroll.TabIndex = 3;
            this.tabPageBlockScroll.Text = "0x04";
            this.tabPageBlockScroll.ToolTipText = "Block Scroll (0x04)";
            this.tabPageBlockScroll.UseVisualStyleBackColor = true;
            // 
            // tabPageAutoText
            // 
            this.tabPageAutoText.Controls.Add(this.textSendControlAutoText);
            this.tabPageAutoText.Location = new System.Drawing.Point(4, 25);
            this.tabPageAutoText.Name = "tabPageAutoText";
            this.tabPageAutoText.Size = new System.Drawing.Size(528, 305);
            this.tabPageAutoText.TabIndex = 4;
            this.tabPageAutoText.Text = "0x10";
            this.tabPageAutoText.ToolTipText = "Auto Text (0x10)";
            this.tabPageAutoText.UseVisualStyleBackColor = true;
            // 
            // tabPageScrollText
            // 
            this.tabPageScrollText.Controls.Add(this.textSendControlScrollText);
            this.tabPageScrollText.Location = new System.Drawing.Point(4, 25);
            this.tabPageScrollText.Name = "tabPageScrollText";
            this.tabPageScrollText.Size = new System.Drawing.Size(528, 305);
            this.tabPageScrollText.TabIndex = 5;
            this.tabPageScrollText.Text = "0x11";
            this.tabPageScrollText.ToolTipText = "Scroll Text (0x11)";
            this.tabPageScrollText.UseVisualStyleBackColor = true;
            // 
            // tabPageStaticText
            // 
            this.tabPageStaticText.Controls.Add(this.textSendControlStaticText);
            this.tabPageStaticText.Location = new System.Drawing.Point(4, 25);
            this.tabPageStaticText.Name = "tabPageStaticText";
            this.tabPageStaticText.Size = new System.Drawing.Size(528, 305);
            this.tabPageStaticText.TabIndex = 6;
            this.tabPageStaticText.Text = "0x12";
            this.tabPageStaticText.ToolTipText = "Static Text (0x12)";
            this.tabPageStaticText.UseVisualStyleBackColor = true;
            // 
            // tabPageColor
            // 
            this.tabPageColor.Controls.Add(this.colorBitmapControl1);
            this.tabPageColor.Location = new System.Drawing.Point(4, 25);
            this.tabPageColor.Name = "tabPageColor";
            this.tabPageColor.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageColor.Size = new System.Drawing.Size(528, 305);
            this.tabPageColor.TabIndex = 9;
            this.tabPageColor.Text = "Color";
            this.tabPageColor.ToolTipText = "Color Bitmap";
            this.tabPageColor.UseVisualStyleBackColor = true;
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Increment = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numericUpDownWidth.Location = new System.Drawing.Point(178, 5);
            this.numericUpDownWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownWidth.TabIndex = 2;
            this.numericUpDownWidth.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(142, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(242, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "x";
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownHeight.Location = new System.Drawing.Point(260, 5);
            this.numericUpDownHeight.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownHeight.TabIndex = 2;
            this.numericUpDownHeight.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // buttonSizeFromStatus
            // 
            this.buttonSizeFromStatus.Enabled = false;
            this.buttonSizeFromStatus.Location = new System.Drawing.Point(324, 3);
            this.buttonSizeFromStatus.Name = "buttonSizeFromStatus";
            this.buttonSizeFromStatus.Size = new System.Drawing.Size(75, 23);
            this.buttonSizeFromStatus.TabIndex = 3;
            this.buttonSizeFromStatus.Text = "From Status";
            this.buttonSizeFromStatus.UseVisualStyleBackColor = true;
            this.buttonSizeFromStatus.Click += new System.EventHandler(this.ButtonSizeFromStatusClick);
            // 
            // checkBoxColor
            // 
            this.checkBoxColor.AutoSize = true;
            this.checkBoxColor.Location = new System.Drawing.Point(121, 7);
            this.checkBoxColor.Name = "checkBoxColor";
            this.checkBoxColor.Size = new System.Drawing.Size(15, 14);
            this.checkBoxColor.TabIndex = 0;
            this.checkBoxColor.UseVisualStyleBackColor = true;
            this.checkBoxColor.CheckedChanged += new System.EventHandler(this.CheckBoxColorCheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(74, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 26);
            this.panel1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(81, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Color:";
            // 
            // staticBitmapControl
            // 
            this.staticBitmapControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.staticBitmapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.staticBitmapControl.Location = new System.Drawing.Point(0, 0);
            this.staticBitmapControl.Name = "staticBitmapControl";
            this.staticBitmapControl.Size = new System.Drawing.Size(528, 305);
            this.staticBitmapControl.TabIndex = 0;
            // 
            // scrollingBitmapControl
            // 
            this.scrollingBitmapControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scrollingBitmapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollingBitmapControl.Location = new System.Drawing.Point(0, 0);
            this.scrollingBitmapControl.Name = "scrollingBitmapControl";
            this.scrollingBitmapControl.Size = new System.Drawing.Size(528, 305);
            this.scrollingBitmapControl.TabIndex = 0;
            // 
            // blockScrollBitmapControl1
            // 
            this.blockScrollBitmapControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.blockScrollBitmapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockScrollBitmapControl1.Location = new System.Drawing.Point(0, 0);
            this.blockScrollBitmapControl1.Name = "blockScrollBitmapControl1";
            this.blockScrollBitmapControl1.Size = new System.Drawing.Size(528, 305);
            this.blockScrollBitmapControl1.TabIndex = 0;
            // 
            // textSendControlAutoText
            // 
            this.textSendControlAutoText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textSendControlAutoText.DisplayMode = Gorba.Common.Protocols.Ahdlc.Frames.DisplayMode.AutoText;
            this.textSendControlAutoText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textSendControlAutoText.Location = new System.Drawing.Point(0, 0);
            this.textSendControlAutoText.Name = "textSendControlAutoText";
            this.textSendControlAutoText.Size = new System.Drawing.Size(528, 305);
            this.textSendControlAutoText.TabIndex = 0;
            // 
            // textSendControlScrollText
            // 
            this.textSendControlScrollText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textSendControlScrollText.DisplayMode = Gorba.Common.Protocols.Ahdlc.Frames.DisplayMode.ScrollText;
            this.textSendControlScrollText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textSendControlScrollText.Location = new System.Drawing.Point(0, 0);
            this.textSendControlScrollText.Name = "textSendControlScrollText";
            this.textSendControlScrollText.Size = new System.Drawing.Size(528, 305);
            this.textSendControlScrollText.TabIndex = 0;
            // 
            // textSendControlStaticText
            // 
            this.textSendControlStaticText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textSendControlStaticText.DisplayMode = Gorba.Common.Protocols.Ahdlc.Frames.DisplayMode.StaticText;
            this.textSendControlStaticText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textSendControlStaticText.Location = new System.Drawing.Point(0, 0);
            this.textSendControlStaticText.Name = "textSendControlStaticText";
            this.textSendControlStaticText.Size = new System.Drawing.Size(528, 305);
            this.textSendControlStaticText.TabIndex = 0;
            // 
            // colorBitmapControl1
            // 
            this.colorBitmapControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorBitmapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorBitmapControl1.Location = new System.Drawing.Point(3, 3);
            this.colorBitmapControl1.Name = "colorBitmapControl1";
            this.colorBitmapControl1.Size = new System.Drawing.Size(522, 299);
            this.colorBitmapControl1.TabIndex = 0;
            // 
            // SignInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonSizeFromStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownHeight);
            this.Controls.Add(this.numericUpDownWidth);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.checkBoxColor);
            this.Controls.Add(this.checkBoxEnabled);
            this.Name = "SignInfoControl";
            this.Size = new System.Drawing.Size(542, 369);
            this.tabControl.ResumeLayout(false);
            this.tabPageStatus.ResumeLayout(false);
            this.tabPageStaticBitmap.ResumeLayout(false);
            this.tabPageScrollingBitmap.ResumeLayout(false);
            this.tabPageBlockScroll.ResumeLayout(false);
            this.tabPageAutoText.ResumeLayout(false);
            this.tabPageScrollText.ResumeLayout(false);
            this.tabPageStaticText.ResumeLayout(false);
            this.tabPageColor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxEnabled;
        private System.Windows.Forms.Timer timerStatusRequest;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageStatus;
        private System.Windows.Forms.PropertyGrid propertyGridStatus;
        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownHeight;
        private System.Windows.Forms.Button buttonSizeFromStatus;
        private System.Windows.Forms.CheckBox checkBoxColor;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPageStaticBitmap;
        private ScrollingBitmapControl scrollingBitmapControl;
        private System.Windows.Forms.TabPage tabPageScrollingBitmap;
        private StaticBitmapControl staticBitmapControl;
        private System.Windows.Forms.TabPage tabPageBlockScroll;
        private BlockScrollBitmapControl blockScrollBitmapControl1;
        private System.Windows.Forms.TabPage tabPageAutoText;
        private TextSendControl textSendControlAutoText;
        private System.Windows.Forms.TabPage tabPageScrollText;
        private TextSendControl textSendControlScrollText;
        private System.Windows.Forms.TabPage tabPageStaticText;
        private TextSendControl textSendControlStaticText;
        private System.Windows.Forms.TabPage tabPageColor;
        private ColorBitmapControl colorBitmapControl1;
    }
}
