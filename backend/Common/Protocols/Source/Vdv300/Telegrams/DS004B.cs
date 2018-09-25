// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004B.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004B type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS004b telegram.
    /// </summary>
    public class DS004B : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the stop identifier (not stop index!) for ticketing.
        /// </summary>
        public int StopId
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