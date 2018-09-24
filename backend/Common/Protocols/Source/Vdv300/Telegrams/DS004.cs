// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS004.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS004 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS004 telegram.
    /// </summary>
    public class DS004 : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the ticketing characteristics.
        /// </summary>
        public int Characteristics
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
