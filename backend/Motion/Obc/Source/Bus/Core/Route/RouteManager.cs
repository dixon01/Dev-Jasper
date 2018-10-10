// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.NetworkInformation;
    using System.Threading;

    using Gorba.Common.Configuration.Obc.Bus;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Eci.Messages;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.Entities.Gps;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Bus.Core.CenterMotion;
    using Gorba.Motion.Obc.Bus.Core.Data;
    using Microsoft.Practices.ServiceLocation;
    using NLog;

    /// <summary>
    /// The manager responsible for bus services.
    /// </summary>
    internal class RouteManager
    {
        private static readonly Logger Logger = LogHelper.GetLogger<RouteManager>();

        private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        private readonly ManualResetEvent runWait = new ManualResetEvent(false);
        private readonly ManualResetEvent messageWait = new ManualResetEvent(false);
        private readonly List<ThreadStart> messageHandlers = new List<ThreadStart>();

        private readonly SimpleStopList simpleStopList = new SimpleStopList();

        private readonly List<TripInfo> trips = new List<TripInfo>();
        private readonly List<DriverTripInfo> driverTrips = new List<DriverTripInfo>();

        private readonly Dictionary<int, int> observedPoints = new Dictionary<int, int>();

        private IPersistenceService persistenceService;

        private IPersistenceContext<CurrentServiceInfo> currentServiceContext;

        private IPersistenceContext<CurrentTripStatus> tripStatusContext;

        private bool tripsLoaded;

        private bool hasErrors;

        private int serviceNumber;

        private int candidate;

        private int lastCandidate;

        private int currentTerminus;

        private Algorithm algorithm;

        private int candidateToTryIndex;

        private TimeSpan delay;

        private ParamIti userParameters;

        private BusConfig config;

        private int currentDayType;

        private GpsData currentGpsData;

        private float? lastLatitude;

        private float? lastLongitude;

        private bool reloadingInProgress;

        private DateTime candidateLoadTime;

        private int busStopLastSaved;

        private RouteInfo currentRoute;

        private bool tripToLoad;

        private AnnouncementHandler annoucement;

        private bool deviationFromMedi;

        private int currentZone = -1;

        private bool isSpeechCallActive;

        private int sentDelay;

        private CandidateStatus candidateStatus;

        private int currentTrip;

        private int currentLine;

        private IDataAccess dataAccess;

        private bool sendStopMessageEci;

        private int currentAlarmId;
        private EciAlarmState alarmState;

        private bool isRunning;

        private long lastSentPositionTicks;

        private CenterClient centerConnection;

        private long nextEciKeepAliveTicks;
        private long eciKeepAliveTimeout;
        private int missingEciKeepAliveAcks;

        private enum Algorithm
        {
            None = 0,
            LineService = 1,
            LineOnly = 2
        }

        /// <summary>
        /// Gets or sets the vehicle id used for ECI.
        /// </summary>
        public int VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the vehicle type.
        /// </summary>
        public VehicleTypeEnum VehicleType { get; set; }

        /// <summary>
        /// Gets or sets config type. This parameter was used in the French code and obsolete here
        /// </summary>
        public int ConfigType { get; set; }

        /// <summary>
        /// Gets the current service information.
        /// </summary>
        public CurrentServiceInfo CurrentService
        {
            get
            {
                return this.currentServiceContext.Value;
            }

            private set
            {
                this.currentServiceContext.Value = value;
            }
        }

        /// <summary>
        /// Gets the current trip status.
        /// </summary>
        public CurrentTripStatus CurrentTripStatus
        {
            get
            {
                return this.tripStatusContext.Value;
            }

            private set
            {
                this.tripStatusContext.Value = value;
            }
        }

        /// <summary>
        /// Gets the current candidate status.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2102:PropertyMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public CandidateStatus CandidateStatus
        {
            get
            {
                return this.candidateStatus;
            }

            private set
            {
                if (this.candidateStatus == value)
                {
                    return;
                }

                this.candidateStatus = value;
                switch (value)
                {
                    case CandidateStatus.ToLoad:
                        this.SetTripPosition(0, 0);
                        break;
                    case CandidateStatus.Loaded:
                        var trip = this.trips[this.candidate];
                        Logger.Debug("Next departure at {0}", trip.StartTime);
                        this.SendServiceMessage(trip.ServiceNumber, trip.LineNumber);
                        this.tripToLoad = true;
                        break;
                    case CandidateStatus.Valid:
                        trip = this.trips[this.candidate];
                        this.SetTripPosition(trip.TripId, trip.LineNumber);
                        this.serviceNumber = trip.ServiceNumber;
                        this.SendTripStartedMessage();
                        Logger.Debug("Trip validated");
                        this.SendEciDutyMessage(
                            this.trips[this.candidate].ServiceNumber, 't', this.CurrentService.DriverId, 0, 0);
                        if (!this.reloadingInProgress)
                        {
                            // on passe ici soit parce que le voyage vient de commencer
                            // soit parce qu'on recharge.
                            this.SaveTripStatus(0);
                        }

                        break;
                    case CandidateStatus.OutsideSchedule:
                        Logger.Log(
                            this.CurrentService.IsValidated ? LogLevel.Error : LogLevel.Warn, "Bus out of schedule");
                        this.SendDeviationDetectedMessage();
                        break;
                }
            }
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="busConfig">
        /// The bus config.
        /// </param>
        public void Configure(BusConfig busConfig)
        {
            this.config = busConfig;

            this.dataAccess = new IqubeCsvDataAccess();

            this.persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();

            this.currentServiceContext = this.persistenceService.GetContext<CurrentServiceInfo>();
            if (!this.currentServiceContext.Valid)
            {
                this.CurrentService = new CurrentServiceInfo();
            }

            this.tripStatusContext = this.persistenceService.GetContext<CurrentTripStatus>();
            this.tripStatusContext.Validity = TimeSpan.FromMinutes(30);
            if (!this.tripStatusContext.Valid)
            {
                this.CurrentTripStatus = new CurrentTripStatus();
            }

            this.simpleStopList.Stops.AddRange(this.dataAccess.LoadSimpleStopList());

            this.VehicleId = this.config.BusInfo.VehicleId;
            this.VehicleType = this.config.BusInfo.VehicleType;
            this.ConfigType = this.config.BusInfo.ConfigType;
        }

        /// <summary>
        /// The connect to center.
        /// </summary>
        public void ConnectToCenter()
        {
            this.centerConnection = new CenterClient(this.config.CenterClient);
            this.centerConnection.MessageReceived += this.CenterConnectionOnMessageReceived;
        }

        /// <summary>
        /// Runs the main loop of this manager.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void Run()
        {
            if (this.isRunning)
            {
                throw new NotSupportedException("Can't run twice");
            }

            this.isRunning = true;
            this.Initialize();
            this.LoadService();
            this.SendConfig();

            GpsData gpsData = null;
            var gpsHandler = new EventHandler<MessageEventArgs<GpsData>>((s, e) => gpsData = e.Message);
            MessageDispatcher.Instance.Subscribe(gpsHandler);

            this.SubscribeToMedi();

            this.centerConnection.Start();

            try
            {
                // we use a loop here because we don't want to deal with multi-threading and some loops might take
                // a considerable amount of time (see uses of Sleep() in algorithm code).
                var starTime = TimeProvider.Current.TickCount;
                var lastTime = starTime;
                var lastMinute = 0L;
                while (true)
                {
                    var ticks = TimeProvider.Current.TickCount;
                    var waitTime = lastTime + 1000 - ticks;
                    var hasMessages = true;
                    if (waitTime > 0)
                    {
                        lock (this.messageHandlers)
                        {
                            hasMessages = this.messageWait.WaitOne((int)waitTime, true);
                        }

                        if (!this.isRunning)
                        {
                            break;
                        }
                    }

                    if (hasMessages)
                    {
                        this.HandleMessages();
                        if (waitTime > 0)
                        {
                            continue;
                        }
                    }

                    lastTime = ticks;

                    if (ticks - lastMinute >= 60 * 1000)
                    {
                        // every minute
                        lastMinute = ticks;
                        if (ticks - starTime < 3 * 60 * 1000)
                        {
                            // during the first three minutes, send the config every 10 seconds
                            lastMinute -= 50 * 1000;
                        }

                        this.SendConfig();
                    }

                    this.currentGpsData = gpsData;
                    this.HandlePosition();

                    if (gpsData == null || !gpsData.SatelliteTimeUtc.HasValue)
                    {
                        continue;
                    }

                    this.currentGpsData = gpsData;
                    this.HandleGps();
                    this.HandleEciKeepAlive();
                }
            }
            catch (AbortLoopException ex)
            {
                // this exception is only thrown by Sleep() when we want to exit the application
                Logger.Trace("Exit was requested through abort exception", ex);
            }

            MessageDispatcher.Instance.Unsubscribe(gpsHandler);
            this.UnsubscribeFromMedi();
        }

        /// <summary>
        /// Stops this manager.
        /// </summary>
        public void Stop()
        {
            this.centerConnection.Stop();
            this.isRunning = false;
            this.runWait.Set();
        }

        private void SubscribeToMedi()
        {
            MessageDispatcher.Instance.Subscribe<evSetService>(this.HandleSetService);
            MessageDispatcher.Instance.Subscribe<evDeviationStarted>(this.HandleDeviationStarted);
            MessageDispatcher.Instance.Subscribe<evDeviationEnded>(this.HandleDeviationEnded);
            MessageDispatcher.Instance.Subscribe<evServiceStarted>(this.HandleServiceStarted);
            MessageDispatcher.Instance.Subscribe<evDriverAlarm>(this.HandleDriverAlarm);
            MessageDispatcher.Instance.Subscribe<evDuty>(this.HandleDuty);
            MessageDispatcher.Instance.Subscribe<evMessageAck>(this.HandleMessageAck);
            MessageDispatcher.Instance.Subscribe<evPassengerCount>(this.HandlePassengerCount);
            MessageDispatcher.Instance.Subscribe<evSpeechConnected>(this.HandleSpeechConnected);
            MessageDispatcher.Instance.Subscribe<evSpeechDisconnected>(this.HandleSpeechDisconnected);
        }

        private void UnsubscribeFromMedi()
        {
            MessageDispatcher.Instance.Unsubscribe<evSetService>(this.HandleSetService);
            MessageDispatcher.Instance.Unsubscribe<evDeviationStarted>(this.HandleDeviationStarted);
            MessageDispatcher.Instance.Unsubscribe<evDeviationEnded>(this.HandleDeviationEnded);
            MessageDispatcher.Instance.Unsubscribe<evServiceStarted>(this.HandleServiceStarted);
            MessageDispatcher.Instance.Unsubscribe<evDriverAlarm>(this.HandleDriverAlarm);
            MessageDispatcher.Instance.Unsubscribe<evDuty>(this.HandleDuty);
            MessageDispatcher.Instance.Unsubscribe<evMessageAck>(this.HandleMessageAck);
            MessageDispatcher.Instance.Unsubscribe<evPassengerCount>(this.HandlePassengerCount);
            MessageDispatcher.Instance.Unsubscribe<evSpeechConnected>(this.HandleSpeechConnected);
            MessageDispatcher.Instance.Unsubscribe<evSpeechDisconnected>(this.HandleSpeechDisconnected);
        }

        private void Initialize()
        {
            // see InitVE4() in the old code
            this.currentRoute = new RouteInfo();
            this.annoucement = new AnnouncementHandler();
            this.ReInit();
            this.serviceNumber = 0;

            this.ReadDayType();
            this.userParameters = this.dataAccess.LoadParamIti(0) ?? new ParamIti();

            this.delay = TimeSpan.Zero;

            var msg = new EciDelayedMessage(this.VehicleId, 0, 0, 0, 0, this.delay);
            this.centerConnection.SendEciMessage(msg);
            this.SendAdvDelayMessage(this.delay);
        }

        private void ReInit()
        {
            // see ReInitVE4() in the old code
            this.tripsLoaded = false;
            this.hasErrors = false;
            this.serviceNumber = 0;
            this.candidate = -1;
            this.lastCandidate = -1;
            this.currentTerminus = 0;

            this.CandidateStatus = CandidateStatus.ToLoad;
            this.algorithm = Algorithm.None;

            this.SendServiceMessage(0, 0);
            this.SendTripMessage(0, 0, TimeSpan.Zero, null);

            this.ReInitFindCandidateAlgorithm();
            this.InitDriverBlocks();
        }

        private void ReadDayType()
        {
            this.currentDayType = this.GetDayType();
        }

        private int GetDayType()
        {
            var now = TimeProvider.Current.Now;
            var date = now.Date;
            if (!this.IsCorrectDay(now))
            {
                // If the time is smaller than the newDayStart, take the date before today/now for the DayType
                date = date.AddDays(-1);
            }

            return this.dataAccess.GetDayType(date);
        }

        /// <summary>
        /// Check if the current date is the one we are driving for.
        /// </summary>
        /// <param name="now">
        /// The current date/time.
        /// </param>
        /// <returns>
        /// Returns false if (after midnight but) time is less than Schedule.DayStart.
        /// </returns>
        private bool IsCorrectDay(DateTime now)
        {
            return now.TimeOfDay >= this.config.Schedule.DayStart;
        }

        private void LoadService()
        {
            if (this.CurrentService.ServiceState != ServiceState.End
                || this.CurrentService.Services.Count == 0
                || this.CurrentService.Date.Day != TimeProvider.Current.Now.Day)
            {
                this.SetServiceState(ServiceState.Free);
                this.serviceNumber = 0;
                this.CurrentService.DriverId = 0;
                return;
            }

            this.LogServiceState();
            this.serviceNumber = this.CurrentService.Services[0].ServiceNumber;
            this.reloadingInProgress = true;
        }

        private void SetTripPosition(int trip, int line)
        {
            this.currentTrip = trip;
            this.currentLine = line;
        }

        private void SetServiceState(ServiceState state)
        {
            // was SetEtatService() in old code
            if (this.CurrentService.ServiceState == state)
            {
                return;
            }

            Logger.Info("State of service changed: {0} -> {1}", this.CurrentService.ServiceState, state);
            switch (state)
            {
                case ServiceState.Sending:

                    // clear the list of services because we will receive a new one soon
                    this.ClearServices();
                    break;
                case ServiceState.End:
                    this.CurrentService.Date = TimeProvider.Current.Now;
                    this.LogServiceState();
                    break;
            }

            this.CurrentService.ServiceState = state;
            this.SavePersistence();
        }

        private void ClearServices()
        {
            // was CleanServices() in the old code
            this.CurrentService.Services.Clear();
            this.CurrentService.IsValidated = false;
            this.SetServiceState(ServiceState.Free);
        }

        private void LogServiceState()
        {
            Logger.Info(
                "Service {0} is in {1}, started on {2:dd/MM/yyyy} ({3}validated)",
                this.CurrentService.Services.Count,
                this.CurrentService.ServiceState,
                this.CurrentService.Date,
                this.CurrentService.IsValidated ? string.Empty : "not ");

            var index = 0;
            foreach (var service in this.CurrentService.Services)
            {
                Logger.Info(
                    "[0:D2] Block {1} - Line {2} - Trip {3}",
                    index++,
                    service.ServiceNumber,
                    service.LineNumber,
                    service.TripNumber);
            }
        }

        private void SavePersistence()
        {
            var service = (IPersistenceServiceImpl)this.persistenceService;
            service.Save();
        }

        private void InitDriverBlocks()
        {
            this.driverTrips.Clear();
        }

        private void Sleep(TimeSpan sleepTime)
        {
            var sleep = (int)sleepTime.TotalMilliseconds;
            if (sleep <= 0)
            {
                Logger.Trace("Not sleeping ({0})", sleep);
                return;
            }

            Logger.Trace("Sleeping {0}ms", sleep);
            if (this.runWait.WaitOne(sleep, true))
            {
                throw new AbortLoopException();
            }
        }

        private void HandleMessages()
        {
            ThreadStart[] handlers = null;
            lock (this.messageHandlers)
            {
                if (this.messageHandlers.Count > 0)
                {
                    handlers = this.messageHandlers.ToArray();
                    this.messageHandlers.Clear();
                    this.messageWait.Reset();
                }
            }

            if (handlers == null)
            {
                return;
            }

            foreach (var handler in handlers)
            {
                try
                {
                    handler();
                }
                catch (AbortLoopException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.WarnException("Couldn't handle Medi message", ex);
                }
            }
        }

        private void HandlePosition()
        {
            // was GestionPosition() in the old code
            if (TimeProvider.Current.TickCount - this.lastSentPositionTicks >= 20 * 1000)
            {
                // don't update lastSentPositionTicks here, it will be done in SendPosition()
                this.SendPosition(PositionEvent.Gps, 0);
            }
        }

        private void HandleEciKeepAlive()
        {
            // TODO: all this could be done in CenterClient
            var ticks = TimeProvider.Current.TickCount;
            if (this.nextEciKeepAliveTicks == 0 || this.nextEciKeepAliveTicks <= ticks)
            {
                this.centerConnection.SendEciMessage(new EciKeepAliveMessage { VehicleId = this.VehicleId });
                this.nextEciKeepAliveTicks = ticks + (2 * 60 * 1000); // in 2 minutes
                this.eciKeepAliveTimeout = ticks + (1 * 60 * 1000); // in 1 minute
                return;
            }

            if (this.eciKeepAliveTimeout < ticks)
            {
                return;
            }

            this.eciKeepAliveTimeout = 0;
            this.missingEciKeepAliveAcks++;
            Logger.Warn("Didn't receive {0} ECI keep-alives", this.missingEciKeepAliveAcks);

            if (this.missingEciKeepAliveAcks > 1)
            {
                Logger.Info("Restarting Center connection");
                this.centerConnection.Stop();
                this.centerConnection.Start();
            }

            // TODO: reboot modem if possible when modulo 3 (see Bus.c:main())
            if (this.config.CenterClient.RebootIfAckMissing && this.missingEciKeepAliveAcks == 10)
            {
                SystemManagerClient.Instance.Reboot("Missing ECI Ack after 10 tries");
            }
        }

        private void HandleGps()
        {
            try
            {
                float longitude = 0;
                float latitude = 0;
                if (this.currentGpsData.IsValid)
                {
                    longitude = this.currentGpsData.Longitude;
                    latitude = this.currentGpsData.Latitude;

                    // if we have a previous position distant from more than 25m
                    // then we apply the algo to the average position too (Bugzilla #66)
                    if (this.lastLatitude.HasValue && this.lastLongitude.HasValue
                        && Wgs84.GetDistance(this.lastLongitude.Value, this.lastLatitude.Value, longitude, latitude)
                        > 25)
                    {
                        this.Calculate(
                            (this.lastLongitude.Value + longitude) / 2,
                            (this.lastLatitude.Value + latitude) / 2);
                    }

                    this.lastLatitude = latitude;
                    this.lastLongitude = longitude;
                }

                this.Calculate(longitude, latitude);
            }
            catch (AbortLoopException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't handle GPS data", ex);
            }
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void Calculate(float longitude, float latitude)
        {
            // see VE4() in the old code
            if (this.hasErrors)
            {
                return;
            }

            if (this.CurrentService.Services.Count == 0)
            {
                this.serviceNumber = 0;
                this.HandleExtraService(longitude, latitude);
                return;
            }

            if (this.trips.Count == 0)
            {
                return;
            }

            if (this.CurrentService.ServiceState != ServiceState.End)
            {
                // the service is being registered, we wait
                return;
            }

            if (this.algorithm == Algorithm.None)
            {
                if (this.CurrentService.Services.Count > 0)
                {
                    this.algorithm = this.CurrentService.Services[0].ServiceNumber == 0
                                         ? Algorithm.LineOnly
                                         : Algorithm.LineService;
                }

                return;
            }

            if (!this.tripsLoaded)
            {
                this.LoadTripList();

                if (this.reloadingInProgress)
                {
                    this.RestoreTripInfo();
                }
                else
                {
                    this.CurrentService.IsValidated = false;
                    this.candidate = -1;

                    if (this.CurrentService.Services[0].ServiceNumber != 0)
                    {
                        this.serviceNumber = this.CurrentService.Services[0].ServiceNumber;
                    }

                    // assuming we are always in EH mode (see old code checking for type)
                    if (this.tripsLoaded && this.trips.Count > 0)
                    {
                        this.SendServiceMessage(this.trips[0].ServiceNumber, this.trips[0].LineNumber);
                    }
                }

                return;
            }

            if (this.candidate == -1)
            {
                this.SearchCandidate();
                this.reloadingInProgress = false;
                return;
            }

            if (this.CandidateStatus == CandidateStatus.ToLoad)
            {
                // we need to load the trip
                this.LoadTrip();
                this.candidateLoadTime = TimeProvider.Current.Now;

                if (this.reloadingInProgress)
                {
                    // we already saved the a valid candidate, not necessary to do it again
                    this.CandidateStatus = CandidateStatus.Valid;

                    // validate all past stops
                    for (int i = 0; i < this.busStopLastSaved; i++)
                    {
                        this.currentRoute.Points[i].Status = PointStatus.Done;
                    }

                    var trip = this.trips[this.candidate];
                    this.SendServiceMessage(trip.ServiceNumber, trip.LineNumber);
                    this.reloadingInProgress = false;
                }

                return;
            }

            // on a chargé un candidat, dans le cas d'une releve, si on est dans l'arret de releve, on valide
            if (this.CandidateStatus == CandidateStatus.Loaded)
            {
                var trip = this.trips[this.candidate];
                if (trip.FirstStopIndex > 0)
                {
                    if (this.currentRoute.IsBusAtPoint(trip.FirstStopIndex, longitude, latitude))
                    {
                        this.CandidateStatus = CandidateStatus.Valid;

                        for (int i = 0; i < trip.FirstStopIndex - 2; i++)
                        {
                            this.currentRoute.Points[i].Status = PointStatus.Done;
                        }

                        this.Sleep(TimeSpan.FromSeconds(10));
                        this.SendTripMessage(trip.TripId, trip.CustomerTripId, trip.StartTime, this.currentRoute);
                        this.SendTripLoadedMessage(
                            this.CurrentService.IsValidated || this.CandidateStatus == CandidateStatus.Valid);
                        this.tripToLoad = false;

                        this.Sleep(TimeSpan.FromSeconds(2));
                        this.HandleStatusChangeAtPoint(trip.FirstStopIndex - 1, PointStatus.Done);

                        this.Sleep(TimeSpan.FromSeconds(2));
                        this.HandleStatusChangeAtPoint(trip.FirstStopIndex, PointStatus.Entry);
                    }
                }
            }

            // On a un candidat, un itineraire
            // mais le voyage n'a pas commencé
            // on verifie s'il est toujours valide
            if (this.CandidateStatus == CandidateStatus.Loaded)
            {
                if (!this.CurrentService.IsValidated
                    && !this.IsCandidateInTime(this.candidate, TimeProvider.Current.Now, this.algorithm))
                {
                    var trip = this.trips[this.candidate];
                    Logger.Info(
                        "Candidate rejected (timeframe 1) [{0}] - {1} - block:{2} - line:{3} - trip:{4} - route:{5}",
                        this.candidate + 1,
                        trip.StartTime,
                        trip.ServiceNumber,
                        trip.LineNumber,
                        trip.TripId,
                        trip.RouteId);

                    this.UnloadTripAndTakeNext();
                    return;
                }

                // On a un candidat, le service n'est pas validé
                // on controle qu'on est geographiquement au terminus
                if (this.algorithm == Algorithm.LineOnly && !this.CurrentService.IsValidated
                    && this.currentRoute.Points[0].Status == PointStatus.NotTouched
                    && !this.currentRoute.IsBusAtPoint(0, latitude, longitude))
                {
                    // Roll terminus every 5 seconds or immediately if a terminus has been detected
                    if (TimeProvider.Current.Now - this.candidateLoadTime > TimeSpan.FromSeconds(5)
                        || this.currentTerminus != 0)
                    {
                        // Il n'est plus valide, on le decharge
                        var trip = this.trips[this.candidate];
                        Logger.Info(
                            "Candidate rejected (geo) [{0}] - {1} - block:{2} - line:{3} - trip:{4} - route:{5}",
                            this.candidate + 1,
                            trip.StartTime,
                            trip.ServiceNumber,
                            trip.LineNumber,
                            trip.TripId,
                            trip.RouteId);

                        this.UnloadTripAndTakeNext();
                    }

                    return;
                }

                // On a un candidat, que le service n'est pas validé
                // et qu'on est dans l'algo #2 (line mais pas service)
                // et qu'on est depuis + de 2mn au terminus,
                // on reboucle pour trouver un meilleur voyage
                if (this.algorithm == Algorithm.LineOnly && !this.CurrentService.IsValidated
                    && this.currentRoute.Points[0].Status != PointStatus.NotTouched
                    && this.currentRoute.Points[0].Status < PointStatus.Done
                    && TimeProvider.Current.Now - this.candidateLoadTime > TimeSpan.FromMinutes(2))
                {
                    this.currentTerminus = this.currentRoute.Points[0].Id;

                    // Il n'est plus valide, on le decharge
                    var trip = this.trips[this.candidate];
                    Logger.Info(
                        "Candidate rejected (roll) [{0}] - {1} - block:{2} - line:{3} - trip:{4} - route:{5}",
                        this.candidate + 1,
                        trip.StartTime,
                        trip.ServiceNumber,
                        trip.LineNumber,
                        trip.TripId,
                        trip.RouteId);

                    this.UnloadTripAndTakeNext();
                    return;
                }

                // We know the service.
                // But do not know where we are and have not seen any stop
                if (this.algorithm == Algorithm.LineService && !this.CurrentService.IsValidated
                    && !(this.currentRoute.IsBusAtPoint(0, latitude, longitude)
                         || this.currentRoute.Points[0].Status != PointStatus.NotTouched))
                {
                    bool doRoll;

                    var nextDayAdditionTime = this.IsCorrectDay(TimeProvider.Current.Now) ? TimeSpan.Zero : OneDay;
                    if (this.trips[this.candidate].StartTime > TimeProvider.Current.Now.TimeOfDay + nextDayAdditionTime)
                    {
                        // Trip in the future: roll if there is another candidate
                        doRoll = this.candidate > this.lastCandidate
                                 || TimeProvider.Current.Now - this.candidateLoadTime > TimeSpan.FromMinutes(2);
                    }
                    else
                    {
                        // Trip in the past: only do periodic roll
                        // DAC: periodic roll inhibits algo to synchronize on trip (multiple stop in a row detected)
                        // Give enough time that bus can pass more than 2 stops (10 min)
                        doRoll = TimeProvider.Current.Now - this.candidateLoadTime > TimeSpan.FromMinutes(10);
                    }

                    if (doRoll)
                    {
                        // Do a roll to check if there is a better trip (now)
                        var trip = this.trips[this.candidate];
                        Logger.Info(
                            "Candidate rejected (roll Terminus) "
                            + "[{0}] - {1} - block:{2} - line:{3} - trip:{4} - route:{5}",
                            this.candidate + 1,
                            trip.StartTime,
                            trip.ServiceNumber,
                            trip.LineNumber,
                            trip.TripId,
                            trip.RouteId);

                        this.lastCandidate = this.candidate;
                        this.UnloadTripAndTakeNext();
                        return;
                    }
                }
            }

            // Load the trip data now (no roll was done if we got here): Avoid sending Trip info if not chosen
            if (this.tripToLoad)
            {
                var trip = this.trips[this.candidate];
                this.SendTripMessage(trip.TripId, trip.CustomerTripId, trip.StartTime, this.currentRoute);
                this.SendTripLoadedMessage(
                    this.CurrentService.IsValidated || this.CandidateStatus == CandidateStatus.Valid);
                this.tripToLoad = false;
            }

            // On surveille les points en cours
            this.ObservePoints(longitude, latitude);

            // la fonction ObservePoints peut décharger l'itineraire (arrive a terminus par exemple)
            if (this.CandidateStatus == CandidateStatus.ToLoad)
            {
                return;
            }

            // On surveille le temps qui passe
            this.ObserveTime();

            // la fonction ObserveTime peut décharger l'itineraire (arrive a terminus par exemple)
            if (this.CandidateStatus == CandidateStatus.ToLoad)
            {
                return;
            }

            // Survey pending announcements
            this.annoucement.HandlePosition(longitude, latitude);

            var pointIndex = this.currentRoute.GetBusFirstTouchedPoint(longitude, latitude);
            if (pointIndex == -1)
            {
                // we are not on a point
                return;
            }

            var nextTheo = this.currentRoute.GetBusNextTheoreticalPoint();

            // On est sur un point
            if (pointIndex == nextTheo)
            {
                // On est sur le point attendu :)
                // Si on est au DEPART, on attend d'en sortir pour valider le voyage
                // Sinon, si on est au point attendu,
                // dans le cas de l'algo 1 :  le voyage est valide.
                // dans le cas de l'algo 2+ : le voyagee est valide si on est passé au DEPART
                if (this.algorithm == Algorithm.LineService)
                {
                    if (pointIndex > 0)
                    {
                        this.CandidateStatus = CandidateStatus.Valid;
                    }
                }
                else
                {
                    if (pointIndex > 0 && this.currentRoute.Points[0].Status == PointStatus.Done)
                    {
                        this.CandidateStatus = CandidateStatus.Valid;
                    }
                }

                this.AddObservedPoint(pointIndex);
                this.HandleStatusChangeAtPoint(pointIndex, PointStatus.Entry);
                return;
            }

            if (pointIndex < nextTheo)
            {
                // On a reculé :(
                // On le signale et on le note
                this.currentRoute.Points[pointIndex].Status = PointStatus.Done;

                Logger.Warn("Backwards to point {0} ({1})", pointIndex, this.currentRoute.Points[pointIndex].Id);
                return;
            }

            // Calculate time difference of the stop seen
            var theo = this.trips[this.candidate].StartTime
                       + this.currentRoute.Points[pointIndex].TheoreticalPassageTime;
            var now = TimeProvider.Current.Now;
            var time = now.TimeOfDay;
            if (!this.IsCorrectDay(now))
            {
                time += TimeSpan.FromDays(1);
            }

            var timeDiff = theo - time;

            // On est pas sur un point attendu a ce moment la
            // 3 cas:
            // - soit on passe au terminus d'arrivé, on le prend en compte
            // - soit n points precedents sont déjà en hors-iti, dans ce cas, on raccroche
            // - sinon on est hors-iti
            if (pointIndex == this.currentRoute.GetTerminusIndex())
            {
                // (bugzilla #64)
                // DAC: if a terminus is seen take it unless it is more than 2 min early
                if (timeDiff <= TimeSpan.FromMinutes(2))
                {
                    this.HandleStatusChangeAtPoint(pointIndex, PointStatus.Entry);
                }
            }
            else if (this.currentRoute.Points[pointIndex].Status != PointStatus.OutsideSchedule)
            {
                // DAC: Bugzilla 64 - do not synchronize on trips that are more than 2 min early
                if (timeDiff <= TimeSpan.FromMinutes(5) && this.TestItiIn(pointIndex))
                {
                    // Le precedent est déjà en hors-iti
                    this.CandidateStatus = CandidateStatus.Valid;
                    this.HandleStatusChangeAtPoint(pointIndex, PointStatus.Entry);
                }
                else
                {
                    // On a loupé au moins un point
                    Logger.Info(
                        "Point reached to early {0} ({1}) - expected point {2} ({3})",
                        pointIndex,
                        this.currentRoute.Points[pointIndex].Id,
                        nextTheo,
                        this.currentRoute.Points[nextTheo].Id);

                    ////this.CandidateStatus = CandidateStatus.OutsideSchedule;
                    this.HandleStatusChangeAtPoint(pointIndex, PointStatus.OutsideSchedule);
                }
            }
        }

        private void AddObservedPoint(int pointIndex)
        {
            this.observedPoints[pointIndex] = pointIndex;
        }

        private void ObserveTime()
        {
            // was SurveyTime() in old code
            var now = TimeProvider.Current.Now;

            // On surveille le depassement du temps autorisé
            var terminusIndex = this.currentRoute.GetTerminusIndex();
            var timeSinceStart = now.TimeOfDay - this.trips[this.candidate].StartTime;
            timeSinceStart += this.IsCorrectDay(now) ? TimeSpan.Zero : OneDay;
            var theoArrival = this.currentRoute.Points[terminusIndex].TheoreticalPassageTime;
            var allowedDelay = TimeSpan.FromSeconds(theoArrival.TotalSeconds * this.userParameters.ItiDelete / 100);

            // if in Line only mode the trip is not valid, there is no need to check timing
            if (this.algorithm == Algorithm.LineOnly && this.CandidateStatus != CandidateStatus.Valid)
            {
                return;
            }

            if (timeSinceStart <= TimeSpan.Zero)
            {
                return;
            }

            if (this.delay > TimeSpan.Zero)
            {
                // Si on sait qu'on est en retard; on le prend en compte dans le timeout
                allowedDelay += this.delay;
            }

            for (int i = this.currentRoute.GetBusNextTheoreticalPoint(); i < terminusIndex; i++)
            {
                var point = this.currentRoute.Points[i];
                if (point.Status != PointStatus.Done && point.Status != PointStatus.Timeout
                    && timeSinceStart > point.TheoreticalPassageTime + allowedDelay)
                {
                    this.HandleStatusChangeAtPoint(i, PointStatus.Timeout);
                }
            }
        }

        private void ObservePoints(float longitude, float latitude)
        {
            foreach (var pointIndex in new List<int>(this.observedPoints.Keys))
            {
                var point = this.currentRoute.Points[pointIndex];
                if (this.currentRoute.IsBusAtPoint(pointIndex, longitude, latitude))
                {
                    if (point.Status == PointStatus.Entry && this.currentGpsData.IsStopped)
                    {
                        this.HandleStatusChangeAtPoint(pointIndex, PointStatus.StopIn);
                    }
                }
                else if (!this.currentGpsData.IsStopped)
                {
                    // we leave the point
                    if (point.Status == PointStatus.Entry || point.Status == PointStatus.StopIn)
                    {
                        this.HandleStatusChangeAtPoint(pointIndex, PointStatus.Done);
                    }

                    this.observedPoints.Remove(pointIndex);
                }
            }
        }

        private bool IsCandidateInTime(int cand, DateTime dateTime, Algorithm? algo)
        {
            var time = dateTime.TimeOfDay;
            if (!this.IsCorrectDay(dateTime))
            {
                time += TimeSpan.FromDays(1);
            }

            TimeSpan minusTime;
            TimeSpan plusTime;
            var startTime = this.trips[cand].StartTime;

            if (!algo.HasValue)
            {
                // replacement for IsCandidatInTimeframe2()
                minusTime = startTime + this.userParameters.TimeMinusValidated;
                plusTime = startTime - this.userParameters.TimePlusValidated;
            }
            else if (algo == Algorithm.LineOnly)
            {
                // Use Line timeframe
                minusTime = startTime + this.userParameters.TimeMinusLine;
                plusTime = startTime - this.userParameters.TimePlusLine;
            }
            else
            {
                // Use Service timeframe
                minusTime = startTime + this.userParameters.TimeMinusService;
                plusTime = startTime - this.userParameters.TimePlusService;
            }

            return minusTime > time && plusTime < time;
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void HandleStatusChangeAtPoint(int index, PointStatus newStatus)
        {
            var point = this.currentRoute.Points[index];
            if (point.Status == newStatus)
            {
                // Le statut n'a pas changé, on n'aurait pas du passer ici
                return;
            }

            point.Status = newStatus;
            switch (newStatus)
            {
                case PointStatus.Entry:
                    if (index == 0)
                    {
                        Logger.Info("Arriving at departure ({0})", point.Id);
                    }
                    else if (index == this.currentRoute.GetTerminusIndex())
                    {
                        Logger.Info("Arriving at terminal ({0})", point.Id);
                        if (!this.CurrentService.IsValidated)
                        {
                            // Si le service n'est pas validé c'est le moment de vérifier
                            if (this.currentRoute.Points[0].Status == PointStatus.Done)
                            {
                                // On est passé aux 2 terminus
                                this.CurrentService.IsValidated = true;
                                Logger.Debug("Block validated");

                                // Dans le cadre de l'algo 2, maintenant on a plus qu'un seul service
                                // on rejoint l'algo 1
                                // il faut purger les services chargés en trop
                                if (this.algorithm == Algorithm.LineOnly)
                                {
                                    var currService = this.trips[this.candidate].ServiceNumber;
                                    this.CurrentService.Services.Clear();
                                    this.CurrentService.Services.Add(new ServiceInfo { ServiceNumber = currService });
                                    this.SavePersistence();

                                    foreach (var trip in this.trips)
                                    {
                                        if (trip.ServiceNumber != currService)
                                        {
                                            trip.ServiceNumber = -1;
                                        }
                                    }

                                    Logger.Debug("Remaining candidates after validation");
                                    this.LogTripList();
                                    this.algorithm = Algorithm.LineService;
                                    this.serviceNumber = currService;
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.Debug("Arriving at point {0} ({1})", index, point.Id);
                    }

                    this.HandleVe4EventEntry(this.currentRoute, this.candidate, index);

                    if (index == this.currentRoute.GetTerminusIndex())
                    {
                        this.SendTripEndedMessage();
                        this.SendTripMessage(0, 0, TimeSpan.Zero, null);
                        this.UnloadTripAndTakeNext();
                    }

                    break;

                case PointStatus.StopIn:

                    // le bus est arreté dans le point
                    if (index == 0)
                    {
                        Logger.Debug("Stopping at departure ({0})", point.Id);
                    }
                    else if (index == this.currentRoute.GetTerminusIndex())
                    {
                        Logger.Debug("Stopping at terminus ({0})", point.Id);
                    }
                    else
                    {
                        Logger.Debug("Stopping at point {0} ({1})", index, point.Id);
                    }

                    ////this.HandleVe4EventStopIn(this.currentRoute, this.candidate, index);
                    break;

                case PointStatus.Done:

                    // le bus quitte le point
                    if (index == 0)
                    {
                        if (this.algorithm == Algorithm.LineOnly)
                        {
                            // Indicate to icenter motion that the service started (duty on)
                            this.SendEciDutyMessage(this.trips[this.candidate].ServiceNumber, 'r', 9999, -1, 0);
                        }

                        Logger.Debug("Leaving from departure ({0})", point.Id);
                        this.currentTerminus = 0;

                        // Si le service est validé, on checke le timeframe #2
                        // bugz #34
                        if (this.CurrentService.IsValidated
                            && !this.IsCandidateInTime(this.candidate, TimeProvider.Current.Now, null))
                        {
                            // D. Lenz 20/02/2008
                            // si on n'est pas dans la fourchette validée alors on recommence tout
                            var trip = this.trips[this.candidate];
                            Logger.Warn(
                                "Candidate rejected (timeframe 2) [{0}]"
                                + " - {1} - block:{2} - line:{3} - trip:{4} - route:{5}",
                                this.candidate + 1,
                                trip.StartTime,
                                trip.ServiceNumber,
                                trip.LineNumber,
                                trip.TripId,
                                trip.RouteId);

                            this.ReInit();
                            this.ClearServices();
                            this.CurrentTripStatus = new CurrentTripStatus();
                            this.CurrentService = new CurrentServiceInfo();
                            return;
                        }

                        this.CandidateStatus = CandidateStatus.Valid;
                    }
                    else
                    {
                        Logger.Debug("Leaving point {0} ({1})", index, point.Id);
                    }

                    if (point.Type == PointType.Stop && point.TheoreticalPassageTime != TimeSpan.MinValue)
                    {
                        var theo = this.trips[this.candidate].StartTime + point.TheoreticalPassageTime;
                        var now = TimeProvider.Current.Now;
                        var time = now.TimeOfDay;
                        if (!this.IsCorrectDay(now))
                        {
                            time += TimeSpan.FromDays(1);
                        }

                        this.delay = time - theo;
                    }

                    this.HandleVe4EventDone(this.currentRoute, this.candidate, index, this.delay);
                    this.SaveTripStatus(index);
                    break;

                case PointStatus.Timeout:
                    if (index == this.currentRoute.GetTerminusIndex())
                    {
                        Logger.Log(
                            this.CurrentService.IsValidated ? LogLevel.Error : LogLevel.Warn,
                            "Allowed time expired");
                        this.UnloadTripAndTakeNext();
                    }
                    else
                    {
                        Logger.Debug("Time-out at point {0} ({1})", index, point.Id);
                        this.annoucement.Remove(index);

                        // Timeouts detected on number of points defined in paramiti => hors-iti
                        if (this.CandidateStatus != CandidateStatus.OutsideSchedule && this.TestItiOut(index))
                        {
                            Logger.Info("Time-outs on {0} points", this.userParameters.ItiOut);
                            if (this.algorithm == Algorithm.LineService)
                            {
                                this.CandidateStatus = CandidateStatus.OutsideSchedule;
                            }
                        }
                    }

                    break;

                case PointStatus.OutsideSchedule:
                    Logger.Debug("Out of schedule at point {0} ({1})", index, point.Id);
                    break;
            }
        }

        private bool TestItiIn(int pointIndex)
        {
            var itiIn = this.userParameters.ItiIn;
            if (itiIn < 1)
            {
                return false;
            }

            if (pointIndex <= itiIn - 2)
            {
                return false;
            }

            for (int i = pointIndex - (itiIn - 1); i < pointIndex - 1; i++)
            {
                if (this.currentRoute.Points[i].Status != PointStatus.OutsideSchedule)
                {
                    return false;
                }
            }

            return true;
        }

        private bool TestItiOut(int index)
        {
            if (this.userParameters.ItiOut < 1)
            {
                return false;
            }

            if (index < this.userParameters.ItiOut - 1)
            {
                return false;
            }

            for (var i = index - (this.userParameters.ItiOut - 1); i <= index - 1; i++)
            {
                if (this.currentRoute.Points[i].Status != PointStatus.Timeout)
                {
                    return false;
                }
            }

            return true;
        }

        private void SaveTripStatus(int index)
        {
            this.CurrentTripStatus = new CurrentTripStatus
                                         {
                                             Time = TimeProvider.Current.Now,
                                             StopIndex = index,
                                             CandidateCourseId =
                                                 this.candidate >= 0 ? this.trips[this.candidate].TripId : -1
                                         };
            this.SavePersistence();
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void HandleVe4EventEntry(RouteInfo route, int cand, int pointIndex)
        {
            if (this.deviationFromMedi)
            {
                return;
            }

            var point = route.Points[pointIndex];
            if (point.Type == PointType.Stop)
            {
                this.SendBusStopEntryMessage(route.GetStopIndex(pointIndex));

                // COS: 18 October 2010
                // I arrive at this point whenever the bus does arrive at a bus stop.
                // Now, the bus stop's ID must be matched here with the informations
                // stored into the POI.csv file in order to get the speech media type
                // for this station.
                if (!this.isSpeechCallActive)
                {
                    this.SendSpeechMediaTypeMessage(route.GetStopIndex(pointIndex), point.VoiceType);
                }

                if (pointIndex == 0)
                {
                    this.SendPosition(
                        this.sendStopMessageEci ? PositionEvent.AtDeparture : PositionEvent.Gps, point.Id);
                }
                else if (pointIndex == route.GetTerminusIndex())
                {
                    this.SendPosition(this.sendStopMessageEci ? PositionEvent.AtTerminus : PositionEvent.Gps, point.Id);
                }
                else
                {
                    this.SendPosition(this.sendStopMessageEci ? PositionEvent.StopEntry : PositionEvent.Gps, point.Id);
                }
            }
            else if (point.Type == PointType.TrafficLight)
            {
                // 2 methodes pour la gestion des feux:
                // soit diaser géré uniquement par bus.exe avec remontée d'info sur le pupitre (SendEHTrameTrafficLight)
                // soit lsacontrol, géré en c# (SendEHTrameTrafficLightCore)
                switch ((PointSubTypeTrafficLight)point.SubType)
                {
                    case PointSubTypeTrafficLight.Entry:
                        var checkPoint1 = route.GetNextBufferByType(
                            pointIndex,
                            PointType.TrafficLight,
                            (int)PointSubTypeTrafficLight.CheckPoint,
                            point.Data);
                        var checkPoint2 = route.GetNextBufferByType(
                            checkPoint1,
                            PointType.TrafficLight,
                            (int)PointSubTypeTrafficLight.CheckPoint,
                            point.Data);
                        var checkPoint3 = route.GetNextBufferByType(
                            checkPoint2,
                            PointType.TrafficLight,
                            (int)PointSubTypeTrafficLight.CheckPoint,
                            point.Data);
                        var trafficLightPos = route.GetNextBufferByType(
                            pointIndex,
                            PointType.TrafficLight,
                            (int)PointSubTypeTrafficLight.Position,
                            point.Data);

                        const float SpeedIncrement = 0.14f; // 0.5 km/h in m/s
                        var dist1 = route.CalculateDistanceBetweenPoints(checkPoint1, trafficLightPos);
                        var time1 = route.CalculateTimeBetweenPoints(
                            checkPoint1,
                            trafficLightPos,
                            this.currentGpsData.Speed + SpeedIncrement);

                        var dist2 = route.CalculateDistanceBetweenPoints(checkPoint2, trafficLightPos);
                        var time2 = route.CalculateTimeBetweenPoints(
                            checkPoint2,
                            trafficLightPos,
                            this.currentGpsData.Speed + SpeedIncrement);

                        var dist3 = route.CalculateDistanceBetweenPoints(checkPoint3, trafficLightPos);
                        var time3 = route.CalculateTimeBetweenPoints(
                            checkPoint3,
                            trafficLightPos,
                            this.currentGpsData.Speed + SpeedIncrement);

                        this.SendTrafficLightMessage(TrafficLightState.Requested, time1 + time2 + time3);

                        // on envoi les 2 trames, suivant l'equipement, seule l'une sera traitée
                        this.SendTrafficLightCoreMessage(
                            TrafficLightPosition.POINT_SSTYPE_TRAFICLIGHT_ENTRY,
                            point.Data);

                        var msgEntry = new EciTrafficLightEntry
                                           {
                                               IntersectionId = point.Data,
                                               RouteId = this.trips[this.currentTrip].RouteId,
                                               Time = time1,
                                               Time2 = time2,
                                               Time3 = time3,
                                               Distance = dist1,
                                               Distance2 = dist2,
                                               Distance3 = dist3
                                           };
                        this.SendEciTrafficLightMessage(msgEntry);
                        Logger.Debug(
                            "Traffic Light: Entry to zone {0} (iti {1})", point.Data, this.trips[cand].RouteId);
                        break;
                    case PointSubTypeTrafficLight.CheckPoint:
                        trafficLightPos = route.GetNextBufferByType(
                            pointIndex,
                            PointType.TrafficLight,
                            (int)PointSubTypeTrafficLight.Position,
                            point.Data);

                        dist1 = route.CalculateDistanceBetweenPoints(pointIndex, trafficLightPos);
                        time1 = route.CalculateTimeBetweenPoints(
                            pointIndex,
                            trafficLightPos,
                            this.currentGpsData.Speed + SpeedIncrement);

                        this.SendTrafficLightCoreMessage(
                            TrafficLightPosition.POINT_SSTYPE_TRAFICLIGHT_CHECKPOINT,
                            point.Data);
                        var msgCheckPoint = new EciTrafficLightCheckPoint
                                                {
                                                    IntersectionId = point.Data,
                                                    Distance = dist1,
                                                    Time = time1
                                                };
                        this.SendEciTrafficLightMessage(msgCheckPoint);
                        Logger.Debug("Traffic Light: Checkpoint {0} (iti {1})", point.Data, this.trips[cand].RouteId);
                        break;
                    case PointSubTypeTrafficLight.Position:
                        this.SendTrafficLightCoreMessage(TrafficLightPosition.POINT_SSTYPE_TRAFICLIGHT_POS, point.Data);
                        Logger.Debug(
                            "Traffic Light: Reached position {0} (iti {1})",
                            point.Data,
                            this.trips[cand].RouteId);
                        break;
                    case PointSubTypeTrafficLight.Exit:
                        this.SendTrafficLightCoreMessage(
                            TrafficLightPosition.POINT_SSTYPE_TRAFICLIGHT_EXIT, point.Data);
                        var msgExit = new EciTrafficLightExit { IntersectionId = point.Data };
                        this.SendEciTrafficLightMessage(msgExit);
                        this.SendTrafficLightMessage(TrafficLightState.Inactive, TimeSpan.Zero);

                        Logger.Debug(
                            "Traffic Light: Exit position {0} (iti {1})", point.Data, this.trips[cand].RouteId);
                        break;
                }
            }

            if (this.currentZone != point.Zone)
            {
                this.currentZone = point.Zone;
                this.SendZoneChangedMessage(point.Zone);
            }
        }

        private void HandleVe4EventDone(RouteInfo route, int cand, int pointIndex, TimeSpan del)
        {
            if (this.deviationFromMedi)
            {
                return;
            }

            var point = route.Points[pointIndex];
            if (point.Type == PointType.Stop)
            {
                this.SendBusStopExitMessage(route.GetStopIndex(pointIndex));
                this.SendPosition(this.sendStopMessageEci ? PositionEvent.StopLeft : PositionEvent.Gps, point.Id);
            }

            var currentDelay = (int)del.TotalMinutes;
            if (this.sentDelay != currentDelay)
            {
              var msg = new EciDelayedMessage(
                    this.VehicleId,
                    this.trips[cand].ServiceNumber,
                    this.trips[cand].RouteId,
                    this.trips[cand].LineNumber,
                    route.Points[pointIndex].Id,
                    del);
                this.centerConnection.SendEciMessage(msg);
                this.SendAdvDelayMessage(del);
                this.sentDelay = currentDelay;
            }

            this.annoucement.Add(route, pointIndex, this.userParameters);
        }

        private void LogTripList()
        {
            // was ShowListVoyages() in old code
            Logger.Debug("Vehicle Trips:");
            var index = 1;
            foreach (var trip in this.trips)
            {
                Logger.Debug(
                    "[{0}] - {1} - block:{2} - line:{3} - trip:{4} - route:{5} - group:{6}",
                    index++,
                    trip.StartTime,
                    trip.ServiceNumber,
                    trip.LineNumber,
                    trip.TripId,
                    trip.RouteId,
                    trip.TimeGroup);
            }
        }

        private void UnloadTripAndTakeNext()
        {
            this.CandidateStatus = CandidateStatus.ToLoad;
            if (this.CurrentService.IsValidated)
            {
                do
                {
                    this.candidate++;
                }
                while (this.candidate < this.trips.Count && this.trips[this.candidate].ServiceNumber == -1);

                Logger.Info("Unload current candidate and take next one");

                if (this.candidate >= this.trips.Count)
                {
                    Logger.Info("No more candidates, impossible to continue");
                    this.SendServiceEndedMessage();
                    this.SendServiceMessage(0, 0);
                    this.SendTripMessage(0, 0, TimeSpan.Zero, null);
                    this.candidate = -1;
                    this.hasErrors = true;
                    this.SaveTripStatus(-1);
                    return;
                }

                var trip = this.trips[this.candidate];
                Logger.Info(
                    "Next candidate [{0}] - {1} - block:{2} - line:{3} - trip:{4} - route:{5}",
                    this.candidate + 1,
                    trip.StartTime,
                    trip.ServiceNumber,
                    trip.LineNumber,
                    trip.TripId,
                    trip.RouteId);
                this.SaveTripStatus(-1);
            }
            else
            {
                Logger.Info("Unload current candidate and take next one");
                this.candidate = -1;
                this.SaveTripStatus(-1);
            }
        }

        private int LoadTripList()
        {
            // was ChargeListeVoyages() in old code
            if (this.trips == null)
            {
                return 0;
            }

            if (this.CurrentService.Services.Count == 0)
            {
                Logger.Debug("Block error, impossible to continue");
                this.hasErrors = true;
                return 0;
            }

            this.trips.Clear();
            foreach (var service in this.CurrentService.Services)
            {
                this.trips.AddRange(
                    this.dataAccess.LoadTrips(
                        this.currentDayType,
                        service.LineNumber,
                        service.ServiceNumber,
                        service.TripNumber));
            }

            Logger.Debug("{0} trips found", this.trips.Count);
            this.trips.Sort((a, b) => a.ServiceNumber != b.ServiceNumber ? 0 : a.StartTime.CompareTo(b.StartTime));

            // Arret de releve, mis a jour apres le tri pour etre bien sur le 1er trip du service
            for (var i = 0; i < this.CurrentService.Services.Count; i++)
            {
                if (this.CurrentService.Services[i].FirstStopIndex > 0)
                {
                    // [ABA] for me it seems to be correct, since first stop of a service is a first stop of a trip
                    this.trips[i].FirstStopIndex = this.CurrentService.Services[i].FirstStopIndex;
                }
            }

            if (this.trips.Count == 0)
            {
                // No trip found
                Logger.Error("Trip error, impossible to continue");
                this.hasErrors = true;
                return 0;
            }

            this.LogTripList();
            this.ReInitFindCandidateAlgorithm();
            this.tripsLoaded = true;
            return this.trips.Count;
        }

        private void LoadTrip()
        {
            // was ChargeItineraire() in the old code
            var trip = this.trips[this.candidate];
            Logger.Debug("Loading route: {0}", trip.RouteId);

            this.currentRoute = new RouteInfo();

            // Remove announcement if scheduled
            this.annoucement.Init();
            this.currentRoute = this.dataAccess.LoadRoute(trip.LineNumber, trip.RouteId, trip.TimeGroup);
            this.dataAccess.LoadVia(this.currentRoute, trip.LineNumber);
            this.dataAccess.LoadPoints(this.currentRoute);

            if (!this.CheckRoute(this.currentRoute))
            {
                this.hasErrors = true;
                return;
            }

            // Bugz #48 ... special destination code
            if (this.candidate + 1 < this.trips.Count)
            {
                // if special destination:
                if (this.currentRoute.Points.Find(p => p.SignCode >= 995 && p.SignCode <= 999) != null)
                {
                    var nextTrip = this.trips[this.candidate + 1];

                    RouteInfo nextRoute = this.dataAccess.LoadRoute(
                        nextTrip.LineNumber, nextTrip.RouteId, nextTrip.TimeGroup);
                    this.dataAccess.LoadVia(nextRoute, nextTrip.LineNumber);
                    this.dataAccess.LoadPoints(nextRoute);

                    if (nextRoute.Points.Count > 0)
                    {
                        var nextSignCode = nextRoute.Points[0].SignCode;
                        foreach (var point in this.currentRoute.Points)
                        {
                            if (point.SignCode >= 995 && point.SignCode <= 999)
                            {
                                point.SignCode = nextSignCode;
                            }
                        }
                    }
                }
            }

            this.observedPoints.Clear();
            this.CandidateStatus = CandidateStatus.Loaded;

            this.LogRoute(this.currentRoute);
        }

        private bool CheckRoute(RouteInfo route)
        {
            // was CheckItineraire() in old code
            if (route == null)
            {
                return false;
            }

            foreach (var point in route.Points)
            {
                // [WES 2015-07-09] changed from || to && for lat/long since just one being 0 is valid
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (point.Buffer < 3 || (point.Longitude == 0 && point.Latitude == 0) || point.Type == 0)
                {
                    Logger.Warn(
                        "Error in data: id {0} - type: {1} - sstype: {2} x={3} y={4} buffer:{5}",
                        point.Id,
                        point.Type,
                        point.SubType,
                        point.Longitude,
                        point.Latitude,
                        point.Buffer);
                    return false;
                }

                // ReSharper restore CompareOfFloatsByEqualityOperator
            }

            return true;
        }

        private void LogRoute(RouteInfo route)
        {
            Logger.Debug("Vehicle Route:");
            if (route == null)
            {
                return;
            }

            foreach (var point in route.Points)
            {
                Logger.Debug(
                    "id {0} - type: {1} - sstype: {2} - x={3} y={4} buffer:{5} ttheo:{6}",
                    point.Id,
                    point.Type,
                    point.SubType,
                    point.Longitude,
                    point.Latitude,
                    point.Buffer,
                    point.TheoreticalPassageTime);
            }
        }

        private void SearchCandidate()
        {
            // was ChercheCandidat() in the old code
            this.candidate = this.FindCandidate(TimeProvider.Current.Now, this.algorithm);
            if (this.candidate == -1)
            {
                return;
            }

            this.CandidateStatus = CandidateStatus.ToLoad;

            var trip = this.trips[this.candidate];
            Logger.Info(
                "Candidate found [{0}] - {1} - block:{2} - line:{3} - trip:{4} - route:{5}",
                this.candidate + 1,
                trip.StartTime,
                trip.ServiceNumber,
                trip.LineNumber,
                trip.TripId,
                trip.RouteId);
        }

        private int FindCandidate(DateTime now, Algorithm algo)
        {
            // was FindCandidat() in old code
            if (this.trips == null)
            {
                return -1;
            }

            var found = new List<int>();

            // On parcours en sens inverse pour favoriser le candidat le plus tardif
            for (int i = this.trips.Count - 1; i >= 0; i--)
            {
                var trip = this.trips[i];
                this.userParameters = this.dataAccess.LoadParamIti(trip.LineNumber);

                if (trip.ServiceNumber != -1 && this.IsCandidateInTime(i, now, algo))
                {
                    found.Add(i);
                }
            }

            if (found.Count == 0)
            {
                Logger.Error("Couldnt find a valid candidate");
                return -1;
            }

            if (this.candidateToTryIndex >= found.Count)
            {
                this.candidateToTryIndex = 0;
            }

            var foundCandidate = found[this.candidateToTryIndex];
            this.userParameters = this.dataAccess.LoadParamIti(this.trips[foundCandidate].LineNumber);
            this.candidateToTryIndex++;
            return foundCandidate;
        }

        private void HandleExtraService(float longitude, float latitude)
        {
            if (this.simpleStopList == null)
            {
                return;
            }

            // special mode where we only check every stop
            for (var i = 0; i < this.simpleStopList.Stops.Count; i++)
            {
                var stop = this.simpleStopList.Stops[i];
                if (this.simpleStopList.IsBusAtPoint(i, longitude, latitude))
                {
                    if (this.currentZone != stop.Zone && stop.Zone != 0)
                    {
                        this.currentZone = stop.Zone;
                        this.SendZoneChangedMessage(stop.Zone);
                    }
                }
            }
        }

        private void RestoreTripInfo()
        {
            var status = this.CurrentTripStatus;
            if (TimeProvider.Current.Now - status.Time >= TimeSpan.FromMinutes(30))
            {
                Logger.Debug("Reload trip failed - too late");
                return;
            }

            this.candidate = -1;
            for (int i = 0; i < this.trips.Count; i++)
            {
                if (status.CandidateCourseId == this.trips[i].TripId)
                {
                    this.candidate = i;
                }
            }

            this.busStopLastSaved = status.StopIndex;
            this.CandidateStatus = CandidateStatus.ToLoad;

            Logger.Debug("Reload trip #{0}", this.candidate);
        }

        private void ReInitFindCandidateAlgorithm()
        {
            this.candidateToTryIndex = 0;
        }

        private void AddService(int lineNumber, int service, int tripNumber, int firstStopIndex)
        {
            this.CurrentService.Services.Add(
                new ServiceInfo
                    {
                        LineNumber = lineNumber,
                        ServiceNumber = service,
                        TripNumber = tripNumber,
                        FirstStopIndex = firstStopIndex
                    });
        }

        private bool IsCurrentService(int service)
        {
            if (this.trips.Count <= this.candidate || this.candidate < 0)
            {
                return false;
            }

            return this.trips[this.candidate].ServiceNumber == service;
        }

        /// <summary>
        /// Checks if the driver has recorded trip. If not, then returns 0,
        /// else number of first service to drive.
        /// </summary>
        /// <returns>0 or service number of the first trip</returns>
        private int CalculateServiceFromDriverBlock()
        {
            var delta = double.MaxValue;
            if (this.driverTrips.Count == 0)
            {
                return 0;
            }

            var ret = 0;

            // il faut prendre le service le plus proche de l'heure actuelle
            // RD 20/08/2013
            var now = TimeProvider.Current.Now;

            foreach (var driverTrip in this.driverTrips)
            {
                var dd = (driverTrip.ReliefTime - now.TimeOfDay).TotalSeconds;

                // After midnight but before <seealso cref="DayStart"> add a day.
                if (!this.IsCorrectDay(now.Date))
                {
                    dd += OneDay.Seconds;
                }

                if (dd < delta)
                {
                    ret = driverTrip.Block;
                    delta = dd;
                }
            }

            return ret;
        }

        private void SendPosition(PositionEvent eventType, int stopId)
        {
            var msg = new EciPositionMessage
                          {
                              VehicleId = this.VehicleId,
                              PositionEvent = eventType,
                              StopId = stopId,
                              AlarmState = this.alarmState,
                              AlarmId = this.currentAlarmId,
                              BlockId = this.serviceNumber,
                              LineId = this.currentLine,
                              TripId = this.currentTrip
                          };
            var gps = this.currentGpsData;
            if (gps != null)
            {
                if (gps.SatelliteTimeUtc.HasValue)
                {
                    msg.TimeStamp = gps.SatelliteTimeUtc.Value;
                }

                if (gps.IsValid)
                {
                    msg.Direction = gps.Direction;
                    msg.GpsNumberSats = gps.SatelliteCount;
                    msg.Latitude = gps.Latitude;
                    msg.Longitude = gps.Longitude;
                    msg.Speed = gps.Speed;
                }
            }

            this.centerConnection.SendEciMessage(msg);
            this.lastSentPositionTicks = TimeProvider.Current.TickCount;
        }

        private void SendConfig()
        {
            // was SendEHTrameConfig() in old code
            MessageDispatcher.Instance.Broadcast(
                new ehConfig
                    {
                        DeviceId = this.GetMacAddress(),
                        VehicleId = this.VehicleId,
                        VehicleType = this.VehicleType,
                        ConfigType = this.ConfigType,
                        DayKind = this.GetDayType()
                    });
        }

        private void SendTripLoadedMessage(bool theoretic)
        {
            MessageDispatcher.Instance.Broadcast(new evTripLoaded(theoretic));
        }

        private void SendTripStartedMessage()
        {
            MessageDispatcher.Instance.Broadcast(new evTripStarted());
        }

        private void SendTripMessage(int tripId, int customerTripId, TimeSpan startTime, RouteInfo route)
        {
            // do not send any hour greater than 23:59:59 --> handling of times until service end.
            var hour = (int)startTime.TotalHours;
            hour %= 24;

            var now = TimeProvider.Current.Now;
            var date = now.Date;

            if (tripId != 0 || startTime > TimeSpan.Zero)
            {
                if (this.IsCorrectDay(now) && startTime >= OneDay)
                {
                    // A service is loaded before midnight but this service starts after midnight. So add one day
                    date += OneDay;
                }
                else if (!this.IsCorrectDay(now) && startTime < OneDay)
                {
                    // A service is loaded after midnight but this service started before midnight. So substract one day
                    date -= OneDay;
                }
            }

            var trip = new Trip
                           {
                               Id = tripId,
                               DateTimeStart = date + new TimeSpan(hour, startTime.Minutes, startTime.Seconds)
                           };
            if (route != null && route.Points.Count > 0)
            {
                trip.CustomerTripId = customerTripId;
                trip.RouteId = route.RouteId;
                trip.LineName = route.Points[0].LineName;
                trip.AnnonceExt = route.Points[0].ExteriorAnnouncement;
                trip.AnnonceExtTTS = route.Points[0].ExteriorAnnouncementTts;

                foreach (var point in route.Points)
                {
                    if (point.Type != PointType.Stop)
                    {
                        continue;
                    }

                    trip.Stop.Add(
                        new BusStop
                            {
                                Ort = point.Id,
                                OrtOrigin = point.OrtOrigin,
                                Name1 = point.Name1,
                                Name2 = point.Name2,
                                Name1TTS = point.Name1Tts,
                                Zone = (short)point.Zone,
                                SignCode = point.SignCode,
                                Announce = (short)point.InteriorAnnouncementMp3,
                                Direction = (short)point.Direction,
                                SecondsFromDeparture = (int)point.TheoreticalPassageTime.TotalSeconds,
                                Didok = point.Didok,
                                DruckName = point.DruckName
                            });
                }
            }

            MessageDispatcher.Instance.Broadcast(trip);
        }

        private void SendTripEndedMessage()
        {
            MessageDispatcher.Instance.Broadcast(new evTripEnded());
        }

        private void SendServiceMessage(int serviceUmlauf, int lineNumber)
        {
            MessageDispatcher.Instance.Broadcast(new Service { Line = lineNumber, Umlauf = serviceUmlauf });
        }

        private void SendServiceEndedMessage()
        {
            MessageDispatcher.Instance.Broadcast(new evServiceEnded());
        }

        private void SendZoneChangedMessage(int zoneId)
        {
            MessageDispatcher.Instance.Broadcast(new evZoneChanged(zoneId));
        }

        private void SendAdvDelayMessage(TimeSpan del)
        {
            MessageDispatcher.Instance.Broadcast(new evAdvDelay((int)del.TotalMinutes));
        }

        private void SendTrafficLightCoreMessage(TrafficLightPosition position, int pointId)
        {
            MessageDispatcher.Instance.Broadcast(new evTrafficLightCore(position, pointId));
        }

        private void SendTrafficLightMessage(TrafficLightState state, TimeSpan approachTime)
        {
            MessageDispatcher.Instance.Broadcast(new evTrafficLight(state, (int)approachTime.TotalSeconds));
        }

        private void SendSpeechMediaTypeMessage(int stopIndex, int mediaType)
        {
            MessageDispatcher.Instance.Broadcast(new evSpeechMediaType(stopIndex, (byte)mediaType));
        }

        private void SendBusStopEntryMessage(int stopIndex)
        {
            MessageDispatcher.Instance.Broadcast(new evBUSStopReached(stopIndex));
        }

        private void SendBusStopExitMessage(int stopIndex)
        {
            MessageDispatcher.Instance.Broadcast(new evBUSStopLeft(stopIndex));
        }

        private void SendDeviationDetectedMessage()
        {
            MessageDispatcher.Instance.Broadcast(new evDeviationDetected());
        }

        private void SendSetServiceAckMessage(bool success)
        {
            MessageDispatcher.Instance.Broadcast(new evSetServiceAck(success));
        }

        private DateTime GetEciTimeStamp()
        {
            var gps = this.currentGpsData;
            return gps == null || !gps.SatelliteTimeUtc.HasValue
                       ? TimeProvider.Current.UtcNow
                       : gps.SatelliteTimeUtc.Value;
        }

        private void SendEciTrafficLightMessage(EciTrafficLightBase msg)
        {
            msg.VehicleId = this.VehicleId;
            msg.RouteId = this.trips[this.currentTrip].RouteId;

            var gps = this.currentGpsData;
            if (gps != null && gps.IsValid)
            {
                msg.Speed = gps.Speed;
            }

            msg.GpsUtcTimeStamp = this.GetEciTimeStamp();

            this.centerConnection.SendEciMessage(msg);
        }

        private void SendEciDutyMessage(int service, char loginType, int driverId, int driverPin, int option)
        {
            var msg = new EciDutyMessage
                          {
                              VehicleId = this.VehicleId,
                              ServiceNumber = service,
                              TimeStamp = this.GetEciTimeStamp(),
                              LoginType = loginType,
                              DriverId = driverId,
                              DriverPin = driverPin,
                              Option = option
                          };
            this.centerConnection.SendEciMessage(msg);
        }

        private void EnqueueMessageHandler(ThreadStart handler)
        {
            // this posts the handler onto the main Run() loop
            // like that all those messages are handled in sequence with GPS (not in parallel)
            lock (this.messageHandlers)
            {
                this.messageHandlers.Add(handler);
                this.messageWait.Set();
            }
        }

        private void CenterConnectionOnMessageReceived(object sender, EciEventArgs e)
        {
            if (e.Message.VehicleId != this.VehicleId)
            {
                Logger.Debug("Got ECI message which was not meant for me: {0}", e.Message);
                return;
            }

            var ack = e.Message as EciAck;
            if (ack != null)
            {
                this.HandleEciAck(ack);
                return;
            }

            var duty = e.Message as EciDutyMessage;
            if (duty != null)
            {
                this.HandleEciDuty(duty);
                return;
            }

            var textMessage = e.Message as EciTextMessage;
            if (textMessage != null)
            {
                this.HandleEciTextMessage(textMessage);
                return;
            }

            var trafficLightAck = e.Message as EciTrafficLightAck;
            if (trafficLightAck != null)
            {
                this.HandleEciTrafficLightAck();
                return;
            }

            var util = e.Message as EciUtilMessage;
            if (util != null)
            {
                this.HandleEciUtil(util);
                return;
            }

            Logger.Info("Got ECI message which we don't handle: {0}", e.Message);
        }

        private void HandleEciAck(EciAck message)
        {
            switch (message.Type)
            {
                case AckType.KeepAlive:
                    this.eciKeepAliveTimeout = 0;
                    this.missingEciKeepAliveAcks = 0;
                    break;
                case AckType.Duty:
                    MessageDispatcher.Instance.Broadcast(new evDutyAck((evDutyAck.Acks)message.Value));
                    break;
                case AckType.AlarmReceived:
                    MessageDispatcher.Instance.Broadcast(new evDriverAlarmAck(AlarmAck.Received));
                    break;
                case AckType.AlarmConfirmed:
                    MessageDispatcher.Instance.Broadcast(new evDriverAlarmAck(AlarmAck.Confirmed));
                    break;
                case AckType.AlarmEnd:
                    MessageDispatcher.Instance.Broadcast(new evDriverAlarmAck(AlarmAck.Ended));
                    break;
            }
        }

        private void HandleEciDuty(EciDutyMessage message)
        {
            // NOTE: we don't support VM.s anymore with this code
            var duty = new evDuty();
            duty.PersonelId = message.DriverId.ToString();
            duty.Service = message.ServiceNumber.ToString();
            duty.Option = (byte)message.Option;
            switch (message.LoginType)
            {
                case 'o':
                    duty.Type = evDuty.Types.DutyOff;
                    break;
                case 'e':
                    duty.Type = evDuty.Types.DutyOnSpecialService;
                    break;
                case 'd':
                    duty.Type = evDuty.Types.DutyOnDriver;
                    break;
                case 'r':
                    duty.Type = evDuty.Types.DutyOnRegular;
                    break;
            }

            MessageDispatcher.Instance.Broadcast(duty);
        }

        private void HandleEciTextMessage(EciTextMessage message)
        {
            if ((message.Target & MessageTarget.Driver) != 0)
            {
                var msg = new evMessage
                              {
                                  MessageId = message.MessageId,
                                  MessageType = evMessage.Types.Text,
                                  MessageText = message.DisplayText,
                                  Destination = evMessage.Destinations.Driver
                              };
                switch (message.SubType)
                {
                    case 'a':
                        msg.SubType = evMessage.SubTypes.Instruction;
                        break;
                    default:
                        msg.SubType = evMessage.SubTypes.Info;
                        break;
                }

                MessageDispatcher.Instance.Broadcast(msg);
            }

            if ((message.Target & (MessageTarget.Display | MessageTarget.Speaker)) != 0)
            {
                var ttsFrame = new evTTSFrame
                                   {
                                       MessageId = message.MessageId,
                                       Duration = (short)message.Duration.TotalMinutes,
                                       CycleTime = (short)message.CycleTime.TotalMinutes,
                                       TotalDuration = (short)message.TotalDuration.TotalMinutes,
                                       DisplayText = message.DisplayText,
                                       SpeechText = message.TtsText
                                   };
                if ((message.Target & MessageTarget.Display) != 0)
                {
                    ttsFrame.OutDst = evTTSFrame.OutDest.Display;
                }

                if ((message.Target & MessageTarget.Speaker) != 0)
                {
                    ttsFrame.OutDst = evTTSFrame.OutDest.Speaker;
                }

                MessageDispatcher.Instance.Broadcast(ttsFrame);

                if (message.MessageMp3 != 0)
                {
                    MessageDispatcher.Instance.Broadcast(new evAnnouncement(message.MessageMp3));
                }
            }

            this.centerConnection.SendEciMessage(
                new EciAckTs(this.VehicleId, this.GetEciTimeStamp(), 't', 1, message.MessageId));
        }

        private void HandleEciTrafficLightAck()
        {
            this.SendTrafficLightMessage(TrafficLightState.Received, TimeSpan.Zero);
        }

        private void HandleEciUtil(EciUtilMessage message)
        {
            switch (message.SubType)
            {
                case EciRequestCode.RequestReboot:
                    SystemManagerClient.Instance.Reboot("Requested by ECI server");
                    break;
                case EciRequestCode.RequestUpdate:
                    // TODO: handle update request
                    Logger.Info("Got update request over ECI");
                    break;
                case EciRequestCode.InitDuty:
                    MessageDispatcher.Instance.Broadcast(new evMaintenance(evMaintenance.Types.InitDuty));
                    break;
                case EciRequestCode.InitAlarm:
                    MessageDispatcher.Instance.Broadcast(new evMaintenance(evMaintenance.Types.InitAlarm));
                    break;
            }
        }

        private void HandleSetService(object sender, MessageEventArgs<evSetService> e)
        {
            this.EnqueueMessageHandler(() => this.HandleSetService(e.Message));
        }

        private void HandleSetService(evSetService message)
        {
            if (this.currentGpsData != null && this.currentGpsData.IsValid)
            {
                this.ReadDayType();
            }

            var block = message.Umlauf;
            if (block == -9)
            {
                // service agent
                block = this.CalculateServiceFromDriverBlock();
            }

            if (block > 0)
            {
                if (this.IsCurrentService(block))
                {
                    // on est déjà sur ce service on se contente d'envoyer la description au eventhandler
                    if (this.trips.Count > this.candidate && this.candidate >= 0)
                    {
                        var trip = this.trips[this.candidate];
                        this.SendTripMessage(trip.TripId, trip.CustomerTripId, trip.StartTime, this.currentRoute);
                        this.SendServiceMessage(trip.ServiceNumber, trip.LineNumber);
                        this.SendSetServiceAckMessage(true);
                    }
                }
                else
                {
                    Logger.Info("SetService {0}", block);
                    this.ClearServices();

                    if (this.driverTrips.Count > 0)
                    {
                        foreach (var driverTrip in this.driverTrips)
                        {
                            if (driverTrip.Block == block)
                            {
                                this.AddService(0, driverTrip.Block, driverTrip.Trip, driverTrip.Relief1Index);
                            }
                        }
                    }
                    else
                    {
                        this.AddService(0, block, 0, 0);
                    }

                    this.SetServiceState(ServiceState.End);
                    this.ReInit();

                    // Read trip list here to give a fast answer to the terminal
                    // We only want to check if the requested service exists
                    if (this.LoadTripList() > 0)
                    {
                        var trip = this.trips[0];
                        this.SendServiceMessage(trip.ServiceNumber, trip.LineNumber);
                        this.serviceNumber = trip.ServiceNumber;
                        this.SendSetServiceAckMessage(true);
                    }
                    else
                    {
                        this.SendSetServiceAckMessage(false);
                    }
                }
            }
            else
            {
                Logger.Info("Clear Service");
                this.ClearServices();
                this.SetServiceState(ServiceState.End);
                this.ReInit();
            }
        }

        private void HandleDeviationStarted(object sender, MessageEventArgs<evDeviationStarted> e)
        {
            this.EnqueueMessageHandler(() => this.deviationFromMedi = true);
        }

        private void HandleDeviationEnded(object sender, MessageEventArgs<evDeviationEnded> e)
        {
            this.EnqueueMessageHandler(() => this.deviationFromMedi = false);
        }

        private void HandleServiceStarted(object sender, MessageEventArgs<evServiceStarted> e)
        {
            this.EnqueueMessageHandler(() => this.HandleServiceStarted(e.Message));
        }

        private void HandleServiceStarted(evServiceStarted message)
        {
            this.sendStopMessageEci = !message.ExtensionCourse && !message.School;

            if (!message.ExtraService)
            {
                return;
            }

            Logger.Info("ExtraService");
            this.ClearServices();
            this.SetServiceState(ServiceState.End);
            this.ReInit();
        }

        private void HandleDriverAlarm(object sender, MessageEventArgs<evDriverAlarm> e)
        {
            this.EnqueueMessageHandler(() => this.HandleDriverAlarm(e.Message));
        }

        private void HandleDriverAlarm(evDriverAlarm message)
        {
            this.SetPositionAlarmState(message.State, message.AlarmID);
        }

        private void SetPositionAlarmState(AlarmState state, int alarmId)
        {
            this.currentAlarmId = alarmId & 0x0F;
            switch (state)
            {
                case AlarmState.Ended:
                case AlarmState.Inactive:
                    this.alarmState = EciAlarmState.Free;
                    break;
                case AlarmState.Reported:
                    this.alarmState = EciAlarmState.StartCall;
                    break;
                case AlarmState.Received:
                    this.alarmState = EciAlarmState.Ack;
                    break;
                case AlarmState.Confirmed:
                    this.alarmState = EciAlarmState.Keep;
                    break;
            }

            this.SendPosition(PositionEvent.Gps, 0);
        }

        private void HandleDuty(object sender, MessageEventArgs<evDuty> e)
        {
            this.EnqueueMessageHandler(() => this.HandleDuty(e.Message));
        }

        private void HandleDuty(evDuty message)
        {
            int service;
            int personelId;
            int personelPin;
            ParserUtil.TryParse(message.Service, out service);
            ParserUtil.TryParse(message.PersonelId, out personelId);
            ParserUtil.TryParse(message.PersonelPin, out personelPin);

            char loginType;
            switch (message.Type)
            {
                case evDuty.Types.DutyOnRegular:
                    loginType = 'r';
                    break;
                case evDuty.Types.DutyOnDriver:
                    loginType = 'R';
                    break;
                case evDuty.Types.DutyOnSpecialService:
                    loginType = 'e';
                    break;
                case evDuty.Types.DutyOff:
                    loginType = 'o';
                    break;
                default:
                    Logger.Debug("Duty type not supported: {0}", message.Type);
                    return;
            }

            if (loginType == 'R')
            {
                loginType = 'r';
                if (this.ReadDriverBlocks(message.Service))
                {
                    service = this.CalculateServiceFromDriverBlock();
                    Logger.Info("Prise de service agent: {0} (bloc {1})", message.Service, service);
                }
                else
                {
                    service = 0;
                }
            }

            this.SendEciDutyMessage(service, loginType, personelId, personelPin, message.Option);
            this.CurrentService.DriverId = personelId;
        }

        private bool ReadDriverBlocks(string name)
        {
            return this.ReadDriverBlocksDayKind(name, this.currentDayType);
        }

        private bool ReadDriverBlocksDayKind(string name, int dayType)
        {
            this.driverTrips.Clear();
            this.driverTrips.AddRange(this.dataAccess.LoadDriverTrips(name, dayType));
            return this.driverTrips.Count > 0;
        }

        private void HandleMessageAck(object sender, MessageEventArgs<evMessageAck> e)
        {
            this.EnqueueMessageHandler(() => this.HandleMessageAck(e.Message));
        }

        private void HandleMessageAck(evMessageAck message)
        {
            var msg = new EciAckTs(
                this.VehicleId,
                this.GetEciTimeStamp(),
                't',
                2,
                message.MessageId);
            this.centerConnection.SendEciMessage(msg);
        }

        private void HandlePassengerCount(object sender, MessageEventArgs<evPassengerCount> e)
        {
            this.EnqueueMessageHandler(() => this.HandlePassengerCount(e.Message));
        }

        private void HandlePassengerCount(evPassengerCount message)
        {
            // [WES]: we can't use passenger count since this would involve changing ECIx,
            // so we use "new message" instead
            var msg = new EciNewMessage
                          {
                              VehicleId = this.VehicleId,
                              LineNumber = message.LineNumber,
                              ServiceNumber = message.BlockNumber,
                              RouteId = message.RoutePathNumber,
                              PositionId = message.DriverNumber,
                              PositionType = 'p',
                              VehicleType = message.PassengerCount
                          };
            this.centerConnection.SendEciMessage(msg);
        }

        private void HandleSpeechConnected(object sender, MessageEventArgs<evSpeechConnected> e)
        {
            this.EnqueueMessageHandler(() => this.isSpeechCallActive = true);
        }

        private void HandleSpeechDisconnected(object sender, MessageEventArgs<evSpeechDisconnected> e)
        {
            this.EnqueueMessageHandler(() => this.isSpeechCallActive = false);
        }

        private int GetMacAddress()
        {
            var netIfs = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var netIf in netIfs)
            {
                if (netIf.NetworkInterfaceType == NetworkInterfaceType.Loopback
                    || netIf.IsReceiveOnly)
                {
                    continue;
                }

                var addressBytes = netIf.GetPhysicalAddress().GetAddressBytes();
                if (addressBytes.Length != 6)
                {
                    continue;
                }

                int addressInt = 0;
                for (int i = 3; i < 6; i++)
                {
                    addressInt *= 256;
                    addressInt += Convert.ToInt32(string.Format("{0:X2}", addressBytes[i]), 16);
                }

                return addressInt;
            }

            throw new NotSupportedException("Couldn't determine hostname from MAC address");
        }

        private class AbortLoopException : Exception
        {
        }
    }
}
