// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateController.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.Medi;
    using Gorba.Common.Update.ServiceModel.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.CommonEmb;
    using Gorba.Motion.Update.Core.Agent;
    using Gorba.Motion.Update.Core.Dispatching;
    using Gorba.Motion.Update.Core.Progress;

    using Luminator.Multicast.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// This class handles the update of the system
    /// </summary>
    public class UpdateController : IUpdateContext, IUpdateController
    {
        private static readonly Logger Logger = LogHelper.GetLogger<UpdateController>();

        private readonly ConfigManager<UpdateConfig> configManager;

        private readonly List<IUpdateClient> updateClients = new List<IUpdateClient>();

        private readonly List<IUpdateProvider> updateProviders = new List<IUpdateProvider>();

        private readonly UpdateAgent updateAgent;

        private readonly IResourceProvider resourceProvider;

        private readonly UpdateProgressManager progressManager;

        private readonly UpdateDispatcher updateDispatcher;

        private readonly IPersistenceServiceImpl persistenceService;

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateController"/> class.
        /// </summary>
        /// <exception cref="ConfiguratorException">Condition.</exception>
        public UpdateController()
        {
            UpdateConfig config = null;
            try
            {
                this.configManager = new ConfigManager<UpdateConfig>
                                         {
                                             FileName = PathManager.Instance.GetPath(FileType.Config, "Update.xml"),
                                             EnableCaching = true,
                                             //          XmlSchema = UpdateConfig.Schema
                                         };
                config = this.configManager.Config;
            }      
            catch (Exception exception)
            {
                Logger.Error(
                   "Failed to read Update Manager settings file {0}. Exception {1}, {2}",
                   this.configManager.FileName,
                   exception.Message,
                   exception.InnerException?.Message ?? "");

                if (File.Exists("UpdateBackup.xml"))
                {
                    Logger.Error("The UpdateManager read failed File:{0}  using UpdateBackup.xml as backup. {1}", this.configManager.FileName, exception.Message);

                    this.configManager = new ConfigManager<UpdateConfig>
                    {
                        FileName = PathManager.Instance.GetPath(FileType.Config, "UpdateBackup.xml"),
                        EnableCaching = true,
                    };
                    config = this.configManager.Config;
                }                
            }
            
            this.ApplyConfigDefaults(config.Agent.RestartApplications);

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            this.persistenceService = PersistenceServiceFactory.CreatePersistenceService();
            this.persistenceService.Configure(
                PathManager.Instance.CreatePath(FileType.Data, "Persistence.xml"), TimeSpan.FromDays(10 * 365), true);
            serviceContainer.RegisterInstance<IPersistenceService>(this.persistenceService);

            var resourceService = MessageDispatcher.Instance.GetService<IResourceService>();
            this.resourceProvider = new MediResourceProvider(resourceService);

            this.progressManager = new UpdateProgressManager(config.Visualization);

            if (config.Agent.Enabled)
            {
                this.updateAgent = new UpdateAgent();
                this.updateAgent.Configure(this, config.Agent);
            }

            foreach (var client in config.Clients)
            {
                if (this.updateClients.Find(c => c.Name == client.Name) != null)
                {
                    throw new ConfiguratorException(
                        "Update client requires a unique name: " + client.Name, client.GetType());
                }

                Logger.Debug("UpdateController - adding update client: \'{0}\'.", client.Name);
                this.updateClients.Add(UpdateClientFactory.Instance.Create(client, this));
            }

            foreach (var provider in config.Providers)
            {
                if (this.updateProviders.Find(c => c.Name == provider.Name) != null)
                {
                    throw new ConfiguratorException(
                        "Update provider requires a unique name: " + provider.Name, provider.GetType());
                }

                Logger.Debug("UpdateController - adding update provider: \'{0}\'.", provider.Name);
                this.updateProviders.Add(UpdateProviderFactory.Instance.Create(provider, this));
            }

            this.updateDispatcher = new UpdateDispatcher(
                this.updateClients,
                this.updateProviders,
                this.updateAgent,
                config.CacheLimits);
        }
        
        IResourceProvider IUpdateContext.ResourceProvider
        {
            get
            {
                return this.resourceProvider;
            }
        }

        string IUpdateContext.TemporaryDirectory
        {
            get
            {
                return PathManager.Instance.CreatePath(FileType.Data, string.Empty);
            }
        }

        IEnumerable<IUpdateSink> IUpdateContext.Sinks
        {
            get
            {
                if (this.updateAgent != null)
                {
                    yield return this.updateAgent;
                }

                foreach (var updateProvider in this.updateProviders)
                {
                    yield return updateProvider;
                }
            }
        }

        /// <summary>
        /// Starts the update controller
        /// </summary>
        /// <param name="application">
        /// The <see cref="UpdateApplication"/> starting this controller.
        /// </param>
        public void Start(UpdateApplication application)
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            Logger.Info("Starting Update controller");

            this.updateDispatcher.Start();

            foreach (var updateProvider in this.updateProviders)
            {
                updateProvider.Start();
            }

            if (this.updateAgent != null)
            {
                this.updateAgent.Start();
            }

            foreach (var updateClient in this.updateClients)
            {
                updateClient.Start();
            }
        }

        /// <summary>
        /// Stops the update controller
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            Logger.Info("Stopping Update controller");

            if (this.updateAgent != null)
            {
                this.updateAgent.Stop();
            }

            foreach (var updateProvider in this.updateProviders)
            {
                updateProvider.Stop();
            }

            foreach (var updateClient in this.updateClients)
            {
                updateClient.Stop();
            }

            this.updateDispatcher.Stop();

            this.persistenceService.Save();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        IProgressMonitor IUpdateContext.CreateProgressMonitor(
            UpdateStage stage, bool showVisualization)
        {
            return showVisualization ? this.progressManager.CreateProgressMonitor(stage) : new NullProgressMonitor();
        }

        private void ApplyConfigDefaults(RestartApplicationsConfig restartApplicationsConfig)
        {
            foreach (var dependency in restartApplicationsConfig.Dependencies)
            {
                dependency.Path = this.configManager.GetAbsolutePathRelatedToConfig(dependency.Path);
                for (var i = 0; i < dependency.ExecutablePaths.Count; i++)
                {
                    dependency.ExecutablePaths[i] =
                        this.configManager.GetAbsolutePathRelatedToConfig(dependency.ExecutablePaths[i]);
                }
            }
        }
    }
}
