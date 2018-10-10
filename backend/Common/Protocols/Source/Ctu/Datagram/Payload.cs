// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Payload.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Datagram
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Representation of the CTU's payload
    /// using an object oriented style.
    /// </summary>
    public class Payload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Payload"/> class.
        /// </summary>
        public Payload()
        {
            this.Triplets = new List<Triplet>();
        }

        /// <summary>
        /// Gets the amount of bytes occupied in memory by this CTU payload.
        /// </summary>
        public int Size
        {
            get
            {
                var total = 0;

                foreach (var triplet in this.Triplets)
                {
                    // and now a variable number of bytes for the triplets
                    total += triplet.Length;
                }

                return total;
            }
        }

        /// <summary>
        /// Gets or sets the list of triplets that belongs to this CTU payload.
        /// </summary>
        public List<Triplet> Triplets { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var triplet in this.Triplets)
            {
                sb.Append(triplet.Tag);
                sb.Append('=');
                sb.Append(triplet);
                sb.Append(", ");
            }

            sb.Length -= 2;
            return sb.ToString();
        }
    }
}
