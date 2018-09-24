// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS001A.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS001A telegram.
    /// </summary>
    public class DS001A : StringTelegram
    {
        /// <summary>
        /// Gets or sets the special line char value.
        /// It can be a number or an alphanumeric string.
        /// </summary>
        public string SpecialLine
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
