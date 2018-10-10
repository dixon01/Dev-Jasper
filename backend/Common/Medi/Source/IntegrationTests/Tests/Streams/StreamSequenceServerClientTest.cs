// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSequenceServerClientTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Test that connects two Medi stacks using pipes and
//   tests what happens when the connection is interrupted.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Streams
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Medi.IntegrationTests.Messages;
    using Gorba.Common.Medi.IntegrationTests.Utils;

    /// <summary>
    /// Test that connects two Medi stacks using pipes and
    /// tests what happens when the connection is interrupted.
    /// </summary>
    public class StreamSequenceServerClientTest : MediIntegrationTest
    {
        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public override void Setup()
        {
            base.Setup();
            PipeStreamFactory.Instance.Reset();
        }

        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test code")]
        public override void Run()
        {
            const int PipeId = 1;

            var clientConfig = new ClientPeerConfig();
            clientConfig.Transport = new PipeTransportClientConfig
                                         {
                                             ServerId = PipeId,
                                             ReconnectWait = 200,
                                             IdleKeepAliveWait = -1
                                         };
            clientConfig.Codec = new XmlCodecConfig();

            var serverConfig = new ServerPeerConfig();
            serverConfig.Transport = new PipeTransportServerConfig
                                         {
                                             ServerId = PipeId,
                                             SessionDisconnectTimeout = 8000
                                         };
            serverConfig.Codec = new XmlCodecConfig();

            var serverDispatcher =
                MessageDispatcher.Create(
                    new ObjectConfigurator(
                        new MediConfig { InterceptLocalLogs = false, Peers = { serverConfig } },
                        "U",
                        "Server"));

            var clientDispatcher =
                MessageDispatcher.Create(
                    new ObjectConfigurator(
                        new MediConfig { InterceptLocalLogs = false, Peers = { clientConfig } },
                        "U",
                        "Client"));

            var receivedMessages = new List<SimpleMessage>();
            clientDispatcher.Subscribe<SimpleMessage>((s, e) => receivedMessages.Add(e.Message));

            // let the subscription pass through the connection
            Thread.Sleep(5000);

            int counter;
            for (counter = 0; counter < 15; counter++)
            {
                serverDispatcher.Broadcast(new SimpleMessage(counter));

                // let the subscription pass through the connection
                for (int j = 0; j < 100 && receivedMessages.Count == 0; j++)
                {
                    Thread.Sleep(100);
                }

                Assert.AreEqual(1, receivedMessages.Count);
                Assert.AreEqual(counter, receivedMessages[0].Value);
                receivedMessages.Clear();

                if ((counter % 3) == 2)
                {
                    // reset the connection manually
                    foreach (IStreamClient client in PipeStreamFactory.Instance[PipeId].Clients)
                    {
                        client.Dispose();
                    }
                }
            }

            clientDispatcher.Dispose();
            serverDispatcher.Broadcast(new SimpleMessage(counter++));

            // wait for the session to expire
            Thread.Sleep(12000);

            clientDispatcher =
                MessageDispatcher.Create(
                    new ObjectConfigurator(
                        new MediConfig { InterceptLocalLogs = false, Peers = { clientConfig } },
                        "U",
                        "Client"));
            clientDispatcher.Subscribe<SimpleMessage>((s, e) => receivedMessages.Add(e.Message));

            // let the subscription pass through the connection
            Thread.Sleep(5000);

            serverDispatcher.Broadcast(new SimpleMessage(counter));

            // let the subscription pass through the connection
            for (int j = 0; j < 100 && receivedMessages.Count == 0; j++)
            {
                Thread.Sleep(100);
            }

            Assert.AreEqual(1, receivedMessages.Count);
            Assert.AreEqual(counter, receivedMessages[0].Value);
            receivedMessages.Clear();

            serverDispatcher.Dispose();
            clientDispatcher.Dispose();
        }
    }
}