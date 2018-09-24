// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMonochromePixelSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMonochromePixelSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Source
{
    /// <summary>
    /// Interface for accessing color pixels of a bitmap (or any other source of pixels).
    /// </summary>
    public interface IMonochromePixelSource : IPixelSource
    {
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
        bool GetPixel(int x, int y);
    }
}