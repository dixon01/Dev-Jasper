// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationActionsControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationActionsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Motion.Update.UsbUpdateManager.Properties;

    /// <summary>
    /// Control that allows to view and optionally edit the pre- and post-installation commands.
    /// </summary>
    public partial class InstallationActionsControl : UserControl
    {
        private const string RegularFileKey = "RegularFile";
        private const string ExecutableFileKey = "ExecutableFile";
        private const string ExecuteLocalKey = "ExecuteLocal";

        private RunCommands runCommands;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationActionsControl"/> class.
        /// </summary>
        public InstallationActionsControl()
        {
            this.InitializeComponent();

            this.imageListLarge.Images.Add(RegularFileKey, Resources.RegularFile);
            this.imageListSmall.Images.Add(RegularFileKey, Resources.RegularFile);

            this.imageListLarge.Images.Add(ExecutableFileKey, Resources.ExecutableFile);
            this.imageListSmall.Images.Add(ExecutableFileKey, Resources.ExecutableFile);

            this.imageListLarge.Images.Add(ExecuteLocalKey, Resources.ExecuteLocal);
            this.imageListSmall.Images.Add(ExecuteLocalKey, Resources.ExecuteLocal);

            this.LoadData();
        }

        /// <summary>
        /// Gets or sets the currently shown <see cref="RunCommands"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RunCommands RunCommands
        {
            get
            {
                return this.runCommands;
            }

            set
            {
                this.runCommands = value;
                this.LoadData();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is read-only (i.e. no tool strip is shown).
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return !this.toolStrip.Visible;
            }

            set
            {
                this.toolStrip.Visible = !value;
            }
        }

        private void LoadData()
        {
            if (this.runCommands == null)
            {
                this.runCommands = new RunCommands();
            }

            this.listView.Items.Clear();
            this.UpdateButtons();
            foreach (var item in this.runCommands.Items)
            {
                this.AddItem(item);
            }
        }

        private void AddItem(FileSystemUpdate update)
        {
            var exe = update as ExecutableFile;
            if (exe != null)
            {
                var item = new ListViewItem(Path.GetFileName(exe.Name), ExecutableFileKey);
                item.SubItems.Add(exe.Arguments);
                item.Tag = exe;
                this.listView.Items.Add(item);
                return;
            }

            var file = update as FileUpdate;
            if (file != null)
            {
                var item = new ListViewItem(Path.GetFileName(file.Name), RegularFileKey);
                item.Tag = file;
                this.listView.Items.Add(item);
                return;
            }

            var run = update as RunApplication;
            if (run != null)
            {
                var item = new ListViewItem(run.Name, ExecuteLocalKey);
                item.SubItems.Add(run.Arguments);
                item.Tag = run;
                this.listView.Items.Add(item);
            }
        }

        private void UpdateButtons()
        {
            this.toolStripButtonUp.Enabled = this.listView.SelectedIndices.Count == 1
                                             && this.listView.SelectedIndices[0] > 0;
            this.toolStripButtonDown.Enabled = this.listView.SelectedIndices.Count == 1
                                               && this.listView.SelectedIndices[0] < this.listView.Items.Count - 1;
            this.toolStripButtonDelete.Enabled = this.listView.SelectedIndices.Count > 0;
        }

        private void Swap(int offset)
        {
            if (this.listView.SelectedIndices.Count != 1)
            {
                return;
            }

            var index = this.listView.SelectedIndices[0];
            var otherIndex = index + offset;

            if (otherIndex < 0 || otherIndex >= this.listView.Items.Count)
            {
                return;
            }

            int upper = Math.Min(index, otherIndex);
            int lower = Math.Max(index, otherIndex);

            this.listView.BeginUpdate();

            var itemUp = this.listView.Items[lower];
            var updateUp = this.runCommands.Items[lower];
            this.listView.Items.RemoveAt(lower);
            this.runCommands.Items.RemoveAt(lower);

            var itemDown = this.listView.Items[upper];
            var updateDown = this.runCommands.Items[upper];
            this.listView.Items.RemoveAt(upper);
            this.runCommands.Items.RemoveAt(upper);

            this.listView.Items.Insert(upper, itemUp);
            this.listView.Items.Insert(lower, itemDown);

            this.runCommands.Items.Insert(upper, updateUp);
            this.runCommands.Items.Insert(lower, updateDown);

            this.listView.EndUpdate();
        }

        private void AddExecutableFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.addExecutableFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var input = new TextInputDialog();
            input.Text = "Add Executable File";
            input.Label = "Command line arguments (optional):";
            input.InputRequired = false;

            if (input.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var item = new ExecutableFile { Name = this.addExecutableFileDialog.FileName, Arguments = input.InputText };
            this.AddItem(item);
            this.runCommands.Items.Add(item);
        }

        private void AddRegularFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.addRegularFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            foreach (var fileName in this.addRegularFileDialog.FileNames)
            {
                var item = new FileUpdate { Name = fileName };
                this.AddItem(item);
                this.runCommands.Items.Add(item);
            }
        }

        private void ExecuteLocalApplicationToolStripMenuItemClick(object sender, EventArgs e)
        {
            var input = new TextInputDialog();
            input.Text = "Execute Local Application";
            input.Label = "Full path to executable:";
            input.InputRequired = true;

            if (input.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var exePath = input.InputText;
            input.InputText = string.Empty;

            input.Label = "Command line arguments (optional):";
            input.InputRequired = false;

            if (input.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var item = new RunApplication { Name = exePath, Arguments = input.InputText };
            this.AddItem(item);
            this.runCommands.Items.Add(item);
        }

        private void ToolStripButtonDeleteClick(object sender, EventArgs e)
        {
            var selected = this.listView.SelectedItems;
            var selectedItems = new ListViewItem[selected.Count];
            selected.CopyTo(selectedItems, 0);

            foreach (var selectedItem in selectedItems)
            {
                this.listView.Items.Remove(selectedItem);
            }
        }

        private void ToolStripButtonUpClick(object sender, EventArgs e)
        {
            this.Swap(-1);
        }

        private void ToolStripButtonDownClick(object sender, EventArgs e)
        {
            this.Swap(1);
        }

        private void ListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateButtons();
        }
    }
}
