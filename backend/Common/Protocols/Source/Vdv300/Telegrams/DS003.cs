// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS003 telegram.
    /// </summary>
    public class DS003 : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the DestinationNumber.
        /// It is a number with 3 digits.
        /// </summary>
        public int DestinationNumber
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
