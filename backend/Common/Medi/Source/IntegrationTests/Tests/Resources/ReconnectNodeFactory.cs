// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReconnectNodeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReconnectNodeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.IntegrationTests.Tests.Streams;

    /// <summary>
    /// Node factory that makes children reconnect after a certain amount of bytes transferred.
    /// This is a wrapper around another factory that actually creates the nodes.
    /// </summary>
    internal class ReconnectNodeFactory : INodeFactory
    {
        private readonly int disconnectAfterBytes;

        private readonly INodeFactory parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReconnectNodeFactory"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent factory that will be sued to create the nodes.
        /// </param>
        /// <param name="disconnectAfterBytes">
        /// The disconnect after bytes.
        /// </param>
        public ReconnectNodeFactory(INodeFactory parent, int disconnectAfterBytes)
        {
            this.parent = parent;
            this.disconnectAfterBytes = disconnectAfterBytes;
        }

        /// <summary>
        /// Creates all nodes, the first and last node will be used to send messages to each other.
        /// </summary>
        /// <returns>
        /// The list of nodes.
        /// </returns>
        public List<MediNode> CreateNodes()
        {
            var parentNodes = this.parent.CreateNodes();
            var nodes = new List<MediNode>(parentNodes.Count);
            foreach (var parentNode in parentNodes)
            {
                var config = this.ChangeConfig(parentNode.Config);
                var node = new MediNode(parentNode.Address);
                node.Configure(config);
                nodes.Add(node);
            }

            return nodes;
        }

        private MediConfig ChangeConfig(MediConfig config)
        {
            foreach (var peer in config.Peers)
            {
                var client = peer as ClientPeerConfig;
                if (client != null && client.Transport is PipeTransportClientConfig)
                {
                    ((PipeTransportClientConfig)client.Transport).DisconnectAfterBytes = this.disconnectAfterBytes;
                    continue;
                }

                var server = peer as ServerPeerConfig;
                if (server != null && server.Transport is PipeTransportServerConfig)
                {
                    ((PipeTransportServerConfig)server.Transport).DisconnectAfterBytes = this.disconnectAfterBytes;
                }
            }

            return config;
        }
    }
}