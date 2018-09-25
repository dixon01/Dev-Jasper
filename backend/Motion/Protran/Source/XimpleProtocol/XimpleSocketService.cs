// InfomediaAll
// Luminator.Protran.XimpleProtocol
// <author>Kevin Hartman</author>
// $Rev::                                   
namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.XimpleProtocol;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Medi.Core.Services;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Protran.Core;
    using Gorba.Motion.SystemManager.ServiceModel.Messages;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    // ReSharper disable once IdentifierTypo
    /// <summary>The Ximple socket service.</summary>
    public class XimpleSocketService : IService, IDisposable
    {
        #region Constants

        /// <summary>The default port.</summary>
        public const int DefaultPort = 1598;

        private const int DefaultSocketReceiveTimeout = 30000;

        #endregion

        #region Static Fields

        private static readonly Logger Logger = LogHelper.GetLogger<XimpleSocketService>();

        private static readonly string XmlEndXimpleTag = $"</{nameof(Ximple)}>";

        private static SimplePort simplePortNetworkConnection;

        #endregion

        #region Fields

        private readonly Dictionary<string, SocketState> backgroundThreads = new Dictionary<string, SocketState>();

        private Dictionary<string, AudioZoneTypes> audioZonesDictionary;

        private CustomXimpleTableActions customXimpleTableActions;

        private Dictionary<int, Action<XimpleTableActionContext, IEnumerable<XimpleCell>>> customXimpleTableMapDictionary;

        private bool disposed;

        private IPEndPoint endPoint;

        private Socket serverSocket;

        #endregion

        public const string XmlStartTag = XmlHelpers.XmlStartTag;

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="XimpleSocketService" /> class.</summary>
        public XimpleSocketService()
        {
            this.XimpleConfig = new XimpleConfig();
            var dictionaryFile = PathManager.Instance.GetPath(FileType.Config, "dictionary.xml");
            this.DictionaryConfigManager = new ConfigManager<Dictionary> { FileName = dictionaryFile };
            this.CreateAudioZonesDictionary(this.XimpleConfig);
        }

        /// <summary>Initializes a new instance of the <see cref="XimpleSocketService" /> class.</summary>
        /// <param name="ximpleConfig">The ximple config.</param>
        /// <param name="configManager">The config manager.</param>
        public XimpleSocketService(IXimpleConfig ximpleConfig, ConfigManager<Dictionary> configManager)
        {
            this.XimpleConfig = ximpleConfig;
            this.DictionaryConfigManager = configManager;
            this.CreateAudioZonesDictionary(ximpleConfig);
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Event that is fired whenever a medi message of InfoTainment AudioStatusMessage is received and
        ///     a new Ximple response is generated.
        /// </summary>
        public event EventHandler<AudioStatusMessage> AudioStatusMessageChanged;

        /// <summary>The bad xml ximple event.</summary>
        public event EventHandler<BadXimpleEventArgs> BadXimple;

        /// <summary>The volume settings message changed.</summary>
        public event EventHandler<VolumeSettingsMessage> VolumeSettingsMessageChanged;

        /// <summary>
        ///     Event that is fired whenever the this protocol creates
        ///     a new Ximple object.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        #endregion

        #region Public Properties

        /// <summary>Gets the dictionary config manager.</summary>
        public ConfigManager<Dictionary> DictionaryConfigManager { get; private set; }

        /// <summary>Gets or sets a value indicating whether running.</summary>
        public bool Running { get; set; }

        /// <summary>Gets or sets the ximple config used for the server.</summary>
        public IXimpleConfig XimpleConfig { get; set; }

        #endregion

        #region Properties

        private Dictionary Dictionary
        {
            get
            {
                return this.DictionaryConfigManager.Config;
            }
        }

        private bool Stopped { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The board cast a network connection medi message to signal a change in the Wifi network connection.</summary>
        /// <param name="connected">The connected flag true to indicate a connection exists.</param>
        public static void BoardcastNetworkConnectionChanged(bool connected)
        {
            // Broadcast the network connection changed message
            Logger.Info("BoardcastNetworkConnectionChanged connected = {0}, Send Medi message Broadcast NetworkChangedMessage", connected);
            var networkConnectionMessage = new NetworkChangedMessage(connected);

            MessageDispatcher.Instance.Broadcast(networkConnectionMessage);
        }

        /// <summary>Raise a network changed event using Medi and/or the SimplePort - ''</summary>
        /// <param name="connected">True when a network connection exists determined by external equipment.</param>
        public static void RaiseNetworkConnectionChanged(bool connected)
        {
            if (simplePortNetworkConnection != null)
            {
                // send the change to the subscriber to the port
                simplePortNetworkConnection.Value = connected ? FlagValues.True : FlagValues.False;
            }

            BoardcastNetworkConnectionChanged(connected);
        }

        /// <summary>Send a response on the socket</summary>
        /// <param name="socket">The Socket</param>
        /// <param name="response"></param>
        /// <exception cref="SocketException">Socket is invalid, closed.</exception>
        public static void SendResponse(Socket socket, XimpleResponse response)
        {
            if (socket != null && response != null && socket.Connected)
            {
                try
                {
                    using (var writer = new Utf8StringWriter())
                    {
                        var xmlSerializer = new XmlSerializer(typeof(XimpleResponse));
                        xmlSerializer.Serialize(writer, response);
                        Debug.WriteLine("Sending socket  response ", writer.ToString());
                        var buffer = Encoding.UTF8.GetBytes(writer.ToString());
                        socket.Send(buffer);
                    }
                }
                catch (SocketException socketException)
                {
                    Logger.Warn("SendResponse Exception {0}", socketException);
                    throw;
                }
            }
        }

        /// <summary>Send ximple response over the socket.</summary>
        /// <param name="socket">The socket state.</param>
        /// <param name="ximple">The ximple message to send on the socket.</param>
        /// <exception cref="SocketException">Invalid Socket.</exception>
        public static void SendXimpleResponse(Socket socket, Ximple ximple)
        {
            // For 3rdparty we will return Ximple data when called to do so 
            if (socket != null && ximple != null && socket.Connected)
            {
                try
                {
                    using (var writer = new Utf8StringWriter())
                    {
                        var xmlSerializer = new XmlSerializer(typeof(Ximple));
                        xmlSerializer.Serialize(writer, ximple);
                        Logger.Debug("In SendXimpleResponse() server sending a socket Ximple response Xml=[{0}]", writer);
                        var buffer = Encoding.UTF8.GetBytes(writer.ToString());
                        socket.Send(buffer);
                    }
                }
                catch (SocketException socketException)
                {
                    Logger.Warn("SendResponse Exception {0}", socketException);
                    throw;
                }
            }
        }

        /// <summary>The socket receive.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferStartOffset">The buffer start offset.</param>
        /// <param name="receiveTimeoutMilliSeconds">The receive timeout milliseconds.</param>
        /// <returns>The <see cref="int" />.</returns>
        /// <exception cref="ArgumentException">Invalid argument for socket operations</exception>
        /// <exception cref="ArgumentNullException">Invalid null argument - socket</exception>
        /// <exception cref="SocketException">An error occurred when attempting to access the socket.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> is less than 0.-or- <paramref name="offset" />
        ///     is greater than the length of <paramref name="buffer" />.-or- <paramref name="size" /> is less than 0.-or-
        ///     <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the
        ///     <paramref name="offset" /> parameter.
        /// </exception>
        /// <exception cref="SecurityException">A caller in the call stack does not have the required permissions. </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed. </exception>
        /// <exception cref="IndexOutOfRangeException">Invalid bufferStartOffset</exception>
        /// <exception cref="OverflowException">
        ///     The array is multidimensional and contains more than
        ///     <see cref="F:System.Int32.MaxValue" /> elements.
        /// </exception>
        [Obsolete("Future")]
        public static int SocketReceive(
            Socket socket,
            byte[] buffer,
            int bufferStartOffset = 0,
            int receiveTimeoutMilliSeconds = DefaultSocketReceiveTimeout)
        {
            var oldReceiveTimeout = socket.ReceiveTimeout;
            var bufferSize = buffer.Length;
            if (bufferSize < 1)
            {
                throw new ArgumentException("Invalid buffer size", nameof(bufferSize));
            }

            if (bufferStartOffset < 0 || bufferStartOffset >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Invalid bufferStartOffset");
            }

            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket), "Invalid Socket");
            }

            var bytesRead = 0;
            try
            {
                socket.ReceiveTimeout = receiveTimeoutMilliSeconds;
                var toReadSize = buffer.Length - bufferStartOffset;
                bytesRead = socket.Receive(buffer, bufferStartOffset, toReadSize, SocketFlags.None);
            }
            catch (TimeoutException)
            {
                Trace.TraceInformation("SocketReceive() Timeout");
            }
            catch (SocketException socketException)
            {
                // socket timeout is possible here              
                Trace.TraceError("SocketReceive() Exception: " + socketException.Message);
                throw;
            }
            finally
            {
                socket.ReceiveTimeout = oldReceiveTimeout;
            }

            return bytesRead;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///     Close the background  threads and perform runtime cleanup.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                Debug.WriteLine("XimpleSocketService.Dispose()");
                this.disposed = true;
                this.Stop();
            }
        }

        /// <summary>
        /// Raise the Ximple Created event
        /// </summary>
        /// <param name="ximpleMessage">Ximple on XimpleCreated handler</param>
        public void RaiseXimpleCreatedEvent(Ximple ximpleMessage)
        {
            try
            {
                var eventHandler = this.XimpleCreated;
                if (eventHandler != null)
                {
                    Logger.Info("Raising Ximple Created message\r\n[{0}]", ximpleMessage.ToXmlString());
                    Debug.WriteLine("XimpleSocketService - Post Ximple Created Event ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                    eventHandler(this, new XimpleEventArgs(ximpleMessage));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RaiseXimpleCreatedEvent Exception {0}", ex.Message);
            }
        }

        /// <summary>Start the service</summary>
        /// <param name="address">The address.</param>
        /// <param name="config">The config.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"><paramref name="address" /> is null.</exception>
        /// <exception cref="SocketException">
        ///     The combination of <paramref name="address" />, <paramref name="socketType" />, and
        ///     <paramref name="protocolType" /> results in an invalid socket.
        /// </exception>
        /// <exception cref="FileNotFoundException">If the config file is not found</exception>
        /// <exception cref="XmlException">If the file content for config could not be loaded</exception>
        /// <exception cref="ConfiguratorException">If errors occurred while deserializing the file</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed. </exception>
        /// <exception cref="SecurityException">
        ///     A caller higher in the call stack does not have permission for the requested
        ///     operation.
        /// </exception>
        public void Start(IPAddress address, IXimpleConfig config = null)
        {
            if (address == null)
            {
                address = IPAddress.Any;
            }

            if (config == null)
            {
                // take class defaults
                if (this.XimpleConfig == null)
                {
                    this.XimpleConfig = new XimpleConfig();
                }
            }
            else
            {
                this.XimpleConfig = config;
            }

            if (this.XimpleConfig.Port <= 0)
            {
                throw new ArgumentException("Invalid Port");
            }

            this.RegisterSimplePortNetworkConnection();
            this.SubscribeToMediMessages();

            this.Stopped = false;
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Logger.Info("Creating TCP Endpoint {0}:{1}", address, this.XimpleConfig.Port);
            this.endPoint = new IPEndPoint(IPAddress.Any, this.XimpleConfig.Port);

            this.customXimpleTableActions = new CustomXimpleTableActions(this.DictionaryConfigManager);

            // build a map for handling specific tables for received Ximple so we can perform custom actions based on the Table number and it's payload
            this.customXimpleTableMapDictionary = new Dictionary<int, Action<XimpleTableActionContext, IEnumerable<XimpleCell>>>();

            // Network status changes
            this.customXimpleTableMapDictionary.Add(
                this.XimpleConfig.NetworkChangedMessageTableIndex,
                this.customXimpleTableActions.NetworkChangedMessageTable);

            // Volume Settings request
            this.customXimpleTableMapDictionary.Add(
                this.XimpleConfig.InfoTainmentVolumeSettingsTableIndex,
                this.customXimpleTableActions.InfotainmentVolumeSettingsTable);

            // Play a canned Message
            this.customXimpleTableMapDictionary.Add(
                this.XimpleConfig.InfoTainmentCannedMsgPlayTableIndex,
                this.customXimpleTableActions.InfoTainmentCannedMsgPlayTable);

            // Request for Network shared folder settings
            this.customXimpleTableMapDictionary.Add(
                this.XimpleConfig.NetworkFileAccessSettingsTableIndex,
                this.customXimpleTableActions.NetworkFileAccessSettingsTable);

            // Request for Audio Status, send medi request for data
            this.customXimpleTableMapDictionary.Add(
                this.XimpleConfig.InfoTainmentAudioStatusTableIndex,
                this.customXimpleTableActions.InfoTainmentAudioStatusTable);

            // Handle Infotainment System Status ie GPIO, GPS 
            this.customXimpleTableMapDictionary.Add(
                this.XimpleConfig.InfoTainmentSystemStatusTableIndex,
                this.customXimpleTableActions.InfoTainmentSystemStatusTable);

            // Table 10 Route, Trip information is of interest
            this.customXimpleTableMapDictionary.Add(
                Gorba.Common.Configuration.Protran.XimpleProtocol.XimpleConfig.RouteTableIndex,
                this.customXimpleTableActions.InfoTainmentSystemStatusTable);

            this.serverSocket.LingerState.Enabled = false;
            this.serverSocket.Bind(this.endPoint);
            this.serverSocket.Listen(25);

            var readyEvent = new ManualResetEvent(false);
            try
            {
                if (ServiceLocator.Current != null)
                {
                    var hostApp = ServiceLocator.Current.GetInstance<ProtranApplication>();
                    var host = hostApp.Protran.ProtocolHost;
                    if (host != null)
                    {
                        host.ProtocolStarted += (sender, args) =>
                            {
                                if (host.AllProtocolsStarted)
                                {
                                    readyEvent.Set();
                                }
                            };
                    }
                }
                else
                {
                    readyEvent.Set();
                }
            }
            catch
            {
                readyEvent.Set();
            }

            // Delay accepting socket connections until all the protocols have indicated started
            Task.Factory.StartNew(
                () =>
                    {
                        var signaled = readyEvent.WaitOne(30000);
                        if (!signaled)
                        {
                            Logger.Warn("Failed to get All Protocols started, continue...");
                        }
                        // Add the other custom ximple table actions here as needed
                        Logger.Info(
                            "BeginAccept accepting new TCP Connections Endpoint {0}:{1}, EnableResponse={2}, signaled={3}",
                            IPAddress.Any,
                            this.XimpleConfig.Port,
                            this.XimpleConfig.EnableResponse,
                            signaled);
                        this.serverSocket.BeginAccept(this.Accept, this.serverSocket);
                    });

            this.Running = true;
        }

        /// <summary>Start the TCP listening accepting connections.</summary>
        /// <summary>Stop the Service from accepting socket connection, close all listeners and close the socket.</summary>
        public void Stop()
        {
            if (!this.Stopped)
            {
                this.Stopped = true;
                this.Running = false;
                Logger.Info("Stopping Ximple Socket Service");
                this.CloseAllSocketConnections();
                this.WaitThreadHandlersExited();
                SafeSocketClose(this.serverSocket);
                this.serverSocket = null;
                this.DeRegisterNetworkConnectedSimplePort();
                this.UnSubscribeFromMediMessages();
            }
        }

        #endregion

        #region Methods

        /// <summary>The create ximple for initial response.</summary>
        /// <returns>The <see cref="Ximple" />.</returns>
        internal static Ximple CreateXimpleForInitialResponse()
        {
            var ximple = new Ximple(Constants.Version2);
            return ximple;
        }

        /// <summary>Safely close a socket, null check and exception handling</summary>
        /// <param name="socket"></param>
        private static void SafeSocketClose(Socket socket)
        {
            try
            {
                if (socket != null)
                {
                    Debug.WriteLine("SafeSocketClose " + socket.LocalEndPoint);
                    Logger.Info("SafeSocketClose() Socket Closing EndPoint...{0}", socket.LocalEndPoint);

                    // disable send and receives
                    if (socket.IsBound && socket.Connected)
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException socketException)
                        {
                            Logger.Warn(socketException.Message);
                        }
                    }

                    // close the socket and release any associated resources
                    Debug.WriteLine("Closing Socket " + socket.LocalEndPoint);
                    socket.Close();
                }
            }
            catch (ObjectDisposedException)
            {
                Logger.Debug("SafeSocketClose() Socket object disposed");
            }
        }

        /// <summary>Async Accept new Connection callback</summary>
        /// <param name="ar">IAsyncResult</param>
        private void Accept(IAsyncResult ar)
        {
            try
            {
                Debug.WriteLine("XimpleSockerService Accepting new connection");
                Logger.Info("*** Accepting new Socket Connection... ***");
                if (this.serverSocket == null || this.Running == false)
                {
                    Logger.Warn("XimpleSockerService Ignored new Socket Connection, Running == false");
                    return;
                }

                var connection = ar.AsyncState as Socket;

                // accept in-coming connection an begin the next possible acceptance
                if (connection != null)
                {
                    var clientSocket = connection.EndAccept(ar);
                    Logger.Warn("XimpleSockerService Accepting new Socket from endpoint...{0}", clientSocket.LocalEndPoint);

                    this.serverSocket.BeginAccept(this.Accept, this.serverSocket);
                    if (clientSocket != null && this.Running)
                    {
                        // start new background thread to process the received Ximple xml                  
                        Logger.Info("XimpleSockerService Accepted new Socket connection... {0}", clientSocket.LocalEndPoint);

                        lock (this.backgroundThreads)
                        {
                            var thread = new Thread(this.XimpleSocketThreadHandler) { IsBackground = true, Name = "XimpleClient-" + clientSocket.LocalEndPoint };
                            var state = new SocketState(Guid.NewGuid(), thread, clientSocket);

                            // add to the state to collection and start
                            Logger.Info(">>>> XimpleSockerService Started background reader thread {0} <<<<", thread.ManagedThreadId);
                            this.SocketStateAdd(state);
                            thread.Start(state);
                        }
                    }
                }
                else
                {
                    Logger.Error("Accept new Connection Failed. AsyncState is Invalid!");
                }
            }
            catch (SocketException ex)
            {
                Logger.Warn("XimpleSockerService Couldn't accept client connection {0}", ex.Message);
            }
        }

        /// <summary>
        ///     Close all the accepted sockets if any. Set the running flag for each background thread to terminate
        /// </summary>
        private void CloseAllSocketConnections()
        {
            Debug.WriteLine("CloseAllSocketConnections()");
            Logger.Info("CloseAllSocketConnections() Enter");
            lock (this.backgroundThreads)
            {
                if (this.backgroundThreads == null || this.backgroundThreads.Count <= 0)
                {
                    return;
                }

                foreach (var socketState in this.backgroundThreads.Values)
                {
                    socketState.Running = false;
                    var s = socketState.Socket;
                    Logger.Info("Signal Thread to terminate for endpoint {0}, {1}", s?.LocalEndPoint, socketState.Key);
                    socketState.TerminateEvent.Set();

                    // Thread will close on it's exit SafeSocketClose(s);
                }
            }
        }

        private void CreateAudioZonesDictionary(IXimpleConfig ximpleConfig)
        {
            this.audioZonesDictionary = new Dictionary<string, AudioZoneTypes>
                                            {
                                                {
                                                    ximpleConfig.AudioZonePresentationValues.Interior,
                                                    AudioZoneTypes.Interior
                                                },
                                                {
                                                    ximpleConfig.AudioZonePresentationValues.Exterior,
                                                    AudioZoneTypes.Exterior
                                                },
                                                {
                                                    ximpleConfig.AudioZonePresentationValues.Both,
                                                    AudioZoneTypes.Both
                                                },
                                                {
                                                    ximpleConfig.AudioZonePresentationValues.None,
                                                    AudioZoneTypes.None
                                                }
                                            };
        }

        private void DeRegisterNetworkConnectedSimplePort()
        {
            if (simplePortNetworkConnection != null)
            {
                GioomClient.Instance.DeregisterPort(simplePortNetworkConnection);
            }
        }

        /// <summary>
        ///     Get the Audio Zone from key string value
        /// </summary>
        /// <param name="keyString">Input string</param>
        /// <returns></returns>
        private AudioZoneTypes GetInfotainmentAudioZoneType(string keyString)
        {
            // known enum for our audio zone ?
            var audioZone = AudioZoneTypes.None;
            if (Enum.GetNames(typeof(AudioZoneTypes)).Contains(keyString))
            {
                audioZone = (AudioZoneTypes)Enum.Parse(typeof(AudioZoneTypes), keyString);
            }
            else
            {
                // try our lookup
                if (this.audioZonesDictionary.ContainsKey(keyString))
                {
                    audioZone = this.audioZonesDictionary[keyString];
                }
            }

            Logger.Info("GetInfotainmentAudioZoneType finding value {0}, AudioZoneTypes={1}", keyString, audioZone);
            return audioZone;
        }

        private void OnAliveRequestHandler(object sender, MessageEventArgs<AliveRequest> e)
        {
            Logger.Info("XimpleSocketService Requested are you Alive");
            var appId = e.Message.ApplicationId;

            // TBD

            //if (e.Message.ApplicationId != this.ApplicaitonId)
            //{
            //    return;
            //}

            // var args = new CancelEventArgs(false);
            //// this.RaiseWatchdogKicked(args);

            // if (args.Cancel)
            // {
            // return;
            // }

            // MessageDispatcher.Instance.Send(e.Source, new AliveResponse { ApplicationId = this.ApplicaitonId, RequestId = e.Message.RequestId });
        }

        private void OnAudioStatusMessageHandler(object sender, MessageEventArgs<AudioStatusMessage> messageEventArgs)
        {
            Logger.Info("OnAudioStatusMessageHandler Enter");
            if (messageEventArgs == null || messageEventArgs.Message == null)
            {
                return;
            }

            // When we get a change in settings we need to reply to the connected equipment via sockets
            Task.Run(
                () =>
                    {
                        var audioStatus = messageEventArgs.Message;

                        // fire event so subscriber and reply to all connected clients
                        lock (this.backgroundThreads)
                        {
                            if (this.backgroundThreads.Count > 0)
                            {
                                Logger.Info("Received new OnAudioStatusMessageHandler Medi Message backgroundThreads={0}", this.backgroundThreads.Count);

                                foreach (var clientThread in this.backgroundThreads)
                                {
                                    var socketState = clientThread.Value;
                                    var clientSocket = socketState.Socket;
                                    if (clientSocket != null && socketState.IsConnected)
                                    {
                                        CustomXimpleTableActions.SendXimpleAudioStatusResponse(clientSocket, this.Dictionary, audioStatus);
                                    }
                                    else
                                    {
                                        Logger.Info("Received Medi InfoTainmentAudioStatusMessage but Socket is Closed");
                                    }
                                }
                            }
                        }

                        var eventhandle = this.AudioStatusMessageChanged;
                        if (eventhandle != null)
                        {
                            Logger.Info("Fire OnAudioStatusMessageHandler Event");
                            eventhandle.Invoke(this, audioStatus);
                        }
                    });
        }

        /// <summary>
        ///     Handle GPIO State changes sent as Medi, create a Ximple message to send to the connected socket Ximple client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGpioStateChangedHandler(object sender, MessageEventArgs<GpioStateChanged> args)
        {
            Logger.Debug("{0} Enter", nameof(this.OnGpioStateChangedHandler));
            var gpioStateChanged = args?.Message;

            if (gpioStateChanged != null && args.Message.GppioStates.Any())
            {
                // break down as Ximple and send back to the connected client(s) as Ximple message
                // Send via Ximple to the connect client the settings
                Task.Run(() =>
                        {
                            lock (this.backgroundThreads)
                            {
                                Logger.Info("Total socket client backgroundThreads={0}", this.backgroundThreads.Count);

                                // use the Infotransite status table to return our GPIO state changed message as Ximple
                                if (this.backgroundThreads.Count > 0)
                                {
                                    var table = this.Dictionary.FindXimpleTable(this.XimpleConfig.InfoTainmentSystemStatusTableIndex);
                                    if (table != null)
                                    {
                                        var ximple = new Ximple(Constants.Version2);
                                        Logger.Info("Received GpioStateChanged {0}", gpioStateChanged);

                                        foreach (var gpioState in gpioStateChanged.GppioStates)
                                        {
                                            try
                                            {
                                                // our ximple data is the column name matches the Gpio name 1 = true, else 0 for false
                                                ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, gpioState.GipoName, gpioState.Active ? "1" : "0"));
                                            }
                                            catch (ArgumentException ex)
                                            {
                                                // GPIO name not matching the column name in the dictionary.xml. We make them match today.
                                                Logger.Warn(ex.Message);
                                            }
                                        }
                                        
                                        foreach (var clientThread in this.backgroundThreads)
                                        {
                                            var socketState = clientThread.Value;
                                            var clientSocket = socketState?.Socket;
                                            if (clientSocket == null)
                                            {
                                                continue;
                                            }

                                            if (socketState.IsConnected)
                                            {
                                                //Logger.Info("Sending Infotransite Ximple GPIO State changed to Socket client...{0}", clientSocket.LocalEndPoint);
                                                SendXimpleResponse(clientSocket, ximple);
                                                XimpleSocketServiceLogger.LogGpioChanged(string.Format("ID={0}, IP={1}", socketState.Key,  clientSocket.LocalEndPoint.ToString()), gpioStateChanged);
                                            }
                                            else
                                            {
                                                Logger.Info("Received Medi GpioStateChanged but Socket is Closed, No Ximple sent");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.Warn("Ignored sending Ximple InfoTainmentSystemStatusTable(GPIO Changes), No Socket Clients connected");
                                }
                            }
                        });
            }
        }     

        private void OnVolumeSettingsHandler(object sender, MessageEventArgs<VolumeSettingsMessage> messageEventArgs)
        {
            Logger.Info("{0} Enter", nameof(this.OnVolumeSettingsHandler));
            if (messageEventArgs == null || messageEventArgs.Message == null)
            {
                return;
            }

            if (messageEventArgs.Message.MessageAction != MessageActions.OK)
            {
                Logger.Info("{0} Ignored action {1}", nameof(this.OnVolumeSettingsHandler), messageEventArgs.Message.MessageAction);
                return;
            }

            // Send via Ximple to the connect client the settings
            Task.Run(() =>
                    {
                        // fire event so subscriber and reply to all connected clients
                        var volumeSettingsMessage = messageEventArgs.Message;
                        lock (this.backgroundThreads)
                        {
                            Logger.Info(
                                "Received VolumeSettingsMessage {0}. Total of {1} Socket Client to receive VolumeSettings Message",
                                volumeSettingsMessage,
                                this.backgroundThreads.Count);

                            foreach (var clientThread in this.backgroundThreads)
                            {
                                var socketState = clientThread.Value;
                                var clientSocket = socketState.Socket;
                                if (clientSocket != null && socketState.IsConnected)
                                {
                                    CustomXimpleTableActions.SendXimpleVolumeSettingsResponse(clientSocket, this.Dictionary, volumeSettingsMessage);
                                }
                                else
                                {
                                    Logger.Info("Received Medi InfoTainmentAudioStatusMessage but Socket is Closed, no Ximple sent.");
                                }
                            }
                        }

                        var eventhandle = this.VolumeSettingsMessageChanged;
                        eventhandle?.Invoke(this, volumeSettingsMessage);
                    });
        }

        private string FixMissingLanguage(string xml)
        {
            if (xml.Contains("Language=\"\""))
            {
                Logger.Warn("!Ximple had missing missing Language='' Index={0}", xml.IndexOf(".Language=\"\"", StringComparison.Ordinal));
                xml = xml.Replace("Language=\"\"", "Language=\"0\"");
            }

            return xml;
        }

        /// <summary>Process the Socket state, attempt to parse the incoming data read to Ximple</summary>
        /// <param name="state"></param>
        /// <returns>The <see cref="List{T}" />Collection of Ximple</returns>
        private List<Ximple> Process(SocketState state)
        {
            var ximpleMessages = new List<Ximple>();

            Debug.WriteLine("Processing new Data...");
            if (state == null || state.Socket == null)
            {
                Logger.Error("Processing... Aborted invalid State");
                return ximpleMessages;
            }

            // process while we can find valid Xml Documents in our string buffer, keep the Ximple documents and ignore the rest.
            // first pass check, is the entire buffer a single Xml document 
            string xml;
            lock (state.StringBuffer)
            {
                xml = state.StringBuffer.ToString();
                var length = state.StringBuffer.Length;

                if (length <= 0)
                {
                    // we have no data.
                    Logger.Warn("Ignored process empty string from EndPoint {0} String Length=0", state.Socket.LocalEndPoint);
                    Debug.WriteLine("Processing No new Data");
                    return ximpleMessages;
                }

                Logger.Info("Processing new data for EndPoint {0} String Length={1}", state.Socket.LocalEndPoint, length);
                Debug.WriteLine("Processing  Data string builder Length = " + length);
            }

            // first just a simple test to see if the entire string is a complete xml doc
            Logger.Info("Process(state) Testing for valid XM Documents...");

            // Trap invalid XML missing the Language and fix it
            var originalXml = xml.Clone().ToString();
            xml = this.FixMissingLanguage(xml);

            int xmlDocumentsCount;
            var validXimple = XmlHelpers.IsValidXmlDocument(xml, out xmlDocumentsCount);
            var ximpleCreated = 0;
            if (validXimple)
            {
                try
                {
                    Logger.Info("Yup buffer is single Xml Ximple Doc xmlDocumentsCount={0}", xmlDocumentsCount);
                    var ximple = XmlHelpers.ToXimple(xml);
                    if (ximple != null)
                    {
                        Debug.WriteLine("Processing Adding new Ximple to collection");
                        Logger.Trace(xml);
                        ximpleMessages.Add(ximple);

                        // clear our string buffer
                        state.ClearBuffer(true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Exception Parsing Ximple xml {0}", ex.Message);
                    state.ClearBuffer(true);
                    this.RaiseBadXimpleEvent(xml);
                }
            }
            else
            {
                // handle partial xml or multiple xml Ximple documents back to back
                var startIdx = xml.IndexOf(XmlStartTag, StringComparison.Ordinal);
                var endIdx = startIdx != -1
                                 ? xml.Substring(startIdx).IndexOf(XmlEndXimpleTag, StringComparison.Ordinal)
                                 : xml.IndexOf(XmlEndXimpleTag, StringComparison.Ordinal);

                // did we find start and end of the xml doc ?
                if (startIdx < 0 && endIdx < 0)
                {
                    // No XML Tags, bogus data or possible middle part of the XML Doc, we are disregarding it
                    Logger.Warn(
                        "Bogus string data clearing buffer, failed to find start and end of Ximple XML. startIdx={0}, endIdx={1}",
                        startIdx,
                        endIdx);
                    state.ClearBuffer();
                    this.RaiseBadXimpleEvent(xml);
                    if (this.XimpleConfig.EnableResponse)
                    {
                        SendResponse(state.Socket, new XimpleResponse(XimpleResonseType.InvalidXimpleXml, xml));
                    }
                }
                else if (startIdx >= 0 && endIdx > startIdx && endIdx + XmlEndXimpleTag.Length < xml.Length)
                {
                    endIdx += XmlEndXimpleTag.Length;
                    lock (state.StringBuffer)
                    {
                        Ximple ximple;
                        do
                        {
                            // de-serialize the XML to Ximple till we can't
                            var xmlFragmet = xml.Substring(startIdx, endIdx);
                            try
                            {
                                ximple = XmlHelpers.ToXimple(xmlFragmet);
                            }
                            catch (InvalidOperationException)
                            {
                                ximple = null;
                                Logger.Warn("Ignored Bad Ximple message, invalid XML [{0}]", xmlFragmet);
                                this.RaiseBadXimpleEvent(xml);
                            }

                            if (ximple != null)
                            {
                                // successful creation add to the collection and remove the string for the buffer
                                ximpleCreated++;
                                Logger.Info("Success {0} of {1} found\nXimple=[{2}]", ximpleCreated, xmlDocumentsCount, xmlFragmet);
                                ximpleMessages.Add(ximple);
                            }
                           
                            state.StringBuffer.Remove(0, endIdx);
                            xml = state.StringBuffer.ToString();
                            Debug.WriteLine("Process()  New string builder Length = " + xml.Length);

                            startIdx = xml.IndexOf(XmlStartTag, StringComparison.Ordinal);
                            endIdx = startIdx != -1
                                         ? xml.Substring(startIdx).IndexOf(XmlEndXimpleTag, StringComparison.Ordinal)
                                         : xml.IndexOf(XmlEndXimpleTag, StringComparison.Ordinal);
                            if (endIdx > 0 && endIdx + XmlEndXimpleTag.Length <= xml.Length)
                            {
                                endIdx += XmlEndXimpleTag.Length;
                            }
                        }
                        while (ximple != null && startIdx >= 0 && endIdx > startIdx);
                    }

                    // do we have a partial start of xml ?
                    if (startIdx > 0 && endIdx == -1)
                    {
                        Debug.WriteLine("! Partial XML Found " + xml);

                        // decide to keep it or throw it away, Don't clear the buffer in other words if we are given
                        // back to back xml in contiguous receptions/reads
                        Logger.Warn("*** Partial XML with beginning tag but no end found in buffer, Ximple was split in TCP transmission!");
                    }
                    else
                    {
                        // clear string buffer
                        if (xmlDocumentsCount != ximpleMessages.Count)
                        {
                            Logger.Warn("Expected XML Documents = {0}, Found valid Ximple XML count = {1}", xmlDocumentsCount, ximpleMessages.Count);
                        }

                        Logger.Info("Clearing String buffer, startIdx={0}, endIdx={1}", startIdx, endIdx);
                        state.ClearBuffer();
                    }
                }
            }

            Debug.WriteLine("Process Exit Total Ximple message = " + ximpleMessages.Count);
            Logger.Info("Ximple message processing...Exit Total Ximple message Count={0}", ximpleMessages.Count);
            return ximpleMessages;
        }

        /// <summary>Process a collection of Ximple message and raise the event XimpleCreated to subscribers</summary>
        /// <param name="socketState">The socket State.</param>
        /// <param name="ximpleMessages">collection of Ximple entities to process, if empty ignored</param>
        private void ProcessNewXimpleMessages(SocketState socketState, List<Ximple> ximpleMessages)
        {
            try
            {
                var totalXimpleMessages = ximpleMessages?.Count ?? 0;
                Logger.Info(
                    "PostXimpleMessagesCreated Enter, Total ximpleMessages = {0}, Endpoint {1}",
                    totalXimpleMessages,
                    socketState.Socket.LocalEndPoint);
                if (ximpleMessages != null && totalXimpleMessages > 0)
                {
                    Logger.Info("Posting Ximple Created Events Total Ximple Messages = {0}", totalXimpleMessages);

                    // normally we have one message to work with
                    foreach (var ximpleMessage in ximpleMessages)
                    {
                        // Bug-feature to support playing the same Canned audio media file back to back. Update the raw Ximple data as necessary
                        // function to inspect the table for canned messages and changed the id value
                        var ximple = this.UpdateXimpleForCannedPlayback(ximpleMessage);

                        // signal the world via Medi we have a new created Ximple message
                        this.RaiseXimpleCreatedEvent(ximple);

                        // find if the Ximple is for the specific table(s)
                        // Some Ximple request will be pushing data through system as normal, others
                        // will require a Ximple response back where the client is asking for data(Ximple)
                        // For our custom tables determine if the received Ximple xml applies here
                        var tableNumbers = this.customXimpleTableMapDictionary.Keys.ToList();
                        if (tableNumbers.Any())
                        {
                            // Does the Ximple we got apply to our custom table numbers, if so process it
                            // Note some Ximple message may contain no Values, we key off the TableNumber in this case.
                            var ximpleCells = ximpleMessage.Cells.Where(m => tableNumbers.Contains(m.TableNumber)).ToList();
                            if (ximpleCells.Any())
                            {
                                // create a new context for the operation, share the socket state case it needs to reply and the ximple cells for the tables
                                var ximpleTableActionContext = new XimpleTableActionContext(socketState, ximpleCells);
                                this.customXimpleTableActions.InvokeTableActions(ximpleTableActionContext, this.customXimpleTableMapDictionary);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception ProcessNewXimpleMessages() {0}", ex.Message);
            }
        }

        /// <summary>Raise Bad Ximple event to subscribers</summary>
        /// <param name="xml"></param>
        private void RaiseBadXimpleEvent(string xml)
        {
            Task.Run(()=>
                    {
                        var eventHandler = this.BadXimple;
                        if (eventHandler != null && xml != null)
                        {
                            Logger.Warn("Posting PostBadXimpleEvent");
                            Debug.WriteLine("Post Bad Xml Event");
                            eventHandler(this, new BadXimpleEventArgs(xml));
                        }
                    });
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as SocketState;
            Debug.WriteLine("Enter ReceiveCallback  - " + DateTime.Now.ToLongTimeString());

            try
            {
                if (state == null || state.Running == false)
                {
                    Logger.Warn("OOps Note ReceiveCallback Ready to read state=={0} or state.Running == {1}", state, state?.Running);
                    Debug.WriteLine("Exit ReceiveCallback Not Running");
                    return;
                }

                SocketError socketError;
                var bytesRead = state.Socket.EndReceive(ar, out socketError);

                if (bytesRead > 0)
                {
                    Logger.Info("ReceiveCallback Bytes Read = {0}, socketError = {1}", bytesRead, socketError);
                    lock (state.StringBuffer)
                    {
                        var xmlRead = Encoding.UTF8.GetString(state.Buffer, 0, bytesRead);
                        state.StringBuffer.Append(xmlRead);
                        Debug.WriteLine("Appending string to String Builder... Buffer Length = " + state.StringBuffer.Length);
                    }

                    // Next Attempt to process what we have in the string builder buffer
                    Debug.WriteLine("ReceiveCallback calling BeginReceive");

                    // attempt to read more               
                    state.BytesReceivedEvent.Set();
                    try
                    {
                        // socket could close any time
                        if (state.Socket.Connected)
                        {
                            state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, this.ReceiveCallback, state);
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    Debug.WriteLine("No Bytes Received from EndReceive " + DateTime.Now.ToLongTimeString());
                    var connected = state.IsConnected;
                    var logLevel = state.IsLoopbackEndpoint ? LogLevel.Info : LogLevel.Warn;
                    Logger.Log(logLevel,
                        "No Data received on Read from Client:{0} bytesRead={1}, testing if still IsConnected={2}, socketError={3}",
                        state.Socket,
                        bytesRead,
                        connected,
                        socketError);

                    // if we are disconnected signal to release the waiting main thread for this connection
                    if (connected == false)
                    {
                        state.TerminateEvent.Set();
                    }
                }
            }
            finally
            {
                Debug.WriteLine("Exit ReceiveCallback - " + DateTime.Now.ToLongTimeString());
            }
        }

        private void RegisterSimplePortNetworkConnection()
        {
            try
            {
                simplePortNetworkConnection = XimpleProtocolFactory.CreateNetworkConnectionSimplePort();
                GioomClient.Instance.RegisterPort(simplePortNetworkConnection);
            }
            catch (ArgumentException ex)
            {
                Logger.Warn(ex, "RegisterSimplePortNetworkConnection() Ignored");
            }
        }

        private void SendInitialConnectionResponse(SocketState state)
        {
            if (this.XimpleConfig.EnableResponse && state != null && state.IsConnected)
            {
                // TODO if necessary for 3rd party. As of now we don't need this function
                // The plant was to return a Ximple message(s) on the initial Socket connection
            }
        }

        private void SocketStateAdd(SocketState state)
        {
            lock (this.backgroundThreads)
            {
                this.backgroundThreads.Add(state.Key, state);
            }
        }

        private void SocketStateRemove(SocketState state)
        {
            lock (this.backgroundThreads)
            {
                if (this.backgroundThreads.ContainsKey(state.Key))
                {
                    this.backgroundThreads.Remove(state.Key);
                }
            }
        }

        /// <summary>
        ///     Subscribe to the Medi message we handle
        /// </summary>
        private void SubscribeToMediMessages()
        {
            MessageDispatcher.Instance.Subscribe<AudioStatusMessage>(this.OnAudioStatusMessageHandler);
            MessageDispatcher.Instance.Subscribe<VolumeSettingsMessage>(this.OnVolumeSettingsHandler);
            MessageDispatcher.Instance.Subscribe<GpioStateChanged>(this.OnGpioStateChangedHandler);

            MessageDispatcher.Instance.Subscribe<AliveRequest>(this.OnAliveRequestHandler);
        }

        /// <summary>Background thread handler for processing socket</summary>
        /// <param name="stateObject">SocketState</param>
        /// <see>SocketState</see>
        private void XimpleSocketThreadHandler(object stateObject)
        {
            var state = stateObject as SocketState;
            var socket = state?.Socket;
            if (state == null)
            {
                Logger.Error("ThreadHandler state==null");
                return;
            }

            if (socket == null)
            {
                Logger.Error("ThreadHandler socket == null, aborting!");
                return;
            }

            var clientEndpoint = socket.LocalEndPoint.ToString();
            try
            {
                state.Running = true;

                // on initial Connection we will reply to the client 
                this.SendInitialConnectionResponse(state);

                // warn log level for easier visualization
                Logger.Info("Thread Handler {0} created to receive Ximple Socket Connection {1}", Thread.CurrentThread.Name, clientEndpoint);
                SocketError errorCode;

                socket.ReceiveTimeout = Timeout.Infinite;
                socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, out errorCode, this.ReceiveCallback, state);

                // stay in the thread till Stop() or some Socket exception terminates processing
                var waitHandles = new WaitHandle[] { state.BytesReceivedEvent, state.TerminateEvent };
                while (this.Running)
                {
                    if (!state.IsConnected)
                    {
                        var logLevel = state.IsLoopbackEndpoint ? LogLevel.Info : LogLevel.Warn;
                        Logger.Log(logLevel,
                            "{0} Socket endpoint {1} is NOT Connected, Existing read thread",
                            clientEndpoint,
                            state.IsLoopbackEndpoint ? "TEST Client" : string.Empty);
                        break;
                    }

                    // Block till we have arrival data or exit. Read the incoming Ximple, add to the Que to be sent to Composer
                    // All Ximple object created from the de-serialized XML will be sent as an event to subscribers. From there they can dispatch further
                    try
                    {
                        Debug.WriteLine("ThreadHandler waiting for BytesReceivedEvent... " + DateTime.Now.ToLongTimeString());
                        Logger.Info("Waiting for new data from {0}", clientEndpoint);
                        var index = WaitHandle.WaitAny(waitHandles, 10000);
                        if (index == 1)
                        {
                            // terminated event
                            Logger.Info("Termination event received, thread exiting....");
                            break;
                        }

                        if (index == 0)
                        {
                            // bytes received event
                            // Process what we have received
                            state.BytesReceivedEvent.Reset();
                            Debug.WriteLine("New Data received, processing BytesReceivedEvent errorCode=" + errorCode);

                            // get back one or more Ximple entities and notify if so
                            List<Ximple> ximpleMessages;
                            lock (state.StringBuffer)
                            {
                                ximpleMessages = this.Process(state);
                            }

                            if (ximpleMessages != null && ximpleMessages.Count > 0)
                            {
                                if (this.XimpleConfig.EnableResponse)
                                {
                                    Debug.WriteLine("Responding Success to client...");
                                    Logger.Debug("Responding Success to client...LocalEndPoint={0}", clientEndpoint);
                                    SendResponse(state.Socket, new XimpleResponse(XimpleResonseType.Success));
                                }

                                Debug.WriteLine("Server Raise Ximple Created and process custom tables");
                                this.ProcessNewXimpleMessages(state, ximpleMessages);
                            }
                        }
                        else
                        {
                            Logger.Debug("Normal Timeout waiting for data LocalEndPoint={0}", clientEndpoint);
                            Debug.WriteLine("BytesReceivedEvent Timeout check our status");
                        }
                    }
                    catch (TimeoutException)
                    {
                        // Logger.Debug("ThreadHandler SocketReceive() Timeout");
                        // should not see this with Timeout Infinite
                    }
                    catch (SocketException ex)
                    {
                        Debug.WriteLine("Socket Exception " + ex.Message);
                        Logger.Info("Socket Exception ThreadHandler {0}, LocalEndPoint={1}", ex.Message, clientEndpoint);
                        if (ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            Logger.Trace("ThreadHandler Socket Timeout no Data received");
                        }
                        else
                        {
                            throw;
                        }

                        break;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Logger.Debug("ThreadHandler Read thread exiting, Socket disposed {0}", clientEndpoint);
            }
            catch (SocketException socketException)
            {
                if (this.Running)
                {
                    Logger.Error("ThreadHandler Read thread Socket Exception {0}", socketException.Message);
                }
            }
            finally
            {
                Logger.Info("ThreadHandler Exiting for LocalEndPoint={0}, Disconnected", clientEndpoint);
                Debug.WriteLine("TheadHandler Signal and Exit... " + clientEndpoint);
                state.Running = false;
                state.SignaledExited.Set();
                SafeSocketClose(state.Socket);
                this.SocketStateRemove(state);
            }
        }

        /// <summary>
        ///     UnSubscribe to cleanup medi subscriptions
        /// </summary>
        private void UnSubscribeFromMediMessages()
        {
            MessageDispatcher.Instance.Unsubscribe<AudioStatusMessage>(this.OnAudioStatusMessageHandler);
            MessageDispatcher.Instance.Unsubscribe<VolumeSettingsMessage>(this.OnVolumeSettingsHandler);
            MessageDispatcher.Instance.Unsubscribe<GpioStateChanged>(this.OnGpioStateChangedHandler);

            MessageDispatcher.Instance.Unsubscribe<AliveRequest>(this.OnAliveRequestHandler);
        }

        /// <summary>Checks a ximple message to see if it is a canned messaged and updates the ID cell to allow repeated playback</summary>
        /// <param name="ximple">The ximple message being processed</param>
        /// <returns>The <see cref="Ximple" />.</returns>
        private Ximple UpdateXimpleForCannedPlayback(Ximple ximple)
        {
            var dictionary = this.Dictionary;
            var table =
                dictionary.FindXimpleTable(Gorba.Common.Configuration.Protran.XimpleProtocol.XimpleConfig.InfoTainmentCannedMsgPlayTableIndexDefault);

            try
            {
                var cells = ximple.Cells;
                var ximpleCells = cells as IList<XimpleCell> ?? cells.ToList();
                if (table != null && (cells != null && ximpleCells.Any()))
                {
                    var cannedMessageId = ximpleCells.FindFirstXimpleCellValue(table, "CannedMessageID");
                    if (string.IsNullOrEmpty(cannedMessageId) == false)
                    {
                        var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                        var secondsSinceEpoch = (int)t.TotalSeconds;
                        ximpleCells.SetXimpleCellValue(table, "CannedMessageID", secondsSinceEpoch);
                    }

                    // set the Audio Zone to None, Interior, Exterior, Both
                    var cannedMessageZone = ximpleCells.FindFirstXimpleCellValue(table, "CannedMessageZone");
                    var audioZoneType = this.GetInfotainmentAudioZoneType(cannedMessageZone);
                    if (audioZoneType != AudioZoneTypes.None)
                    {
                        ximpleCells.SetXimpleCellValue(table, "CannedMessageZone", audioZoneType);
                    }

                    return ximple;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("InfotainmentCannedMsgIdUpdate Failed {0}", ex.Message);
            }

            return ximple;
        }

        /// <summary>Wait for background threads to terminate</summary>
        /// <param name="timeout">Timeout</param>
        private void WaitThreadHandlersExited(int timeout = 10000)
        {
            // Get list of running threads and wait from them to terminate gracefully
            var manualResetEvents = this.backgroundThreads.Values.Select(state => state.SignaledExited).ToList();
            var count = manualResetEvents.Count;
            if (count > 0)
            {
                Debug.WriteLine("WaitThreadHandlersExited, Threads to Exit = " + count);
                Logger.Info("WaitThreadHandlersExited() timeout={0}", timeout);

                WaitHandle.WaitAll(manualResetEvents.ToArray(), timeout);
            }
        }

        #endregion
    }

    internal static class XimpleSocketServiceLogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static void LogGpioChanged(string clientEndpoint, GpioStateChanged gpioStateChanged)
        {
            Logger.Info("GPIO = {0} sent to Ximple client {0} {1}", gpioStateChanged.ToString(), clientEndpoint);
        }
    }

    /// <summary>The utf 8 string writer.</summary>
    public class Utf8StringWriter : StringWriter
    {
        #region Public Properties

        /// <summary>The encoding.</summary>
        public override Encoding Encoding => Encoding.UTF8;

        #endregion
    }
}