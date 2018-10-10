// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileMessageTransport.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileMessageTransport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Streams;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="IMessageTransport"/> for the file exchange protocol.
    /// There is one <see cref="FileMessageTransport"/> for each peer that is communicating
    /// using the same location.
    /// </summary>
    internal class FileMessageTransport : MessageTransport, TransportServer.IClientMessageTransport
    {
        /// <summary>
        /// The DateTime format string used to create timestamps.
        /// </summary>
        internal static readonly string TimeStampFormat = "yyMMddHHmmssfff";

        private static readonly string MessageNameTempFormat = "m{0}_{1}_{2:" + TimeStampFormat + "}.tmp";
        private static readonly string MessageNameFormat = "m{0}_{1}_{2:" + TimeStampFormat + "}.msg";

        private static readonly Logger Logger = LogHelper.GetLogger<FileMessageTransport>();

        private static readonly Regex MessageNameRegex =
            new Regex(@"^m([0-9a-f]+)_([0-9a-f]+)_(\d+).msg$", RegexOptions.IgnoreCase);

        private readonly FileSessionId localSessionId;

        private readonly Queue<SendMessageBuffer> sendQueue = new Queue<SendMessageBuffer>();
        private readonly Queue<MessageBuffer> receiveQueue = new Queue<MessageBuffer>();

        private readonly ITimer sessionTimeout = TimerFactory.Current.CreateTimer("FileMsgTrspSessionTimeout");

        private readonly FileTransportServerConfig config;

        private ReadMessageRequest readRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMessageTransport"/> class.
        /// </summary>
        /// <param name="localSessionId">
        /// The local session id.
        /// </param>
        /// <param name="sessionId">
        /// The session id of the opposite peer.
        /// </param>
        /// <param name="config">
        /// The local configuration.
        /// </param>
        public FileMessageTransport(
            FileSessionId localSessionId, FileSessionId sessionId, FileTransportServerConfig config)
        {
            this.localSessionId = localSessionId;
            this.config = config;

            // TODO: support FrameController!
            this.Session = new TransportSession(sessionId, GatewayMode.None, null);

            this.sessionTimeout.AutoReset = false;
            this.sessionTimeout.Elapsed += (s, e) => this.Stop();
        }

        /// <summary>
        /// This event is fired when this transport is connected.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// This event is fired in addition to (and after) <see cref="Connected"/>
        /// when this transport is connected with a new session.
        /// </summary>
        public event EventHandler SessionConnected;

        /// <summary>
        /// This event is fired when this transport is disconnected.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// This event is fired when a new message was enqueued in this
        /// transport. This is needed to immediately send messages from
        /// <see cref="FileTransportServer"/> instead of just when the
        /// polling interval is expired.
        /// </summary>
        public event EventHandler MessageSendEnqueued;

        /// <summary>
        /// Gets the session associated to this client.
        /// </summary>
        public ITransportSession Session { get; private set; }

        /// <summary>
        /// Gets the session identification of the remote peer.
        /// </summary>
        public FileSessionId SessionId
        {
            get
            {
                return (FileSessionId)this.Session.SessionId;
            }
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
        /// An async result that can be used in <see cref="EndReadMessage"/>.
        /// </returns>
        public override IAsyncResult BeginReadMessage(
            IReadBufferProvider bufferProvider, AsyncCallback callback, object state)
        {
            MessageBuffer receiveBuffer;
            lock (this.receiveQueue)
            {
                this.readRequest = new ReadMessageRequest(bufferProvider, callback, state);
                if (this.receiveQueue.Count == 0)
                {
                    return this.readRequest;
                }

                receiveBuffer = this.receiveQueue.Dequeue();
            }

            // TODO: we need the codec ID somehow!
            var request = this.readRequest;
            var result = new MessageReadResult(receiveBuffer.Count, this.Session, null);
            this.readRequest.Complete(result, receiveBuffer, true);
            return request;
        }

        /// <summary>
        /// Ends the async request issued by <see cref="MessageTransport.BeginReadMessage"/>.
        /// </summary>
        /// <param name="result">
        ///   The result returned by <see cref="MessageTransport.BeginReadMessage"/>.
        /// </param>
        /// <returns>
        /// The read result including the session from which the data was received.
        /// </returns>
        public override MessageReadResult EndReadMessage(IAsyncResult result)
        {
            var readResult = this.readRequest.ReadResult;
            this.readRequest = null;
            return readResult;
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

            // TODO: we need the codec ID somehow!
            var buffers = bufferProvider.GetMessageBuffers(this.Session, null);
            if (buffers == null)
            {
                return;
            }

            lock (this.sendQueue)
            {
                foreach (var buffer in buffers)
                {
                    this.sendQueue.Enqueue(buffer);
                }
            }

            this.RaiseMessageSendEnqueued(EventArgs.Empty);
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ends the async request issued by <see cref="IMessageTransport.BeginReadStream"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="IMessageTransport.BeginReadStream"/>.
        /// </param>
        /// <returns>
        /// The read result including the session from which the data was received.
        /// </returns>
        public override StreamReadResult EndReadStream(IAsyncResult result)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            // nothing to do here
        }

        /// <summary>
        /// Starts this transport by raising the necessary events
        /// to let everyone know we are now ready to serve requests.
        /// </summary>
        public void Start()
        {
            this.RaiseConnected(EventArgs.Empty);
            this.RaiseSessionConnected(EventArgs.Empty);
        }

        /// <summary>
        /// Stops this transport.
        /// </summary>
        public override void Stop()
        {
            this.RaiseDisconnected(EventArgs.Empty);
            ((TransportSession)this.Session).RaiseDisconnected(EventArgs.Empty);
        }

        /// <summary>
        /// Sets the expiry timeout of this session.
        /// If the value is not set to a newer date until the given time,
        /// this session will be stopped and <see cref="Disconnected"/>
        /// is risen.
        /// </summary>
        /// <param name="timestamp">the new expiry time of this transport</param>
        public void SetExpiry(DateTime timestamp)
        {
            this.sessionTimeout.Enabled = false;
            this.sessionTimeout.Interval = timestamp - DateTime.UtcNow;
            this.sessionTimeout.Enabled = true;
        }

        /// <summary>
        /// Uploads all currently enqueued messages as a single file to the drop location.
        /// </summary>
        /// <param name="session">
        /// The file transfer session to use for the upload.
        /// </param>
        public void UploadMessages(IFileTransferSession session)
        {
            SendMessageBuffer[] buffers;
            lock (this.sendQueue)
            {
                if (this.sendQueue.Count == 0)
                {
                    return;
                }

                buffers = new SendMessageBuffer[this.sendQueue.Count];
                this.sendQueue.CopyTo(buffers, 0);
                this.sendQueue.Clear();
            }

            try
            {
                var timestamp = DateTime.UtcNow + TimeSpan.FromMilliseconds(this.config.MessageTimeToLive);
                var tempFileName = string.Format(
                    MessageNameTempFormat, this.localSessionId.Hash, this.SessionId.Hash, timestamp);
                var fileName = string.Format(
                    MessageNameFormat, this.localSessionId.Hash, this.SessionId.Hash, timestamp);
                try
                {
                    using (var file = session.OpenWrite(tempFileName))
                    {
                        foreach (var buffer in buffers)
                        {
                            file.Write(buffer.Buffer, buffer.Offset, buffer.Count);
                        }
                    }
                }
                catch (Exception)
                {
                    // we couldn't send the buffers, so let's try to delete the temp file
                    try
                    {
                        session.DeleteFile(tempFileName);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Couldn't delete file after failed transfer: " + tempFileName);
                    }

                    throw;
                }

                session.Rename(tempFileName, fileName);
            }
            catch (Exception)
            {
                // we couldn't send the buffers, so we have to return them into
                // the queue, but of course in the right order
                lock (this.sendQueue)
                {
                    var allBuffers = new SendMessageBuffer[buffers.Length + this.sendQueue.Count];
                    Array.Copy(buffers, allBuffers, buffers.Length);
                    this.sendQueue.CopyTo(buffers, buffers.Length);
                    this.sendQueue.Clear();
                    foreach (var buffer in allBuffers)
                    {
                        this.sendQueue.Enqueue(buffer);
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Downloads all available files destined for this session from the
        /// drop location and enqueues them to be read later.
        /// </summary>
        /// <param name="session">
        /// The file transfer session to use for the download.
        /// </param>
        /// <param name="availableFiles">
        /// The files available on the remote server.
        /// </param>
        public void DownloadMessages(IFileTransferSession session, string[] availableFiles)
        {
            foreach (var file in availableFiles)
            {
                this.DownloadMessage(session, file);
            }

            MessageBuffer receiveBuffer;
            lock (this.receiveQueue)
            {
                if (this.receiveQueue.Count == 0 || this.readRequest == null)
                {
                    return;
                }

                receiveBuffer = this.receiveQueue.Dequeue();
            }

            // TODO: we need the codec ID somehow!
            var result = new MessageReadResult(receiveBuffer.Count, this.Session, null);
            this.readRequest.Complete(result, receiveBuffer, false);
        }

        private void DownloadMessage(IFileTransferSession session, string file)
        {
            var match = MessageNameRegex.Match(file);
            if (!match.Success
                || !string.Equals(
                        match.Groups[1].Value, this.SessionId.Hash, StringComparison.InvariantCultureIgnoreCase)
                || !string.Equals(
                        match.Groups[2].Value, this.localSessionId.Hash, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            DateTime timestamp;
            if (
                !ParserUtil.TryParseExact(
                    match.Groups[3].Value, TimeStampFormat, null, DateTimeStyles.AssumeUniversal, out timestamp))
            {
                return;
            }

            timestamp = timestamp.ToUniversalTime();
            if (timestamp < DateTime.UtcNow)
            {
                // the message has expired
                Logger.Info("Deleting expired message {0}", file);
                try
                {
                    session.DeleteFile(file);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't delete expired message: " + file );
                }

                return;
            }

            MessageBuffer buffer;
            try
            {
                using (var fileStream = session.OpenRead(file))
                {
                    buffer = new MessageBuffer((int)fileStream.Length + 1);
                    int offset = 0;
                    int read;
                    while ((read = fileStream.Read(buffer.Buffer, offset, buffer.Size - offset)) > 0)
                    {
                        buffer.Count += read;
                        offset += read;
                    }
                }

                session.DeleteFile(file);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't download message: " + file);
                return;
            }

            lock (this.receiveQueue)
            {
                // only enqueue after the file was successfully deleted
                this.receiveQueue.Enqueue(buffer);
            }
        }

        private void RaiseConnected(EventArgs e)
        {
            var handler = this.Connected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseSessionConnected(EventArgs e)
        {
            var handler = this.SessionConnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseDisconnected(EventArgs e)
        {
            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseMessageSendEnqueued(EventArgs e)
        {
            var handler = this.MessageSendEnqueued;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
