// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bold.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Bold type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Bold BBCode tag [b][/b].
    /// </summary>
    public sealed class Bold : BbTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bold"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        internal Bold(BbBranch parent, string tagName)
            : base(parent, tagName)
        {
        }
    }
}