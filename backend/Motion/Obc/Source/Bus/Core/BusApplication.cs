// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Obc.Bus.Core
{
    using System;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Obc.Bus;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Bus.Core.Route;
    using Gorba.Motion.Obc.Common;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The bus application handling everything.
    /// </summary>
    public class BusApplication : ApplicationBase
    {
        /// <summary>
        /// The management name used in Medi and System Manager.
        /// </summary>
        public static readonly string ManagementName = "Bus";

        private static readonly TimeSpan DefaultPersistenceValidity = TimeSpan.FromDays(1);

        private readonly RouteManager routeManager = new RouteManager();

        private readonly XimpleProvider ximpleProvider = new XimpleProvider();

        private IPersistenceServiceImpl persistenceService;

        private BusConfig config;

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// Implementing classes should either override <see cref="ApplicationBase.DoRun(string[])"/> or this method.
        /// </summary>
        protected override void DoRun()
        {
            TimeProvider.Current = new GpsTimeProvider();
            this.SetupServices();
            RemoteEventHandler.Initialize();

            this.config = LoadConfig();
            this.routeManager.Configure(this.config);
            this.routeManager.ConnectToCenter();

            this.ximpleProvider.Start();

            this.SetRunning();

            this.routeManager.Run();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="ApplicationBase.DoRun()"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.ximpleProvider.Stop();
            this.routeManager.Stop();
            this.persistenceService.Save();
        }

        private static BusConfig LoadConfig()
        {
            var configManager = new ConfigManager<BusConfig>();
            configManager.FileName = PathManager.Instance.GetPath(FileType.Config, "Bus.xml");
            return configManager.Config;
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
