// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS036.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS036 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS036 telegram.
    /// </summary>
    public class DS036 : StringTelegram
    {
        /// <summary>
        /// Gets or sets the announcement index; this is usually a 4 digit number.
        /// </summary>
        public string AnnouncementIndex
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
