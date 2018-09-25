// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecTcpConnectTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecTcpConnectTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;

    /// <summary>
    /// Test that sets up a connection between a BEC/TCP client and a BEC/TCP server.
    /// </summary>
    public class BecTcpConnectTest : ConnectClientServerTest
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            this.Setup(
                new BecCodecConfig(),
                new TcpTransportServerConfig(),
                new BecCodecConfig(),
                new TcpTransportClientConfig());
        }
    }
}