// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004C.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004C type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS004c telegram.
    /// </summary>
    public class DS004C : StringTelegram
    {
        /// <summary>
        /// Gets or sets the stop name for ticketing.
        /// </summary>
        public string StopName
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