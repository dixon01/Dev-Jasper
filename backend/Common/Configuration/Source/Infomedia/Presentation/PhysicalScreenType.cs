// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalScreenType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhysicalScreenType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation
{
    /// <summary>
    /// The physical screen type.
    /// </summary>
    public enum PhysicalScreenType
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Unknown type of screen
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// TFT screen
        /// </summary>
        TFT = 1,

        /// <summary>
        /// LED matrix display (iqube or exterior sign)
        /// </summary>
        LED = 2,

        /// <summary>
        /// Audio output to a speaker
        /// </summary>
        Audio = 128,

        // ReSharper restore InconsistentNaming
    }
}