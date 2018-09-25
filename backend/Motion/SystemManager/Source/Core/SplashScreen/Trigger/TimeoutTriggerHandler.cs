// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeoutTriggerHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeoutTriggerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using System;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Trigger handler that triggers after a given timeout.
    /// This can be used to show a splash screen for a given time.
    /// </summary>
    public class TimeoutTriggerHandler : TriggerHandlerBase
    {
        private readonly ITimer timer;

        private SplashScreenHandler owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutTriggerHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public TimeoutTriggerHandler(TimeoutTriggerConfig config)
        {
            this.timer = TimerFactory.Current.CreateTimer("SplashScreenTrigger");
            this.timer.AutoReset = false;
            this.timer.Interval = config.Delay;
            this.timer.Elapsed += (s, e) => this.RaiseTriggered(e);
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        /// <param name="splashScreenHandler">
        /// The handler which owns this trigger handler.
        /// </param>
        public override void Start(SplashScreenHandler splashScreenHandler)
        {
            this.owner = splashScreenHandler;
            this.owner.Triggered += this.OwnerOnTriggered;
            this.owner.Hidden += this.OwnerOnHidden;
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public override void Stop()
        {
            this.timer.Enabled = false;
            this.owner.Triggered -= this.OwnerOnTriggered;
            this.owner.Hidden -= this.OwnerOnHidden;
            this.timer.Dispose();
        }

        private void OwnerOnTriggered(object sender, EventArgs e)
        {
            this.timer.Enabled = false;
            this.timer.Enabled = true;
        }

        private void OwnerOnHidden(object sender, EventArgs e)
        {
            this.timer.Enabled = false;
        }
    }
}