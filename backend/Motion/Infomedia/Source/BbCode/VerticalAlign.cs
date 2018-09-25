// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerticalAlign.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VerticalAlign type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System;

    /// <summary>
    /// The vertical alignment BB code tag <c>[valign=xxx][/valign]</c>.
    /// </summary>
    public class VerticalAlign : BbValueTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerticalAlign"/> class.
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
        internal VerticalAlign(BbBranch parent, string tagName, string alignment)
            : base(parent, tagName, alignment)
        {
            this.Alignment = (Align)Enum.Parse(typeof(Align), alignment, true);
        }

        /// <summary>
        /// Vertical alignment of text.
        /// </summary>
        public enum Align
        {
            /// <summary>
            /// The vertical alignment should not be changed.
            /// </summary>
            Default,

            /// <summary>
            /// Top of the text is at the given Y coordinate.
            /// </summary>
            Top,

            /// <summary>
            /// Arithmetical center of the text is at the given Y coordinate.
            /// </summary>
            Middle,

            /// <summary>
            /// Bottom of the text (bottom of lowest character; e.g. g, y, ...) is at the given Y coordinate.
            /// </summary>
            Bottom
        }

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        public Align Alignment { get; private set; }
    }
}
