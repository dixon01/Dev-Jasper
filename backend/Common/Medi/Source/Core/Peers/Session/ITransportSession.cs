// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransportSession.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITransportSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    using System;

    using Gorba.Common.Medi.Core.Network;

    /// <summary>
    /// Representation of a session between two peers.
    /// Be aware that a session is not the same as an
    /// actual (TCP) connection. It is possible that the underlying
    /// TCP connection gets disconnected and connected multiple
    /// times (even possibly for each message), but the session stays the same.
    /// </summary>
    internal interface ITransportSession
    {
        /// <summary>
        /// This event is fired when this session is disconnected or expired.
        /// Be aware that a session is not the same as an
        /// actual (TCP) connection. It is possible that the underlying
        /// TCP connection gets disconnected and connected multiple
        /// times (even possibly for each message) without this
        /// event getting fired.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        /// Gets the session ID associated to this session.
        /// This ID is usually given by the server and allows
        /// to identify a session and possibly reopen a session
        /// using some session specific settings/data.
        /// </summary>
        ISessionId SessionId { get; }

        /// <summary>
        /// Gets the local gateway mode.
        /// The following values are possible:
        /// - <see cref="GatewayMode.None"/>: the session operates normally
        /// - <see cref="GatewayMode.Client"/>: this node is a gateway client and the opposite end is a server
        /// - <see cref="GatewayMode.Server"/>: this node is a server and the opposite end is a gateway client
        /// </summary>
        GatewayMode LocalGatewayMode { get; }

        /// <summary>
        /// Gets the frame controller, this might be null.
        /// </summary>
        IFrameController FrameController { get; }
    }
}
