// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayRoutingTestBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayRoutingTestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Gateway
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using Gorba.Common.Medi.Core.Tests.Messages;
    using Gorba.Common.Medi.IntegrationTests.Tests.Resources;
    using Gorba.Common.Medi.IntegrationTests.Tests.Streams;
    using Gorba.Common.Medi.IntegrationTests.Utils;
    using Gorba.Common.Utility.Core;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Base class for tests that send messages in an environment with a gateway.
    /// </summary>
    public abstract class GatewayRoutingTestBase : MediIntegrationTest
    {
        /// <summary>
        /// The this.Logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayRoutingTestBase"/> class.
        /// </summary>
        protected GatewayRoutingTestBase()
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
        }

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
        /// <param name="factory">
        /// The factory that creates the <see cref="MediNode"/>s.
        /// </param>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Integration test method.")]
        internal void Run(INodeFactory factory)
        {
            var nodes = factory.CreateNodes();

            foreach (var node in nodes)
            {
                // remove the resource service from the config
                node.Config.Services.Clear();
                node.Start();
            }

            try
            {
                var server = nodes.First();
                var client = nodes.Last();

                this.Logger.Info("Started all dispatchers");

                var connectWaitTime = TimeSpan.FromMilliseconds(500 * Math.Pow(1.5, nodes.Count));
                var transferWaitTime = TimeSpan.FromMilliseconds(1000 * nodes.Count);

                // allow the peers to be connected
                Thread.Sleep(connectWaitTime);

                var serverReceiveWait = new AutoResetEvent(false);
                Hello serverReceived = null;
                server.Dispatcher.Subscribe<Hello>(
                    (s, e) =>
                        {
                            serverReceived = e.Message;
                            serverReceiveWait.Set();
                        });

                var clientReceiveWait = new AutoResetEvent(false);
                Hello clientReceived = null;
                client.Dispatcher.Subscribe<Hello>(
                    (s, e) =>
                        {
                            clientReceived = e.Message;
                            clientReceiveWait.Set();
                        });

                // allow the subscriptions to be exchanged
                Thread.Sleep(connectWaitTime);

                // ==========================
                // SEND FROM SERVER TO CLIENT
                // ==========================
                var hello = new Hello();
                this.Logger.Info("Sending {0} from server to client", hello);
                server.Dispatcher.Send(client.Address, hello);

                // wait for the message to get to the other peer
                Assert.AreEqual(true, clientReceiveWait.WaitOne(transferWaitTime));
                Assert.IsNotNull(clientReceived);
                clientReceived = null;
                clientReceiveWait.Reset();

                this.Logger.Info("Message {0} successfully recieved", hello);

                Thread.Sleep(transferWaitTime);

                // ==========================
                // SEND FROM CLIENT TO SERVER
                // ==========================
                hello = new Hello();
                this.Logger.Info("Sending {0} from server to client", hello);
                client.Dispatcher.Send(server.Address, hello);

                // wait for the message to get to the other peer
                Assert.AreEqual(true, serverReceiveWait.WaitOne(transferWaitTime));
                Assert.IsNotNull(serverReceived);
                serverReceived = null;
                serverReceiveWait.Reset();

                this.Logger.Info("Message {0} successfully recieved", hello);

                Thread.Sleep(transferWaitTime);

                // =====================
                // BROADCAST FROM SERVER
                // =====================
                hello = new Hello();
                this.Logger.Info("Broadcasting {0} from server", hello);
                server.Dispatcher.Broadcast(hello);

                // wait for the message to get to the other peer
                Assert.AreEqual(true, serverReceiveWait.WaitOne(transferWaitTime));
                Assert.AreEqual(false, clientReceiveWait.WaitOne(transferWaitTime));
                Assert.IsNotNull(serverReceived);
                Assert.IsNull(clientReceived);
                serverReceived = null;
                serverReceiveWait.Reset();

                this.Logger.Info("Broadcast message {0} successfully recieved on server", hello);

                Thread.Sleep(transferWaitTime);

                // =====================
                // BROADCAST FROM CLIENT
                // =====================
                hello = new Hello();
                this.Logger.Info("Broadcasting {0} from client", hello);
                client.Dispatcher.Broadcast(hello);

                // wait for the message to get to the other peer
                Assert.AreEqual(true, clientReceiveWait.WaitOne(transferWaitTime));
                Assert.AreEqual(false, serverReceiveWait.WaitOne(transferWaitTime));
                Assert.IsNotNull(clientReceived);
                Assert.IsNull(serverReceived);
                clientReceived = null;
                clientReceiveWait.Reset();

                this.Logger.Info("Broadcast message {0} successfully recieved on client", hello);
            }
            finally
            {
                this.Logger.Info("Stopping all nodes");
                foreach (var node in nodes)
                {
                    node.Stop();
                }
            }
        }
    }
}