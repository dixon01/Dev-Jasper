// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpStreamServer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpStreamServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Tcp
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;

    using NLog;

    /// <summary>
    /// Stream server implementation for TCP.
    /// </summary>
    internal class TcpStreamServer : IStreamServer
    {
        private static readonly Logger Logger = LogHelper.GetLogger<TcpStreamServer>();

        private readonly Socket serverSocket;

        private readonly IPEndPoint endPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpStreamServer"/> class.
        /// </summary>
        /// <param name="localPort">
        /// The local TCP port.
        /// </param>
        public TcpStreamServer(int localPort)
        {
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.endPoint = new IPEndPoint(IPAddress.Any, localPort);
            this.serverSocket.Bind(this.endPoint);
            this.serverSocket.Listen(10);
        }

        /// <summary>
        /// Accepts asynchronously a remote request from a <see cref="IStreamClient"/>.
        /// </summary>
        /// <param name="callback">
        /// The callback that is called when a client connected.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The result to use when completing this request with
        /// <see cref="EndAccept"/>.
        /// </returns>
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<IStreamConnection>(callback, state);
            this.serverSocket.BeginAccept(this.Accepted, result);

            return result;
        }

        /// <summary>
        /// Completes the accept process that was initiated with
        /// <see cref="BeginAccept"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="BeginAccept"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="TcpStreamConnection"/> to the remote client.
        /// </returns>
        public IStreamConnection EndAccept(IAsyncResult result)
        {
            var asyncResult = (SimpleAsyncResult<IStreamConnection>)result;
            return asyncResult.Value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.serverSocket.Close();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("TCP Server on port {0}", this.endPoint.Port);
        }

        private void Accepted(IAsyncResult ar)
        {
            SimpleAsyncResult<IStreamConnection> asyncResult = null;
            try
            {
                asyncResult = (SimpleAsyncResult<IStreamConnection>)ar.AsyncState;
                var clientSocket = this.serverSocket.EndAccept(ar);
                asyncResult.Complete(new TcpStreamConnection(clientSocket), false);
            }
            catch (Exception ex)
            {
                if (asyncResult != null)
                {
                    if (asyncResult.TryCompleteException(
                        new IOException("Couldn't accept client connection", ex), false))
                    {
                        return;
                    }
                }

                Logger.Warn(ex, "Couldn't accept client connection");
            }
        }
    }
}
