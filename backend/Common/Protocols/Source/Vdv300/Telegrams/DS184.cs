// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS184.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS184 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS184 telegram.
    /// </summary>
    public class DS184 : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the status.
        /// 0 means "OK", 1 means "new data", 2 means "still measuring", 3 means "device error".
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