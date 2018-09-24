// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerRole.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WorkerRole type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading;

    using Gorba.Center.BackgroundSystem.Core.Resources;
    using Gorba.Center.BackgroundSystem.Core.Security;
    using Gorba.Center.BackgroundSystem.Core.Update.Azure;
    using Gorba.Center.BackgroundSystem.Data;
    using Gorba.Center.BackgroundSystem.Host;
    using Gorba.Center.BackgroundSystem.WorkerRole.Extensions;
    using Gorba.Center.BackgroundSystem.WorkerRole.Resources;
    using Gorba.Center.BackgroundSystem.WorkerRole.Security;
    using Gorba.Center.BackgroundSystem.WorkerRole.Update;
    using Gorba.Center.Common.Azure;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Portal.Azure;
    using Gorba.Center.Portal.Host;
    using Gorba.Center.Portal.Host.Configuration;
    using Gorba.Center.Portal.Host.Settings;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Compatibility;

    using Microsoft.WindowsAzure.ServiceRuntime;

    using NLog;

    /// <summary>
    /// Defines the worker role to host the BackgroundSystem (portal and services).
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        private const int DefaultConnectionLimit = 512;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        /// <inheritdoc/>
        public override void Run()
        {
            Trace.TraceInformation("WorkerRole is running");

            try
            {
                Run(this.cancellationTokenSource.Token);
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        /// <inheritdoc/>
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at <see cref="http://go.microsoft.com/fwlink/?LinkId=166357"/>.
            var result = base.OnStart();

            Trace.TraceInformation("WorkerRole has been started");
            Logger.Info("WorkerRole has been started");

            return result;
        }

        /// <inheritdoc/>
        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole has stopped");
        }

        private static T GetConfigurationSettingValue<T>(string name, T defaultValue = default(T))
        {
            var stringValue = RoleEnvironment.GetConfigurationSettingValue(name);
            if (string.IsNullOrEmpty(stringValue))
            {
                return defaultValue;
            }

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            return (T)typeConverter.ConvertFromString(stringValue);
        }

        private static void Run(CancellationToken cancellationToken)
        {
            try
            {
                Logger.Info("WorkerRole starting");
                ConfigureMediServer();
                StartupConfigurator.Set(new AzureStartupConfigurator());
                BackgroundSystemConfigurationProvider.Set(new AzureBackgroundSystemConfigurationProvider());
                PortalSettingsProvider.SetCurrent(new ExtendedAzurePortalSettingsProvider());
                PortalRepositoryConfigurationProvider.Set(new AzurePortalRepositoryConfigurationProvider());
                HostingSettingsProvider.Set(new AzureHostingSettingsProvider());
                DataContextFactory.SetCurrent(new AzureDataContextFactory());
                LoginValidatorProvider.SetCurrent(new AzureLoginValidatorProvider());
                ResourceServiceProvider.Set(new AzureResourceServiceProvider());
                ContentResourceServiceProvider.Set(new AzureContentResourceServiceProvider());
                AzureConfigurator.Set(new WcfLoggingAzureConfigurator());
                AzureConfigurator.Current.EnsureConfiguration();
                LogsFeedbackHandlerProvider.Set(new AzureLogsFeedbackHandlerProvider());
                ServicePointManager.DefaultConnectionLimit =
                    GetConfigurationSettingValue(
                        Common.Azure.PredefinedAzureItems.Settings.ConnectionLimit,
                        DefaultConnectionLimit);
                Logger.Debug("Connection limit set to {0}", ServicePointManager.DefaultConnectionLimit);
                var systemHost = new BackgroundSystemHost();
                systemHost.Started += SystemHostOnStarted;
                if (GetConfigurationSettingValue(PredefinedAzureItems.Settings.StartPortalHost, false))
                {
                    Logger.Debug("Starting the Portal Host");
                    using (
                        PortalUtility.GetHost(Portal.Host.PredefinedAzureItems.Endpoints.HttpEndpoint))
                    {
                        using (
                            PortalUtility.GetHost(
                                Portal.Host.PredefinedAzureItems.Endpoints.HttpsEndpoint,
                                Portal.Host.PredefinedAzureItems.Settings.EnableHttps))
                        {
                            Logger.Info("Portal host created. Starting BackgroundSystem host");
                            systemHost.Start();
                            MoveResources();
                            systemHost.VerifyResources();
                            cancellationToken.WaitHandle.WaitOne();
                        }
                    }
                }
                else
                {
                    Logger.Debug("Starting BackgroundSystem");
                    systemHost.Start();
                    MoveResources();
                    systemHost.VerifyResources();
                    cancellationToken.WaitHandle.WaitOne();
                }

                systemHost.Stop();
                systemHost.Started -= SystemHostOnStarted;
            }
            catch (Exception exception)
            {
                Trace.TraceError(
                    "Can't start the worker role because of an unhandled exception: {0}",
                    exception.StackTrace);
                Logger.Error(exception, "Error during run");
            }
        }

        private static void SystemHostOnStarted(object sender, EventArgs eventArgs)
        {
            Logger.Trace("The system host is started. Ensuring the configuration.");
            AzureConfigurator.Current.EnsureConfiguration();
        }

        private static void MoveResources()
        {
            Logger.Debug("Move locally stored resources to the storage");
            var resourcesPath = GetResourcesPath();
            var resourceService = DependencyResolver.Current.Get<IResourceProvider>() as AzureResourceService;
            if (resourceService == null)
            {
                throw new InvalidCastException("Loaded ResourceService is not of type AzureResourceService");
            }

            var movedResources = 0;

            foreach (var fileInfo in resourcesPath.GetFiles())
            {
                movedResources++;
                resourceService.MoveResourceToStorageAsync(Path.GetFileNameWithoutExtension(fileInfo.Name), fileInfo)
                    .Wait();
            }

            Logger.Debug("Moved {0} resources to storage", movedResources);
        }

        private static DirectoryInfo GetResourcesPath()
        {
            var settings = HostingSettingsProvider.Current.GetSettings();
            return string.IsNullOrEmpty(settings.ResourcesPath)
                                    ? GetRootPath()
                                    : new DirectoryInfo(settings.ResourcesPath);
        }

        private static DirectoryInfo GetRootPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            var fileInfo = new FileInfo(assembly.Location);

            // ReSharper disable once PossibleNullReferenceException
            var resourcesPath = Path.Combine(fileInfo.Directory.FullName, "Resources");
            return new DirectoryInfo(resourcesPath);
        }

        private static void ConfigureMediServer()
        {
            var mediConfig = new MediConfig
                                 {
                                     Peers =
                                         {
                                             new ServerPeerConfig
                                                 {
                                                     Codec = new BecCodecConfig(),
                                                     Transport = new TcpTransportServerConfig()
                                                 }
                                         },
                                     Services =
                                         {
                                             new LocalResourceServiceConfig
                                                 {
                                                     DataStore = new BackgroundSystemResourceDataStoreConfig(),
                                                     ResourceStore = new BackgroundSystemResourceStoreConfig()
                                                 }
                                         }
                                 };
            MessageDispatcher.Instance.Configure(
                new ObjectConfigurator(mediConfig, ApplicationHelper.MachineName, "BackgroundSystem"));
        }
    }
}