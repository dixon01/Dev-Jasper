// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectClientServerTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectClientServerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// Base class for tests that connect a server with a client but don't send any actual messages
    /// </summary>
    public abstract class ConnectClientServerTest : ClientServerMessageTest<object, object>
    {
        /// <summary>
        /// Sets up this test.
        /// </summary>
        /// <param name="serverCodec">
        /// The codec configuration for the server.
        /// </param>
        /// <param name="transportServer">
        /// The transport server config.
        /// </param>
        /// <param name="clientCodec">
        /// The codec configuration for the client.
        /// </param>
        /// <param name="transportClient">
        /// The transport client config.
        /// </param>
        protected void Setup(
            CodecConfig serverCodec,
            TransportServerConfig transportServer,
            CodecConfig clientCodec,
            TransportClientConfig transportClient)
        {
            this.Setup(serverCodec, transportServer, clientCodec, transportClient, null, null);
        }
    }
}