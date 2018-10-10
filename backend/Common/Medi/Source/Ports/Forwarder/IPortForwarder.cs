// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPortForwarder.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPortForwarder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Forwarder
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Ports.Config;

    /// <summary>
    /// Interface of a port forwarder the actual work horse of the port forwarding.
    /// </summary>
    internal interface IPortForwarder
    {
        /// <summary>
        /// Starts this forwarder.
        /// </summary>
        /// <param name="dispatcher">
        /// The message dispatcher to use for sending messages.
        /// </param>
        /// <param name="localId">
        /// The local forwarding id.
        /// </param>
        /// <param name="remoteId">
        /// The remote forwarding id.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote Medi address.
        /// </param>
        /// <returns>
        /// The real <see cref="ForwardingEndPointConfig"/> used.
        /// </returns>
        ForwardingEndPointConfig Start(IMessageDispatcher dispatcher, string localId, string remoteId, MediAddress remoteAddress);

        /// <summary>
        /// Stops this forwarder.
        /// </summary>
        void Stop();
    }
}