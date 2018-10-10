// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS002.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS002 telegram.
    /// </summary>
    public class DS002 : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the RunNumber.
        /// It is a number with 2 digits.
        /// </summary>
        public int RunNumber
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
