// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemHost.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Host
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using Gorba.Center.BackgroundSystem.Core;
    using Gorba.Center.BackgroundSystem.Core.ChangeTracking;
    using Gorba.Center.BackgroundSystem.Core.Dynamic;
    using Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper;
    using Gorba.Center.BackgroundSystem.Core.Membership;
    using Gorba.Center.BackgroundSystem.Core.Qube.Configuration;
    using Gorba.Center.BackgroundSystem.Core.Resources;
    using Gorba.Center.BackgroundSystem.Core.Security;
    using Gorba.Center.BackgroundSystem.Core.Setup;
    using Gorba.Center.BackgroundSystem.Core.Units;
    using Gorba.Center.BackgroundSystem.Core.Update;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Resources;
    using NLog;

    /// <summary>
    /// BackgroundSystemHost to launch the BackgroundSystem services.
    /// </summary>
    public class BackgroundSystemHost
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly object locker = new object();

        private IEnumerable<IServiceHost> allServices;

        private DynamicDataManager dynamicDataManager;

        private volatile bool isStarted;

        private UnitManager unitManager;

        /// <summary>
        /// Event raised after the host is started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Bootstraps the system.
        /// </summary>
        public void Start()
        {
            if (this.isStarted)
            {
                return;
            }

            lock (this.locker)
            {
                if (this.isStarted)
                {
                    return;
                }

                this.isStarted = true;
            }

            var dataBoostrapper = new Data.Bootstrapper();
            dataBoostrapper.Bootstrap();
            SetNotificationManagerFactory();
            var backgroundSystemConfiguration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();

            Logger.Debug("Starting local change tracking managers");
            var bootstrapper = new LocalChangeTrackingManagementBootstrapper(
                backgroundSystemConfiguration,
                "BackgroundSystem");
            bootstrapper.ProgressChanged += BootstrapperOnProgressChanged;
            var result = bootstrapper.RunAsync(null).Result as ChangeTrackingManagementHostBootstrapperResult;
            if (result == null)
            {
                throw new Exception("No local bootstrapper result.");
            }

            Logger.Info("All local change tracking managers started");

            Logger.Debug("Creating functional services");
            var functionalServices = CreateFunctionalServices(backgroundSystemConfiguration).ToList();
            Logger.Info("All functional services created");

            Logger.Debug("Starting web host");

            var serializer = new MainUnitConfigurationSerializer();
            DependencyResolver.Current.Register<IConfigurationSerializer>(serializer);

            var converter = new EPaperConverter();
            DependencyResolver.Current.Register<IEPaperConverter>(converter);

            var updateCommandDynamicUpdater = new MainUnitUpdateCommandUpdater();
            updateCommandDynamicUpdater.Start();
            DependencyResolver.Current.Register<IMainUnitUpdateCommandUpdater>(updateCommandDynamicUpdater);

            this.unitManager = new UnitManager();
            DependencyResolver.Current.Register<IUnitManager>(this.unitManager);
            this.unitManager.Start();

            this.dynamicDataManager = new DynamicDataManager();
            this.dynamicDataManager.Start();

            IServiceHost[] apiHostServices = GetApiHostService(backgroundSystemConfiguration);
            
            this.allServices = result.ChangeTrackingServiceHosts
                               .Concat(result.NonChangeTrackingServiceHosts)
                               .Concat(functionalServices)
                               .Concat(apiHostServices);

            foreach (var serviceHost in this.allServices)
            {
                serviceHost.Open();
                Logger.Debug("Service '{0}' opened", serviceHost.Name);
            }

            Logger.Info("All services started");
            this.RaiseStarted();
        }

        /// <summary>
        /// Get the configured Api Host service
        /// </summary>
        /// <param name="configuration">Configuration used for the communications port</param>
        /// <returns></returns>
        private static IServiceHost[] GetApiHostService(BackgroundSystemConfiguration configuration)
        {
            int apiHostPort = BackgroundSystemConfiguration.DefaultApiHostPort;
            
            if (configuration != null)
            {
                apiHostPort = configuration.ApiHostPort;
            }
            
            Logger.Debug($"Api Host Port: {apiHostPort}");
            return new IServiceHost[] { new ApiHost<Startup>("Api host", $"http://*:{apiHostPort}") };
        }

        /// <summary>
        /// Stops the BackgroundSystem.
        /// </summary>
        public void Stop()
        {
            if (!this.isStarted)
            {
                return;
            }

            lock (this.locker)
            {
                if (!this.isStarted)
                {
                    return;
                }

                this.isStarted = false;
            }

            if (this.dynamicDataManager != null)
            {
                this.dynamicDataManager.Stop();
            }

            if (this.unitManager != null)
            {
                this.unitManager.Stop();
            }

            foreach (var serviceHost in this.allServices)
            {
                serviceHost.Close();
                var disposable = serviceHost as IDisposable;
                if (disposable == null)
                {
                    continue;
                }

                disposable.Dispose();
            }
        }

        /// <summary>
        /// Verifies that all resource entries in database have a content
        /// (local *.rx file on OnPremises systems, storage blob on Azure system).
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if the resource service could not be found.
        /// </exception>
        public void VerifyResources()
        {
            Logger.Debug("Verifying all resources in database");
            var resourceService = DependencyResolver.Current.Get<IResourceService>() as ResourceServiceBase;
            if (resourceService == null)
            {
                throw new InvalidCastException("Could not find resource service.");
            }

            var resourceDataService = DependencyResolver.Current.Get<IResourceDataService>();
            var query = new ResourceQuery();
            var resources = resourceDataService.QueryAsync(query).Result.ToList();
            var progressTotal = resources.Count;
            var progress = 1;
            var missingResources = new List<Resource>();
            foreach (var resource in resources)
            {
                Logger.Trace("Resource {0} of {1}", progress, progressTotal);
                if (!resourceService.ContentExists(resource.Hash))
                {
                    missingResources.Add(resource);
                }

                progress++;
            }

            if (missingResources.Any())
            {
                var message = new StringBuilder("The content is missing for the following resources with hash: ");

                foreach (var missingResource in missingResources)
                {
                    message.AppendFormat("{0}, ", missingResource.Hash);
                }

                Logger.Error(message.ToString());
            }
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        protected virtual void RaiseStarted()
        {
            var handler = this.Started;
            if (handler == null)
            {
                return;
            }

            handler(this, EventArgs.Empty);
        }

        private static void SetNotificationManagerFactory()
        {
            NotificationManagerFactoryUtility.ConfigureMediNotificationManager(true);
        }

        private static IEnumerable<IServiceHost> CreateFunctionalServices(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var membershipService = new MembershipService();
            DependencyResolver.Current.Register<IMembershipService>(membershipService);
            var remoteMembershipService = new RemoteMembershipService(membershipService);
            var serviceHost = backgroundSystemConfiguration.FunctionalServices.CreateFunctionalServiceHost(
                "Membership",
                remoteMembershipService);
            serviceHost.HandleEvents();
            yield return new ServiceHostWrapper("Membership", serviceHost);

            var updateService = new UpdateService();
            DependencyResolver.Current.Register<IUpdateService>(updateService);
            var remoteUpdateService = new RemoteUpdateService(updateService);
            serviceHost = backgroundSystemConfiguration.FunctionalServices.CreateFunctionalServiceHost(
                "Update",
                remoteUpdateService);
            serviceHost.HandleEvents();
            yield return new ServiceHostWrapper("Update", serviceHost);

            var resourceService = ResourceServiceProvider.Current.Create();
            DependencyResolver.Current.Register<IResourceService>(resourceService);
            DependencyResolver.Current.Register<IResourceProvider>(resourceService);
            var remoteResourceService = new RemoteResourceService(resourceService);
            serviceHost = backgroundSystemConfiguration.FunctionalServices.CreateFunctionalServiceHost(
                "Resources",
                remoteResourceService);
            serviceHost.HandleEvents();
            yield return new ServiceHostWrapper("Resources", serviceHost);

            var contentResourceService = ContentResourceServiceProvider.Current.Create();
            DependencyResolver.Current.Register<IContentResourceService>(contentResourceService);
            var remoteContentResourceService = new RemoteContentResourceService(contentResourceService);
            serviceHost =
                backgroundSystemConfiguration.FunctionalServices.CreateFunctionalServiceHost(
                    "ContentResources",
                    remoteContentResourceService);
            serviceHost.HandleEvents();
            yield return new ServiceHostWrapper("ContentResources", serviceHost);
        }

        private static void BootstrapperOnProgressChanged(object sender, ProgressChangedEventArgs eventArgs)
        {
            Logger.Debug("Local managers loading: {0}%", eventArgs.ProgressPercentage);
            if (eventArgs.ProgressPercentage == 100)
            {
                Logger.Info("Local change tracking managers loaded");
            }
        }
    }
}