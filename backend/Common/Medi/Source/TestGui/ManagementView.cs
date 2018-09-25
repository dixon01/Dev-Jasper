// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementView.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   View for Medi node management information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Medi.TestGui.Management;

    /// <summary>
    /// View for Medi node management information.
    /// </summary>
    public partial class ManagementView : UserControl
    {
        private int lastSelectedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementView"/> class.
        /// </summary>
        public ManagementView()
        {
            this.InitializeComponent();

            this.cbxSource.SelectedIndex = 0;
        }

        private static TreeNode CreateNode(IManagementProvider provider)
        {
            var node = new TreeNode(provider.Name);
            node.Nodes.Add(string.Empty);

            var objectProvider = provider as IManagementObjectProvider;
            var tableProvider = provider as IManagementTableProvider;
            if (objectProvider != null)
            {
                node.Tag = new ManagementObject(objectProvider);
            }
            else if (tableProvider != null)
            {
                node.Tag = new ManagementTable(tableProvider);
            }
            else
            {
                node.Tag = provider;
            }

            return node;
        }

        private void SetRootNode(IManagementProvider root)
        {
            this.treeManagement.Nodes.Clear();
            this.treeManagement.Nodes.Add(CreateNode(root));
        }

        private void LoadChildNodes(TreeNode node, bool reload)
        {
            if (!reload && (node.Nodes.Count != 1 || node.Nodes[0].Tag != null))
            {
                // already expanded before
                return;
            }

            var oldCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            var provider = node.Tag as IManagementProvider;
            if (provider == null)
            {
                var mgmtObj = node.Tag as ManagementObject;
                var mgmtTable = node.Tag as ManagementTable;
                if (mgmtObj != null)
                {
                    provider = mgmtObj.Provider;
                }
                else if (mgmtTable != null)
                {
                    provider = mgmtTable.Provider;
                }
                else
                {
                    return;
                }
            }

            try
            {
                node.Nodes.Clear();

                if (reload)
                {
                    var remote = provider as IRemoteManagementProvider;
                    if (remote != null)
                    {
                        remote.Reload();
                    }
                }

                foreach (var child in provider.Children)
                {
                    node.Nodes.Add(CreateNode(child));
                }
            }
            finally
            {
                this.Cursor = oldCursor;
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
                    this.SetRootNode(MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot);
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

            this.SetRootNode(
                MessageDispatcher.Instance.ManagementProviderFactory.CreateRemoteProvider(selectedAddress));
            this.lastSelectedIndex = this.cbxSource.SelectedIndex;
        }

        private void TreeManagementAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.ShowNodeInfo(e.Node);
        }

        private void TreeManagementBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            this.LoadChildNodes(e.Node, false);
        }

        private void ContextMenuOpening(object sender, CancelEventArgs e)
        {
            this.refreshToolStripMenuItem.Enabled = this.treeManagement.SelectedNode != null
                                                    && this.treeManagement.SelectedNode.Tag != null;
        }

        private void RefreshToolStripMenuItemClick(object sender, EventArgs e)
        {
            var node = this.treeManagement.SelectedNode;
            bool expanded = node.IsExpanded;

            this.LoadChildNodes(node, true);

            this.ShowNodeInfo(node);

            if (expanded)
            {
                node.Expand();
            }
        }

        private void ShowNodeInfo(TreeNode node)
        {
            if (node != null && node.Tag != null)
            {
                var obj = node.Tag as ManagementObject;
                if (obj != null)
                {
                    this.pgridManagement.Visible = true;
                    this.gridTable.Visible = false;
                    this.pgridManagement.SelectedObject = obj;
                    return;
                }

                var table = node.Tag as ManagementTable;
                if (table != null)
                {
                    this.pgridManagement.Visible = false;
                    this.gridTable.Visible = true;
                    this.gridTable.DataSource = new ManagementTable(table.Provider);
                    return;
                }
            }

            this.pgridManagement.Visible = false;
            this.gridTable.Visible = false;
            this.pgridManagement.SelectedObject = null;
        }
    }
}
