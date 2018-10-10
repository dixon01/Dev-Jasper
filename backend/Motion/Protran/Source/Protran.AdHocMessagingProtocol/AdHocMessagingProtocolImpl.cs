// InfomediaAll
// Luminator.Protran.AdHocMessagingProtocol
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Timers;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Protran.Core.Protocols;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;
    using Luminator.Motion.Protran.AdHocMessagingProtocol.Models;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using Timer = System.Timers.Timer;

    [SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1305:FieldNamesMustNotUseHungarianNotation",
        Justification = "Reviewed. Suppression is OK here.")]
    public class AdHocMessagingProtocolImpl : IMessagingProtocolImpl
    {
        private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

        /// <summary>The vehicle info changed.</summary>
        public EventHandler<VehicleInfo> VehicleInfoChanged;

        /// <summary>
        ///     Event used to manage the running status of this protocol.
        /// </summary>
        private readonly ManualResetEvent protocolRunningEvent = new ManualResetEvent(false);

        private readonly object vehicleUnitInfoLock = new object();

        private readonly object serviceRunStateLock = new object();

        private Timer adhocServiceTimer;

        private bool configured;

        private VehicleUnitInfo currentVehicleUnitInfo;

        private ServiceRunState currentServiceRunState;

        private bool disposed;

        /// <summary>Initializes a new instance of the <see cref="AdHocMessagingProtocolImpl" /> class.</summary>
        /// <param name="config">The Message implementation config.</param>
        /// <param name="adHocMessageService">The adhoc message service.</param>
        /// <param name="dictionary">The dictionary.</param>
        public AdHocMessagingProtocolImpl(
            IAdHocMessagingProtocolConfig config = null,
            IAdHocMessageService adHocMessageService = null,
            Dictionary dictionary = null)
            : this()
        {
            this.Configure(dictionary, config, adHocMessageService);
        }

        /// <summary>Initializes a new instance of the <see cref="AdHocMessagingProtocolImpl" /> class.</summary>
        public AdHocMessagingProtocolImpl()
        {
            this.CurrentVehicleUnitInfo = new VehicleUnitInfo();
            this.CurrentVehicleUnitInfo.VehicleInfo.UnitNames.Add(GetLocalUnitName());
            this.AdHocMessageService = null;            
            this.Dictionary = null;
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance(this);
        }

        /// <summary>The adhoc messages received event.</summary>
        public event EventHandler<IAdHocMessages> OnAdHocMessagesReceived;

        /// <summary>The on adhoc register completed by a unit.</summary>
        public event EventHandler<VehicleInfo> OnRegisterCompleted;

        /// <summary>The ximple ad hoc messages created event when new Ximple Adhoc was created.</summary>
        public event EventHandler<IList<IXimpleAdHocMessage>> OnXimpleAdHocMessageCreated;

        /// <summary>The protocol started event handler.</summary>
        public event EventHandler Started;

        /// <summary>Gets or sets the ad hoc message service.</summary>
        public IAdHocMessageService AdHocMessageService { get; set; }

        /// <summary>Gets or sets the adhoc registration count.</summary>
        public int AdhocRegistrationCount { get; set; }

        /// <summary>Gets or sets the config.</summary>
        public IAdHocMessagingProtocolConfig Config { get; set; }

        public VehicleUnitInfo CurrentVehicleUnitInfo
        {
            get
            {
                lock (this.vehicleUnitInfoLock)
                {
                    return this.currentVehicleUnitInfo;
                }
            }

            set
            {
                if (value != null)
                {
                    lock (this.vehicleUnitInfoLock)
                    {
                        this.currentVehicleUnitInfo = value;
                        Debug.WriteLine(value);
                        Logging.Info("Current VehicleUnitInfo updated {0}", value);
                    }
                }
            }
        }

        /// <summary> Gets the dictionary used for Ximple creation.</summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>Gets the protocol host.</summary>
        public IProtocolHost Host { get; private set; }

        /// <sumary>Gets a value indicating whether this protocol is opened or not.</sumary>
        public bool IsOpen
        {
            get
            {
                return true;
            }

            private set
            {
            }
        }

        /// <summary>Gets the required protocol name name.</summary>
        public string Name => nameof(AdHocMessagingProtocolImpl);

        /// <summary>Gets or sets the service run state for timer functionality.</summary>
        public ServiceRunState ServiceRunState
        {
            get
            {
                lock (this.serviceRunStateLock)
                {
                    return this.currentServiceRunState;
                }
            }
            set
            {
                lock (this.serviceRunStateLock)
                {
                    this.currentServiceRunState = value;
                }
            }
        }

        private bool IsValidAdhocPollInterval => this.Config.AdHocMessageTimerSettings.IsValid;

        private bool IsValidAdhocRegisterUnitPollInterval => this.Config.RegisterUnitTimerSettings.IsValid;

        /// <summary>Configure the protocol.</summary>
        /// <param name="dictionary">The dictionary to use for Ximple.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Configure(Dictionary dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "Ximple Dictionary is undefined");
            }

            var configFile = PathManager.Instance.CreatePath(FileType.Config, AdHocMessagingProtocolConfig.DefaultConfigFileName);
            try
            {
                if (!File.Exists(configFile))
                {
                    configFile = AdHocMessagingProtocolConfig.DefaultConfigFileName;
                }

                var config = AdHocMessagingProtocolConfig.ReadConfig(configFile);
                this.Configure(dictionary, config, null);
            }
            catch (Exception ex)
            {
                Logging.Error("Failed to read configuration file {0} {1}", configFile, ex.Message);
            }
        }

        /// <summary>The configure.</summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="config">The config.</param>
        /// <param name="adHocMessageService">The ad hoc message service.</param>
        /// <exception cref="ArgumentNullException">Null arguments</exception>
        public virtual void Configure(Dictionary dictionary, IAdHocMessagingProtocolConfig config, IAdHocMessageService adHocMessageService)
        {
            if (!this.configured)
            {
                this.Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary), "Missing Dictionary");
                this.Config = config ?? throw new ArgumentNullException(nameof(config));
                this.AdHocMessageService = adHocMessageService ?? this.CreateAdHocMessageService(config);
                Logging.Info("Configure Executed");
                this.configured = true;
            }
        }

        /// <summary>Create Ximple for Adhoc medi event</summary>
        /// <param name="messages">The Source Event args to use</param>
        /// <returns>The <see cref="Ximple" />.</returns>
        public Ximple CreateXimpleAdHocMessage(IList<IXimpleAdHocMessage> messages)
        {
            Ximple ximple = null;
            var table = this.Dictionary?.FindXimpleTable("InfoTainmentAdhoc");
            if (table == null)
            {
                this.Dictionary?.FindXimpleTable("InfotainmentAdhoc");
            }

            if (table != null)
            {
                Logging.Info("Creating Ximple AdHoc messages Count={0}", messages.Count);
                ximple = new Ximple(Constants.Version2);

                var row = 0;
                foreach (var m in messages)
                {
                    var language = m.Language;
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "Guid", m.Id.ToString(), row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "Text", m.Text, row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "Type", m.Type, row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "Title", m.Title, row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "StartDate", m.StartDate.HasValue ? m.StartDate.ToString() : "", row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "EndDate", m.EndDate.HasValue? m.EndDate.ToString() : "", row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "TimeToLive", m.TimeToLive.ToString(), row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "VehicleId", m.VehicleId, row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "Route", m.Route, row, language));
                    ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, "Destinations", m.Destinations, row, language));
                    row++;
                }
            }
            else
            {
                Logging.Warn("Failed to find Table for Check Dictionary.xml is present and contains table=InfoTainmentAdhoc");
            }

            return ximple;
        }

        /// <summary>Dispose and Stop execution.</summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.Stop();
            }
        }

        public IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        public IEnumerable<ManagementProperty> GetProperties()
        {
            yield return new ManagementProperty<bool>("Channel open", this.IsOpen, true);
        }

        public VehicleInfo ReadVehiclePositionFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            using (var fileStream = File.OpenRead(fileName))
            {
                var s = new XmlSerializer(typeof(VehicleInfo));
                return s.Deserialize(fileStream) as VehicleInfo;
            }
        }

        /// <summary>Start up and Run the protocol blocks till stopped.</summary>
        /// <param name="host">The host.</param>
        public void Run(IProtocolHost host)
        {
            this.Host = host;
            try
            {
                if (this.AdHocMessageService != null && this.Config != null && this.Initialize())
                {
                    this.Start();
                    Logging.Info("AdHoc MessagingProtocolImpl Started Successfully");
                }
                else
                {
                    Logging.Warn("AdHoc Messaging Protocol Not Started!");
                }

                // Block this thread
                this.protocolRunningEvent.WaitOne();
            }
            catch (IOException ex)
            {
                Logging.Warn("MessagingProtocolImpl will not be available {0}", ex.Message);
            }
        }

        /// <summary>Start the service.</summary>
        /// <param name="runState">The initial default run state.</param>
        /// <exception cref="ArgumentNullException">Invalid arguments</exception>
        public virtual void Start(ServiceRunState runState = ServiceRunState.RegisterUnit)
        {
            Logging.Info("Started AdHocMessaging Services");

            if (this.Config == null)
            {
                throw new NullReferenceException("Configuration has not been set!");
            }

            if (this.AdHocMessageService == null)
            {
                throw new NullReferenceException("AdHocMessageService has not been set!");
            }

            this.ServiceRunState = runState;
            this.CreateAndStartTimers(this.ServiceRunState);

            // tell the host the protocol has fully started
            this.RaiseStarted(EventArgs.Empty);
        }

        /// <summary>Stop the AdHoc Service.</summary>
        public void Stop()
        {
            // Kill active timer   
            Logging.Info("Stopped AdHocMessaging Timer");
            this.adhocServiceTimer?.Stop();
        }

        /// <summary>Update vehicle info.</summary>
        /// <param name="vehicleInfo">The vehicle info.</param>
        public void UpdateVehicleInfo(VehicleInfo vehicleInfo)
        {
            lock (this.vehicleUnitInfoLock)
            {
                if (this.CurrentVehicleUnitInfo.VehicleInfo.VehicleId == vehicleInfo.VehicleId
                    && this.CurrentVehicleUnitInfo.VehicleInfo.UnitNames.SequenceEqual(vehicleInfo.UnitNames))
                {
                    // no change
                    return;
                }

                this.CurrentVehicleUnitInfo.VehicleInfo = vehicleInfo;

                // Change the state to register when the vehicle id or unit names change.
                // The bus driver can change the vehicle Id manually via the MCU after we first registered on start
                this.ServiceRunState = ServiceRunState.RegisterUnit;
                this.AdhocRegistrationCount = 0;
            }

            this.VehicleInfoChanged?.Invoke(this, vehicleInfo);
            this.WriteVehiclePosition(vehicleInfo);
        }

        /// <summary>Update vehicle position.</summary>
        /// <param name="vehiclePosition">The vehicle position.</param>
        public void UpdateVehiclePosition(VehiclePositionMessage vehiclePosition)
        {
            lock (this.vehicleUnitInfoLock)
            {
                this.CurrentVehicleUnitInfo.VehiclePosition = vehiclePosition;
            }
        }

        /// <summary>Update vehicle unit info.</summary>
        /// <param name="vehicleUnitInfo">The vehicle unit info.</param>
        public void UpdateVehicleUnitInfo(VehicleUnitInfo vehicleUnitInfo)
        {
            lock (this.vehicleUnitInfoLock)
            {
                if (vehicleUnitInfo != null)
                {
                    if (vehicleUnitInfo.VehicleInfo != null)
                    {
                        this.UpdateVehicleInfo(vehicleUnitInfo.VehicleInfo);
                    }

                    if (vehicleUnitInfo.VehiclePosition != null)
                    {
                        this.UpdateVehiclePosition(vehicleUnitInfo.VehiclePosition);
                    }
                }
            }
        }

        protected virtual void DoAdHocService()
        {
            var updateTimerInterval = true;
            try
            {
                this.adhocServiceTimer.Enabled = false;
                Logging.Info(nameof(this.AdhocServiceTimerOnElapsed) + "ServiceRunState=" + this.ServiceRunState);

                switch (this.ServiceRunState)
                {
                    case ServiceRunState.Idle:
                        this.AdhocRegistrationCount = 0;
                        break;

                    case ServiceRunState.RequestVehicleInfo:
                        this.RequestVehicleInfo();
                        break;

                    case ServiceRunState.RegisterUnit:
                        // If registration of units is disabled or we maxed out the attempts
                        var skipRegistration = false;
                        if (this.Config.EnableUnitRegistration == false)
                        {
                            Logging.Warn("Registration is disabled");
                            skipRegistration = true;
                        }

                        if (this.AdhocRegistrationCount >= this.Config.MaxAdhocRegistrationAttempts)
                        {
                            Logging.Warn("AdhocRegistrationCount {0} >= {1}", this.AdhocRegistrationCount, this.Config.MaxAdhocRegistrationAttempts);
                            skipRegistration = true;
                        }

                        if (skipRegistration)
                        {
                            // skip to next state
                            Logging.Info("Unit Registration skipped setting next state to Request AdHoc Messages");
                            this.ServiceRunState = ServiceRunState.RequestAdHocMessages;
                            updateTimerInterval = false;
                            this.SetServiceTimerInterval(1000);
                            break;
                        }

                        if (this.currentVehicleUnitInfo.IsValidVehicleInfo)
                        {
                            this.AdhocRegistrationCount++;
                            var vehicleId = this.CurrentVehicleUnitInfo?.VehicleInfo.VehicleId;
                            var unitNames = this.currentVehicleUnitInfo?.VehicleInfo.UnitNames;
                            var request = new AdHocRegisterRequest(vehicleId, unitNames);
                            Logging.Info("Unit Registration Attempt={0}, for Registration Request={1}", this.AdhocRegistrationCount, request);

                            var response = this.AdHocMessageService.RegisterVehicleAndUnit(request);
                            if (IsResponseOk(response))
                            {
                                // TODO add any further checks for registration before changing state
                                this.ServiceRunState = ServiceRunState.RequestAdHocMessages;

                                if (response.IsRegistered)
                                {
                                    this.OnRegisterCompleted?.Invoke(this, this.CurrentVehicleUnitInfo?.VehicleInfo);
                                }
                            }
                            else
                            {
                                Logging.Warn("Registration Response != HttpStatusCode.Ok request={0}", request);
                            }
                        }
                        else
                        {
                            Logging.Warn("Invalid Vehicle Info, unable to register!");
                            Debug.WriteLine("!VehicleInfo incomplete currentVehicleUnitInfo=" + this.currentVehicleUnitInfo);
                        }

                        break;

                    case ServiceRunState.RequestAdHocMessages:
                        var primaryUnit = this.CurrentVehicleUnitInfo?.FirstUnitName;

                        if (!string.IsNullOrEmpty(primaryUnit))
                        {
                            var route = this.CurrentVehicleUnitInfo.VehiclePosition.Route;
                            var vehicleId = this.CurrentVehicleUnitInfo.IsValidVehicleId
                                                ? this.CurrentVehicleUnitInfo.VehicleInfo.VehicleId
                                                : string.Empty;
                            var request = new AdHocGetMessagesRequest(route, primaryUnit, vehicleId, DateTime.Now);
                            var adHocMessages = this.AdHocMessageService?.GetUnitAdHocMessages(request);

                            if (adHocMessages != null && IsResponseOk(adHocMessages))
                            {
                                this.RaiseXimpleAdHocMessages(adHocMessages.Messages);
                                this.OnAdHocMessagesReceived?.Invoke(this, adHocMessages);
                            }
                            else
                            {
                                this.ProcessCachedAdHocMessages();
                            }

                            this.ServiceRunState = ServiceRunState.RequestAdHocMessages;
                        }
                        else
                        {
                            Logging.Warn("Invalid Current VehicleUnitInfo RequestAdHocMessages Ignored!");
                        }

                        break;
                }
            }
            finally
            {
                if (updateTimerInterval)
                {
                    this.SetServiceTimerInterval(this.GetServiceStateTimeSettingsMilliseconds(this.ServiceRunState));
                    var interval = this.adhocServiceTimer?.Interval;
                    if (interval != null)
                    {
                        Logging.Info("Timer Exited Next State={0} Interval={1}ms", this.ServiceRunState, (int)interval);
                    }
                }

                this.adhocServiceTimer.Enabled = true;
            }
        }

        private static string GetLocalUnitName()
        {
            try
            {
                return MessageDispatcher.Instance.LocalAddress.Unit;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool IsResponseOk(IAdHocResponse response)
        {
            return response != null && response.Status == HttpStatusCode.OK;
        }

        private void AdhocServiceTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            this.DoAdHocService();
        }

        private AdHocMessageService CreateAdHocMessageService(IAdHocMessagingProtocolConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), "Invalid  AdHocMessagingProtocolConfig");
            }

            // create the manager's config and the adhoc manger
            if (config.AdHocApiUri != null && config.AdHocApiUri.HostUri.IsWellFormedOriginalString())
            {
                var adhocManagerConfig = new AdHocMessageServiceConfig(config);

                var service = new AdHocMessageService(adhocManagerConfig, null);
                return service;
            }

            throw new UriFormatException("Invalid ServerUri AdHoc Manager URI, check settings");
        }

        private void CreateAndStartTimers(ServiceRunState serviceRunState)
        {
            Debug.WriteLine("CreateAndStartTimers Enter");
            if (this.adhocServiceTimer == null)
            {
                var milliseconds = this.GetServiceStateTimeSettingsMilliseconds(serviceRunState);
                if ((int)milliseconds <= 0)
                {
                    milliseconds = 1000;
                }

                try
                {
                    // Zero interval is invalid!
                    this.adhocServiceTimer = new Timer { Enabled = true, AutoReset = true, Interval = milliseconds };
                    this.adhocServiceTimer.Elapsed += this.AdhocServiceTimerOnElapsed;
                    this.adhocServiceTimer.Start();
                    Logging.Info("Started timer Interval = {0}", milliseconds);
                }
                catch (Exception ex)
                {
                    Logging.Error("Failed to create timer {0}", ex.Message);
                    throw;
                }
            }
        }

        private List<IXimpleAdHocMessage> CreateXimpleAdhocMessages(IAdHocMessages adHocMessages)
        {
            var messages = new List<IXimpleAdHocMessage>();

            if (adHocMessages?.Messages == null)
            {
                return messages;
            }

            foreach (var m in adHocMessages?.Messages)
            {
                var ttlTimeSpan = TimeSpan.Zero;
                if (m.StartDate.HasValue && m.EndDate.HasValue)
                {
                    ttlTimeSpan = m.EndDate.Value.Subtract(m.StartDate.Value);
                }

                var adhoc = new XimpleAdHocMessage
                                {
                                    Language = m.Language >= 0 ? m.Language : 0,
                                    Title = m.Title,
                                    Text = m.Text,
                                    Type = m.Type,
                                    Destinations = m.Destinations,
                                    StartDate = m.StartDate,
                                    EndDate = m.EndDate,
                                    Route = m.Route,
                                    TimeToLive = ttlTimeSpan
                                };
                messages.Add(adhoc);
            }

            return messages;
        }

        private TimeSpan GetServiceStateTimeSettings(ServiceRunState serviceRunState)
        {
            switch (serviceRunState)
            {
                default: return this.adhocServiceTimer != null ? TimeSpan.FromMilliseconds(this.adhocServiceTimer.Interval) : TimeSpan.Zero;
                case ServiceRunState.Idle: return TimeSpan.FromSeconds(1);

                case ServiceRunState.RegisterUnit:
                    return this.Config.EnableUnitRegistration ? this.Config.RegisterUnitTimerSettings.Interval : TimeSpan.FromSeconds(1);
                case ServiceRunState.RequestAdHocMessages: return this.Config.AdHocMessageTimerSettings.Interval;
                case ServiceRunState.RequestVehicleInfo: return this.Config.RequestUnitInfoTimerSettings.Interval;
            }
        }

        private double GetServiceStateTimeSettingsMilliseconds(ServiceRunState serviceRunState)
        {
            return this.GetServiceStateTimeSettings(serviceRunState).TotalMilliseconds;
        }

        private bool Initialize()
        {
            Logging.Info("Initialize Enter");

            // initialize our work create the required medi subscriptions and adhoc manager
            if (this.Config == null)
            {
                var configFile = PathManager.Instance.GetPath(FileType.Config, AdHocMessagingProtocolConfig.DefaultConfigFileName);

                if (File.Exists(configFile) == false)
                {
                    configFile = AdHocMessagingProtocolConfig.DefaultConfigFileName;
                }

                Logging.Info("Run Enter Creating MessagingProtocolImpl Reading Config file: {0}", configFile);
                this.Config = AdHocMessagingProtocolConfig.ReadConfig(configFile);
            }

            MessageDispatcher.Instance.Subscribe<VehicleUnitInfo>(
                (sender, args) =>
                    {
                        if (args?.Message != null)
                        {
                            Logging.Info("=> Received medi VehicleUnitInfo event " + args.Message);
                            this.UpdateVehicleUnitInfo(args.Message);
                        }
                    });

            MessageDispatcher.Instance.Subscribe<VehicleInfo>(
                (sender, args) =>
                    {
                        if (args?.Message != null)
                        {
                            Logging.Info("=> Received medi VehicleInfo event " + args.Message);
                            this.UpdateVehicleInfo(args.Message);
                        }
                    });

            MessageDispatcher.Instance.Subscribe<VehiclePositionMessage>(
                (sender, args) =>
                    {
                        if (args?.Message != null)
                        {
                            Logging.Info("=> Received medi VehiclePositionMessage event " + args.Message);
                            this.UpdateVehiclePosition(args.Message);
                        }
                    });

            // Create our AdHoc manager to support REST API calls to the backend
            if (this.AdHocMessageService == null)
            {
                this.AdHocMessageService = this.CreateAdHocMessageService(this.Config);
            }

            return true;
        }

        private void ProcessCachedAdHocMessages()
        {
            // TODO send cached reply handle TTL
        }

        /// <summary>The raise started.</summary>
        /// <param name="e">The e.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        private void RaiseStarted(EventArgs e)
        {
            Logging.Info("{0} Enter", nameof(this.RaiseStarted));
            var handler = this.Started;
            handler?.Invoke(this, e);
        }

        /// <summary>
        ///     Raise a XimpleCreated medi message for the presentation layer for a collection of adhco content
        /// </summary>
        /// <param name="messages">Collection of AdHoc messages</param>
        private Ximple RaiseXimpleAdHocMessages(IList<IXimpleAdHocMessage> messages)
        {
            if (messages != null && messages.Any())
            {
                var count = 0;
                messages.ToList().ForEach(m => Logging.Info("   {0} Text=[{1}], Title=[{2}]", ++count, m.Text, m.Title));

                var ximple = this.CreateXimpleAdHocMessage(messages);
                if (ximple != null && ximple.Cells.Any())
                {
                    // Broadcast the Ximple medi message for the presentation layer
                    this.RaiseXimpleCreatedMediMessage(new XimpleEventArgs(ximple));

                    // broadcast we have new Ximple AdHoc data created
                    this.OnXimpleAdHocMessageCreated?.Invoke(this, messages);
                }

                return ximple;
            }

            return null;
        }

        private void RaiseXimpleCreatedMediMessage(XimpleEventArgs e)
        {
            Logging.Info("{0} Enter", nameof(this.RaiseXimpleCreatedMediMessage));
            var ximple = e.Ximple;
            if (ximple == null || this.Host == null)
            {
                // no XIMPLE to send.
                return;
            }

            this.Host.OnDataFromProtocol(this, e.Ximple);
            var firstCell = e.Ximple.Cells.FirstOrDefault();
            var tableNumber = 0;
            if (firstCell != null)
            {
                tableNumber = firstCell.TableNumber;
            }

            Logging.Info("Exit {0} sending XIMPLE TableNumber:{1} Cell Count={2}", nameof(AdHocMessagingProtocolImpl), tableNumber, e.Ximple.Cells.Count);
        }

        private void RequestVehicleInfo()
        {
            MessageDispatcher.Instance.Broadcast(new VehicleUnitInfoRequest());
        }

        private void RequestVehicleInfoTimerOnElapsed(object o, ElapsedEventArgs elapsedEventArgs)
        {
            this.RequestVehicleInfo();
        }

        private void SetServiceTimerInterval(double msInterval)
        {
            if (msInterval > 0 && this.adhocServiceTimer != null)
            {
                this.adhocServiceTimer.Interval = msInterval;
                Logging.Info("AdHoc Service Timer Interval={0}", this.adhocServiceTimer.Interval);
            }
        }

        private void WriteVehiclePosition(VehicleInfo vehicleInfo, string fileName = "VehiclePosition.xml")
        {
            var file = PathManager.Instance.CreatePath(FileType.Data, fileName);
            Logging.Info("Writing Config file {0}", file);
            using (var fileStream = File.Create(file))
            {
                var s = new XmlSerializer(typeof(VehicleInfo));
                s.Serialize(fileStream, vehicleInfo);
            }
        }
    }
}