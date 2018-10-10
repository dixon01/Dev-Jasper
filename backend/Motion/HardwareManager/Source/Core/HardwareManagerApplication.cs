// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareManagerApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The Hardware Manager application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core
{
    using System;
    using System.Threading;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Microsoft.Practices.ServiceLocation;
    using NLog;

    /// <summary>
    /// The Hardware Manager application.
    /// </summary>
    public class HardwareManagerApplication : ApplicationBase
    {
        private static readonly TimeSpan DefaultPersistenceValidity = TimeSpan.FromDays(30 * 365);
        private static new readonly Logger Logger = LogHelper.GetLogger<HardwareManagerApplication>();

        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private HardwareManagementController controller;

        private IPersistenceServiceImpl persistenceService;

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method doesn't return until after <see cref="ApplicationBase.Stop"/> was called.
        /// </summary>
        protected override void DoRun()
        {
            Logger.Info(
                "Environment.Version={0}, OSVersion={1}, ApplicationData={2}",
                Environment.Version,
                Environment.OSVersion,
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            this.SetupServices();

            this.controller = new HardwareManagementController();

            this.controller.Start();

            this.SetRunning();

            this.runWait.WaitOne();

            this.persistenceService.Save();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method stops whatever is running in <see cref="DoRun"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.controller.Stop();
            this.controller.Dispose();
            this.controller = null;

            this.runWait.Set();
        }

        /// <summary>
        /// Registers all services with the service container.
        /// </summary>
        private void SetupServices()
        {
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();

            this.persistenceService = PersistenceServiceFactory.CreatePersistenceService();
            this.persistenceService.Configure(
                PathManager.Instance.CreatePath(FileType.Data, "Persistence.xml"), DefaultPersistenceValidity, true);
            serviceContainer.RegisterInstance<IPersistenceService>(this.persistenceService);
        }
    }
}
