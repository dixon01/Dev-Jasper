// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManualTimeProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManualTimeProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="TimeProvider"/> that allows to manually set and update the time.
    /// </summary>
    public class ManualTimeProvider : TimeProvider
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ManualTimeProvider>();

        private DateTime utcNow;

        private long tickCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualTimeProvider"/> class.
        /// </summary>
        /// <param name="utcNow">
        /// The start time (in UTC).
        /// </param>
        public ManualTimeProvider(DateTime utcNow)
        {
            this.utcNow = utcNow;
            this.tickCount = 123456;
        }

        /// <summary>
        /// Gets the UTC now.
        /// </summary>
        public override DateTime UtcNow
        {
            get
            {
                return this.utcNow;
            }
        }

        /// <summary>
        /// Gets the system tick count (see <see cref="System.Environment.TickCount"/>).
        /// </summary>
        public override long TickCount
        {
            get
            {
                return this.tickCount;
            }
        }

        /// <summary>
        /// Adds a timespan to <see cref="UtcNow"/> and <see cref="TickCount"/>.
        /// </summary>
        /// <param name="duration">
        /// The duration.
        /// </param>
        public void AddTime(TimeSpan duration)
        {
            this.utcNow += duration;
            this.tickCount += (long)duration.TotalMilliseconds;

            Logger.Info(
                "Added {0} seconds, simulated time is now {1}",
                duration.TotalSeconds,
                this.utcNow.ToLongTimeString());
        }

        /// <summary>
        /// Sets the <see cref="UtcNow"/>.
        /// </summary>
        /// <param name="now">
        /// The new time to set.
        /// </param>
        public void SetUtcNow(DateTime now)
        {
            this.utcNow = now;

            Logger.Info("Set simulated time to {0}", this.utcNow.ToLongTimeString());
        }
    }
}