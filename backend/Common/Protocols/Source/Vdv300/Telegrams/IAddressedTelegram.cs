// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAddressedTelegram.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAddressedTelegram type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Telegram that has an IBIS address.
    /// </summary>
    public interface IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        int IbisAddress { get; set; }
    }
}