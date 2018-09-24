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
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.XimpleProtocol;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core;
    using Gorba.Motion.Protran.Core.Protocols;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    ///     The implementation of the Gorba Ximple protocol over sockets
    /// </summary>
    public class XimpleProtocolImpl : IProtocol, IManageableObject
    {
        #region Constants

        /// <summary>The class name.</summary>
        /// <summary>
        ///     The name of the configuration file.
        ///     Attention (this is not the absolute file's name).
        /// </summary>
        private const string XimpleCfgFileName = "XimpleConfig.xml";

        #endregion

        #region Static Fields

        /// <summary>
        ///     Default failed socket connections before action
        /// </summary>
        private static readonly int DefaultMaxFailedSocketConnections = 2;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        ///     The container of all the dictionary' entries.
        /// </summary>
        private readonly ConfigManager<Dictionary> dictionaryConfigManager;

        /// <summary>
        ///     Event used to manage the running status of this protocol.
        /// </summary>
        private readonly ManualResetEvent protocolRunningEvent = new ManualResetEvent(false);

        private readonly ConfigManager<XimpleConfig> ximpleConfigManager;

        /// <summary>
        ///     Current failed connections in monitoring the Ximple TCP Server
        /// </summary>
        private int failedConnections;

        /// <summary>
        ///     background time to monitor the server
        /// </summary>
        private ITimer monitorXimpleServerTimer;

        private XimpleSocketService socketService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="XimpleProtocolImpl" /> class.
        /// </summary>
        /// <exception cref="Exception">Condition.</exception>
        public XimpleProtocolImpl()
        {
            try
            {
                Logger.Info("Construct XimpleProtocolImpl");
                var configFile = PathManager.Instance.GetPath(FileType.Config, XimpleCfgFileName);
                if (File.Exists(configFile) == false)
                {
                    configFile = XimpleCfgFileName;
                }

                Logger.Debug("Initialize XimpleProtocolImpl reading Configuration file = {0}", configFile);

                this.ximpleConfigManager = new ConfigManager<XimpleConfig> { FileName = configFile, XmlSchema = XimpleConfig.Schema };
                this.Config = this.ximpleConfigManager.Config;

                var dictionaryFile = PathManager.Instance.GetPath(FileType.Config, "dictionary.xml");
                Logger.Debug("Reading Dictionary {0}", dictionaryFile);
                this.dictionaryConfigManager = new ConfigManager<Dictionary>
                {
                    XmlSchema = Dictionary.Schema,
                    FileName = dictionaryFile,
                    EnableCaching = true
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to read config file {0}", ex.Message);
                throw;
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Event that is fired when the protocol has finished starting up.
        /// </summary>
        public event EventHandler Started;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the protocol config.
        /// </summary>
        public XimpleConfig Config { get; }

        /// <summary>
        ///     Gets the dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        ///     Gets the host.
        /// </summary>
        public IProtocolHost Host { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether
        ///     this channel is opened or not.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        ///     Gets the name of this protocol.
        /// </summary>
        public string Name
        {
            get
            {
                return "XimpleProtocolImpl";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Configures this protocol with the given configuration.</summary>
        /// <param name="dictionary">The generic view dictionary.</param>
        public void Configure(Dictionary dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>
        ///     The main function of your protocol.
        ///     Will be invoked by the protocol's host.
        /// </summary>
        /// <param name="host">The owner of this protocol.</param>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="ConfiguratorException">If errors occurred while deserializing the file</exception>
        public void Run(IProtocolHost host)
        {
            this.Host = host;

            try
            {
                Debug.Assert(this.ximpleConfigManager != null, "ConfigManger is null");

                if (this.ximpleConfigManager.Config.Enabled)
                {
                    Logger.Info(
                        "Luminator TCP Ximple protocol is running. Listening on Port:{0}",
                        this.ximpleConfigManager.Config.Port);

                    // Start our socket service
                    Debug.Assert(this.ximpleConfigManager.Config != null, "Config is Null object");
                    var configManager =
                        new ConfigManager<Dictionary>
                            {
                                FileName = PathManager.Instance.GetPath(
                                    FileType.Config,
                                    "dictionary.xml")
                            };
                    this.StartXimpleServer(this.ximpleConfigManager.Config, configManager);

                    // background time to routinely test the TCP Socket connection to the XimpleServer as a client             
                    if (this.Config.XimpleServerMonitorTimerInterval > 0)
                    {
                        this.monitorXimpleServerTimer = TimerFactory.Current.CreateTimer("XimpleServerMonitor");
                        this.monitorXimpleServerTimer.AutoReset = true;
                        this.monitorXimpleServerTimer.Elapsed += this.MonitorXimpleServerTimerOnElapsed;
                        this.monitorXimpleServerTimer.Interval =
                            TimeSpan.FromSeconds(this.Config.XimpleServerMonitorTimerInterval);
                        this.monitorXimpleServerTimer.Enabled = true;
                    }

                    var app = ServiceLocator.Current.GetInstance<ProtranApplication>();
                    app.RegistrationCompleted += this.RegistrationCompleted;
                }
                else
                {
                    Logger.Info("Luminator TCP Ximple protocol is disabled");
                }

                this.RaiseStarted(EventArgs.Empty);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Logger.Error(fileNotFoundException.Message);
                throw;
            }
            catch (SocketException)
            {
                Logger.Error("Ximple TCP Prototol failed to start, check TCP settings and premissions");
            }

            this.protocolRunningEvent.WaitOne();
        }

        /// <summary>
        ///     Stop this protocol.
        /// </summary>
        public void Stop()
        {
            Logger.Info("Luminator XimplePrototol protocol is stopping Enter");

            // SendEmptyXimple();
            if (this.monitorXimpleServerTimer != null)
            {
                this.monitorXimpleServerTimer.Enabled = false;
            }

            this.protocolRunningEvent.Set();
            this.StopXimpleService();
            Logger.Info("Luminator XimplePrototol protocol is stopped Exit");
        }

        #endregion

        #region Explicit Interface Methods

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("Channel open", this.IsOpen, true);
        }

        #endregion

        #region Methods

        private void MonitorXimpleServerTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            var loopback = IPAddress.Loopback;
            var restartProtranApp = false;
            var port = this.Config?.Port ?? 0;
            var maxFailures = this.Config?.MaxXimpleServerFailuresBeforeRestart ?? DefaultMaxFailedSocketConnections;
            bool testRestart = maxFailures <= 0;
            try
            {
                this.monitorXimpleServerTimer.Enabled = false;
                if (port <= 0)
                {
                    return;
                }

                Logger.Info("Monitor XimpleServer... {0}:{1}, testRestart={2}", loopback, port, testRestart);
                using (var client = new TcpClient())
                {
                    client.Connect(loopback, port);
                    if (client.Connected)
                    {
                        // good
                        Logger.Info("Socket Test Successfully connected to XimpleServer. Normal Socket Close {0}", client.Client.LocalEndPoint);
                        Thread.Sleep(1000);
                        client.Close();
                        this.failedConnections = 0;
                    }
                    else
                    {
                        this.failedConnections++;
                        Logger.Warn("Connection to XimpleServer Failed. Count={0}, maxFailures={1}", this.failedConnections, maxFailures);
                    }

                    if (this.failedConnections >= maxFailures || testRestart)
                    {
                        this.failedConnections = 0;
                        Logger.Error("XimpleServer Loop-back failed to connect. Restarting Application to recover.");
                        restartProtranApp = true;

                        var alarm = ApplicationAlarmFactory.CreateRelaunch(
                             ApplicationRelaunchAttribute.Watchdog,
                             "Restart Protran XimpleServer to receive incoming TCP connections");
                        MessageDispatcher.Instance.Broadcast(alarm);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to connect to local Ximple server {0}", ex.Message);
            }
            finally
            {
                if (restartProtranApp)
                {
                    Task.Factory.StartNew(
                        () =>
                            {
                                Logger.Error("Restarting Protran....");
                                var app = ServiceLocator.Current.GetInstance<ProtranApplication>();
                                app?.Relaunch("Relaunching Protran XimpleServer did not accept new TCP Connection");
                            });
                }
                else
                {
                    this.monitorXimpleServerTimer.Enabled = true;
                }

                Logger.Info("MonitorXimpleServerTimer Exit");
            }
        }

        /// <summary>Raises the protocol started <see cref="Started" /> event back to the Host.</summary>
        /// <param name="e">The e.</param>
        private void RaiseStarted(EventArgs e)
        {
            Logger.Info("{0} Enter", nameof(this.RaiseStarted));
            var handler = this.Started;
            handler?.Invoke(this, e);
        }

        private void RaiseXimpleCreated(Ximple ximple)
        {
            if (ximple != null && this.socketService != null)
            {
                this.socketService.RaiseXimpleCreatedEvent(ximple);
            }
        }

        private void RegistrationCompleted(object sender, EventArgs eventArgs)
        {
            Logger.Info("Host Application Register Completed");
        }

        private void SendEmptyXimple(string text = "")
        {
            var ximple = new Ximple(Constants.Version2);
            for (int row = 0; row < 6; row++)
            {
                ximple.Cells.Add(new XimpleCell(text, 12, row, 0, 0));
                ximple.Cells.Add(new XimpleCell(string.Empty, 12, row, 1, 0));
                ximple.Cells.Add(new XimpleCell(string.Empty, 12, row, 2, 0));
                ximple.Cells.Add(new XimpleCell(string.Empty, 12, row, 3, 0));
                ximple.Cells.Add(new XimpleCell(string.Empty, 12, row, 4, 0));
            }

            ximple.Cells.Add(new XimpleCell(string.Empty, 11, 0, 0, 0)); // DestinationName
            Logger.Warn("Sending Empty Table 12 to clear Stops....");
            this.RaiseXimpleCreated(ximple);
        }

        private void StartXimpleServer(IXimpleConfig config, ConfigManager<Dictionary> dictionaryConfigManager)
        {
            if (this.socketService != null)
            {
                return;
            }

            Logger.Info("StartXimpleServer() Enter");
            this.socketService = new XimpleSocketService(config, dictionaryConfigManager);
            this.socketService.XimpleCreated += (sender, args) =>
                {
                    // send the Ximple on here
                    if (this.Host != null && args.Ximple != null)
                    {
                        Logger.Info("+++ XimpleCreated event: Forwarding received Ximple to Host...");
                        this.Host.OnDataFromProtocol(this, args.Ximple);
                    }
                };

            this.socketService.BadXimple += (sender, args) =>
                {
                    Logger.Warn("XimpleProtocolImpl received bad Ximple Xml {0}", args.Xml);
                };
            try
            {
                this.socketService.Start(IPAddress.Any, config);
                Logger.Info("StartXimpleServer() Exit");
            }
            catch (SocketException socketException)
            {
                Logger.Error("Failed to bind to Port:{0} to start Ximple Server {1}", config.Port, socketException.Message);
                throw;
            }
        }

        private void StopXimpleService()
        {
            if (this.socketService != null)
            {
                Logger.Info("StopXimpleService() Enter");
                this.socketService.Stop();
            }
        }

        #endregion
    }
}