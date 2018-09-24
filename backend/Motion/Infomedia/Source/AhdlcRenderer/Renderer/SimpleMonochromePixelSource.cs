// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleMonochromePixelSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleMonochromePixelSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Renderer
{
    using System;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// A simple monochrome pixel source that allows to set values.
    /// </summary>
    public class SimpleMonochromePixelSource : IMonochromePixelSource, IGraphicsContext
    {
        private readonly byte[,] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMonochromePixelSource"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public SimpleMonochromePixelSource(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            this.data = new byte[(width + 7) / 8, height];
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
            if (x < 0 || x >= this.Width)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if (y < 0 || y >= this.Height)
            {
                throw new ArgumentOutOfRangeException("y");
            }

            var b = this.data[x / 8, y];
            return ((b >> (x % 8)) & 1) == 1;
        }

        /// <summary>
        /// Sets the black or white value of the given pixel.
        /// </summary>
        /// <param name="x">
        /// The x coordinate (indexed from 0).
        /// </param>
        /// <param name="y">
        /// The y coordinate (indexed from 0).
        /// </param>
        /// <param name="value">
        /// True if the pixel is white, false if it is black.
        /// </param>
        public void SetPixel(int x, int y, bool value)
        {
            var b = this.data[x / 8, y];
            if (value)
            {
                b |= (byte)(1 << (x % 8));
            }
            else
            {
                b &= (byte)~(1 << (x % 8));
            }

            this.data[x / 8, y] = b;
        }

        /// <summary>
        /// Draws an <see cref="IBitmap"/> onto this bitmap.
        /// </summary>
        /// <param name="offsetX">
        /// The x position where to draw the bitmap.
        /// </param>
        /// <param name="offsetY">
        /// The y position where to draw the bitmap.
        /// </param>
        /// <param name="bitmap">
        /// The bitmap to draw.
        /// </param>
        public void DrawBitmap(int offsetX, int offsetY, IBitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                var posX = offsetX + x;
                if (posX < 0 || posX >= this.Width)
                {
                    continue;
                }

                for (int y = 0; y < bitmap.Height; y++)
                {
                    var posY = offsetY + y;
                    if (posY < 0 || posY >= this.Height)
                    {
                        continue;
                    }

                    var pixel = bitmap.GetPixel(x, y);
                    if (!pixel.Transparent)
                    {
                        this.SetPixel(posX, posY, pixel.R != 0 || pixel.G != 0 || pixel.B != 0);
                    }
                }
            }
        }
    }
}
