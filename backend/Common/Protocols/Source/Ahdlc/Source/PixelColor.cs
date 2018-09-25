// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PixelColor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PixelColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Source
{
    /// <summary>
    /// Simple implementation of <see cref="IPixelColor"/>.
    /// </summary>
    public class PixelColor : IPixelColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelColor"/> class.
        /// </summary>
        /// <param name="red">
        /// The red color component.
        /// </param>
        /// <param name="green">
        /// The green color component.
        /// </param>
        /// <param name="blue">
        /// The blue color component.
        /// </param>
        public PixelColor(byte red, byte green, byte blue)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
        }

        /// <summary>
        /// Gets the red value.
        /// </summary>
        public byte R { get; private set; }

        /// <summary>
        /// Gets the green value.
        /// </summary>
        public byte G { get; private set; }

        /// <summary>
        /// Gets the blue value.
        /// </summary>
        public byte B { get; private set; }
    }
}