// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeerImpl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPeerImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    /// <summary>
    /// Interface to be implemented by all peers.
    /// </summary>
    internal interface IPeerImpl : IPeer
    {
        /// <summary>
        /// Starts the peer.
        /// The implementation of this method should register
        /// to events from the given <see cref="IMessageDispatcherImpl"/>.
        /// </summary>
        /// <param name="medi">
        /// The message dispatcher to be used by this peer.
        /// </param>
        void Start(IMessageDispatcherImpl medi);

        /// <summary>
        /// Stops the peer.
        /// The implementation of this method should deregister
        /// from events from the <see cref="IMessageDispatcherImpl"/>
        /// provided in <see cref="Start"/>.
        /// </summary>
        void Stop();
    }
}
