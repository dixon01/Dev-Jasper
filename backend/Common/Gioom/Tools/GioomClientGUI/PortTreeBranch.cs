// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortTreeBranch.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortTreeBranch type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.ClientGUI
{
    using System.Collections.Generic;

    /// <summary>
    /// A branch (intermediate node) of the <see cref="PortsTreeModel"/>.
    /// </summary>
    public class PortTreeBranch : PortTreeNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortTreeBranch"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public PortTreeBranch(string name)
            : base(name)
        {
            this.Children = new List<PortTreeNodeBase>();
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public List<PortTreeNodeBase> Children { get; private set; }

        /// <summary>
        /// Clears this branch, disposing all its children.
        /// </summary>
        public void Clear()
        {
            foreach (var child in this.Children)
            {
                child.Dispose();
            }

            this.Children.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.Clear();
        }
    }
}