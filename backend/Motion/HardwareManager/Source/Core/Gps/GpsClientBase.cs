// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsClientBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Gps
{
    using Gorba.Common.Configuration.HardwareManager.Gps;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Common.Entities.Gps;

    /// <summary>
    /// Base class for all GPS receivers.
    /// </summary>
    public abstract class GpsClientBase
    {
        private readonly SimplePort gpsCoverage;

        private bool isRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsClientBase"/> class.
        /// Subclasses should call <see cref="SendGpsData"/> whenever they have new data available.
        /// </summary>
        protected GpsClientBase()
        {
            this.gpsCoverage = new SimplePort("GpsCoverage", true, false, new FlagValues(), FlagValues.False);
        }

        /// <summary>
        /// Creates a <see cref="GpsClientBase"/> for the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The GPS receiver configuration.
        /// </param>
        /// <returns>
        /// The <see cref="GpsClientBase"/> subclass or null if it is not found or not enabled.
        /// </returns>
        public static GpsClientBase Create(GpsConfig config)
        {
            switch (config.ConnectionType)
            {
                case GpsConnectionType.GpsPilot:
                    {
                        if (config.Client == null || !config.Client.Enabled)
                        {
                            return null;
                        }

                        var gpsPilot = config.Client as GpsPilotConfig;
                        if (gpsPilot != null)
                        {
                            return new GpsPilotClient(gpsPilot);
                        }

                        break;
                    }

                case GpsConnectionType.GpsSerial:
                    {
                        if (config.GpsSerialClient == null || !config.GpsSerialClient.Enabled)
                        {
                            return null;
                        }

                        var gpsSerialClient = config.GpsSerialClient as GpsSerialPortConfig;
                        if (gpsSerialClient != null)
                        {
                            return new GpsSerialClient(gpsSerialClient);
                        }

                        break;
                    }
            }

            return null;
        }

        /// <summary>
        /// Starts this client.
        /// </summary>
        public void Start()
        {
            if (this.isRunning)
            {
                return;
            }

            this.isRunning = true;
            GioomClient.Instance.RegisterPort(this.gpsCoverage);
            this.DoStart();
        }

        /// <summary>
        /// Stops this client.
        /// </summary>
        public void Stop()
        {
            if (!this.isRunning)
            {
                return;
            }

            this.isRunning = false;
            GioomClient.Instance.DeregisterPort(this.gpsCoverage);
            this.DoStop();
        }

        /// <summary>
        /// Implementation of the start method.
        /// </summary>
        protected abstract void DoStart();

        /// <summary>
        /// Implementation of the stop method.
        /// </summary>
        protected abstract void DoStop();

        /// <summary>
        /// Sends the given GPS data through Medi to all receivers.
        /// </summary>
        /// <param name="gpsData">
        /// The GPS data.
        /// </param>
        protected void SendGpsData(GpsData gpsData)
        {
            this.gpsCoverage.Value = FlagValues.GetValue(gpsData.IsValid);
            MessageDispatcher.Instance.Broadcast(gpsData);
        }
    }
}
