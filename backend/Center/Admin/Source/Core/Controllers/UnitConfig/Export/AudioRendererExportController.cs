// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioRendererExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioRendererExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.AudioRenderer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.Infomedia.AudioRenderer;
    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The export controller that exports everything related to Audio Renderer.
    /// </summary>
    public class AudioRendererExportController : ExportControllerBase
    {
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
        public async override Task CreateExportStructureAsync(List<ExportFolder> rootFolders)
        {
            if (this.SoftwarePackageIds.Count == 0)
            {
                return;
            }

            var application = this.GetSoftwareVersion(PackageIds.Motion.AudioRenderer);
            await this.AddPackageVersionFilesAsync(rootFolders, application);

            var config = this.CreateAudioRendererXml();

            await this.AddVersionedXmlConfigFileAsync(
                config,
                application,
                @"Config\AudioRenderer\AudioRenderer.xml",
                rootFolders);

            this.AddFile(
                @"Config\AudioRenderer\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            this.AddXmlConfigFile(this.CreateMediClientConfig(), @"Config\AudioRenderer\medi.config", rootFolders);
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
                this.CreateProcessConfig(@"Progs\AudioRenderer\AudioRenderer.exe", "Audio Renderer", 150, 50);
        }

        private AudioRendererConfig CreateAudioRendererXml()
        {
            var general = this.Parent.GetPart<AudioGeneralPartController>();
            var config = new AudioRendererConfig();
            config.IO.VolumePort = new IOPortConfig { PortName = "SystemVolume" };

            for (int i = 1; i <= general.NumberOfChannels; i++)
            {
                var key = string.Format(UnitConfigKeys.AudioRenderer.ChannelFormat, i);
                var channel = this.Parent.GetPart<AudioChannelPartController>(key);

                var channelConfig = new AudioChannelConfig { Id = i.ToString(CultureInfo.InvariantCulture) };
#if __UseLuminatorTftDisplay
                if (key.Contains("3"))
                {
                    channelConfig.SpeakerPorts.Add(new IOPortConfig { PortName = "Both" });
                }
                else
#endif
                {
                    foreach (var speakerPort in channel.GetSpeakerPorts())
                    {
                        channelConfig.SpeakerPorts.Add(new IOPortConfig { PortName = speakerPort });
                    }
                }

                config.AudioChannels.Add(channelConfig);
            }

            if (general.UseAcapela)
            {
                config.TextToSpeech.Api = TextToSpeechApi.Acapela;
                config.TextToSpeech.HintPath = @"..\..\Progs\Acapela";
            }
            else
            {
                config.TextToSpeech.Api = TextToSpeechApi.Microsoft;
                config.TextToSpeech.HintPath = string.Empty;
            }

            return config;
        }

        private void UpdateSoftwarePackages()
        {
            if (this.outgoing.HasSelected(OutgoingData.Audio))
            {
                this.AddSoftwarePackageId(PackageIds.Motion.AudioRenderer);
            }
            else
            {
                this.RemoveSoftwarePackageId(PackageIds.Motion.AudioRenderer);
            }
        }
    }
}
