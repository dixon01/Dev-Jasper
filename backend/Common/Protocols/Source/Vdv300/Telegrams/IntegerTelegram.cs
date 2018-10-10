// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerTelegram.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Container of all the meaningful information
    /// that belong to a specific IBIS telegram having
    /// the proper data as an integer.
    /// </summary>
    public abstract class IntegerTelegram : Telegram
    {
        /// <summary>
        /// Gets or sets the "pure" IBIS payload
        /// as an integer.
        /// </summary>
        public int Data { get; set; }
    }
}
