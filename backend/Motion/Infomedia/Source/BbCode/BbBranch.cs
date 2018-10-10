// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbBranch.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System.Collections.Generic;

    /// <summary>
    /// Non-leaf node in the BBCode tree.
    /// </summary>
    public abstract class BbBranch : BbNode
    {
        private readonly List<BbNode> children = new List<BbNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BbBranch"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        internal BbBranch(BbBranch parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets an enumeration over all children.
        /// </summary>
        public IEnumerable<BbNode> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Adds a node to the list of <see cref="Children"/>.
        /// This method should only be called during parsing.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        internal virtual void Add(BbNode node)
        {
            node.UpdateParent(this);

            this.children.Add(node);
        }

        /// <summary>
        /// Method called after this tag was successfully
        /// parsed. Subclasses have the possibility to
        /// replace children or create a completely new
        /// node representing the contents of this node.
        /// </summary>
        /// <param name="context">
        /// The context
        /// </param>
        /// <returns>
        /// The default implementation simply returns this instance.
        /// </returns>
        internal virtual BbNode Cleanup(IBbParserContext context)
        {
            return this;
        }
    }
}