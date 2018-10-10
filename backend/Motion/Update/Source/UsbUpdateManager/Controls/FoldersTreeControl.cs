// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FoldersTreeControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FoldersTreeControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// The folders tree control.
    /// </summary>
    public partial class FoldersTreeControl : UserControl
    {
        private const string ImageKey = "Folder";

        private FoldersTreeModel treeModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="FoldersTreeControl"/> class.
        /// </summary>
        public FoldersTreeControl()
        {
            this.InitializeComponent();

            var icon = FileIconManager.CreateFolderIcon(Environment.CurrentDirectory, true);
            this.imageList.Images.Add(ImageKey, icon);
        }

        /// <summary>
        /// Event that is fired when the <see cref="SelectedFolder"/> changes.
        /// </summary>
        public event EventHandler SelectedFolderChanged;

        /// <summary>
        /// Gets or sets the tree model from which this tree will be created.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FoldersTreeModel TreeModel
        {
            get
            {
                return this.treeModel;
            }

            set
            {
                if (this.treeModel == value)
                {
                    return;
                }

                this.treeView.BeginUpdate();
                try
                {
                    this.ClearTree();
                    this.treeModel = value;
                    this.CreateTree();
                }
                finally
                {
                    this.treeView.EndUpdate();
                }

                this.treeView.ExpandAll();
                this.RaiseSelectedFolderChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected folder.
        /// Null means that no folder is selected.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FoldersTreeModel.Folder SelectedFolder
        {
            get
            {
                if (this.treeView.SelectedNode == null)
                {
                    return null;
                }

                return this.treeView.SelectedNode.Tag as FoldersTreeModel.Folder;
            }

            set
            {
                this.treeView.SelectedNode = value == null ? null : value.TreeNode;
            }
        }

        /// <summary>
        /// Raises the <see cref="SelectedFolderChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseSelectedFolderChanged(EventArgs e)
        {
            var handler = this.SelectedFolderChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ClearTree()
        {
            if (this.treeModel == null)
            {
                return;
            }

            this.ClearTree(this.treeView.Nodes);

            this.treeView.Nodes.Clear();
        }

        private void ClearTree(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                this.ClearNode(node);
            }
        }

        private void ClearNode(TreeNode node)
        {
            var folder = node.Tag as FoldersTreeModel.Folder;
            if (folder != null)
            {
                folder.ChildrenChanged -= this.FolderOnChildrenChanged;
            }

            this.ClearTree(node.Nodes);
        }

        private void CreateTree()
        {
            if (this.treeModel == null || this.treeModel.Root == null)
            {
                return;
            }

            this.PopulateTree(this.treeModel.Root, this.treeView.Nodes);
        }

        private void PopulateTree(FoldersTreeModel.Folder folder, TreeNodeCollection nodes)
        {
            folder.ChildrenChanged += this.FolderOnChildrenChanged;
            var children = new List<TreeNode>(folder.Children.Count);
            foreach (var child in folder.Children)
            {
                var node = this.CreateNode(child);
                children.Add(node);
            }

            children.Sort((a, b) => string.Compare(a.Text, b.Text, StringComparison.CurrentCultureIgnoreCase));
            nodes.AddRange(children.ToArray());
        }

        private TreeNode CreateNode(FoldersTreeModel.Folder child)
        {
            var node = new TreeNode(child.Name) { Tag = child, ImageKey = ImageKey };
            child.TreeNode = node;
            this.PopulateTree(child, node.Nodes);
            return node;
        }

        private void InsertNodeSorted(TreeNodeCollection nodes, TreeNode node)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (string.Compare(node.Text, nodes[i].Text, StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    nodes.Insert(i, node);
                    return;
                }
            }

            nodes.Add(node);
        }

        private void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            this.RaiseSelectedFolderChanged(e);
        }

        private void FolderOnChildrenChanged(object sender, ListChangedEventArgs e)
        {
            var folder = sender as FoldersTreeModel.Folder;
            if (folder == null)
            {
                return;
            }

            var node = folder.TreeNode;
            if (node == null)
            {
                return;
            }

            switch (e.ListChangedType)
            {
                case ListChangedType.Reset:
                    {
                        this.ClearTree(node.Nodes);
                        this.PopulateTree(folder, node.Nodes);
                        break;
                    }

                case ListChangedType.ItemAdded:
                    {
                        var child = folder.Children[e.NewIndex];
                        var childNode = this.CreateNode(child);
                        this.InsertNodeSorted(node.Nodes, childNode);
                        node.Expand();
                        break;
                    }

                case ListChangedType.ItemDeleted:
                    {
                        foreach (TreeNode childNode in node.Nodes)
                        {
                            var found = false;
                            foreach (var child in folder.Children)
                            {
                                if (childNode.Tag == child)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                node.Nodes.Remove(childNode);
                                this.ClearNode(childNode);
                                break;
                            }
                        }

                        break;
                    }
            }
        }
    }
}
