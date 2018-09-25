// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003A.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS003A telegram.
    /// </summary>
    public class DS003A : StringTelegram
    {
        /// <summary>
        /// Gets or sets the destination value.
        /// This string contains the transformed telegram content.
        /// </summary>
        public string Destination
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
