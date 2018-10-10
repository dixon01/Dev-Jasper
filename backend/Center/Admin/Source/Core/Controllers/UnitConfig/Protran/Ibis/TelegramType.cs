// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    /// <summary>
    /// The telegram type used for handling the transformations in ibis.xml.
    /// </summary>
    public enum TelegramType
    {
        /// <summary>
        /// Empty telegram that doesn't contain any payload.
        /// </summary>
        Empty,

        /// <summary>
        /// Regular telegram that has a string payload (default).
        /// </summary>
        String,

        /// <summary>
        /// Telegram that has an integer payload.
        /// </summary>
        Integer,
    }
}