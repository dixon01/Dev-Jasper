// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Indicator of the mode of operation.
//   In unicast and anycast modes, the client sets this field to 3 (client) in the request
//   and the server sets it to 4 (server) in the reply.
//   In multicast mode, the server sets this field to 5 (broadcast).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Sntp
{
    /// <summary>
    /// Indicator of the mode of operation.
    /// In unicast and any-cast modes, the client sets this field to 3 (client) in the request
    /// and the server sets it to 4 (server) in the reply.
    /// In multicast mode, the server sets this field to 5 (broadcast).
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// Reserved mode value.
        /// </summary>
        Reserved = 0,

        /// <summary>
        /// Symmetric active.
        /// </summary>
        SymmetricActive = 1,

        /// <summary>
        /// Symmetric passive.
        /// </summary>
        SymmetricPassive = 2,

        /// <summary>
        /// Request from a client.
        /// </summary>
        Client = 3,

        /// <summary>
        /// Response from a server.
        /// </summary>
        Server = 4,

        /// <summary>
        /// Broadcast message.
        /// </summary>
        Broadcast = 5,

        /// <summary>
        /// Reserved for NTP control message.
        /// </summary>
        ReservedNtpControl = 6,

        /// <summary>
        /// Reserved for private use.
        /// </summary>
        ReservedPrivate = 7
    }
}
