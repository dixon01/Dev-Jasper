// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS010B.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS010b telegram.
    /// </summary>
    public class DS010B : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the stop index.
        /// It can be a number with 2 or 3 digits.
        /// </summary>
        public int StopIndex
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
