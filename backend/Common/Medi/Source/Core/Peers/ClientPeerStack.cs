// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientPeerStack.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClientPeerStack type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Peers.Transport;

    /// <summary>
    /// Implementation of a peer stack that uses an <see cref="ITransportClient"/>.
    /// </summary>
    internal sealed class ClientPeerStack : PeerStackBase<ITransportClient, TransportClientConfig>
    {
        /// <summary>
        /// Configures this peer with the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public override void Configure(PeerStackConfig<TransportClientConfig> config)
        {
            var clientConfig = (ClientPeerConfig)config;
            base.Configure(config);
            this.Transport.IsGateway = clientConfig.IsGateway;
        }
    }
}