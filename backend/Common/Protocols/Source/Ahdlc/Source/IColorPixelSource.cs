// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IColorPixelSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IColorPixelSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Source
{
    /// <summary>
    /// Interface for accessing color pixels of a bitmap (or any other source of pixels).
    /// </summary>
    public interface IColorPixelSource : IPixelSource
    {
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
        IPixelColor GetPixel(int x, int y);
    }
}