// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System.Diagnostics;

    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Core.ResourceUsage;
    using Gorba.Common.SystemManagement.ServiceModel;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Application controller that monitors the system manager itself and its resources.
    /// There should always only be one instance of this class in an <see cref="ApplicationManager"/>.
    /// </summary>
    public partial class SystemManagerApplicationController : ApplicationControllerBase
    {
        private readonly ProcessConfig config;

        private readonly ProcessResourcesObserver resourcesObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManagerApplicationController"/> class.
        /// </summary>
        /// <param name="manager">
        /// The application manager.
        /// </param>
        public SystemManagerApplicationController(ApplicationManager manager)
            : this(CreateConfig(), manager)
        {
        }

        private SystemManagerApplicationController(ProcessConfig config, ApplicationManager manager)
            : base(config, manager)
        {
            this.config = config;
            this.resourcesObserver = new ProcessResourcesObserver(
                Process.GetCurrentProcess(), this, config.CpuLimit, config.RamLimit);
        }

        /// <summary>
        /// Gets the main file path.
        /// </summary>
        protected override string FilePath
        {
            get
            {
                return this.config.ExecutablePath;
            }
        }

        /// <summary>
        /// The create application info.
        /// </summary>
        /// <returns>
        /// The <see cref="ApplicationInfo"/>.
        /// </returns>
        public override ApplicationInfo CreateApplicationInfo()
        {
            var info = base.CreateApplicationInfo();
            info.CpuUsage = this.resourcesObserver.CpuUsage;
            info.RamBytes = this.resourcesObserver.RamBytes;
            return info;
        }

        /// <summary>
        /// Implementation of the launch of the application.
        /// </summary>
        protected override void DoLaunch()
        {
            this.resourcesObserver.Start();
            this.SetState(ApplicationState.Running, ApplicationReason.SystemBoot, string.Empty);
        }

        /// <summary>
        /// Implementation of the exit of the application.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        protected override void DoExit(ApplicationReason reason, string explanation)
        {
            this.resourcesObserver.Stop();
            this.SetState(ApplicationState.Exiting, ApplicationReason.Requested, string.Empty);
            ServiceLocator.Current.GetInstance<SystemManagementControllerBase>().Stop(false, reason, explanation);
        }
    }
}
