// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="SocketState.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.SystemManagement.Host.Path;

    using NLog;

    /// <summary>The socket state.</summary>
    public class SocketState : IDisposable
    {
        #region Constants

        /// <summary>The default buffer size.</summary>
        public const int DefaultBufferSize = 8192 * 2;

        #endregion

        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        private int bufferSize;
        private bool disposed;

        /// <summary>The string builder buffer for incoming encoded xml.</summary>
        private StringBuilder sb = new StringBuilder();

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SocketState" /> class.</summary>
        protected SocketState()
            : this(new Guid(), DefaultBufferSize)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SocketState"/> class.</summary>
        /// <param name="bufferSize">The read buffer size.</param>
        public SocketState(Guid guid, int bufferSize)
        {
            this.Key = guid.ToString();
            this.BufferSize = bufferSize;
            this.Buffer = new byte[this.BufferSize];
        }

        /// <summary>Initializes a new instance of the <see cref="SocketState"/> class.</summary>
        /// <param name="guid"></param>
        /// <param name="threadReader">The reader Thread.</param>
        /// <param name="socket">The socket.</param>
        /// <param name="bufferSize">The read buffer Size.</param>
        public SocketState(Guid guid, Thread threadReader, Socket socket, int bufferSize = DefaultBufferSize)
            : this(guid, bufferSize)
        {
            this.ThreadReader = threadReader;
            this.Socket = socket;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the socket buffer.</summary>
        public byte[] Buffer { get; set; }

        /// <summary>Gets or sets the buffer size.</summary>
        /// <exception cref="InvalidOperationException">Invalid buffer size, less than zero.</exception>
        public int BufferSize
        {
            get
            {
                return this.bufferSize;
            }

            set
            {
                if (value > 0)
                {
                    this.bufferSize = value;
                }
                else
                {
                    throw new InvalidOperationException("Invalid Buffer size");
                }
            }
        }

        /// <summary>Gets the bytes received event.</summary>
        public ManualResetEvent BytesReceivedEvent { get; } = new ManualResetEvent(false);

        public ManualResetEvent TerminateEvent { get; } = new ManualResetEvent(false);

        /// <summary>Gets a value indicating whether is connected.</summary>
        public bool IsConnected
        {
            get
            {
                return IsSocketConnected(this.Socket);
            }
        }

        public bool IsLoopbackEndpoint
        {
            get
            {
                return this.Socket != null && this.Socket.LocalEndPoint.ToString().StartsWith(IPAddress.Loopback.ToString());
            }
        }

        /// <summary>Gets or sets a value indicating whether the thread is running.</summary>
        public bool Running { get; set; }

        /// <summary>The signaled exited event used to signal exited.</summary>
        public ManualResetEvent SignaledExited { get; } = new ManualResetEvent(false);

        /// <summary>Gets or sets the socket.</summary>
        public Socket Socket { get; set; }

        /// <summary>Gets or sets the string buffer.</summary>
        public StringBuilder StringBuffer
        {
            get
            {
                return this.sb;
            }

            set
            {
                this.sb = value;
            }
        }

        /// <summary>Gets or sets the reader.</summary>
        public Thread ThreadReader { get; set; }

        public string Key { get; internal set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Test if the Socket is connected by polling in read mode.</summary>
        /// <param name="socket">The socket to check.</param>
        /// <returns>True if connected else false.</returns>
        public static bool IsSocketConnected(Socket socket)
        {
            if (socket == null)
            {
                return false;
            }

            var connected = true;
            try
            {
                // Poll returns true if connection is closed, reset terminated or pending w/o connection
                var part1 = socket.Poll(100, SelectMode.SelectRead);
                connected = (!part1 || socket.Available != 0);
                if (!connected)
                {
                    // Not an error when/if the client close the socket first.
                    Logger.Warn("IsSocketConnected() Socket endpoint {0} has closed! Thread:{1}. Server will disconnect socket to free.",
                        socket.LocalEndPoint, Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK 
                if (e.NativeErrorCode.Equals(10035))
                {
                    return true;
                }
            }
            catch (ObjectDisposedException)
            {
                Logger.Warn("ObjectDisposedException on IsSocketConnected() Socket disposed and is Not Connected");
                return false;
            }

            return connected;
        }

        /// <summary>The clear string builder buffer.</summary>
        public void ClearBuffer(bool ignoreWarning = false)
        {
            lock (this.sb)
            {
                Debug.WriteLine("! Clearing StringBuffer");
                if (this.sb.Length > 0)
                {
                    if (!ignoreWarning)
                    {
                        Logger.Warn("Throwing away string[{0}] from the Socket String buffer", this.sb.ToString());
                    }
                }
                this.sb.Length = 0;
            }
        }

        #endregion

        // Note Creating the NLog Logger using this approach fails with NLog version 2.0
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
            }
        }
    }
}