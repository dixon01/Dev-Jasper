// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListMessageBecTcpTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListMessageBecTcpTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Medi.IntegrationTests.Messages;

    /// <summary>
    /// Test that sets up a connection between a BEC/TCP client and a BEC/TCP server and then sends
    /// one <see cref="ListMessage"/> in both directions.
    /// </summary>
    public class ListMessageBecTcpTest : ClientServerMessageTest<ListMessage, ListMessage>
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            var msgCliSrv = new ListMessage();
            msgCliSrv.Integers = new List<int> { -1, 2, 5, 9, 42, int.MaxValue };
            msgCliSrv.Messages = new[] { new SimpleMessage(17), new SimpleMessage(33) };

            var msgSrvCli = new ListMessage();
            msgSrvCli.Integers = new List<int> { int.MinValue, -55, 243, 554, 128 };
            msgSrvCli.Messages = new[] { new SimpleMessage(-19), new SimpleMessage(1024) };

            this.Setup(
                new BecCodecConfig(),
                new TcpTransportServerConfig(),
                new BecCodecConfig(),
                new TcpTransportClientConfig(),
                msgCliSrv,
                msgSrvCli);
        }
    }
}