// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteUpdate.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RouteUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Network
{
    /// <summary>
    /// Operation on a routing table.
    /// </summary>
    public class RouteUpdate
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entry was added or removed.
        /// </summary>
        public bool Added { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public MediAddress Address { get; set; }

        /// <summary>
        /// Gets or sets the number of hops to reach the given address.
        /// </summary>
        public int Hops { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}{1} [{2}]", this.Added ? '+' : '-', this.Address, this.Hops);
        }
    }
}
