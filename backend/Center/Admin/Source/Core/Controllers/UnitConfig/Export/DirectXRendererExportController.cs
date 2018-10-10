// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXRendererExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXRendererExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.DirectXRenderer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.Update.Application;

    /// <summary>
    /// The DirectX Renderer export controller.
    /// </summary>
    public class DirectXRendererExportController : ExportControllerBase
    {
        /// <summary>
        /// The application name as shown on the System Manager splash screen.
        /// </summary>
        public static readonly string AppName = "DirectX Renderer";

        private OutgoingPartController outgoing;

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public override void Initialize(UnitConfiguratorController parentController)
        {
            base.Initialize(parentController);
            this.IsRenderer = true;
            this.outgoing = this.Parent.GetPart<OutgoingPartController>();
            this.outgoing.ViewModelUpdated += (s, e) => this.UpdateSoftwarePackages();
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

            var application = this.GetSoftwareVersion(PackageIds.Motion.DirectXRenderer);
            await this.AddPackageVersionFilesAsync(rootFolders, application);

            var composerConfig = await this.CreateDirectXRendererXmlAsync(rootFolders);

            await this.AddVersionedXmlConfigFileAsync(
                composerConfig,
                application,
                @"Config\DirectXRenderer\DirectXRenderer.xml",
                rootFolders);

            this.AddFile(
                @"Config\DirectXRenderer\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            this.AddXmlConfigFile(this.CreateMediClientConfig(), @"Config\DirectXRenderer\medi.config", rootFolders);
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

            var image = this.Parent.GetPart<DirectXImagePartController>();
            var config = this.CreateProcessConfig(
                @"Progs\DirectXRenderer\DirectXRenderer.exe",
                AppName,
                (int)(image.MaxBitmapCacheBytes / 1024 / 1024) + 200);
            config.WindowMode = null;
            config.LaunchDelay = TimeSpan.FromSeconds(5);
            yield return config;
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
                        Path = @"D:\Presentation\",
                        ExecutablePaths = { @"D:\Progs\DirectXRenderer\DirectXRenderer.exe" }
                    };
#else
            var currentVersion = new Version(
                this.GetSoftwareVersion(PackageIds.Motion.DirectXRenderer).SoftwareVersion);
            if (currentVersion >= SoftwareVersions.Infomedia.WithoutUpdateRestart)
            {
                yield break;
            }

            yield return
                new DependencyConfig
                    {
                        Path = @"..\..\Presentation\",
                        ExecutablePaths = { @"..\..\Progs\DirectXRenderer\DirectXRenderer.exe" }
                    };
#endif
        }

        private async Task<RendererConfig> CreateDirectXRendererXmlAsync(List<ExportFolder> rootFolders)
        {
            var config = new RendererConfig();

            var general = this.Parent.GetPart<DirectXGeneralPartController>();
            config.FallbackTimeout = general.FallbackTimeout;

            string fallbackImage = null;
            if (!general.UseDefaultFallbackImage)
            {
                using (
                    var resourceService =
                        this.Parent.DataController.ConnectionController.CreateChannelScope<IResourceService>())
                {
                    var resource = await resourceService.Channel.GetAsync(general.FallbackResourceHash);
                    if (resource != null)
                    {
                        fallbackImage = resource.OriginalFilename
                                        ?? "fallback."
                                        + resource.MimeType.Substring(resource.MimeType.LastIndexOf('/') + 1);
                        this.AddFile(
                            @"Config\DirectXRenderer\" + fallbackImage,
                            rootFolders,
                            name => new ExportResourceFile(name, resource));
                    }
                }
            }

            var screenResolutions = this.Parent.GetPart<ScreenResolutionsPartController>();
            this.AddResolution(screenResolutions.PrimaryResolution, general.PrimaryScreenId, config, fallbackImage);
            this.AddResolution(screenResolutions.SecondaryResolution, general.SecondaryScreenId, config, fallbackImage);

            var text = this.Parent.GetPart<DirectXTextPartController>();
            config.Text.TextMode = text.TextMode;
            config.Text.FontQuality = text.FontQuality;

            var image = this.Parent.GetPart<DirectXImagePartController>();
            config.Image.BitmapCacheTimeout = image.BitmapCacheTimeout;
            config.Image.MaxBitmapCacheBytes = image.MaxBitmapCacheBytes;
            config.Image.MaxCacheBytesPerBitmap = image.MaxCacheBytesPerBitmap;
            if (image.PreloadImages)
            {
                config.Image.PreloadDirectories.Add(@"..\..\Presentation\Pools");
                config.Image.PreloadDirectories.Add(@"..\..\Presentation\Images");
                config.Image.PreloadDirectories.Add(@"..\..\Presentation\Symbols");
            }

            var video = this.Parent.GetPart<DirectXVideoPartController>();
            config.Video.VideoMode = video.VideoRenderingMode ?? VideoMode.DirectShow;

            //// Temporarily fixed, beacause of a bug that fullscreenExclusive is not working
            config.WindowMode = WindowMode.FullScreenWindowed;
            ////config.WindowMode = config.Video.VideoMode == VideoMode.DirectShow
            ////                        ? WindowMode.FullScreenExclusive
            ////                        : WindowMode.FullScreenWindowed;

            return config;
        }

        private void AddResolution(
            ScreenResolution resolution, string screenId, RendererConfig config, string fallbackImage)
        {
            if (resolution == null)
            {
                return;
            }

            var screen = new ScreenConfig
                             {
                                 Width = resolution.PhysicalWidth,
                                 Height = resolution.PhysicalHeight,
                                 FallbackImage = fallbackImage,
                                 VisibleRegion =
                                     new VisibleRegionConfig
                                         {
                                             Width = resolution.VisibleWidth,
                                             Height = resolution.VisibleHeight
                                         }
                             };
            if (!string.IsNullOrEmpty(screenId))
            {
                screen.Id = screenId;
            }

            config.Screens.Add(screen);
        }

        private void UpdateSoftwarePackages()
        {
            if (this.outgoing.HasSelected(OutgoingData.DirectX))
            {
                this.AddSoftwarePackageId(PackageIds.Motion.DirectXRenderer);
            }
            else
            {
                this.RemoveSoftwarePackageId(PackageIds.Motion.DirectXRenderer);
            }
        }
    }
}
