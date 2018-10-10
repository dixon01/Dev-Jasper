namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    partial class BitmapCreatorControl
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
            this.panelDrawing = new System.Windows.Forms.Panel();
            this.panelScrolling = new System.Windows.Forms.Panel();
            this.comboBoxZoom = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelColor = new System.Windows.Forms.Panel();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelScrolling.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDrawing
            // 
            this.panelDrawing.BackColor = System.Drawing.Color.White;
            this.panelDrawing.Cursor = System.Windows.Forms.Cursors.Cross;
            this.panelDrawing.Location = new System.Drawing.Point(0, 0);
            this.panelDrawing.Name = "panelDrawing";
            this.panelDrawing.Size = new System.Drawing.Size(112, 16);
            this.panelDrawing.TabIndex = 0;
            this.panelDrawing.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelDrawingPaint);
            this.panelDrawing.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelDrawingMouseDown);
            this.panelDrawing.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PanelDrawingMouseMove);
            // 
            // panelScrolling
            // 
            this.panelScrolling.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelScrolling.AutoScroll = true;
            this.panelScrolling.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelScrolling.Controls.Add(this.panelDrawing);
            this.panelScrolling.Cursor = System.Windows.Forms.Cursors.No;
            this.panelScrolling.Location = new System.Drawing.Point(3, 30);
            this.panelScrolling.Name = "panelScrolling";
            this.panelScrolling.Size = new System.Drawing.Size(436, 268);
            this.panelScrolling.TabIndex = 1;
            // 
            // comboBoxZoom
            // 
            this.comboBoxZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxZoom.FormattingEnabled = true;
            this.comboBoxZoom.Items.AddRange(new object[] {
            "25%",
            "50%",
            "100%",
            "200%",
            "400%",
            "800%"});
            this.comboBoxZoom.Location = new System.Drawing.Point(364, 3);
            this.comboBoxZoom.Name = "comboBoxZoom";
            this.comboBoxZoom.Size = new System.Drawing.Size(75, 21);
            this.comboBoxZoom.TabIndex = 2;
            this.comboBoxZoom.SelectedIndexChanged += new System.EventHandler(this.ComboBoxZoomSelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Color:";
            // 
            // panelColor
            // 
            this.panelColor.BackColor = System.Drawing.Color.White;
            this.panelColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelColor.Location = new System.Drawing.Point(43, 4);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(20, 20);
            this.panelColor.TabIndex = 4;
            this.panelColor.Click += new System.EventHandler(this.PanelColorClick);
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.Location = new System.Drawing.Point(125, 4);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(233, 20);
            this.textBox.TabIndex = 5;
            this.textBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TextBoxPreviewKeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Set Text:";
            // 
            // BitmapCreatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.panelColor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxZoom);
            this.Controls.Add(this.panelScrolling);
            this.Name = "BitmapCreatorControl";
            this.Size = new System.Drawing.Size(442, 301);
            this.panelScrolling.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelDrawing;
        private System.Windows.Forms.Panel panelScrolling;
        private System.Windows.Forms.ComboBox comboBoxZoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelColor;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label label2;
    }
}
