// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PeerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration class for peers.
    /// </summary>
    [XmlInclude(typeof(ClientPeerConfig))]
    [XmlInclude(typeof(ServerPeerConfig))]
    [XmlInclude(typeof(EventHandlerPeerConfig))]
    [XmlInclude(typeof(EventHandlerClientPeerConfig))]
    [XmlInclude(typeof(EventHandlerServerPeerConfig))]
    public class PeerConfig
    {
    }
}
