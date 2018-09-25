// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamHandshakeTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamHandshakeTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transport.Stream
{
    using System.Threading;

    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Transport.Stream;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="StreamHandshake"/>.
    /// </summary>
    [TestClass]
    public class StreamHandshakeTest
    {
        /// <summary>
        /// Tests a normal successful handshake.
        /// </summary>
        [TestMethod]
        public void TestSuccessfulHandshake()
        {
            var localAddress = new MediAddress("U", "A");
            var codec = new MockMessageCodec(new CodecIdentification('A', 1));

            IStreamServer server = new PipeServer(1);
            var client = new PipeClient((PipeServer)server);

            var serverEvent = new AutoResetEvent(false);
            var clientEvent = new AutoResetEvent(false);

            StreamSessionId serverId = null;
            StreamSessionId clientId = null;

            var serverHandshake = new StreamHandshake(codec, localAddress, 15);
            serverHandshake.Failed += (sender, args) => serverEvent.Set();
            serverHandshake.Connected += (sender, args) =>
            {
                serverId = args.SessionId;
                serverEvent.Set();
            };
            server.BeginAccept(ar => serverHandshake.BeginConnectToClient(server.EndAccept(ar), id => false), null);

            var clientHandshake = new StreamHandshake(codec, localAddress, 99);
            clientHandshake.Failed += (sender, args) => clientEvent.Set();
            clientHandshake.Connected += (sender, args) =>
            {
                clientId = args.SessionId;
                clientEvent.Set();
            };

            clientHandshake.BeginConnectToServer(client, ChannelType.Message, false);

            Assert.IsTrue(serverEvent.WaitOne(1000));
            Assert.IsTrue(clientEvent.WaitOne(1000));

            Assert.IsNotNull(serverId);
            Assert.IsNotNull(clientId);

            Assert.AreEqual(15, clientId.Id);
            Assert.AreEqual(15, serverId.Id);
        }

        /// <summary>
        /// Tests a failing handshake with a wrong codec config.
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestFailedHandshakeWrongCodec()
        {
            var localAddress = new MediAddress("U", "A");
            var clientCodec = new MockMessageCodec(new CodecIdentification('A', 1));
            var serverCodec = new MockMessageCodec(new CodecIdentification('B', 1));

            IStreamServer server = new PipeServer(1);
            var client = new PipeClient((PipeServer)server);

            var serverEvent = new AutoResetEvent(false);
            var clientEvent = new AutoResetEvent(false);

            var serverFailure = HandshakeError.Sucess;
            var clientFailure = HandshakeError.Sucess;

            var serverHandshake = new StreamHandshake(serverCodec, localAddress, 15);
            serverHandshake.Failed += (sender, args) =>
                {
                    serverFailure = args.Cause;
                    serverEvent.Set();
                };
            serverHandshake.Connected += (sender, args) => serverEvent.Set();
            server.BeginAccept(ar => serverHandshake.BeginConnectToClient(server.EndAccept(ar), id => false), null);

            var clientHandshake = new StreamHandshake(clientCodec, localAddress, 99);
            clientHandshake.Failed += (sender, args) =>
                {
                    clientFailure = args.Cause;
                    clientEvent.Set();
                };
            clientHandshake.Connected += (sender, args) => clientEvent.Set();

            clientHandshake.BeginConnectToServer(client, ChannelType.Message, false);

            Assert.IsTrue(serverEvent.WaitOne(1000));
            Assert.IsTrue(clientEvent.WaitOne(1000));

            Assert.AreEqual(HandshakeError.CodecNotSupported, clientFailure);
            Assert.AreEqual(HandshakeError.CodecNotSupported, serverFailure);
        }

        /// <summary>
        /// Tests a successful handshake when the client reconnects (known session ID)
        /// </summary>
        [TestMethod]
        public void TestSuccessfulHandshakeReconnect()
        {
            var localAddress = new MediAddress("U", "A");
            var codec = new MockMessageCodec(new CodecIdentification('A', 1));

            IStreamServer server = new PipeServer(1);
            var client = new PipeClient((PipeServer)server);

            var serverEvent = new AutoResetEvent(false);
            var clientEvent = new AutoResetEvent(false);

            StreamSessionId serverId = null;
            StreamSessionId clientId = null;

            var serverHandshake = new StreamHandshake(codec, localAddress, 15);
            serverHandshake.Failed += (sender, args) => serverEvent.Set();
            serverHandshake.Connected += (sender, args) =>
            {
                serverId = args.SessionId;
                serverEvent.Set();
            };
            server.BeginAccept(ar => serverHandshake.BeginConnectToClient(server.EndAccept(ar), id => true), null);

            var clientHandshake = new StreamHandshake(codec, localAddress, 99);
            clientHandshake.Failed += (sender, args) => clientEvent.Set();
            clientHandshake.Connected += (sender, args) =>
            {
                clientId = args.SessionId;
                clientEvent.Set();
            };

            clientHandshake.BeginConnectToServer(client, ChannelType.Message, false);

            Assert.IsTrue(serverEvent.WaitOne(1000));
            Assert.IsTrue(clientEvent.WaitOne(1000));

            Assert.IsNotNull(serverId);
            Assert.IsNotNull(clientId);

            Assert.AreEqual(99, clientId.Id);
            Assert.AreEqual(99, serverId.Id);
        }
    }
}
