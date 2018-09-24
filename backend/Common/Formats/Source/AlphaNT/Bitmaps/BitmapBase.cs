// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BitmapBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Bitmaps
{
    using System;
    using System.Text;

    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// Base class for bitmaps.
    /// </summary>
    public abstract class BitmapBase : IBitmap
    {
        /// <summary>
        /// Gets or sets the width of the bitmap.
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// Gets or sets the height of the bitmap.
        /// </summary>
        public int Height { get; protected set; }

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
        public abstract IColor GetPixel(int x, int y);

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    builder.Append(this.GetPixel(x, y) == Colors.White ? "X" : " ");
                }

                builder.Append("\r\n");
            }

            if (builder.Length > 0)
            {
                builder.Length -= 2;
            }

            return builder.ToString();
        }
    }
}
