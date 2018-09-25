// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortsTreeModel.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortsTreeModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.ClientGUI
{
    using System;
    using System.Collections;

    using Aga.Controls.Tree;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// The ports tree model.
    /// </summary>
    public class PortsTreeModel : TreeModelBase
    {
        private readonly PortTreeBranch root;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortsTreeModel"/> class.
        /// </summary>
        public PortsTreeModel()
        {
            this.root = new PortTreeBranch("ROOT");
        }

        /// <summary>
        /// Loads the given ports into this model, discarding any existing nodes.
        /// </summary>
        /// <param name="ports">
        /// The ports.
        /// </param>
        public void LoadPorts(IPortInfo[] ports)
        {
            this.root.Clear();
            foreach (var port in ports)
            {
                var parent = this.FindAppNode(port.Address);
                var node = new PortTreeLeaf(port);
                node.ValueChanged += this.NodeOnValueChanged;
                parent.Children.Add(node);
            }

            this.Refresh();
        }

        /// <summary>
        /// Gets the children of the given path.
        /// </summary>
        /// <param name="treePath">
        /// The tree path.
        /// </param>
        /// <returns>
        /// The children.
        /// </returns>
        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.FirstNode == null)
            {
                return this.root.Children;
            }

            var branch = treePath.LastNode as PortTreeBranch;
            if (branch != null)
            {
                return branch.Children;
            }

            return new object[0];
        }

        /// <summary>
        /// Gets a value indicating if the given path is a leaf in this tree.
        /// </summary>
        /// <param name="treePath">
        /// The tree path.
        /// </param>
        /// <returns>
        /// A value indicating if the given path is a leaf in this tree.
        /// </returns>
        public override bool IsLeaf(TreePath treePath)
        {
            return treePath.LastNode is PortTreeLeaf;
        }

        private PortTreeBranch FindAppNode(MediAddress address)
        {
            var unitNode = this.FindNode(this.root, address.Unit);
            return this.FindNode(unitNode, address.Application);
        }

        private PortTreeBranch FindNode(PortTreeBranch branch, string name)
        {
            foreach (PortTreeBranch child in branch.Children)
            {
                if (child.Name == name)
                {
                    return child;
                }
            }

            var unitNode = new PortTreeBranch(name);
            branch.Children.Add(unitNode);
            return unitNode;
        }

        private void NodeOnValueChanged(object sender, EventArgs e)
        {
            var node = (PortTreeLeaf)sender;
            var unitNode = this.FindNode(this.root, node.Port.Info.Address.Unit);
            var appNode = this.FindNode(unitNode, node.Port.Info.Address.Application);

            var args = new TreeModelEventArgs(new TreePath(new object[] { unitNode, appNode }), new object[] { node });
            this.OnNodesChanged(args);
        }
    }
}