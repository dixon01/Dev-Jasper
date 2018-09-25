// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralSerialClient.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Models;

    using NLog;

    /// <summary>The peripheral serial client.</summary>
    public abstract class PeripheralSerialClient : IPeripheralSerialClient
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>The ACK received event.</summary>
        protected ManualResetEvent AckReceivedEvent = new ManualResetEvent(false);

        /// <summary>The data received event.</summary>
        protected AutoResetEvent DataReceivedEvent = new AutoResetEvent(false);

        private object ReadLock = new object();

        /// <summary>The serial reader task.</summary>
        protected Task SerialReaderTask;

        /// <summary>The terminate event.</summary>
        protected ManualResetEvent TerminateEvent = new ManualResetEvent(false);

        /// <summary>The disposed.</summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralSerialClient" /> class.</summary>
        protected PeripheralSerialClient()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralSerialClient"/> class.</summary>
        /// <param name="serialPortSettings">The serial port settings.</param>
        /// <exception cref="IOException">The specified port could not be found or opened.</exception>
        protected PeripheralSerialClient(ISerialPortSettings serialPortSettings)
            : this()
        {
            try
            {
                this.Open(serialPortSettings);
            }
            catch (IOException ioException)
            {
                Logger.Error("Failed to Open peripheral dimmer {0}", ioException.Message);
                throw;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralSerialClient"/> class.</summary>
        /// <param name="serialPortSettingsFileName">The serial port settings file name.</param>
        /// <exception cref="IOException">The specified port could not be found or opened.</exception>
        protected PeripheralSerialClient(string serialPortSettingsFileName)
            : this()
        {
            var serialPortSettings = SerialPortSettings.Read(serialPortSettingsFileName);
            this.Open(serialPortSettings);
        }

        #endregion

        #region Public Events

        /// <summary>The peripheral data ready received event.</summary>
        public event EventHandler<PeripheralDataReadyEventArg> PeripheralDataReady;

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether the serial port is open.</summary>
        public bool IsOpen
        {
            get
            {
                return this.SerialPort != null && this.SerialPort.IsOpen;
            }
        }

        /// <summary>Gets a value indicating whether reader task running.</summary>
        public bool ReaderTaskRunning { get; private set; }

        /// <summary>Gets or sets the serial port.</summary>
        public SerialPort SerialPort { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The close.</summary>
        public virtual void Close()
        {
            Logger.Info("Close Enter");
            Debug.WriteLine("PeripheralSerialClient.Close Enter");

            this.TerminateEvent.Set();
            Thread.Sleep(10);
            if (this.SerialPort != null && this.SerialPort.IsOpen)
            {
                Logger.Info("Closing Serial Port {0}", this.SerialPort.PortName);
                this.SerialPort.DataReceived -= this.SerialPortOnDataReceived;
                this.SerialPort.ErrorReceived -= this.SerialPortOnErrorReceived;
                this.SerialPort.Close();
            }

            Debug.WriteLine("PeripheralSerialClient.Close Exit");
        }

        /// <summary>The dispose.</summary>
        public virtual void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.Close();
            }
        }

        /// <summary>Open the Serial Port.</summary>
        /// <param name="serialPortSettings">The serial port settings.</param>
        /// <returns>The <see cref="Stream"/>.</returns>
        /// <exception cref="IOException">The specified port could not be found or opened.</exception>
        public virtual Stream Open(ISerialPortSettings serialPortSettings)
        {
            Logger.Info("Opening Serial Port {0} for Dimmer Peripheral", serialPortSettings);
            Debug.WriteLine("Open Serial device " + serialPortSettings.ToString());
            this.SerialPort = new SerialPort(
                serialPortSettings.ComPort, 
                serialPortSettings.BaudRate, 
                serialPortSettings.Parity, 
                serialPortSettings.DataBits, 
                serialPortSettings.StopBits) {
                                                Handshake = Handshake.None, ReceivedBytesThreshold = DimmerConstants.RecieveBytesThreshold 
                                             };

            if (serialPortSettings.BufferSize > 0)
            {
                this.SerialPort.ReadBufferSize = serialPortSettings.BufferSize;
                this.SerialPort.WriteBufferSize = serialPortSettings.BufferSize;
            }

            this.SerialPort.DataReceived += this.SerialPortOnDataReceived;
            this.SerialPort.ErrorReceived += this.SerialPortOnErrorReceived;
            this.SerialPort.Open();
            Stream stream = null;
            if (this.IsOpen)
            {
                if (serialPortSettings.EnableBackgroundReader)
                {
                    Logger.Info("Starting Dimmer background reader task on {0}", this.SerialPort.PortName);
                    this.SerialReaderTask = new Task(this.DataReaderAction);
                    this.SerialReaderTask.Start();
                }

                stream = this.SerialPort.BaseStream;
            }
            else
            {
                Logger.Error("Failed to Open Serial Port {0}", serialPortSettings.ComPort);
            }

            return stream;
        }

        /// <summary>Read series of bytes to Peripheral message</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="networkByteOrderToHost">The network Byte Order To Host.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="object"/>.</returns>
        /// <exception cref="MissingMethodException">Default Constructor missing.</exception>
        public T Read<T>(byte[] bytes, bool networkByteOrderToHost = true) where T : class, IPeripheralBaseMessage
        {
            Logger.Info("Reading for type {0}, bytes length={1}, networkByteOrderToHost={2}", typeof(T), bytes.Length, networkByteOrderToHost);
            bytes = bytes.RemoveFramingBytes();
            T model = bytes.FromBytes<T>();
            if (networkByteOrderToHost && model != null)
            {
                model.Header.NetworkByteOrderToHost();
            }

            return model;
        }

        /// <summary>The read.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="networkByteOrderToHost">The network byte order to host.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public abstract IPeripheralBaseMessage Read(byte[] bytes, bool networkByteOrderToHost = true);

        /// <summary>The read.</summary>
        /// <param name="msTimeout">The ms timeout.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        /// <exception cref="IOException">The port is in an invalid state.  - or - An attempt to set the state of the underlying
        ///     port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort"/> object were
        ///     invalid.</exception>
        public byte[] Read(int msTimeout = 1000)
        {
            var oldTimeout = this.SerialPort.ReadTimeout;
            var memoryStream = new MemoryStream();

            try
            {
                var buffer = new byte[4096];
                int bytesRead = 0;

                var attempts = msTimeout > 0 ? msTimeout / 100 : 1000;

                do
                {
                    this.SerialPort.ReadTimeout = 500;
                    try
                    {
                        Debug.WriteLine(string.Empty);
                        bytesRead = this.SerialPort.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            Logger.Info("Serial Reader Task Bytes read = {0}, append to buffer & read again", bytesRead);
                            Debug.WriteLine("Serial Reader Task Bytes read = {0}, append to buffer & read again", bytesRead);
                            var offset = (int)memoryStream.Position;
                            memoryStream.Write(buffer, offset, bytesRead);

                            if (this.SerialPort.BytesToRead > 0)
                            {
                                Debug.WriteLine("SerialPort.BytesToRead  = " + this.SerialPort.BytesToRead);
                            }
                            else
                            {
                                Debug.WriteLine("SerialPort.BytesToRead == 0, read again with timeout " + this.SerialPort.ReadTimeout);
                                //break;
                            }
                        }
                    }
                    catch (TimeoutException ex)
                    {
                        Logger.Info("Timeout Reading from serial Port {0} {1}", this.SerialPort.PortName, ex.Message);
                    }
                }
                while (attempts-- > 0 && this.IsOpen);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            finally
            {
                this.SerialPort.ReadTimeout = oldTimeout;
            }

            return memoryStream.ToArray();
        }

        /// <summary>The serial port on data received.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="serialDataReceivedEventArgs">The serial data received event args.</param>
        public virtual void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            if (serialDataReceivedEventArgs.EventType == SerialData.Chars)
            {
                Logger.Info("SerialPortOnDataReceived Event");
                Debug.WriteLine("Serial Data Ready");
                this.DataReceivedEvent.Set();
            }
        }

        /// <summary>The serial port on error received.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="serialErrorReceivedEventArgs">The serial error received event args.</param>
        public virtual void SerialPortOnErrorReceived(object sender, SerialErrorReceivedEventArgs serialErrorReceivedEventArgs)
        {
            Debug.WriteLine("SerialPortOnErrorReceived = " + serialErrorReceivedEventArgs.EventType);
            Logger.Error("Serial Data Error {0}", serialErrorReceivedEventArgs.EventType);
        }

        /// <summary>Write to the Stream a peripheral message.</summary>
        /// <param name="model">The model to serialize and write to the Stream.</param>
        /// <param name="waitForAck">True to wait for Ack RX response after write</param>
        /// <param name="stream">The stream to write to.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int"/>Total bytes written or -1 on error or Ack not received when requested.</returns>
        public int Write<T>(T model, bool waitForAck = false, Stream stream = null) where T : class, IPeripheralBaseMessage
        {
            Logger.Info("Writing<{0}> waitForAck={1}, Values {2}", typeof(T), model, model.ToString());

            if (!this.IsOpen)
            {
                Logger.Warn("Serial device not opened");
                return -1;
            }

            if (model.Header.Length > 0)
            {
                model.Header.Length--; // less the checksum byte
            }
            else
            {
                // Caution Marshal.SizeOf() will fail on a Generic Class!
                model.Header.Length = (ushort)(Marshal.SizeOf<T>() - sizeof(byte));
            }

            Debug.WriteLine("Message Header\n{0}, waitForAck={1}", model.Header.ToString(), waitForAck);
            if (stream == null)
            {
                stream = this.SerialPort.BaseStream;
            }

            // set header to network byte order on TX, include the leading framing byte
            var buffer = GetPeripheralBytesWriteBuffer(model, true);
            if (buffer.Length > 0)
            {
#if DEBUG
                var s = buffer.Aggregate(string.Empty, (current, b) => current + $"0x{b:X},");
                Debug.WriteLine("TX Data to stream " + s);
#endif
                try
                {
                    this.AckReceivedEvent.Reset();
                    stream.Write(buffer, 0, buffer.Length);
                    if (waitForAck)
                    {
                        Debug.WriteLine("Waiting for ACK after TXing " + typeof(T));
                        var signaled = this.AckReceivedEvent.WaitOne(2000);
                        if (!signaled)
                        {
                            Logger.Warn("Ack was not received in time for TX message of {0}", typeof(T));
                            return -1;
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    return -1;
                }
                catch (AbandonedMutexException)
                {
                    return -1;
                }
            }

            return buffer.Length;
        }

        #endregion

        #region Methods

        /// <summary>The process new peripheral message.</summary>
        /// <param name="peripheralBaseMessage">The peripheral base message.</param>
        /// <param name="bytesRead">The bytes read.</param>
        protected virtual void ProcessNewPreipheralMessage(IPeripheralBaseMessage peripheralBaseMessage, byte[] bytesRead)
        {
            if (peripheralBaseMessage == null)
            {
                return;
            }
            Logger.Info("RX response Fire event new message Bytes = {0}, Message Type {1}", bytesRead.Length, peripheralBaseMessage.GetType());
            Debug.WriteLine("RX response Fire event new message Bytes = {0}, Message Type {1}", bytesRead.Length, peripheralBaseMessage.GetType());

            this.FireDataReceived(bytesRead, peripheralBaseMessage);
        }

        private static byte[] GetPeripheralBytesWriteBuffer<T>(T model, bool headerInNetworkByteOrder = true, int totalFrameBytesToInclude = 1)
            where T : class, IPeripheralBaseMessage
        {
            // The Length byte in the Header does not include the byte for the Checksum (Not my Idea KSH)
            // So we expect the Header.Length here to be correct. Sizeof(Model) - One Byte for checksum
            // Convert to byte array and re-calculate the checksum and set as the last byte            
            var buffer = model.ToBytes();

            // update the checksum which must be the last byte
            var checksum = CheckSumUtil.CheckSum(buffer);
            Debug.WriteLine("Checksum = " + checksum);

            // Last byte in array is the checksum by definition            
            buffer[buffer.Length - 1] = checksum;

            var writeBuffer = new byte[totalFrameBytesToInclude + buffer.Length];
            if (headerInNetworkByteOrder)
            {
                // convert the header to Network Byte order and replace it to the buffer
                var header = buffer.FromBytes<PeripheralHeader>();
                if (header != null)
                {
                    if (headerInNetworkByteOrder)
                    {
                        header.HostToNetworkByteOrder();
                    }

                    var headerBytes = header.ToBytes();
                    headerBytes.CopyTo(buffer, 0);
                }
            }

            // Add the Frame Byte(s) as the first part of a write buffer message. Normally this would be one byte but we allow up to two.
            for (var i = 0; i < totalFrameBytesToInclude; i++)
            {
                writeBuffer[i] = DimmerConstants.PeripheralFramingByte;
            }

            buffer.CopyTo(writeBuffer, totalFrameBytesToInclude);
            return writeBuffer;
        }

        /// <summary>The serial background reader action.</summary>
        private void DataReaderAction()
        {
            try
            {
                var waitHandles = new WaitHandle[] { this.DataReceivedEvent, this.TerminateEvent };
                var buffer = new byte[4096];
                this.ReaderTaskRunning = true;
                var receivedBytesThreshold = this.SerialPort.ReceivedBytesThreshold;
                var originalRxTimeout = this.SerialPort.ReadTimeout;

                while (this.IsOpen)
                {
                    Debug.WriteLine("Waiting for RX Data or Termination events");
                    var idx = WaitHandle.WaitAny(waitHandles);
                    if (idx != 0)
                    {
                        Debug.WriteLine("SerialReader Task Terminated");
                        break;
                    }
                    
                    var memoryStream = new MemoryStream();
                    try
                    {
                        int bytesRead;
                        PeripheralHeader header = null;
                        try
                        {
                            this.SerialPort.ReadTimeout = 500;

                            do
                            {
                                bytesRead = this.SerialPort.Read(buffer, 0, buffer.Length);
                                if (bytesRead > 0)
                                {
                                    //Logger.Trace("Serial Reader Task Bytes read = {0}, append to buffer & read again", bytesRead);
                                    var bytes = buffer.Take(bytesRead).ToArray();
                                    Debug.WriteLine("Serial Reader Task Bytes read = {0}, at Position {1} append to buffer & read again", bytesRead, memoryStream.Position);

                                    memoryStream.Write(bytes, 0, bytes.Length);
                                    try
                                    {
                                        if (header == null)
                                        {
                                            header = new PeripheralHeader(bytes, true);
                                            if (header != null)
                                            {
                                                Debug.WriteLine("*** Found Header *** " + header.ToString());
                                                this.SerialPort.ReadTimeout = 10;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        // not enough to make header
                                    }
                                }

                                // Debug.WriteLine("Memory Stream Length = " + memoryStream.Length);
                            }
                            while (bytesRead > 0 && this.IsOpen);
                        }
                        catch (TimeoutException)
                        {
                            // expected, continue
                            Debug.WriteLine("RX Timeout");
                        }

                        var bytesReady = memoryStream.ToArray();

                        if (bytesReady.Length >= DimmerConstants.SmallestMessageSize)
                        {
                            Debug.WriteLine("Processing Stream Bytes = " + bytesReady.Length);
                            var peripheralBaseMessage = this.Read(bytesReady);
                            ProcessNewPreipheralMessage(peripheralBaseMessage, bytesReady);
                        }
                        else
                        {
                            Debug.WriteLine("Waiting for next RX!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Unhandled exception in background reader {0}", ex.Message);
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        this.DataReceivedEvent.Reset();
                        this.SerialPort.ReceivedBytesThreshold = receivedBytesThreshold;
                        this.SerialPort.ReadTimeout = originalRxTimeout;
                    }
                }
            }
            finally
            {
                this.ReaderTaskRunning = false;
                Debug.WriteLine("Reader Task Exited");
                Logger.Info("Reader Task has exited normally");
            }
        }

        /// <summary>The fire data received.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="message">The message.</param>
        private void FireDataReceived(byte[] bytes, IPeripheralBaseMessage message)
        {
            Task.Factory.StartNew(
                () =>
                    {
                        Debug.WriteLine("Peripheral Serial Data Read Bytes=" + bytes.Length);
                        var handler = this.PeripheralDataReady;
                        handler?.Invoke(this, new PeripheralDataReadyEventArg(bytes, message));
                    });
        }

        #endregion
    }
}