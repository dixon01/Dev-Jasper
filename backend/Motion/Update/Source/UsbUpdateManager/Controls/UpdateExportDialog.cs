// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExportDialog.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateExportDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Update.UsbUpdateManager.Data;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Dialog that is shown before exporting an update.
    /// </summary>
    public partial class UpdateExportDialog : Form
    {
        private readonly IProjectManager projectManager;

        private UpdateExportPreview exportPreview;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateExportDialog"/> class.
        /// </summary>
        public UpdateExportDialog()
        {
            this.InitializeComponent();
            this.comboBoxUnitGroups.DisplayMember = "Name";
            this.ValidFromDateTime.Value = TimeProvider.Current.Now;

            try
            {
                this.projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Gets or sets the currently shown <see cref="UpdateExportPreview"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateExportPreview ExportPreview
        {
            get
            {
                return this.exportPreview;
            }

            set
            {
                if (this.exportPreview == value)
                {
                    return;
                }

                this.exportPreview = value;
                this.LoadData();
            }
        }

        /// <summary>
        /// Creates the export using the things changed in this dialog.
        /// Call this method after having shown the dialog.
        /// </summary>
        /// <returns>
        /// The <see cref="UpdateExport"/>.
        /// </returns>
        public UpdateExport CreateExport()
        {
            var progress = new ProgressDialog
            {
                Text = "Export Update",
                Label = "Adding pre and post installation actions...",
                Task = new AddFilesTask(this.ExportPreview, this)
            };

            if (progress.ShowDialog(this) != DialogResult.OK)
            {
                MessageBox.Show(
                    this,
                    "User cancelled adding actions",
                    "Export Update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return null;
            }

            var validFrom = this.ValidFromDateTime.Value;
            validFrom = DateTime.SpecifyKind(validFrom, DateTimeKind.Local);

            return this.projectManager.CreateExport(
                this.ExportPreview,
                this.textBoxName.Text,
                validFrom.ToUniversalTime(),
                this.checkBoxInstallAfterBoot.Checked);
        }

        private void LoadData()
        {
            this.comboBoxUnitGroups.Items.Clear();
            this.tabControl1.Visible = false;
            if (this.ExportPreview == null)
            {
                return;
            }

            foreach (var unitGroup in this.ExportPreview.UnitGroups)
            {
                this.comboBoxUnitGroups.Items.Add(unitGroup.UnitGroup);
            }
        }

        private void TextBoxNameTextChanged(object sender, EventArgs e)
        {
            this.buttonOk.Enabled = this.textBoxName.TextLength > 0;
        }

        private void ComboBoxUnitGroupsSelectedIndexChanged(object sender, EventArgs e)
        {
            var unitGroup = this.comboBoxUnitGroups.SelectedItem as UnitGroup;
            if (unitGroup == null)
            {
                this.tabControl1.Visible = false;
                return;
            }

            var preview = this.ExportPreview.UnitGroups.Find(p => p.UnitGroup == unitGroup);
            if (preview == null)
            {
                this.tabControl1.Visible = false;
                return;
            }

            var dummyCommand = new UpdateCommand();
            foreach (FolderUpdate item in preview.UpdateRoot.Items)
            {
                dummyCommand.Folders.Add(item);
            }

            this.preInstallationActionsControl.RunCommands = preview.PreInstallation;
            this.postInstallationActionsControl.RunCommands = preview.PostInstallation;

            this.updateFolderStructureControl.UpdateCommand = dummyCommand;

            this.tabControl1.Visible = true;
        }

        private class AddFilesTask : ProgressTaskBase
        {
            private readonly UpdateExportPreview preview;

            private readonly UpdateExportDialog parent;

            public AddFilesTask(UpdateExportPreview preview, UpdateExportDialog parent)
            {
                this.preview = preview;
                this.parent = parent;
            }

            public override void Run()
            {
                var maxProgress = 1.0;
                foreach (var unitGroup in this.preview.UnitGroups)
                {
                    if (unitGroup.PreInstallation != null)
                    {
                        maxProgress += unitGroup.PreInstallation.Items.Count;
                    }

                    if (unitGroup.PostInstallation != null)
                    {
                        maxProgress += unitGroup.PostInstallation.Items.Count;
                    }
                }

                var index = 0;
                foreach (var unitGroup in this.preview.UnitGroups)
                {
                    this.AddFiles(unitGroup.PreInstallation, maxProgress, ref index);
                    this.AddFiles(unitGroup.PostInstallation, maxProgress, ref index);
                }
            }

            private void AddFiles(RunCommands runCommands, double maxProgress, ref int index)
            {
                if (runCommands == null)
                {
                    return;
                }

                var resourceService = this.parent.projectManager.CurrentResourceService;

                foreach (var item in runCommands.Items)
                {
                    this.Value = (index + 1) / maxProgress;
                    this.State = Path.GetFileName(item.Name);

                    var file = item as FileUpdate;
                    if (file == null)
                    {
                        continue;
                    }

                    var id = resourceService.RegisterResource(file.Name, false);
                    file.Name = Path.GetFileName(file.Name);
                    file.Hash = id.Hash;

                    if (this.preview.Resources.Find(r => r.Id.Equals(id)) == null)
                    {
                        this.preview.Resources.Add(resourceService.GetResource(id));
                    }
                }
            }
        }
    }
}
