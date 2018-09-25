// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamTransportClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamTransportClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;
    using System.IO;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Transport.Stream.Messages;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Common.Utility.Core.IO;

    /// <summary>
    /// Base class for stream transport clients.
    /// Subclasses usually only have to create a specific
    /// <see cref="IStreamFactory"/> implementation and provide it
    /// to the constructor.
    /// </summary>
    internal abstract class StreamTransportClient : StreamMessageTransport, ITransportClient
    {
        private readonly IStreamFactory streamFactory;

        private readonly ITimer reconnectTimer = TimerFactory.Current.CreateTimer("StreamTransportClientReconnect");

        private readonly ITimer streamConnectTimer =
            TimerFactory.Current.CreateTimer("StreamTransportClientStreamConnect");

        private readonly ITimer streamDisconnectTimer =
            TimerFactory.Current.CreateTimer("StreamTransportClientStreamDisconnect");

        private StreamTransportClientConfig config;

        private IMessageDispatcherImpl dispatcher;

        private MessageTranscoder messageTranscoder;

        private bool running;

        private SimpleAsyncResult<int> reconnectWaitingReadMessageRequest;
        private SimpleAsyncResult<int> reconnectWaitingReadStreamRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamTransportClient"/> class.
        /// </summary>
        /// <param name="streamFactory">
        /// The stream factory.
        /// </param>
        protected StreamTransportClient(IStreamFactory streamFactory)
        {
            this.streamFactory = streamFactory;

            this.reconnectTimer.AutoReset = false;
            this.reconnectTimer.Elapsed += this.ReconnectTimerOnElapsed;

            this.streamConnectTimer.AutoReset = false;
            this.streamConnectTimer.Interval = TimeSpan.FromSeconds(2);
            this.streamConnectTimer.Elapsed += this.StreamConnectTimerOnElapsed;

            this.streamDisconnectTimer.AutoReset = false;
            this.streamDisconnectTimer.Interval = TimeSpan.FromSeconds(60);
            this.streamDisconnectTimer.Elapsed += this.StreamDisconnectTimerOnElapsed;
        }

        /// <summary>
        /// Event that is fired when the transport has finished starting up.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Event that is fired when a session has been connected.
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionConnected;

        /// <summary>
        /// Gets the resource service used by this transport.
        /// </summary>
        protected override IResourceServiceImpl ResourceService
        {
            get
            {
                return this.dispatcher.GetService<IResourceServiceImpl>();
            }
        }

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="clientConfig">
        /// The config object.
        /// </param>
        public virtual void Configure(TransportClientConfig clientConfig)
        {
            var streamConfig = clientConfig as StreamTransportClientConfig;
            if (streamConfig == null)
            {
                throw new ArgumentException("StreamTransportClientConfig expected", "clientConfig");
            }

            this.config = streamConfig;

            this.reconnectTimer.Interval = TimeSpan.FromMilliseconds(this.config.ReconnectWait);
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
        public void Start(IMessageDispatcherImpl medi, MessageTranscoder messageTrans)
        {
            if (this.config == null)
            {
                throw new NotSupportedException("StreamTransportClient has not been configured properly");
            }

            this.dispatcher = medi;
            this.messageTranscoder = messageTrans;
            this.Connect();
        }

        /// <summary>
        /// Stops this transport.
        /// </summary>
        public override void Stop()
        {
            this.running = false;
            this.reconnectTimer.Enabled = false;
            base.Stop();
        }

        /// <summary>
        /// Asynchronously reads a message from the transport.
        /// </summary>
        /// <param name="bufferProvider">
        /// The provider for the buffer to be filled.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// An async result that can be used in <see cref="MessageTransport.EndReadMessage"/>.
        /// </returns>
        public override IAsyncResult BeginReadMessage(
            IReadBufferProvider bufferProvider, AsyncCallback callback, object state)
        {
            this.reconnectWaitingReadMessageRequest = null;
            try
            {
                return base.BeginReadMessage(bufferProvider, callback, state);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Could not begin read message from " + this.SessionId);
            }

            // we failed reading the message, now let's close this connection and hope we get reconnected
            this.Close();
            this.reconnectWaitingReadMessageRequest =
                new SimpleAsyncResult<int>(ar => this.BeginReadMessage(bufferProvider, callback, state), state);
            this.RaiseStreamFailed(EventArgs.Empty);
            return this.reconnectWaitingReadMessageRequest;
        }

        /// <summary>
        /// Asynchronously reads a stream from the transport.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// An async result that can be used in <see cref="IMessageTransport.EndReadStream"/>.
        /// </returns>
        public override IAsyncResult BeginReadStream(AsyncCallback callback, object state)
        {
            this.reconnectWaitingReadStreamRequest = null;
            try
            {
                return base.BeginReadStream(callback, state);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Could not begin read stream from " + this.SessionId);
            }

            // we failed reading the stream, now let's close this connection and hope we get reconnected
            this.Close();
            this.reconnectWaitingReadStreamRequest =
                new SimpleAsyncResult<int>(ar => this.BeginReadStream(callback, state), state);
            this.RaiseStreamFailed(EventArgs.Empty);
            return this.reconnectWaitingReadStreamRequest;
        }

        /// <summary>
        /// Previews a received and decoded message.
        /// This can be used to handle and/or change any received message.
        /// If you set <see cref="message"/> to null, handling of the message
        /// will be stopped.
        /// </summary>
        /// <param name="session">
        /// The session through which the message was received.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public override void PreviewDecodedMessage(ITransportSession session, ref MediMessage message)
        {
            if (!this.running)
            {
                return;
            }

            this.CheckForStreamSocket(message.Payload);

            base.PreviewDecodedMessage(session, ref message);
        }

        /// <summary>
        /// Closes the underlying network stream.
        /// </summary>
        protected override void Close()
        {
            this.streamDisconnectTimer.Enabled = false;
            this.streamConnectTimer.Enabled = false;
            this.reconnectTimer.Enabled = false;
            base.Close();
        }

        /// <summary>
        /// Sends an <see cref="IInternalMessage"/> to the connected remote peer.
        /// </summary>
        /// <param name="message">
        /// The message to be sent.
        /// </param>
        protected override void SendInternalMessage(IInternalMessage message)
        {
            this.CheckForStreamSocket(message);

            base.SendInternalMessage(message);
        }

        /// <summary>
        /// Fires the <see cref="StreamMessageTransport.StreamFailed"/> event
        /// and tries to reconnect to the server.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseStreamFailed(EventArgs e)
        {
            base.RaiseStreamFailed(e);

            this.StartReconnectTimer();
        }

        /// <summary>
        /// Fires the <see cref="Started"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected void RaiseStarted(EventArgs e)
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseSessionConnected(SessionEventArgs e)
        {
            var handler = this.SessionConnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void CheckForStreamSocket(object message)
        {
            if (this.Session.LocalGatewayMode == GatewayMode.None)
            {
                return;
            }

            var resourceStateResponse = message as ResourceStateResponse;
            if (resourceStateResponse == null || resourceStateResponse.ResourceStatus.IsAvailableCompletely)
            {
                return;
            }

            // when we receive or send a ResourceStateResponse, we need to check
            // if we need to (re)open the stream connection
            if (this.streamDisconnectTimer.Enabled)
            {
                this.Logger.Debug("Stream socket is already connected, restarting timer");
                this.streamDisconnectTimer.Enabled = false;
                this.streamDisconnectTimer.Enabled = true;
                return;
            }

            this.Logger.Debug("Reconnecting stream socket for transfer of {0}", resourceStateResponse.Id);
            this.streamDisconnectTimer.Enabled = true;
            this.Connect(ChannelType.Stream, this.ConnectedStreamSocket);
        }

        private void Connect()
        {
            this.Stop();
            this.Connect(ChannelType.Message, this.ConnectedMessagesSocket);
        }

        private void Connect(ChannelType type, EventHandler<HandshakeSuccessEventArgs> connectedHandler)
        {
            this.running = true;
            var sessionId = this.Session != null ? this.SessionId.Id : 0;
            var client = this.streamFactory.CreateClient(this.config);

            var handshake = new StreamHandshake(this.messageTranscoder.Codec, this.dispatcher.LocalAddress, sessionId);
            handshake.Connected += connectedHandler;

            handshake.Failed += (s, e) =>
                {
                    client.Dispose();
                    if (e.Cause != HandshakeError.CodecNotSupported &&
                        e.Cause != HandshakeError.VersionNotSupported)
                    {
                        this.StartReconnectTimer();
                    }
                };

            this.Logger.Info("Connecting to {0} for {1}", client, type);
            handshake.BeginConnectToServer(client, type, type == ChannelType.Message && this.IsGateway);
        }

        private void ConnectedMessagesSocket(object sender, HandshakeSuccessEventArgs e)
        {
            this.Logger.Info("Connected message socket to {0}", e.Connection);

            var session = this.Session;
            var oldSessionId = session != null ? this.SessionId.Id : 0;
            var newSession = false;
            if (oldSessionId != e.SessionId.Id)
            {
                if (session != null)
                {
                    session.RaiseDisconnected(EventArgs.Empty);
                }

                var frameController = (e.Features & StreamFeature.Framing) != 0 ? new FrameController() : null;
                var gatewayMode = (e.Features & StreamFeature.Gateway) != 0 ? GatewayMode.Client : GatewayMode.None;

                session = new TransportSession(e.SessionId, gatewayMode, frameController);
                newSession = true;
                this.Logger.Debug("Handshake successful: new session {0}", e.SessionId);
            }
            else
            {
                this.Logger.Debug("Handshake successful: existing session {0}", e.SessionId);
            }

            this.StartWritingMessages(
                session, e.Connection, this.config.IdleKeepAliveWait, e.AgreedCodec, this.messageTranscoder);

            if (this.reconnectWaitingReadMessageRequest != null)
            {
                this.reconnectWaitingReadMessageRequest.Complete(oldSessionId, false);
            }
            else
            {
                this.RaiseStarted(EventArgs.Empty);
            }

            if (newSession)
            {
                this.RaiseSessionConnected(new SessionEventArgs(this.Session));
            }

            if (this.ResourceService != null && this.Session.LocalGatewayMode == GatewayMode.None)
            {
                // connect the stream connection with a little delay, otherwise we
                // might connect it before the message socket
                this.streamConnectTimer.Enabled = true;
            }
        }

        private void ConnectedStreamSocket(object sender, HandshakeSuccessEventArgs e)
        {
            this.Logger.Info("Connected stream socket to {0}", e.Connection);

            var session = this.Session;
            var oldSessionId = session != null ? ((StreamSessionId)session.SessionId).Id : 0;
            if (oldSessionId != e.SessionId.Id)
            {
                this.Logger.Warn(
                    "Got different session id for stream: {0} instead of {1}", e.SessionId.Id, oldSessionId);
            }

            var connection = this.streamDisconnectTimer.Enabled
                                 ? new TimeoutStreamConnection(e.Connection, this.streamDisconnectTimer)
                                 : e.Connection;
            this.StartWritingStreams(connection, e.AgreedCodec);

            if (this.reconnectWaitingReadStreamRequest != null)
            {
                this.reconnectWaitingReadStreamRequest.Complete(oldSessionId, false);
            }
        }

        private void StartReconnectTimer()
        {
            if (!this.running)
            {
                return;
            }

            lock (this.reconnectTimer)
            {
                if (this.reconnectTimer.Enabled)
                {
                    this.Logger.Trace("Reconnect timer already started ({0})", this.reconnectTimer.Interval);
                    return;
                }

                this.Logger.Debug("Starting reconnect timer ({0})", this.reconnectTimer.Interval);
                this.reconnectTimer.Enabled = false;
                this.reconnectTimer.Enabled = true;
            }
        }

        private void ReconnectTimerOnElapsed(object sender, EventArgs args)
        {
            this.Connect();
        }

        private void StreamConnectTimerOnElapsed(object sender, EventArgs e)
        {
            this.Connect(ChannelType.Stream, this.ConnectedStreamSocket);
        }

        private void StreamDisconnectTimerOnElapsed(object sender, EventArgs e)
        {
            this.Logger.Debug(
                "Stream connection was not used for {0}, disconnecting it",
                this.streamDisconnectTimer.Interval);
            this.CloseStreamConnection();
        }

        private class TimeoutStreamConnection : IStreamConnection
        {
            private readonly IStreamConnection connection;

            public TimeoutStreamConnection(IStreamConnection connection, ITimer disconnectTimer)
            {
                this.connection = connection;
                this.Stream = new TimeoutStream(connection.Stream, disconnectTimer);
            }

            public Stream Stream { get; private set; }

            public int CreateId()
            {
                return this.connection.CreateId();
            }

            public void Dispose()
            {
                this.connection.Dispose();
            }
        }

        private class TimeoutStream : WrapperStream
        {
            private readonly ITimer disconnectTimer;

            public TimeoutStream(Stream stream, ITimer disconnectTimer)
            {
                this.disconnectTimer = disconnectTimer;
                this.Open(stream);
            }

            public override IAsyncResult BeginRead(
                byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                this.RestartTimer();
                return base.BeginRead(buffer, offset, count, callback, state);
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                var read = base.EndRead(asyncResult);
                this.RestartTimer();
                return read;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                this.RestartTimer();
                var read = base.Read(buffer, offset, count);
                this.RestartTimer();
                return read;
            }

            public override int ReadByte()
            {
                this.RestartTimer();
                var read = base.ReadByte();
                if (read >= 0)
                {
                    this.RestartTimer();
                }

                return read;
            }

            public override IAsyncResult BeginWrite(
                byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                this.RestartTimer();
                return base.BeginWrite(buffer, offset, count, callback, state);
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                base.EndWrite(asyncResult);
                this.RestartTimer();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.RestartTimer();
                base.Write(buffer, offset, count);
                this.RestartTimer();
            }

            public override void WriteByte(byte value)
            {
                this.RestartTimer();
                base.WriteByte(value);
                this.RestartTimer();
            }

            private void RestartTimer()
            {
                this.disconnectTimer.Enabled = false;
                this.disconnectTimer.Enabled = true;
            }
        }
    }
}