// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTimeSync.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisTimeSync type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.TimeSync
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Ibis.Handlers;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Time synchronization using DS005 with some config parameters.
    /// IMPORTANT: this class never uses DateTime.Now since the telegrams
    /// we send might end up changing the system time. The only valid time
    /// source is <see cref="TimeProvider.TickCount"/> which is a stable
    /// counter of milliseconds since the system start-up.
    /// </summary>
    public class IbisTimeSync : InputHandler<DS005>, IManageableTable
    {
        private const string DS006ADateTimeFormat = "ddMMyyyyHHmmss";

        private static readonly Logger Logger = LogHelper.GetLogger<IbisTimeSync>();

        private static readonly DateTime UnixZero = new DateTime(1970, 1, 1);

        private readonly Queue<ReceivedTimestamp> receivedTimes = new Queue<ReceivedTimestamp>();

        private readonly PortListener timePortListener;

        private TimeSyncConfig config;

        private DS006Config dateTelegramConfig;

        private long startTickCount;

        private ReceivedTimestamp lastUpdate;

        private DateTime? receivedDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisTimeSync"/> class.
        /// </summary>
        public IbisTimeSync()
            : base(1)
        {
            this.timePortListener = new PortListener(
                new MediAddress(MessageDispatcher.Instance.LocalAddress.Unit, "*"),
                "SystemTime");
            this.receivedDate = null;
        }

        /// <summary>
        /// Configures this object with the given configuration.
        /// </summary>
        /// <param name="cfg">
        /// The config.
        /// </param>
        /// <param name="dateConfig">
        /// The <see cref="DS006Config"/> used for handling of 5 digit dates.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public void Configure(TimeSyncConfig cfg, DS006Config dateConfig, IIbisConfigContext configContext)
        {
            this.Configure(configContext);
            this.config = cfg;
            this.dateTelegramConfig = dateConfig;
        }

        /// <summary>
        /// Starts the time synchronization.
        /// </summary>
        public void Start()
        {
            this.startTickCount = TimeProvider.Current.TickCount;
            this.timePortListener.Start(TimeSpan.FromSeconds(20));
        }

        /// <summary>
        /// Stops this time synchronization.
        /// </summary>
        public void Stop()
        {
            this.timePortListener.Dispose();
        }

        /// <summary>
        /// Check whether this object can handle the given event.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given event.
        /// </returns>
        public override bool Accept(Telegram telegram)
        {
            return (telegram is DS005 || telegram is DS006A || telegram is DS006) && this.config != null;
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        public override void HandleInput(Telegram telegram)
        {
            try
            {
                var ds005 = telegram as DS005;
                if (ds005 != null)
                {
                    this.HandleInput(ds005);
                    return;
                }

                var ds006 = telegram as DS006;
                if (ds006 != null)
                {
                    this.HandleInput(ds006);
                }

                var ds006A = telegram as DS006A;
                if (ds006A != null)
                {
                    this.HandleInput(ds006A);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not handle " + telegram.GetType().Name);
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var time in this.receivedTimes)
            {
                var value = TimeSpan.FromMilliseconds(time.TickCount);
                yield return
                    new List<ManagementProperty>
                        {
                            new ManagementProperty<string>("Uptime", value.ToString(), true),
                            new ManagementProperty<string>("Time", time.Time.ToString(), true),
                        };
            }
        }

        /// <summary>
        /// Handles the input event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// The input event.
        /// </param>
        protected override void HandleInput(DS005 telegram)
        {
            DateTime time;
            if (!DateTime.TryParseExact(
                    telegram.Time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
            {
                Logger.Warn("Invalid time format: {0}, expected HH:mm", telegram.Time);
                return;
            }

            this.HandleReceivedTime(time.TimeOfDay);
        }

        private void HandleInput(DS006 telegram)
        {
            var date = telegram.Date;
            if (date == null)
            {
                return;
            }

            var length = date.Length;
            if (length < 5 || length > 6)
            {
                this.receivedDate = DateTime.Parse(telegram.Date);
                return;
            }

            var format = length == 5 ? "ddMMy" : "ddMMyy";
            DateTime dateTime;

            if (!DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                if (length == 5 && date.StartsWith("2902"))
                {
                    // special leap year handling (if we get 29026, we would convert it to 29.2.2006 which
                    // doesn't exist)
                    dateTime = DateTime.ParseExact("29021" + date.Substring(4), "ddMMyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new FormatException("Could not parse date: " + date);
                }
            }

            var mod = 10 * (length - 4);
            var initialYear = this.dateTelegramConfig.InitialYear;
            if (dateTime.Year % mod < initialYear % mod)
            {
                dateTime = new DateTime(
                    (dateTime.Year % mod) + (initialYear / mod * mod) + mod, dateTime.Month, dateTime.Day);
            }
            else
            {
                dateTime = new DateTime(
                    (dateTime.Year % mod) + (initialYear / mod * mod), dateTime.Month, dateTime.Day);
            }

            this.receivedDate = dateTime.Date;
            telegram.Date = dateTime.ToString(this.dateTelegramConfig.OutputFormat, CultureInfo.InvariantCulture);
        }

        private void HandleInput(DS006A telegram)
        {
            DateTime time;
            if (!DateTime.TryParseExact(
                    telegram.DateTime,
                    DS006ADateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out time))
            {
                Logger.Warn("Invalid date/time format: '{0}', expected {1}", telegram.DateTime, DS006ADateTimeFormat);
                return;
            }

            this.receivedDate = time.Date;
            this.HandleReceivedTime(time.TimeOfDay);
        }

        private void HandleReceivedTime(TimeSpan time)
        {
            if (TimeProvider.Current.TickCount < this.startTickCount + this.config.InitialDelay.TotalMilliseconds)
            {
                // skip first InitialDelay seconds
                Logger.Debug("Skipping received telegram during InitialDelay ({0})", this.config.InitialDelay);
                return;
            }

            this.receivedTimes.Enqueue(new ReceivedTimestamp(time));
            if (this.receivedTimes.Count < this.config.WaitTelegrams)
            {
                // we have to wait for some more telegrams
                return;
            }

            var first = this.receivedTimes.Dequeue();
            var previous = first;
            bool lastIsDifferent = true;

            // check that the sequence of n telegrams is in ascending order
            foreach (var timestamp in this.receivedTimes)
            {
                if (previous.Time > timestamp.Time)
                {
                    if (previous.Time.Hours == 23 && timestamp.Time.Hours == 0)
                    {
                        // don't write a warning around midnight, we can't use this timestamp, but it's not that bad
                        Logger.Debug(
                            "Got midnight time sequence, ignoring it: {0} before {1}", timestamp.Time, previous.Time);
                        return;
                    }

                    Logger.Warn(
                        "Got invalid time sequence: {0} before {1}", timestamp.Time, previous.Time);
                    return;
                }

                lastIsDifferent = previous.Time != timestamp.Time;
                previous = timestamp;
            }

            var now = previous;
            var timeDiff = now.Time - first.Time;
            var tickDiff = now.TickCount - first.TickCount;

            // the delta of [the difference between the first and the last telegram]
            // and [their arrival time] should be less than one minute, then we
            // accept the time stamps as a valid sequence (no big jumps)
            var absDiff = Math.Abs(timeDiff.TotalMilliseconds - tickDiff);
            if (absDiff > 60000)
            {
                Logger.Warn(
                    "Time difference too big: {0} ms; first: [{1:HH:mm:ss}]@{2}; last: [{3:HH:mm:ss}]@{4}",
                    absDiff,
                    first.Time,
                    first.TickCount,
                    now.Time,
                    now.TickCount);
                return;
            }

            // we only continue when the next-to-last timestamp had a different time
            // than the last timestamp received (this means, we had a change in the minute
            // between the two timestamps)
            if (!lastIsDifferent)
            {
                Logger.Debug("Last timestamp is not different: {0:HH:mm:ss}", now.Time);
                return;
            }

            this.SendTimeUpdate(now);
        }

        private void SendTimeUpdate(ReceivedTimestamp timestamp)
        {
            // check if the time difference between the last update and the current
            // is greater or equal to the configured tolerance; if so, we continue.
            if (this.lastUpdate != null)
            {
                var diffMs = timestamp.TickCount - this.lastUpdate.TickCount;
                var now = this.lastUpdate.Time + new TimeSpan(0, 0, 0, 0, diffMs);
                var diff = timestamp.Time - now;
                if (Math.Abs(diff.TotalMilliseconds) < this.config.Tolerance.TotalMilliseconds)
                {
                    Logger.Debug(
                        "Time difference smaller than tolerance: abs({0}) < {1}",
                        diff.TotalSeconds,
                        this.config.Tolerance);
                    return;
                }
            }

            if (this.receivedDate == null)
            {
                Logger.Warn("Didn't receive the date yet, can't set time");
                return;
            }

            if (this.timePortListener.Port == null)
            {
                Logger.Warn("SystemTime port not available, can't set time");
                return;
            }

            this.lastUpdate = timestamp;

            var dateTime = this.receivedDate.Value + timestamp.Time;
            var utc = dateTime.ToUniversalTime();
            Logger.Info("Setting system time (UTC) to {0:R}", utc);

            var unixTimestamp = (int)(utc - UnixZero).TotalSeconds;
            this.timePortListener.Value = this.timePortListener.Port.CreateValue(unixTimestamp);
        }

        private class ReceivedTimestamp
        {
            public ReceivedTimestamp(TimeSpan time)
            {
                this.Time = time;
                this.TickCount = Environment.TickCount;
            }

            public int TickCount { get; private set; }

            public TimeSpan Time { get; private set; }
        }
    }
}
