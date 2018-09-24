namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    partial class ParserControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxData = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.propertyGridTelegram = new Gorba.Motion.Protran.Visualizer.Controls.TypePropertyGrid();
            this.propertyGridParser = new Gorba.Motion.Protran.Visualizer.Controls.TypePropertyGrid();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.propertyGridAnswer = new Gorba.Motion.Protran.Visualizer.Controls.TypePropertyGrid();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBoxData);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(636, 45);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Telegram Data";
            // 
            // textBoxData
            // 
            this.textBoxData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxData.Location = new System.Drawing.Point(6, 19);
            this.textBoxData.Name = "textBoxData";
            this.textBoxData.ReadOnly = true;
            this.textBoxData.Size = new System.Drawing.Size(624, 20);
            this.textBoxData.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.propertyGridParser);
            this.groupBox2.Location = new System.Drawing.Point(3, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(636, 146);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Parser";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.propertyGridTelegram);
            this.groupBox3.Location = new System.Drawing.Point(3, 206);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(636, 146);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Telegram";
            // 
            // propertyGridTelegram
            // 
            this.propertyGridTelegram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridTelegram.Location = new System.Drawing.Point(3, 16);
            this.propertyGridTelegram.Name = "propertyGridTelegram";
            this.propertyGridTelegram.SelectedObject = null;
            this.propertyGridTelegram.Size = new System.Drawing.Size(630, 127);
            this.propertyGridTelegram.TabIndex = 1;
            // 
            // propertyGridParser
            // 
            this.propertyGridParser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridParser.Location = new System.Drawing.Point(3, 16);
            this.propertyGridParser.Name = "propertyGridParser";
            this.propertyGridParser.SelectedObject = null;
            this.propertyGridParser.Size = new System.Drawing.Size(630, 127);
            this.propertyGridParser.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.propertyGridAnswer);
            this.groupBox4.Location = new System.Drawing.Point(3, 358);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(636, 146);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Answer";
            // 
            // propertyGridAnswer
            // 
            this.propertyGridAnswer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridAnswer.Location = new System.Drawing.Point(3, 16);
            this.propertyGridAnswer.Name = "propertyGridAnswer";
            this.propertyGridAnswer.SelectedObject = null;
            this.propertyGridAnswer.Size = new System.Drawing.Size(630, 127);
            this.propertyGridAnswer.TabIndex = 1;
            // 
            // ParserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ParserControl";
            this.Size = new System.Drawing.Size(642, 533);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private TypePropertyGrid propertyGridParser;
        private TypePropertyGrid propertyGridTelegram;
        private System.Windows.Forms.GroupBox groupBox4;
        private TypePropertyGrid propertyGridAnswer;
    }
}
