// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleBitmap.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleBitmap type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Bitmaps
{
    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// Simple <see cref="IBitmap"/> implementation that supports alpha values.
    /// </summary>
    public class SimpleBitmap : IBitmap
    {
        /// <summary>
        /// Every 32 bit integer in the data is:
        /// 1 byte Alpha (currently only 0xFF for opaque or 0x00 for transparent)
        /// 1 byte Red
        /// 1 byte Green
        /// 1 byte Blue
        /// </summary>
        private readonly int[,] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBitmap"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public SimpleBitmap(int width, int height)
        {
            this.data = new int[width, height];

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
        /// Gets the color value of the given pixel.
        /// </summary>
        /// <param name="x">
        /// The x coordinate (indexed from 0).
        /// </param>
        /// <param name="y">
        /// The y coordinate (indexed from 0).
        /// </param>
        /// <returns>
        /// The <see cref="IColor"/> value of the pixel.
        /// </returns>
        public IColor GetPixel(int x, int y)
        {
            var color = this.data[x, y];
            if ((color & 0xFF000000) == 0)
            {
                return Colors.Transparent;
            }

            return new SimpleColor(
                (byte)((color >> 16) & 0xFF), // R
                (byte)((color >> 8) & 0xFF), // G
                (byte)(color & 0xFF)); // B
        }

        /// <summary>
        /// Sets the color value of the given pixel.
        /// </summary>
        /// <param name="x">
        /// The x coordinate (indexed from 0).
        /// </param>
        /// <param name="y">
        /// The y coordinate (indexed from 0).
        /// </param>
        /// <param name="color">
        /// The color value.
        /// </param>
        public void SetPixel(int x, int y, IColor color)
        {
            this.SetPixel(x, y, color.R, color.G, color.B);
        }

        /// <summary>
        /// Sets the color value of the given pixel.
        /// </summary>
        /// <param name="x">
        /// The x coordinate (indexed from 0).
        /// </param>
        /// <param name="y">
        /// The y coordinate (indexed from 0).
        /// </param>
        /// <param name="red">
        /// The red color component.
        /// </param>
        /// <param name="green">
        /// The green color component.
        /// </param>
        /// <param name="blue">
        /// The blue color component.
        /// </param>
        public void SetPixel(int x, int y, byte red, byte green, byte blue)
        {
            unchecked
            {
                this.data[x, y] = (int)0xFF000000 | (red << 16) | (green << 8) | blue;
            }
        }

        /// <summary>
        /// Sets the color value of the given pixel.
        /// </summary>
        /// <param name="x">
        /// The x coordinate (indexed from 0).
        /// </param>
        /// <param name="y">
        /// The y coordinate (indexed from 0).
        /// </param>
        public void SetPixelTransparent(int x, int y)
        {
            this.data[x, y] = 0x00000000;
        }
    }
}
