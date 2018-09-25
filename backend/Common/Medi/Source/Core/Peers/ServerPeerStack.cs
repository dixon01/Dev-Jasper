// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerPeerStack.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServerPeerStack type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Peers.Transport;

    /// <summary>
    /// Implementation of a peer stack that uses an <see cref="ITransportServer"/>.
    /// </summary>
    internal sealed class ServerPeerStack : PeerStackBase<ITransportServer, TransportServerConfig>
    {
    }
}