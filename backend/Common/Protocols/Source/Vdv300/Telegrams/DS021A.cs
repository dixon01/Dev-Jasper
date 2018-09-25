// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021A.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS021a telegram.
    /// </summary>
    public class DS021A : StringArrayTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }

        /// <summary>
        /// Gets or sets the stop data of this DS021a telegram.
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
