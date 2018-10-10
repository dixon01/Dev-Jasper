// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTranscoder.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageTranscoder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Codec
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Transcoder that converts <see cref="MediMessage"/>s to <see cref="MessageBuffer"/>s
    /// and vice versa using an <see cref="IMessageCodec"/>. This class handles the
    /// wiring between the <see cref="IMessageCodec"/> and the underlying
    /// <see cref="IMessageTransport"/>.
    /// </summary>
    internal sealed class MessageTranscoder : IManageableObject
    {
        private static readonly Logger Logger = LogHelper.GetLogger<MessageTranscoder>();

        private readonly BufferProvider bufferProvider = new BufferProvider();

        private bool stop;

        private int totalMessagesEncoded;
        private int totalMessagesDecoded;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTranscoder"/> class.
        /// </summary>
        /// <param name="codec">
        /// The codec that will be used by this object.
        /// </param>
        public MessageTranscoder(IMessageCodec codec)
        {
            this.Codec = codec;
        }

        /// <summary>
        /// Event that is risen whenever message has successfully been decoded.
        /// </summary>
        public event EventHandler<MediMessageEventArgs> MessageDecoded;

        /// <summary>
        /// Gets the codec used to encode and decode messages.
        /// </summary>
        public IMessageCodec Codec { get; private set; }

        /// <summary>
        /// Encodes a given message and sends it to the given transport using a filter.
        /// </summary>
        /// <param name="message">
        /// The message to be encoded.
        /// </param>
        /// <param name="transport">
        /// The transport to which the encoded message will be sent.
        /// </param>
        /// <param name="destinationSessionId">
        /// The id of the session to which the message will actually be sent.
        /// </param>
        public void Encode(MediMessage message, IMessageTransport transport, ISessionId destinationSessionId)
        {
            var provider = this.Codec.Encode(message);
            Logger.Debug("Message encoded: {0}", message.Payload);
            transport.WriteMessage(provider, destinationSessionId);
            this.totalMessagesEncoded++;
        }

        /// <summary>
        /// Asynchronously starts decoding messages.
        /// </summary>
        /// <param name="transport">
        /// The transport from which <see cref="MessageBuffer"/>s will be read.
        /// </param>
        public void StartDecode(IMessageTransport transport)
        {
            this.stop = false;
            this.bufferProvider.Clear();
            transport.BeginReadMessage(this.bufferProvider, this.MessageRead, transport);
        }

        /// <summary>
        /// Stops the decoding after the next message has been received (or an error happened).
        /// </summary>
        public void StopDecode()
        {
            this.stop = true;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<int>("TotalMessagesDecoded", this.totalMessagesDecoded, true);

            yield return new ManagementProperty<int>("TotalMessagesEncoded", this.totalMessagesEncoded, true);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider(this.Codec.GetType().Name, parent, this.Codec);
        }

        private void RaiseMessageDecoded(MediMessageEventArgs e)
        {
            var handler = this.MessageDecoded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void MessageRead(IAsyncResult result)
        {
            var transport = result.AsyncState as IMessageTransport;
            if (transport == null)
            {
                Logger.Error("Got MessageRead() without a transport, aborting this transcoder");
                this.StopDecode();
                return;
            }

            MessageReadResult readResult = null;
            try
            {
                readResult = transport.EndReadMessage(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception while end reading message");
            }

            this.Decode(readResult);

            if (!this.stop)
            {
                transport.BeginReadMessage(this.bufferProvider, this.MessageRead, transport);
            }
        }

        private void Decode(MessageReadResult readResult)
        {
            if (readResult == null)
            {
                return;
            }

            MessageBuffer readBuffer;
            if (!this.bufferProvider.TryGetBuffer(readResult.Session.SessionId, out readBuffer))
            {
                Logger.Warn("Couldn't find read buffer for {0}", readResult.Session.SessionId);
                return;
            }

            Logger.Debug(
                "Read {0} bytes from {1}; now at {2} bytes",
                readResult.BytesRead,
                readResult.Session.SessionId,
                readBuffer.Count);

            int lastCount = 0;
            while (readBuffer.Count > 0 && readBuffer.Count != lastCount)
            {
                lastCount = readBuffer.Count;

                MediMessage message = null;
                try
                {
                    message = this.Codec.Decode(readBuffer, readResult);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Exception while decoding message");
                }

                if (message != null)
                {
                    this.totalMessagesDecoded++;
                    Logger.Debug("Message decoded: {0}", message.Payload);
                    this.RaiseMessageDecoded(new MediMessageEventArgs(readResult.Session, message));
                }
            }
        }

        private class BufferProvider : IReadBufferProvider
        {
            private readonly Dictionary<ISessionId, MessageBuffer> readBuffers =
                new Dictionary<ISessionId, MessageBuffer>();

            public void Clear()
            {
                lock (this.readBuffers)
                {
                    this.readBuffers.Clear();
                }
            }

            public bool TryGetBuffer(ISessionId sessionId, out MessageBuffer readBuffer)
            {
                lock (this.readBuffers)
                {
                    return this.readBuffers.TryGetValue(sessionId, out readBuffer);
                }
            }

            public MessageBuffer GetReadBuffer(ITransportSession session)
            {
                lock (this.readBuffers)
                {
                    MessageBuffer buffer;
                    if (this.readBuffers.TryGetValue(session.SessionId, out buffer))
                    {
                        return buffer;
                    }

                    buffer = new MessageBuffer();
                    session.Disconnected += this.SessionOnDisconnected;
                    this.readBuffers.Add(session.SessionId, buffer);
                    return buffer;
                }
            }

            private void SessionOnDisconnected(object sender, EventArgs eventArgs)
            {
                var session = sender as ITransportSession;
                if (session == null)
                {
                    return;
                }

                session.Disconnected -= this.SessionOnDisconnected;
                lock (this.readBuffers)
                {
                    this.readBuffers.Remove(session.SessionId);
                }
            }
        }
    }
}
