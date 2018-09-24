// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerPeerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventHandlerPeerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Edi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Subscription;
    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Base class for EventHandler client and server peers.
    /// </summary>
    internal abstract class EventHandlerPeerBase : IPeerImpl
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private const string TaggedBegin = "<|1>";
        private const string TaggedEnd = "<|2>";

        private readonly object receiveLock = new object();

        private EventHandlerPeerConfigBase config;

        private Encoding encoding;

        private TypeName[] supportedMessages;

        private EventHandlerSubscription subscription;

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerPeerBase"/> class.
        /// </summary>
        protected EventHandlerPeerBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the message dispatcher.
        /// This property is only available after calling <see cref="Start"/>
        /// </summary>
        protected IMessageDispatcherImpl MessageDispatcher { get; private set; }

        /// <summary>
        /// Starts this peer.
        /// </summary>
        /// <param name="medi">
        /// The message dispatcher.
        /// </param>
        public void Start(IMessageDispatcherImpl medi)
        {
            this.Stop();

            this.running = true;
            this.MessageDispatcher = medi;

            this.subscription = new EventHandlerSubscription(this);

            this.DoStart();

            this.MessageDispatcher.AddSubscription(this.subscription);
        }

        /// <summary>
        /// Stops this peer.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;

            this.MessageDispatcher.RemoveSubscription(this.subscription);

            this.DoStop();
        }

        /// <summary>
        /// Configures this peer with the given config.
        /// This method needs to be called by subclasses before <see cref="Start"/> is called.
        /// </summary>
        /// <param name="peerConfig">
        /// The peer config.
        /// </param>
        protected void Configure(EventHandlerPeerConfigBase peerConfig)
        {
            this.config = peerConfig;
            this.encoding = Encoding.GetEncoding(this.config.Encoding);
            this.supportedMessages = this.config.SupportedMessages.ConvertAll(t => new TypeName(t)).ToArray();
        }

        /// <summary>
        /// Implementation of the <see cref="Start"/> method.
        /// </summary>
        protected abstract void DoStart();

        /// <summary>
        /// Implementation of the <see cref="Stop"/> method.
        /// </summary>
        protected abstract void DoStop();

        /// <summary>
        /// Sends a message to the connected peer.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        protected abstract void SendMessage(object message, byte[] data, int offset, int length);

        /// <summary>
        /// Gets an identifier for this peer.
        /// This identifier is used for logging purposes only and must be available
        /// after calling <see cref="Configure"/> but before calling <see cref="Start"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected abstract string GetIdentifier();

        private void SendMessage(object message)
        {
            try
            {
                byte[] bytes;
                var typeName = TypeName.GetNameFor(message);
                if (typeName.IsKnown)
                {
                    var serializer = new XmlSerializer(message.GetType());
                    var stream = new MemoryStream();
                    if (this.config.TaggedMode)
                    {
                        var tag = this.encoding.GetBytes(TaggedBegin);
                        stream.Write(tag, 0, tag.Length);
                    }

                    serializer.Serialize(stream, message);
                    if (this.config.TaggedMode)
                    {
                        var tag = this.encoding.GetBytes(TaggedEnd);
                        stream.Write(tag, 0, tag.Length);
                    }

                    bytes = stream.ToArray();

                    if (this.Logger.IsTraceEnabled)
                    {
                        this.Logger.Trace(
                            "Sending known object:\n{0}", this.encoding.GetString(bytes, 0, bytes.Length));
                    }
                }
                else
                {
                    var unknown = message as UnknownXmlObject;
                    if (unknown == null)
                    {
                        this.Logger.Error("Could not determine unknown object");
                        return;
                    }

                    var xml = unknown.Xml;
                    if (!xml.StartsWith("<?"))
                    {
                        xml = "<?xml version=\"1.0\"?>" + xml;
                    }

                    if (this.config.TaggedMode)
                    {
                        xml = TaggedBegin + xml + TaggedEnd;
                    }

                    bytes = this.encoding.GetBytes(xml);

                    this.Logger.Trace("Sending unknown object:\n{0}", xml);
                }

                this.SendMessage(message, bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't send message");
            }
        }

        /// <summary>
        /// The handler for a single connection between two EventHandler peers.
        /// </summary>
        protected class ConnectionHandler : IManageableObject
        {
            private static readonly Logger Logger = LogHelper.GetLogger<ConnectionHandler>();

            private readonly byte[] readBuffer = new byte[40960];

            private readonly IStreamConnection connection;
            private readonly EventHandlerPeerBase peer;

            private readonly ProducerConsumerQueue<MessageBuffer> writeQueue;

            private readonly Regex messagePattern;

            private string readString = string.Empty;

            private object receivingMessage;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionHandler"/> class.
            /// </summary>
            /// <param name="connection">
            /// The underlying connection.
            /// </param>
            /// <param name="peer">
            /// The peer which creates this handler.
            /// </param>
            public ConnectionHandler(IStreamConnection connection, EventHandlerPeerBase peer)
            {
                this.connection = connection;
                this.peer = peer;
                this.writeQueue = new ProducerConsumerQueue<MessageBuffer>(this.Write, 100);

                var expression =
                    @"<\?xml(.*?)\?>\s*((<([A-Za-z][A-Za-z0-9]*)[^>]*>(.*?)</\4>)|(<([A-Za-z][A-Za-z0-9]*)[^>]*/>))";
                if (this.peer.config.TaggedMode)
                {
                    expression = Regex.Escape(TaggedBegin) + expression + Regex.Escape(TaggedEnd);
                }

                this.messagePattern = new Regex(expression, RegexOptions.Singleline);

                Logger.Info("Client connected: {0}", this.connection);
            }

            /// <summary>
            /// Event that is fired when the underlying connection fails.
            /// </summary>
            public event EventHandler Failed;

            /// <summary>
            /// Starts this handler.
            /// This will start reading from and write to the underlying connection.
            /// </summary>
            public void Start()
            {
                this.BeginRead();
                this.writeQueue.StartConsumer();
            }

            /// <summary>
            /// Stops this handler.
            /// </summary>
            public void Stop()
            {
                Logger.Info("Closing connection to {0}", this.connection);
                this.writeQueue.StopConsumer();
                this.connection.Dispose();
            }

            /// <summary>
            /// Enqueues new data to be written to the underlying connection.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <param name="data">
            /// The data to be written.
            /// </param>
            /// <param name="offset">
            /// The offset.
            /// </param>
            /// <param name="count">
            /// The count.
            /// </param>
            public void EnqueueWrite(object message, byte[] data, int offset, int count)
            {
                if (object.ReferenceEquals(this.receivingMessage, message))
                {
                    // the message was from us, so let's ignore it
                    return;
                }

                if (!this.writeQueue.Enqueue(new MessageBuffer(data, offset, count)))
                {
                    this.HandleExceptionAndClose("Write queue is full", new IOException());
                }
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override string ToString()
            {
                return this.connection.ToString();
            }

            IEnumerable<ManagementProperty> IManageableObject.GetProperties()
            {
                yield return new ManagementProperty<string>("Remote", this.connection.ToString(), true);
            }

            IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
            {
                yield break;
            }

            /// <summary>
            /// Raises the <see cref="Failed"/> event.
            /// </summary>
            /// <param name="e">
            /// The event arguments.
            /// </param>
            protected virtual void RaiseFailed(EventArgs e)
            {
                var handler = this.Failed;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            private void Write(MessageBuffer buffer)
            {
                Logger.Trace("Sending {0} bytes to {1}", buffer.Count, this.connection);

                try
                {
                    this.connection.Stream.Write(buffer.Buffer, buffer.Offset, buffer.Count);
                    Logger.Trace("Sent {0} bytes to {1}", buffer.Count, this.connection);
                }
                catch (Exception ex)
                {
                    this.HandleExceptionAndClose("Writing to client failed", ex);
                }
            }

            private void BeginRead()
            {
                try
                {
                    this.connection.Stream.BeginRead(this.readBuffer, 0, this.readBuffer.Length, this.Read, null);
                }
                catch (Exception ex)
                {
                    this.HandleExceptionAndClose("Begin reading from client failed", ex);
                }
            }

            private void Read(IAsyncResult result)
            {
                int read;
                try
                {
                    read = this.connection.Stream.EndRead(result);
                    if (read == 0)
                    {
                        throw new EndOfStreamException("Read zero bytes");
                    }
                }
                catch (Exception ex)
                {
                    this.HandleExceptionAndClose("Reading from client failed", ex);
                    return;
                }

                // unforunate, but since Regex only accepts strings, this is
                // actually the most efficient way
                this.readString += this.peer.encoding.GetString(this.readBuffer, 0, read);

                this.CheckMessageReceived();
                this.BeginRead();
            }

            private void CheckMessageReceived()
            {
                while (true)
                {
                    var match = this.messagePattern.Match(this.readString);
                    if (!match.Success)
                    {
                        return;
                    }

                    var type = match.Groups[match.Groups[7].Success ? 7 : 4].Value;

                    {
                        this.ReceiveMessage(type, match.Groups[2].Value);
                    }

                    this.readString = this.readString.Substring(match.Index + match.Length);
                }
            }

            private void ReceiveMessage(string messageType, string xml)
            {
                try
                {
                    var ending = "." + messageType;
                    var fullName = this.peer.config.SupportedMessages.Find(m => m.EndsWith(ending));
                    if (fullName == null)
                    {
                        Logger.Error("Received unknown message type {0}", messageType);
                        return;
                    }

                    Logger.Trace("Received message:\n{0}", xml);

                    var typeName = new TypeName(fullName);
                    object message;
                    if (typeName.IsKnown)
                    {
                        var serializer = new XmlSerializer(typeName.Type);
                        message = serializer.Deserialize(new StringReader(xml));
                    }
                    else
                    {
                        message = new UnknownXmlObject(typeName, xml);
                    }

                    lock (this.peer.receiveLock)
                    {
                        this.receivingMessage = message;
                        this.peer.MessageDispatcher.Broadcast(message);
                        this.receivingMessage = null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Couldn't receive message " + messageType);
                }
            }

            private void HandleExceptionAndClose(string message, Exception ex)
            {
                Logger.Warn(ex, message);

                this.RaiseFailed(EventArgs.Empty);

                this.Stop();
            }
        }

        private class EventHandlerSubscription : Subscription
        {
            private readonly EventHandlerPeerBase peer;

            public EventHandlerSubscription(EventHandlerPeerBase peer)
                : base(peer.MessageDispatcher.LocalAddress, peer.supportedMessages)
            {
                this.peer = peer;
            }

            public override void Handle(
                IMessageDispatcherImpl medi,
                ISessionId sourceSessionId,
                MediAddress source,
                MediAddress destination,
                object message)
            {
                this.peer.SendMessage(message);
            }

            public override string ToString()
            {
                return string.Format("EventHandlerSubscription<{0}>", this.peer.GetIdentifier());
            }
        }
    }
}