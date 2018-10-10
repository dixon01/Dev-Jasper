// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCreationControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCreationControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Media;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.Ftp;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Update.Usb;
    using Gorba.Motion.Update.UsbUpdateManager.Data;
    using Gorba.Motion.Update.UsbUpdateManager.Properties;
    using Gorba.Motion.Update.UsbUpdateManager.Utility;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The update creation control.
    /// </summary>
    public partial class UpdateCreationControl : UserControl
    {
        private const StringComparison DefaultStringComparison = StringComparison.CurrentCultureIgnoreCase;

        private const string DeliveryRoot = @"R:\Softwareserver_Delivery\Gorba\SW\02_imotion\02_TFT\";

        private static readonly Dictionary<string, string> DefaultSourceDirectories = new Dictionary<string, string>
            {
                {
                    @"\Progs\HardwareManager",
                    DeliveryRoot + @"13_HardwareManager\TfsBuild\Motion_HardwareManager"
                },
                {
                    @"\Progs\SystemManager",
                    DeliveryRoot + @"12_SystemManager\TfsBuild\Motion_SystemManager"
                },
                {
                    @"\Progs\Update",
                    DeliveryRoot + @"14_Update\TfsBuild\Motion_Update"
                },
                {
                    @"\Progs\Protran",
                    DeliveryRoot + @"09_ProtocolTranslator\TfsBuild\Motion_Protran"
                },
                {
                    @"\Progs\Composer",
                    DeliveryRoot + @"06_imotion Media Player\TfsBuild\Motion_Infomedia"
                },
                {
                    @"\Progs\Renderer",
                    DeliveryRoot + @"06_imotion Media Player\TfsBuild\Motion_Infomedia"
                },
                {
                    @"\Progs\DirectXRenderer",
                    DeliveryRoot + @"06_imotion Media Player\TfsBuild\Motion_Infomedia"
                },
                {
                    @"\Progs\AhdlcRenderer",
                    DeliveryRoot + @"06_imotion Media Player\TfsBuild\Motion_Infomedia"
                },
                {
                    @"\Progs\AudioRenderer",
                    DeliveryRoot + @"06_imotion Media Player\TfsBuild\Motion_Infomedia"
                },
                {
                    @"\Progs\Infomedia",
                    DeliveryRoot + @"06_imotion Media Player\TfsBuild\Motion_Infomedia"
                },
            };

        private readonly BindingList<UnitGroup> unitGroupsModel = new BindingList<UnitGroup>();

        private readonly IProjectManager projectManager;

        private UnitGroup selectedUnitGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCreationControl"/> class.
        /// </summary>
        public UpdateCreationControl()
        {
            this.InitializeComponent();

            this.toolStripButtonReplaceFiles.Visible = Settings.Default.DeveloperMode;
            this.toolStripSeparatorDeveloperMode.Visible = Settings.Default.DeveloperMode;
            this.toolStripButtonEditFile.Visible = Settings.Default.DeveloperMode;

            this.comboBoxUnitGroup.DataSource = this.unitGroupsModel;
            this.comboBoxUnitGroup.DisplayMember = "Name";

            this.largeIconToolStripMenuItem.Tag = View.LargeIcon;
            this.smallIconToolStripMenuItem.Tag = View.SmallIcon;
            this.listToolStripMenuItem.Tag = View.List;
            this.detailToolStripMenuItem.Tag = View.Details;
            this.tilesToolStripMenuItem.Tag = View.Tile;

            this.SetView(View.Details);

            try
            {
                this.projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
                this.projectManager.CurrentProjectChanged += this.ProjectManagerOnCurrentProjectChanged;
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Gets or sets the selected Unit Group.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UnitGroup SelectedUnitGroup
        {
            get
            {
                return this.comboBoxUnitGroup.SelectedItem as UnitGroup;
            }

            set
            {
                this.comboBoxUnitGroup.SelectedItem = value;
                this.LoadUnitGroup(value);
            }
        }

        private void LoadUnitGroup(UnitGroup unitGroup)
        {
            if (this.selectedUnitGroup == unitGroup)
            {
                return;
            }

            this.selectedUnitGroup = this.SelectedUnitGroup;
            if (this.selectedUnitGroup == null)
            {
                this.splitContainer.Visible = false;
                this.foldersTreeControl.TreeModel = null;
                this.fileExplorerListView.Items.Clear();
                return;
            }

            var foldersTreeModel = new FoldersTreeModel();
            this.PopulateTreeModel(this.selectedUnitGroup.CurrentDirectoryStructure, foldersTreeModel.Root);
            this.foldersTreeControl.TreeModel = foldersTreeModel;
            this.splitContainer.Visible = true;
        }

        private void SetView(View view)
        {
            foreach (ToolStripMenuItem item in this.viewToolStripDropDownButton.DropDownItems)
            {
                item.Checked = object.Equals(view, item.Tag);
            }

            this.fileExplorerListView.View = view;
        }

        private void PopulateTreeModel(DirectoryNode parentDir, FoldersTreeModel.Folder parentFolder)
        {
            foreach (var directory in parentDir.Directories)
            {
                var folder = this.CreateFolder(directory);
                parentFolder.Children.Add(folder);
                this.PopulateTreeModel(directory, folder);
            }
        }

        private void PopulateFileListView()
        {
            this.fileExplorerListView.Items.Clear();
            this.toolStripButtonRemoveFiles.Enabled = false;
            this.toolStripButtonEditFile.Enabled = false;

            if (this.foldersTreeControl.SelectedFolder == null)
            {
                return;
            }

            var directory = this.foldersTreeControl.SelectedFolder.Tag as DirectoryNode;
            if (directory == null)
            {
                return;
            }

            foreach (var dir in directory.Directories)
            {
                this.AddFileListViewItem(dir);
            }

            foreach (var file in directory.Files)
            {
                this.AddFileListViewItem(file);
            }
        }

        private void AddFileListViewItem(FileNode file)
        {
            var resource = this.projectManager.CurrentResourceService.GetResource(file.ResourceId);
            var size = FileUtility.GetFileSizeString(resource.Size);
            var item = new ListViewItem(new[] { file.Name, size, file.ResourceId.ToString() })
            {
                ImageIndex = this.fileExplorerListView.IconManager.AddFileIcon(Path.GetExtension(file.Name)),
                Tag = file
            };

            this.fileExplorerListView.Items.Add(item);
        }

        private void AddFileListViewItem(DirectoryNode directory)
        {
            var item = new ListViewItem(directory.Name)
            {
                ImageIndex = this.fileExplorerListView.IconManager.AddFolderIcon(Environment.CurrentDirectory),
                Tag = directory
            };

            this.fileExplorerListView.Items.Add(item);
        }

        private FoldersTreeModel.Folder CreateFolder(DirectoryNode directory)
        {
            return new FoldersTreeModel.Folder(directory.Name, directory);
        }

        private void AddFolder(DirectoryNode parentDir, FoldersTreeModel.Folder parent, string name)
        {
            if (parentDir.Directories.Find(d => d.Name.Equals(name, DefaultStringComparison)) != null)
            {
                MessageBox.Show(
                    this,
                    "A folder with the same name already exists",
                    "New Folder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var directory = new DirectoryNode { Name = name };
            parentDir.Directories.Add(directory);
            var folder = this.CreateFolder(directory);
            parent.Children.Add(folder);
            this.projectManager.Save();

            this.PopulateFileListView();
        }

        private void DeleteFolder(FoldersTreeModel.Folder folder, DirectoryNode directory)
        {
            if (folder.Parent == null)
            {
                return;
            }

            var parent = folder.Parent.Tag as DirectoryNode;
            if (parent == null)
            {
                return;
            }

            if (MessageBox.Show(
                this,
                string.Format("Do you really want to remove the folder \"{0}\"?", folder.Path),
                "Remove Unit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            this.DeleteFolder(folder, directory, parent);
        }

        private void DeleteFolder(FoldersTreeModel.Folder folder, DirectoryNode directory, DirectoryNode parent)
        {
            folder.Parent.Children.Remove(folder);
            parent.Directories.Remove(directory);
            this.projectManager.Save();
        }

        private void AddFiles(DirectoryNode parent, params string[] fileNames)
        {
            var allFiles = new List<string>();
            var toDelete = new List<ListViewItem>();
            foreach (var file in fileNames)
            {
                var filePath = file;
                var fileName = Path.GetFileName(file);

                if (parent.Files.Find(f => f.Name.Equals(fileName, DefaultStringComparison)) != null)
                {
                    var result = MessageBox.Show(
                        this,
                        string.Format("The file '{0}' already exists, do you want to overwrite it?", fileName),
                        "Add Files",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation);
                    switch (result)
                    {
                        case DialogResult.Cancel:
                            // don't do anything
                            return;
                        case DialogResult.No:
                            // don't add the current file
                            continue;
                    }

                    foreach (ListViewItem item in this.fileExplorerListView.Items)
                    {
                        if (item.Text != null && item.Text.Equals(fileName, DefaultStringComparison))
                        {
                            toDelete.Add(item);
                            break;
                        }
                    }
                }

                allFiles.Add(filePath);
            }

            if (allFiles.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "No files added",
                    "Add Files",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            foreach (var item in toDelete)
            {
                this.DeleteFile(parent, item);
            }

            var progress = new ProgressDialog
            {
                Text = "Add Files",
                Label = string.Format("Adding {0} files...", allFiles.Count),
                Task = new AddFilesTask(allFiles, parent, this)
            };
            if (progress.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show(
                    this,
                    string.Format("Sucessfully added {0} files", allFiles.Count),
                    "Add Files",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    this,
                    "User cancelled adding files",
                    "Add Files",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void AddFile(DirectoryNode directory, string fileName, ResourceId id)
        {
            // IMPORTANT: this method doesn't save the project file, you need to do that afterwards yourself
            var file = new FileNode { Name = fileName, ResourceId = id };
            directory.Files.Add(file);
            this.AddFileListViewItem(file);
        }

        private void DeleteFiles(DirectoryNode directory, List<ListViewItem> items)
        {
            if (MessageBox.Show(
                this,
                string.Format("Do you really want to remove the selected {0} files/folders?", items.Count),
                "Remove File",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            foreach (var item in items)
            {
                var subDir = item.Tag as DirectoryNode;
                if (subDir != null)
                {
                    var folder = this.foldersTreeControl.TreeModel.Root.Find(f => f.Tag == subDir);
                    if (folder == null || folder.Parent == null)
                    {
                        continue;
                    }

                    this.DeleteFolder(folder, subDir, (DirectoryNode)folder.Parent.Tag);
                    this.fileExplorerListView.Items.Remove(item);
                }
                else
                {
                    this.DeleteFile(directory, item);
                }
            }

            this.projectManager.Save();
        }

        private void DeleteFile(DirectoryNode directory, ListViewItem item)
        {
            var file = item.Tag as FileNode;
            if (file == null)
            {
                return;
            }

            directory.Files.Remove(file);
            this.fileExplorerListView.Items.Remove(item);
        }

        private void ExportAllToUsb(DirectoryInfo root)
        {
            if (!root.Exists)
            {
                root.Create();
            }

            var preview = this.projectManager.CreateExportPreview();
            var exportDialog = new UpdateExportDialog { ExportPreview = preview };
            if (exportDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var updateProvider = new UsbUpdateProvider();
            updateProvider.Configure(
                new UsbUpdateProviderConfig { RepositoryBasePath = root.FullName },
                new GuiUpdateContext(this.projectManager.CurrentResourceService));
            var repoConfig = updateProvider.GetRepositoryConfig().GetCurrentConfig();

            var export = exportDialog.CreateExport();
            if (export == null)
            {
                return;
            }

            var newResources = new List<ResourceInfo>(export.Resources);

            var resourceDir = root.CreateSubdirectory(repoConfig.ResourceDirectory);

            // remove all resources that are already there
            foreach (var file in resourceDir.GetFiles())
            {
                var resourceId = Path.GetFileNameWithoutExtension(file.Name);
                newResources.RemoveAll(r => r.Id.Hash.Equals(resourceId, StringComparison.InvariantCultureIgnoreCase));
            }

            long totalSize = 0;
            newResources.ForEach(r => totalSize += r.Size);

            totalSize += export.Commands.Count * 20000; // an update command is max 20kB

            var availableSize = new DriveInfo(root.Root.FullName).AvailableFreeSpace;

            if (availableSize < totalSize)
            {
                var msg =
                    string.Format(
                        "The expected size of the update (~{0:0.00}MB) exceeds the available disk space ({1:0.00}MB)",
                        totalSize / FileUtility.MegaBytes,
                        availableSize / FileUtility.MegaBytes);
                MessageBox.Show(this, msg, "Export Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var monitor = new ExportUpdateProgress(export.Commands.Count, this);
            ThreadPool.QueueUserWorkItem(s => updateProvider.HandleCommands(export.Commands, monitor));
        }

        private void ExportAllToFtp(FtpUpdateProviderConfig ftpConfig)
        {
            var preview = this.projectManager.CreateExportPreview();
            var exportDialog = new UpdateExportDialog { ExportPreview = preview };
            if (exportDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var updateProvider = new FtpUpdateProvider();
            updateProvider.Configure(
                ftpConfig,
                new GuiUpdateContext(this.projectManager.CurrentResourceService));

            var export = exportDialog.CreateExport();
            if (export == null)
            {
                return;
            }

            var monitor = new ExportUpdateProgress(export.Commands.Count, this);
            ThreadPool.QueueUserWorkItem(s => updateProvider.HandleCommands(export.Commands, monitor));
        }

        private void ProjectManagerOnCurrentProjectChanged(object sender, EventArgs eventArgs)
        {
            this.SelectedUnitGroup = null;
            this.buttonExport.Enabled = !this.projectManager.CurrentProjectReadOnly;
        }

        private void ComboBoxUnitGroupDropDown(object sender, EventArgs e)
        {
            var selected = this.SelectedUnitGroup;
            this.unitGroupsModel.Clear();
            var unitGroups =
                new List<UnitGroup>(ServiceLocator.Current.GetInstance<IProjectManager>().CurrentProject.UnitGroups);
            unitGroups.Sort((a, b) => string.Compare(a.Name, b.Name, DefaultStringComparison));
            foreach (var unitGroup in unitGroups)
            {
                this.unitGroupsModel.Add(unitGroup);
            }

            this.SelectedUnitGroup = selected;
        }

        private void ComboBoxUnitGroupDropDownClosed(object sender, EventArgs e)
        {
            this.LoadUnitGroup(this.SelectedUnitGroup);
        }

        private void FoldersTreeControlSelectedFolderChanged(object sender, EventArgs e)
        {
            this.toolStripButtonAddFolder.Enabled = this.foldersTreeControl.SelectedFolder != null
                                                    && !this.projectManager.CurrentProjectReadOnly;
            this.toolStripButtonRemoveFolder.Enabled =
                this.foldersTreeControl.SelectedFolder != null
                && !this.projectManager.CurrentProjectReadOnly
                && ((DirectoryNode)this.foldersTreeControl.SelectedFolder.Tag).Modifiable;

            this.toolStripButtonAddFiles.Enabled = this.foldersTreeControl.SelectedFolder != null
                                                   && !this.projectManager.CurrentProjectReadOnly;
            this.toolStripButtonReplaceFiles.Enabled = this.foldersTreeControl.SelectedFolder != null
                                                       && !this.projectManager.CurrentProjectReadOnly;

            this.PopulateFileListView();
        }

        private void ToolStripButtonAddFolderClick(object sender, EventArgs e)
        {
            var parent = this.foldersTreeControl.SelectedFolder;
            if (parent == null)
            {
                return;
            }

            var parentDir = parent.Tag as DirectoryNode;
            if (parentDir == null)
            {
                return;
            }

            var input = new TextInputDialog
            {
                Label = "Please provide a name for the folder:",
                Text = "New Folder",
                InputRequired = true
            };

            if (input.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.AddFolder(parentDir, parent, input.InputText);
        }

        private void ToolStripButtonRemoveFolderClick(object sender, EventArgs e)
        {
            var folder = this.foldersTreeControl.SelectedFolder;
            if (folder == null)
            {
                return;
            }

            var directory = folder.Tag as DirectoryNode;
            if (directory == null || !directory.Modifiable)
            {
                return;
            }

            this.DeleteFolder(folder, directory);
        }

        private void ToolStripButtonAddFilesClick(object sender, EventArgs e)
        {
            if (this.foldersTreeControl.SelectedFolder == null)
            {
                return;
            }

            var directory = this.foldersTreeControl.SelectedFolder.Tag as DirectoryNode;
            if (directory == null)
            {
                return;
            }

            if (this.addFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.AddFiles(directory, this.addFileDialog.FileNames);
        }

        private void ToolStripButtonRemoveFilesClick(object sender, EventArgs e)
        {
            var folder = this.foldersTreeControl.SelectedFolder;
            if (folder == null)
            {
                return;
            }

            var directory = folder.Tag as DirectoryNode;
            if (directory == null)
            {
                return;
            }

            var items = new List<ListViewItem>();
            foreach (ListViewItem item in this.fileExplorerListView.SelectedItems)
            {
                items.Add(item);
            }

            this.DeleteFiles(directory, items);
        }

        private void ToolStripButtonReplaceFilesClick(object sender, EventArgs e)
        {
            var selectedFolder = this.foldersTreeControl.SelectedFolder;
            if (selectedFolder == null)
            {
                return;
            }

            var directory = selectedFolder.Tag as DirectoryNode;
            if (directory == null)
            {
                return;
            }

            string sourceDir;
            if (DefaultSourceDirectories.TryGetValue(selectedFolder.Path, out sourceDir))
            {
                var dir = Path.GetDirectoryName(sourceDir);
                Debug.Assert(dir != null, "dir != null");
                var branches = Directory.GetDirectories(dir, Path.GetFileName(sourceDir) + "*");
                if (branches.Length > 1)
                {
                    var selection = new SelectionDialog { Text = "Replace Files", Label = "Select the branch to use:" };
                    foreach (var branch in branches)
                    {
                        dir = Path.GetFileName(branch);
                        Debug.Assert(dir != null, "dir != null");
                        selection.Items.Add(dir);
                    }

                    if (selection.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    sourceDir = branches[selection.SelectedIndex];
                }

                var sourceFolders = Directory.GetDirectories(sourceDir);
                if (sourceFolders.Length == 0)
                {
                    this.folderBrowserDialog.SelectedPath = sourceDir;
                }
                else
                {
                    Array.Sort(sourceFolders);
                    this.folderBrowserDialog.SelectedPath = sourceFolders[sourceFolders.Length - 1];
                }
            }

            this.folderBrowserDialog.Description =
                "Please select a folder from which to take the files to replace the existing files:"
                + "\r\n(Only files that already exist are replaced, other files are ignored)";
            if (this.folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var files = new List<string>();

            foreach (var file in Directory.GetFiles(this.folderBrowserDialog.SelectedPath))
            {
                var fileName = Path.GetFileName(file);
                if (directory.Files.Find(f => f.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                    != null)
                {
                    files.Add(file);
                }
            }

            this.AddFiles(directory, files.ToArray());
        }

        private void ToolStripButtonEditFileClick(object sender, EventArgs e)
        {
            if (this.fileExplorerListView.SelectedItems.Count != 1)
            {
                return;
            }

            var selectedItem = this.fileExplorerListView.SelectedItems[0];
            var file = selectedItem.Tag as FileNode;
            if (file == null)
            {
                return;
            }

            var directory = this.foldersTreeControl.SelectedFolder.Tag as DirectoryNode;
            if (directory == null)
            {
                return;
            }

            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var tempFile = Path.Combine(tempDir, file.Name);
            var resource = this.projectManager.CurrentResourceService.GetResource(file.ResourceId);
            this.projectManager.CurrentResourceService.ExportResource(resource, tempFile);

            var process = new Process();
            process.StartInfo = new ProcessStartInfo("Notepad.exe", tempFile);
            process.EnableRaisingEvents = true;
            process.Exited += (o, args) =>
                {
                    var hash = ResourceHash.Create(tempFile);
                    if (hash != file.ResourceId.Hash)
                    {
                        this.Invoke(new MethodInvoker(() => this.AddFiles(directory, tempFile)));
                    }

                    Directory.Delete(tempDir, true);
                };

            process.Start();
        }

        private void ViewToolStripMenuItemClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null || !(menuItem.Tag is View))
            {
                return;
            }

            this.SetView((View)menuItem.Tag);
        }

        private void FileExplorerListViewItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.toolStripButtonRemoveFiles.Enabled = this.fileExplorerListView.SelectedItems.Count > 0
                                                      && !this.projectManager.CurrentProjectReadOnly;
            this.toolStripButtonEditFile.Enabled = this.fileExplorerListView.SelectedItems.Count == 1
                                                   && this.fileExplorerListView.SelectedItems[0].Tag is FileNode
                                                   && !this.projectManager.CurrentProjectReadOnly;
        }

        private void FileExplorerListViewItemActivate(object sender, EventArgs e)
        {
            if (this.fileExplorerListView.SelectedItems.Count != 1)
            {
                SystemSounds.Exclamation.Play();
                return;
            }

            var selectedItem = this.fileExplorerListView.SelectedItems[0];
            var file = selectedItem.Tag as FileNode;
            if (file == null)
            {
                this.foldersTreeControl.SelectedFolder =
                    this.foldersTreeControl.TreeModel.Root.Find(f => f.Tag == selectedItem.Tag);
                return;
            }

            var tempFile = Path.GetTempFileName();
            var resource = this.projectManager.CurrentResourceService.GetResource(file.ResourceId);
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

        private void ButtonExportClick(object sender, EventArgs e)
        {
            this.contextMenuStripExport.Items.Clear();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType != DriveType.Removable || !drive.IsReady)
                {
                    continue;
                }

                var item =
                    this.contextMenuStripExport.Items.Add(
                        string.Format("{0} ({1})", drive.VolumeLabel, drive.Name.TrimEnd('\\')));
                item.Tag = drive.RootDirectory;
                item.AutoToolTip = true;
                item.ToolTipText = string.Format("{0:0.0} MB free", drive.AvailableFreeSpace / FileUtility.MegaBytes);
                item.Image = FileIconManager.CreateFolderIcon(drive.RootDirectory.FullName, true).ToBitmap();
                item.Click += this.ExportItemOnClick;
            }

            if (this.contextMenuStripExport.Items.Count == 0)
            {
                this.contextMenuStripExport.Items.Add("No USB stick found").Enabled = false;
            }

            this.contextMenuStripExport.Items.Add(new ToolStripSeparator());
            foreach (var ftpServer in this.projectManager.CurrentProject.FtpServers)
            {
                var item =
                    this.contextMenuStripExport.Items.Add(
                        string.Format("{0}@{1}", ftpServer.Username, ftpServer.Host), Resources.FtpServer);
                item.Tag = ftpServer;
                item.Click += this.ExportItemOnClick;
            }

            this.contextMenuStripExport.Items.Add("Manage FTP Servers...").Click += this.AddFtpServerOnClick;
            this.contextMenuStripExport.Items.Add(new ToolStripSeparator());

            var parent = new ToolStripMenuItem("Create Folders Locally", Resources.repfld);
            this.contextMenuStripExport.Items.Add(parent);
            foreach (var unitGroup in this.projectManager.CurrentProject.UnitGroups)
            {
                var item = parent.DropDownItems.Add(unitGroup.Name + "...", Resources.UnitGroup);
                item.Tag = unitGroup;
                item.Click += this.CreateFoldersLocallyOnClick;
            }

            this.contextMenuStripExport.Show(this.buttonExport, 0, this.buttonExport.Height);
        }

        private void AddFtpServerOnClick(object sender, EventArgs e)
        {
            var ftpServersDialog = new FtpServersDialog();
            ftpServersDialog.ShowDialog(this);
        }

        private void ExportItemOnClick(object sender, EventArgs eventArgs)
        {
            var item = sender as ToolStripItem;
            if (item == null)
            {
                return;
            }

            try
            {
                var usbRoot = item.Tag as DirectoryInfo;
                if (usbRoot != null)
                {
                    this.ExportAllToUsb(usbRoot.CreateSubdirectory("Gorba").CreateSubdirectory("Update"));
                    return;
                }

                var ftpConfig = item.Tag as FtpUpdateProviderConfig;
                if (ftpConfig != null)
                {
                    this.ExportAllToFtp(ftpConfig);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    string.Format("Couldn't export update:\r\n{0}", ex),
                    "Export Update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CreateFoldersLocallyOnClick(object sender, EventArgs eventArgs)
        {
            var item = sender as ToolStripItem;
            if (item == null)
            {
                return;
            }

            var unitGroup = item.Tag as UnitGroup;
            if (unitGroup == null)
            {
                return;
            }

            this.folderBrowserDialog.Description =
                "Please select an empty folder where the structure should be created:";
            if (this.folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var path = this.folderBrowserDialog.SelectedPath;
            if (Directory.GetFileSystemEntries(path).Length > 0)
            {
                var msg = "Do you really want to create the structure in '" + path
                          + "'?\r\nThe directory is not empty, all existing files and folders will be deleted.";
                if (MessageBox.Show(
                    this,
                    msg,
                    "Create Folders Locally",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }
            }

            var progress = new ProgressDialog
            {
                Text = "Create Folders Locally",
                Label = "Creating local folder structure in " + path,
                Task = new CreateFoldersLocallyTask(unitGroup, path, this)
            };
            if (progress.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show(
                    this,
                    "Sucessfully created local folder structure in " + path,
                    "Create Folders Locally",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    this,
                    "User cancelled creating local folder structure in " + path,
                    "Create Folders Locally",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private class CreateFoldersLocallyTask : ProgressTaskBase
        {
            private readonly UnitGroup unitGroup;

            private readonly string path;

            private readonly UpdateCreationControl parent;

            public CreateFoldersLocallyTask(UnitGroup unitGroup, string path, UpdateCreationControl parent)
            {
                this.unitGroup = unitGroup;
                this.path = path;
                this.parent = parent;
            }

            private delegate void NodeAction(FileNode file, string filePath);

            public override void Run()
            {
                this.Value = 0;
                this.State = "Deleting " + this.path;
                this.DeleteLocalFolder(this.path);

                var fileCount = 0;
                this.TraverseFolders(this.unitGroup.CurrentDirectoryStructure, this.path, (n, p) => fileCount++);

                if (this.IsCancelled)
                {
                    return;
                }

                var maxProgress = fileCount + 1.0;

                var progress = 0;

                try
                {
                    this.TraverseFolders(
                        this.unitGroup.CurrentDirectoryStructure,
                        this.path,
                        (file, filePath) =>
                            {
                                if (this.IsCancelled)
                                {
                                    throw new InvalidDataException();
                                }

                                this.Value = (++progress) / maxProgress;
                                this.State = string.Format("Exporting {0}", file.Name);
                                var resource =
                                    this.parent.projectManager.CurrentResourceService.GetResource(file.ResourceId);
                                this.parent.projectManager.CurrentResourceService.ExportResource(resource, filePath);
                            });
                }
                catch (InvalidDataException)
                {
                    // that's just the signal that we were cancelled
                }
                catch (Exception ex)
                {
                    this.parent.Invoke(
                        new MethodInvoker(
                            () =>
                            MessageBox.Show(
                                this.parent,
                                string.Format("Couldn't create folders locally:\r\n{0}", ex),
                                "Create Folders Locally",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)));
                }
            }

            private void DeleteLocalFolder(string directory)
            {
                foreach (var subDir in Directory.GetDirectories(directory))
                {
                    this.DeleteLocalFolder(subDir);
                }

                foreach (var file in Directory.GetFiles(directory))
                {
                    File.Delete(file);
                }

                Directory.Delete(directory);
            }

            private void TraverseFolders(DirectoryNode directory, string dirPath, NodeAction action)
            {
                Directory.CreateDirectory(dirPath);
                foreach (var subDirectory in directory.Directories)
                {
                    this.TraverseFolders(subDirectory, Path.Combine(dirPath, subDirectory.Name), action);
                }

                foreach (var file in directory.Files)
                {
                    var filePath = Path.Combine(dirPath, file.Name);
                    action(file, filePath);
                }
            }
        }

        private class AddFilesTask : ProgressTaskBase
        {
            private readonly List<string> fileNames;

            private readonly DirectoryNode directory;

            private readonly UpdateCreationControl parent;

            public AddFilesTask(List<string> fileNames, DirectoryNode directory, UpdateCreationControl parent)
            {
                this.fileNames = fileNames;
                this.directory = directory;
                this.parent = parent;
            }

            public override void Run()
            {
                var maxProgress = this.fileNames.Count + 1.0;
                var resourceService = this.parent.projectManager.CurrentResourceService;

                for (int i = 0; i < this.fileNames.Count && !this.IsCancelled; i++)
                {
                    var filePath = this.fileNames[i];
                    var fileName = Path.GetFileName(filePath);
                    this.Value = (i + 1) / maxProgress;
                    this.State = string.Format(
                        "{0} ({1}/{2})", Path.GetFileName(fileName), i + 1, this.fileNames.Count);
                    var id = resourceService.RegisterResource(filePath, false);
                    this.Invoke(() => this.parent.AddFile(this.directory, fileName, id));
                }

                this.parent.projectManager.Save();
            }

            private void Invoke(MethodInvoker method)
            {
                this.parent.Invoke(method);
            }
        }

        private class ExportUpdateProgress : IProgressMonitor
        {
            private readonly UpdateCreationControl parent;

            private readonly int unitCount;

            private ExportUpdateTask task;

            public ExportUpdateProgress(int unitCount, UpdateCreationControl parent)
            {
                this.unitCount = unitCount;
                this.parent = parent;
            }

            public bool IsCancelled { get; set; }

            public void Start()
            {
                this.task = new ExportUpdateTask(this);
                this.parent.BeginInvoke(new ThreadStart(this.ShowDialog));
            }

            public void Progress(double value, string note)
            {
                if (this.task != null)
                {
                    this.task.Progress(value, note);
                }
            }

            public IPartProgressMonitor CreatePart(double startValue, double endValue)
            {
                return new PartProgressMonitor(this, startValue, endValue);
            }

            public void Complete(string errorMessage, string successMessage)
            {
                if (this.task != null)
                {
                    this.task.Complete(errorMessage, successMessage);
                }
            }

            private void ShowDialog()
            {
                var progressDialog = new ProgressDialog
                {
                    Text = "Export Update",
                    Label = "Exporting update...",
                    Task = this.task
                };
                if (progressDialog.ShowDialog(this.parent) == DialogResult.OK)
                {
                    MessageBox.Show(
                        this.parent,
                        string.Format("Sucessfully exported update for {0} units", this.unitCount),
                        "Export Update",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        this.parent,
                        "User cancelled exporting update",
                        "Export Update",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
        }

        private class ExportUpdateTask : ProgressTaskBase
        {
            private readonly ExportUpdateProgress updateProgress;

            private readonly AutoResetEvent waitCompleted = new AutoResetEvent(false);

            private string errorMessage;

            public ExportUpdateTask(ExportUpdateProgress updateProgress)
            {
                this.updateProgress = updateProgress;
            }

            public override void Run()
            {
                this.waitCompleted.WaitOne();
                if (this.errorMessage != null)
                {
                    throw new UpdateException(this.errorMessage);
                }
            }

            public override void Cancel()
            {
                base.Cancel();
                this.updateProgress.IsCancelled = true;
            }

            public void Complete(string message, string successMessage)
            {
                this.errorMessage = message;
                this.waitCompleted.Set();
            }

            public void Progress(double value, string note)
            {
                this.State = note;
                this.Value = value;
            }
        }
    }
}
