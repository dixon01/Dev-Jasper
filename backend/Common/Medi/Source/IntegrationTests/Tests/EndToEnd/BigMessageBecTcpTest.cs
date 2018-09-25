// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BigMessageBecTcpTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BigMessageXmlTcpTest type.
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
    /// a message with a lot of data in both directions.
    /// </summary>
    public class BigMessageBecTcpTest : ClientServerMessageTest<TreeMessage, TreeMessage>
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            var msg1 = new TreeMessage();
            this.CreateTree(msg1, "First message", 4, 3);

            var msg2 = new TreeMessage();
            this.CreateTree(msg1, "Second message", 3, 4);

            this.Setup(
                new BecCodecConfig(),
                new TcpTransportServerConfig(),
                new BecCodecConfig(),
                new TcpTransportClientConfig(),
                msg1,
                msg2);
        }

        private void CreateTree(TreeMessage node, string prefix, int numLevels, int numChildren)
        {
            if (numLevels < 0)
            {
                return;
            }

            for (int i = 0; i < numChildren; i++)
            {
                var name = string.Format("{0}>child>{1}", prefix, numLevels);
                var child = new TreeMessage();
                var messages = new List<SimpleMessage>();
                child.List = new ListMessage();
                for (int j = 0; j < numLevels  + 1; j++)
                {
                    messages.Add(new SimpleMessage(j * i));
                    child.List.Integers.Add((numChildren * i) + j);
                }

                child.List.Messages = messages.ToArray();

                for (int j = 0; j < numChildren * numChildren * numLevels; j++)
                {
                    child.Strings.Add(new StringMessage(string.Format("{0}>[inner text]>{1}", name, j)));
                }

                node.Children.Add(child);
                this.CreateTree(child, name, numLevels - 1, numChildren);
            }
        }
    }
}