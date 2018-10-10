namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    partial class IbisTabControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sideTab5 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab4 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab3 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab2 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab1 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.sideTab6 = new Gorba.Motion.Protran.Visualizer.Controls.SideTab();
            this.panelMain = new System.Windows.Forms.Panel();
            this.genericDataControl1 = new Gorba.Motion.Protran.Visualizer.Controls.Ibis.IbisGenericDataControl();
            this.handlerControl1 = new Gorba.Motion.Protran.Visualizer.Controls.Ibis.HandlerControl();
            this.logsControl1 = new Gorba.Motion.Protran.Visualizer.Controls.Ibis.IbisLogsControl();
            this.transformationControl1 = new Gorba.Motion.Protran.Visualizer.Controls.Ibis.IbisTransformationControl();
            this.parserControl1 = new Gorba.Motion.Protran.Visualizer.Controls.Ibis.ParserControl();
            this.telegramControl1 = new Gorba.Motion.Protran.Visualizer.Controls.Ibis.TelegramControl();
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
            this.tableLayoutPanel1.Controls.Add(this.sideTab5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.sideTab4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.sideTab3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.sideTab2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.sideTab1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sideTab6, 0, 5);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 20);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 20, 3, 20);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(184, 392);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // sideTab5
            // 
            this.sideTab5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab5.Location = new System.Drawing.Point(0, 260);
            this.sideTab5.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab5.Name = "sideTab5";
            this.sideTab5.Size = new System.Drawing.Size(184, 65);
            this.sideTab5.TabIndex = 4;
            this.sideTab5.Title = "Generic Data";
            this.sideTab5.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab4
            // 
            this.sideTab4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab4.Location = new System.Drawing.Point(0, 195);
            this.sideTab4.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab4.Name = "sideTab4";
            this.sideTab4.Size = new System.Drawing.Size(184, 65);
            this.sideTab4.TabIndex = 3;
            this.sideTab4.Title = "Handler";
            this.sideTab4.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab3
            // 
            this.sideTab3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab3.Location = new System.Drawing.Point(0, 130);
            this.sideTab3.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab3.Name = "sideTab3";
            this.sideTab3.Size = new System.Drawing.Size(184, 65);
            this.sideTab3.TabIndex = 2;
            this.sideTab3.Title = "Transformation";
            this.sideTab3.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab2
            // 
            this.sideTab2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab2.Location = new System.Drawing.Point(0, 65);
            this.sideTab2.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab2.Name = "sideTab2";
            this.sideTab2.Size = new System.Drawing.Size(184, 65);
            this.sideTab2.TabIndex = 1;
            this.sideTab2.Title = "Parser";
            this.sideTab2.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab1
            // 
            this.sideTab1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab1.Location = new System.Drawing.Point(0, 0);
            this.sideTab1.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab1.Name = "sideTab1";
            this.sideTab1.Selected = true;
            this.sideTab1.Size = new System.Drawing.Size(184, 65);
            this.sideTab1.TabIndex = 0;
            this.sideTab1.Title = "Telegram";
            this.sideTab1.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // sideTab6
            // 
            this.sideTab6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideTab6.Location = new System.Drawing.Point(0, 325);
            this.sideTab6.Margin = new System.Windows.Forms.Padding(0);
            this.sideTab6.Name = "sideTab6";
            this.sideTab6.Size = new System.Drawing.Size(184, 67);
            this.sideTab6.TabIndex = 5;
            this.sideTab6.Title = "Logs";
            this.sideTab6.Click += new System.EventHandler(this.OnSideTabClick);
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMain.Controls.Add(this.genericDataControl1);
            this.panelMain.Controls.Add(this.handlerControl1);
            this.panelMain.Controls.Add(this.logsControl1);
            this.panelMain.Controls.Add(this.transformationControl1);
            this.panelMain.Controls.Add(this.parserControl1);
            this.panelMain.Controls.Add(this.telegramControl1);
            this.panelMain.Location = new System.Drawing.Point(186, 3);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(513, 426);
            this.panelMain.TabIndex = 3;
            // 
            // genericDataControl1
            // 
            this.genericDataControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.genericDataControl1.Location = new System.Drawing.Point(3, 3);
            this.genericDataControl1.Name = "genericDataControl1";
            this.genericDataControl1.Size = new System.Drawing.Size(507, 420);
            this.genericDataControl1.TabIndex = 10;
            // 
            // handlerControl1
            // 
            this.handlerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.handlerControl1.Location = new System.Drawing.Point(3, 3);
            this.handlerControl1.Name = "handlerControl1";
            this.handlerControl1.Size = new System.Drawing.Size(507, 420);
            this.handlerControl1.TabIndex = 9;
            // 
            // logsControl1
            // 
            this.logsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logsControl1.Location = new System.Drawing.Point(3, 3);
            this.logsControl1.Name = "logsControl1";
            this.logsControl1.Size = new System.Drawing.Size(507, 420);
            this.logsControl1.TabIndex = 8;
            // 
            // transformationControl1
            // 
            this.transformationControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transformationControl1.BackColor = System.Drawing.Color.White;
            this.transformationControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.transformationControl1.Location = new System.Drawing.Point(3, 3);
            this.transformationControl1.Name = "transformationControl1";
            this.transformationControl1.Size = new System.Drawing.Size(507, 420);
            this.transformationControl1.TabIndex = 7;
            // 
            // parserControl1
            // 
            this.parserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.parserControl1.Location = new System.Drawing.Point(3, 3);
            this.parserControl1.Name = "parserControl1";
            this.parserControl1.Size = new System.Drawing.Size(507, 420);
            this.parserControl1.TabIndex = 6;
            // 
            // telegramControl1
            // 
            this.telegramControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.telegramControl1.Location = new System.Drawing.Point(3, 3);
            this.telegramControl1.Name = "telegramControl1";
            this.telegramControl1.Size = new System.Drawing.Size(507, 420);
            this.telegramControl1.TabIndex = 5;
            // 
            // IbisTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panelMain);
            this.Name = "IbisTabControl";
            this.Size = new System.Drawing.Size(702, 432);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private SideTab sideTab5;
        private SideTab sideTab4;
        private SideTab sideTab3;
        private SideTab sideTab2;
        private SideTab sideTab1;
        private System.Windows.Forms.Panel panelMain;
        private TelegramControl telegramControl1;
        private ParserControl parserControl1;
        private IbisTransformationControl transformationControl1;
        private IbisLogsControl logsControl1;
        private HandlerControl handlerControl1;
        private SideTab sideTab6;
        private IbisGenericDataControl genericDataControl1;
    }
}
