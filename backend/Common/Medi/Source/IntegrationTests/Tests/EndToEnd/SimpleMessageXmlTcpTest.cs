// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleMessageXmlTcpTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleMessageXmlTcpTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Medi.IntegrationTests.Messages;

    /// <summary>
    /// Test that sets up a connection between a XML/TCP client and a XML/TCP server and then sends
    /// one <see cref="SimpleMessage"/> in both directions.
    /// </summary>
    public class SimpleMessageXmlTcpTest : ClientServerMessageTest<SimpleMessage, SimpleMessage>
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            this.Setup(
                new XmlCodecConfig(),
                new TcpTransportServerConfig(),
                new XmlCodecConfig(),
                new TcpTransportClientConfig(),
                new SimpleMessage(12),
                new SimpleMessage(100));
        }
    }
}