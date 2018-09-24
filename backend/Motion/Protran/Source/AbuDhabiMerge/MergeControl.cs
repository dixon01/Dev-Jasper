// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MergeControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabiMerge
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;

    using NLog;

    /// <summary>
    /// Merge Control for file merge for Cu5 and Protran files
    /// </summary>
    public partial class MergeControl : UserControl
    {
        /// <summary>
        /// Local temporary folder to merge the files
        /// </summary>
        public const string LocalTempFolder = @"/FilesToZip";

        /// <summary>
        /// List of files or folders to merge
        /// </summary>
        public readonly List<string> ListToMerge;

        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Indicates whether a folder was added previously
        /// </summary>
        private bool alreadyAdded;

        private string currentExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeControl"/> class.
        /// </summary>
        public MergeControl()
        {
            this.ListToMerge = new List<string>();
            this.InitializeComponent();
            this.SelectedFilesCheckedListBox.ItemCheck += this.AddRemoveFilesFromList;
            this.currentExtension = string.Empty;
        }

        /// <summary>
        /// Show Final Folder Structure Call Back
        /// </summary>
        /// <param name="localdir">
        /// The local directory.
        /// </param>
        public delegate void ShowFinalFolderStructureCallBack(string localdir);

        /// <summary>
        /// Gets the configuration information
        /// </summary>
        protected AbuDhabiMergeConfig Config { get; private set; }

        /// <summary>
        /// Gets or sets TotalFileCount.
        /// </summary>
        protected int TotalFileCount { get; set; }

        /// <summary>
        /// Gets or sets CurrentFileCount.
        /// </summary>
        protected int CurrentFileCount { get; set; }

        /// <summary>
        /// Gets or sets Index.
        /// </summary>
        protected int Index { get; set; }

        /// <summary>
        /// Gets or sets MergedFileName.
        /// </summary>
        protected string MergedFileName { get; set; }

        /// <summary>
        /// Configures this control
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public virtual void Configure(AbuDhabiMergeConfig config)
        {
            this.Config = config;

            if (this.Config.Extensions.TopboxData == string.Empty)
            {
                this.radioButtonData.Enabled = false;
                this.radioButtonSoftware.Checked = true;
            }

            if (this.Config.Extensions.TopboxSoftware == string.Empty)
            {
                this.radioButtonSoftware.Enabled = false;
                this.radioButtonData.Checked = true;
            }
        }

        /// <summary>
        /// Collect Files To Merge
        /// </summary>
        protected virtual void CollectFilesToMerge()
        {
            var folderName = this.SelectFoldersOpenFileDialog.SelectedPath;

            if ((!this.CheckIfFolderAlreadyAdded(folderName)) &&
                (!this.SelectedFilesCheckedListBox.Items.Contains(folderName)))
            {
                this.SelectedFilesCheckedListBox.DisplayMember = folderName;
                this.SelectedFilesCheckedListBox.Items.Add(folderName);
                this.SelectedFilesCheckedListBox.SetSelected(this.Index, true);
                this.SelectedFilesCheckedListBox.SetItemChecked(this.Index, true);
                this.Index++;
                this.ShowFilesInFolder(folderName);
            }
            else
            {
                MessageBox.Show(
                    this, "Already added.", "AbuDhabiMerge", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Process Files For Merge
        /// </summary>
        protected virtual void ProcessFilesForMerge()
        {
            this.ClearMergeStatus();

            if (this.ListToMerge.Count <= 0)
            {
                this.AppendLog("No folders selected for merge!");
                return;
            }

            var mergeThread = new Thread(this.MergeFiles);
            mergeThread.IsBackground = true;
            mergeThread.Start();
        }

        /// <summary>
        /// Clear Directory
        /// </summary>
        /// <param name="localDir">
        /// The local directory.
        /// </param>
        protected void ClearDirectory(string localDir)
        {
            // Delete the local directory after zip
            if (!Directory.Exists(localDir))
            {
                return;
            }

            try
            {
                Directory.Delete(localDir, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Error on deleting local directory: {0}", localDir);
            }
        }

        /// <summary>
        /// Process File Method
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void ProcessingFile(object sender, ScanEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ProcessFileHandler(this.ProcessingFile), sender, args);
                return;
            }

            this.ProgressBar.Maximum = this.TotalFileCount;
            this.CurrentFileCount++;
            this.ProgressBar.Increment(1);

            var fileName = args.Name;
            this.AppendLog("Merging file {0}: {1}", this.CurrentFileCount, fileName);
        }

        /// <summary>
        /// Clear Merge Status
        /// </summary>
        protected void ClearMergeStatus()
        {
            this.mergeStatusTextbox.Clear();
            this.ProgressBar.Value = 0;
            this.MergedFileTreeView.Nodes.Clear();
            this.MergePathTextbox.Clear();
        }

        /// <summary>
        /// Browse Button Click
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void BrowseButtonClick(object sender, EventArgs e)
        {
            if (this.SelectFoldersOpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.CollectFilesToMerge();
        }

        /// <summary>
        /// Get Extension
        /// </summary>
        /// <returns>
        /// the extension
        /// </returns>
        protected virtual string GetExtension()
        {
            var extension = string.Empty;

            if (this.radioButtonData.Checked)
            {
                extension = string.Format("Topbox Data(*{0})|*{0}", this.Config.Extensions.TopboxData);
                this.currentExtension = "Infomedia";
            }
            else if (this.radioButtonSoftware.Checked)
            {
                extension = string.Format("Topbox Software(*{0})|*{0}", this.Config.Extensions.TopboxSoftware);
                this.currentExtension = "Progs";
            }

            return extension;
        }

        /// <summary>
        /// Merge files
        /// </summary>
        protected virtual void MergeFiles()
        {
            var origCursor = this.Cursor;
            this.SetCursor(Cursors.WaitCursor);

            try
            {
                var localPath = Path.GetTempPath();
                var localDir = Path.Combine(localPath, LocalTempFolder);
                this.TotalFileCount = 0;
                this.ClearDirectory(localDir);

                // Create a local directory to move the files to be zipped
                if (!Directory.Exists(localDir))
                {
                    Directory.CreateDirectory(localDir);
                }

                // Move all the files in the selected list to the new directory
                foreach (var folder in this.ListToMerge)
                {
                    this.VerifyFolderStructure(folder, localDir);
                }

                // Zip all the files in the local directory and store the zipped file at the zipPath specified
                // this.totalFileCount = this.listToMerge.Count;
                this.CurrentFileCount = 0;

                // Show the final tree view of destination folder
                this.ShowFinalFolderStructure(localDir);

                var events = new FastZipEvents { ProcessFile = this.ProcessingFile };

                var zip = new FastZip(events);

                // for the empty folders, I must specify the property "CreateEmptyDirectories"
                // otherwise the zip library will not be able to create them in the zip archive.
                zip.CreateEmptyDirectories = true;
                zip.CreateZip(this.MergedFileName, localDir, true, string.Empty);

                this.ClearDirectory(localDir);

                this.AppendLog("Selected folders have been merged.");
                this.AppendLog("Merged file is located at:");
                this.AppendLog(this.MergedFileName);
            }
            catch (Exception ex)
            {
                this.AppendLog("Merge for selected folders failed!");
                this.AppendLog(ex.Message);
            }
            finally
            {
                this.SetCursor(origCursor);
            }
        }

        /// <summary>
        /// Appends one line of logging to the status textbox.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void AppendLog(string format, params object[] args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.AppendLog(format, args)));
                return;
            }

            this.mergeStatusTextbox.AppendText(string.Format(format, args) + Environment.NewLine);
        }

        /// <summary>
        /// Sets the cursor thread safe.
        /// </summary>
        /// <param name="cursor">
        /// The cursor.
        /// </param>
        protected void SetCursor(Cursor cursor)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.SetCursor(cursor)));
                return;
            }

            this.Cursor = cursor;
        }

        private bool CheckIfFolderAlreadyAdded(string folderName)
        {
            var directories = new DirectoryInfo(folderName);
            var topNode = this.FilesInFolderTreeView.TopNode;
            if (topNode != null)
            {
                var nodes = this.FilesInFolderTreeView.Nodes;
                foreach (TreeNode node in nodes)
                {
                    this.NodeExists(node, directories.Name);
                }
            }

            return this.alreadyAdded;
        }

        private void NodeExists(TreeNode nodes, string key)
        {
            foreach (TreeNode tn in nodes.Nodes)
            {
                if (tn.Text == key)
                {
                    this.alreadyAdded = true;
                }

                this.NodeExists(tn, key);
            }
        }

        private void VerifyFolderStructure(string sourcePath, string destPath)
        {
            var dir = new DirectoryInfo(sourcePath);
            var storePath = this.GetPathToStore(dir, destPath);

            this.CopyFilesInFolder(dir, storePath);
        }

        private string GetPathToStore(DirectoryInfo dir, string destPath)
        {
            string storePath;

            if (dir.Name.Equals("Progs", StringComparison.InvariantCultureIgnoreCase) ||
                (dir.Name.Equals("Infomedia", StringComparison.InvariantCultureIgnoreCase) &&
                this.currentExtension.Equals("Infomedia", StringComparison.InvariantCultureIgnoreCase)))
            {
                storePath = Path.Combine(destPath, dir.Name);
            }
            else if (dir.Name.Equals("Update", StringComparison.InvariantCultureIgnoreCase) ||
                dir.Name.Equals("Scripts", StringComparison.InvariantCultureIgnoreCase))
            {
                storePath = Path.Combine(destPath, dir.Name);
            }
            else
            {
                var temp = Path.Combine(destPath, this.currentExtension);
                storePath = Path.Combine(temp, dir.Name);
            }

            return storePath;
        }

        private void CopyFilesInFolder(DirectoryInfo dir, string storePath)
        {
            if (!Directory.Exists(storePath))
            {
                Directory.CreateDirectory(storePath);
            }

            // Get the file contents of the directory to copy.
            var subdirectories = dir.GetDirectories();
            foreach (var subdir in subdirectories)
            {
                if (this.ShouldAddFolder(subdir, this.FilesInFolderTreeView.Nodes))
                {
                    this.CopyFilesInFolder(subdir, Path.Combine(storePath, subdir.Name));
                }
            }

            var allFiles = dir.GetFiles();
            foreach (var file in allFiles)
            {
                if (this.ShouldAddFile(file, this.FilesInFolderTreeView.Nodes))
                {
                    var temppath = Path.Combine(storePath, file.Name);
                    file.CopyTo(temppath, false);
                    this.TotalFileCount++;
                }
            }
        }

        private bool ShouldAddFolder(DirectoryInfo subdir, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                var dirInfo = node.Tag as DirectoryInfo;
                if (dirInfo != null && dirInfo.FullName == subdir.FullName)
                {
                    return node.Checked;
                }

                if (this.ShouldAddFolder(subdir, node.Nodes))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldAddFile(FileInfo file, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                var fileInfo = node.Tag as FileInfo;
                if (fileInfo != null && fileInfo.FullName == file.FullName)
                {
                    return node.Checked;
                }

                if (this.ShouldAddFile(file, node.Nodes))
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowFinalFolderStructure(string localDir)
        {
            if (this.InvokeRequired)
            {
                var d = new ShowFinalFolderStructureCallBack(this.ShowFinalFolderStructure);
                this.Invoke(d, new object[] { localDir });
                return;
            }

            var directories = new DirectoryInfo(localDir);
            this.MergedFileTreeView.BeginUpdate();
            var node = new TreeNode(directories.Name);
            node.Tag = directories;
            this.MergedFileTreeView.Nodes.Add(node);

            this.DisplayAllNodesInMergedFile(directories, node);

            this.MergedFileTreeView.EndUpdate();
        }

        private void DisplayAllNodesInMergedFile(DirectoryInfo directories, TreeNode node)
        {
            foreach (var dir in directories.GetDirectories())
            {
                var treeNode = new TreeNode(dir.Name);
                this.DisplayAllNodesInMergedFile(dir, treeNode);
                node.Nodes.Add(treeNode);
            }

            foreach (var file in directories.GetFiles())
            {
                if (file.IsReadOnly)
                {
                    file.IsReadOnly = false;
                }

                var t = new TreeNode(file.Name);
                node.Nodes.Add(t);
                t.Tag = file;
            }
        }

        private void ShowFilesInFolder(string folderName)
        {
            var directories = new DirectoryInfo(folderName);

            this.FilesInFolderTreeView.BeginUpdate();
            var node = new TreeNode(directories.Name);
            node.Tag = directories;
            this.FilesInFolderTreeView.Nodes.Add(node);

            this.ShowFolderTree(directories, node);

            this.FilesInFolderTreeView.ExpandAll();
            this.FilesInFolderTreeView.EndUpdate();
        }

        private void ShowFolderTree(DirectoryInfo directories, TreeNode node)
        {
            foreach (var dir in directories.GetDirectories())
            {
                var treeNode = new TreeNode(dir.Name);
                this.ShowFolderTree(dir, treeNode);
                node.Nodes.Add(treeNode);
                treeNode.Checked = true;
                treeNode.Tag = dir;
            }

            foreach (var file in directories.GetFiles())
            {
                    var t = new TreeNode(file.Name);
                    node.Nodes.Add(t);
                    t.Checked = true;
                    t.Tag = file;
            }
        }

        private void AddRemoveFilesFromList(object sender, ItemCheckEventArgs e)
        {
            var selectedFilesCheckedListBox = this.SelectedFilesCheckedListBox;
            if (selectedFilesCheckedListBox != null)
            {
                var item = selectedFilesCheckedListBox.SelectedItem.ToString();
                switch (e.NewValue)
                {
                    case CheckState.Checked:
                        if (!this.ListToMerge.Contains(item))
                        {
                            this.ListToMerge.Add(item);
                        }

                        break;
                    case CheckState.Unchecked:
                        this.ListToMerge.Remove(item);

                        break;
                }
            }

            this.FileCountTextbox.Text = this.ListToMerge.Count.ToString(CultureInfo.InvariantCulture);
        }

        private void ClearFilesAndCount()
        {
            this.ListToMerge.Clear();
            this.FileCountTextbox.Text = this.ListToMerge.Count.ToString(CultureInfo.InvariantCulture);
            this.SelectedFilesCheckedListBox.Items.Clear();
            this.FilesInFolderTreeView.Nodes.Clear();
            this.Index = 0;
        }

        private void ClearFilesButtonClick(object sender, EventArgs e)
        {
            this.ClearFilesAndCount();
        }

        private void MergeButtonClick(object sender, EventArgs e)
        {
            var extension = this.GetExtension();
            if (extension == string.Empty)
            {
                this.AppendLog("No extension type selected for merge!");
                return;
            }

            this.MergeFilesSaveFileDialog.Filter = extension;

            if (this.MergeFilesSaveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.MergePathTextbox.Text = this.MergeFilesSaveFileDialog.FileName;
            this.MergedFileName = this.MergePathTextbox.Text;

            this.ProcessFilesForMerge();
        }

        private void ClearMergeStatusButtonClick(object sender, EventArgs e)
        {
            this.ClearMergeStatus();
        }

        private void RadioButtonDataCheckedChanged(object sender, EventArgs e)
        {
        }

        private void RadioButtonSoftwareCheckedChanged(object sender, EventArgs e)
        {
        }
    }
}
