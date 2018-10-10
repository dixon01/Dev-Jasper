// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlDynamicTcpConnectTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlDynamicTcpConnectTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using Gorba.Common.Medi.Core.Transcoder.Dynamic;
    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Transport.Tcp;

    /// <summary>
    /// Test that sets up a connection between a XML/TCP client and a Dynamic/TCP server.
    /// </summary>
    public class XmlDynamicTcpConnectTest : ConnectClientServerTest
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            this.Setup(
                new DynamicCodecConfig(),
                new TcpTransportServerConfig(),
                new XmlCodecConfig(),
                new TcpTransportClientConfig());
        }
    }
}