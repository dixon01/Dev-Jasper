// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncSocketListener.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements the connection logic for the socket server.
//   After accepting a connection, all data read from the client
//   is sent back to the client. The read and echo back to the client pattern
//   is continued until the client disconnects.
//   Based on http://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using NLog;

    /// <summary>
    /// Implements the connection logic for the socket server.  
    /// After accepting a connection, all data read from the client 
    /// is sent back to the client. The read and echo back to the client pattern 
    /// is continued until the client disconnects.
    /// Based on http://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx
    /// </summary>
    /// <remarks>
    /// It is a technical class used intenally. It's a compagnion component of SocketHandler. You don't need to use it directly.
    /// </remarks>
    public class AsyncSocketListener
    {
        #region Constants and Fields

        /// <summary>
        /// The connectio n_ any.
        /// </summary>
        public const int ConnectionAny = -1;

        /// <summary>
        /// The connectio broadcast.
        /// </summary>
        public const int ConnectionBroadcast = 0;

        /// <summary>
        /// Nlogger used for logging global information to debug and trace
        /// </summary>
        protected readonly Logger GlobalLogger;

        /// <summary>
        /// List of ConnectionInfo (value) per ConnectionId (=key)
        /// </summary>
        private readonly SortedList<int, SocketConnection> connectedSocketList;

        /// <summary>
        /// Locker to handle concurrent access to the internal connected socket list
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The m_ owner.
        /// </summary>
        private readonly SocketHandler socketHandlerOwner;

        /// <summary>
        /// Handles the maximum connections to the socket handler authorized simultaneaously 
        /// </summary>
        private readonly Semaphore maxNumberAcceptedClients;

        private readonly SocketListenerSettings settings;

        /// <summary>
        /// The total number of clients connected to the server 
        /// </summary>
        private int connectedSocketCount;

        /// <summary>
        /// To keep a record of maximum number of simultaneous connections that occur while the server is running. 
        /// This can be limited by operating system and hardware. It will not be higher than the value that you set
        /// for maxNumberAcceptedClients.
        /// </summary>
        private int maxSimultaneousClientsThatWereConnected;

        /// <summary>
        /// The connection timeout.
        /// </summary>
        private int connectionTimeout;

        /// <summary>
        /// The socket used to listen for incoming connection requests
        /// </summary>
        private Socket listenSocket;

        /// <summary>
        /// The m_ timeout checking interval.
        /// </summary>
        private int timeoutCheckingInterval = 1000;

        /// <summary>
        /// Counter of the total # bytes received by the server
        /// </summary>
        private int totalBytesRead;

        /// <summary>
        /// Counter of the total # bytes sent by the server.
        /// </summary>
        private int totalBytesSent;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSocketListener"/> class. 
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method 
        /// </summary>
        /// <param name="socketHandler">
        /// The owner.
        /// </param>
        /// <param name="socketSettings">
        /// Contains all information needed for socket connections 
        /// </param>
        internal AsyncSocketListener(SocketHandler socketHandler, SocketListenerSettings socketSettings)
        {
            if (socketHandler == null)
            {
                throw new ArgumentNullException("socketHandler", "The socket handler owner must be not null");
            }

            this.GlobalLogger = LogManager.GetLogger("GlobalLog");

            this.socketHandlerOwner = socketHandler;
            this.settings = socketSettings;

            this.totalBytesRead = 0;
            this.totalBytesSent = 0;
            this.connectedSocketCount = 0;

            this.maxNumberAcceptedClients = new Semaphore(socketSettings.MaxConnections, socketSettings.MaxConnections);
            this.connectedSocketList = new SortedList<int, SocketConnection>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AsyncSocketListener"/> class. 
        /// Destructor to set the memory free.
        /// </summary>
        ~AsyncSocketListener()
        {
            this.maxNumberAcceptedClients.Dispose();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the connection timeout in milliseconds. If equals 0, then the system doesn't check is there is a timeout.
        /// A timeout event is raised if no data are received since ConnectionTimeout milliseconds (if ConnectionTimeout > 0!).
        /// </summary>
        /// <value>By default, ConnectionTimeout = 0 ms</value>
        public int ConnectionTimeout
        {
            get
            {
                return this.connectionTimeout;
            }

            set
            {
                this.connectionTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the interval time in milliseconds between two timeout checkings. 
        /// </summary>
        /// <value>By default, TimeoutCheckingInterval = 1000 ms.</value>
        public int TimeoutCheckingInterval
        {
            get
            {
                return this.timeoutCheckingInterval;
            }

            set
            {
                this.timeoutCheckingInterval = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of current connected clients on the socket handler
        /// </summary>
        internal int ConnectedSocketCount
        {
            get
            {
                return this.connectedSocketCount;
            }
        }

        /// <summary>
        /// Gets the total bytes read by the socket handler from start.
        /// </summary>
        internal int TotalBytesRead
        {
            get
            {
                return this.totalBytesRead;
            }
        }

        /// <summary>
        /// Gets the total bytes sent by the socket handler from start.
        /// </summary>
        internal int TotalBytesSent
        {
            get
            {
                return this.totalBytesSent;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send binary data to connected client(s)
        /// </summary>
        /// <param name="connectionId">
        /// Identifier of the connection (socket handle).
        /// <value>
        /// if eaqual to 0, then operates a broadcast else sends data to the connection id if exists.
        /// </value>
        /// </param>
        /// <param name="buffer">
        /// Data to send
        /// </param>
        /// <returns>
        /// <b>true</b> if it's a broadcast (the connectionId = 0) or if the connection id is found in the socket pool 
        /// and no exception has been thrown, otherwize <b>false</b>
        /// </returns>
        internal bool SendBuffer(int connectionId, byte[] buffer)
        {
            lock (this.locker)
            {
                SocketConnection sockConn;
                if (connectionId == ConnectionBroadcast)
                {
                    // Broadcast
                    var success = true;
                    foreach (var kvp in this.connectedSocketList)
                    {
                        try
                        {
                            sockConn = kvp.Value;
                            this.SendBuffer(sockConn, buffer);
                            success = false;
                        }
                        catch (Exception ex)
                        {
                            this.GlobalLogger.WarnException("SendBuffer broadcast", ex);
                            return false;
                        }
                    }

                    return success;
                }

                // unicast                
                if (this.connectedSocketList.TryGetValue(connectionId, out sockConn))
                {
                    try
                    {
                        this.SendBuffer(sockConn, buffer);
                    }
                    catch (Exception ex)
                    {
                        this.GlobalLogger.WarnException("SendBuffer unicast", ex);
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Sends text to connected client(s).
        /// </summary>
        /// <param name="connectionId">
        /// Identifier of the connection. If connectionId == 0, then broadcast text for every connected clients except to srcConnectionId if &gt; 0.
        /// </param>
        /// <param name="text">
        /// Text to send.
        /// </param>
        /// <returns>
        /// <b>true</b> if it's a broadcast (the connectionId = 0) or if the connection id is found in the socket pool 
        /// and no exception has been thrown, otherwize <b>false</b>
        /// </returns>
        internal bool SendText(int connectionId, string text)
        {
            return this.SendBuffer(connectionId, Encoding.ASCII.GetBytes(text)); // , srcConnectionId);
        }

        /// <summary>
        /// Starts the server for listening incoming connection requests.    
        /// </summary>
        internal void Start()
        {
            // create the socket which listens for incoming connections
            this.listenSocket = new Socket(
                this.settings.LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (this.settings.LocalEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // Set dual-mode (IPv4 & IPv6) for the socket listener.
                // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,
                // based on http://blogs.msdn.com/wndp/archive/2006/10/24/creating-ip-agnostic-applications-part-2-dual-mode-sockets.aspx
                this.listenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                this.listenSocket.Bind(
                    new IPEndPoint(IPAddress.IPv6Any, this.settings.LocalEndPoint.Port));
            }
            else
            {
                // Associate the socket with the local endpoint.
                this.listenSocket.Bind(this.settings.LocalEndPoint);
            }

            // start the server with a listen backlog of 100 connections
            this.listenSocket.Listen(this.settings.Backlog);

            // post accepts on the listening socket
            this.StartAccept(null);
        }

        /// <summary>
        /// Begins an operation to accept a connection request from the client 
        /// </summary>
        /// <param name="acceptEventArg">
        /// The context object to use when issuing 
        /// the accept operation on the server's listening socket
        /// </param>
        internal void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += this.AcceptEventArgCompleted;
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArg.AcceptSocket = null;
            }

            this.maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = this.listenSocket.AcceptAsync(acceptEventArg);

            // Please notes that false mean not asynchroneous but in immediatly
            if (!willRaiseEvent)
            {
                this.ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// This method is the callback method associated with Socket.AcceptAsync 
        /// operations and is invoked when an accept operation is complete
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AcceptEventArgCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        /// <summary>
        /// The close client socket.
        /// </summary>
        /// <param name="e">
        /// The connection id.
        /// </param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            var token = e.UserToken as AsyncUserToken;
            
            if (token == null)
            {
                this.GlobalLogger.Error("CloseClientSocket: the AsyncUserToken should be not null");
                return;
            }

            try
            {
                int connectionId = token.ConnectionId;
                SocketConnection connInfo;
                lock (this.locker)
                {                    
                    if (this.connectedSocketList.TryGetValue(connectionId, out connInfo))
                    {                                               
                        this.connectedSocketList.Remove(connectionId);
                    }
                }

                if (connInfo == null)
                {
                    this.GlobalLogger.Error(
                        "The connection id {} is nout found into the connected socket list.", connectionId);
                    return;
                }

                // stores the stats
                int recBytes = connInfo.ReceivedBytes;
                int sentBytes = connInfo.SentBytes;

                // Decrement the counter keeping track of the total number of clients connected to the server
                Interlocked.Decrement(ref this.connectedSocketCount);

                // Fires notification before disposing the connInfo, otherwize the unit is not found when finalize the connection                
                this.socketHandlerOwner.NotifySocketDisconnect(connectionId, recBytes, sentBytes);

                try
                {
                    // close the socket associated with the client
                    connInfo.Dispose();
                }
                catch (Exception ex)
                {
                    this.GlobalLogger.WarnException("Close connection", ex);
                }

                this.GlobalLogger.Debug("Release token id {0} on connection {1}", ((AsyncUserToken)connInfo.WriteEventArgs.UserToken).TokenId, connectionId);
                this.GlobalLogger.Debug("Release token id {0}  on connection {1}", ((AsyncUserToken)connInfo.ReadEventArgs.UserToken).TokenId, connectionId);

                connInfo.ReadEventArgs.Dispose();
                connInfo.WriteEventArgs.Dispose();
            }
            catch (Exception ex)
            {
                this.GlobalLogger.WarnException("ProcessCloseClientSocket", ex);
            }

            this.maxNumberAcceptedClients.Release();
        }

        /// <summary>
        /// Called if no data have been received since a defined time.
        /// </summary>
        /// <param name="sender">
        /// Arg that contains ConnectionInfo data.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/>. 
        /// </param>
        private void ConnectionTimeoutHandler(object sender, EventArgs e)
        {
            var connInfo = sender as SocketConnection;
            if (connInfo == null)
            {
                throw new Exception("The sender should be a ConnectionInfo");
            }

            if (this.socketHandlerOwner.AcceptConnectionClosure(connInfo.ConnectionId))
            {
                connInfo.Close();
            }
        }

        /// <summary>
        /// This method is called whenever a receive or send operation is completed on a socket 
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="ArgumentException">
        /// thrown if the last operation is not Receive or Send opertation
        /// </exception>
        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        /// <summary>
        /// The process accept.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError != SocketError.Success)
                {
                    this.GlobalLogger.Error("ProcessAccept: connection refused, socket error : {0}", e.SocketError);
                    this.HandleBadAccept(e);

                    // Listen for another connection
                    this.StartAccept(null);
                    return;
                }

                Interlocked.Increment(ref this.connectedSocketCount);

                if (this.connectedSocketCount > this.maxSimultaneousClientsThatWereConnected)
                {
                    Interlocked.Increment(ref this.maxSimultaneousClientsThatWereConnected);
                }

                int connectionId = e.AcceptSocket.Handle.ToInt32();

                // Get the socket for the accepted client connection and put it into the 
                // ReadEventArg object user token
                // SocketAsyncEventArgs readEventArgs = this.poolOfReadWriteSaea.Pop();
                var readEventArgs = new SocketAsyncEventArgs();
                var buffer = new byte[this.settings.ReceiveBufferSize];
                readEventArgs.SetBuffer(buffer, 0, this.settings.ReceiveBufferSize);
                readEventArgs.UserToken = new AsyncUserToken(Guid.NewGuid()) { ConnectionId = connectionId };
                readEventArgs.Completed += this.IOCompleted;

                // for writing operation the size is defined each time we need to send data.
                var writeEventArgs = new SocketAsyncEventArgs
                    { UserToken = new AsyncUserToken(Guid.NewGuid()) { ConnectionId = connectionId } };
                writeEventArgs.Completed += this.IOCompleted;

                // Add an entry into Connected Socket list:
                var connInfo = new SocketConnection(e.AcceptSocket, readEventArgs, writeEventArgs, this.settings.ReceiveBufferSize, this.connectionTimeout, this.timeoutCheckingInterval);

                this.GlobalLogger.Trace(
                    "Client connection accepted on port {0}, connection id {1}. There are {2} clients connected to the server",
                    this.settings.LocalEndPoint.Port,
                    connInfo.ConnectionId,
                    this.connectedSocketCount);

                connInfo.OnConnectionTimeout += this.ConnectionTimeoutHandler;
                lock (this.locker)
                {
                    // If the key doesn't exist, then this adds one otherwize that overwrites the existing one
                    this.connectedSocketList[connInfo.ConnectionId] = connInfo;
                }

                // Fires notification
                this.socketHandlerOwner.ProcessSocketConnect(connInfo.ConnectionId);

                // As soon as the client is connected, post a receive to the connection
                if (!connInfo.StartReceive(readEventArgs))
                {
                        this.ProcessReceive(readEventArgs);
                }
            }
            catch (Exception ex)
            {
                this.GlobalLogger.WarnException("ProcessAccept", ex);
            }

            // Accept the next connection request
            this.StartAccept(e);
        }

        /// <summary>
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// </summary>
        /// <param name="e">
        /// A <see cref="SocketAsyncEventArgs"/> that contains the event data.
        /// </param>
        /// <remarks>
        /// This method increases the total amount of received bytes and updates the connection info for stats.
        /// </remarks>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // check if the remote host closed the connection
                var token = (AsyncUserToken)e.UserToken;
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    SocketConnection connInfo;
                    lock (this.locker)
                    {
                        if (this.connectedSocketList.TryGetValue(token.ConnectionId, out connInfo))
                        {
                            connInfo.ReceivedBytes += e.BytesTransferred;
                            connInfo.LastReception = DateTime.Now;
                        }
                        else
                        {
                            this.GlobalLogger.Trace(
                                "ProcessReceive: Connection id {0} is not found", token.ConnectionId);
                        }
                    }

                    // Increments the count of the total bytes receive by the server
                    Interlocked.Add(ref this.totalBytesRead, e.BytesTransferred);

                    // Fire event
                    var localBuffer = new byte[e.BytesTransferred];
                    Array.Copy(e.Buffer, e.Offset, localBuffer, 0, e.BytesTransferred);
                    this.socketHandlerOwner.NotifyReadData(token.ConnectionId, localBuffer);

                    bool willRaiseEvent;

                    if (connInfo.StartReceiving(out willRaiseEvent))
                    {
                        if (!willRaiseEvent)
                        {
                            this.ProcessReceive(e);
                        }
                    }
                }
                else
                {
                    this.CloseClientSocket(e);
                }
            }
            catch (Exception ex)
            {
                this.GlobalLogger.WarnException("ProcessReceive", ex);
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional
        /// data sent from the client
        /// </summary>
        /// This method increases the total amount of sent bytes and updates the connection info for stats
        /// <param name="e">
        /// A <see cref="SocketAsyncEventArgs"/> that contains the event data.
        /// </param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            try
            {
                var token = (AsyncUserToken)e.UserToken;

                if (e.SocketError == SocketError.Success)
                {
                    SocketConnection sockConn;

                    lock (this.locker)
                    {
                        if (this.connectedSocketList.TryGetValue(token.ConnectionId, out sockConn))
                        {
                            sockConn.SentBytes += e.BytesTransferred;
                        }
                        else
                        {
                            this.GlobalLogger.Warn("No ConnectionInfo was found into list for connection id: {0}", token.ConnectionId);
                        }
                    }

                    // Increments the count of the total bytes sent by the server
                    Interlocked.Add(ref this.totalBytesSent, e.BytesTransferred);

                    if (sockConn != null)
                    {
                        bool willRaiseEvent;
                        if (sockConn.StartWriting(true, out willRaiseEvent))
                        {
                            if (!willRaiseEvent)
                            {
                                this.ProcessSend(e);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.GlobalLogger.WarnException("ProcessSend", ex);
            }
        }

        private void HandleBadAccept(SocketAsyncEventArgs acceptEventArgs)
        {                           
            try
            {
                // This method closes the socket and releases all resources, both
                // managed and unmanaged. It internally calls Dispose.        
                acceptEventArgs.AcceptSocket.Close();
            }
            catch (Exception ex)
            {
                this.GlobalLogger.WarnException("HandleBadAccept::Error closing socket", ex);
            }            

            // Release semaphore to avoid reaching the max allowed
            this.maxNumberAcceptedClients.Release();
        }

        private void SendBuffer(SocketConnection connInfo, byte[] buffer)
        {
            bool willRaiseEvent;
            
            connInfo.EnqueueBuffer(buffer);

            if (connInfo.StartWriting(false, out willRaiseEvent))
            {
                if (!willRaiseEvent)
                {
                    this.ProcessSend(connInfo.WriteEventArgs);
                }
            }
        }
        #endregion
    }
}