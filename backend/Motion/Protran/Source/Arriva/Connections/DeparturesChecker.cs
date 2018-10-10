// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeparturesChecker.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva.Connections
{
    using System;
    using System.Globalization;
    using System.Threading;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Objected tasked to check the validity period of a set of departures.
    /// Once the departures are expired, it notifies properly all the registered handlers.
    /// </summary>
    public class DeparturesChecker
    {
        /// <summary>
        /// The format that must be respected in the departures.xml, about the validity time.
        /// </summary>
        private const string ExpirationTimeStampFormat = "yyyy-MM-dd HH:mm";

        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The departures that will be checked by this object.
        /// </summary>
        private readonly DeparturesConfig departuresToCheck;

        /// <summary>
        /// The timer tasked to check the validity time.
        /// </summary>
        private Timer timerChecker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeparturesChecker"/> class.
        /// </summary>
        /// <param name="departures">The departures that will be checked by this object.</param>
        public DeparturesChecker(DeparturesConfig departures)
        {
            this.departuresToCheck = departures;
        }

        /// <summary>
        /// Event that is fired every time a data item is received.
        /// </summary>
        public event EventHandler<DeparturesExpiredEventArgs> DeparturesExpired;

        /// <summary>
        /// Tells whether the departures are expired or not.
        /// </summary>
        /// <returns>True if the departures are expired, else false.</returns>
        public bool AreDeparturesExpired()
        {
            int totSecondsDiff = this.CalculateDiffInSeconds();
            return totSecondsDiff <= 0;
        }

        /// <summary>
        /// Starts the monitoring activity about the expiration
        /// of a set of departures.
        /// </summary>
        public void Start()
        {
            int seconds = this.CalculateDiffInSeconds();

            // Attention:
            // we can't use the System.Timer.Timer because its max interval
            // is equal to 2^31 (around 25 days) milliseconds
            // but we might need more.
            // The System.Threading.Timer supports such big intervals.
            var span = new TimeSpan(0, 0, 0, seconds);
            var disablePeriodic = new TimeSpan(0, 0, 0, 0, -1);
            this.timerChecker = new Timer(this.OnElapsedTime, null, span, disablePeriodic);
        }

        /// <summary>
        /// Tells whether the departures have a corrected expiration field.
        /// </summary>
        /// <returns>
        /// True if the departures have a well formatted "expiration" field, else false.
        /// </returns>
        public bool IsExpirationCorrect()
        {
            DateTime expiration;
            bool ok = DateTime.TryParseExact(
                this.departuresToCheck.Expiration,
                ExpirationTimeStampFormat,
                CultureInfo.InstalledUICulture,
                DateTimeStyles.None,
                out expiration);

            if (!ok)
            {
                // the format of the recevied expiration time is not compliant with the one that we expect.
                Logger.Info("Departures' expiration time not well formated. Not processed");
                return false;
            }

            // yes, the "expiration" field is well formatted.
            return true;
        }

        /// <summary>
        /// Calculates the remaining seconds between the current timestamp
        /// and the departures' expiration one. If a number less or equal to zero is returned,
        /// it means that the departures are expired.
        /// </summary>
        /// <returns>The amount of seconds between the current timestamp
        /// and the one that belongs to the departures.</returns>
        private int CalculateDiffInSeconds()
        {
            DateTime start = TimeProvider.Current.Now;
            DateTime end = DateTime.ParseExact(
                this.departuresToCheck.Expiration, ExpirationTimeStampFormat, CultureInfo.InvariantCulture);

            var diff = end - start;
            return (int)diff.TotalSeconds;
        }

        /// <summary>
        /// Function invoked asynchronously by the O.S. whenever expires the
        /// the timer tasked to monitor the departures' expiration time.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        private void OnElapsedTime(object sender)
        {
            // it's the time to notify all the registerd handlers about
            // the departures' expiration.
            var handler = this.DeparturesExpired;
            if (handler != null)
            {
                handler(this, new DeparturesExpiredEventArgs(this.departuresToCheck));
            }

            this.timerChecker.Dispose();
        }
    }
}
