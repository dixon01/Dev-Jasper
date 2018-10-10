// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportSession.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransportSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    using System;

    using Gorba.Common.Medi.Core.Network;

    /// <summary>
    /// Representation of a session between two peers.
    /// This class can be used as-is or sub-classed if necessary
    /// for a certain transport.
    /// A session can be valid longer than a connection in
    /// the transport layer; if a client reconnects to a server,
    /// it is possible to reuse the existing session and thus
    /// save connection information (like subscriptions, Codec
    /// specific data, ...)
    /// </summary>
    internal class TransportSession : ITransportSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransportSession"/> class.
        /// </summary>
        /// <param name="sessionId">
        /// The session id of this session. Multiple session objects can
        /// represent the same session, as long as the session id is
        /// equal (<see cref="object.Equals(object)"/> and <see cref="object.GetHashCode"/>).
        /// </param>
        /// <param name="localGatewayMode">
        /// The local gateway mode.
        /// </param>
        /// <param name="frameController">
        /// The frame controller used for verifying frame IDs. This can be null.
        /// </param>
        public TransportSession(ISessionId sessionId, GatewayMode localGatewayMode, IFrameController frameController)
        {
            this.SessionId = sessionId;
            this.LocalGatewayMode = localGatewayMode;
            this.FrameController = frameController;
        }

        /// <summary>
        /// Event fired when this session was disconnected.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Gets the id of this session. Multiple session objects can
        /// represent the same session, as long as the session id is
        /// equal (<see cref="object.Equals(object)"/> and <see cref="object.GetHashCode"/>).
        /// </summary>
        public ISessionId SessionId { get; private set; }

        /// <summary>
        /// Gets the local gateway mode.
        /// The following values are possible:
        /// - <see cref="GatewayMode.None"/>: the session operates normally
        /// - <see cref="GatewayMode.Client"/>: this node is a gateway client and the opposite end is a server
        /// - <see cref="GatewayMode.Server"/>: this node is a server and the opposite end is a gateway client
        /// </summary>
        public GatewayMode LocalGatewayMode { get; private set; }

        /// <summary>
        /// Gets the frame controller, this might be null.
        /// </summary>
        public IFrameController FrameController { get; private set; }

        /// <summary>
        /// Fires the <see cref="Disconnected"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void RaiseDisconnected(EventArgs e)
        {
            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
