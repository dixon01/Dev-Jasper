// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbNode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Base class for all nodes in the BBCode tree.
    /// </summary>
    public abstract class BbNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbNode"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        internal BbNode(BbBranch parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public BbBranch Parent { get; private set; }

        /// <summary>
        /// Finds the closest parent of the given type.
        /// This method can be used to find the valid 
        /// style for a text part.
        /// </summary>
        /// <typeparam name="T">
        /// the type of parent to be found.
        /// </typeparam>
        /// <returns>
        /// returns the closes parent or null if it is not found.
        /// </returns>
        public T GetClosestParent<T>() where T : BbTag
        {
            for (var branch = this.Parent; branch != null; branch = branch.Parent)
            {
                var tag = branch as T;
                if (tag != null)
                {
                    return tag;
                }
            }

            return null;
        }

        /// <summary>
        /// This method should only be called during parsing when 
        /// children are moved around.
        /// </summary>
        /// <param name="parent">
        /// The new parent.
        /// </param>
        internal void UpdateParent(BbBranch parent)
        {
            this.Parent = parent;
        }
    }
}