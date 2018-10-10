// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemShutdownTriggerHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemShutdownTriggerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using System;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;
    using Gorba.Common.SystemManagement.Core;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Trigger handler that triggers when the system is shutting down.
    /// </summary>
    public class SystemShutdownTriggerHandler : TriggerHandlerBase
    {
        private readonly SystemManagementControllerBase controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemShutdownTriggerHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public SystemShutdownTriggerHandler(SystemShutdownTriggerConfig config)
        {
            this.controller = ServiceLocator.Current.GetInstance<SystemManagementControllerBase>();
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        /// <param name="owner">
        /// The handler which owns this trigger handler.
        /// </param>
        public override void Start(SplashScreenHandler owner)
        {
            this.controller.Stopping += this.ControllerOnStopping;
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public override void Stop()
        {
            this.controller.Stopping -= this.ControllerOnStopping;
        }

        private void ControllerOnStopping(object sender, EventArgs e)
        {
            this.RaiseTriggered(e);
        }
    }
}