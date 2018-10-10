// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IColor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Common
{
    /// <summary>
    /// Interface to get the values of a color.
    /// Alpha NT does not support alpha-values, therefore colors only have red, green and blue components.
    /// </summary>
    public interface IColor
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

        /// <summary>
        /// Gets a value indicating whether this color is transparent (i.e. has no color at all).
        /// </summary>
        bool Transparent { get; }
    }
}