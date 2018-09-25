// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Telegram.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the meaningful information
    /// that belong to a specific IBIS telegram.
    /// </summary>
    public abstract class Telegram
    {
        /// <summary>
        /// Gets or sets the "pure" IBIS payload.
        /// </summary>
        [XmlIgnore]
        public byte[] Payload { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
