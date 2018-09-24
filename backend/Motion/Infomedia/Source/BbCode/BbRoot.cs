// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbRoot.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbRoot type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Root node in the BBCode tree.
    /// This node's <see cref="BbNode.Parent"/> is always null.
    /// </summary>
    public sealed class BbRoot : BbBranch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbRoot"/> class.
        /// </summary>
        internal BbRoot()
            : base(null)
        {
        }

        /// <summary>
        /// Finds all nodes of a given type.
        /// </summary>
        /// <typeparam name="T">
        /// the type of the nodes to be returned.
        /// </typeparam>
        /// <returns>
        /// A depth-first enumeration over all nodes of the given type in this tree.
        /// </returns>
        public IEnumerable<T> FindNodesOfType<T>() where T : BbNode
        {
            foreach (T node in this.Traverse(n => n is T))
            {
                yield return node;
            }
        }

        /// <summary>
        /// Traverses all nodes of this tree in depth-first manner.
        /// </summary>
        /// <returns>
        /// A depth-first enumeration over all nodes in this tree.
        /// </returns>
        public IEnumerable<BbNode> Traverse()
        {
            return this.Traverse(n => true);
        }

        /// <summary>
        /// Traverses all nodes of this tree in depth-first manner
        /// and filters them with the given expression.
        /// Nodes' children will still be examined (and returned)
        /// even if the node itself did not fulfill the filter condition.
        /// </summary>
        /// <param name="filter">
        /// The filter condition to be fulfilled by nodes.
        /// </param>
        /// <returns>
        /// A depth-first enumeration over all nodes in this tree
        /// fulfilling the given filter condition.
        /// </returns>
        public IEnumerable<BbNode> Traverse(Predicate<BbNode> filter)
        {
            return Traverse(this, filter);
        }

        /// <summary>
        /// Converts this BBcode tree to a plain text string removing all tags.
        /// </summary>
        /// <returns>
        /// the plain text without any tags.
        /// </returns>
        public string ToPlainString()
        {
            var sb = new StringBuilder();
            foreach (BbText node in this.FindNodesOfType<BbText>())
            {
                sb.Append(node.Text);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Recursive implememntation of the traversal.
        /// </summary>
        /// <param name="parent">the parent to be traversed.</param>
        /// <param name="filter">the filter condition.</param>
        /// <returns>
        /// A depth-first enumeration over all nodes in this tree
        /// fulfilling the given filter condition.
        /// </returns>
        private static IEnumerable<BbNode> Traverse(BbBranch parent, Predicate<BbNode> filter)
        {
            foreach (var child in parent.Children)
            {
                if (filter(child))
                {
                    yield return child;
                }

                var branch = child as BbBranch;
                if (branch == null)
                {
                    continue;
                }

                foreach (var subChild in Traverse(branch, filter))
                {
                    yield return subChild;
                }
            }
        }
    }
}