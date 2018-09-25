// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSendTestBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSendTestBase type.
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
    public abstract class FileSendTestBase : MediIntegrationTest
    {
        /// <summary>
        /// The this.Logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSendTestBase"/> class.
        /// </summary>
        protected FileSendTestBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
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
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Integration test method.")]
        public override void Run()
        {
            const string ReceivedFromClientFilePath = "C:\\ReceivedFromClient.txt";
            const string ReceivedFromServerFilePath = "C:\\ReceivedFromServer.txt";
            var factory = this.CreateNodeFactory();
            var fileSystem = new TestingFileSystem();
            FileSystemManager.ChangeLocalFileSystem(fileSystem);

            // ID: 74ED340B3BB29D0EB04125F3AD0CEA86
            var clientFile = fileSystem.CreateFile("C:\\FromClient.txt");
            using (var writer = new StreamWriter(clientFile.OpenWrite()))
            {
                writer.Write("From client");
            }

            // ID: 355ABC62A0818D77CB3C6E6B13FA0418
            var serverFile = fileSystem.CreateFile("C:\\FromServer.txt");
            using (var writer = new StreamWriter(serverFile.OpenWrite()))
            {
                writer.Write("From server!");
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

                // allow the peers to be connected
                Thread.Sleep(connectWaitTime);

                var serverReceiveWait = new AutoResetEvent(false);
                FileReceivedEventArgs serverReceived = null;
                server.ResourceService.FileReceived += (s, e) =>
                    {
                        serverReceived = e;
                        e.CopyTo(ReceivedFromClientFilePath);
                        serverReceiveWait.Set();
                    };

                var clientReceiveWait = new AutoResetEvent(false);
                FileReceivedEventArgs clientReceived = null;
                client.ResourceService.FileReceived += (s, e) =>
                {
                    clientReceived = e;
                    e.CopyTo(ReceivedFromServerFilePath);
                    clientReceiveWait.Set();
                };

                // allow the subscriptions to be exchanged
                Thread.Sleep(connectWaitTime);

                // ==========================
                // SEND FROM SERVER TO CLIENT
                // ==========================
                this.Logger.Info("Sending {0} from server to client", serverFile.FullName);
                Assert.AreEqual(true, server.ResourceService.SendFile(serverFile.FullName, client.Address));

                // wait for the resource announcement to get to the other peer
                Assert.AreEqual(true, clientReceiveWait.WaitOne(transferWaitTime));
                clientReceiveWait.Reset();

                Assert.IsNotNull(clientReceived);
                Assert.AreEqual(serverFile.FullName, clientReceived.OriginalFileName);
                var clientRxFile = fileSystem.GetFile(ReceivedFromServerFilePath);
                Assert.IsNotNull(clientRxFile);
                Assert.AreEqual(12L, clientRxFile.Size);
                this.Logger.Info(
                    "File {0} sucessfully received from {1}", clientReceived.OriginalFileName, clientReceived.Source);

                Thread.Sleep(transferWaitTime);

                // ==========================
                // SEND FROM CLIENT TO SERVER
                // ==========================
                this.Logger.Info("Sending {0} from client to server", clientFile.FullName);
                Assert.AreEqual(true, client.ResourceService.SendFile(clientFile.FullName, server.Address));

                // wait for the resource announcement to get to the other peer
                Assert.AreEqual(true, serverReceiveWait.WaitOne(transferWaitTime));
                serverReceiveWait.Reset();

                Assert.IsNotNull(serverReceived);
                Assert.AreEqual(clientFile.FullName, serverReceived.OriginalFileName);
                var serverRxFile = fileSystem.GetFile(ReceivedFromClientFilePath);
                Assert.IsNotNull(serverRxFile);
                Assert.AreEqual(11L, serverRxFile.Size);
                this.Logger.Info(
                    "File {0} sucessfully received from {1}", serverReceived.OriginalFileName, serverReceived.Source);
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

        /// <summary>
        /// Creates the node factory used by this test.
        /// </summary>
        /// <returns>
        /// The <see cref="INodeFactory"/>.
        /// </returns>
        internal abstract INodeFactory CreateNodeFactory();
    }
}