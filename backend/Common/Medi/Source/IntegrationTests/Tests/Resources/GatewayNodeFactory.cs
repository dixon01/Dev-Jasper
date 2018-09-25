// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayNodeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayNodeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.IntegrationTests.Tests.Streams;

    /// <summary>
    /// Node factory that creates two nodes: Client (Gateway) - Server.
    /// </summary>
    public class GatewayNodeFactory : INodeFactory
    {
        /// <summary>
        /// Creates all nodes, the first and last node will be used to send messages to each other.
        /// </summary>
        /// <returns>
        /// The list of nodes.
        /// </returns>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Integration test code.")]
        List<MediNode> INodeFactory.CreateNodes()
        {
            const int ServerClientPipeId = 1;

            var clientConfig = new MediConfig
                {
                    InterceptLocalLogs = false,
                    Peers =
                        {
                            new ClientPeerConfig
                                {
                                    Transport = new PipeTransportClientConfig
                                                    {
                                                        ServerId = ServerClientPipeId,
                                                        ReconnectWait = 500,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig(),
                                    IsGateway = true
                                }
                        },
                    Services =
                        {
                            new LocalResourceServiceConfig
                                {
                                    ResourceDirectory = @"V:\Client\Resources"
                                }
                        }
                };

            var serverConfig = new MediConfig
                {
                    InterceptLocalLogs = false,
                    Peers =
                        {
                            new ServerPeerConfig
                                {
                                    Transport = new PipeTransportServerConfig
                                                    {
                                                        ServerId = ServerClientPipeId,
                                                        SessionDisconnectTimeout = 8000
                                                    },
                                    Codec = new BecCodecConfig()
                                }
                        },
                    Services =
                        {
                            new LocalResourceServiceConfig
                                {
                                    ResourceDirectory = @"V:\Intermediate\Resources"
                                }
                        }
                };

            var client = new MediNode(new MediAddress("UC", "Client"));
            client.Configure(clientConfig);

            var server = new MediNode(new MediAddress("US", "Server"));
            server.Configure(serverConfig);

            return new List<MediNode> { server, client };
        }
    }
}