// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PipeServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transport.Stream
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Utility.Core.Async;

    /// <summary>
    /// <see cref="IStreamServer"/> implementation that accepts virtual
    /// connections from <see cref="PipeClient"/>.
    /// </summary>
    internal class PipeServer : IStreamServer
    {
        private readonly object waitLock = new object();
        private readonly List<PipeClient> clients = new List<PipeClient>();

        private readonly Queue<SimpleAsyncResult<IStreamConnection>> acceptWaits =
            new Queue<SimpleAsyncResult<IStreamConnection>>();

        private readonly Queue<ClientConnectAsyncResult> connectWaits = new Queue<ClientConnectAsyncResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeServer"/> class.
        /// </summary>
        /// <param name="pipeId">
        /// The pipe id.
        /// </param>
        public PipeServer(int pipeId)
        {
            this.PipeId = pipeId;
            this.Enabled = true;
        }

        /// <summary>
        /// Event that is fired when this object is being disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Gets the pipe id.
        /// </summary>
        public int PipeId { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this server accepts new
        /// connections. This property can be used to block clients from
        /// connecting to this server.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets after how many bytes the connection should be closed.
        /// This is used for testing to make sure connections can be reopened and transfers recovered.
        /// </summary>
        public int DisconnectAfterBytes { get; set; }

        /// <summary>
        /// Gets the currently connected clients.
        /// </summary>
        public IEnumerable<PipeClient> Clients
        {
            get
            {
                lock (this.clients)
                {
                    return this.clients.ToArray();
                }
            }
        }

        /// <summary>
        /// Connects asynchronously to this server.
        /// This method should only be called from <see cref="PipeClient"/>.
        /// </summary>
        /// <param name="client">
        /// The client that connects to this server.
        /// </param>
        /// <param name="callback">
        /// The callback that is called when the client is connected.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The result to use when completing this request with
        /// <see cref="EndConnect"/>.
        /// </returns>
        public IAsyncResult BeginConnect(PipeClient client, AsyncCallback callback, object state)
        {
            var request = new ClientConnectAsyncResult(client, callback, state);

            if (!this.Enabled)
            {
                ThreadPool.QueueUserWorkItem(
                    s => request.CompleteException(new IOException("Server refused connection"), false));
                return request;
            }

            SimpleAsyncResult<IStreamConnection> accept;
            lock (this.waitLock)
            {
                if (this.acceptWaits.Count == 0)
                {
                    this.connectWaits.Enqueue(request);
                    return request;
                }

                accept = this.acceptWaits.Dequeue();
            }

            IStreamConnection local;
            IStreamConnection remote;
            this.Connect(client, out local, out remote);

            ThreadPool.QueueUserWorkItem(s => accept.Complete(local, false));
            ThreadPool.QueueUserWorkItem(s => request.Complete(remote, false));
            return request;
        }

        /// <summary>
        /// Completes the connection process that was initiated with
        /// <see cref="BeginConnect"/>.
        /// This method should only be called from <see cref="PipeClient"/>.
        /// </summary>
        /// <param name="ar">
        /// The result returned by <see cref="BeginConnect"/>.
        /// </param>
        /// <returns>
        /// A new stream connection to this server.
        /// </returns>
        public IStreamConnection EndConnect(IAsyncResult ar)
        {
            var result = (SimpleAsyncResult<IStreamConnection>)ar;
            return result.Value;
        }

        /// <summary>
        /// Disconnects the given client from this server.
        /// This method should only be called from <see cref="PipeClient"/>.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public void Disconnect(PipeClient client)
        {
            lock (this.clients)
            {
                this.clients.Remove(client);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("PipeServer#{0}", this.PipeId);
        }

        void IDisposable.Dispose()
        {
            var handler = this.Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            PipeClient[] closePipes;

            lock (this.clients)
            {
                closePipes = this.clients.ToArray();
                this.clients.Clear();
            }

            foreach (IDisposable client in closePipes)
            {
                client.Dispose();
            }
        }

        IAsyncResult IStreamServer.BeginAccept(AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<IStreamConnection>(callback, state);
            ClientConnectAsyncResult connect;
            lock (this.waitLock)
            {
                if (this.connectWaits.Count == 0)
                {
                    this.acceptWaits.Enqueue(result);
                    return result;
                }

                connect = this.connectWaits.Dequeue();
            }

            IStreamConnection local;
            IStreamConnection remote;
            this.Connect(connect.Client, out local, out remote);

            ThreadPool.QueueUserWorkItem(s => connect.Complete(remote, false));
            ThreadPool.QueueUserWorkItem(s => result.Complete(local, false));
            return result;
        }

        IStreamConnection IStreamServer.EndAccept(IAsyncResult ar)
        {
            var result = (SimpleAsyncResult<IStreamConnection>)ar;
            return result.Value;
        }

        private void Connect(
            PipeClient client, out IStreamConnection localConnection, out IStreamConnection remoteConnection)
        {
            lock (this.clients)
            {
                this.clients.Add(client);
            }

            var localStream = new PipeStream { DisconnectAfterWrittenBytes = client.DisconnectAfterBytes };
            var remoteStream = new PipeStream { DisconnectAfterWrittenBytes = this.DisconnectAfterBytes };

            localStream.Connect(remoteStream);
            localConnection = new PipeConnection(this.PipeId + "@Server", localStream);
            remoteConnection = new PipeConnection(this.PipeId + "@Client", remoteStream);
        }

        private class ClientConnectAsyncResult : SimpleAsyncResult<IStreamConnection>
        {
            public ClientConnectAsyncResult(PipeClient client, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.Client = client;
            }

            public PipeClient Client { get; private set; }
        }
    }
}