// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayMultiHopNodeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayMultiHopNodeFactory type.
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
    /// Node factory that creates six nodes on three units: A:1 - A:2 | B:1 - B:2 - C:1 - C:2.
    /// Between unit A and unit B there is a gateway configured.
    /// The units only have one resource service each (always on the node with app name 1).
    /// </summary>
    public class GatewayMultiHopNodeFactory : INodeFactory
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
            const int PipeIdA2A = 1;
            const int PipeIdA2B = 2;
            const int PipeIdB2B = 3;
            const int PipeIdB2C = 4;
            const int PipeIdC2C = 5;

            var configA1 = new MediConfig
                {
                    InterceptLocalLogs = false,
                    Peers =
                        {
                            new ClientPeerConfig
                                {
                                    Transport = new PipeTransportClientConfig
                                                    {
                                                        ServerId = PipeIdA2A,
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
                                    ResourceDirectory = @"V:\A1\Resources"
                                }
                        }
                };

            var configA2 = new MediConfig
                {
                    InterceptLocalLogs = false,
                    Peers =
                        {
                            new ServerPeerConfig
                                {
                                    Transport = new PipeTransportServerConfig
                                                    {
                                                        ServerId = PipeIdA2A,
                                                        SessionDisconnectTimeout = 8000,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig()
                                },
                            new ClientPeerConfig
                                {
                                    Transport = new PipeTransportClientConfig
                                                    {
                                                        ServerId = PipeIdA2B,
                                                        ReconnectWait = 500,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig(),
                                    IsGateway = true
                                }
                        },
                    Services =
                        {
                            new RemoteResourceServiceConfig
                                {
                                    ResourceDirectory = @"V:\A2\Resources"
                                }
                        }
                };

            var configB1 = new MediConfig
            {
                InterceptLocalLogs = false,
                Peers =
                        {
                            new ServerPeerConfig
                                {
                                    Transport = new PipeTransportServerConfig
                                                    {
                                                        ServerId = PipeIdA2B,
                                                        SessionDisconnectTimeout = 8000,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig()
                                },
                            new ClientPeerConfig
                                {
                                    Transport = new PipeTransportClientConfig
                                                    {
                                                        ServerId = PipeIdB2B,
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
                                    ResourceDirectory = @"V:\B1\Resources"
                                }
                        }
            };

            var configB2 = new MediConfig
            {
                InterceptLocalLogs = false,
                Peers =
                        {
                            new ServerPeerConfig
                                {
                                    Transport = new PipeTransportServerConfig
                                                    {
                                                        ServerId = PipeIdB2B,
                                                        SessionDisconnectTimeout = 8000,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig()
                                },
                            new ClientPeerConfig
                                {
                                    Transport = new PipeTransportClientConfig
                                                    {
                                                        ServerId = PipeIdB2C,
                                                        ReconnectWait = 500,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig()
                                }
                        },
                Services =
                        {
                            new RemoteResourceServiceConfig
                                {
                                    ResourceDirectory = @"V:\B2\Resources"
                                }
                        }
            };

            var configC1 = new MediConfig
            {
                InterceptLocalLogs = false,
                Peers =
                        {
                            new ServerPeerConfig
                                {
                                    Transport = new PipeTransportServerConfig
                                                    {
                                                        ServerId = PipeIdB2C,
                                                        SessionDisconnectTimeout = 8000,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig()
                                },
                            new ClientPeerConfig
                                {
                                    Transport = new PipeTransportClientConfig
                                                    {
                                                        ServerId = PipeIdC2C,
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
                                    ResourceDirectory = @"V:\C1\Resources"
                                }
                        }
            };

            var configC2 = new MediConfig
            {
                InterceptLocalLogs = false,
                Peers =
                        {
                            new ServerPeerConfig
                                {
                                    Transport = new PipeTransportServerConfig
                                                    {
                                                        ServerId = PipeIdC2C,
                                                        SessionDisconnectTimeout = 8000,
                                                        IdleKeepAliveWait = -1
                                                    },
                                    Codec = new BecCodecConfig()
                                }
                        },
                Services =
                        {
                            new RemoteResourceServiceConfig
                                {
                                    ResourceDirectory = @"V:\C2\Resources"
                                }
                        }
            };

            var nodeA1 = new MediNode(new MediAddress("A", "1"));
            nodeA1.Configure(configA1);

            var nodeA2 = new MediNode(new MediAddress("A", "2"));
            nodeA2.Configure(configA2);

            var nodeB1 = new MediNode(new MediAddress("B", "1"));
            nodeB1.Configure(configB1);

            var nodeB2 = new MediNode(new MediAddress("B", "2"));
            nodeB2.Configure(configB2);

            var nodeC1 = new MediNode(new MediAddress("C", "1"));
            nodeC1.Configure(configC1);

            var nodeC2 = new MediNode(new MediAddress("C", "2"));
            nodeC2.Configure(configC2);

            var nodes = new List<MediNode> { nodeA1, nodeA2, nodeB1, nodeB2, nodeC1, nodeC2 };

            // we need to reverse the list since the servers must be created before the clients
            nodes.Reverse();
            return nodes;
        }
    }
}