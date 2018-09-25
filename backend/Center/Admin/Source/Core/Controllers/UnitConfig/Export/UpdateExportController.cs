// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Update;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Medi.Core.Resources;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The export controller that exports everything related to Update.
    /// </summary>
    public class UpdateExportController : ExportControllerBase
    {
        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public override void Initialize(UnitConfiguratorController parentController)
        {
            base.Initialize(parentController);
            this.AddSoftwarePackageId(PackageIds.Motion.Update);
        }

        /// <summary>
        /// Asynchronously creates the part of the folder structure for which this controller is responsible.
        /// </summary>
        /// <param name="rootFolders">
        /// The root folders to fill with the structure.
        /// Implementers should only add files and folder, but never remove anything created by another controller.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait for the completion of this method.
        /// </returns>
        public async override Task CreateExportStructureAsync(List<ExportFolder> rootFolders)
        {
            var updateApplication = this.GetSoftwareVersion(PackageIds.Motion.Update);
            await this.AddPackageVersionFilesAsync(rootFolders, updateApplication);

            var updateConfig = this.CreateUpdateXml();

            await this.AddVersionedXmlConfigFileAsync(
                updateConfig,
                updateApplication,
                @"Config\Update\Update.xml",
                rootFolders);

            this.AddFile(
                @"Config\Update\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            var mediConfig = this.CreateMediClientConfig();
            mediConfig.Services.Add(new LocalResourceServiceConfig
                                        {
                                            ResourceDirectory = @"..\..\Data\Update\Medi\Resources"
                                        });
            this.AddXmlConfigFile(mediConfig, @"Config\Update\medi.config", rootFolders);
        }

        /// <summary>
        /// Creates the application configurations required for System Manager.
        /// </summary>
        /// <returns>
        /// The list of application configurations. The list can be empty, but never null.
        /// </returns>
        public override IEnumerable<ApplicationConfigBase> CreateApplicationConfigs()
        {
            yield return
                this.CreateProcessConfig(@"Progs\Update\Update.exe", "Update", 200);
        }

        private UpdateConfig CreateUpdateXml()
        {
            var updateConfig = new UpdateConfig();

            var resolutions = this.Parent.GetPart<ScreenResolutionsPartController>();
            var resolution = resolutions.PrimaryResolution ?? resolutions.SecondaryResolution;
            updateConfig.Visualization.SplashScreen.X = 0;
            updateConfig.Visualization.SplashScreen.Y = 0;
            updateConfig.Visualization.SplashScreen.Width = resolution.VisibleWidth;
            updateConfig.Visualization.SplashScreen.Height = resolution.VisibleHeight;

            updateConfig.CacheLimits = new CacheLimitsConfig { Enabled = true, FreeSpaceMb = 100, NumberOfFiles = 250 };

            var agent = this.Parent.GetPart<UpdateAgentPartController>();
            updateConfig.Agent = new AgentConfig { Enabled = true, ShowVisualization = agent.ShouldShowVisualizations };
            updateConfig.Agent.RestartApplications = this.CreateRestartApplications();

            var methods = this.Parent.GetPart<UpdateMethodsPartController>();
            if (methods.HasUsbChecked)
            {
                var usb = this.Parent.GetPart<UsbUpdatePartController>();
                updateConfig.Clients.Add(
                    new UsbUpdateClientConfig
                        {
                            Name = "USB_E",
                            RepositoryBasePath = @"E:\Gorba\Update",
                            UsbDetectionTimeOut = TimeSpan.FromSeconds(3),
                            PollInterval = TimeSpan.FromSeconds(30),
                            ShowVisualization = usb.ShouldShowVisualizations
                        });
            }

            if (methods.HasAzureChecked)
            {
                var state = ServiceLocator.Current.GetInstance<IConnectedApplicationState>();
                Uri server;
                if (Uri.TryCreate(state.LastServer, UriKind.Absolute, out server)
                    || Uri.TryCreate(string.Format("http://{0}", state.LastServer), UriKind.Absolute, out server))
                {
                    var azure = this.Parent.GetPart<AzureUpdatePartController>();
                    updateConfig.Clients.Add(
                        new AzureUpdateClientConfig
                        {
                            Name = "AzureUpdateClient",
                            RepositoryUrl = new Uri(server, "Repository").ToString(),
                            ShowVisualization = azure.ShouldShowVisualizations
                        });
                }
            }

            if (methods.HasFtpChecked)
            {
                ConfigureFtpUpdateClient(updateConfig, methods.HasRequireWifiChecked);
            }

            if (methods.HasMediMasterChecked)
            {
                var mediMaster = this.Parent.GetPart<MediMasterUpdatePartController>();
                updateConfig.Providers.Add(
                    new MediUpdateProviderConfig
                    {
                        Name = "MediProvider",
                        ShowVisualization = mediMaster.ShouldShowVisualizations
                    });
            }

            if (methods.HasMediSlaveChecked)
            {
                var mediSlave = this.Parent.GetPart<MediSlaveUpdatePartController>();
                updateConfig.Clients.Add(
                    new MediUpdateClientConfig
                    {
                        Name = "MediClient",
                        ShowVisualization = mediSlave.ShouldShowVisualizations
                    });
            }

            return updateConfig;
        }

        private void ConfigureFtpUpdateClient(UpdateConfig updateConfig, bool requireWifiNetworkConnection)
        {
            var ftp = this.Parent.GetPart<FtpUpdatePartController>();
            var index = 0;
            foreach (var provider in ftp.GetSelectedProviders())
            {
                updateConfig.Clients.Add(
                    new FtpUpdateClientConfig
                        {
                            Name = "FTP-" + index,
                            Host = provider.Host,
                            Port = provider.Port,
                            RepositoryBasePath = provider.RepositoryBasePath,
                            Username = provider.Username,
                            Password = provider.Password,
                            PollInterval = ftp.FtpUpdateIntervall,
                            ShowVisualization = ftp.ShouldShowVisualizations,
                            RequireWifiNetworkConnection = requireWifiNetworkConnection
                        });
                index++;
            }
        }

        private RestartApplicationsConfig CreateRestartApplications()
        {
            var config = new RestartApplicationsConfig();

            foreach (
                var dependency in this.Parent.GetExportControllers().SelectMany(c => c.CreateRestartDependencies()))
            {
                var existing =
                    config.Dependencies.FirstOrDefault(
                        d => d.Path.Equals(dependency.Path, StringComparison.InvariantCultureIgnoreCase));
                if (existing != null)
                {
                    existing.ExecutablePaths.AddRange(dependency.ExecutablePaths);
                    continue;
                }

                config.Dependencies.Add(dependency);
            }

            return config;
        }
    }
}
