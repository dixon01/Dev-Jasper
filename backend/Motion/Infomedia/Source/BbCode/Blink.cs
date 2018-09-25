// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Blink.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Blink type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Bold BBCode tag [bl][/bl].
    /// </summary>
    public sealed class Blink : BbTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Blink"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        internal Blink(BbBranch parent, string tagName)
            : base(parent, tagName)
        {
        }
    }
}
