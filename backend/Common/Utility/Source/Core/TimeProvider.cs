// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provides a wrapper around DateTime so that is is be possible to abstract and mock date and time.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    /// <summary>
    /// Provides a wrapper around DateTime so that is is be possible to abstract and mock date and time.
    /// </summary>
    public abstract class TimeProvider
    {
        /// <summary>
        /// Static reference to the current time provider.
        /// </summary>
        private static TimeProvider current = DefaultTimeProvider.Instance;

        /// <summary>
        /// Gets or sets the current time provider.
        /// </summary>
        /// <value>
        /// The current time provider.
        /// </value>
        public static TimeProvider Current
        {
            get
            {
                return current;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                current = value;
            }
        }

        /// <summary>
        /// Gets the UTC now.
        /// </summary>
        public abstract DateTime UtcNow { get; }

        /// <summary>
        /// Gets the local time.
        /// </summary>
        public virtual DateTime Now
        {
            get
            {
                return this.UtcNow.ToLocalTime();
            }
        }

        /// <summary>
        /// Gets the system tick count (see <see cref="Environment.TickCount"/>).
        /// </summary>
        public virtual long TickCount
        {
            get
            {
                return Environment.TickCount;
            }
        }

        /// <summary>
        /// Resets to default.
        /// </summary>
        public static void ResetToDefault()
        {
            current = DefaultTimeProvider.Instance;
        }

        /// <summary>
        /// Defines the default <see cref="TimeProvider"/> which uses the <see cref="DateTime.UtcNow"/> value.
        /// </summary>
        private sealed class DefaultTimeProvider : TimeProvider
        {
            private static TimeProvider instance;

            private int lastTickCount;

            private long tickCountOffset;

            public static TimeProvider Instance
            {
                get
                {
                    return instance ?? (instance = new DefaultTimeProvider());
                }
            }

            public override DateTime UtcNow
            {
                get
                {
                    return DateTime.UtcNow;
                }
            }

            public override DateTime Now
            {
                get
                {
                    return DateTime.Now;
                }
            }

            public override long TickCount
            {
                get
                {
                    var tickCount = Environment.TickCount & int.MaxValue;
                    lock (this)
                    {
                        if (this.lastTickCount - 1000 > tickCount)
                        {
                            // we wrapped around with the tick counter
                            this.tickCountOffset += 1L + int.MaxValue;
                        }

                        this.lastTickCount = tickCount;
                    }

                    return this.tickCountOffset + tickCount;
                }
            }
        }
    }
}
