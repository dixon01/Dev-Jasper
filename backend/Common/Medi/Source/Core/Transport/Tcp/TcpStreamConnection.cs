// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpStreamConnection.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpStreamConnection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Tcp
{
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// Stream connection using a TCP/IP socket.
    /// </summary>
    internal class TcpStreamConnection : IStreamConnection
    {
        private readonly Socket socket;

        private readonly NetworkStream stream;

        private readonly EndPoint remoteEndPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpStreamConnection"/> class.
        /// </summary>
        /// <param name="socket">
        /// The socket.
        /// </param>
        public TcpStreamConnection(Socket socket)
        {
            this.socket = socket;
            this.stream = new NetworkStream(socket, true);
            this.remoteEndPoint = socket.RemoteEndPoint;
        }

        /// <summary>
        /// Gets the underlying <see cref="NetworkStream"/>.
        /// </summary>
        public Stream Stream
        {
            get
            {
                return this.stream;
            }
        }

        /// <summary>
        /// Creates a more or less unique ID for this connection.
        /// </summary>
        /// <returns>
        /// The pseudo-unique ID.
        /// </returns>
        public int CreateId()
        {
            var localAddr = (IPEndPoint)this.socket.LocalEndPoint;
            var remoteAddr = (IPEndPoint)this.socket.RemoteEndPoint;
            return localAddr.Port ^ remoteAddr.Port ^ remoteAddr.Address.GetAddressBytes()[1];
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.stream.Close();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("TCP: {0}", this.remoteEndPoint);
        }
    }
}