// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamMessageTransport.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamMessageTransport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Streams;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Transport.Stream.Messages;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Common.Utility.Core.IO;

    using NLog;

    /// <summary>
    /// Message transport implementation for streams.
    /// </summary>
    internal abstract class StreamMessageTransport : MessageTransport, IManageableObject
    {
        /// <summary>
        /// Logger for this class and subclasses.
        /// </summary>
        protected readonly Logger Logger;

        private const byte StreamMarkHeader = 0;
        private const byte StreamMarkContent = 1;

        private const byte StreamHeaderVersion = 1;

        private readonly SendMessageQueue messageSendQueue = new SendMessageQueue();
        private readonly Queue<StreamMessage> streamSendQueue = new Queue<StreamMessage>();
        private readonly object sendLock = new object();

        private readonly MessageBuffer streamReadHeaderBuffer = new MessageBuffer(1 + sizeof(long));

        private readonly Dictionary<string, StreamMessage> streamsToSend = new Dictionary<string, StreamMessage>();

        private readonly Dictionary<ResourceId, IUploadHandler> uploadHandlers =
            new Dictionary<ResourceId, IUploadHandler>();

        private ITimer keepAliveTimer;
        private ITimer keepAliveResponseTimer;

        private int lastKeepAliveTimestamp;

        private CodecIdentification agreedMessageCodecId;
        private MessageTranscoder messageTranscoder;
        private IStreamConnection messageConnection;
        private bool writingMessage;

        private MessageBuffer readBuffer;
        private Exception readMessageException;

        private long totalBytesReceived;
        private long totalBytesSent;

        private IStreamConnection streamConnection;
        private SimpleAsyncResult<StreamReadResult> waitingReadStream;
        private SimpleAsyncResult<StreamReadResult> currentReadStream;
        private ContentStream currentContentStream;
        private ResourceId currentContentStreamId;
        private bool writingStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamMessageTransport"/> class.
        /// </summary>
        protected StreamMessageTransport()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Event that is fired when the underlying network stream fails.
        /// </summary>
        public event EventHandler StreamFailed;

        /// <summary>
        /// Gets the session associated to this transport.
        /// The session ID is usually given by the server and allows
        /// to identify a session and possibly reopen a session
        /// using some session specific settings/data.
        /// </summary>
        public TransportSession Session { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this transport is has a gateway on one side of the connection.
        /// </summary>
        public bool IsGateway { get; set; }

        /// <summary>
        /// Gets the session ID for the current session.
        /// </summary>
        protected StreamSessionId SessionId
        {
            get
            {
                return (StreamSessionId)this.Session.SessionId;
            }
        }

        /// <summary>
        /// Gets the resource service used by this transport.
        /// </summary>
        protected abstract IResourceServiceImpl ResourceService { get; }

        /// <summary>
        /// Starts this transport with the given session and using
        /// the given network stream for sending and receiving data.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="connection">
        /// The network connection.
        /// </param>
        /// <param name="idleKeepAliveWait">
        /// Time in milliseconds to wait before sending a keep-alive
        /// message when nothing is happening on this connection (no receive or send).
        /// Set to -1 to disable.
        /// </param>
        /// <param name="agreedCodec">
        /// The codec on which the handshake has agreed.
        /// </param>
        /// <param name="transcoder">
        /// The transcoder that is on top of this transport.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If <see cref="StartWritingMessages"/> was called before without calling
        /// <see cref="Stop"/> afterwards.
        /// </exception>
        public void StartWritingMessages(
            TransportSession session,
            IStreamConnection connection,
            int idleKeepAliveWait,
            CodecIdentification agreedCodec,
            MessageTranscoder transcoder)
        {
            if (this.messageConnection != null)
            {
                throw new NotSupportedException("Can't call StartWritingMessages() twice.");
            }

            this.Logger.Debug("Starting to write messages");
            this.agreedMessageCodecId = agreedCodec;
            this.messageTranscoder = transcoder;

            if (idleKeepAliveWait > 0)
            {
                this.keepAliveTimer = TimerFactory.Current.CreateTimer("StreamKeepAlive");
                this.keepAliveTimer.AutoReset = false;
                this.keepAliveTimer.Elapsed += (s, e) => this.SendKeepAlive();

                this.keepAliveResponseTimer = TimerFactory.Current.CreateTimer("StreamKeepAliveTimeout");
                this.keepAliveResponseTimer.AutoReset = false;
                this.keepAliveResponseTimer.Elapsed += (s, e) => this.KeepAliveTimeout();

                // randomize the wait time, otherwise the whole network might start pumping out
                // keep-alives at exactly the same time; like this some nodes might not be
                // sending keep-alives since they always get them first from another node
                var timeout = TimeSpan.FromMilliseconds(idleKeepAliveWait + new Random().Next(idleKeepAliveWait / 10));
                this.keepAliveTimer.Interval = timeout;
                this.keepAliveResponseTimer.Interval = timeout;

                if (connection.Stream.CanTimeout)
                {
                    // set the write timeout, so we get an exception if we can't write the
                    // keep-alive within half the timeout time
                    connection.Stream.WriteTimeout = (int)(timeout.TotalMilliseconds / 2);

                    // set the read timeout, so we get an exception if we can't read the
                    // keep-alive response (Pong) within double the timeout time
                    connection.Stream.ReadTimeout = (int)(timeout.TotalMilliseconds / 2);
                }

                this.ResetKeepAliveTimer();
            }

            if (this.Session != null && !this.SessionId.Equals(session.SessionId))
            {
                // clear the queue when we have a new session
                this.messageSendQueue.Clear();
            }

            this.Session = session;

            lock (this.sendLock)
            {
                this.messageSendQueue.FramingEnabled = session.FrameController != null;
                this.messageConnection = connection;
            }

            this.StartWritingMessages(false);
        }

        /// <summary>
        /// Starts this transport with the given session and using
        /// the given network stream for sending and receiving streams.
        /// </summary>
        /// <param name="connection">
        /// The network connection.
        /// </param>
        /// <param name="agreedCodec">
        /// The codec on which the handshake has agreed.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If <see cref="StartWritingMessages"/> was called before without calling
        /// <see cref="Stop"/> afterwards.
        /// </exception>
        public void StartWritingStreams(
            IStreamConnection connection,
            CodecIdentification agreedCodec)
        {
            if (this.streamConnection != null)
            {
                this.Logger.Debug(
                    "Closing old stream connection ({0}) since we got a new one ({1})",
                    this.streamConnection,
                    connection);
                this.CloseStreamConnection();
            }

            this.Logger.Debug("Starting to write streams");

            SimpleAsyncResult<StreamReadResult> waitingRead;
            lock (this.sendLock)
            {
                this.streamConnection = connection;
                waitingRead = this.waitingReadStream;
                this.waitingReadStream = null;
            }

            this.StartWritingStreams(false);
            if (waitingRead != null)
            {
                this.BeginReadStream(waitingRead);
            }
        }

        /// <summary>
        /// Stops this transport.
        /// </summary>
        public override void Stop()
        {
            this.Close();
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
            if (this.readBuffer != null)
            {
                throw new NotSupportedException("Can't have more than one BeginReadMessage outstanding.");
            }

            var buffer = bufferProvider.GetReadBuffer(this.Session);

            if (buffer.Size <= buffer.Count)
            {
                buffer.EnsureSize(buffer.Size * 2);
            }

            if (this.messageSendQueue.FramingEnabled)
            {
                lock (this.sendLock)
                {
                    this.messageSendQueue.Acknowledge(this.Session.FrameController.LastAcknowledgedFrameId);
                }
            }

            this.readBuffer = buffer;
            var result = new SimpleAsyncResult<int>(callback, state);
            this.BeginReadMessage(result);

            return result;
        }

        /// <summary>
        /// Ends the async request issued by <see cref="BeginReadMessage"/>.
        /// </summary>
        /// <param name="ar">
        ///   The result returned by <see cref="BeginReadMessage"/>.
        /// </param>
        /// <returns>
        /// The read result including the session from which the data was received.
        /// </returns>
        public override MessageReadResult EndReadMessage(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<int>;
            if (result == null)
            {
                throw new ArgumentException("Call EndReadMessage() with the provided IAsyncResult, nothing else");
            }

            if (this.readBuffer == null)
            {
                throw new NotSupportedException("Call BeginReadMessage() before calling EndReadMessage().");
            }

            this.readBuffer.Count += result.Value;
            this.readBuffer = null;

            if (this.readMessageException != null)
            {
                var exception = this.readMessageException;
                this.readMessageException = null;
                throw new IOException("Could not read from underlying stream", exception);
            }

            return new MessageReadResult(result.Value, this.Session, this.agreedMessageCodecId);
        }

        /// <summary>
        /// Sends messages to the remote party.
        /// </summary>
        /// <param name="bufferProvider">
        /// The buffer provider which will be queried for message buffers.
        /// </param>
        /// <param name="destinationSessionId">
        /// The id of the session to which the given buffers must be written.
        /// </param>
        public override void WriteMessage(IMessageBufferProvider bufferProvider, ISessionId destinationSessionId)
        {
            if (!this.SessionId.Equals(destinationSessionId))
            {
                return;
            }

            var buffers = bufferProvider.GetMessageBuffers(this.Session, this.agreedMessageCodecId);
            if (buffers == null)
            {
                this.Logger.Warn("Got null buffers for {0}", this.SessionId);
                return;
            }

            int count = 0;
            int queueSize;
            lock (this.sendLock)
            {
                foreach (var buffer in buffers)
                {
                    if (buffer != null)
                    {
                        this.messageSendQueue.Enqueue(buffer);
                        count++;
                    }
                }

                queueSize = this.messageSendQueue.Count;
            }

            this.Logger.Trace(
                "Enqueued {0} message buffers (now at {1}) to be written to {2}", count, queueSize, this.SessionId);

            this.StartWritingMessages(false);
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
            var result = new SimpleAsyncResult<StreamReadResult>(callback, state);
            lock (this.sendLock)
            {
                if (this.streamConnection == null)
                {
                    this.waitingReadStream = result;
                    return result;
                }
            }

            this.BeginReadStream(result);
            return result;
        }

        /// <summary>
        /// Ends the async request issued by <see cref="IMessageTransport.BeginReadStream"/>.
        /// </summary>
        /// <param name="ar">
        /// The result returned by <see cref="IMessageTransport.BeginReadStream"/>.
        /// </param>
        /// <returns>
        /// The read result including the session from which the data was received.
        /// </returns>
        public override StreamReadResult EndReadStream(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<StreamReadResult>;
            if (result == null)
            {
                throw new ArgumentException("Call EndReadMessage() with the provided IAsyncResult, nothing else");
            }

            return result.Value;
        }

        /// <summary>
        /// Sends a stream to the remote party.
        /// </summary>
        /// <param name="message">
        /// The stream message to send.
        /// </param>
        /// <param name="destinationSessionId">
        /// The id of the session to which the given stream message must be written.
        /// </param>
        public override void WriteStream(StreamMessage message, ISessionId destinationSessionId)
        {
            if (!this.SessionId.Equals(destinationSessionId))
            {
                return;
            }

            lock (this.streamsToSend)
            {
                this.streamsToSend[message.Header.Hash] = message;
            }

            // send a resource state request for which we will get a resource state response telling
            // us if we need to send the message or not
            this.Logger.Debug(
                "Remote resource status of {0} is unknown, sending ResourceStateRequest to {1}",
                message.Header.Hash,
                this.SessionId);
            this.SendInternalMessage(new ResourceStateRequest { Id = new ResourceId(message.Header.Hash) });
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
            if (this.HandleInternalMessage(message))
            {
                this.Logger.Debug("Handled internal message: {0}", message.Payload);
                message = null;
            }
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<string>("SessionId", this.SessionId.ToString(), true);

            yield return new ManagementProperty<bool>("IsGateway", this.IsGateway, true);

            yield return new ManagementProperty<long>("TotalBytesReceived", this.totalBytesReceived, true);

            yield return new ManagementProperty<long>("TotalBytesSent", this.totalBytesSent, true);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        /// <summary>
        /// Closes the underlying network stream.
        /// </summary>
        protected virtual void Close()
        {
            this.readBuffer = null;
            this.messageTranscoder = null;

            var connectionsToClose = new List<IStreamConnection>(2);
            lock (this.sendLock)
            {
                this.messageSendQueue.Restart();
                this.streamSendQueue.Clear();

                if (this.messageConnection != null)
                {
                    connectionsToClose.Add(this.messageConnection);
                }

                this.messageConnection = null;
                this.writingMessage = false;

                if (this.streamConnection != null)
                {
                    connectionsToClose.Add(this.streamConnection);
                }

                this.currentReadStream = null;
                this.streamConnection = null;
                this.writingStream = false;
            }

            if (this.keepAliveTimer != null)
            {
                this.keepAliveTimer.Enabled = false;
                this.keepAliveTimer = null;
            }

            if (this.keepAliveResponseTimer != null)
            {
                this.keepAliveResponseTimer.Enabled = false;
                this.keepAliveResponseTimer = null;
            }

            if (this.Session != null && connectionsToClose.Count > 0)
            {
                this.Logger.Info("Closing {0} connections of session {1}", connectionsToClose.Count, this.SessionId);
            }

            foreach (var connection in connectionsToClose)
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// Closes the stream connection (but leaves the message connection open).
        /// </summary>
        protected void CloseStreamConnection()
        {
            SimpleAsyncResult<StreamReadResult> currentRead;
            IStreamConnection connection;
            lock (this.sendLock)
            {
                currentRead = this.currentReadStream;
                this.currentReadStream = null;

                connection = this.streamConnection;
                this.streamConnection = null;
            }

            if (connection != null)
            {
                connection.Dispose();
            }

            if (currentRead != null)
            {
                this.waitingReadStream = currentRead;
            }
        }

        /// <summary>
        /// Sends an <see cref="IInternalMessage"/> to the connected remote peer.
        /// </summary>
        /// <param name="message">
        /// The message to be sent.
        /// </param>
        protected virtual void SendInternalMessage(IInternalMessage message)
        {
            var transcoder = this.messageTranscoder;
            if (transcoder == null)
            {
                return;
            }

            var mediMessage = new MediMessage
                                  {
                                      Source = MediAddress.Empty,
                                      Destination = MediAddress.Empty,
                                      Payload = message
                                  };
            transcoder.Encode(mediMessage, this, this.SessionId);
        }

        /// <summary>
        /// Fires the <see cref="StreamFailed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStreamFailed(EventArgs e)
        {
            var handler = this.StreamFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void BeginReadMessage(SimpleAsyncResult<int> result)
        {
            var connection = this.messageConnection;
            var buffer = this.readBuffer;
            if (connection == null || buffer == null)
            {
                return;
            }

            connection.Stream.BeginRead(
                buffer.Buffer, buffer.Count, buffer.Size - buffer.Count, this.ReadFromMessageStream, result);
        }

        private void ReadFromMessageStream(IAsyncResult ar)
        {
            var result = ar.AsyncState as SimpleAsyncResult<int>;
            Debug.Assert(result != null, "Got SocketRead without a chained result");

            var connection = this.messageConnection;
            if (connection == null)
            {
                return;
            }

            int read;
            try
            {
                read = connection.Stream.EndRead(ar);
                if (read == 0)
                {
                    this.BeginReadMessage(result);
                    return;
                }
            }
            catch (IOException ex)
            {
                this.Logger.Warn(ex,"Could not read message from " + this.SessionId);

                this.readMessageException = ex;
                result.Complete(0, false);

                this.Close();

                this.RaiseStreamFailed(EventArgs.Empty);
                return;
            }
            catch (ObjectDisposedException ex)
            {
                // the stream was already closed, this is OK
                this.Logger.Trace(ex,"Trying to read from a closed message stream");
                return;
            }

            this.ResetKeepAliveTimer();
            this.totalBytesReceived += read;
            result.Complete(read, false);
        }

        private void StartWritingMessages(bool force)
        {
            SendMessageBuffer buffer;
            Stream output;
            lock (this.sendLock)
            {
                if (this.writingMessage && !force)
                {
                    return;
                }

                if (this.messageSendQueue.Count == 0 || this.messageConnection == null)
                {
                    this.writingMessage = false;
                    return;
                }

                buffer = this.messageSendQueue.Dequeue();
                this.writingMessage = true;
                output = this.messageConnection.Stream;
            }

            this.Logger.Trace("Writing {0} bytes message to {1}", buffer.Count, this.SessionId);
            try
            {
                output.BeginWrite(buffer.Buffer, buffer.Offset, buffer.Count, this.MessageWritten, buffer);
            }
            catch (ObjectDisposedException)
            {
                this.Logger.Debug("Could not begin writing to {0} because the object was disposed", this.SessionId);
            }
            catch (IOException ex)
            {
                this.Logger.Warn(ex, "Could not begin writing to " + this.SessionId);
                this.Close();

                this.RaiseStreamFailed(EventArgs.Empty);
            }
        }

        private void MessageWritten(IAsyncResult ar)
        {
            var connection = this.messageConnection;
            if (connection == null)
            {
                return;
            }

            try
            {
                connection.Stream.EndWrite(ar);
            }
            catch (IOException ex)
            {
                this.Logger.Warn(ex, "Could not write to " + this.SessionId);
                this.Close();

                this.RaiseStreamFailed(EventArgs.Empty);
                return;
            }

            var buffer = ar.AsyncState as MessageBuffer;
            if (buffer != null)
            {
                this.Logger.Debug("Wrote {0} bytes to {1}", buffer.Count, this.SessionId);
                this.totalBytesSent += buffer.Count;
            }

            this.ResetKeepAliveTimer();
            this.StartWritingMessages(true);
        }

        private void BeginReadStream(SimpleAsyncResult<StreamReadResult> result)
        {
            var contentStream = this.currentContentStream;
            this.currentContentStream = null;
            if (contentStream != null)
            {
                try
                {
                    contentStream.Close();
                    this.SendInternalMessage(new ResourceDownloaded { Id = this.currentContentStreamId });
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't close content stream");
                }
            }

            var connection = this.streamConnection;
            if (connection == null)
            {
                result.CompleteException(new IOException("Connection closed"), false);
                return;
            }

            this.currentReadStream = result;
            this.streamReadHeaderBuffer.Clear();
            connection.Stream.BeginRead(
                this.streamReadHeaderBuffer.Buffer,
                0,
                this.streamReadHeaderBuffer.Size,
                this.ReadFromStreamStream,
                result);
        }

        private void ReadFromStreamStream(IAsyncResult ar)
        {
            var result = ar.AsyncState as SimpleAsyncResult<StreamReadResult>;
            Debug.Assert(result != null, "Got SocketRead without a chained result");

            if (result == this.currentReadStream)
            {
                this.currentReadStream = null;
            }

            var connection = this.streamConnection;
            if (connection == null)
            {
                return;
            }

            var input = connection.Stream;

            try
            {
                int read = input.EndRead(ar);
                if (read == 0)
                {
                    this.BeginReadStream(result);
                    return;
                }

                // read the remaining bytes of the header's header
                this.totalBytesReceived += read;
                this.streamReadHeaderBuffer.Count = read;
                var headerLength = this.ReadStreamHeading(input, StreamMarkHeader);

                if (headerLength > 4096)
                {
                    throw new IOException("Bad header length decoded: " + headerLength);
                }

                // read the header message
                var headerBuffer = new MessageBuffer((int)headerLength);
                this.ReadCompletely(input, headerBuffer);

                var header = this.DecodeStreamHeader(headerBuffer);
                this.Logger.Trace("Read stream header: {0}", header);

                // read the content header
                this.streamReadHeaderBuffer.Clear();
                var contentLength = this.ReadStreamHeading(input, StreamMarkContent);
                this.totalBytesReceived += contentLength;

                var stream = new ContentStream(input, contentLength);
                this.Logger.Trace("Created content stream for {0} bytes", contentLength);
                var message = new SimpleStreamMessage(header, stream);
                this.currentContentStreamId = new ResourceId(header.Hash);
                this.currentContentStream = stream;
                result.Complete(new StreamReadResult(message, this.Session), false);
            }
            catch (IOException ex)
            {
                var msg = "Could not read stream header from " + this.SessionId;
                if (!result.TryCompleteException(new IOException(msg, ex), false))
                {
                    this.Logger.Warn(ex, msg);
                }

                this.Close();

                this.RaiseStreamFailed(EventArgs.Empty);
            }
            catch (ObjectDisposedException ex)
            {
                // the stream was already closed, this is OK
                this.Logger.Trace(ex, "Trying to read from a closed stream stream");
            }
        }

        private StreamHeader DecodeStreamHeader(MessageBuffer buffer)
        {
            using (var reader = new BinaryReader(buffer.OpenRead(0)))
            {
                var headerVersion = reader.ReadByte();
                if (headerVersion != StreamHeaderVersion)
                {
                    throw new IOException("Found unsupported stream header version: " + headerVersion);
                }

                var header = new StreamHeader();
                header.Source = this.ReadHeaderAddress(reader);
                header.Destination = this.ReadHeaderAddress(reader);
                header.Offset = reader.ReadInt64();
                header.Length = reader.ReadInt64();
                header.Hash = reader.ReadString();

                return header;
            }
        }

        private MediAddress ReadHeaderAddress(BinaryReader reader)
        {
            var unit = reader.ReadString();
            var app = reader.ReadString();
            return new MediAddress { Unit = unit, Application = app };
        }

        private long ReadStreamHeading(Stream input, byte mark)
        {
            this.ReadCompletely(input, this.streamReadHeaderBuffer);

            if (this.streamReadHeaderBuffer.Buffer[0] != mark)
            {
                throw new IOException("Found wrong header marker: " + this.streamReadHeaderBuffer.Buffer[0]);
            }

            long headerLength = BitConverter.ToInt64(this.streamReadHeaderBuffer.Buffer, 1);

            this.Logger.Trace("Reading stream heading, mark={0}, header length={1}", mark, headerLength);
            return headerLength;
        }

        private void ReadCompletely(Stream input, MessageBuffer buffer)
        {
            while (buffer.Count < buffer.Size)
            {
                int r = input.Read(buffer.Buffer, buffer.Count, buffer.Size - buffer.Count);
                buffer.Count += r;
                this.totalBytesReceived += r;
                if (r == 0)
                {
                    throw new EndOfStreamException("Read zero bytes from socket");
                }
            }
        }

        private void EnqueueStreamToSend(StreamMessage message, long offset)
        {
            this.Logger.Trace("EnqueueStreamToSend({0} @ {1}", message.Header.Hash, offset);
            if (offset == message.Header.Length)
            {
                this.Logger.Debug(
                    "Don't have to send resource {0}, it already exists at the destination", message.Header.Hash);

                var resourceService = this.ResourceService;
                if (resourceService == null)
                {
                    this.Logger.Warn("Couldn't find resource service to create upload handler");
                    return;
                }

                // let the service know that we are already done
                var uploadHandler = resourceService.CreateUploadHandler(
                    new ResourceId(message.Header.Hash), message.Header.Destination);
                uploadHandler.Complete(true);
                message.Dispose();
                return;
            }

            if (offset != 0)
            {
                message = new OffsetStreamMessage(message, offset);
            }

            int queueSize;
            lock (this.sendLock)
            {
                this.streamSendQueue.Enqueue(message);
                queueSize = this.streamSendQueue.Count;
            }

            this.Logger.Trace("Enqueued stream buffer (now at {0}) to be written to {1}", queueSize, this.SessionId);

            this.StartWritingStreams(false);
        }

        private void StartWritingStreams(bool force)
        {
            StreamMessage message;
            IStreamConnection connection;
            lock (this.sendLock)
            {
                if (this.writingStream && !force)
                {
                    return;
                }

                if (this.streamSendQueue.Count == 0 || this.streamConnection == null)
                {
                    this.writingStream = false;
                    return;
                }

                message = this.streamSendQueue.Dequeue();
                this.writingStream = true;
                connection = this.streamConnection;
            }

            var headerStream = new MemoryStream(512);
            using (var writer = new BinaryWriter(headerStream))
            {
                this.WriteStreamHeading(StreamMarkHeader, 0, writer);
                this.EncodeStreamHeader(message.Header, writer);
                writer.Flush();

                // update the length in the buffer (it was previously set to 0)
                long headerLength = headerStream.Position - sizeof(byte) - sizeof(long);
                writer.Seek(1, SeekOrigin.Begin);
                writer.Write(headerLength);
            }

            var headerBytes = headerStream.ToArray();

            this.Logger.Trace("Writing {0} bytes stream header to {1}", headerBytes.Length, this.SessionId);
            try
            {
                connection.Stream.BeginWrite(
                    headerBytes,
                    0,
                    headerBytes.Length,
                    this.StreamHeaderWritten,
                    new WriteStreamHeaderState(headerBytes.Length, message));
            }
            catch (IOException ex)
            {
                this.Logger.Warn(ex, "Could not begin writing stream to " + this.SessionId);
                this.Close();

                lock (this.sendLock)
                {
                    // re-enqueue this message
                    this.streamSendQueue.Enqueue(message);
                }

                this.RaiseStreamFailed(EventArgs.Empty);
            }
        }

        private void StreamHeaderWritten(IAsyncResult ar)
        {
            var connection = this.streamConnection;
            if (connection == null)
            {
                return;
            }

            var output = connection.Stream;
            try
            {
                output.EndWrite(ar);
                output.Flush();
            }
            catch (IOException ex)
            {
                this.Logger.Warn(ex, "Could not write stream to " + this.SessionId);
                this.Close();

                this.RaiseStreamFailed(EventArgs.Empty);
                return;
            }

            var state = (WriteStreamHeaderState)ar.AsyncState;
            var message = state.Message;
            var hash = message.Header.Hash;

            this.Logger.Debug("Wrote {0} bytes stream header to {1}", state.HeaderLength, this.SessionId);
            this.totalBytesSent += state.HeaderLength;

            try
            {
                var id = new ResourceId(hash);
                var uploadHandler = this.ResourceService.CreateUploadHandler(id, message.Header.Destination);
                long length = message.Header.Length - message.Header.Offset;

                // Important: don't dispose this writer
                var writer = new BinaryWriter(output);
                this.WriteStreamHeading(StreamMarkContent, length, writer);
                this.totalBytesSent += 9;

                this.Logger.Debug("Writing stream {0} with {1} bytes to {2}", hash, length, this.SessionId);
                try
                {
                    uploadHandler.Upload(message, output, this.Session);
                    lock (this.uploadHandlers)
                    {
                        this.uploadHandlers[id] = uploadHandler;
                    }
                }
                catch
                {
                    uploadHandler.Complete(false);
                }
                finally
                {
                    this.totalBytesSent += uploadHandler.TotalBytesUploaded;
                }

                this.Logger.Debug("Wrote stream with {0} bytes to {1}", length, this.SessionId);
            }
            catch (ObjectDisposedException ex)
            {
                this.Logger.Warn(ex, "Could not write stream contents to disposed socket of " + this.SessionId);
                return;
            }
            catch (IOException ex)
            {
                this.Logger.Warn(ex, "Could not write stream contents to " + this.SessionId);
            }
            finally
            {
                message.Dispose();
            }

            this.StartWritingStreams(true);
        }

        private void EncodeStreamHeader(StreamHeader header, BinaryWriter writer)
        {
            writer.Write(StreamHeaderVersion);
            this.WriteHeaderAddress(header.Source, writer);
            this.WriteHeaderAddress(header.Destination, writer);
            writer.Write(header.Offset);
            writer.Write(header.Length);
            writer.Write(header.Hash);
        }

        private void WriteHeaderAddress(MediAddress address, BinaryWriter writer)
        {
            writer.Write(address.Unit);
            writer.Write(address.Application);
        }

        private void WriteStreamHeading(byte mark, long length, BinaryWriter writer)
        {
            writer.Write(mark);
            writer.Write(length);
            writer.Flush();
        }

        private bool HandleInternalMessage(MediMessage message)
        {
            var keepAlive = message.Payload as KeepAlive;
            if (keepAlive != null)
            {
                this.HandleKeepAlive(keepAlive);
                return true;
            }

            var resourceStateRequest = message.Payload as ResourceStateRequest;
            if (resourceStateRequest != null)
            {
                this.HandleResourceStateRequest(resourceStateRequest);
                return true;
            }

            var resourceStateResponse = message.Payload as ResourceStateResponse;
            if (resourceStateResponse != null)
            {
                this.HandleResourceStateResponse(resourceStateResponse);
                return true;
            }

            var resourceStateAck = message.Payload as ResourceStateAck;
            if (resourceStateAck != null)
            {
                // this message is only used to make sure the frames are properly acknowledged,
                // we don't need its contents (for now)
                return true;
            }

            var resourceDownloaded = message.Payload as ResourceDownloaded;
            if (resourceDownloaded != null)
            {
                this.HandleResourceDownloaded(resourceDownloaded);
                return true;
            }

            return false;
        }

        private void ResetKeepAliveTimer()
        {
            var timer = this.keepAliveTimer;
            if (timer == null)
            {
                return;
            }

            timer.Enabled = false;
            timer.Enabled = true;
        }

        private void SendKeepAlive()
        {
            var timer = this.keepAliveResponseTimer;
            if (this.messageTranscoder == null || timer == null)
            {
                return;
            }

            this.Logger.Debug("Sending keep-alive request to {0}", this.SessionId);
            this.lastKeepAliveTimestamp = Environment.TickCount;
            timer.Enabled = true;
            this.SendInternalMessage(new KeepAlive { Timestamp = this.lastKeepAliveTimestamp, IsRequest = true });
        }

        private void HandleKeepAlive(KeepAlive keepAlive)
        {
            if (!keepAlive.IsRequest)
            {
                if (this.lastKeepAliveTimestamp != keepAlive.Timestamp)
                {
                    this.Logger.Warn(
                        "Received bad keep-alive response from {0}: received {1} instead of {2}",
                        this.SessionId,
                        keepAlive.Timestamp,
                        this.lastKeepAliveTimestamp);
                    return;
                }

                this.Logger.Debug("Received keep-alive response from {0}", this.SessionId);
                this.keepAliveResponseTimer.Enabled = false;
                return;
            }

            this.Logger.Debug("Received keep-alive request from {0}", this.SessionId);

            this.SendInternalMessage(new KeepAlive { Timestamp = keepAlive.Timestamp, IsRequest = false });
        }

        private void KeepAliveTimeout()
        {
            this.Logger.Warn("Didn't receive keep-alive response from {0}", this.SessionId);
            this.Close();

            this.RaiseStreamFailed(EventArgs.Empty);
        }

        private void HandleResourceStateResponse(ResourceStateResponse response)
        {
            this.Logger.Debug("Got {0}", response);
            this.SendInternalMessage(new ResourceStateAck { Id = response.Id });

            var hash = response.Id.Hash;
            var offset = response.ResourceStatus.AvailableBytes;
            StreamMessage message;
            lock (this.streamsToSend)
            {
                if (!this.streamsToSend.TryGetValue(hash, out message))
                {
                    this.Logger.Info("Don't know what to do with {0}", response);
                    return;
                }

                this.streamsToSend.Remove(hash);
            }

            this.EnqueueStreamToSend(message, offset);
        }

        private void HandleResourceDownloaded(ResourceDownloaded resourceDownloaded)
        {
            IUploadHandler handler;
            lock (this.uploadHandlers)
            {
                if (!this.uploadHandlers.TryGetValue(resourceDownloaded.Id, out handler))
                {
                    this.Logger.Warn("Couldn't find upload handler for {0}, can't complete it", resourceDownloaded.Id);
                    return;
                }

                this.uploadHandlers.Remove(resourceDownloaded.Id);
            }

            handler.Complete(true);
        }

        private void HandleResourceStateRequest(ResourceStateRequest request)
        {
            if (this.ResourceService == null)
            {
                this.Logger.Error("Got ResourceStateRequest, but the streams channel is not active");
                return;
            }

            this.ResourceService.BeginGetResourceStatus(
                request.Id,
                result =>
                    {
                        ResourceStatus status;
                        try
                        {
                            status = this.ResourceService.EndGetResourceStatus(result);
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Debug(ex,"Couldn't get resource status yet, retrying");
                            ThreadPool.QueueUserWorkItem(s => this.HandleResourceStateRequest(request));
                            return;
                        }

                        this.SendInternalMessage(
                            new ResourceStateResponse { Id = request.Id, ResourceStatus = status });
                    },
                null);
        }

        private class WriteStreamHeaderState
        {
            public WriteStreamHeaderState(long headerLength, StreamMessage message)
            {
                this.HeaderLength = headerLength;
                this.Message = message;
            }

            public long HeaderLength { get; private set; }

            public StreamMessage Message { get; private set; }
        }

        private class OffsetStreamMessage : StreamMessage
        {
            private readonly StreamMessage message;

            private readonly long offset;

            public OffsetStreamMessage(StreamMessage message, long offset)
                : base(
                    new StreamHeader
                        {
                            Source = message.Header.Source,
                            Destination = message.Header.Destination,
                            Offset = offset,
                            Length = message.Header.Length,
                            Hash = message.Header.Hash
                        })
            {
                this.message = message;
                this.offset = offset;
            }

            public override Stream OpenRead()
            {
                var stream = this.message.OpenRead();
                stream.Seek(this.offset, SeekOrigin.Begin);
                return stream;
            }

            public override void Dispose()
            {
                this.message.Dispose();
            }

            public override string ToString()
            {
                return string.Format("OffsetStreamMessage[{0},{1}]", this.offset, this.message);
            }
        }
    }
}
