namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelDeveloperMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelProjectName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSaving = new System.Windows.Forms.ToolStripStatusLabel();
            this.openProjectDialog = new System.Windows.Forms.OpenFileDialog();
            this.newProjectDialog = new System.Windows.Forms.SaveFileDialog();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageUnitConfig = new System.Windows.Forms.TabPage();
            this.unitConfigControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.UnitConfigControl();
            this.tabPageUpdate = new System.Windows.Forms.TabPage();
            this.updateCreationControl = new Gorba.Motion.Update.UsbUpdateManager.Controls.UpdateCreationControl();
            this.saveAsDialog = new System.Windows.Forms.SaveFileDialog();
            this.developerModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageUnitConfig.SuspendLayout();
            this.tabPageUpdate.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(626, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.toolStripMenuItem2,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.recentProjectsToolStripMenuItem,
            this.toolStripMenuItem4,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.newProjectToolStripMenuItem.Text = "New Project...";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.NewProjectToolStripMenuItemClick);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project...";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.OpenProjectToolStripMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(192, 6);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItemClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(192, 6);
            // 
            // recentProjectsToolStripMenuItem
            // 
            this.recentProjectsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3});
            this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
            this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.recentProjectsToolStripMenuItem.Text = "Recent Projects";
            this.recentProjectsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.RecentProjectsToolStripMenuItemDropDownOpening);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(79, 22);
            this.toolStripMenuItem3.Text = "*";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(192, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.developerModeToolStripMenuItem,
            this.toolStripMenuItem5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelDeveloperMode,
            this.toolStripStatusLabelProjectName,
            this.toolStripStatusLabelSaving});
            this.statusStrip.Location = new System.Drawing.Point(0, 359);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(626, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // toolStripStatusLabelDeveloperMode
            // 
            this.toolStripStatusLabelDeveloperMode.Name = "toolStripStatusLabelDeveloperMode";
            this.toolStripStatusLabelDeveloperMode.Size = new System.Drawing.Size(102, 17);
            this.toolStripStatusLabelDeveloperMode.Text = "[Developer Mode]";
            this.toolStripStatusLabelDeveloperMode.Visible = false;
            // 
            // toolStripStatusLabelProjectName
            // 
            this.toolStripStatusLabelProjectName.Name = "toolStripStatusLabelProjectName";
            this.toolStripStatusLabelProjectName.Size = new System.Drawing.Size(272, 17);
            this.toolStripStatusLabelProjectName.Text = "Please create or open a project from the File menu";
            // 
            // toolStripStatusLabelSaving
            // 
            this.toolStripStatusLabelSaving.Name = "toolStripStatusLabelSaving";
            this.toolStripStatusLabelSaving.Size = new System.Drawing.Size(49, 17);
            this.toolStripStatusLabelSaving.Text = "(saving)";
            this.toolStripStatusLabelSaving.Visible = false;
            // 
            // openProjectDialog
            // 
            this.openProjectDialog.Filter = "Update Manager Project (*.guproj)|*.guproj";
            this.openProjectDialog.RestoreDirectory = true;
            this.openProjectDialog.SupportMultiDottedExtensions = true;
            this.openProjectDialog.Title = "Open Project";
            // 
            // newProjectDialog
            // 
            this.newProjectDialog.DefaultExt = "guproj";
            this.newProjectDialog.Filter = "Update Manager Project (*.guproj)|*.guproj";
            this.newProjectDialog.RestoreDirectory = true;
            this.newProjectDialog.Title = "Create New Project";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageUnitConfig);
            this.tabControl.Controls.Add(this.tabPageUpdate);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(626, 335);
            this.tabControl.TabIndex = 3;
            this.tabControl.Visible = false;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControlSelectedIndexChanged);
            // 
            // tabPageUnitConfig
            // 
            this.tabPageUnitConfig.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageUnitConfig.Controls.Add(this.unitConfigControl);
            this.tabPageUnitConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPageUnitConfig.Name = "tabPageUnitConfig";
            this.tabPageUnitConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUnitConfig.Size = new System.Drawing.Size(618, 309);
            this.tabPageUnitConfig.TabIndex = 0;
            this.tabPageUnitConfig.Text = "Unit Configuration";
            // 
            // unitConfigControl
            // 
            this.unitConfigControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unitConfigControl.Location = new System.Drawing.Point(3, 3);
            this.unitConfigControl.Name = "unitConfigControl";
            this.unitConfigControl.Size = new System.Drawing.Size(612, 303);
            this.unitConfigControl.TabIndex = 0;
            // 
            // tabPageUpdate
            // 
            this.tabPageUpdate.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageUpdate.Controls.Add(this.updateCreationControl);
            this.tabPageUpdate.Location = new System.Drawing.Point(4, 22);
            this.tabPageUpdate.Name = "tabPageUpdate";
            this.tabPageUpdate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUpdate.Size = new System.Drawing.Size(618, 309);
            this.tabPageUpdate.TabIndex = 1;
            this.tabPageUpdate.Text = "Update";
            // 
            // updateCreationControl
            // 
            this.updateCreationControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateCreationControl.Location = new System.Drawing.Point(3, 3);
            this.updateCreationControl.Name = "updateCreationControl";
            this.updateCreationControl.Size = new System.Drawing.Size(612, 303);
            this.updateCreationControl.TabIndex = 0;
            // 
            // saveAsDialog
            // 
            this.saveAsDialog.DefaultExt = "guproj";
            this.saveAsDialog.Filter = "Update Manager Project (*.guproj)|*.guproj";
            this.saveAsDialog.RestoreDirectory = true;
            this.saveAsDialog.Title = "Save Project As...";
            // 
            // developerModeToolStripMenuItem
            // 
            this.developerModeToolStripMenuItem.Name = "developerModeToolStripMenuItem";
            this.developerModeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.developerModeToolStripMenuItem.Text = "Developer Mode";
            this.developerModeToolStripMenuItem.Click += new System.EventHandler(this.DeveloperModeToolStripMenuItemClick);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(158, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 381);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "imotion USB Update Manager";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageUnitConfig.ResumeLayout(false);
            this.tabPageUpdate.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem recentProjectsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelProjectName;
        private System.Windows.Forms.OpenFileDialog openProjectDialog;
        private System.Windows.Forms.SaveFileDialog newProjectDialog;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageUnitConfig;
        private UnitConfigControl unitConfigControl;
        private System.Windows.Forms.TabPage tabPageUpdate;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSaving;
        private UpdateCreationControl updateCreationControl;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDeveloperMode;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveAsDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem developerModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
    }
}

