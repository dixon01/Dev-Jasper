// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS010J.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS010J type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS DS010J telegram.
    /// </summary>
    public class DS010J : IntegerTelegram
    {
        /// <summary>
        /// Gets or sets the stop index.
        /// </summary>
        public int StopIndex
        {
            get
            {
                return this.Data % 1000;
            }

            set
            {
                this.Data = (this.Status * 1000) + value;
            }
        }

        /// <summary>
        /// Gets or sets the status code:
        /// 2: all stops have to be shown
        /// 3: Field "current stop" has to be omitted
        /// </summary>
        public int Status
        {
            get
            {
                return this.Data / 1000;
            }

            set
            {
                this.Data = this.StopIndex + (value * 1000);
            }
        }
    }
}
