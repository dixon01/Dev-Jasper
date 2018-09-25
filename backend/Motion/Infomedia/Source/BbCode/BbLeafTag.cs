// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbLeafTag.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbLeafTag type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System;

    /// <summary>
    /// Base class for tags that can't contain anything.
    /// </summary>
    public abstract class BbLeafTag : BbTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbLeafTag"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        protected BbLeafTag(BbBranch parent, string tagName)
            : base(parent, tagName)
        {
        }

        /// <summary>
        /// Adds a node to the list of <see cref="BbBranch.Children"/>.
        /// This method should only be called during parsing.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// always because you can't add a node to a leaf tag.
        /// </exception>
        internal override void Add(BbNode node)
        {
            throw new NotSupportedException("Can't add a node to a leaf tag");
        }
    }
}