// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlTcpConnectTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlTcpConnectTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Transport.Tcp;

    /// <summary>
    /// Test that sets up a connection between a XML/TCP client and a XML/TCP server.
    /// </summary>
    public class XmlTcpConnectTest : ConnectClientServerTest
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
                new TcpTransportClientConfig());
        }
    }
}
