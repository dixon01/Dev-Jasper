// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeepAlive.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the KeepAlive type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream.Messages
{
    /// <summary>
    /// Message sent between peers to keep the connection alive.
    /// </summary>
    public class KeepAlive : IInternalMessage
    {
        /// <summary>
        /// Gets or sets the timestamp that identifies this message.
        /// </summary>
        public int Timestamp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a request (true) or a response (false).
        /// </summary>
        public bool IsRequest { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("KeepAlive[{0}, {1}]", this.IsRequest ? "Request" : "Response", this.Timestamp);
        }
    }
}
