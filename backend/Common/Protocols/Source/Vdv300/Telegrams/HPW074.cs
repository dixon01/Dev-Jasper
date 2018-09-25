// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HPW074.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HPW074 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// HPW074 Special Text Reference as defined by
    /// IBIS2 producer (HPW/Siemens)
    /// </summary>
    public class HPW074 : IntegerTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }
    }
}
