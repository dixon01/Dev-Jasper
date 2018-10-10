// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Colors.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Colors type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Common
{
    /// <summary>
    /// Static definition of commonly used <see cref="IColor"/> values.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// The black color (#000000).
        /// </summary>
        public static readonly IColor Black = new SimpleColor(0, 0, 0);

        /// <summary>
        /// The white color (#FFFFFF).
        /// </summary>
        public static readonly IColor White = new SimpleColor(255, 255, 255);

        /// <summary>
        /// The transparent color (no color information).
        /// </summary>
        public static readonly IColor Transparent = new SimpleColor();
    }
}
