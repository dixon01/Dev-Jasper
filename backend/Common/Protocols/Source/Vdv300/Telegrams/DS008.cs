// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS008.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS008 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained in an IBIS DS008 telegram.
    /// </summary>
    public class DS008 : StringTelegram
    {
        /// <summary>
        /// Gets or sets the wagon address.
        /// This is usually a 3 digit IBIS-hex number.
        /// </summary>
        public string WagonAddress
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
