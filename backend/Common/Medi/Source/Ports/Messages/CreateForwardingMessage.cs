// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateForwardingMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CreateForwardingMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Messages
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Ports.Config;

    /// <summary>
    /// The create forwarding message.
    /// Don't use this class outside this library.
    /// </summary>
    public class CreateForwardingMessage
    {
        /// <summary>
        /// Gets or sets the local id.
        /// </summary>
        public string LocalId { get; set; }

        /// <summary>
        /// Gets or sets the remote id.
        /// </summary>
        public string RemoteId { get; set; }

        /// <summary>
        /// Gets or sets the remote address.
        /// </summary>
        public MediAddress RemoteAddress { get; set; }

        /// <summary>
        /// Gets or sets the endpoint configuration.
        /// </summary>
        public ForwardingEndPointConfig Config { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(
                "{0}: local: {1} remote: {2}, address: {3}, config: {4}",
                this.GetType().Name,
                this.LocalId,
                this.RemoteId,
                this.RemoteAddress,
                this.Config);
        }
    }
}