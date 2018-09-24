namespace Luminator.DimmerPeripheral.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using NLog;

    using Luminator.DimmerPeripheral.Core.Interfaces;
    using Luminator.DimmerPeripheral.Core.Models;
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Models;
    
    [Obsolete("See Luminator.PeripheralDimmer")]
    public class DimmerPeripheralSerialClient
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Events

        /// <summary>
        /// The peripheral data ready.
        /// </summary>
        public event EventHandler<DimmerPeripheralDataReadyEventArg> PeripheralDataReady;

        #endregion

        #region Fields

        /// <summary>
        /// The data received event.
        /// </summary>
        protected AutoResetEvent dataReceivedEvent = new AutoResetEvent(false);

        /// <summary>
        /// The serial reader task.
        /// </summary>
        protected Task serialReaderTask;

        /// <summary>
        /// The terminate event.
        /// </summary>
        protected ManualResetEvent terminateEvent = new ManualResetEvent(false);

        private readonly Dictionary<DimmerPeripheralMessageType, Type> messageTypesDictionary = new Dictionary<DimmerPeripheralMessageType, Type>();

        private readonly object timerLock = new object();

        private Timer backgroundRequestTimer;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DimmerPeripheralSerialClient{THandler}" /> class.
        /// </summary>
        public DimmerPeripheralSerialClient()
        {
            messageTypesDictionary.Add(DimmerPeripheralMessageType.Nak, typeof(DimmerPeripheralNak));
            messageTypesDictionary.Add(DimmerPeripheralMessageType.Ack, typeof(DimmerPeripheralAck));

            messageTypesDictionary.Add(DimmerPeripheralMessageType.QueryResponse, typeof(DimmerPeripheralQueryResponse));
            messageTypesDictionary.Add(DimmerPeripheralMessageType.VersionResponse,
                typeof(DimmerPeripheralVersionResponse));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimmerPeripheralSerialClient" /> class.
        /// </summary>
        /// <param name="serialPortSettings">The serial port settings.</param>
        public DimmerPeripheralSerialClient(IDimmerPeripheraISerialPortSettings serialPortSettings)
            : this()
        {
            Open(serialPortSettings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimmerPeripheralSerialClient" /> class.
        /// </summary>
        /// <param name="serialPortSettingsFileName">The serial port settings file name.</param>
        public DimmerPeripheralSerialClient(string serialPortSettingsFileName)
            : this()
        {
            var serialPortSettings = DimmerPeripheralSerialPortSettings.Read(serialPortSettingsFileName);
            this.Open(serialPortSettings);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the peripheral has connected an it's version retrieved.
        /// </summary>
        public bool IsConnected
        {
            get { return IsOpen && VersionInfo != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the serial port is open.
        /// </summary>
        public bool IsOpen
        {
            get { return SerialPort != null && SerialPort.IsOpen; }
        }

        /// <summary>
        /// Gets a value indicating whether reader task running.
        /// </summary>
        public bool ReaderTaskRunning { get; private set; }

        /// <summary>
        /// Gets or sets the serial port.
        /// </summary>
        public SerialPort SerialPort { get; set; }

        /// <summary>
        /// Gets the version info.
        /// </summary>
        public DimmerPeripheralVersionResponse VersionInfo { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The close.
        /// </summary>
        public virtual void Close()
        {
            Logger.Info("Close Enter");
            if (PeripheralDataReady != null)
                PeripheralDataReady -= PeripheralSerialClient_PeripheralDataReady;

            terminateEvent.Set();
            Thread.Sleep(10);
            if (SerialPort != null && SerialPort.IsOpen)
            {
                Logger.Info("Closing Serial Port {0}", SerialPort.PortName);
                SerialPort.Close();
            }
        }

        /// <summary>The dispose.</summary>
        public virtual void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Close();
            }
        }

        /// <summary>The open.</summary>
        /// <param name="serialPortSettings">The serial port settings.</param>
        /// <returns>The <see cref="Stream" />.</returns>
        /// <exception cref="IOException">The specified port could not be found or opened.</exception>
        public virtual Stream Open(IDimmerPeripheraISerialPortSettings serialPortSettings)
        {
            Logger.Info("Opening Serial Port {0} for Dimmer Peripheral", serialPortSettings);
            PeripheralDataReady += PeripheralSerialClient_PeripheralDataReady;

            SerialPort = new SerialPort(
                serialPortSettings.ComPort,
                serialPortSettings.BaudRate,
                serialPortSettings.Parity,
                serialPortSettings.DataBits,
                serialPortSettings.StopBits)
            {
                Handshake = Handshake.None,
                ReceivedBytesThreshold = DimmerConstants.RecieveBytesThreshold
            };

            if (serialPortSettings.BufferSize > 0)
            {
                SerialPort.ReadBufferSize = serialPortSettings.BufferSize;
                SerialPort.WriteBufferSize = serialPortSettings.BufferSize;
            }

            SerialPort.DataReceived += SerialPortOnDataReceived;
            SerialPort.ErrorReceived += SerialPortOnErrorReceived;
            SerialPort.Open();

            Stream stream = null;

            if (IsOpen)
            {
                if (serialPortSettings.EnableBackgroundReader)
                {
                    Logger.Info("Starting Dimmer background reader task on {0}", SerialPort.PortName);
                    serialReaderTask = new Task(DataReaderAction);
                    serialReaderTask.Start();
                }

                stream = SerialPort.BaseStream;
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
        /// <returns>The <see cref="object" />.</returns>
        public T Read<T>(byte[] bytes, bool networkByteOrderToHost = true) where T : class, IDimmerPeripheralBaseMessage
        {
            Logger.Info("Reading for type {0}, bytes length={1}, networkByteOrderToHost={2}", typeof(T), bytes.Length,
                networkByteOrderToHost);
            bytes = bytes.RemoveFramingBytes();
            var model = bytes.FromBytes<T>();
            if (networkByteOrderToHost && model != null)
                model.Header.NetworkByteOrderToHost();

            return model;
        }

        /// <summary>The read.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="networkByteOrderToHost">The network byte order to host.</param>
        /// <returns>The <see cref="object" />.</returns>
        public IDimmerPeripheralBaseMessage Read(byte[] bytes, bool networkByteOrderToHost = true)
        {
            try
            {
                var header = new PeripheralHeader(bytes, networkByteOrderToHost);
                switch ((DimmerPeripheralMessageType) header.MessageType)
                {
                    case DimmerPeripheralMessageType.QueryResponse:
                        return Read<DimmerPeripheralQueryResponse>(bytes, networkByteOrderToHost);
                    case DimmerPeripheralMessageType.VersionResponse:
                        return Read<DimmerPeripheralVersionResponse>(bytes, networkByteOrderToHost);
                    case DimmerPeripheralMessageType.Ack:
                        return Read<DimmerPeripheralAck>(bytes, networkByteOrderToHost);
                    case DimmerPeripheralMessageType.Nak:
                        return Read<DimmerPeripheralNak>(bytes, networkByteOrderToHost);
                    default:
                        return null;
                }
            }
            catch (NotSupportedException notSupportedException)
            {
                Logger.Error(notSupportedException.Message);
                return null;
            }
        }

        /// <summary>The serial port on data received.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="serialDataReceivedEventArgs">The serial data received event args.</param>
        public virtual void SerialPortOnDataReceived(object sender,
            SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            if (serialDataReceivedEventArgs.EventType == SerialData.Chars)
            {
                Logger.Info("Serial Data Ready");
                Debug.WriteLine("Serial Data Ready");
                dataReceivedEvent.Set();
            }
        }

        /// <summary>The serial port on error received.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="serialErrorReceivedEventArgs">The serial error received event args.</param>
        public virtual void SerialPortOnErrorReceived(object sender,
            SerialErrorReceivedEventArgs serialErrorReceivedEventArgs)
        {
            Debug.WriteLine("SerialPortOnErrorReceived = " + serialErrorReceivedEventArgs.EventType);
            Logger.Error("Serial Data Error {0}", serialErrorReceivedEventArgs.EventType);
        }

        /// <summary>The start background processing.</summary>
        /// <param name="msInterval">The ms interval.</param>
        public virtual void StartBackgroundProcessing(uint msInterval)
        {
            Logger.Info("StartBackgroundProcessing({0})", msInterval);
            StopBackgroundProcessing();
            backgroundRequestTimer = new Timer(TimerProc);
            backgroundRequestTimer.Change(0, msInterval);
        }

        /// <summary>The stop background processing.</summary>
        public virtual void StopBackgroundProcessing()
        {
            if (backgroundRequestTimer != null)
            {
                Logger.Info("StopBackgroundProcessing()");
                backgroundRequestTimer.Dispose();
                backgroundRequestTimer = null;
            }
        }

        /// <summary>Write to the Stream.</summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int" />.</returns>
        public int Write<T>(T model, Stream stream = null) where T : class, IDimmerPeripheralBaseMessage
        {
            if (model.Header.Length > 0)
                model.Header.Length--; // less the checksum byte
            else
                model.Header.Length = (ushort) (Marshal.SizeOf<T>() - sizeof(byte));

            Debug.WriteLine("Header\n" + model.Header);
            if (stream == null)
                stream = SerialPort.BaseStream;

            // set header to network byte order on TX
            var bytes = GetPeripheralBytesWriteBuffer(model, true);
            if (bytes.Length > 0)
            {
#if DEBUG
                var s = string.Empty;
                foreach (var b in bytes)
                    s += string.Format("0x{0:X},", b);

                Debug.WriteLine("TX Data to stream " + s);
#endif
                stream.Write(bytes, 0, bytes.Length);
            }

            return bytes.Length;
        }

        /// <summary>The write query request.</summary>
        public void WriteQueryRequest()
        {
            Write(new DimmerPeripheralQueryRequest());
        }

        #endregion

        #region Methods

        private static byte[] GetPeripheralBytesWriteBuffer<T>(T model, bool headerInNetworkByteOrder = true,
            int totalFrameBytesToInclude = 1)
            where T : class, IDimmerPeripheralBaseMessage
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
                        header.HostToNetworkByteOrder();

                    var headerBytes = header.ToBytes();
                    headerBytes.CopyTo(buffer, 0);
                }
            }

            // Add the Frame Byte as the first part of a write buffer message. Normally this would be one byte but we allow up to two.
            for (var i = 0; i < totalFrameBytesToInclude; i++)
                writeBuffer[i] = DimmerConstants.PeripheralFramingByte;

            buffer.CopyTo(writeBuffer, totalFrameBytesToInclude);
            return writeBuffer;
        }

        /// <summary>The serial reader action.</summary>
        private void DataReaderAction()
        {
            try
            {
                var waitHandles = new WaitHandle[] {dataReceivedEvent, terminateEvent};
                var buffer = new byte[4096];
                ReaderTaskRunning = true;

                while (IsOpen)
                {
                    var idx = WaitHandle.WaitAny(waitHandles);
                    if (idx != 0)
                    {
                        Debug.WriteLine("SerialReader Task Terminated");
                        break;
                    }

                    SerialPort.ReadTimeout = 1000;
                    var memoryStream = new MemoryStream();

                    try
                    {
                        int bytesRead;

                        do
                        {
                            bytesRead = SerialPort.Read(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                Logger.Info("Serial Reader Task Bytes read {0}", bytesRead);
                                var offset = (int) memoryStream.Position;
                                memoryStream.Write(buffer, offset, bytesRead);

                                if (SerialPort.BytesToRead > 0)
                                    SerialPort.ReadTimeout = 100;
                                else
                                    break;
                            }
                        } while (bytesRead > 0 && IsOpen);
                    }
                    catch (TimeoutException)
                    {
                    }

                    var bytes = memoryStream.ToArray();
                    if (bytes.Length >= DimmerConstants.SmallestMessageSize)
                        try
                        {
                            var obj = Read(bytes);
                            FireDataReceived(bytes, obj);
                        }
                        catch
                        {
                        }
                }
            }
            finally
            {
                ReaderTaskRunning = false;
                Debug.WriteLine("Reader Task Exited");
                Logger.Info("Reader Task has exited normally");
            }
        }

        public byte[] Read(int msTimeout = 3000)
        {
            var oldTimeout = SerialPort.ReadTimeout;
            try
            {
                if (SerialPort.IsOpen)
                {
                    SerialPort.ReadTimeout = msTimeout;
                    var buffer = new byte[4096];
                    var bytesRead = SerialPort.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                        return buffer.Take(bytesRead).ToArray();
                }
            }
            catch (TimeoutException ex)
            {
                Logger.Info("Timeout Reading from serial Port {0} {1}", SerialPort.PortName, ex.Message);
            }
            finally
            {
                SerialPort.ReadTimeout = oldTimeout;
            }

            return new byte[0];
        }

        private void DoTimerRequest()
        {
            // TODO custom action here to send the Query message for brightness levels
            WriteQueryRequest();
        }

        private Type FindMessageType(DimmerPeripheralMessageType type)
        {
            return messageTypesDictionary.ContainsKey(type) ? messageTypesDictionary[type] : null;
        }

        /// <summary>The fire data received.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="message">The message.</param>
        private void FireDataReceived(byte[] bytes, IDimmerPeripheralBaseMessage message)
        {
            Task.Factory.StartNew(
                () =>
                {
                    Debug.WriteLine("Serial Data Read Bytes=" + bytes.Length);
                    var handler = PeripheralDataReady;
                    handler?.Invoke(this, new DimmerPeripheralDataReadyEventArg(bytes, message));
                });
        }

        private void PeripheralSerialClient_PeripheralDataReady(object sender, DimmerPeripheralDataReadyEventArg e)
        {
        }

        private void TimerProc(object state)
        {
            Debug.WriteLine("TimerProc Enter");
            if (Monitor.TryEnter(timerLock, 1000))
            {
                DoTimerRequest();
                Monitor.Exit(timerLock);
            }
        }

        #endregion
    }
}