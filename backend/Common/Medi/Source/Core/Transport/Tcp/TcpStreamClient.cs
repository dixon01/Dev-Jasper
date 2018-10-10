// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpStreamClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpStreamClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Tcp
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;

    using NLog;

    /// <summary>
    /// Stream client implementation for TCP protocol.
    /// </summary>
    internal class TcpStreamClient : IStreamClient
    {
        private static readonly Logger Logger = LogHelper.GetLogger<TcpStreamClient>();

        private readonly string remoteHost;
        private readonly int remotePort;

        private readonly Socket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpStreamClient"/> class.
        /// </summary>
        /// <param name="remoteHost">
        /// The remote host name or IP address.
        /// </param>
        /// <param name="remotePort">
        /// The remote TCP port.
        /// </param>
        public TcpStreamClient(string remoteHost, int remotePort)
        {
            this.remoteHost = remoteHost;
            this.remotePort = remotePort;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Connects asynchronously to a remote <see cref="IStreamServer"/>.
        /// </summary>
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
        public IAsyncResult BeginConnect(AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<IStreamConnection>(callback, state);

            IPAddress address;
            if (ParserUtil.TryParse(this.remoteHost, out address))
            {
                this.socket.BeginConnect(new IPEndPoint(address, this.remotePort), this.Connected, result);
            }
            else
            {
                Dns.BeginGetHostEntry(this.remoteHost, this.HostResolved, result);
            }

            return result;
        }

        /// <summary>
        /// Completes the connection process that was initiated with
        /// <see cref="IStreamClient.BeginConnect"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="IStreamClient.BeginConnect"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="TcpStreamConnection"/> to the remote server.
        /// </returns>
        public IStreamConnection EndConnect(IAsyncResult result)
        {
            var asyncResult = (SimpleAsyncResult<IStreamConnection>)result;
            return asyncResult.Value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.socket.Close();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("TCP<{0}:{1}>", this.remoteHost, this.remotePort);
        }

        private void HostResolved(IAsyncResult ar)
        {
            var result = (SimpleAsyncResult<IStreamConnection>)ar.AsyncState;
            try
            {
                IPAddress address = null;
                foreach (var addr in Dns.EndGetHostEntry(ar).AddressList)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        address = addr;
                        break;
                    }
                }

                if (address == null)
                {
                    throw new IOException("Host not found: " + this.remoteHost);
                }

                Logger.Debug("Resolved {0} to {1}", this.remoteHost, address);
                this.socket.BeginConnect(new IPEndPoint(address, this.remotePort), this.Connected, result);
            }
            catch (Exception ex)
            {
                result.CompleteException(new IOException("Couldn't resolve host name", ex), false);
            }
        }

        private void Connected(IAsyncResult ar)
        {
            SimpleAsyncResult<IStreamConnection> result = null;
            try
            {
                result = (SimpleAsyncResult<IStreamConnection>)ar.AsyncState;
                this.socket.EndConnect(ar);
                result.Complete(new TcpStreamConnection(this.socket), false);
            }
            catch (Exception ex)
            {
                if (result != null)
                {
                    if (result.TryCompleteException(new IOException("Couldn't connect to remote host", ex), false))
                    {
                        return;
                    }
                }

                Logger.Warn(ex, "Couldn't connect to remote host " + this.remoteHost);
            }
        }
    }
}