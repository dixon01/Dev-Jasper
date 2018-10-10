// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemBootTriggerHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemBootTriggerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using System;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;

    /// <summary>
    /// Trigger handler that triggers when the system boots.
    /// This trigger actually just triggers when <see cref="Start"/> is called,
    /// since this only happens when the system is booting (i.e. SM is starting).
    /// </summary>
    public class SystemBootTriggerHandler : TriggerHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemBootTriggerHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public SystemBootTriggerHandler(SystemBootTriggerConfig config)
        {
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        /// <param name="owner">
        /// The handler which owns this trigger handler.
        /// </param>
        public override void Start(SplashScreenHandler owner)
        {
            this.RaiseTriggered(EventArgs.Empty);
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public override void Stop()
        {
            // nothing to do
        }
    }
}