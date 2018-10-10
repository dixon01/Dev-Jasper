// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckerBoradPixelSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CheckerBoradPixelSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Source
{
    /// <summary>
    /// A pixel source that shows a checker board (black-white pattern).
    /// </summary>
    public class CheckerBoradPixelSource : IMonochromePixelSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckerBoradPixelSource"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public CheckerBoradPixelSource(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the width of the source.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the source.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the black or white value of the given pixel.
        /// </summary>
        /// <param name="x">
        /// The x coordinate (indexed from 0).
        /// </param>
        /// <param name="y">
        /// The y coordinate (indexed from 0).
        /// </param>
        /// <returns>
        /// True if the pixel is white, false if it is black.
        /// </returns>
        public bool GetPixel(int x, int y)
        {
            return (x % 2) == (y % 2);
        }
    }
}