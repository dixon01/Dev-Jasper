// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained in an IBIS DS021 telegram (destination).
    /// </summary>
    public class DS021 : StringTelegram, IAddressedTelegram
    {
        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public int IbisAddress { get; set; }

        /// <summary>
        /// Gets or sets the destination name.
        /// This is a multiple of 16 character string (unless changed by a transformation).
        /// </summary>
        public string DestinationName
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
