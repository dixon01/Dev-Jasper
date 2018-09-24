// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecDynamicTcpConnectTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecDynamicTcpConnectTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transcoder.Dynamic;
    using Gorba.Common.Medi.Core.Transport.Tcp;

    /// <summary>
    /// Test that sets up a connection between a BEC/TCP client and a Dynamic/TCP server.
    /// </summary>
    public class BecDynamicTcpConnectTest : ConnectClientServerTest
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            this.Setup(
                new DynamicCodecConfig(),
                new TcpTransportServerConfig(),
                new BecCodecConfig(),
                new TcpTransportClientConfig());
        }
    }
}