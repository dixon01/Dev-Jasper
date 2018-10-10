// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportServer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransportServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Streams;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core.Async;

    using NLog;

    /// <summary>
    /// Abstract base implementation of <see cref="ITransportServer"/> that
    /// can be sub-classed by transport implementations that contain a server
    /// part.
    /// </summary>
    internal abstract class TransportServer : ITransportServer, IManageable
    {
        /// <summary>
        /// The logger for this class.
        /// </summary>
        protected readonly Logger Logger;

        private readonly List<Client> clients = new List<Client>();
        private readonly ReadWriteLock clientsLock = new ReadWriteLock();

        private readonly AutoResetEvent messageReadWait = new AutoResetEvent(false);
        private readonly AutoResetEvent streamReadWait = new AutoResetEvent(false);

        private ReadMessageRequest messageReadRequest;
        private SimpleAsyncResult<StreamReadResult> streamReadRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportServer"/> class.
        /// </summary>
        protected TransportServer()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
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
        /// Interface for a <see cref="IMessageTransport"/> which is able
        /// to handle client stuff.
        /// </summary>
        internal interface IClientMessageTransport : IMessageTransport
        {
            /// <summary>
            /// This event is fired when this transport is connected.
            /// </summary>
            event EventHandler Connected;

            /// <summary>
            /// This event is fired in addition to (and after) <see cref="Connected"/>
            /// when this transport is connected with a new session.
            /// </summary>
            event EventHandler SessionConnected;

            /// <summary>
            /// This event is fired when this transport is disconnected.
            /// </summary>
            event EventHandler Disconnected;

            /// <summary>
            /// Gets the session associated to this client.
            /// </summary>
            ITransportSession Session { get; }
        }

        /// <summary>
        /// Configures this transport server with the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public abstract void Configure(TransportServerConfig config);

        /// <summary>
        /// Starts the transport implementation, connecting it with the given codec.
        /// </summary>
        /// <param name="medi">
        ///     The local message dispatcher implementation
        /// </param>
        /// <param name="messageTranscoder">
        ///     The message transcoder that is on top of this transport.
        /// </param>
        public abstract void Start(IMessageDispatcherImpl medi, MessageTranscoder messageTranscoder);

        /// <summary>
        /// Stops the transport and releases all resources.
        /// </summary>
        public virtual void Stop()
        {
            Client[] array;
            using (this.clientsLock.AcquireReadLock())
            {
                array = this.clients.ToArray();
            }

            foreach (var client in array)
            {
                this.RemoveClient(client);
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
        /// An async result that can be used in <see cref="IMessageTransport.EndReadMessage"/>.
        /// </returns>
        public virtual IAsyncResult BeginReadMessage(
            IReadBufferProvider bufferProvider, AsyncCallback callback, object state)
        {
            if (this.messageReadRequest != null)
            {
                throw new NotSupportedException("Cannot have more than one outstanding message read request");
            }

            // put the handling of the callback into the thread pool, so the reader doesn't get blocked
            // if the decoding/handling of the message takes some time; we have to do it here because
            // the ReadMessageRequest.Complete() must be called synchronously (because the reader is reusing
            // the same buffer)
            AsyncCallback async = ar => ThreadPool.QueueUserWorkItem(s => callback(ar));

            this.messageReadRequest = new ReadMessageRequest(bufferProvider, async, state);
            this.messageReadWait.Set();
            return this.messageReadRequest;
        }

        /// <summary>
        /// Ends the async request issued by <see cref="IMessageTransport.BeginReadMessage"/>.
        /// </summary>
        /// <param name="result">
        ///   The result returned by <see cref="IMessageTransport.BeginReadMessage"/>.
        /// </param>
        /// <returns>
        /// The read result including the session from which the data was received.
        /// </returns>
        public virtual MessageReadResult EndReadMessage(IAsyncResult result)
        {
            if (result == null || result != this.messageReadRequest)
            {
                throw new ArgumentException("Expected ReadMessageRequest", "result");
            }

            var readResult = this.messageReadRequest.ReadResult;
            this.messageReadRequest = null;
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
        public virtual void WriteMessage(IMessageBufferProvider bufferProvider, ISessionId destinationSessionId)
        {
            Client[] array;
            using (this.clientsLock.AcquireReadLock())
            {
                array = this.clients.ToArray();
            }

            foreach (var client in array)
            {
                client.WriteMessage(bufferProvider, destinationSessionId);
            }
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
        public virtual IAsyncResult BeginReadStream(AsyncCallback callback, object state)
        {
            if (this.streamReadRequest != null)
            {
                throw new NotSupportedException("Cannot have more than one outstanding stream read request");
            }

            this.streamReadRequest = new SimpleAsyncResult<StreamReadResult>(callback, state);
            this.streamReadWait.Set();
            return this.streamReadRequest;
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
        public virtual StreamReadResult EndReadStream(IAsyncResult result)
        {
            if (result == null || result != this.streamReadRequest)
            {
                throw new ArgumentException("Expected result from BeginReadStream()", "result");
            }

            var readResult = this.streamReadRequest.Value;
            this.streamReadRequest = null;
            return readResult;
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
        public virtual void WriteStream(StreamMessage message, ISessionId destinationSessionId)
        {
            Client[] array;
            using (this.clientsLock.AcquireReadLock())
            {
                array = this.clients.ToArray();
            }

            foreach (var client in array)
            {
                client.WriteStream(message, destinationSessionId);
            }
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
        public void PreviewDecodedMessage(ITransportSession session, ref MediMessage message)
        {
            IClientMessageTransport transport = null;
            using (this.clientsLock.AcquireReadLock())
            {
                foreach (var client in this.clients)
                {
                    if (client.Transport != null && client.Transport.Session != null
                        && client.Transport.Session.SessionId.Equals(session.SessionId))
                    {
                        transport = client.Transport;
                        break;
                    }
                }
            }

            if (transport != null)
            {
                transport.PreviewDecodedMessage(session, ref message);
            }
        }

        /// <summary>
        /// Gets all <see cref="ManagementProperty"/> objects for this object.
        /// This implementations only has one property called "ClientCount".
        /// </summary>
        /// <returns>
        /// all properties.
        /// </returns>
        public virtual IEnumerable<ManagementProperty> GetProperties()
        {
            yield return new ManagementProperty<int>("ClientCount", this.clients.Count, true);
        }

        /// <summary>
        /// Gets all management children for this object.
        /// Children of this objects are all client connections (sessions).
        /// </summary>
        /// <param name="parent">
        /// The parent provider object.
        /// </param>
        /// <returns>
        /// all children.
        /// </returns>
        public virtual IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent)
        {
            Client[] array;
            using (this.clientsLock.AcquireReadLock())
            {
                array = this.clients.ToArray();
            }

            int i = 0;
            foreach (var client in array)
            {
                yield return parent.Factory.CreateManagementProvider("Client" + i, parent, client.Transport);
                i++;
            }
        }

        /// <summary>
        /// Adds a new client to the list of known clients.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        protected void AddClient(Client client)
        {
            using (this.clientsLock.AcquireWriteLock())
            {
                foreach (var other in this.clients)
                {
                    if (object.ReferenceEquals(other.Transport.Session, client.Transport.Session))
                    {
                        throw new ArgumentException("Can't add the same session as a new client");
                    }
                }

                this.clients.Add(client);
            }
        }

        /// <summary>
        /// Removes a client from the list of known clients and calls its
        /// <see cref="Client.Dispose"/> method.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        protected void RemoveClient(Client client)
        {
            using (this.clientsLock.AcquireWriteLock())
            {
                this.clients.Remove(client);
            }

            client.Dispose();
        }

        /// <summary>
        /// Fires the <see cref="Started"/> event when the transport has finished starting up.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStarted(EventArgs e)
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the <see cref="SessionConnected"/> event.
        /// </summary>
        /// <param name="e">
        /// The session event arguments.
        /// </param>
        protected virtual void RaiseSessionConnected(SessionEventArgs e)
        {
            var handler = this.SessionConnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void MessageRead(MessageReadResult readResult, MessageBuffer buffer)
        {
            this.messageReadWait.WaitOne();

            if (this.messageReadRequest == null)
            {
                this.Logger.Error("Finished reading message, but no outstanding request");
                return;
            }

            this.messageReadRequest.Complete(readResult, buffer, false);
        }

        private void StreamRead(StreamReadResult readResult)
        {
            this.streamReadWait.WaitOne();

            if (this.streamReadWait == null)
            {
                this.Logger.Error("Finished reading message, but no outstanding request");
                return;
            }

            this.streamReadRequest.Complete(readResult, false);
        }

        /// <summary>
        /// Container for a client connection.
        /// <see cref="TransportServer"/> keeps a list of all connected <see cref="Client"/>s.
        /// They are only for use inside this class and its subclasses.
        /// </summary>
        protected class Client : IReadBufferProvider, IDisposable
        {
            private readonly TransportServer server;

            private readonly MessageBuffer buffer = new MessageBuffer();

            private bool disposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="Client"/> class.
            /// </summary>
            /// <param name="server">
            /// Reference to the server who created this object.
            /// </param>
            /// <param name="transport">
            /// The message transport which this client uses to send and receive data.
            /// </param>
            public Client(TransportServer server, IClientMessageTransport transport)
            {
                this.server = server;
                this.Transport = transport;

                transport.Connected += this.TransportOnConnected;
                transport.SessionConnected += this.TransportOnSessionConnected;
            }

            ~Client()
            {
                this.Dispose(false);
            }

            /// <summary>
            /// Gets the transport session which this client represents.
            /// </summary>
            public IClientMessageTransport Transport { get; private set; }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
            }

            /// <summary>
            /// Write a message to the underlying transport session.
            /// </summary>
            /// <param name="bufferProvider">
            /// The buffer provider given from the <see cref="IMessageCodec"/>.
            /// </param>
            /// <param name="destinationSessionId">
            /// The id of the session to which the given buffers must be written.
            /// </param>
            public void WriteMessage(IMessageBufferProvider bufferProvider, ISessionId destinationSessionId)
            {
                if (this.disposed)
                {
                    return;
                }

                try
                {
                    this.Transport.WriteMessage(bufferProvider, destinationSessionId);
                }
                catch (Exception ex)
                {
                    this.HandleException("Error while writing message", ex);
                }
            }

            /// <summary>
            /// Write a stream to the underlying transport session.
            /// </summary>
            /// <param name="message">
            /// The stream message to be written.
            /// </param>
            /// <param name="destinationSessionId">
            /// The id of the session to which the given stream message must be written.
            /// </param>
            public void WriteStream(StreamMessage message, ISessionId destinationSessionId)
            {
                if (this.disposed)
                {
                    return;
                }

                try
                {
                    this.Transport.WriteStream(message, destinationSessionId);
                }
                catch (Exception ex)
                {
                    this.HandleException("Error while writing stream", ex);
                }
            }

            MessageBuffer IReadBufferProvider.GetReadBuffer(ITransportSession session)
            {
                return this.buffer;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <param name="disposing">
            /// Whether the object is being disposed by the application or by the GC.
            /// </param>
            protected virtual void Dispose(bool disposing)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;

                if (!disposing)
                {
                    return;
                }

                this.Transport.Connected -= this.TransportOnConnected;
                this.Transport.SessionConnected -= this.TransportOnSessionConnected;

                if (this.Transport.Session != null)
                {
                    this.Transport.Session.Disconnected -= this.SessionOnDisconnected;
                }

                this.Transport.Stop();
            }

            /// <summary>
            /// Starts reading a message from the underlying transport session.
            /// </summary>
            protected void BeginReadMessage()
            {
                this.buffer.Clear();
                try
                {
                    this.Transport.BeginReadMessage(this, this.MessageRead, null);
                }
                catch (Exception ex)
                {
                    this.HandleException("Error while beginning to read message", ex);
                }
            }

            /// <summary>
            /// Starts reading a message from the underlying transport session.
            /// </summary>
            protected void BeginReadStream()
            {
                try
                {
                    this.Transport.BeginReadStream(this.StreamRead, null);
                }
                catch (Exception ex)
                {
                    this.HandleException("Error while beginning to read stream", ex);
                }
            }

            /// <summary>
            /// Callback that is called when a message was read from the
            /// underlying transport session.
            /// </summary>
            /// <param name="result">
            /// The result.
            /// </param>
            protected virtual void MessageRead(IAsyncResult result)
            {
                try
                {
                    var readResult = this.Transport.EndReadMessage(result);
                    this.PostMessageRead(readResult, this.buffer);
                }
                catch (Exception ex)
                {
                    this.HandleException(
                        "Error while reading message from " + this.Transport.Session.SessionId, ex);
                    return;
                }

                if (this.disposed)
                {
                    return;
                }

                this.BeginReadMessage();
            }

            /// <summary>
            /// Callback that is called when a stream was read from the
            /// underlying transport session.
            /// </summary>
            /// <param name="result">
            /// The result.
            /// </param>
            protected virtual void StreamRead(IAsyncResult result)
            {
                try
                {
                    var readResult = this.Transport.EndReadStream(result);
                    this.PostStreamRead(readResult);
                }
                catch (Exception ex)
                {
                    this.HandleException(
                        "Error while reading stream from " + this.Transport.Session.SessionId, ex);
                    return;
                }

                if (this.disposed)
                {
                    return;
                }

                this.BeginReadStream();
            }

            /// <summary>
            /// Posts the read message to the server that created this client.
            /// </summary>
            /// <param name="readResult">
            /// The read result.
            /// </param>
            /// <param name="readBuffer">
            /// The buffer that was read from the underlying transport session.
            /// </param>
            protected virtual void PostMessageRead(MessageReadResult readResult, MessageBuffer readBuffer)
            {
                this.server.MessageRead(readResult, readBuffer);
            }

            /// <summary>
            /// Posts the read stream to the server that created this client.
            /// </summary>
            /// <param name="readResult">
            /// The read result.
            /// </param>
            protected virtual void PostStreamRead(StreamReadResult readResult)
            {
                this.server.StreamRead(readResult);
            }

            private void TransportOnConnected(object sender, EventArgs e)
            {
                this.BeginReadMessage();
                this.BeginReadStream();
            }

            private void TransportOnSessionConnected(object sender, EventArgs e)
            {
                this.Transport.Session.Disconnected += this.SessionOnDisconnected;
                this.server.RaiseSessionConnected(new SessionEventArgs(this.Transport.Session));
            }

            private void SessionOnDisconnected(object sender, EventArgs eventArgs)
            {
                this.server.RemoveClient(this);
            }

            private void HandleException(string message, Exception ex)
            {
                this.server.Logger.Error(ex, message);
                this.Transport.Stop();
            }
        }
    }
}