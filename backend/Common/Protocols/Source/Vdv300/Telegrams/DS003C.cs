// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003C.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained in an IBIS DS003c telegram.
    /// </summary>
    public class DS003C : StringTelegram
    {
        /// <summary>
        /// Gets or sets the stop name.
        /// This is a multiple of 4 character string (unless changed by a transformation).
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
