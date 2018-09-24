// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicControlPin.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GraphicControlPin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    /// <summary>
    /// The four possible graphic control pins.
    /// </summary>
    public enum GraphicControlPin
    {
        /// <summary>
        /// The trim pin (bits 0 and 4).
        /// </summary>
        Trim = 0,

        /// <summary>
        /// The CCT1 pin (bits 1 and 5).
        /// </summary>
        Cct1 = 1,

        /// <summary>
        /// The CCT2 pin (bits 2 and 6).
        /// </summary>
        Cct2 = 2,

        /// <summary>
        /// The HPD pin (bits 3 and 7).
        /// </summary>
        HotPlugDetection = 3
    }
}