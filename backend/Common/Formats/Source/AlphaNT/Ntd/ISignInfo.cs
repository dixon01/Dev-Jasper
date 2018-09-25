// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISignInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISignInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd
{
    using System.Collections.Generic;

    using Gorba.Common.Formats.AlphaNT.Ntd.Telegrams;

    /// <summary>
    /// Interface to access information about a single sign defined in a <see cref="NtdFile"/>.
    /// </summary>
    public interface ISignInfo
    {
        /// <summary>
        /// Gets the address of the sign (1..15).
        /// </summary>
        int Address { get; }

        /// <summary>
        /// Gets the width of the sign in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the sign in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the types of telegrams that are configured for this sign.
        /// </summary>
        TelegramTypes TelegramTypes { get; }

        /// <summary>
        /// Gets all line (<c>lXXX</c>) telegrams defined for this sign.
        /// </summary>
        /// <returns>
        /// The list of <see cref="ITelegramInfo"/>
        /// </returns>
        IEnumerable<ITelegramInfo> GetLineTelegrams();

        /// <summary>
        /// Gets all destination (<c>zXXX</c>) telegrams defined for this sign.
        /// </summary>
        /// <returns>
        /// The list of <see cref="ITelegramInfo"/>
        /// </returns>
        IEnumerable<ITelegramInfo> GetDestinationTelegrams();
    }
}