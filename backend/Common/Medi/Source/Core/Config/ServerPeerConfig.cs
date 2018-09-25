// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerPeerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServerPeerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// <see cref="PeerConfig"/> subclass used to configure Medi servers.
    /// </summary>
    [Implementation(typeof(ServerPeerStack))]
    public class ServerPeerConfig : PeerStackConfig<TransportServerConfig>
    {
    }
}