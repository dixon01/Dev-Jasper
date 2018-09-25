// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemView.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   View for Medi node management information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.FileSystem;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// View for Medi node management information.
    /// </summary>
    public partial class FileSystemView : UserControl
    {
        private int lastSelectedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemView"/> class.
        /// </summary>
        public FileSystemView()
        {
            this.InitializeComponent();

            this.cbxSource.SelectedIndex = 0;
        }

        private static TreeNode CreateNode(IFileSystemInfo item)
        {
            var node = new TreeNode(item.Name);
            if (item is IDirectoryInfo)
            {
                node.Nodes.Add(string.Empty);
            }

            node.Tag = item;
            return node;
        }

        private void SetRootNode(IFileSystem fileSystem)
        {
            this.treeFileSystem.Nodes.Clear();
            foreach (var drive in fileSystem.GetDrives())
            {
                this.treeFileSystem.Nodes.Add(CreateNode(drive.RootDirectory));
            }
        }

        private void CbxSourceSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedString = this.cbxSource.SelectedItem as string;
            if (selectedString != null)
            {
                if (this.cbxSource.SelectedIndex == 0)
                {
                    // use local
                    this.SetRootNode(FileSystemManager.Local);
                    this.lastSelectedIndex = this.cbxSource.SelectedIndex;
                }
                else
                {
                    // Browse...
                    var peerSelection = new PeerSelectionForm();
                    peerSelection.Text = "Browse...";
                    if (peerSelection.ShowDialog(this) == DialogResult.OK && peerSelection.SelectedAddress != null)
                    {
                        int index = this.cbxSource.Items.IndexOf(peerSelection.SelectedAddress);
                        if (index >= 0)
                        {
                            this.cbxSource.SelectedIndex = index;
                        }
                        else
                        {
                            this.cbxSource.Items.Insert(this.cbxSource.Items.Count - 1, peerSelection.SelectedAddress);
                            this.cbxSource.SelectedItem = peerSelection.SelectedAddress;
                        }
                    }
                    else
                    {
                        this.cbxSource.SelectedIndex = this.lastSelectedIndex;
                    }
                }

                return;
            }

            var selectedAddress = this.cbxSource.SelectedItem as MediAddress;
            if (selectedAddress == null)
            {
                return;
            }

            this.SetRootNode(new RemoteFileSystem(selectedAddress));
            this.lastSelectedIndex = this.cbxSource.SelectedIndex;
        }

        private void TreeManagementAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.pgridInfo.SelectedObject = e.Node.Tag;
        }

        private void TreeManagementBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Nodes.Count != 1 || node.Nodes[0].Tag != null)
            {
                // already expanded before
                return;
            }

            var directory = node.Tag as IDirectoryInfo;
            if (directory == null)
            {
                return;
            }

            var oldCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                node.Nodes.Clear();

                foreach (var info in directory.GetFileSystemInfos())
                {
                    node.Nodes.Add(CreateNode(info));
                }
            }
            finally
            {
                this.Cursor = oldCursor;
            }
        }

        private void DownloadFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            var selectedNode = this.treeFileSystem.SelectedNode;
            if (selectedNode == null)
            {
                return;
            }

            var file = selectedNode.Tag as IFileInfo;
            if (file == null || !(file.FileSystem is RemoteFileSystem))
            {
                return;
            }

            this.saveFileDialog.Title = "Download " + file.FullName;
            this.saveFileDialog.FileName = file.Name;
            if (this.saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                var fileSystem = (RemoteFileSystem)file.FileSystem;
                fileSystem.BeginDownloadFile(
                    file,
                    this.saveFileDialog.FileName,
                    this.FileDownloaded,
                    new KeyValuePair<IFileInfo, string>(file, this.saveFileDialog.FileName));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FileDownloaded(IAsyncResult ar)
        {
            IFileInfo file;
            string localFileName;
            try
            {
                var state = (KeyValuePair<IFileInfo, string>)ar.AsyncState;
                file = state.Key;
                localFileName = state.Value;
                var fileSystem = (RemoteFileSystem)file.FileSystem;
                fileSystem.EndDownloadFile(ar);
            }
            catch (Exception ex)
            {
                this.Invoke(
                    new MethodInvoker(
                        () =>
                        MessageBox.Show(
                            this, ex.Message, "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return;
            }

            var msg = string.Format("{0} was downloaded successfully to {1}", file.FullName, localFileName);
            this.Invoke(
                new MethodInvoker(
                    () =>
                    MessageBox.Show(
                        this, msg, "Download Completed", MessageBoxButtons.OK, MessageBoxIcon.Information)));
        }

        private void ContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            var selectedNode = this.treeFileSystem.SelectedNode;
            if (selectedNode != null)
            {
                var file = selectedNode.Tag as IFileInfo;
                if (file != null)
                {
                    return;
                }
            }

            e.Cancel = true;
        }
    }
}
