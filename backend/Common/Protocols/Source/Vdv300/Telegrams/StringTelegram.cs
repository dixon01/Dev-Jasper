// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringTelegram.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringTelegram type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Container of all the meaningful information
    /// that belong to a specific IBIS telegram having
    /// the proper data as a string.
    /// </summary>
    public abstract class StringTelegram : Telegram
    {
        /// <summary>
        /// Gets or sets the "pure" IBIS payload
        /// in a string format.
        /// </summary>
        public string Data { get; set; }
    }
}