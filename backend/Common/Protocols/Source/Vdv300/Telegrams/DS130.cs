// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS130.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS130 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS130 telegram.
    /// </summary>
    public class DS130 : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the status.
        /// 0: "OK"
        /// 1: "Searching"
        /// 2: "Reached file end"
        /// 3: "Missing file"
        /// 4: "Distortion in announcement"
        /// 5: "System error"
        /// 6: "Wrong index"
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