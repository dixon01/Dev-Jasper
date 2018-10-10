// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerApplication.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.SystemManagement.Core;
    using Gorba.Common.SystemManagement.Core.ResourceUsage;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The system manager main application.
    /// This is a wrapper around <see cref="SystemManagementControllerBase"/> that sets up
    /// all the necessary stuff needed by System Manager.
    /// </summary>
    public partial class SystemManagerApplication : IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SystemManagerApplication>();

        /// <summary>
        /// Gets the controller.
        /// This property will only be set in <see cref="Configure"/> when it returned true.
        /// </summary>
        public SystemManagementControllerBase Controller { get; private set; }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="systemManagerOptions">
        /// The options.
        /// </param>
        public void Configure(SystemManagerOptions systemManagerOptions)
        {
            this.SetupServices();

            this.Controller = new SystemManagerController();
        }

        /// <summary>
        /// Runs this application. This method will only return when System Manager should exit.
        /// </summary>
        public void Run()
        {
            this.Run(null);
        }

        /// <summary>
        /// Runs this application. This method will only return when System Manager should exit.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Run(ApplicationContext context)
        {
            if (this.Controller == null)
            {
                return;
            }

            this.Controller.Run(context);

            Logger.Info("System Manager stopped with success");
            LogManager.Flush();
        }

        /// <summary>
        /// Registers all services with the service container.
        /// </summary>
        private void SetupServices()
        {
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance<ICpuUsageObserverFactory>(new CpuUsageObserverFactory());

            var persistence = PersistenceServiceFactory.CreatePersistenceService();
            serviceContainer.RegisterInstance<IPersistenceService>(persistence);
        }
    }
}
