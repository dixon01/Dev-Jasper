// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramFormat.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramFormat type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    /// <summary>
    /// Possible formats to (de)serialize telegrams.
    /// </summary>
    public enum TelegramFormat
    {
        /// <summary>
        /// The telegram contains header, payload, CR and checksum.
        /// </summary>
        Full,

        /// <summary>
        /// The telegram contains header, payload and CR but no checksum.
        /// </summary>
        NoChecksum,

        /// <summary>
        /// The telegram contains header and payload but no CR and no checksum.
        /// </summary>
        NoFooter
    }
}
