// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForwarderMessageBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ForwarderMessageBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Messages
{
    /// <summary>
    /// Base class for all forwarder messages.
    /// Don't use this class outside this library.
    /// </summary>
    public abstract class ForwarderMessageBase
    {
        /// <summary>
        /// Gets or sets the forwarding id.
        /// </summary>
        public string ForwardingId { get; set; }

        /// <summary>
        /// Gets or sets the stream id.
        /// </summary>
        public int StreamId { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0}: {1} [{2}]", this.GetType().Name, this.ForwardingId, this.StreamId);
        }
    }
}