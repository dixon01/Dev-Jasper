// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HorizontalAlign.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HorizontalAlign type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System;

    /// <summary>
    /// The horizontal alignment BB code tag <c>[align=xxx][/align]</c>.
    /// </summary>
    public class HorizontalAlign : BbValueTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalAlign"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent tag.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        /// <param name="alignment">
        /// The alignment.
        /// </param>
        internal HorizontalAlign(BbBranch parent, string tagName, string alignment)
            : base(parent, tagName, alignment)
        {
            this.Alignment = (Align)Enum.Parse(typeof(Align), alignment, true);
        }

        /// <summary>
        /// Horizontal alignment of text.
        /// </summary>
        public enum Align
        {
            /// <summary>
            /// The horizontal alignment should not be changed.
            /// </summary>
            Default,

            /// <summary>
            /// Left alignment.
            /// </summary>
            Left,

            /// <summary>
            /// Center alignment.
            /// </summary>
            Center,

            /// <summary>
            /// Right alignment.
            /// </summary>
            Right
        }

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        public Align Alignment { get; private set; }
    }
}
