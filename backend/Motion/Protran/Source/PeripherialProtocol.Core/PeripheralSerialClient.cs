// InfomediaAll
// Luminator.PeripheralProtocol.Core
// <author>Kevin Hartman</author>
// $Rev::                                   
namespace Luminator.PeripheralProtocol.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.SystemManagement.Host.Path;

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    using NLog;

    /// <summary>The peripheral serial client.</summary>
    /// <typeparam name="THandler"></typeparam>
    public class PeripheralSerialClient<THandler> : IPeripheralSerialClient
        where THandler : class, IPeripheralHandler, new()
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool versionRequestFailedErrorGiven;

        #endregion

        #region Fields

        private readonly ManualResetEvent endEvent = new ManualResetEvent(false);

        private readonly ManualResetEvent readerThreadReady = new ManualResetEvent(false);

        private readonly AutoResetEvent serialDataReceivedEvent = new AutoResetEvent(false);

        private readonly ManualResetEvent stoppedEvent = new ManualResetEvent(false);

        private readonly AutoResetEvent streamReadyForProcessing = new AutoResetEvent(false);

        private readonly object writeLock = new object();

        private Thread backgroundReaderThread;

        private WriteModelWithAck currentWriteModelWithAck = new WriteModelWithAck(null);

        private bool disposed;

        private Task processMessageTask;

        private int versionReceived;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralSerialClient{THandler}" /> class.</summary>
        public PeripheralSerialClient()
        {
            this.PeripheralDataReceived += this.OnPeripheralDataReceived;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PeripheralSerialClient{THandler}" /> class. Initializes a new
        ///     instance of the <see cref="PeripheralSerialClient" /> class. The audio switch client.
        /// </summary>
        /// <param name="peripheralConfig">The PeripheralSerialClient Configuration.</param>
        /// <exception cref="Exception">Construction Failed, see log.</exception>
        public PeripheralSerialClient(PeripheralConfig peripheralConfig)
            : this()
        {
            try
            {
                if (peripheralConfig == null)
                {
                    // use the local file if found
                    var fileName = PeripheralConfig.DefaultPeripheralConfigName;
                    if (File.Exists(fileName) == false)
                    {
                        fileName = PathManager.Instance.CreatePath(FileType.Config, AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName);
                    }
                    else
                    {
                        throw new FileNotFoundException("Failed to find default PeripheralConfig file", fileName);
                    }

                    peripheralConfig = PeripheralConfig.ReadPeripheralConfig<PeripheralConfig>(fileName);
                }

                if (peripheralConfig?.SerialPortSettings == null)
                {
                    throw new ArgumentNullException(nameof(peripheralConfig), "Invalid AudioSwitchConfig Settings");
                }

                var result = this.Open(peripheralConfig);
                Logger.Info("Open Peripheral Result = {0}", result);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to construct AudioSwitchClient {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PeripheralSerialClient{THandler}" /> class. Initializes a new
        ///     instance of the <see cref="PeripheralSerialClient" /> class.
        /// </summary>
        /// <param name="serialPortSettings">The serial port settings.</param>
        /// <param name="peripheralConfigFile">The peripheral Config File.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ApplicationException">Reader thread already started</exception>
        public PeripheralSerialClient(SerialPortSettings serialPortSettings, string peripheralConfigFile)
            : this()
        {
            if (serialPortSettings == null || string.IsNullOrEmpty(serialPortSettings.ComPort))
            {
                throw new ArgumentNullException(nameof(serialPortSettings), "Invalid SerialPortSettings");
            }

            var peripheralConfig = PeripheralConfig.ReadPeripheralConfig<PeripheralConfig>(peripheralConfigFile);
            peripheralConfig.SerialPortSettings = serialPortSettings;
            this.Open(peripheralConfig);
        }

        #endregion

        #region Public Events

        /// <summary>The peripheral data received.</summary>
        public event EventHandler<PeripheralDataReceivedEventArgs> PeripheralDataReceived;

        /// <summary>The peripheral image update info changed.</summary>
        public event EventHandler<PeripheralImageUpdateInfo> PeripheralImageUpdateInfoChanged;

        /// <summary>The serial data received.</summary>
        public event EventHandler<SerialDataReceivedEventArgs> SerialDataReceived;

        /// <summary>The serial error received.</summary>
        public event EventHandler<SerialErrorReceivedEventArgs> SerialErrorReceived;

        /// <summary>The version info changed.</summary>
        public event EventHandler<PeripheralVersionsInfo> VersionInfoChanged;

        #endregion

        #region Public Properties

        /// <summary>Gets the default com port.</summary>
        public static string DefaultComPort
        {
            get
            {
                var comport = Environment.GetEnvironmentVariable("AudiowitchComPort");
                return string.IsNullOrEmpty(comport) ? "COM4" : comport;
            }
        }

        public PeripheralConfig Config { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether is valid com port and opened.
        ///     <see>IsAudioSwitchConnected</see>
        /// </summary>
        public bool IsComPortOpen
        {
            get
            {
                return this.SerialPort != null && this.SerialPort.IsOpen;
            }
        }

        /// <summary>Gets a value indicating whether is peripheral has connected and exchanged it's version information.</summary>
        public virtual bool IsPeripheralConnected
        {
            get
            {
                // we have a open serial port and we have received the audio switch's version after we configured it
                // See WriteAudioConfig
                var hasVersion = this.PeripheralVersionInfo != null;
                return this.SerialPort != null && this.IsComPortOpen && hasVersion;
            }
        }

        /// <summary>Gets or sets a value indicating whether the peripheral has been configured.</summary>
        public bool IsVersionInfoReceived
        {
            get
            {
                return this.versionReceived != 0;
            }

            set
            {
                Interlocked.Increment(ref this.versionReceived);
            }
        }

        /// <summary>Gets the peripheral framing bytes count.</summary>
        public int PeripheralFramingBytesCount { get; private set; }

        /// <summary>Gets or sets the peripheral handler.</summary>
        public IPeripheralHandler PeripheralHandler { get; set; }

        /// <summary>Gets or sets the peripheral version info.</summary>
        public PeripheralVersionsInfo PeripheralVersionInfo
        {
            get
            {
                return this.State != null && this.State.PeripheralContext != null ? this.State.PeripheralContext.PeripheralVersions : null;
            }

            set
            {
                this.State.PeripheralContext.PeripheralVersions = value;
                var handler = this.VersionInfoChanged;
                handler?.Invoke(this, value);
            }
        }

        /// <summary>Gets the port name.</summary>
        public string PortName
        {
            get
            {
                return this.SerialPort != null ? this.SerialPort.PortName : string.Empty;
            }
        }

        /// <summary>Gets the serial port.</summary>
        public SerialPort SerialPort { get; set; }

        /// <summary>Gets or sets the state.</summary>
        public PeripheralState State { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Flush the serial port</summary>
        /// <param name="serialPort">Opened Serial Port</param>
        public static void FlushSerialPort(SerialPort serialPort)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                Logger.Info("Flushing Serial Port {0} Enter", serialPort.PortName);
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                var rxTimeout = serialPort.ReadTimeout;
                serialPort.ReadTimeout = 1000;
                try
                {
                    var buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = serialPort.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Logger.Warn("!Flushing Serial RX Buffer bytes removed = {0}", bytesRead);
                    }
                }
                catch
                {
                }
                finally
                {
                    serialPort.ReadTimeout = rxTimeout;
                    Logger.Info("Flushing Serial Port {0} Exit", serialPort.PortName);
                }
            }
        }

        /// <summary>The open serial port.</summary>
        /// <param name="settings">The Serial port settings.</param>
        /// <param name="serialDataReceivedEventHandler">The optional Serial Data Received handler.</param>
        /// <param name="serialErrorReceivedHandler">The optional serial Error Received.</param>
        /// <exception cref="IOException">The specified port could not be found or opened.</exception>
        /// <returns>The <see cref="SerialPort" />.</returns>
        /// <exception cref="UnauthorizedAccessException">
        ///     Access is denied to the port.- or -The current process, or another
        ///     process on the system, already has the specified COM port open either by a
        ///     <see cref="T:System.IO.Ports.SerialPort" /> instance or in unmanaged code.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The specified port on the current instance of the
        ///     <see cref="T:System.IO.Ports.SerialPort" /> is already open.
        /// </exception>
        public static SerialPort OpenSerialPort(
            SerialPortSettings settings, 
            SerialDataReceivedEventHandler serialDataReceivedEventHandler = null, 
            SerialErrorReceivedEventHandler serialErrorReceivedHandler = null)
        {
            Logger.Info("Creating PeripherialSerialClient Opening Serial device on {0}", settings);

            var serialPort = OpenSerialPort(
                settings.ComPort, 
                settings.BaudRate, 
                settings.DataBits, 
                settings.Parity, 
                settings.StopBits, 
                settings.BufferSize, 
                settings.ReceivedBytesThreshold);
            if (serialPort.IsOpen)
            {
                serialPort.DtrEnable = settings.DtrControl;
                serialPort.RtsEnable = settings.RtsControl;

                serialPort.ErrorReceived += (sender, args) =>
                    {
                        Logger.Error("Error on serial port SerialError: {0}", args.EventType);
                        Debug.WriteLine("ErrorReceived on port Error = " + args.EventType);
                    };

                FlushSerialPort(serialPort);

                if (serialErrorReceivedHandler != null)
                {
                    serialPort.ErrorReceived += serialErrorReceivedHandler;
                }

                if (serialDataReceivedEventHandler != null)
                {
                    serialPort.DataReceived += serialDataReceivedEventHandler;
                }
            }
            else
            {
                throw new IOException("Failed to open serial port " + settings);
            }

            return serialPort;
        }

        /// <summary>The open serial port.</summary>
        /// <param name="comPort">The com Port.</param>
        /// <param name="baudRate">The baud Rate.</param>
        /// <param name="dataBits">The data Bits.</param>
        /// <param name="parity">The parity.</param>
        /// <param name="stopBits">The stop Bits.</param>
        /// <param name="bufferSize"></param>
        /// <param name="receiveBytesThreshold">
        ///     The number of bytes in the internal input buffer before a DataReceived event is
        ///     fired. The default is 1.
        /// </param>
        /// <exception cref="IOException">The specified port could not be found or opened.</exception>
        /// <returns>The <see cref="SerialPort" />.</returns>
        /// <exception cref="UnauthorizedAccessException">
        ///     Access is denied to the port.- or -The current process, or another
        ///     process on the system, already has the specified COM port open either by a
        ///     <see cref="T:System.IO.Ports.SerialPort" /> instance or in unmanaged code.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The specified port on the current instance of the
        ///     <see cref="T:System.IO.Ports.SerialPort" /> is already open.
        /// </exception>
        public static SerialPort OpenSerialPort(
            string comPort, 
            int baudRate = SerialPortSettings.DefaultBaudRate, 
            int dataBits = SerialPortSettings.DefaultDataBits, 
            Parity parity = Parity.None, 
            StopBits stopBits = StopBits.One, 
            int bufferSize = PeripheralState.DefaultBufferSize, 
            int receiveBytesThreshold = 1)
        {
            try
            {
                // set the RX threshold to be at least the size of or smallest message before signaling data ready event
                if (receiveBytesThreshold < 1)
                {
                    receiveBytesThreshold = 1;
                }

                var port = new SerialPort(comPort, baudRate, parity, dataBits, stopBits)
                               {
                                   Encoding = Encoding.UTF8, 
                                   ReadBufferSize = bufferSize, 
                                   WriteBufferSize = bufferSize, 

                                   // signals RX data ready only when we receive the Frame Byte + Message & Checksum trailing byte!!!
                                   ReceivedBytesThreshold = receiveBytesThreshold
                               };

                port.Open();
                return port;
            }
            catch (UnauthorizedAccessException accessException)
            {
                Logger.Error("Failed to open Serial port {0}. Cause: {1}", comPort, accessException.Message);
                throw;
            }
        }

        /// <summary>
        ///     Close the Serial Port
        /// </summary>
        public virtual void Close()
        {
            Logger.Info("PeripheralSerialClient.CLose Enter");
            this.StopPeripheralReaderThread();
            if (this.PeripheralHandler != null)
            {
                Logger.Info("PeripheralSerialClient. Disposing of Handler...");
                this.PeripheralHandler.Dispose();
            }

            this.PeripheralHandler = null;
            if (this.IsComPortOpen)
            {
                try
                {
                    Logger.Info("PeripheralSerialClient. Closing Serial Port");
                    this.SerialPort.Close();
                }
                catch (IOException ioException)
                {
                    Logger.Error("Failed To Close Serial Port {0}", ioException.Message);
                }
            }
        }

        /// <summary>The create peripheral handler.</summary>
        /// <param name="config">The config.</param>
        /// <returns>The <see cref="THandler" />.</returns>
        /// <exception cref="MissingMethodException">
        ///     NoteIn the .NET for Windows Store applications or the Portable Class Library,
        ///     catch the base class exception, <see cref="T:System.MissingMemberException" />, instead.The type that is specified
        ///     for <paramref name="T" /> does not have a parameter less constructor.
        /// </exception>
        /// <exception cref="ApplicationException">Reader thread already started</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread. </exception>
        /// <exception cref="ThreadStateException">Thread failed to start.</exception>
        public IPeripheralHandler CreatePeripheralHandler(PeripheralConfig config)
        {
            Logger.Info("{0} Enter PeripheralHandler for PeripheralConfig...", nameof(this.CreatePeripheralHandler));
            THandler handler = default(THandler);
            if (config.Enabled)
            {
                // throw new NotImplementedException("HEY DUDE DO THIS");
                Stream stream;
                var startSerialReader = false;
                if (string.IsNullOrEmpty(config.SerialPortSettings.ComPort) == false)
                {
                    this.SerialPort = this.Open(config.SerialPortSettings);
                    stream = this.SerialPort.BaseStream;
                    startSerialReader = this.SerialPort.IsOpen;
                }
                else
                {
                    // No Com Port Name given use internal stream for testing
                    stream = new MemoryStream();
                    Logger.Warn(
                        "{0} PeripheralSerialClient! Skipped opening Serial port with name missing or undefined", 
                        nameof(this.CreatePeripheralHandler));
                }

                var peripheralContext = new PeripheralContext(config, stream);
                this.State = new PeripheralState(peripheralContext);
                handler = Activator.CreateInstance<THandler>();
                handler.PeripheralContext = peripheralContext;

                if (startSerialReader)
                {
                    this.processMessageTask = this.StartMessageProcessingTask(this.State);
                    this.StartPeripheralReaderThread(this.State);
                }
            }
            else
            {
                Logger.Warn("PeripheralSerialClient.CreateHandler() Enabled == False. Handler create ignored");
            }

            return handler;
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            this.PeripheralHandler?.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>The get update image total records.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="imageUpdateType">The image update type.</param>
        /// <returns>The <see cref="int" />.</returns>
        public virtual int GetUpdateImageTotalRecords(string fileName, PeripheralImageUpdateType imageUpdateType)
        {
            switch (imageUpdateType)
            {
                case PeripheralImageUpdateType.IntelHex:
                    return File.ReadLines(fileName).Count();
            }

            return 0;
        }

        /// <summary>The initialize peripheral.</summary>
        /// <param name="stateObject">The state object.</param>
        public virtual void InitializePeripheral(object stateObject)
        {
            var state = stateObject as PeripheralState;
            if (state == null || state.Running || this.IsComPortOpen == false)
            {
                Logger.Error("InitializeClient() Invalid State");
            }
        }

        /// <summary>Opens the Serial Port creating a handler.</summary>
        /// <param name="peripheralConfig">The Peripheral Configuration</param>
        /// <exception cref="ApplicationException">Audio Switch Already created and Opened</exception>
        /// <returns>The <see cref="bool" />.</returns>
        /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread. </exception>
        /// <exception cref="ThreadStateException">Thread failed to start.</exception>
        public bool Open(PeripheralConfig peripheralConfig)
        {
            Logger.Info("Open() Peripheral Client Enter settings: {0}", peripheralConfig.SerialPortSettings);
            this.Config = peripheralConfig;

            if (this.PeripheralHandler == null)
            {
                try
                {
                    this.PeripheralHandler = this.CreatePeripheralHandler(peripheralConfig);
                }
                catch (MissingMethodException missingMethodException)
                {
                    Logger.Error("Open() Peripheral Client Failed {0}", missingMethodException.Message);
                    throw;
                }
            }

            this.PeripheralFramingBytesCount = peripheralConfig.DefaultMaxPeripheralFramingBytes;

            if (string.IsNullOrEmpty(peripheralConfig?.SerialPortSettings.ComPort) == false)
            {
                if (this.SerialPort == null || this.IsComPortOpen == false)
                {
                    throw new ApplicationException("Peripheral Serial port " + this.PortName + " is already created and opened!");
                }

                var portOpened = this.IsComPortOpen;
                Logger.Info("Open() Peripheral Client completed {0}", portOpened ? "Success" : "Failed");
                return portOpened;
            }

            return true;
        }

        /// <summary>Remove next message from the stream.</summary>
        /// <param name="state">The state.</param>
        /// <returns>The <see cref="object" />Message object or null if nothing.</returns>
        public virtual object RemoveNextMessage(PeripheralState state)
        {
            try
            {
                state.LockStream();
                return this.PeripheralHandler?.RemoveNextMessage(state);
            }
            finally
            {
                state.UnLockStream();
            }
        }

        /// <summary>The update image.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="imageUpdateType">The image update type.</param>
        /// <returns></returns>
        public virtual bool UpdateImage(string fileName, PeripheralImageUpdateType imageUpdateType)
        {
            Logger.Info("UpdateImage started File={0}, Type={1}", fileName, imageUpdateType);
            var totalRecords = this.GetUpdateImageTotalRecords(fileName, imageUpdateType);
            return totalRecords > 0;
        }

        /// <summary>
        /// Write the model w/o waiting for ACK
        /// </summary>
        /// <typeparam name="T">Message type IPeripheralBaseMessage</typeparam>
        /// <param name="model">Model to write</param>
        /// <returns></returns>
        public int Write<T>(T model) where T : class, IPeripheralBaseMessage
        {
            bool ackReceived;
            return this.Write(model, out ackReceived, false);
        }

        /// <summary>Write the peripheral model.</summary>
        /// <param name="model">The model.</param>
        /// <param name="waitForAck">The wait For Ack.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int" />.</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public int Write<T>(T model, out bool ackReceived, bool waitForAck = false) where T : class, IPeripheralBaseMessage
        {
            lock (this.writeLock)
            {
                if (model.Header.MessageType != PeripheralMessageType.Ack && model.Header.MessageType != PeripheralMessageType.Nak)
                {
                    this.State.LastMessageType = model.Header.MessageType;
                }

                int maxAttempts = this.Config.MaxWritesWhenWaitForAck;
                int waitForAckTimeout = this.Config.WaitForAckTimeout;
                ackReceived = false;
                int bytesWritten = 0;
                var attempts = 0;

                // record our last model/message written to tie a received ACK for if we wait for it - Debug case
                this.currentWriteModelWithAck = new WriteModelWithAck(model);

                do
                {
                    if (this.SerialPort.IsOpen)
                    {
                        Logger.Info(
                            "TX Write<{0}> MessageType={1} waitForAck={2}, waitForAckTimeout={3}", 
                            typeof(T), 
                            model.Header.MessageType, 
                            waitForAck, 
                            waitForAckTimeout);
                        Debug.WriteLine("TX to serial ");

                        try
                        {
                            // write the model with header in the correct byte order
                            bytesWritten = this.PeripheralHandler.Write(model, this.SerialPort.BaseStream);
                        }
                        catch (InvalidOperationException ex)
                        {
                            return 0;
                        }
                        
                        if (waitForAck && bytesWritten > 0)
                        {
                            Logger.Info(
                                "Waiting for ACK for TX of MessageType {0}, Attempts {1} of {2}", 
                                model.Header.MessageType, 
                                attempts + 1, 
                                maxAttempts);
                        }

                        if (waitForAck)
                        {
                            // wait for ACK message reply for this model sent
                            try
                            {
                                ackReceived = this.currentWriteModelWithAck.AckRecievedEvent.WaitOne(waitForAckTimeout);
                                if (ackReceived)
                                {
                                    Logger.Info("Success Received ACK for TX of {0}", typeof(T));
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                ackReceived = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                while (waitForAck && (bytesWritten > 0 && !ackReceived) && ++attempts < maxAttempts);

                if (waitForAck && !ackReceived)
                {
                    Logger.Warn("Failed to received ACK for message {0}", model.Header.MessageType);
                }

                Debug.WriteLine(
                    "TX {0} bytes MessageType={1} waitForAck={2}, Ack Received={3}, Attempts={4}", 
                    bytesWritten, 
                    model.Header.MessageType, 
                    waitForAck, 
                    ackReceived, 
                    attempts);

                return bytesWritten;
            }
        }

        /// <summary>Write ack message.</summary>
        /// <returns>The <see cref="int" />Total Bytes written</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public virtual int WriteAck()
        {
            return this.Write(new PeripheralAck(Constants.DefaultPeripheralAddress));
        }

        /// <summary>Write NAK message.</summary>
        /// <returns>The <see cref="int" />Total Bytes written</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public virtual int WriteNak()
        {
            return this.Write(new PeripheralNak(Constants.DefaultPeripheralAddress));
        }

        /// <summary>The write poll.</summary>
        /// <returns>The <see cref="int" />.</returns>
        public virtual int WritePoll()
        {
            return this.Write(new PeripheralPoll(Constants.DefaultPeripheralAddress));
        }

        /// <summary>
        ///     Write the raw bytes out to the Stream with the header written in network byte order and the lead framing byte
        ///     written as the first byte to the stream.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <exception cref="ArgumentException">Invalid Buffer for PeripheralHeader! Bytes does not contain a valid header</exception>
        /// <returns>The <see cref="int" />Total Bytes written</returns>
        /// <exception cref="ArgumentException">Invalid Buffer for PeripheralHeader!</exception>
        /// <exception cref="OverflowException">
        ///     The array is multidimensional and contains more than
        ///     <see cref="F:System.Int32.MaxValue" /> elements.
        /// </exception>
        public int WriteRaw(byte[] bytes)
        {
            if (bytes == null)
            {
                return 0;
            }

            int byteSent = 0;
            lock (this.writeLock)
            {
                // can we determine the message type from the header
                PeripheralHeader peripheralHeader;
                try
                {
                    peripheralHeader = new PeripheralHeader(bytes);
                }
                catch
                {
                    // buffer content is not a header
                    peripheralHeader = null;
                }

                if (peripheralHeader == null)
                {
                    // just write what we have
                    byteSent = bytes.Length;
                    try
                    {
                        this.SerialPort.Write(bytes, 0, byteSent);
                    }
                    catch (TimeoutException)
                    {
                    }
                }
                else
                {
                    var peripheralMessageType = Core.PeripheralHandler.FindPeripheralMessageType(peripheralHeader);
                    if (peripheralMessageType != null)
                    {
                        if (peripheralHeader.MessageType != PeripheralMessageType.Ack && peripheralHeader.MessageType != PeripheralMessageType.Nak)
                        {
                            this.State.LastMessageType = peripheralHeader.MessageType;
                        }

                        var writeBuffer = Core.PeripheralHandler.GetPeripheralBytesWriteBuffer(bytes);
                        Debug.WriteLine("WriteRaw bytes type " + peripheralMessageType);
                        if (writeBuffer != null)
                        {
                            // HACK to write an extra Frame Byte            
                            // Needed if Header.Length include Checksum byte which it should not when TX to Peripheral                
                            if (writeBuffer.Count(x => x == Constants.PeripheralFramingByte) <= 1)
                            {
                                this.SerialPort.Write(new[] { Constants.PeripheralFramingByte }, 0, 1);
                                byteSent++;
                            }

                            byteSent += writeBuffer.Length;
                            this.SerialPort.Write(writeBuffer, 0, writeBuffer.Length);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Unknown raw audio switch peripheral buffer type. Not Supported");
                    }
                }
            }

            return byteSent;
        }

        /// <summary>Write a request for version</summary>
        /// <param name="msTimeout">The Timeout in milliseconds to wait for the reply.</param>
        /// <param name="address">Header Address</param>
        /// <returns>The <see cref="int" />.</returns>
        public virtual bool WriteVersionRequest(int msTimeout = 3000, ushort address = Constants.DefaultPeripheralAddress)
        {
            var versionReceivedManualEvent = new ManualResetEvent(false);
            var nakEvent = new ManualResetEvent(false);
            bool success = false;
            try
            {
                this.VersionInfoChanged += (sender, version) =>
                    {
                        Logger.Info("Success Peripheral Versions received = {0} ", version);
                        success = true;
                        versionReceivedManualEvent.Set();
                    };

                //this.PeripheralDataReceived += delegate(object sender, PeripheralDataReceivedEventArgs args)
                //    {
                //        var versions = args.Message as PeripheralAudioVersions;
                //        if (versions != null)
                //        {
                //            var version = versions;
                //            Logger.Info("Success Peripheral Versions received = {0} ", version);
                //            versionReceivedManualEvent.Set();
                //        }
                //        else if (args.Message.IsNakMessage())
                //        {
                //            Logger.Info("NAK received for WriteVersionRequest(), Legacy hardware detected");
                //            nakEvent.Set();
                //        }
                //    };

                Logger.Info("TX Version Request from Audio Mux Peripherial...");
                lock (this.writeLock)
                {
                    if (this.Write(new PeripheralRequestVersion(address)) > 0)
                    {
                        var idx = WaitHandle.WaitAny(new WaitHandle[] { versionReceivedManualEvent, nakEvent }, msTimeout);
                        success = idx == 0;
                    }
                }
            }
            catch
            {
            }

            if (!success)
            {
                var logLevel = LogLevel.Warn;
                var s = string.Format("(!) No Reply from Audio Switch Peripheral for requesting Version! Check hardware connections and Serial Port {0}", this.PortName);
                if (!versionRequestFailedErrorGiven)
                {
                    logLevel = LogLevel.Fatal;
                    versionRequestFailedErrorGiven = true;

                    // Create a Alarm and broadcast to Alarm log
                    var alarm = SystemAlarmFactory.CreateTest(SystemTestAttribute.Test, s);
                    MessageDispatcher.Instance.Broadcast(alarm);
                }

                Logger.Log(logLevel, s);
            }

            return success;
        }      

        #endregion

        #region Methods

        /// <summary>The fire peripheral image update info.</summary>
        /// <param name="peripheralImageUpdateInfo">The peripheral image update info.</param>
        protected void FirePeripheralImageUpdateInfo(PeripheralImageUpdateInfo peripheralImageUpdateInfo)
        {
            var handler = this.PeripheralImageUpdateInfoChanged;
            handler?.Invoke(this, peripheralImageUpdateInfo);
        }

        /// <summary>
        ///     Close the serial port.
        /// </summary>
        private void CloseSerialPort()
        {
            try
            {
                if (this.SerialPort != null && this.SerialPort.IsOpen)
                {
                    this.PeripheralDataReceived -= this.OnPeripheralDataReceived;

                    Debug.WriteLine("CloseSerialPort Closing Serial Port " + this.SerialPort.PortName);
                    this.SerialPort.Close();
                    Logger.Info("{0} closed serial port {1}", nameof(this.CloseSerialPort), this.SerialPort.PortName);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception closing serial port {0}", ex.Message);
            }
        }

        /// <summary>The dispose.</summary>
        /// <param name="disposing">The disposing.</param>
        private void Dispose(bool disposing)
        {
            try
            {
                Logger.Info("Dispose PeripheralSerialClient");
                if (disposing && this.disposed == false)
                {
                    this.Close();
                }
            }
            finally
            {
                this.disposed = true;
            }
        }

        /// <summary>Find a valid Peripheral header in the buffer</summary>
        /// <param name="buffer">buffer data</param>
        /// <returns>PeripheralHeader or null if not found</returns>
        private PeripheralHeader FindPeripheralHeader(byte[] buffer)
        {
            try
            {
                Debug.WriteLine("FindPeripheralHeader finding header...");
                return buffer.GetValidPeripheralHeader(this.PeripheralHandler.IsHeaderNetworkByteOrder);
            }
            catch (InvalidDataException ex)
            {
                Logger.Warn("Failed to find valid Peripheral header {0}", ex.Message);
                return null;
            }
        }

        /// <summary>Fire the Peripheral message received</summary>
        /// <param name="message"></param>
        private void FirePeripheralDataReceived(object message)
        {
            if (message != null)
            {
                var peripheralDataReceivedEventArgs = new PeripheralDataReceivedEventArgs(message);
                this.FirePeripheralDataReceivedEvent(this, peripheralDataReceivedEventArgs);
            }
        }

        private void FirePeripheralDataReceivedEvent(object sender, PeripheralDataReceivedEventArgs peripheralDataReceivedEventArgs)
        {
            var handler = this.PeripheralDataReceived;
            if (handler != null)
            {
                Debug.WriteLine("FirePeripheralDataReceivedEvent() Enter");
                Logger.Info(
                    "Fire Event PeripheralDataReceived message type = {0}", 
                    peripheralDataReceivedEventArgs.Message != null
                        ? peripheralDataReceivedEventArgs.Message.Header.MessageType
                        : PeripheralMessageType.Unknown);
                handler(sender, peripheralDataReceivedEventArgs);
            }
        }

        private void OnPeripheralDataReceived(object sender, PeripheralDataReceivedEventArgs args)
        {
            var model = this.currentWriteModelWithAck;
            var message = model.PeripheralMessage;
            if (message == null)
            {
                return;
            }

            if (args.Message.IsAckMessage())
            {
                var peripheralAudioEnable = message as PeripheralAudioEnable;
                Debug.WriteLine("ACK received for TX of MessageType " + message.Header.MessageType);
                if (peripheralAudioEnable != null)
                {
                    Logger.Info(
                        "ACK received for TX of {0} ActiveSpeakerZone = {1}", 
                        peripheralAudioEnable.Header.MessageType, 
                        peripheralAudioEnable.ActiveSpeakerZone);
                }
                else
                {
                    Logger.Info("ACK received for TX of {0}", message.Header.MessageType);
                }

                // signal we got the expected ACK
                model.AckRecievedEvent.Set();
            }
            else if (args.Message.IsNakMessage())
            {
                Debug.WriteLine("NAK received for TX of MessageType " + message.Header.MessageType);
                Logger.Warn("! NAK received for TX of {0}", message.Header.MessageType);
            }
        }

        /// <summary>Open the Serial Port. Close it if already opened.</summary>
        /// <param name="serialPortSettings">Settings to use</param>
        /// <returns>The <see cref="SerialPort" />.</returns>
        private SerialPort Open(SerialPortSettings serialPortSettings)
        {
            if (this.SerialPort != null && this.SerialPort.IsOpen)
            {
                this.Close();
            }

            this.SerialPort = OpenSerialPort(serialPortSettings, this.SerialPortOnDataReceived, this.SerialPortErrorReceived);

            return this.SerialPort;
        }

        /// <summary>Process the data in the state model, removing message from the stream</summary>
        /// <param name="state"></param>
        private void ProcessBufferHandler(PeripheralState state)
        {
            Debug.WriteLine("ProcessBufferHandler() Enter");
            try
            {
                // state.LockStream(); // ???
                object message;
                do
                {
                    // process the buffer/stream pulling out from the start messages and shrink down the steam as we go till no more then back to idle
                    // take the read serial data and add to our buffer to be parsed and reviewed there
                    message = this.RemoveNextMessage(state);
                    if (message != null)
                    {
                        var baseMsg = this.ProcessReceivedMessages(state, message);
                        this.FirePeripheralDataReceived(baseMsg);
                    }
                    else
                    {
                        // data left in buffer ? Part of another message ?
                        try
                        {
                            state.LockStream();
                            var streamLen = state.StreamLength;
                            if (streamLen > 0)
                            {
                                // do we have a partial message? Look for framing byte
                                if (state.IsPartialMessagesInStream == false)
                                {
                                    // no framing byte so empty the buffer
                                    state.EmptyStream();
                                }
                            }

                            if (streamLen != 0 && streamLen < PeripheralHeader.Size)
                            {
                                Logger.Warn(
                                    "Remaining stream buffer bytes {0} is < PeripherailHeader size {1}, exit loop to get receive data", 
                                    state.StreamLength, 
                                    PeripheralHeader.Size);
                                break;
                            }
                        }
                        finally
                        {
                            state.UnLockStream();
                        }
                    }
                }
                while (message != null);
            }
            catch (InvalidDataException exception)
            {
                Logger.Warn("ProcessBufferHandler Exception {0}, Emptying the Stream", exception.Message);
                state.EmptyStream();
            }
            finally
            {
                Debug.WriteLine("ProcessBufferHandler() Exit");

                // state.UnLockStream();
            }
        }

        /// <summary>Process the state buffer turning the bytes into valid know models/messages</summary>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <returns>The <see cref="IPeripheralBaseMessage" />.</returns>
        private IPeripheralBaseMessage ProcessReceivedMessages(PeripheralState state, object message)
        {
            return this.PeripheralHandler?.ProcessReceivedMessages(state, message);
        }

        private void SerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs serialErrorReceived)
        {
            var handler = this.SerialErrorReceived;
            handler?.Invoke(sender, serialErrorReceived);
        }

        /// <summary>Process new received data</summary>
        /// <param name="sender"></param>
        /// <param name="serialDataReceivedEventArgs"></param>
        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            if (serialDataReceivedEventArgs == null || serialDataReceivedEventArgs.EventType != SerialData.Chars)
            {
                Debug.WriteLine("SerialPortOnDataReceived()  Ignored! Not Chars Received " + this.PortName);
                return;
            }

            try
            {
                var bytesToRead = this.SerialPort.BytesToRead;
                Debug.WriteLine("SerialPortOnDataReceived() RX on Serial {0} Bytes to Read = {1}", this.PortName, bytesToRead);

                // signal the reader there we have data to process
                this.serialDataReceivedEvent.Set();
                this.SerialDataReceived?.Invoke(this, serialDataReceivedEventArgs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Error("SerialPortOnDataReceived() Exception {0}!", ex.Message);
            }
        }

        private void SerialReaderThreadHandler(object stateObject)
        {
            Logger.Info("PeripheralSerialClient SerialReaderThreadHandler Enter");
            var state = stateObject as PeripheralState;
            if (state == null || state.Running)
            {
                Logger.Error("SerialReaderThreadHandler() Invalid State");
                return;
            }

            try
            {
                Debug.WriteLine("SerialReaderThreadHandler started");
                state.Running = true;
                var eventHandles = new WaitHandle[] { this.serialDataReceivedEvent, this.stoppedEvent };
                this.readerThreadReady.Set();
                var readTimeout = this.SerialPort.ReadTimeout;

                Logger.Info("PeripheralSerialClient SerialReaderThreadHandler ready & running");
                while (state.Running && this.IsComPortOpen)
                {
                    // hangout waiting for serialDataReceivedEvent event or terminate
                    Logger.Info("*** SerialReaderThreadHandler() Waiting for data ***");
                    var eventIndex = WaitHandle.WaitAny(eventHandles);
                    Logger.Info("<<<< SerialReaderThreadHandler() Event Index = {0} Signaled >>>>", eventIndex);
                    if (eventIndex != 0 || this.SerialPort.IsOpen == false)
                    {
                        break;
                    }

                    // got signaled data ready to read from the serial port
                    var byteList = new List<byte>();
                    var readCount = 0;

                    try
                    {
                        int bytesRead;
                        bool readAgain = false;
                        do
                        {
                            Logger.Info("SerialPort.Read enter ReadTimeout={0}, ReadCount={1}", this.SerialPort.ReadTimeout, ++readCount);

                            bytesRead = this.SerialPort.Read(this.State.SerialBuffer, 0, this.State.SerialBuffer.Length);
                            if (bytesRead > 0)
                            {
                                // take what we received from the buffer
                                var bytesReadBuffer = this.State.SerialBuffer.Take(bytesRead).ToArray();
                                Debug.WriteLine(
                                    "SerialReaderThreadHandler() Adding buffer to MemoryStream, bytes read=" + bytesRead + " buffer length="
                                    + bytesReadBuffer.Length);
                                bytesRead = bytesReadBuffer.Length;

                                byteList.AddRange(bytesReadBuffer);
                                var currentBytesArray = byteList.ToArray();
                                Logger.Info(
                                    "SerialPort.Read completed, bytesRead={0}. currentBytesArray.Length={1}, Finding Frame...", 
                                    bytesRead, 
                                    currentBytesArray.Length);

                                // here we can verify if we read the entire message based on a valid header and it's length
                                var framingPosition = currentBytesArray.FindFramingPosition();
                                try
                                {
                                    // enough data in stream to at least find a header ?
                                    if (framingPosition >= 0 && bytesRead >= PeripheralHeader.Size)
                                    {
                                        // Framing byte found in the stream, position or skip to the byte after this one
                                        var testForHeaderBuffer = currentBytesArray.Skip(framingPosition + 1).ToArray();

                                        var peripheralHeader = this.FindPeripheralHeader(testForHeaderBuffer);
                                        var expectedMessageLength = peripheralHeader?.Length + 1 ?? 0;

                                        // remember extra byte for expected checksum as last byte in message. This should have been in the Length !!!
                                        if (peripheralHeader == null || bytesRead < expectedMessageLength)
                                        {
                                            Logger.Warn(
                                                "Found Header but data in stream is short of expected Length!. ReadAgain. Header Length={0}, bytes Read from Stream={1}", 
                                                expectedMessageLength, 
                                                bytesRead);
                                            readAgain = true;
                                            this.SerialPort.ReadTimeout = 250;
                                            Logger.Info("Read again with RX Timeout = {0}", this.SerialPort.ReadTimeout);
                                            Debug.WriteLine("Reading again from serial port");
                                        }
                                        else
                                        {
                                            readAgain = false;
                                        }
                                    }
                                    else
                                    {
                                        if (currentBytesArray.Length > PeripheralHeader.Size)
                                        {
                                            Logger.Warn("!Incomplete Message in Serial Buffer! Reading again... bytesRead so far = {0}", byteList.Count);
                                        }
                                        this.SerialPort.ReadTimeout = 250;
                                        readAgain = true;
                                    }
                                }
                                catch (InvalidDataException)
                                {
                                    // bad data in stream
                                    Logger.Warn("Invalid Header in Stream, Ignored & Flush Data");
                                }
                                catch (InvalidOperationException ex)
                                {
                                    Logger.Error("Serial Read Exception {0}", ex.Message);
                                    break;
                                }
                            }
                        }
                        while (bytesRead > 0 && readAgain && this.IsComPortOpen);
                    }
                    catch (TimeoutException)
                    {
                        Logger.Warn("SerialReaderThreadHandler Timeout Exception - Serial Buffer empty, exiting reading normally");
                        Debug.WriteLine("SerialReaderThreadHandler Timeout Exception reading " + this.SerialPort.PortName);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Unhandled exception in serial read {0}", ex.Message);
                    }
                    finally
                    {
                        this.SerialPort.ReadTimeout = readTimeout;
                    }

                    // append the data we read to our memory stream to be examined
                    var bytesToProcess = byteList.ToArray();
                    this.State.AppendStream(bytesToProcess);
                    this.streamReadyForProcessing.Set();
                }
            }
            catch (ObjectDisposedException)
            {
                Logger.Debug("ReaderThreadHandler Read thread exiting, Socket disposed");
            }
            catch (EndOfStreamException endOfStreamException)
            {
                Logger.Warn("ReaderThreadHandler Read thread EndOfStreamException {0}", endOfStreamException.Message);
            }
            finally
            {
                Logger.Info("ReaderThreadHandler Exited");
                Debug.WriteLine("ReaderThreadHandler Signal and Exit...");
                this.CloseSerialPort();
                this.endEvent.Set();
                Debug.WriteLine("ReaderThreadHandler() Terminated");
            }
        }

        private Task StartMessageProcessingTask(PeripheralState state)
        {
            var task = Task.Factory.StartNew(
                () =>
                    {
                        var swIdle = new Stopwatch();
                        var handles = new WaitHandle[] { this.streamReadyForProcessing, this.endEvent };
                        Thread.CurrentThread.Name = "MessageProcessingTask";
                        PeripheralState peripheralState = state;
                        while (!this.disposed)
                        {
                            swIdle.Reset();
                            swIdle.Start();
                            if (WaitHandle.WaitAny(handles) != 0)
                            {
                                break;
                            }

                            swIdle.Stop();
                            var swProcessing = new Stopwatch();
                            swProcessing.Start();
                            this.ProcessBufferHandler(peripheralState);
                            swProcessing.Stop();
                            Logger.Info(
                                ">>>>>>>>>>>>>>> ProcessBufferHandler Time {0} ms Idle Time {1} ms", 
                                swProcessing.ElapsedMilliseconds, 
                                swIdle.ElapsedMilliseconds);
                        }

                        Logger.Info("StartMessageProcessingTask Exiting");
                    });
            return task;
        }

        /// <summary>Start background thread reader.</summary>
        /// <param name="state">The state.</param>
        /// <exception cref="ApplicationException">Reader thread already started</exception>
        /// <exception cref="ArgumentNullException"><paramref name="state" /> is null. </exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread. </exception>
        /// <exception cref="ThreadStateException">Thead failed to start.</exception>
        private void StartPeripheralReaderThread(PeripheralState state)
        {
            Logger.Info("PeripheralSerialClient StartBackgroundReader() Enter");
            Debug.WriteLine("Staring BackgroundReader");
            if (this.backgroundReaderThread == null)
            {
                this.backgroundReaderThread = new Thread(this.SerialReaderThreadHandler)
                                                  {
                                                      IsBackground = true, 
                                                      Name = "PeripheralSerialClientThread"
                                                  };

                Logger.Info("PeripheralSerialClient StartBackgroundReader() Start thread");
                try
                {
                    Debug.Assert(state != null, "StartPeripheralReaderThread State = null!");
                    this.backgroundReaderThread.Start(state);

                    // give time for thread to start and wait for initial events before clients first writes
                    var signaledStarted = this.readerThreadReady.WaitOne(3000);
                    if (!signaledStarted)
                    {
                        Logger.Warn("Failed to get signal of started thread, increase timeout or review for possible bug");
                    }
                }
                catch (ThreadStateException threadStateException)
                {
                    Logger.Error(threadStateException.Message);
                    throw;
                }
            }
            else
            {
                throw new ApplicationException("PeripheralSerialClient Reader thread already started");
            }
        }

        /// <summary>Stop background reader.</summary>
        private void StopPeripheralReaderThread()
        {
            Logger.Info("PeripheralSerialClient StopBackgroundReader() Enter");
            if (this.backgroundReaderThread != null)
            {
                if (this.backgroundReaderThread.IsAlive)
                {
                    Debug.WriteLine("PeripheralSerialClient StopBackgroundReader() stopping thread...");
                    this.State.Running = false;
                    Debug.WriteLine("Signal background thread to terminate");
                    this.stoppedEvent.Set();
                    this.backgroundReaderThread.Join(1000);
                    Logger.Info("PeripheralSerialClient StopBackgroundReader() Exit");
                    Debug.WriteLine("StopBackgroundReader() Exit");
                }

                this.backgroundReaderThread = null;
            }
        }

        #endregion
    }
}