namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    partial class UnitConfigControl
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.unitTree = new Gorba.Motion.Update.UsbUpdateManager.Controls.UnitTree();
            this.unitDetailsControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.UnitDetailsControl();
            this.unitGroupDetailsControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.UnitGroupDetailsControl();
            this.buttonImport = new System.Windows.Forms.Button();
            this.contextMenuStripImport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.unitTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.buttonImport);
            this.splitContainer1.Panel2.Controls.Add(this.unitDetailsControl);
            this.splitContainer1.Panel2.Controls.Add(this.unitGroupDetailsControl);
            this.splitContainer1.Size = new System.Drawing.Size(794, 503);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.TabIndex = 0;
            // 
            // unitTree
            // 
            this.unitTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unitTree.Location = new System.Drawing.Point(0, 0);
            this.unitTree.Name = "unitTree";
            this.unitTree.Size = new System.Drawing.Size(215, 503);
            this.unitTree.TabIndex = 0;
            this.unitTree.SelectedItemChanged += new System.EventHandler(this.UnitTreeSelectedItemChanged);
            // 
            // unitDetailsControl
            // 
            this.unitDetailsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unitDetailsControl.Location = new System.Drawing.Point(0, 0);
            this.unitDetailsControl.Name = "unitDetailsControl";
            this.unitDetailsControl.Size = new System.Drawing.Size(575, 503);
            this.unitDetailsControl.TabIndex = 1;
            this.unitDetailsControl.Visible = false;
            // 
            // unitGroupDetailsControl
            // 
            this.unitGroupDetailsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unitGroupDetailsControl.Location = new System.Drawing.Point(0, 0);
            this.unitGroupDetailsControl.Name = "unitGroupDetailsControl";
            this.unitGroupDetailsControl.Size = new System.Drawing.Size(575, 503);
            this.unitGroupDetailsControl.TabIndex = 0;
            this.unitGroupDetailsControl.Visible = false;
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImport.Location = new System.Drawing.Point(439, 3);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(133, 23);
            this.buttonImport.TabIndex = 4;
            this.buttonImport.Text = "Import Feedback...";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.ButtonImportClick);
            // 
            // contextMenuStripImport
            // 
            this.contextMenuStripImport.Name = "contextMenuStripImport";
            this.contextMenuStripImport.Size = new System.Drawing.Size(61, 4);
            // 
            // UnitConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UnitConfigControl";
            this.Size = new System.Drawing.Size(794, 503);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private UnitTree unitTree;
        private UnitGroupDetailsControl unitGroupDetailsControl;
        private UnitDetailsControl unitDetailsControl;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripImport;
    }
}
