// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketHandler.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provides data for the <see cref="OnReceiveData" /> event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System;
    using System.Net;
   
    /// <summary>
    /// The class server socket handler is the visible part of sockets name space. It's closely link with 
    /// </summary>
    public class SocketHandler : IConnectionHandler
    {       
        /// <summary>
        /// The size of the queue of incoming connections for the listen socket.
        /// </summary>
        private const int Backlog = 100;

        /// <summary>
        /// Read, write (don't alloc buffer space for accepts)
        /// 1 for receive, 1 for send and one
        /// </summary>
        private const int OpsToPreAlloc = 2;

        /// <summary>
        /// Asyncroneous socket server that is the connection point for remote units.
        /// </summary>
        private AsyncSocketListener socketListner;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketHandler"/> class. 
        /// </summary>
        /// <param name="listenPortNumber">
        /// Port number of the server socket listner.
        /// </param>
        /// <param name="maxConnections">
        /// Maximum simultaneaous connections handle by the socket handler
        /// </param>
        /// <param name="receptionBufferSize">
        /// Maximum buffer size for data reception.
        /// </param>
        public SocketHandler(int listenPortNumber, int maxConnections = 3000, int receptionBufferSize = 2048)
        {
            this.TimeoutCheckingInterval = TimeSpan.FromSeconds(10);
            this.ConnectionTimeout = TimeSpan.FromSeconds(0);
            this.ListenPortNumber = listenPortNumber;
            this.MaxConnections = maxConnections;
            this.ReceptionBufferSize = receptionBufferSize;
            
            var localEndPopint = new IPEndPoint(IPAddress.Any, listenPortNumber);

            var socketSettings = new SocketListenerSettings(
                maxConnections, 
                Backlog,
                receptionBufferSize,
                OpsToPreAlloc,
                localEndPopint);

            this.socketListner = new AsyncSocketListener(this, socketSettings); 
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SocketHandler"/> class. 
        /// Sets the internal socket listen free.
        /// </summary>
        ~SocketHandler()
        {
            this.socketListner = null;           
        }

        /// <summary>
        /// Gets or sets the maximum socket connections
        /// </summary>
        public int MaxConnections { get; set; }

        /// <summary>
        /// Gets the maximum buffer size of data sent/receive
        /// </summary>
        public int ReceptionBufferSize { get; private set; }

        /// <summary>
        /// Gets or sets the port number used by the socket handler to listen client socket connection requests.
        /// </summary>
        public int ListenPortNumber { get; set; }

        /// <summary>
        /// Gets or sets the timeout in secondes before ending the connection if no data are received.
        /// </summary>
        public TimeSpan ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the interval time in seconds between two timeout checkings. 
        /// </summary>
        /// <value>By default, TimeoutCheckingInterval = 10 s.</value>
        public TimeSpan TimeoutCheckingInterval { get; set; }

        /// <summary>
        /// Gets the number of current connected client on the socket handler.
        /// </summary>
        public int ConnectedSocketCount
        {
            get { return this.GetConnectedCount(); }
        }

        /// <summary>
        /// Gets TotalBytesSent.
        /// </summary>
        public int TotalBytesSent
        {
            get { return this.GetTotalBytesSent(); }
        }

        /// <summary>
        /// Gets TotalByteRead.
        /// </summary>
        public int TotalByteRead
        {
            get { return this.GetTotalBytesRead(); }
        }

        /// <summary>
        /// Gets or sets the event occurs when socket handler receives data from client socket connection.
        /// </summary>
        public EventHandler<ReadDataArgs> OnReadData { get; set; }

        /// <summary>
        /// Gets or sets the event occurs when socket handler detects a client socket disconnection.
        /// </summary>
        public EventHandler<SocketDisconnectArgs> OnSocketDisconnect { get; set; }

        /// <summary>
        /// Gets or sets the event occurs when socket handler accepts a new client socket connection.
        /// </summary>
        public EventHandler<SocketConnectArgs> OnSocketConnect { get; set; }

        /// <summary>
        /// Gets or sets the event occurs just before the socket listener is closing a socket connection
        /// </summary>
        public EventHandler<AcceptSocketClosureArgs> OnAcceptSocketClosure { get; set; }       

        /// <summary>
        /// Initialize underlaying socket server listener and starts it to listen 
        /// incomming client socket connections.
        /// </summary>
        public void Start()
        {
            this.socketListner.ConnectionTimeout = (int)this.ConnectionTimeout.TotalSeconds * 1000; // because secondes to milliseconds
            this.socketListner.TimeoutCheckingInterval = (int)this.TimeoutCheckingInterval.TotalSeconds * 1000; // because secondes to milliseconds
            this.socketListner.Start();            
        }

        /// <summary>
        /// Stops the sending data queue thread
        /// </summary>
        public void Stop()
        {
            /*
             * if (m_SendingDataQueueThread != null) {
                // End the sending buffer thread 
                m_SendingDataQueueThreadEvent.Set();
                // Wait for thread ending :
                m_SendingDataQueueThread.Join();
                m_SendingDataQueueThread = null;
            } // if
             * */
        }

        /// <summary>
        /// Sends string to the connected client linked on the socket with the given connection identifier. 
        /// </summary>
        /// <param name="socketHandle">Unique connection identifier of the underlaying socket connection.</param>
        /// <param name="text">Text to send.</param>
        public void SendText(int socketHandle, string text)
        {
            if (socketHandle > 0) 
            {
                this.socketListner.SendText(socketHandle, text);
            } 
            else 
            {
                throw new ArgumentOutOfRangeException("SocketHandle", socketHandle, "The value must be > than 0.\r\nTo broadcast text, see BroadcastText method.");
            }
        }

        /// <summary>
        /// Send text to every client socket connections
        /// </summary>
        /// <param name="text">To to send.</param>
        public void BroadcastText(string text)
        {
            this.socketListner.SendText(AsyncSocketListener.ConnectionBroadcast, text);
        }

        /// <summary>
        /// Send data buffer over the underlying socket
        /// </summary>
        /// <param name="socketHandle">
        /// Handle of the underlying socket.
        /// </param>
        /// <param name="buffer">Buffer of data to send.</param>
        /// <returns>
        /// <b>true</b> if the send operation succeeds
        /// </returns>
        public bool SendData(int socketHandle, byte[] buffer)
        {
            if (socketHandle <= 0)
            {
                throw new ArgumentOutOfRangeException(
                "socketHandle",
                socketHandle,
                "The value must be greater than 0.\r\nTo broadcast data, see BroadcastBuffer and ForwardBuffer methods.");
            }

            return this.socketListner.SendBuffer(socketHandle, buffer);
        }

        /// <summary>
        /// Broadcast data to every client socket connections.
        /// </summary>
        /// <param name="buffer">Buffer of data to send.</param>
        public void BroadcastData(byte[] buffer)
        {
            this.socketListner.SendBuffer(0, buffer);
        }

        /// <summary>
        /// Process socket connection. It's called ad soon as the socket server accepts a client socket connection.
        /// </summary>
        /// <param name="socketHandle">Unique identifer of the socket connection.</param>
        internal void ProcessSocketConnect(int socketHandle)
        {
            var handler = this.OnSocketConnect;

            if (handler != null)
            {
                handler(this, new SocketConnectArgs(socketHandle));
            }
        }

        /// <summary>
        /// Process read data received on the socket server.
        /// </summary>
        /// <param name="socketHandle">Unique identifer of the socket connection.</param>
        /// <param name="buffer">Received binary data from socket client connection.</param>
        /// <remarks>For the first implementation, only the ECI stack is reeady, so, each received data are for the ECI stack.</remarks>
        internal void NotifyReadData(int socketHandle, byte[] buffer)
        {
            var handler = this.OnReadData;
            if (handler != null) 
            {
                handler(this, new ReadDataArgs(socketHandle, buffer));
            }
        }

        /// <summary>
        /// Notify socket disconnection. It's called ad soon as the socket server detects a client socket disconnection.
        /// </summary>
        /// <param name="socketHandle">Unique identifer of the socket connection.</param>
        /// <param name="receivedBytes">Total amount of received bytes during the connection.</param>
        /// <param name="sentBytes">Total amount of sent bytes during the connection.</param>
        internal void NotifySocketDisconnect(int socketHandle, int receivedBytes, int sentBytes)
        {
            var handler = this.OnSocketDisconnect;
            if (handler != null) 
            {
                handler(this, new SocketDisconnectArgs(socketHandle, receivedBytes, sentBytes));
            } 
        }

        /// <summary>
        /// If return true, the connection will be closed by the socket listener.
        /// </summary>
        /// <param name="socketHandle">Identifier of the connection that will be closed</param>
        /// <returns>True if the socket handler accept that the connection will be closed.</returns>
        internal bool AcceptConnectionClosure(int socketHandle)
        {
            var handler = this.OnAcceptSocketClosure;
            if (handler != null) 
            {
                var args = new AcceptSocketClosureArgs(socketHandle);
                handler(this, args);
                return args.Accept;
            } 

            return true;
        }

        /// <summary>
        /// Gets the number of clients currently connected on the socket handler
        /// </summary>
        /// <returns>Integer, number of connected clients.</returns>
        private int GetConnectedCount()
        {
            if (this.socketListner != null) 
            {
                return this.socketListner.ConnectedSocketCount;
            }

            return 0;
        }

        /// <summary>
        /// Gets the total bytes red on on the socket handler from start
        /// </summary>
        /// <returns>
        /// Total of read bytes.
        /// </returns>
        private int GetTotalBytesRead()
        {
            return this.socketListner.TotalBytesRead;
        }

        /// <summary>
        /// Gets the total bytes sent from the socket handler from start 
        /// </summary>
        /// <returns>
        /// Total of sent bytes.
        /// </returns>
        private int GetTotalBytesSent()
        {
            return this.socketListner.TotalBytesSent;
        }
    }
}
