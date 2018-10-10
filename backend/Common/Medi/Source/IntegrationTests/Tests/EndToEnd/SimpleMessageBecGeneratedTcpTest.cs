// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleMessageBecGeneratedTcpTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleMessageBecGeneratedTcpTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Medi.IntegrationTests.Messages;

    /// <summary>
    /// Test that sets up a connection between a BEC/TCP client and a BEC/TCP server and then sends
    /// one <see cref="SimpleMessage"/> in both directions.
    /// Both client and server use the "generated" option of BEC.
    /// </summary>
    public class SimpleMessageBecGeneratedTcpTest : ClientServerMessageTest<SimpleMessage, SimpleMessage>
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            this.Setup(
                new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Generated },
                new TcpTransportServerConfig(),
                new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Generated },
                new TcpTransportClientConfig(),
                new SimpleMessage(12),
                new SimpleMessage(100));
        }
    }
}