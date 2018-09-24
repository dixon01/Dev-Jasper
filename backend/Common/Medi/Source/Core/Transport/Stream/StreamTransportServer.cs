// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamTransportServer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamTransportServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Base class for stream transport servers.
    /// Subclasses usually only have to create a specific
    /// <see cref="IStreamFactory"/> implementation and provide it
    /// to the constructor.
    /// </summary>
    internal abstract class StreamTransportServer : TransportServer
    {
        private readonly IStreamFactory streamFactory;

        private readonly Dictionary<StreamSessionId, ClientSession> sessions =
            new Dictionary<StreamSessionId, ClientSession>();

        private StreamTransportServerConfig config;

        private IMessageDispatcherImpl dispatcher;

        private IStreamServer streamServer;

        private int nextSessionId;

        private MessageTranscoder messageTranscoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamTransportServer"/> class.
        /// </summary>
        /// <param name="streamFactory">
        /// The stream factory.
        /// </param>
        protected StreamTransportServer(IStreamFactory streamFactory)
        {
            this.streamFactory = streamFactory;
            this.nextSessionId = new Random().Next(0xFFFF);
        }

        /// <summary>
        /// Configures this transport server with the given config.
        /// </summary>
        /// <param name="cfg">
        /// The config.
        /// </param>
        public override void Configure(TransportServerConfig cfg)
        {
            var streamConfig = cfg as StreamTransportServerConfig;
            if (streamConfig == null)
            {
                throw new ArgumentException("StreamTransportServerConfig expected", "cfg");
            }

            this.config = streamConfig;
        }

        /// <summary>
        /// Starts the transport implementation, connecting it with the given codec.
        /// </summary>
        /// <param name="medi">
        ///     The local message dispatcher implementation
        /// </param>
        /// <param name="messageTrans">
        ///     The message transcoder that is on top of this transport.
        /// </param>
        public override void Start(IMessageDispatcherImpl medi, MessageTranscoder messageTrans)
        {
            if (this.config == null)
            {
                throw new NotSupportedException("StreamTransportServer has not been configured properly");
            }

            this.Stop();

            this.dispatcher = medi;
            this.messageTranscoder = messageTrans;

            this.streamServer = this.streamFactory.CreateServer(this.config);
            this.streamServer.BeginAccept(this.Accepted, null);

            this.Logger.Info("Server started: {0}", this.streamServer);

            this.RaiseStarted(EventArgs.Empty);
        }

        /// <summary>
        /// Stops the transport and releases all resources.
        /// </summary>
        public override void Stop()
        {
            if (this.streamServer == null)
            {
                return;
            }

            this.Logger.Info("Stopping server: {0}", this.streamServer);
            this.streamServer.Dispose();
            this.streamServer = null;

            base.Stop();
        }

        private void Accepted(IAsyncResult result)
        {
            var server = this.streamServer;
            if (server == null)
            {
                return;
            }

            IStreamConnection connection;
            try
            {
                connection = server.EndAccept(result);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Could not accept connection");
                return;
            }

            this.Logger.Info("Client connected: {0}", connection);

            server.BeginAccept(this.Accepted, null);

            // create a unique session id
            int id = Interlocked.Increment(ref this.nextSessionId);
            var sessionId = connection.CreateId() | (id << 16);
            var handshake = new StreamHandshake(this.messageTranscoder.Codec, this.dispatcher.LocalAddress, sessionId);
            handshake.Connected += this.HandshakeOnConnected;
            handshake.Failed += (s, e) => connection.Dispose();

            handshake.BeginConnectToClient(
                connection,
                sid =>
                    {
                        lock (this.sessions)
                        {
                            return this.sessions.ContainsKey(sid);
                        }
                    });
        }

        private void HandshakeOnConnected(object sender, HandshakeSuccessEventArgs e)
        {
            ClientSession session;
            bool newSession;
            lock (this.sessions)
            {
                newSession = !this.sessions.TryGetValue(e.SessionId, out session);
                if (newSession)
                {
                    this.Logger.Debug("Handshake successful: new session {0}", e.SessionId);
                    var frameController = (e.Features & StreamFeature.Framing) != 0 ? new FrameController() : null;
                    var isGateway = (e.Features & StreamFeature.Gateway) != 0;
                    var gatewayMode = isGateway ? GatewayMode.Server : GatewayMode.None;

                    session = new ClientSession(
                        e.SessionId, gatewayMode, frameController, this.config.SessionDisconnectTimeout);
                    this.sessions.Add(e.SessionId, session);

                    session.Disconnected += (s, ev) => this.sessions.Remove((StreamSessionId)session.SessionId);

                    session.Transport = new ClientMessageTransport(this.dispatcher) { IsGateway = isGateway };
                }
                else
                {
                    this.Logger.Debug("Handshake successful: existing session {0}", e.SessionId);

                    if (e.ChannelType == ChannelType.Message)
                    {
                        // we got reconnected, stop the timer
                        session.StopDisconnectTimer();
                    }
                    else
                    {
                        session.Transport.StartWritingStreams(e.Connection, e.AgreedCodec);
                        return;
                    }
                }
            }

            if (e.ChannelType != ChannelType.Message)
            {
                throw new NotSupportedException(e.ChannelType + " channel connected before message channel");
            }

            var transport = session.Transport;
            transport.StartWritingMessages(
                session, e.Connection, this.config.IdleKeepAliveWait, e.AgreedCodec, this.messageTranscoder);
            if (newSession)
            {
                this.AddClient(new Client(this, transport));
            }

            transport.RaiseConnected(EventArgs.Empty);
            if (newSession)
            {
                transport.RaiseSessionConnected(EventArgs.Empty);
            }

            transport.StreamFailed += (s, ev) => session.StartDisconnectTimer();
        }

        private class ClientSession : TransportSession
        {
            private static readonly Logger Logger = LogHelper.GetLogger<ClientSession>();

            private readonly ITimer disconnectTimer;

            public ClientSession(
                ISessionId sessionId, GatewayMode gatewayMode, IFrameController frameController, int disconnectTimeout)
                : base(sessionId, gatewayMode, frameController)
            {
                this.disconnectTimer = TimerFactory.Current.CreateTimer("StreamServerSessionDisconnect");
                this.disconnectTimer.Interval = TimeSpan.FromMilliseconds(disconnectTimeout);
                this.disconnectTimer.AutoReset = false;
                this.disconnectTimer.Elapsed += this.DisconnectTimerOnElapsed;
            }

            public ClientMessageTransport Transport { get; set; }

            public void StartDisconnectTimer()
            {
                this.disconnectTimer.Enabled = true;
            }

            public void StopDisconnectTimer()
            {
                this.disconnectTimer.Enabled = false;
            }

            private void DisconnectTimerOnElapsed(object s, EventArgs e)
            {
                Logger.Debug("Session expried, removing: {0}", this.SessionId);
                this.RaiseDisconnected(e);
            }
        }

        private class ClientMessageTransport : StreamMessageTransport, IClientMessageTransport
        {
            private readonly IMessageDispatcherImpl dispatcher;

            public ClientMessageTransport(IMessageDispatcherImpl dispatcher)
            {
                this.dispatcher = dispatcher;
            }

            public event EventHandler Connected;

            public event EventHandler SessionConnected;

            public event EventHandler Disconnected;

            ITransportSession IClientMessageTransport.Session
            {
                get
                {
                    return this.Session;
                }
            }

            protected override IResourceServiceImpl ResourceService
            {
                get
                {
                    return this.dispatcher.GetService<IResourceServiceImpl>();
                }
            }

            public void RaiseConnected(EventArgs e)
            {
                EventHandler handler = this.Connected;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            public void RaiseSessionConnected(EventArgs e)
            {
                EventHandler handler = this.SessionConnected;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            protected override void RaiseStreamFailed(EventArgs e)
            {
                base.RaiseStreamFailed(e);

                this.Stop();
            }

            protected override void Close()
            {
                base.Close();
                this.RaiseDisconnected(EventArgs.Empty);
            }

            private void RaiseDisconnected(EventArgs e)
            {
                EventHandler handler = this.Disconnected;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
    }
}