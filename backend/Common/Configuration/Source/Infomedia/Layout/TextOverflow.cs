// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextOverflow.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextOverflow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    /// <summary>
    /// The handling of oversized text in screen rendering.
    /// </summary>
    public enum TextOverflow
    {
        /// <summary>
        /// Reduce the font size until the text fits in the given rectangle.
        /// </summary>
        Scale = 0,

        /// <summary>
        /// Let the text flow out of the given rectangle (and even the screen).
        /// </summary>
        Extend = 1,

        /// <summary>
        /// Clip the text at the boundaries of the given rectangle.
        /// </summary>
        Clip = 2,

        /// <summary>
        /// Wrap the text onto a new line if the right border of the given rectangle is reached.
        /// If the text doesn't fit vertically, it will overflow.
        /// </summary>
        Wrap = 3,

        /// <summary>
        /// Scroll the text (at a definable speed and direction) inside the given rectangle
        /// if the text doesn't fit inside the rectangle.
        /// </summary>
        Scroll = 4,

        /// <summary>
        /// Scroll the text (at a definable speed and direction) inside the given rectangle
        /// even if the text would fit inside the rectangle.
        /// </summary>
        ScrollAlways = 5,

        /// <summary>
        /// Scroll the text (at a definable speed and direction) inside the given rectangle
        /// after the text there will be a delimiter and then the text will repeat immediately.
        /// </summary>
        ScrollRing = 6,

        /// <summary>
        /// Wrap the text onto a new line if the right border of the given rectangle is reached.
        /// If the text doesn't fit vertically, scale (and re-wrap) it until it fits.
        /// </summary>
        WrapScale = 13,
    }
}