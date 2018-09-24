// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewLine.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NewLine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// The BBCode [br] tag. This tag has no children.
    /// </summary>
    public class NewLine : BbLeafTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewLine"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        public NewLine(BbBranch parent, string tagName)
            : base(parent, tagName)
        {
        }
    }
}