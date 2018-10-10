// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS120.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS120 telegram.
    /// </summary>
    public class DS120 : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the status.
        /// 0 means "OK", 3 means "Error".
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
