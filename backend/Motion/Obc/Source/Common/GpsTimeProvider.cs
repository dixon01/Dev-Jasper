// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsTimeProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Common
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.Entities.Gps;

    /// <summary>
    /// Time provider that uses GPS (when available) to provide the current time.
    /// </summary>
    public class GpsTimeProvider : TimeProvider
    {
        private DateTime? gpsTime;

        private long gpsTicks;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsTimeProvider"/> class.
        /// </summary>
        public GpsTimeProvider()
        {
            MessageDispatcher.Instance.Subscribe<GpsData>(this.HandleGpsData);
        }

        /// <summary>
        /// Gets the UTC now.
        /// </summary>
        public override DateTime UtcNow
        {
            get
            {
                if (!this.gpsTime.HasValue)
                {
                    return DateTime.UtcNow;
                }

                var ticks = Environment.TickCount;
                return this.gpsTime.Value + TimeSpan.FromMilliseconds(ticks - this.gpsTicks);
            }
        }

        private void HandleGpsData(object sender, MessageEventArgs<GpsData> e)
        {
            if (!e.Message.IsValid || !e.Message.SatelliteTimeUtc.HasValue)
            {
                return;
            }

            var ticks = Environment.TickCount;
            this.gpsTime = new DateTime(e.Message.SatelliteTimeUtc.Value.Ticks, DateTimeKind.Utc);
            this.gpsTicks = ticks;
        }
    }
}
