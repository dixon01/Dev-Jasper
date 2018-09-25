// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Italic.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Italic type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Italic BBCode tag [i][/i].
    /// </summary>
    public sealed class Italic : BbTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Italic"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        internal Italic(BbBranch parent, string tagName)
            : base(parent, tagName)
        {
        }
    }
}