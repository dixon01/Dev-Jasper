// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS170.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS170 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS170 telegram.
    /// </summary>
    public class DS170 : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the status.
        /// 0 means "OK", 1 means "Error", 2 means "Paper issue", 3 means "Cash issue".
        /// </summary>
        public int Status
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