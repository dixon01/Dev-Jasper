// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core
{
    using System;
    using System.Text;

    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Ibis.Core.Providers;

    using NLog;

    /// <summary>
    /// Implementation of the IBIS context used by telegram providers.
    /// </summary>
    internal class IbisContext : IIbisContext, IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<IbisContext>();

        private readonly PortListener doorsOpen;

        private int currentStopId;

        private int currentTripId;

        private int currentZone;

        private bool razzia;

        private bool tripLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisContext"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public IbisContext(IbisConfig config)
        {
            this.Config = config;

            // Status data
            this.tripLoaded = false;
            this.razzia = false;
            this.IsTheBusDriving = false;
            this.IsDoorCycled = false;
            this.IsInStopBuffer = false;
            this.currentZone = config.Functionality.DefaultZoneNumber;
            this.currentTripId = 0;

            // Add events we're interested in
            MessageDispatcher.Instance.Subscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Subscribe<evTripEnded>(this.HandleTripEnded);
            MessageDispatcher.Instance.Subscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Subscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Subscribe<evServiceStarted>(this.HandleServiceStarted);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.HandleServiceEnded);
            MessageDispatcher.Instance.Subscribe<evRazziaStart>(this.HandleRazziaStart);
            MessageDispatcher.Instance.Subscribe<evRazziaEnded>(this.HandleRazziaEnded);
            MessageDispatcher.Instance.Subscribe<evZoneChanged>(this.HandleZoneChanged);
            MessageDispatcher.Instance.Subscribe<evBusDriving>(this.HandleBusDriving);

            ////MessageDispatcher.Instance.Subscribe<evComptageRequest>(HandleComptageRequest);

            this.doorsOpen = new PortListener(MediAddress.Broadcast, "DoorsOpen");
            this.doorsOpen.ValueChanged += this.DoorsOpenOnValueChanged;
            this.doorsOpen.Start(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Gets the entire IBIS configuration.
        /// </summary>
        public IbisConfig Config { get; private set; }

        /// <summary>
        /// Gets the current line number (used for DS001).
        /// </summary>
        public int Line
        {
            get
            {
                if (RemoteEventHandler.CurrentService != null)
                {
                    try
                    {
                        return RemoteEventHandler.CurrentService.Line;
                    }
                    catch (Exception ex)
                    {
                        Logger.WarnException("Couldn't get line information", ex);
                    }
                }

                return this.Config.Functionality.DefaultLineNumber;
            }
        }

        /// <summary>
        /// Gets the current Didok number (used for DS004b).
        /// </summary>
        public int Didok
        {
            get
            {
                return RemoteEventHandler.CurrentTrip != null
                       && RemoteEventHandler.CurrentTrip.Stop.Count > this.currentStopId
                           ? RemoteEventHandler.CurrentTrip.Stop[this.currentStopId].Didok
                           : 0;
            }
        }

        /// <summary>
        /// Gets the current stop name for the ticket printer (used for DS004c).
        /// </summary>
        public string DruckName
        {
            get
            {
                return RemoteEventHandler.CurrentTrip != null
                       && RemoteEventHandler.CurrentTrip.Stop.Count > this.currentStopId
                           ? RemoteEventHandler.CurrentTrip.Stop[this.currentStopId].DruckName
                           : string.Empty;
            }
        }

        /// <summary>
        /// Gets the current route number (used for DS002).
        /// </summary>
        public int RouteId
        {
            get
            {
                return RemoteEventHandler.CurrentTrip != null ? RemoteEventHandler.CurrentTrip.RouteId : 0;
            }
        }

        /// <summary>
        /// Gets the current destination code (used for DS003).
        /// </summary>
        public int Destination
        {
            get
            {
                try
                {
                    if (RemoteEventHandler.CurrentTrip != null)
                    {
                        return RemoteEventHandler.CurrentTrip.Stop[this.currentStopId].SignCode;
                    }

                    if (RemoteEventHandler.CurrentExtraService != null)
                    {
                        return RemoteEventHandler.CurrentExtraService.DestinationCode;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WarnException("GetDestination: CurrentTrip.Stop[].Ziel. ", ex);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the current razzia code (used for DS004a).
        /// </summary>
        public int RazziaCode
        {
            get
            {
                if (this.Config.Devices.TicketingConfig.Model == TicketingModel.Krauth)
                {
                    // code for Krauth
                    return this.razzia ? 0 : 100;
                }

                // code for Atron
                return this.razzia ? 0 : 3;
            }
        }

        /// <summary>
        /// Gets the current zone (used for DS004).
        /// </summary>
        public int CurrentZone
        {
            get
            {
                return this.CalculateCurrentZone();
            }
        }

        /// <summary>
        /// Gets a value indicating whether is inside the stop buffer.
        /// </summary>
        public bool IsInStopBuffer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is door cycled.
        /// This is true between closing of the door (in the buffer) and exiting the buffer.
        /// </summary>
        public bool IsDoorCycled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the bus is driving.
        /// </summary>
        public bool IsTheBusDriving { get; private set; }

        /// <summary>
        /// Gets a string representing all names of the given stop (used for DS021b).
        /// </summary>
        /// <param name="stop">
        /// The stop.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetStopNames(BusStop stop)
        {
            var names = new StringBuilder();

            try
            {
                names.Append(stop.Name1);
                if (!string.IsNullOrEmpty(stop.Name2))
                {
                    names.Append("/").Append(stop.Name2);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't get stop names", ex);
            }

            return names.ToString();
        }

        /// <summary>
        /// Gets the connection hints for a given stop index (used for DS021b).
        /// </summary>
        /// <param name="stopIndex">
        /// The stop index.
        /// </param>
        /// <returns>
        /// The connection hints as a string.
        /// </returns>
        public string GetConnectionHints(int stopIndex)
        {
            string hints = string.Empty;
            try
            {
                // get next HST pictures number
                // TO DO

                // get drive time since last stop
                if (RemoteEventHandler.CurrentTrip != null)
                {
                    int seconds;
                    if (stopIndex == 0)
                    {
                        seconds = RemoteEventHandler.CurrentTrip.Stop[stopIndex].SecondsFromDeparture;
                    }
                    else
                    {
                        seconds = RemoteEventHandler.CurrentTrip.Stop[stopIndex].SecondsFromDeparture
                                  - RemoteEventHandler.CurrentTrip.Stop[stopIndex - 1].SecondsFromDeparture;
                    }

                    int minutes = seconds / 60;
                    if (minutes > 0)
                    {
                        hints = "$" + minutes;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't get the chonnection hints", ex);
            }

            return hints;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            MessageDispatcher.Instance.Unsubscribe<evBUSStopLeft>(this.HandleBusStopLeft);
            MessageDispatcher.Instance.Unsubscribe<evTripEnded>(this.HandleTripEnded);
            MessageDispatcher.Instance.Unsubscribe<evTripLoaded>(this.HandleTripLoaded);
            MessageDispatcher.Instance.Unsubscribe<evBUSStopReached>(this.HandleBusStopReached);
            MessageDispatcher.Instance.Unsubscribe<evServiceStarted>(this.HandleServiceStarted);
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.HandleServiceEnded);
            MessageDispatcher.Instance.Unsubscribe<evRazziaStart>(this.HandleRazziaStart);
            MessageDispatcher.Instance.Unsubscribe<evRazziaEnded>(this.HandleRazziaEnded);
            MessageDispatcher.Instance.Unsubscribe<evZoneChanged>(this.HandleZoneChanged);
            MessageDispatcher.Instance.Unsubscribe<evBusDriving>(this.HandleBusDriving);

            ////MessageDispatcher.Instance.Unsubscribe<evComptageRequest>(HandleComptageRequest);

            this.doorsOpen.Dispose();
        }

        private int CalculateCurrentZone()
        {
            int zone = this.currentZone;
            try
            {
                if (RemoteEventHandler.CurrentService != null && RemoteEventHandler.CurrentTrip != null)
                {
                    zone = (RemoteEventHandler.CurrentService.Umlauf * 1000)
                           + RemoteEventHandler.CurrentTrip.Stop[this.currentStopId].Zone;
                }

                Logger.Debug("Zone: " + zone);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't get the zone information", ex);
            }

            return zone;
        }

        private void HandleServiceStarted(object sender, MessageEventArgs<evServiceStarted> e)
        {
            Logger.Info("Block Started event received");

            ////TransN.StartService();
        }

        private void HandleBusDriving(object sender, MessageEventArgs<evBusDriving> e)
        {
            this.IsTheBusDriving = e.Message.IsDriving;

            ////TransN.BusDriving(isDriving);
        }

        private void DoorsOpenOnValueChanged(object sender, EventArgs e)
        {
            var isOpen = FlagValues.True.Equals(this.doorsOpen.Value);
            Logger.Debug("Door changed: {0}", isOpen ? "opened" : "closed");

            if (this.IsInStopBuffer && !this.IsTheBusDriving && !isOpen)
            {
                this.IsDoorCycled = true;
            }

            ////TransN.OpenDoor(isOpened);
        }

        private void HandleRazziaEnded(object sender, MessageEventArgs<evRazziaEnded> e)
        {
            Logger.Info("RazziaEnded event received");
            this.razzia = false;
        }

        private void HandleRazziaStart(object sender, MessageEventArgs<evRazziaStart> e)
        {
            Logger.Info("RazziaStart event received");
            this.razzia = true;
        }

        private void HandleBusStopLeft(object sender, MessageEventArgs<evBUSStopLeft> e)
        {
            Logger.Info("BUSStopLeft event received - Stop: {0}", e.Message.StopId);

            // Reinitialise vars for next stop
            this.IsInStopBuffer = false;
            this.IsDoorCycled = false;
        }

        private void HandleTripLoaded(object sender, MessageEventArgs<evTripLoaded> e)
        {
            try
            {
                Logger.Info("TripLoaded event received, theoretic={0}", e.Message.Theoretic);
                if (!this.tripLoaded
                    || (this.tripLoaded && RemoteEventHandler.CurrentTrip != null
                        && this.currentTripId != RemoteEventHandler.CurrentTrip.Id))
                {
                    this.tripLoaded = true;
                    this.currentTripId = RemoteEventHandler.CurrentTrip.Id;
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't handle trip loaded event", ex);
            }
        }

        private void ResetTrip()
        {
            ////this.onDeviation = false;
            this.tripLoaded = false;
            this.currentTripId = 0;
            this.currentStopId = 0;
        }

        private void HandleTripEnded(object sender, MessageEventArgs<evTripEnded> e)
        {
            Logger.Info("TripEnded event received");
            this.ResetTrip();
        }

        private void HandleServiceEnded(object sender, MessageEventArgs<evServiceEnded> e)
        {
            Logger.Info("Service ended event received");
            this.ResetTrip();
        }

        private void HandleZoneChanged(object sender, MessageEventArgs<evZoneChanged> e)
        {
            Logger.Info("Zone Changed event received: {0}", e.Message.ZoneId);
            this.currentZone = e.Message.ZoneId;
        }

        private void HandleBusStopReached(object sender, MessageEventArgs<evBUSStopReached> e)
        {
            // Initialise now the values we may need in the buffer
            this.currentStopId = e.Message.StopId;
            this.IsInStopBuffer = true;
            this.IsDoorCycled = false;
        }
    }
}
