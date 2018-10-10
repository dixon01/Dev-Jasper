namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi;

    partial class Vdv301TabControl
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
            this.logsControl = new Gorba.Motion.Protran.Visualizer.Controls.Vdv301.Vdv301LogsControl();
            this.inputControl = new Gorba.Motion.Protran.Visualizer.Controls.Vdv301.Vdv301InputControl();
            this.transformationsControl = new Gorba.Motion.Protran.Visualizer.Controls.Vdv301.Vdv301AllTransformationsControl();
            this.genericDataControl = new Gorba.Motion.Protran.Visualizer.Controls.Vdv301.Vdv301GenericDataControl();
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
            this.sideTab1.Title = "XML Input";
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
            this.panelMain.Controls.Add(this.logsControl);
            this.panelMain.Controls.Add(this.inputControl);
            this.panelMain.Controls.Add(this.transformationsControl);
            this.panelMain.Controls.Add(this.genericDataControl);
            this.panelMain.Location = new System.Drawing.Point(186, 3);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(3);
            this.panelMain.Size = new System.Drawing.Size(408, 569);
            this.panelMain.TabIndex = 4;
            // 
            // logsControl
            // 
            this.logsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logsControl.Location = new System.Drawing.Point(3, 3);
            this.logsControl.Name = "logsControl";
            this.logsControl.Size = new System.Drawing.Size(400, 561);
            this.logsControl.TabIndex = 6;
            // 
            // inputControl
            // 
            this.inputControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputControl.Location = new System.Drawing.Point(3, 3);
            this.inputControl.Name = "inputControl";
            this.inputControl.Size = new System.Drawing.Size(400, 561);
            this.inputControl.TabIndex = 2;
            // 
            // transformationsControl
            // 
            this.transformationsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transformationsControl.Location = new System.Drawing.Point(3, 3);
            this.transformationsControl.Name = "transformationsControl";
            this.transformationsControl.Size = new System.Drawing.Size(400, 561);
            this.transformationsControl.TabIndex = 1;
            // 
            // genericDataControl
            // 
            this.genericDataControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genericDataControl.Location = new System.Drawing.Point(3, 3);
            this.genericDataControl.Name = "genericDataControl";
            this.genericDataControl.Size = new System.Drawing.Size(400, 561);
            this.genericDataControl.TabIndex = 0;
            // 
            // Vdv301TabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panelMain);
            this.Name = "Vdv301TabControl";
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
        private System.Windows.Forms.Panel panelMain;
        private Vdv301InputControl inputControl;
        private Vdv301AllTransformationsControl transformationsControl;
        private Vdv301GenericDataControl genericDataControl;
        private SideTab sideTab4;
        private Vdv301LogsControl logsControl;

    }
}
