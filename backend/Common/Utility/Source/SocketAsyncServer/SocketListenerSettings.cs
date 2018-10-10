// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketListenerSettings.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Contains all needed inforation to handle the listenr socket.
//   For details about SAEA, please see <see cref="SocketAsyncEventArgs" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Contains all needed inforation to handle the listenr socket.
    /// For details about SAEA, please see <see cref="SocketAsyncEventArgs"/>
    /// </summary>
    internal class SocketListenerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocketListenerSettings"/> class.
        /// </summary>
        /// <param name="maxConnections">
        /// The max connections.
        /// </param>
        /// <param name="backlog">
        /// The backlog parameter specifies the number of incoming connections that can be queued for acceptance. 
        /// </param>
        /// <param name="receiveBufferSize">
        /// The receive buffer size.
        /// </param>
        /// <param name="opsToPreAlloc">
        /// The ops to pre alloc.
        /// </param>
        /// <param name="theLocalEndPoint">
        /// The the local end point.
        /// </param>
        public SocketListenerSettings(int maxConnections, int backlog, int receiveBufferSize, int opsToPreAlloc, IPEndPoint theLocalEndPoint)
        {
            this.MaxConnections = maxConnections;
            this.NumberOfSaeaForRecSend = maxConnections * opsToPreAlloc;
            this.Backlog = backlog;
            this.ReceiveBufferSize = receiveBufferSize;
            this.OpsToPreAllocate = opsToPreAlloc;
            this.LocalEndPoint = theLocalEndPoint;
        }

        /// <summary>
        /// Gets the maximum number of connections the sample is designed to handle simultaneously 
        /// </summary>
        public int MaxConnections { get; private set; }

        /// <summary>
        /// Gets this variable allows us to create some extra SAEA objects for the pool, if we wish.
        /// </summary>
        public int NumberOfSaeaForRecSend { get; private set; }

        /// <summary>
        /// Gets the maximum number of pending connections the listener can hold in queue
        /// </summary>
        public int Backlog { get; private set; }

        /// <summary>
        /// Gets the buffer size to use for each socket receive operation
        /// </summary>
        public int ReceiveBufferSize { get; private set; }

        /// <summary>
        /// Gets the number of pre allocated operations. See comments in buffer manager.
        /// </summary>
        public int OpsToPreAllocate { get; private set; }

        /// <summary>
        /// Gets the <see cref="IPEndPoint"/> for the listener.
        /// </summary>
        public IPEndPoint LocalEndPoint { get; private set; }
    }
}
