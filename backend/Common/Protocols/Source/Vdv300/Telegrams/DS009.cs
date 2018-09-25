// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS009.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS009 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained in an IBIS DS009 telegram.
    /// </summary>
    public class DS009 : StringTelegram
    {
        /// <summary>
        /// Gets or sets the stop name.
        /// This is a 16 character string (unless changed by a transformation).
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
