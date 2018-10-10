// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcRendererExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcRendererExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.AhdlcRenderer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The AHDLC Renderer export controller.
    /// </summary>
    public class AhdlcRendererExportController : ExportControllerBase
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

            var ahdlcRenderer = this.GetSoftwareVersion(PackageIds.Motion.AhdlcRenderer);
            await this.AddPackageVersionFilesAsync(rootFolders, ahdlcRenderer);

            var ahdlcRendererConfig = this.CreateAhdlcRendererXml();

            await this.AddVersionedXmlConfigFileAsync(
                ahdlcRendererConfig,
                ahdlcRenderer,
                @"Config\AhdlcRenderer\AhdlcRenderer.xml",
                rootFolders);

            this.AddFile(
                @"Config\AhdlcRenderer\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            this.AddXmlConfigFile(this.CreateMediClientConfig(), @"Config\AhdlcRenderer\medi.config", rootFolders);
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
                this.CreateProcessConfig(@"Progs\AhdlcRenderer\AhdlcRenderer.exe", "AHDLC Renderer", 150, 80);
        }

        private AhdlcRendererConfig CreateAhdlcRendererXml()
        {
            var general = this.Parent.GetPart<AhdlcGeneralPartController>();
            var config = new AhdlcRendererConfig();
            var channel = new ChannelConfig();
            channel.SerialPort.ComPort = general.ComPort;

            foreach (var address in general.GetCheckedAddresses())
            {
                var key = string.Format(UnitConfigKeys.AhdlcRenderer.SignFormat, address);
                var controller = this.Parent.GetPart<AhdlcSignPartController>(key);
                channel.Signs.Add(
                    new SignConfig
                        {
                            Address = address,
                            Mode = controller.SignMode,
                            Width = controller.Width,
                            Height = controller.Height,
                            Brightness = controller.Brightness
                        });
            }

            config.Channels.Add(channel);
            return config;
        }

        private void UpdateSoftwarePackages()
        {
            if (this.outgoing.HasSelected(OutgoingData.Ahdlc))
            {
                this.AddSoftwarePackageId(PackageIds.Motion.AhdlcRenderer);
            }
            else
            {
                this.RemoveSoftwarePackageId(PackageIds.Motion.AhdlcRenderer);
            }
        }
    }
}
