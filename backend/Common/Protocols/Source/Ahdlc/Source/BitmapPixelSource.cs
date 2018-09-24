// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapPixelSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BitmapPixelSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Source
{
    using System.Drawing;

    /// <summary>
    /// Pixel source that takes pixels from a <see cref="Bitmap"/>.
    /// </summary>
    public class BitmapPixelSource : IMonochromePixelSource, IColorPixelSource
    {
        private readonly Bitmap bitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapPixelSource"/> class.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        public BitmapPixelSource(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        /// <summary>
        /// Gets the width of the bitmap.
        /// </summary>
        public int Width
        {
            get
            {
                return this.bitmap.Width;
            }
        }

        /// <summary>
        /// Gets the height of the bitmap.
        /// </summary>
        public int Height
        {
            get
            {
                return this.bitmap.Height;
            }
        }

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
            // TODO: it is most probably faster to directly access the bitmap data
            var color = this.bitmap.GetPixel(x, y);
            return new PixelColor(color.R, color.G, color.B);
        }

        bool IMonochromePixelSource.GetPixel(int x, int y)
        {
            var color = this.GetPixel(x, y);
            return GetGreyLevel(color.R, color.G, color.B) >= 0.5;
        }

        private static double GetGreyLevel(byte r, byte g, byte b)
        {
            return ((r * 0.299) + (g * 0.587) + (b * 0.114)) / 255;
        }
    }
}