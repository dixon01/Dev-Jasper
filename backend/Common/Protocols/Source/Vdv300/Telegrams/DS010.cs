// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS010.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    ///  Representation of all the information
    /// contained into an IBIS DS010 telegram.
    /// </summary>
    public class DS010 : StringTelegram
    {
        /// <summary>
        /// Gets or sets the stop index.
        /// Usually this is a 4 digit number.
        /// </summary>
        public string StopIndex
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
