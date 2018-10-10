// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFolderStructureControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateFolderStructureControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Motion.Update.UsbUpdateManager.Utility;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Control that shows the folder structure of an update.
    /// </summary>
    public partial class UpdateFolderStructureControl : UserControl
    {
        private readonly IProjectManager projectManager;

        private UpdateCommand updateCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFolderStructureControl"/> class.
        /// </summary>
        public UpdateFolderStructureControl()
        {
            this.InitializeComponent();

            try
            {
                this.projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Gorba.Common.Update.ServiceModel.Messages.UpdateCommand"/> to be shown.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateCommand UpdateCommand
        {
            get
            {
                return this.updateCommand;
            }

            set
            {
                this.updateCommand = value;

                this.LoadData();
            }
        }

        private void LoadData()
        {
            var model = new FoldersTreeModel();
            if (this.UpdateCommand != null)
            {
                this.LoadFolderTree(this.UpdateCommand.Folders, model.Root);
            }

            this.foldersTreeControl.TreeModel = model;
            this.LoadSelectedFolder();
        }

        private void LoadFolderTree(IEnumerable<FolderUpdate> folderUpdates, FoldersTreeModel.Folder folder)
        {
            foreach (var folderUpdate in folderUpdates)
            {
                var child = new FoldersTreeModel.Folder(folderUpdate.Name, folderUpdate);
                folder.Children.Add(child);

                var subFolders = folderUpdate.Items.FindAll(i => i is FolderUpdate).ConvertAll(i => (FolderUpdate)i);
                this.LoadFolderTree(subFolders, child);
            }
        }

        private void LoadSelectedFolder()
        {
            this.fileExplorerListView.Items.Clear();

            var selected = this.foldersTreeControl.SelectedFolder;
            if (selected == null)
            {
                return;
            }

            var folderUpdate = selected.Tag as FolderUpdate;
            if (folderUpdate == null)
            {
                return;
            }

            foreach (var updateItem in folderUpdate.Items)
            {
                var item = new ListViewItem(updateItem.Name) { Tag = updateItem };
                var file = updateItem as FileUpdate;
                if (file == null)
                {
                    item.ImageIndex = this.fileExplorerListView.IconManager.AddFolderIcon(Environment.CurrentDirectory);
                }
                else
                {
                    var resource = this.projectManager.CurrentResourceService.GetResource(new ResourceId(file.Hash));
                    item.SubItems.Add(FileUtility.GetFileSizeString(resource.Size));
                    item.SubItems.Add(file.Hash);
                    item.ImageIndex = this.fileExplorerListView.IconManager.AddFileIcon(Path.GetExtension(file.Name));
                }

                this.fileExplorerListView.Items.Add(item);
            }
        }

        private void FoldersTreeControlSelectedFolderChanged(object sender, EventArgs e)
        {
            this.LoadSelectedFolder();
        }

        private void FileExplorerListViewItemActivate(object sender, EventArgs e)
        {
            if (this.fileExplorerListView.SelectedItems.Count != 1)
            {
                return;
            }

            var selectedItem = this.fileExplorerListView.SelectedItems[0];
            var file = selectedItem.Tag as FileUpdate;

            if (file == null)
            {
                this.foldersTreeControl.SelectedFolder =
                    this.foldersTreeControl.TreeModel.Root.Find(f => f.Tag == selectedItem.Tag);
                return;
            }

            var tempFile = Path.GetTempFileName();
            var resourceId = new ResourceId(file.Hash);
            var resource = this.projectManager.CurrentResourceService.GetResource(resourceId);
            this.projectManager.CurrentResourceService.ExportResource(resource, tempFile);

            File.SetAttributes(tempFile, FileAttributes.ReadOnly);

            var process = new Process();
            process.StartInfo = new ProcessStartInfo("Notepad.exe", tempFile);
            process.EnableRaisingEvents = true;
            process.Exited += (o, args) =>
            {
                File.SetAttributes(tempFile, FileAttributes.Normal);
                File.Delete(tempFile);
            };

            process.Start();
        }
    }
}
