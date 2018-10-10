// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO006.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS GO006 telegram.
    /// </summary>
    public class GO006 : StringTelegram
    {
        /// <summary>
        /// Gets or sets the line.
        /// It's an alphanumeric string.
        /// </summary>
        public string Line
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
