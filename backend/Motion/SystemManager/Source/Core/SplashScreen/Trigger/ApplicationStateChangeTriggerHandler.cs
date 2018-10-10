// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationStateChangeTriggerHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationStateChangeTriggerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using System;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;
    using Gorba.Common.SystemManagement.Core.Applications;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Trigger handler for the state change of an application.
    /// </summary>
    public class ApplicationStateChangeTriggerHandler : TriggerHandlerBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ApplicationStateChangeTriggerHandler>();

        private readonly ApplicationStateChangeTriggerConfig config;

        private readonly ApplicationManager appManager;

        private ApplicationControllerBase application;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStateChangeTriggerHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public ApplicationStateChangeTriggerHandler(ApplicationStateChangeTriggerConfig config)
        {
            this.config = config;
            this.appManager = ServiceLocator.Current.GetInstance<ApplicationManager>();
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        /// <param name="owner">
        /// The handler which owns this trigger handler.
        /// </param>
        public override void Start(SplashScreenHandler owner)
        {
            this.application = this.appManager.GetController(this.config.Application);
            if (this.application == null)
            {
                Logger.Warn("Couldn't find application {0}", this.config.Application);
                return;
            }

            this.application.StateChanged += this.ApplicationOnStateChanged;
            this.CheckState();
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public override void Stop()
        {
            if (this.application == null)
            {
                return;
            }

            this.application.StateChanged -= this.ApplicationOnStateChanged;
        }

        private void CheckState()
        {
            if (this.application.State == this.config.State)
            {
                this.RaiseTriggered(EventArgs.Empty);
            }
        }

        private void ApplicationOnStateChanged(object sender, EventArgs eventArgs)
        {
            this.CheckState();
        }
    }
}