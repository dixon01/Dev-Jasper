// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientPeerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClientPeerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// <see cref="PeerConfig"/> subclass used to configure Medi clients.
    /// </summary>
    [Implementation(typeof(ClientPeerStack))]
    public class ClientPeerConfig : PeerStackConfig<TransportClientConfig>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this peer is a gateway to a different system.
        /// Gateway connections don't forward broadcast messages and handle routing differently.
        /// In any Medi configuration there should always only be one gateway, otherwise
        /// routing of messages to other systems is not defined.
        /// </summary>
        public bool IsGateway { get; set; }
    }
}