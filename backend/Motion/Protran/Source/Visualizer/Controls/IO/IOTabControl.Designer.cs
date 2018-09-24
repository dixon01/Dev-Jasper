namespace Gorba.Motion.Protran.Visualizer.Controls.IO
{
    partial class IOTabControl
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sideTab1 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab2 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab3 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab4 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.ioTransformationControl1 = new Gorba.Motion.Protran.Visualizer.Controls.IO.IoTransformationControl();
            this.buttonControl1 = new Gorba.Motion.Protran.Visualizer.Controls.IO.ButtonControl();
            this.ioLogsControl1 = new Gorba.Motion.Protran.Visualizer.Controls.IO.IoLogsControl();
            this.ioGenericDataControl1 = new Gorba.Motion.Protran.Visualizer.Controls.IO.IoGenericDataControl();
            this.panelMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMain.Controls.Add(this.ioTransformationControl1);
            this.panelMain.Controls.Add(this.buttonControl1);
            this.panelMain.Controls.Add(this.ioLogsControl1);
            this.panelMain.Controls.Add(this.ioGenericDataControl1);
            this.panelMain.Location = new System.Drawing.Point(186, 3);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(405, 340);
            this.panelMain.TabIndex = 11;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.sideTab1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sideTab2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.sideTab3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.sideTab4, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 20);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 20, 3, 20);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(184, 306);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // sideTab1
            // 
            this.sideTab1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab1.Location = new System.Drawing.Point(0, 0);
            this.sideTab1.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab1.Name = "sideTab1";
            this.sideTab1.Size = new System.Drawing.Size(184, 76);
            this.sideTab1.TabIndex = 0;
            this.sideTab1.Title = "I/O";
            this.sideTab1.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab2
            // 
            this.sideTab2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab2.Location = new System.Drawing.Point(0, 76);
            this.sideTab2.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab2.Name = "sideTab2";
            this.sideTab2.Size = new System.Drawing.Size(184, 76);
            this.sideTab2.TabIndex = 1;
            this.sideTab2.Title = "Transformation";
            this.sideTab2.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab3
            // 
            this.sideTab3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab3.Location = new System.Drawing.Point(0, 152);
            this.sideTab3.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab3.Name = "sideTab3";
            this.sideTab3.Size = new System.Drawing.Size(184, 76);
            this.sideTab3.TabIndex = 2;
            this.sideTab3.Title = "Generic Data";
            this.sideTab3.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab4
            // 
            this.sideTab4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab4.Location = new System.Drawing.Point(0, 228);
            this.sideTab4.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab4.Name = "sideTab4";
            this.sideTab4.Size = new System.Drawing.Size(184, 78);
            this.sideTab4.TabIndex = 3;
            this.sideTab4.Title = "Logs";
            this.sideTab4.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // ioTransformationControl1
            // 
            this.ioTransformationControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ioTransformationControl1.Location = new System.Drawing.Point(3, 3);
            this.ioTransformationControl1.Name = "ioTransformationControl1";
            this.ioTransformationControl1.Size = new System.Drawing.Size(397, 332);
            this.ioTransformationControl1.TabIndex = 8;
            // 
            // buttonControl1
            // 
            this.buttonControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonControl1.Location = new System.Drawing.Point(3, 3);
            this.buttonControl1.Name = "buttonControl1";
            this.buttonControl1.Size = new System.Drawing.Size(397, 332);
            this.buttonControl1.TabIndex = 7;
            // 
            // ioLogsControl1
            // 
            this.ioLogsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ioLogsControl1.Location = new System.Drawing.Point(3, 3);
            this.ioLogsControl1.Name = "ioLogsControl1";
            this.ioLogsControl1.Size = new System.Drawing.Size(397, 332);
            this.ioLogsControl1.TabIndex = 9;
            // 
            // ioGenericDataControl1
            // 
            this.ioGenericDataControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ioGenericDataControl1.Location = new System.Drawing.Point(3, 3);
            this.ioGenericDataControl1.Name = "ioGenericDataControl1";
            this.ioGenericDataControl1.Size = new System.Drawing.Size(397, 332);
            this.ioGenericDataControl1.TabIndex = 10;
            // 
            // IOTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panelMain);
            this.Name = "IOTabControl";
            this.Size = new System.Drawing.Size(594, 346);
            this.panelMain.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SideTab sideTab1;
        private SideTab sideTab2;
        private SideTab sideTab3;
        private SideTab sideTab4;
        private ButtonControl buttonControl1;
        private IoTransformationControl ioTransformationControl1;
        private IoLogsControl ioLogsControl1;
        private IoGenericDataControl ioGenericDataControl1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;


    }
}
