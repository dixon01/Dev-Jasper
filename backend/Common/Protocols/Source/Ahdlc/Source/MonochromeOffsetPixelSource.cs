// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonochromeOffsetPixelSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MonochromeOffsetPixelSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Source
{
    /// <summary>
    /// Pixel source wrapper that shows the wrapped pixel source inside a bigger image at a given offset.
    /// </summary>
    public class MonochromeOffsetPixelSource : IMonochromePixelSource
    {
        private readonly int offsetX;

        private readonly int offsetY;

        private readonly IMonochromePixelSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonochromeOffsetPixelSource"/> class.
        /// </summary>
        /// <param name="width">
        /// The width of the resulting image.
        /// </param>
        /// <param name="height">
        /// The height of the resulting image.
        /// </param>
        /// <param name="offsetX">
        /// The horizontal offset where the <see cref="source"/> should be shown.
        /// </param>
        /// <param name="offsetY">
        /// The vertical offset where the <see cref="source"/> should be shown.
        /// </param>
        /// <param name="source">
        /// The wrapped pixel source.
        /// </param>
        public MonochromeOffsetPixelSource(
            int width, int height, int offsetX, int offsetY, IMonochromePixelSource source)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.source = source;
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
            x -= this.offsetX;
            y -= this.offsetY;
            if (x < 0 || y < 0 || x >= this.source.Width || y >= this.source.Height)
            {
                return false;
            }

            return this.source.GetPixel(x, y);
        }
    }
}
