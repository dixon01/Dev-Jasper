// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS006.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS005 telegram.
    /// </summary>
    public class DS006 : StringTelegram
    {
        /// <summary>
        /// Gets or sets the date's value.
        /// It can be a number or an alphanumeric string.
        /// </summary>
        public string Date
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
