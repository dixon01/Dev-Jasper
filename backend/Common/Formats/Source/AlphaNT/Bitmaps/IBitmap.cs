// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBitmap.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBitmap type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Bitmaps
{
    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// Interface to access pixels of a bitmap.
    /// </summary>
    public interface IBitmap
    {
        /// <summary>
        /// Gets the width of the bitmap.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the bitmap.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the color of the pixel at the given position.
        /// </summary>
        /// <param name="x">
        /// The horizontal position in the bitmap (0 ... <see cref="Width"/> - 1).
        /// </param>
        /// <param name="y">
        /// The vertical position in the bitmap (0 ... <see cref="Height"/> - 1).
        /// </param>
        /// <returns>
        /// The <see cref="IColor"/> at the given position.
        /// If this bitmap is monochrome, the returned value will always either be
        /// <see cref="Colors.Black"/> or <see cref="Colors.White"/>.
        /// </returns>
        IColor GetPixel(int x, int y);
    }
}