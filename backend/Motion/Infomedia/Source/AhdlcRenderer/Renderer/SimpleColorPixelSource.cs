// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleColorPixelSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleColorPixelSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Renderer
{
    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// A simple color pixel source that allows to set values.
    /// </summary>
    public class SimpleColorPixelSource : IColorPixelSource, IGraphicsContext
    {
        private readonly int[,] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleColorPixelSource"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public SimpleColorPixelSource(int width, int height)
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
        /// The <see cref="IPixelColor"/> value of the pixel.
        /// </returns>
        public IPixelColor GetPixel(int x, int y)
        {
            var color = this.data[x, y];
            return new PixelColor(
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
        public void SetPixel(int x, int y, IPixelColor color)
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
            this.data[x, y] = (red << 16) | (green << 8) | blue;
        }

        /// <summary>
        /// Draws an <see cref="IBitmap"/> onto this context.
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
                        this.SetPixel(posX, posY, pixel.R, pixel.G, pixel.B);
                    }
                }
            }
        }
    }
}