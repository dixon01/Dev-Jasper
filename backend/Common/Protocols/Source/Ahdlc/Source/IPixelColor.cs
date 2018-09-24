// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPixelColor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPixelColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Source
{
    /// <summary>
    /// Interface to get the color values of a pixel.
    /// AHDLC does not support alpha-values, therefore colors only have red, green and blue components.
    /// </summary>
    public interface IPixelColor
    {
        /// <summary>
        /// Gets the red value.
        /// </summary>
        byte R { get; }

        /// <summary>
        /// Gets the green value.
        /// </summary>
        byte G { get; }

        /// <summary>
        /// Gets the blue value.
        /// </summary>
        byte B { get; }
    }
}