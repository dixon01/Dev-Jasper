// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStateAck.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceStateAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream.Messages
{
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// Acknowledge that a <see cref="ResourceStateResponse"/> was handled by the remote peer.
    /// </summary>
    public class ResourceStateAck : IInternalMessage
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("ResourceStateAck[{0}]", this.Id);
        }
    }
}