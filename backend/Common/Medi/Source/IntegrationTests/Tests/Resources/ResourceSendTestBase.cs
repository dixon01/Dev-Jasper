// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceSendTestBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceSendTestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.IntegrationTests.Tests.Streams;
    using Gorba.Common.Medi.IntegrationTests.Utils;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.FilesTesting;

    using NLog;

    /// <summary>
    /// The resource send test base.
    /// </summary>
    public abstract class ResourceSendTestBase : MediIntegrationTest
    {
        /// <summary>
        /// The this.Logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSendTestBase"/> class.
        /// </summary>
        protected ResourceSendTestBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets or sets the minimum file size.
        /// </summary>
        protected int MinimumFileSize { get; set; }

        /// <summary>
        /// Gets or sets the expected transfer delay.
        /// </summary>
        protected TimeSpan ExpectedTransferDelay { get; set; }

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
            var fileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(fileSystem);

            var clientFile = fileSystem.CreateFile("C:\\FromClient.txt");
            using (var writer = new StreamWriter(clientFile.OpenWrite()))
            {
                writer.Write("From client");
                this.FillFile(writer);
            }

            var serverFile = fileSystem.CreateFile("C:\\FromServer.txt");
            using (var writer = new StreamWriter(serverFile.OpenWrite()))
            {
                writer.Write("From server!");
                this.FillFile(writer);
            }

            var nodes = factory.CreateNodes();

            foreach (var node in nodes)
            {
                node.Start();
            }

            try
            {
                var server = nodes.First();
                var client = nodes.Last();

                this.Logger.Info("Started all dispatchers");

                var connectWaitTime = TimeSpan.FromMilliseconds(500 * Math.Pow(1.5, nodes.Count));
                var transferWaitTime = TimeSpan.FromMilliseconds(1000 * nodes.Count);
                var fileTransferWaitTime = transferWaitTime
                                           + TimeSpan.FromMilliseconds(this.MinimumFileSize / 10 * nodes.Count)
                                           + this.ExpectedTransferDelay;

                // allow the peers to be connected
                Thread.Sleep(connectWaitTime);

                var serverReceiveWait = new AutoResetEvent(false);
                server.Dispatcher.Subscribe<ResourceSentInfo>((s, e) => serverReceiveWait.Set());

                var clientReceiveWait = new AutoResetEvent(false);
                client.Dispatcher.Subscribe<ResourceSentInfo>((s, e) => clientReceiveWait.Set());

                // allow the subscriptions to be exchanged
                Thread.Sleep(connectWaitTime);

                this.Logger.Info("Registering resources");

                var serverRid = server.ResourceService.RegisterResource(serverFile.FullName, false);
                if (this.MinimumFileSize <= 0)
                {
                    Assert.AreEqual(new ResourceId("355ABC62A0818D77CB3C6E6B13FA0418"), serverRid);
                }

                var serverRes = server.ResourceService.GetResource(serverRid);
                Assert.IsNotNull(serverRes);
                Assert.AreEqual(serverRid, serverRes.Id);
                Assert.AreEqual(serverFile.Size, serverRes.Size);

                var clientRid = client.ResourceService.RegisterResource(clientFile.FullName, false);
                if (this.MinimumFileSize <= 0)
                {
                    Assert.AreEqual(new ResourceId("74ED340B3BB29D0EB04125F3AD0CEA86"), clientRid);
                }

                var clientRes = client.ResourceService.GetResource(clientRid);
                Assert.IsNotNull(clientRes);
                Assert.AreEqual(clientRid, clientRes.Id);
                Assert.AreEqual(clientFile.Size, clientRes.Size);

                if (client.Address.Unit != server.Address.Unit)
                {
                    try
                    {
                        // the server shouldn't contain the client resource
                        server.ResourceService.GetResource(clientRid);
                        throw new IntegrationTestException("Expected ApplicationException to be thrown");
                    }
                    catch (ApplicationException)
                    {
                    }
                }

                Thread.Sleep(connectWaitTime);

                // ==========================
                // SEND FROM SERVER TO CLIENT
                // ==========================
                this.Logger.Info("Sending {0} from server to client", serverRes.Id);
                Assert.AreEqual(true, server.ResourceService.SendResource(serverRes, client.Address));
                server.Dispatcher.Send(client.Address, new ResourceSentInfo(serverRes.Id));

                // wait for the resource announcement to get to the other peer
                Assert.AreEqual(true, clientReceiveWait.WaitOne(transferWaitTime));
                clientReceiveWait.Reset();

                var getResource = client.ResourceService.BeginGetResource(serverRid, null, null);
                Assert.AreEqual(true, getResource.AsyncWaitHandle.WaitOne(fileTransferWaitTime));
                var receivedResource = client.ResourceService.EndGetResource(getResource);

                Assert.IsNotNull(receivedResource);
                Assert.AreEqual(serverRid, receivedResource.Id);
                Assert.AreEqual(serverRes.Size, receivedResource.Size);
                this.Logger.Info("Resource {0} sucessfully received", receivedResource.Id);

                Thread.Sleep(transferWaitTime);

                // ==========================
                // SEND FROM CLIENT TO SERVER
                // ==========================
                this.Logger.Info("Sending {0} from client to server", clientRes.Id);
                Assert.AreEqual(true, client.ResourceService.SendResource(clientRes, server.Address));
                client.Dispatcher.Send(server.Address, new ResourceSentInfo(clientRes.Id));

                // wait for the resource announcement to get to the other peer
                Assert.AreEqual(true, serverReceiveWait.WaitOne(transferWaitTime));
                serverReceiveWait.Reset();

                // this will block until we completely received the resource
                getResource = server.ResourceService.BeginGetResource(clientRid, null, null);
                Assert.AreEqual(true, getResource.AsyncWaitHandle.WaitOne(fileTransferWaitTime));
                receivedResource = server.ResourceService.EndGetResource(getResource);

                Assert.IsNotNull(receivedResource);
                Assert.AreEqual(clientRid, receivedResource.Id);
                Assert.AreEqual(clientRes.Size, receivedResource.Size);
                this.Logger.Info("Resource {0} sucessfully received", receivedResource.Id);
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

        private void FillFile(StreamWriter writer)
        {
            writer.Flush();

            writer.AutoFlush = true;
            for (int i = 0; writer.BaseStream.Length < this.MinimumFileSize; i++)
            {
                writer.Write((char)(0x20 + (i % 0x5F)));
            }
        }
    }
}