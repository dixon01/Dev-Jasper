// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramTypes.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramTypes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd
{
    using System;

    /// <summary>
    /// The telegram types for a given sign.
    /// </summary>
    [Flags]
    public enum TelegramTypes
    {
        /// <summary>
        /// The line (L) telegram is enabled.
        /// </summary>
        Line = 1,

        /// <summary>
        /// The destination (Z) telegram is enabled.
        /// </summary>
        Destination = 2,

        /// <summary>
        /// The text (DS021, DS003a) telegrams are enabled.
        /// </summary>
        Text = 4
    }
}