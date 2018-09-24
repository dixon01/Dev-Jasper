// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS084.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS084 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS084 telegram.
    /// </summary>
    public class DS084 : EmptyTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }
    }
}
