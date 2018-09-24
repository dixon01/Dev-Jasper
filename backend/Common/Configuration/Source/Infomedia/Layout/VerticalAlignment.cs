// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerticalAlignment.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VerticalAlignment type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    /// <summary>
    /// Vertical alignment of text.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// Top of the text is at the given Y coordinate.
        /// </summary>
        Top,

        /// <summary>
        /// Arithmetical center of the text is at the given Y coordinate.
        /// </summary>
        Middle,

        /// <summary>
        /// Baseline (bottom of normal text; e.g. x, w, ...) of the text is at the given Y coordinate.
        /// </summary>
        Baseline,

        /// <summary>
        /// Bottom of the text (bottom of lowest character; e.g. g, y, ...) is at the given Y coordinate.
        /// </summary>
        Bottom
    }
}