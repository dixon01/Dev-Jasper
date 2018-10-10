// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO005.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS GO005 telegram.
    /// </summary>
    public class GO005 : StringArrayTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }

        /// <summary>
        /// Gets or sets the stop data of this GO005 telegram.
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
