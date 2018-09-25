// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.SystemManager.SplashScreen;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Core;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.SystemManager.Core.Hal;
    using Gorba.Motion.SystemManager.Core.SplashScreen;

    /// <summary>
    /// The controller implementation for System Manager.
    /// </summary>
    public class SystemManagerController : SystemManagementControllerBase
    {
        private readonly SplashScreenManager splashScreenManager;

        private readonly PopupBlocker popupBlocker;

        private readonly HardwareAbstractionBase hardwareAbstraction;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManagerController"/> class.
        /// </summary>
        public SystemManagerController()
            : base(PathManager.Instance.GetPath(FileType.Config, "SystemManager.xml"))
        {
            this.ShutdownCatcher = new ShutdownCatcher();
            this.ShutdownCatcher.ShuttingDown += this.ShutdownCatcherOnShuttingDown;

            this.popupBlocker = new PopupBlocker(this.Config.System.PreventPopups, this);

            this.hardwareAbstraction = HardwareAbstractionBase.Create(this.Config.System);
            this.hardwareAbstraction.ShutdownRequested += this.HardwareAbstractionOnShutdownRequested;

            if (this.Config.System.ShutDownSplashScreenVisibleTime > TimeSpan.Zero)
            {
                var shutdownSplashScreen = new SplashScreenConfig
                    {
                        Enabled = true,
                        Background = "#E6ECF0",
                        Foreground = "Black",
                        Name = ShutDown,
                        ShowOn = new List<SplashScreenTriggerConfigBase>
                            {
                                new SystemShutdownTriggerConfig()
                            },
                        Items = new List<SplashScreenItemBase>
                            {
                                new LogoSplashScreenItem(),
                                new ShutDownMessageSplashScreenItem
                                    {
                                        ShutdownTime = this.Config.System.ShutDownSplashScreenVisibleTime
                                    }
                            }
                    };
                this.Config.SplashScreens.Items.Add(shutdownSplashScreen);
            }

            this.splashScreenManager = new SplashScreenManager(
                this.Config.SplashScreens, this.hardwareAbstraction, this.ShutdownCatcher);
        }

        /// <summary>
        /// Gets the shutdown catcher.
        /// </summary>
        public ShutdownCatcher ShutdownCatcher { get; private set; }

        /// <summary>
        /// Implementation of starting the controller.
        /// </summary>
        /// <param name="context">
        /// The application context in which the controller is started.
        /// </param>
        protected override void DoStart(ApplicationContext context)
        {
            this.ShutdownCatcher.Start(context);
            this.hardwareAbstraction.Start();
            this.splashScreenManager.Start(context);
            base.DoStart(context);

            this.popupBlocker.Start();
        }

        /// <summary>
        /// Implementation of stopping the controller.
        /// </summary>
        protected override void DoStop()
        {
            base.DoStop();

            this.popupBlocker.Stop();

            this.splashScreenManager.Stop();

            this.hardwareAbstraction.Stop();

            this.ShutdownCatcher.Stop();
        }

        /// <summary>
        /// Implementation of the stop of System Manager.
        /// </summary>
        /// <param name="exitApplications">
        /// Flag to tell if all managed applications should be exited (true) or not (false).
        /// </param>
        /// <param name="reason">
        /// The reason for stopping System Manager.
        /// </param>
        /// <param name="explanation">
        /// The explanation for stopping System Manager.
        /// </param>
        protected override void ExecuteStop(bool exitApplications, ApplicationReason reason, string explanation)
        {
            if (exitApplications && this.Config.System.ShutDownSplashScreenVisibleTime > TimeSpan.Zero &&
                (reason == ApplicationReason.SystemShutdown || reason == ApplicationReason.Requested))
            {
                var timer = TimerFactory.Current.CreateTimer("ShutdownSplashScreen");
                timer.AutoReset = false;
                timer.Interval = this.Config.System.ShutDownSplashScreenVisibleTime;
                timer.Elapsed += (s, e) => base.ExecuteStop(true, reason, explanation);
                timer.Enabled = true;
                return;
            }

            base.ExecuteStop(exitApplications, reason, explanation);
        }

        private void HardwareAbstractionOnShutdownRequested(object sender, EventArgs eventArgs)
        {
            this.Shutdown("Hardware requested");
        }

        private void ShutdownCatcherOnShuttingDown(object sender, EventArgs eventArgs)
        {
            this.Shutdown("Windows shutdown");
        }
    }
}
