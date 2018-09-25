// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ping.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Ping type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Messages
{
    /// <summary>
    /// For internal use only.
    /// Simple ping message.
    /// </summary>
    public class Ping : INetworkMessage
    {
        /// <summary>
        /// Gets or sets the timestamp when the message was given to the Medi stack.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Ping[{0}]", this.Timestamp);
        }
    }
}
