// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayMode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    /// <summary>
    /// The possible gateway modes for a session.
    /// </summary>
    internal enum GatewayMode
    {
        /// <summary>
        /// The session operates normally.
        /// </summary>
        None,

        /// <summary>
        /// A gateway client doesn't want routing updates and doesn't send or receive subscriptions
        /// over the given session.
        /// </summary>
        Client,

        /// <summary>
        /// A gateway server (the server side of a session to a gateway client) doesn't provide
        /// routing updates and doesn't send or receive subscriptions over the given session.
        /// A server peer can be in different modes for each session depending on how the sessions connected.
        /// </summary>
        Server
    }
}