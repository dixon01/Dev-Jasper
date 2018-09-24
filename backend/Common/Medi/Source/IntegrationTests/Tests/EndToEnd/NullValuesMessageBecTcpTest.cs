// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullValuesMessageBecTcpTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NullValuesMessageBecTcpTest type.
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
    /// one <see cref="ListMessage"/> in both directions. Both messages contain some null references.
    /// </summary>
    public class NullValuesMessageBecTcpTest : ClientServerMessageTest<ListMessage, ListMessage>
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            var msgCliSrv = new ListMessage();
            msgCliSrv.Integers = null;
            msgCliSrv.Messages = new[] { null, new SimpleMessage(33) };

            var msgSrvCli = new ListMessage();
            msgSrvCli.Integers = new List<int> { int.MinValue, -55, 243, 554, 128 };
            msgSrvCli.Messages = null;

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