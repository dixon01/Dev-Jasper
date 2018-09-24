// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerServerPeerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventHandlerServerPeerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.ComponentModel;

    using Gorba.Common.Medi.Core.Peers.Edi;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the legacy EventHandler server (EdiServer) peer.
    /// </summary>
    [Implementation(typeof(EventHandlerServerPeer))]
    public class EventHandlerServerPeerConfig : EventHandlerPeerConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerServerPeerConfig"/> class.
        /// </summary>
        public EventHandlerServerPeerConfig()
        {
            this.LocalPort = EventHandlerPeerConfigBase.DefaultPort;
        }

        /// <summary>
        /// Gets or sets the local server TCP port.
        /// Default value is 1598.
        /// </summary>
        [DefaultValue(DefaultPort)]
        public int LocalPort { get; set; }
    }
}