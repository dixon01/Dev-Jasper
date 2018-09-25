// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO001.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS telegram for example regarding the stop approach.
    /// </summary>
    public class GO001 : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the event code.
        /// </summary>
        public int EventCode
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
