// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SignMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AhdlcRenderer
{
    /// <summary>
    /// The rendering mode of a sign.
    /// </summary>
    public enum SignMode
    {
        /// <summary>
        /// The sign only displays monochrome graphics.
        /// </summary>
        Monochrome,

        /// <summary>
        /// The sign displays color graphics.
        /// </summary>
        Color,

        /// <summary>
        /// The sign only displays (monochrome) texts (i.e. it's an interior sign).
        /// </summary>
        Text
    }
}