// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiHopNodeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Node factory that creates three nodes: Client - Intermediate - Server.
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
    /// Node factory that creates three nodes: Client - Intermediate - Server.
    /// </summary>
    public class MultiHopNodeFactory : INodeFactory
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
            const int ServerIntermediatePipeId = 1;
            const int IntermediateClientPipeId = 2;

            var clientConfig = new MediConfig
                {
                    InterceptLocalLogs = false,
                    Peers =
                        {
                            new ClientPeerConfig
                                {
                                    Transport = new PipeTransportClientConfig
                                                    {
                                                        ServerId = IntermediateClientPipeId,
                                                        ReconnectWait = 500,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig()
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

            var intermediateConfig = new MediConfig
                {
                    InterceptLocalLogs = false,
                    Peers =
                        {
                            new ClientPeerConfig
                                {
                                    Transport = new PipeTransportClientConfig
                                                    {
                                                        ServerId = ServerIntermediatePipeId,
                                                        ReconnectWait = 500,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig()
                                },
                            new ServerPeerConfig
                                {
                                    Transport = new PipeTransportServerConfig
                                                    {
                                                        ServerId = IntermediateClientPipeId,
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

            var serverConfig = new MediConfig
                {
                    InterceptLocalLogs = false,
                    Peers =
                        {
                            new ServerPeerConfig
                                {
                                    Transport = new PipeTransportServerConfig
                                                    {
                                                        ServerId = ServerIntermediatePipeId,
                                                        SessionDisconnectTimeout = 8000
                                                    },
                                    Codec = new BecCodecConfig()
                                }
                        },
                    Services =
                        {
                            new LocalResourceServiceConfig
                                {
                                    ResourceDirectory = @"V:\Server\Resources"
                                }
                        }
                };

            var client = new MediNode(new MediAddress("UC", "Client"));
            client.Configure(clientConfig);

            var intermediate = new MediNode(new MediAddress("UI", "Intermediate"));
            intermediate.Configure(intermediateConfig);

            var server = new MediNode(new MediAddress("US", "Server"));
            server.Configure(serverConfig);

            return new List<MediNode> { server, intermediate, client };
        }
    }
}