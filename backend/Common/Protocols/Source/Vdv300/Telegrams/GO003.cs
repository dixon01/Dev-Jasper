// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO003.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS GO003 telegram.
    /// </summary>
    public class GO003 : StringArrayTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }
    }
}
