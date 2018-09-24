// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketConnection.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Timers;

    using NLog;

    /// <summary>
    /// Holds the information linked to the connection.
    /// </summary>
    internal class SocketConnection : IDisposable
    {
        /// <summary>
        /// Nlogger used for logging global information to debug and trace
        /// </summary>
        protected readonly Logger GlobalLogger;

        private readonly Timer communicationWatchDogTimer;

        /// <summary>
        /// Queue that contains all send messages as array of bytes
        /// </summary>
        private readonly Queue<byte[]> sendQueue;

        /// <summary>
        /// Locker used to send data 
        /// </summary>
        private readonly object sendLock = new object();

        private readonly int maxBufferSize;

        /// <summary>
        /// Time in milliseconds after which the socket connection is automatically closed. If the value = 0, then it will be never disconnected automatically.
        /// </summary>
        private readonly int timeout;

        /// <summary>
        /// Windows socket corresponding to the connection.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Value indicating whether the conection is sending data or not
        /// </summary>
        private bool writing;

        private bool closing;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketConnection"/> class.
        /// </summary>
        public SocketConnection()
        {
            this.ReceivedBytes = 0;
            this.ConnectionId = 0;
            this.SentBytes = 0;
            this.timeout = 0;
            this.GlobalLogger = LogManager.GetLogger("GlobalLog");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketConnection"/> class.
        /// </summary>
        /// <param name="socket">
        /// Socket to store. ReceivedByte and SentBytes properties are reset !
        /// </param>
        /// <param name="readEventArgs">
        /// The read Event Args used during all the timelife of the connection to read data on the underlying connection.
        /// </param>
        /// <param name="writeEventArgs">
        /// The write Event Args used during all the timelife of the connection to write data on the underlying connection.
        /// </param>
        /// <param name="sendBufferMaxSize">
        /// The max size of the buffer in bytes to send data on socket per sending opertation.
        /// </param>        /// <param name="timeout">
        /// Time in milliseconds before raise timeout connection event. By default, there is no timeout
        /// </param>
        /// <param name="checkTimeoutInterval">
        /// Time in milliseconds between two timeout checkings.
        /// </param>
        public SocketConnection(
            Socket socket,
            SocketAsyncEventArgs readEventArgs,
            SocketAsyncEventArgs writeEventArgs,
            int sendBufferMaxSize, 
            int timeout, 
            int checkTimeoutInterval)
            : this()
        {
            if (readEventArgs == null)
            {
                throw new ArgumentNullException("readEventArgs");
            }

            if (writeEventArgs == null)
            {
                throw new ArgumentNullException("writeEventArgs");
            }

            this.communicationWatchDogTimer = new Timer(checkTimeoutInterval) { AutoReset = true };
            this.communicationWatchDogTimer.Elapsed += this.CommunicationWatchDog;
            this.timeout = timeout;
            this.SetSocket(socket);
            this.WriteEventArgs = writeEventArgs;
            this.ReadEventArgs = readEventArgs;
            this.LastReception = DateTime.Now;
            this.sendQueue = new Queue<byte[]>();
            this.maxBufferSize = sendBufferMaxSize;
        }

        /// <summary>
        /// Event notified when the connection doesn't receive data for the specified time out.
        /// </summary>
        public event EventHandler<EventArgs> OnConnectionTimeout;

        /// <summary>
        /// Gets ReadEventArgs linked to the connection during its entire timelife 
        /// </summary>
        public SocketAsyncEventArgs ReadEventArgs { get; private set; }

        /// <summary>
        /// Gets WriteEventArgs linked to the connection during its entire timelife 
        /// </summary>
        public SocketAsyncEventArgs WriteEventArgs { get; private set; }

        /// <summary>
        /// Gets or sets the number of received bytes on the socket.
        /// </summary>
        public int ReceivedBytes { get; set; }

        /// <summary>
        /// Gets or sets the number of sent bytes on the socket.
        /// </summary>
        public int SentBytes { get; set; }

        /// <summary>
        /// Gets the identifier of the connection. Corresponds to the underlaying socket handle converted into int32
        /// </summary>
        public int ConnectionId { get; private set; }

        /// <summary>
        /// Gets or sets the timestamp for the last data rececption. This fied is update each time the socket read data from the connection
        /// </summary>
        public DateTime LastReception { get; set; }

        /// <summary>
        /// Add data (array of bytes) to be sent over the socket.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        public void EnqueueBuffer(byte[] buffer)
        {
            int queueSize;
            lock (this.sendLock)
            {
                this.sendQueue.Enqueue(buffer);
                queueSize = this.sendQueue.Count;
            }

            this.GlobalLogger.Trace("Enqueued {0} bytes at {1} to be written to connection id {2}", buffer.Length, queueSize, this.ConnectionId);
        }

        /// <summary>
        /// Start asynchroneaous call on the underlying socket.
        /// </summary>
        /// <param name="readAsyncEventArgs">
        /// The read async event args.
        /// </param>
        /// <returns>
        /// <b>true</b> is the opertaion is really asynchroneous, otherwize <b>false</b>.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown in case of socket is null.
        /// </exception>
        public bool StartReceive(SocketAsyncEventArgs readAsyncEventArgs)
        {
            this.CheckSocket("StartReceive");

            return this.socket.ReceiveAsync(readAsyncEventArgs);
        }

        /// <summary>
        /// Try to send data if it's not sending data 
        /// </summary>
        /// <param name="force">
        /// Value indicating if we have to force writing.
        /// </param>
        /// <param name="willRaiseEvent">
        /// Value indicating if the asynchroneaous operation will send an event when will be completed.
        /// </param>
        /// <returns>
        /// <b>true</b> if the sending operation is executed, otherwize <b>false</b>.
        /// </returns>
        public bool StartWriting(bool force, out bool willRaiseEvent)
        {
            byte[] buffer;
            Socket sock;
            willRaiseEvent = false;
            lock (this.sendLock)
            {
                if (this.closing)
                {
                    return false;
                }

                if (this.writing && !force)
                {
                    return false;
                }

                if (this.sendQueue.Count == 0 || this.socket == null)
                {
                    this.writing = false;
                    return false;
                }

                buffer = this.sendQueue.Dequeue();
                this.writing = true;
                sock = this.socket;
            }

            if (sock.Connected)
            {
                try
                {
                    var writeEventArgs = this.WriteEventArgs;
                    if (buffer.Length <= this.maxBufferSize)
                    {
                        writeEventArgs.SetBuffer(buffer, 0, buffer.Length);
                        this.GlobalLogger.Trace("Writing {0} bytes to connection id {1}", buffer.Length, this.ConnectionId);
                        willRaiseEvent = sock.SendAsync(writeEventArgs);
                    }
                    else
                    {
                        this.GlobalLogger.Warn("The buffer length is greater than the maximum size of the receive/send buffer.");
                    }
                }
                catch (Exception ex)
                {
                    this.GlobalLogger.WarnException("SendAsync", ex);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Start waitng to receive data on the socket asynchroneously.
        /// </summary>
        /// <param name="willRaiseEvent">
        /// Value indicating if the asynchroneaous operation will send an event when will be completed.
        /// </param>
        /// <returns>
        /// <b>true</b> if the async reading operation is executed, otherwize <b>false</b>.
        /// </returns>
        public bool StartReceiving(out bool willRaiseEvent)
        {
            willRaiseEvent = false;

            try
            {
                willRaiseEvent = this.socket.ReceiveAsync(this.ReadEventArgs);
            }
            catch (Exception ex)
            {
                this.GlobalLogger.WarnException("ReceiveAsync", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes the Socket connection and clear the sendQueue. 
        /// </summary>
        public void Close()
        {
            if (this.closing)
            {
                return;
            }

            this.CheckSocket("Close");

            this.closing = true;
            try
            {
                this.socket.Shutdown(SocketShutdown.Send);
            }            
            catch (Exception ex)
            {
                // throws if client process has already closed
                this.GlobalLogger.WarnException("Close", ex);
            }

            // Closes the Socket connection and releases all associated resources. 
            this.socket.Close();

            this.SetSocket(null);

            lock (this.sendLock)
            {
                this.sendQueue.Clear();
                this.writing = false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// The check the underlying socket.
        /// </summary>
        /// <param name="methodName">
        /// The name of the caller method.
        /// </param>
        /// <exception cref="Exception">
        /// Thrown in case of socket is null.
        /// </exception>
        private void CheckSocket(string methodName)
        {
            if (this.socket == null)
            {
                throw new Exception(methodName + ": The underlying socket should be not null.");
            }
        }

        private void NotifyConnectionTimeout()
        {
            var handler = this.OnConnectionTimeout;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Delegate call by the timer each second
        /// </summary>
        /// <param name="sender">
        /// The current connection info.
        /// </param>
        /// <param name="e">Arguments handled by ElapseEventHandler.</param>
        private void CommunicationWatchDog(object sender, ElapsedEventArgs e)
        {
            if (this.closing)
            {
                return;
            }

            if (this.timeout > 0)
            {
                this.communicationWatchDogTimer.Stop();
                try
                {
                    if (this.LastReception.AddMilliseconds(this.timeout).CompareTo(DateTime.Now) < 0)
                    {
                        this.NotifyConnectionTimeout();
                    }                    
                }
                finally
                {
                    this.communicationWatchDogTimer.Start();
                }
            }
        }

        /// <summary>
        /// Assignes the underlaying socket to the connection info. The internal information like received and sent bytes are reset after this operation. 
        /// </summary>
        /// <param name="value">Underlaying socket. It could be null.</param>
        private void SetSocket(Socket value)
        {
            this.socket = value;
            if (this.socket != null)
            {
                this.ConnectionId = this.socket.Handle.ToInt32();
                this.communicationWatchDogTimer.Enabled = true;
            }
            else
            {
                this.ConnectionId = 0;
                this.communicationWatchDogTimer.Enabled = false;
            }

            // Reset previous connection information
            this.ReceivedBytes = 0;
            this.SentBytes = 0;
            this.LastReception = DateTime.MinValue;
        }
    }
}
