// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Gorba AG" file="GO007.cs">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Representation of all the information
//   contained into an IBIS GO007 telegram.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS GO007 telegram.
    /// </summary>
    public class GO007 : StringArrayTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }

        /// <summary>
        /// Gets or sets the stop data of this GO007 telegram.
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
