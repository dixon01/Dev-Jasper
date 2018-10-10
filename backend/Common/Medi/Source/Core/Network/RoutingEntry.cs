// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Routing table entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Network
{
    /// <summary>
    /// Routing table entry.
    /// </summary>
    public class RoutingEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingEntry"/> class.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="hops">
        /// The hops.
        /// </param>
        internal RoutingEntry(MediAddress address, int hops)
        {
            this.Hops = hops;
            this.Address = address;
        }

        /// <summary>
        /// Gets the address.
        /// Please be aware that the address is
        /// treated as if it was immutable. If you try to
        /// change the address after assigning it to this
        /// property, the routing algorithms will likely fail.
        /// </summary>
        public MediAddress Address { get; private set; }

        /// <summary>
        /// Gets the number of hops required to reach the given address.
        /// </summary>
        public int Hops { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            var other = (RoutingEntry)obj;
            return this.Address.Equals(other.Address);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Address.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.Address, this.Hops);
        }
    }
}