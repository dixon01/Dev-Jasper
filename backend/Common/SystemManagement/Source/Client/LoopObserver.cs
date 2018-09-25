// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoopObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Implementation of the <see cref="ILoopObserver"/>.
    /// </summary>
    internal class LoopObserver : ILoopObserver
    {
        private readonly Logger logger;

        private readonly long timeout;

        private readonly IApplicationRegistration registration;

        private long lastTriggerTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopObserver"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <param name="registration">
        /// The registration that created this observer.
        /// </param>
        public LoopObserver(string name, TimeSpan timeout, IApplicationRegistration registration)
        {
            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + name);

            this.timeout = (long)timeout.TotalMilliseconds;

            this.lastTriggerTime = TimeProvider.Current.TickCount;

            this.registration = registration;
            this.registration.WatchdogKicked += this.RegistrationOnWatchdogKicked;

            this.logger.Debug("Started");
        }

        /// <summary>
        /// Triggers this loop observer.
        /// This method has to be called regularly.
        /// </summary>
        public void Trigger()
        {
            this.lastTriggerTime = TimeProvider.Current.TickCount;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.registration.WatchdogKicked -= this.RegistrationOnWatchdogKicked;
            this.logger.Debug("Stopped");
        }

        private void RegistrationOnWatchdogKicked(object sender, CancelEventArgs e)
        {
            var now = TimeProvider.Current.TickCount;
            if (now > this.lastTriggerTime + this.timeout)
            {
                this.logger.Warn(
                    "Didn't get trigger within {0} seconds, cancelling watchdog request", this.timeout / 1000);
                e.Cancel = true;
            }
        }
    }
}
