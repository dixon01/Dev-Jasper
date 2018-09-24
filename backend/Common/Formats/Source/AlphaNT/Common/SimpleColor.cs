// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleColor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Common
{
    /// <summary>
    /// Implementation of <see cref="IColor"/> that just has the three components.
    /// </summary>
    public class SimpleColor : IColor
    {
        private readonly byte transparent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleColor"/> class.
        /// <see cref="Transparent"/> will be false.
        /// </summary>
        /// <param name="red">
        /// The red component.
        /// </param>
        /// <param name="green">
        /// The green component.
        /// </param>
        /// <param name="blue">
        /// The blue component.
        /// </param>
        public SimpleColor(byte red, byte green, byte blue)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
            this.transparent = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleColor"/> class
        /// that represents a (fully) transparent color.
        /// <see cref="Transparent"/> will be true.
        /// </summary>
        internal SimpleColor()
        {
            this.transparent = 1;
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
        public bool Transparent
        {
            get
            {
                return this.transparent != 0;
            }
        }
    }
}