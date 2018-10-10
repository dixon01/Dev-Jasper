// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransportImplementation.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITransportImplementation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Transport
{
    using System;

    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;

    /// <summary>
    /// Interface to be implemented by a transport implementation.
    /// <see cref="ITransportClient"/> and <see cref="ITransportServer"/>
    /// implement this interface and it is used by <see cref="PeerStackBase{TTransport,TTransportConfig}"/>.
    /// </summary>
    internal interface ITransportImplementation
    {
        /// <summary>
        /// Event that is fired when the transport has finished starting up.
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Event that is fired when a session has been connected.
        /// </summary>
        event EventHandler<SessionEventArgs> SessionConnected;

        /// <summary>
        /// Starts the transport implementation, connecting it with the given codec.
        /// </summary>
        /// <param name="medi">
        ///     The local message dispatcher implementation
        /// </param>
        /// <param name="messageTranscoder">
        ///     The message transcoder that is on top of this transport.
        /// </param>
        void Start(IMessageDispatcherImpl medi, MessageTranscoder messageTranscoder);
    }
}
