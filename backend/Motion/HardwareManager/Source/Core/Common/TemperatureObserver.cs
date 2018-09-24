// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemperatureObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemperatureObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Class that is checking WMI to get and log temperature sensor readings.
    /// </summary>
    public partial class TemperatureObserver
    {
        private const int InfoLogMultiplier = 30; // write an "Info" log every 30 * 20 seconds = 10 minutes

        private static readonly Logger Logger = LogHelper.GetLogger<TemperatureObserver>();

        private readonly ITimer timer;

        private readonly SimplePort port;

        private int logCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemperatureObserver"/> class.
        /// </summary>
        public TemperatureObserver()
        {
            this.timer = TimerFactory.Current.CreateTimer("TemperatureObserver");
            this.timer.AutoReset = true;
            this.timer.Interval = TimeSpan.FromSeconds(20);
            this.timer.Elapsed += (sender, args) => this.QuerySensors();

            this.port = new SimplePort("Temperature", true, false, new IntegerValues(-275, 1000), 0);

            this.Initialize();
        }

        /// <summary>
        /// Starts this observer.
        /// </summary>
        public void Start()
        {
            try
            {
                this.QuerySensors();
                this.timer.Enabled = true;
                GioomClient.Instance.RegisterPort(this.port);
            }
            catch (Exception ex)
            {
                Logger.Warn("Couldn't start temperature observation, cause: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Stops this observer.
        /// </summary>
        public void Stop()
        {
            this.timer.Enabled = false;
            GioomClient.Instance.DeregisterPort(this.port);
        }

        partial void Initialize();

        partial void ReadSensors(ICollection<TemperatureInfo> sensors);

        private void QuerySensors()
        {
            Logger.Info("Query for Sensors...");
            var infos = new List<TemperatureInfo>();
            this.ReadSensors(infos);
            var logLevel = (this.logCounter++) % InfoLogMultiplier == 0 ? LogLevel.Info : LogLevel.Trace;
            var first = true;
            foreach (var info in infos)
            {
                Logger.Log(logLevel, "Sensor {0} has value {1} ({2:0.0}°C)", info.Name, info.RawValue, info.Degrees);
                if (first)
                {
                    // only report the first port found
                    this.port.IntegerValue = (int)info.Degrees;
                    first = false;
                }
            }
        }

        private class TemperatureInfo
        {
            public TemperatureInfo(string name, int rawValue, double degrees)
            {
                this.Name = name;
                this.RawValue = rawValue;
                this.Degrees = degrees;
            }

            public string Name { get; private set; }

            public int RawValue { get; private set; }

            public double Degrees { get; private set; }
        }
    }
}
