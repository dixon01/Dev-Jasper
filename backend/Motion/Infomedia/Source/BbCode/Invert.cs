// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Invert.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Invert type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Inversion BB code tag <c>[inv][/inv]</c>.
    /// </summary>
    public sealed class Invert : BbTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Invert"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        internal Invert(BbBranch parent, string tagName)
            : base(parent, tagName)
        {
        }
    }
}
