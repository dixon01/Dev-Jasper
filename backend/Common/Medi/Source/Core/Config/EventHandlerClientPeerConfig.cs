// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerClientPeerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventHandlerClientPeerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.ComponentModel;
    using System.Net;

    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the legacy EventHandler client peer.
    /// </summary>
    [Implementation(typeof(Peers.Edi.EventHandlerClientPeer))]
    public class EventHandlerClientPeerConfig : EventHandlerPeerConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerClientPeerConfig"/> class.
        /// </summary>
        public EventHandlerClientPeerConfig()
        {
            this.RemoteHost = IPAddress.Loopback.ToString();
            this.RemotePort = EventHandlerPeerConfigBase.DefaultPort;
        }

        /// <summary>
        /// Gets or sets the remote server host name.
        /// </summary>
        [DefaultValue("127.0.0.1")]
        public string RemoteHost { get; set; }

        /// <summary>
        /// Gets or sets the remote server TCP port.
        /// </summary>
        [DefaultValue(DefaultPort)]
        public int RemotePort { get; set; }
    }
}