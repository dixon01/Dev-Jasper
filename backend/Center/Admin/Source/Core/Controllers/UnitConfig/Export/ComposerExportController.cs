// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComposerExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Composer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.ThorebC90;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.Infomedia.Composer;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// The export controller that exports everything related to Infomedia Composer.
    /// </summary>
    public class ComposerExportController : ExportControllerBase
    {
        private IncomingPartController incoming;

        private BusPartController busPart;

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public override void Initialize(UnitConfiguratorController parentController)
        {
            base.Initialize(parentController);

            this.incoming = this.Parent.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += (s, e) => this.UpdateSoftwarePackages();
            this.busPart = this.Parent.GetPart<BusPartController>();
            this.busPart.ViewModelUpdated += (s, e) => this.UpdateSoftwarePackages();
            this.UpdateSoftwarePackages();
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
        public override async Task CreateExportStructureAsync(List<ExportFolder> rootFolders)
        {
            if (this.SoftwarePackageIds.Count == 0)
            {
                return;
            }

            var application = this.GetSoftwareVersion(PackageIds.Motion.Composer);
            await this.AddPackageVersionFilesAsync(rootFolders, application);

            var composerConfig = this.CreateComposerXml();

            await this.AddVersionedXmlConfigFileAsync(
                composerConfig,
                application,
                @"Config\Composer\Composer.xml",
                rootFolders);

            this.AddFile(
                @"Config\Composer\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            var mediConfig = this.CreateMediClientConfig();
            if (this.incoming.HasSelected(IncomingData.Ximple))
            {
                mediConfig.Peers.Add(
                    new EventHandlerServerPeerConfig { SupportedMessages = { typeof(Ximple).FullName } });
            }

            this.AddXmlConfigFile(mediConfig, @"Config\Composer\medi.config", rootFolders);
        }

        /// <summary>
        /// Creates the application configurations required for System Manager.
        /// </summary>
        /// <returns>
        /// The list of application configurations. The list can be empty, but never null.
        /// </returns>
        public override IEnumerable<ApplicationConfigBase> CreateApplicationConfigs()
        {
            if (this.SoftwarePackageIds.Count == 0)
            {
                yield break;
            }

            yield return
                this.CreateProcessConfig(@"Progs\Composer\Composer.exe", "Composer", 200);
        }

        /// <summary>
        /// Creates the restart dependencies required for Update.
        /// </summary>
        /// <returns>
        /// The list of dependencies. The list can be empty, but never null.
        /// </returns>
        public override IEnumerable<DependencyConfig> CreateRestartDependencies()
        {
            if (this.SoftwarePackageIds.Count == 0)
            {
                yield break;
            }

#if __UseLuminatorTftDisplay
            yield return
                new DependencyConfig
                {
                    Path = @"D:\Presentation\main.im2",
                    ExecutablePaths = { @"D:\Progs\Composer\Composer.exe" }
                };
#else
            var currentVersion = new Version(
                this.GetSoftwareVersion(PackageIds.Motion.Composer).SoftwareVersion);
            if (currentVersion >= SoftwareVersions.Infomedia.WithoutUpdateRestart)
            {
                yield break;
            }

            yield return
                new DependencyConfig
                {
                    Path = @"..\..\Presentation\",
                    ExecutablePaths = { @"..\..\Progs\Composer\Composer.exe" }
                };
#endif
        }

        private ComposerConfig CreateComposerXml()
        {
            var config = new ComposerConfig();

            var general = this.Parent.GetPart<ComposerGeneralPartController>();
            if (general.XimpleInactivityEnabled)
            {
                config.XimpleInactivity.AtStartup = general.AtStartup;
                config.XimpleInactivity.Timeout = general.XimpleInactivityTimeout;
            }
            else
            {
                config.XimpleInactivity.AtStartup = false;
                config.XimpleInactivity.Timeout = TimeSpan.FromDays(3650);
            }

            config.EnablePresentationLogging = general.PresentationLoggingEnabled;

            return config;
        }

        private void UpdateSoftwarePackages()
        {
            if (!this.busPart.ViewModel.IsVisible && (this.incoming.HasSelected(IncomingData.Ibis)
                || this.incoming.HasSelected(IncomingData.Vdv301)
                || this.incoming.HasSelected(IncomingData.Ximple)
                || !this.incoming.HasSelected(IncomingData.Medi)))
            {
                this.AddSoftwarePackageId(PackageIds.Motion.Composer);
            }
            else
            {
                this.RemoveSoftwarePackageId(PackageIds.Motion.Composer);
            }
        }
    }
}
