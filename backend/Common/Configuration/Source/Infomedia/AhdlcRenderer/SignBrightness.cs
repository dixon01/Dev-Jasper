// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignBrightness.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SignBrightness type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AhdlcRenderer
{
    /// <summary>
    /// The brightness of a Gorba exterior sign.
    /// </summary>
    public enum SignBrightness
    {
        /// <summary>
        /// The default value (0).
        /// </summary>
        Default = 0,

        /// <summary>
        /// The first brightness level (darkest, 10).
        /// </summary>
        Brightness1 = 10,

        /// <summary>
        /// The second brightness level (11).
        /// </summary>
        Brightness2 = 11,

        /// <summary>
        /// The third brightness level (brightest, 12).
        /// </summary>
        Brightness3 = 12,
    }
}