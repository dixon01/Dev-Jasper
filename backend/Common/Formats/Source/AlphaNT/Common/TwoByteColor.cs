// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwoByteColor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TwoByteColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Common
{
    /// <summary>
    /// A color represented by two bytes.
    /// </summary>
    internal class TwoByteColor : IColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TwoByteColor"/> class.
        /// </summary>
        /// <param name="colorValueMsb">
        /// The color value in MSB first order.
        /// </param>
        public TwoByteColor(ushort colorValueMsb)
            : this((byte)((colorValueMsb >> 8) & 0xFF), (byte)(colorValueMsb & 0xFF))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoByteColor"/> class.
        /// </summary>
        /// <param name="msb">
        /// The MSB.
        /// </param>
        /// <param name="lsb">
        /// The LSB.
        /// </param>
        public TwoByteColor(byte msb, byte lsb)
        {
            this.B = (byte)((msb & 0x0F) << 4);
            this.G = (byte)(lsb & 0xF0);
            this.R = (byte)((lsb & 0x0F) << 4);
            this.Transparent = false;
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

        /// <summary>
        /// Gets a value indicating whether this color is transparent (i.e. has no color at all).
        /// </summary>
        public bool Transparent { get; private set; }
    }
}