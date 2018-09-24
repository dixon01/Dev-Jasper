// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientServerMessageTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClientServerMessageTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.EndToEnd
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;
    using System.Threading;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Tests.Utils;
    using Gorba.Common.Medi.IntegrationTests.Utils;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Base class for all tests that are testing client-server communication
    /// using multiple MessageDispatcher instances running in different AppDomains.
    /// </summary>
    /// <typeparam name="TMsgCliSrv">
    /// The type of the message to be sent from the client to the server.
    /// </typeparam>
    /// <typeparam name="TMsgSrvCli">
    /// The type of the message to be sent from the server to the client.
    /// </typeparam>
    public abstract class ClientServerMessageTest<TMsgCliSrv, TMsgSrvCli> : MediIntegrationTest
        where TMsgCliSrv : class
        where TMsgSrvCli : class
    {
        private readonly List<MessageEventArgs> receivedMessages = new List<MessageEventArgs>();
        private readonly AutoResetEvent receiveWait = new AutoResetEvent(false);

        private MediConfig serverConfig;
        private MediConfig clientConfig;

        private TMsgCliSrv msgCliSrv;
        private TMsgSrvCli msgSrvCli;

        private Exception remoteException;

        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        public override void Run()
        {
            const string UnitName = "U";
            const string ServerName = "Server";
            const string ClientName = "Client";
            const int ConnectWaitTime = 750;
            const int MessageWaitTime = 5000;
            const int SubscriptionWaitTime = 200;

            var server = this.CreateDomainWithMedi(UnitName, ServerName, this.serverConfig);
            server.Start();

            // wait so the server gets a chance to start before we launch the client
            Thread.Sleep(ConnectWaitTime);

            var client = this.CreateDomainWithMedi(UnitName, ClientName, this.clientConfig);
            client.Start();

            // wait so the client gets a chance to connect before we continue
            Thread.Sleep(ConnectWaitTime);

            if (this.msgCliSrv != null)
            {
                server.Subscribe<TMsgCliSrv>();

                // give the subscription a chance to be sent to the peer
                Thread.Sleep(SubscriptionWaitTime);

                client.BroadcastMessage(this.msgCliSrv);

                // wait for the message
                this.receiveWait.WaitOne(MessageWaitTime);

                server.Unsubscribe<TMsgCliSrv>();

                // wait for the message
                Thread.Sleep(SubscriptionWaitTime);
            }

            if (this.msgSrvCli != null)
            {
                client.Subscribe<TMsgSrvCli>();

                // give the subscription a chance to be sent to the peer
                Thread.Sleep(SubscriptionWaitTime);

                server.BroadcastMessage(this.msgSrvCli);

                this.receiveWait.WaitOne(MessageWaitTime);
            }

            server.UnhandledException -= this.ReceiverUnhandledException;
            server.MessageReceived -= this.ReceiverMessageReceived;
            client.UnhandledException -= this.ReceiverUnhandledException;
            client.MessageReceived -= this.ReceiverMessageReceived;

            server.Stop();
            client.Stop();

            var receivedCliSrv = this.receivedMessages.FirstOrDefault(e => e.Message.Equals(this.msgCliSrv));
            var receivedSrvCli = this.receivedMessages.FirstOrDefault(e => e.Message.Equals(this.msgSrvCli));

            if (this.msgCliSrv != null)
            {
                Assert.IsNotNull(receivedCliSrv);
                Assert.AreEqual(receivedCliSrv.Message, this.msgCliSrv);
                Assert.AreEqual(receivedCliSrv.Source.Application, ClientName);
                Assert.AreEqual(receivedCliSrv.Source.Unit, UnitName);
            }

            if (this.msgSrvCli != null)
            {
                Assert.IsNotNull(receivedSrvCli);
                Assert.AreEqual(receivedSrvCli.Message, this.msgSrvCli);
                Assert.AreEqual(receivedSrvCli.Source.Application, ServerName);
                Assert.AreEqual(receivedSrvCli.Source.Unit, UnitName);
            }
        }

        /// <summary>
        /// Cleans up all test resources.
        /// </summary>
        public override void Teardown()
        {
            if (this.remoteException != null)
            {
                throw new IntegrationTestException("Unhandled exception in AppDomain", this.remoteException);
            }
        }

        /// <summary>
        /// Sets up this test.
        /// </summary>
        /// <param name="serverCodec">
        /// The codec configuration for the server.
        /// </param>
        /// <param name="transportServer">
        /// The transport server config.
        /// </param>
        /// <param name="clientCodec">
        /// The codec configuration for the client.
        /// </param>
        /// <param name="transportClient">
        /// The transport client config.
        /// </param>
        /// <param name="msgClientServer">
        /// The message to be sent from the client to the server.
        /// </param>
        /// <param name="msgServerClient">
        /// The message to be sent from the server to the client.
        /// </param>
        protected void Setup(
            CodecConfig serverCodec,
            TransportServerConfig transportServer,
            CodecConfig clientCodec,
            TransportClientConfig transportClient,
            TMsgCliSrv msgClientServer,
            TMsgSrvCli msgServerClient)
        {
            this.remoteException = null;
            this.receivedMessages.Clear();

            this.serverConfig = new MediConfig { InterceptLocalLogs = false };
            this.serverConfig.Peers.Add(new ServerPeerConfig { Codec = serverCodec, Transport = transportServer });

            this.clientConfig = new MediConfig { InterceptLocalLogs = false };
            this.clientConfig.Peers.Add(new ClientPeerConfig { Codec = clientCodec, Transport = transportClient });

            this.msgCliSrv = msgClientServer;
            this.msgSrvCli = msgServerClient;
        }

        private AppDomainMediReceiver CreateDomainWithMedi(string unitName, string appName, MediConfig config)
        {
            var serializer = new XmlSerializer(typeof(MediConfig));
            var writer = new StringWriter();
            serializer.Serialize(writer, config);
            var configXml = writer.ToString();

            var appBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var appDomain = AppDomain.CreateDomain(appName, new Evidence(), appBasePath, ".", true);

            var loaderProxy = appDomain.CreateInstanceAndUnwrap<AssemblyLoaderHelper>();

            loaderProxy.Load(typeof(MessageDispatcher).Assembly.Location);
            loaderProxy.Load(typeof(ProducerConsumerQueue<>).Assembly.Location);

            var proxy = appDomain.CreateInstanceAndUnwrap<AppDomainMediProxy>();

            proxy.Configure(configXml, unitName, appName);

            var receiver = new AppDomainMediReceiver(proxy, appDomain);
            receiver.UnhandledException += this.ReceiverUnhandledException;
            receiver.MessageReceived += this.ReceiverMessageReceived;
            return receiver;
        }

        private void ReceiverMessageReceived(object sender, MessageEventArgs e)
        {
            this.receivedMessages.Add(e);
            this.receiveWait.Set();
        }

        private void ReceiverUnhandledException(object sender, ExceptionEventArgs e)
        {
            if (this.remoteException == null)
            {
                this.remoteException = e.Exception;
            }
        }
    }
}