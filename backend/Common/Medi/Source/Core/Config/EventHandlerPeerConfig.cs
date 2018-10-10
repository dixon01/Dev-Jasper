// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerPeerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventHandlerPeerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using Gorba.Common.Medi.Core.Peers.Edi;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Obsolete configuration for the legacy EventHandler server peer.
    /// This class is only available to support old versions of the medi.config file
    /// that had <c>EventHandlerPeerConfig</c> as type for the event handler server peer.
    /// Please use <see cref="EventHandlerServerPeerConfig"/> instead.
    /// </summary>
    [Implementation(typeof(EventHandlerServerPeer))]
    public class EventHandlerPeerConfig : EventHandlerServerPeerConfig
    {
    }
}
