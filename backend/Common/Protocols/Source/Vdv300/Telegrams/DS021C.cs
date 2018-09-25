// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021C.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021C type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS021c telegram.
    /// </summary>
    public class DS021C : StringArrayTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }

        /// <summary>
        /// Gets or sets the stop data of this DS021c telegram.
        /// </summary>
        public string[] StopData
        {
            get
            {
                return this.Data;
            }

            set
            {
                this.Data = value;
            }
        }
    }
}
