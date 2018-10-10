// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralState.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    using NLog;

    public class AudioSwitchPeripheralState : PeripheralState<PeripheralAudioSwitchMessageType>
    {
        public AudioSwitchPeripheralState(PeripheralContext<PeripheralAudioSwitchMessageType> context)
            : base(context)
        {
        }
    }

    /// <summary>The Peripheral device state.</summary>
    public class PeripheralState<TMessageType> : IPeripheralState<TMessageType>
    {
        #region Constants

        /// <summary>The default buffer size.</summary>
        public const int DefaultBufferSize = 4096;

        #endregion

        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>The event sent when data from the Peripheral has been processed and received.</summary>
        public EventHandler<PeripheralDataReceivedEventArgs> OnPeripheralDataReceivedEvent;

        /// <summary>The buffer updated event.</summary>
        protected readonly ManualResetEvent BufferUpdatedEvent = new ManualResetEvent(false);

        /// <summary>The read write lock.</summary>
        // protected read-only ReaderWriterLock ReadWriteLock = new ReaderWriterLock();

        /// <summary>The stream lock.</summary>
        protected readonly object StreamLock = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralState"/> class.</summary>
        /// <param name="context">The context.</param>
        public PeripheralState(PeripheralContext<TMessageType> context)
        {
            this.SerialBuffer = new byte[DefaultBufferSize];
            this.Running = false;
            this.PeripheralContext = context;
            this.MemoryStream = new MemoryStream(DefaultBufferSize);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the buffer size.</summary>
        public int BufferSize
        {
            get
            {
                return this.SerialBuffer?.Length ?? 0;
            }
        }

        /// <summary>
        ///     Gets state of the memory stream true if it is empty
        /// </summary>
        public bool IsEmptyStream
        {
            get
            {
                return this.StreamLength == 0;
            }
        }

        /// <summary>Gets a value indicating whether is partial messages.</summary>
        public bool IsPartialMessagesInStream
        {
            get
            {
                lock (this.StreamLock)
                {
                    return this.MemoryStreamBytes.FindFramingPosition() >= 0;
                }
            }
        }

        /// <summary>Gets or sets the last message type.</summary>
        public TMessageType LastMessageType { get; set; }   

        /// <summary>Gets the memory stream as bytes. Stream position remains the same</summary>
        public byte[] MemoryStreamBytes
        {
            get
            {
                lock (this.StreamLock)
                {
                    return this.MemoryStream?.ToArray() ?? new byte[0];
                }
            }
        }

        /// <summary>Gets the memory stream position.</summary>
        public long MemoryStreamPosition
        {
            get
            {
                lock (this.StreamLock)
                {
                    return this.MemoryStream?.Position ?? 0;
                }
            }
        }

        /// <summary>Gets the audio switch context.</summary>
        public PeripheralContext<TMessageType> PeripheralContext { get; private set; }

        /// <summary>Gets or sets a value indicating the running state.</summary>
        public bool Running { get; set; }

        /// <summary>Gets the buffer.</summary>
        public byte[] SerialBuffer { get; private set; }

        /// <summary>Gets the stream length.</summary>
        public long StreamLength
        {
            get
            {
                lock (this.StreamLock)
                {
                    return this.MemoryStream?.Length ?? 0;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets the memory stream.</summary>
        protected MemoryStream MemoryStream { get; private set; }       

        #endregion

        #region Public Methods and Operators

        /// <summary>The append stream.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The <see cref="long"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="Invalid null buffer"/> is <see langword="null"/>.</exception>
        public long AppendStream(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("Invalid null buffer", nameof(buffer));
            }

            lock (this.StreamLock)
            {
                Debug.WriteLine("AppendStream Enter adding bytes to stream Length=" + buffer.Length);

                // buffer.DebugDump("Rx");
                if (buffer.Length + this.MemoryStream.Length < this.MemoryStream.Capacity)
                {
                    var position = (int)this.MemoryStream.Length;
                    var bytes = this.WriteStream(buffer, position);
                    this.BufferUpdatedEvent.Set();
                    return bytes;
                }

                Logger.Warn("Buffer Full, throwing away");
                return this.MemoryStream.Length;
            }
        }

        /// <summary>Clear the memory stream.</summary>
        public void EmptyStream()
        {
            lock (this.StreamLock)
            {
                Debug.WriteLine("EmptyStream");
                this.RewindStream();
                this.MemoryStream.SetLength(0);
            }
        }

        /// <summary>The lock stream.</summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool LockStream(int timeout = Timeout.Infinite)
        {
            try
            {
                return Monitor.TryEnter(this.StreamLock, timeout);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Get a BinaryReader on the stream.</summary>
        /// <returns>The <see cref="byte[]" />.</returns>
        [Obsolete]
        public BinaryReader ReadStream()
        {
            lock (this.StreamLock)
            {
                return new BinaryReader(new MemoryStream(this.MemoryStream.ToArray()), Encoding.UTF8);
            }
        }

        /// <summary>Remove bytes from beginning of the stream.</summary>
        /// <param name="bytesToRemove">The total bytes to remove from the stream and resize the stream length.</param>
        /// <exception cref="ArgumentException"></exception>
        public void RemoveBytesFromStream(int bytesToRemove)
        {
            lock (this.StreamLock)
            {
                if (this.MemoryStream.Length > 0 && bytesToRemove > 0)
                {
                    if (bytesToRemove <= this.MemoryStream.Length)
                    {
                        // remove from the start of the stream
                        var oldBuffer = this.MemoryStream.ToArray().Skip(bytesToRemove).ToArray();
                        var bytesRemaining = oldBuffer.Length;
                        this.MemoryStream = new MemoryStream(this.BufferSize);
                        if (bytesRemaining > 0)
                        {
                            this.MemoryStream.Write(oldBuffer, 0, oldBuffer.Length);
                        }

                        this.MemoryStream.Position = 0;
                    }
                    else
                    {
                        this.MemoryStream = new MemoryStream(this.BufferSize);
                    }
                }
            }
        }

        /// <summary>The rewind stream.</summary>
        public void RewindStream()
        {
            lock (this.StreamLock)
            {
                if (this.MemoryStream.CanSeek)
                {
                    this.MemoryStream.Seek(0, SeekOrigin.Begin);
                }
            }
        }

        /// <summary>The un lock stream.</summary>
        public void UnLockStream()
        {
            try
            {
                Monitor.Exit(this.StreamLock);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        /// <summary>Write to stream.</summary>
        /// <param name="buffer">The bytes to write to buffer.</param>
        /// <param name="position">The stream position.</param>
        /// <returns>The <see cref="long"/>.</returns>
        public long WriteStream(byte[] buffer, int position = 0)
        {
            if (buffer != null && buffer.Length > 0)
            {
                lock (this.StreamLock)
                {
                    var currentPosition = this.MemoryStream.Position;
                    if (position >= 0 && position < this.MemoryStream.Capacity)
                    {
                        this.MemoryStream.Seek(position, SeekOrigin.Begin);
                    }

                    this.MemoryStream.Write(buffer, 0, buffer.Length);
                    this.MemoryStream.Position = currentPosition;
                }
            }

            return this.MemoryStream.Length;
        }

        #endregion
    }
}