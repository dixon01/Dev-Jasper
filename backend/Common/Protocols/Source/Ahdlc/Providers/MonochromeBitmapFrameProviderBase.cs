// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonochromeBitmapFrameProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MonochromeBitmapFrameProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using System;

    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// Base class for all monochrome bitmap frame providers.
    /// </summary>
    public abstract class MonochromeBitmapFrameProviderBase : FrameProviderBase
    {
        /// <summary>
        /// Gets the bitmap data for the output command for the given block number.
        /// </summary>
        /// <param name="pixelSource">
        /// The pixel source.
        /// </param>
        /// <param name="offsetX">
        /// The horizontal offset into the <see cref="pixelSource"/> where to get the 8 pixels.
        /// </param>
        /// <returns>
        /// The bitmap data for the given output block.
        /// </returns>
        protected byte[] GetBitmapData(IMonochromePixelSource pixelSource, int offsetX)
        {
            var maxX = Math.Min(offsetX + 8, pixelSource.Width);
            var data = new byte[pixelSource.Height];
            for (int y = 0; y < pixelSource.Height; y++)
            {
                byte value = 0;
                for (int posX = 0; posX < 8; posX++)
                {
                    var x = offsetX + posX;
                    if (x >= maxX)
                    {
                        break;
                    }

                    if (pixelSource.GetPixel(x, y))
                    {
                        value |= (byte)(1 << (7 - posX));
                    }
                }

                data[y] = value;
            }

            return data;
        }

        /// <summary>
        /// Calculates the number of 8-bit blocks from the width of an image.
        /// </summary>
        /// <param name="width">
        /// The image width.
        /// </param>
        /// <returns>
        /// The block count.
        /// </returns>
        protected int CalculateBlockCount(int width)
        {
            return (width + 7) / 8;
        }
    }
}