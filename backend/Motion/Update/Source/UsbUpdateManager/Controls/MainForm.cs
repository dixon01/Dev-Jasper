// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    using Gorba.Motion.Update.UsbUpdateManager.Properties;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The main form of the USB Update Manager.
    /// </summary>
    public partial class MainForm : Form
    {
        private const int MaxRecentCount = 5;

        private readonly IProjectManager projectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            this.Size = new Size(800, 600);

            this.toolStripStatusLabelDeveloperMode.Visible = Settings.Default.DeveloperMode;
            this.developerModeToolStripMenuItem.Checked = Settings.Default.DeveloperMode;

            this.projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
            this.projectManager.Saving += this.ProjectManagerOnSaving;
            this.projectManager.Saved += this.ProjectManagerOnSaved;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var fileVersion = this.GetFileVersion().FileVersion;
            if (Settings.Default.CurrentVersion != fileVersion)
            {
                Settings.Default.Upgrade();
                Settings.Default.CurrentVersion = fileVersion;
                Settings.Default.Save();
            }

            var recent = Settings.Default.RecentProjectFiles;
            if (recent == null)
            {
                Settings.Default.RecentProjectFiles = new StringCollection();
                return;
            }

            // open the first available file
            foreach (var file in recent)
            {
                if (File.Exists(file))
                {
                    this.OpenProject(file, false);
                    break;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnClosed(EventArgs e)
        {
            Settings.Default.Save();

            base.OnClosed(e);
        }

        private void OpenProject(string filename, bool create)
        {
            try
            {
                if (create)
                {
                    this.projectManager.Create(filename);
                }
                else
                {
                    this.projectManager.Load(filename);
                }

                var recent = Settings.Default.RecentProjectFiles;
                recent.Remove(filename);
                recent.Insert(0, filename);
                while (recent.Count > MaxRecentCount)
                {
                    recent.RemoveAt(MaxRecentCount);
                }

                this.saveAsToolStripMenuItem.Enabled = true;
                this.tabControl.Visible = true;
                this.toolStripStatusLabelProjectName.Text = filename;
                if (this.projectManager.CurrentProjectReadOnly)
                {
                    this.toolStripStatusLabelProjectName.Text += " (read-only)";
                }

                this.toolStripStatusLabelSaving.Visible = false;
                this.updateCreationControl.SelectedUnitGroup = null;
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
            }
        }

        private FileVersionInfo GetFileVersion()
        {
            var asm = Assembly.GetEntryAssembly();
            return FileVersionInfo.GetVersionInfo(asm.Location);
        }

        private void ShowException(Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void NewProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.newProjectDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.OpenProject(this.newProjectDialog.FileName, true);
        }

        private void OpenProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.openProjectDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.OpenProject(this.openProjectDialog.FileName, false);
        }

        private void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            var msg =
                "Saving a copy of the current project will only retain all unit groups and units with their "
                + "current directory structures without updates, feedback and log files.\r\n"
                + "All resources required for the current directory structures are also copied to the new project.";
            if (MessageBox.Show(this, msg, "Save As...", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                != DialogResult.OK)
            {
                return;
            }

            if (this.saveAsDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var fileName = this.saveAsDialog.FileName;
            if (!this.projectManager.SaveAs(fileName))
            {
                return;
            }

            msg = string.Format("Do you want to open the newly created copy of the project?\r\n{0}", fileName);
            if (MessageBox.Show(
                this, msg, "Save As...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.OpenProject(fileName, false);
            }
        }

        private void RecentProjectsToolStripMenuItemDropDownOpening(object sender, EventArgs e)
        {
            this.recentProjectsToolStripMenuItem.DropDownItems.Clear();
            foreach (var file in Settings.Default.RecentProjectFiles)
            {
                var item = this.recentProjectsToolStripMenuItem.DropDownItems.Add(file);
                item.Enabled = File.Exists(file);
                item.Tag = file;
                item.Click += this.RecentProjectsDropDownItemOnClick;
            }
        }

        private void RecentProjectsDropDownItemOnClick(object sender, EventArgs eventArgs)
        {
            var item = sender as ToolStripItem;
            if (item == null)
            {
                return;
            }

            var fileName = item.Tag as string;
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            this.OpenProject(fileName, false);
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            var version = this.GetFileVersion();
            MessageBox.Show(
                this,
                string.Format("{0} {1}\r\n{2}", version.FileDescription, version.FileVersion, version.LegalCopyright),
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.None);
        }

        private void ToggleDeveloperMode()
        {
            if (Settings.Default.DeveloperMode)
            {
                this.developerModeToolStripMenuItem.Checked = false;
                Settings.Default.DeveloperMode = false;
                Settings.Default.Save();
                MessageBox.Show(
                    this,
                    "Developer Mode has been disabled, please restart the application",
                    "Developer Mode",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }

            var message = "Do you really want to enable Developer Mode?" + Environment.NewLine
                          + "Developer Mode gives you new features that are not fully tested and not documented. "
                          + "Use those features at your own risk!";
            if (MessageBox.Show(
                this,
                message,
                "Developer Mode",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            this.developerModeToolStripMenuItem.Checked = true;
            Settings.Default.DeveloperMode = true;
            Settings.Default.Save();
            MessageBox.Show(
                this,
                "Developer Mode has been enabled, please restart the application",
                "Developer Mode",
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk);
        }

        private void ProjectManagerOnSaving(object sender, CancelEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CancelEventHandler(this.ProjectManagerOnSaving), sender, e);
                return;
            }

            this.toolStripStatusLabelSaving.Visible = true;
            Application.DoEvents();
        }

        private void ProjectManagerOnSaved(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.ProjectManagerOnSaved));
                return;
            }

            this.toolStripStatusLabelSaving.Visible = false;
        }

        private void TabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            this.updateCreationControl.SelectedUnitGroup = null;
        }

        private void DeveloperModeToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.ToggleDeveloperMode();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
