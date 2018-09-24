namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    partial class AbuDhabiTabControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sideTab3 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab2 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab1 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab4 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.panelMain = new System.Windows.Forms.Panel();
            this.abuDhabiLogsControl1 = new Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi.AbuDhabiLogsControl();
            this.abuDhabiGenericDataControl1 = new Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi.AbuDhabiGenericDataControl();
            this.dataItemsControl1 = new Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi.DataItemsControl();
            this.abuDhabiTransformationsControl1 = new Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi.AbuDhabiTransformationsControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.sideTab3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.sideTab2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.sideTab1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sideTab4, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 20);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 20, 3, 20);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(184, 535);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // sideTab3
            // 
            this.sideTab3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab3.Location = new System.Drawing.Point(0, 266);
            this.sideTab3.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab3.Name = "sideTab3";
            this.sideTab3.Size = new System.Drawing.Size(184, 133);
            this.sideTab3.TabIndex = 4;
            this.sideTab3.Title = "Generic Data";
            this.sideTab3.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab2
            // 
            this.sideTab2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab2.Location = new System.Drawing.Point(0, 133);
            this.sideTab2.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab2.Name = "sideTab2";
            this.sideTab2.Size = new System.Drawing.Size(184, 133);
            this.sideTab2.TabIndex = 2;
            this.sideTab2.Title = "Transformation";
            this.sideTab2.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab1
            // 
            this.sideTab1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab1.Location = new System.Drawing.Point(0, 0);
            this.sideTab1.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab1.Name = "sideTab1";
            this.sideTab1.Selected = true;
            this.sideTab1.Size = new System.Drawing.Size(184, 133);
            this.sideTab1.TabIndex = 0;
            this.sideTab1.Title = "ISI Data Items";
            this.sideTab1.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab4
            // 
            this.sideTab4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab4.Location = new System.Drawing.Point(0, 399);
            this.sideTab4.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab4.Name = "sideTab4";
            this.sideTab4.Size = new System.Drawing.Size(184, 136);
            this.sideTab4.TabIndex = 5;
            this.sideTab4.Title = "Logs";
            this.sideTab4.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMain.Controls.Add(this.abuDhabiTransformationsControl1);
            this.panelMain.Controls.Add(this.abuDhabiLogsControl1);
            this.panelMain.Controls.Add(this.abuDhabiGenericDataControl1);
            this.panelMain.Controls.Add(this.dataItemsControl1);
            this.panelMain.Location = new System.Drawing.Point(186, 3);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(3);
            this.panelMain.Size = new System.Drawing.Size(408, 569);
            this.panelMain.TabIndex = 4;
            // 
            // abuDhabiLogsControl1
            // 
            this.abuDhabiLogsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.abuDhabiLogsControl1.Location = new System.Drawing.Point(3, 3);
            this.abuDhabiLogsControl1.Name = "abuDhabiLogsControl1";
            this.abuDhabiLogsControl1.Size = new System.Drawing.Size(400, 561);
            this.abuDhabiLogsControl1.TabIndex = 7;
            // 
            // abuDhabiGenericDataControl1
            // 
            this.abuDhabiGenericDataControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.abuDhabiGenericDataControl1.Location = new System.Drawing.Point(3, 3);
            this.abuDhabiGenericDataControl1.Name = "abuDhabiGenericDataControl1";
            this.abuDhabiGenericDataControl1.Size = new System.Drawing.Size(400, 561);
            this.abuDhabiGenericDataControl1.TabIndex = 6;
            // 
            // dataItemsControl1
            // 
            this.dataItemsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataItemsControl1.Location = new System.Drawing.Point(3, 3);
            this.dataItemsControl1.Name = "dataItemsControl1";
            this.dataItemsControl1.Size = new System.Drawing.Size(400, 561);
            this.dataItemsControl1.TabIndex = 1;
            // 
            // abuDhabiTransformationsControl1
            // 
            this.abuDhabiTransformationsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.abuDhabiTransformationsControl1.Location = new System.Drawing.Point(3, 3);
            this.abuDhabiTransformationsControl1.Name = "abuDhabiTransformationsControl1";
            this.abuDhabiTransformationsControl1.Size = new System.Drawing.Size(400, 561);
            this.abuDhabiTransformationsControl1.TabIndex = 8;
            // 
            // AbuDhabiTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panelMain);
            this.Name = "AbuDhabiTabControl";
            this.Size = new System.Drawing.Size(597, 575);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private SideTab sideTab3;
        private SideTab sideTab2;
        private SideTab sideTab1;
        private SideTab sideTab4;
        private System.Windows.Forms.Panel panelMain;
        private DataItemsControl dataItemsControl1;
        private AbuDhabiGenericDataControl abuDhabiGenericDataControl1;
        private AbuDhabiLogsControl abuDhabiLogsControl1;
        private AbuDhabiTransformationsControl abuDhabiTransformationsControl1;
    }
}
