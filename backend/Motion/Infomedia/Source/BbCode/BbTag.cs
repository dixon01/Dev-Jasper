// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbTag.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbTag type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Base class for all BBCode tags.
    /// </summary>
    public abstract class BbTag : BbBranch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbTag"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        internal BbTag(BbBranch parent, string tagName)
            : base(parent)
        {
            this.TagName = tagName;
        }

        /// <summary>
        /// Gets the tag name of this tag.
        /// </summary>
        public string TagName { get; private set; }
    }
}