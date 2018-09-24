// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS006A.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS006A type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained in an IBIS DS006a telegram.
    /// </summary>
    public class DS006A : StringTelegram
    {
        /// <summary>
        /// Gets or sets the date and time value.
        /// It should be a 14 digit numeric string.
        /// </summary>
        public string DateTime
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
