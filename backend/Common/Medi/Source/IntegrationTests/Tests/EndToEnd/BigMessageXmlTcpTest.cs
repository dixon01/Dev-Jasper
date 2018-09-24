// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BigMessageXmlTcpTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BigMessageXmlTcpTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Medi.IntegrationTests.Messages;

    /// <summary>
    /// Test that sets up a connection between a XML/TCP client and a XML/TCP server and then sends
    /// one <see cref="ListMessage"/> with a lot of data in both directions.
    /// </summary>
    public class BigMessageXmlTcpTest : ClientServerMessageTest<ListMessage, ListMessage>
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            var msgs = new List<SimpleMessage>(200);
            var ints = new List<int>(200);
            for (int i = 0; i < msgs.Capacity; i++)
            {
                var n = i * i * i * i;
                ints.Add(n);
                msgs.Add(new SimpleMessage(n * 2));
            }

            var msg1 = new ListMessage { Integers = ints, Messages = msgs.ToArray() };

            // we have to modify the second message, otherwise the test fails
            msgs.Reverse();
            var msg2 = new ListMessage { Integers = ints, Messages = msgs.ToArray() };

            this.Setup(
                new XmlCodecConfig(),
                new TcpTransportServerConfig(),
                new XmlCodecConfig(),
                new TcpTransportClientConfig(),
                msg1,
                msg2);
        }
    }
}