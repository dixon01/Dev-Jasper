// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusModuleExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.ThorebC90;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.Obc.Bus;
    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The export controller that exports everything related to Bus module.
    /// </summary>
    public class BusModuleExportController : ExportControllerBase
    {
        /// <summary>
        /// The application name as shown on the System Manager splash screen.
        /// </summary>
        public static readonly string AppName = "Bus";

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
            this.IsRenderer = true;
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

            var application = this.GetSoftwareVersion(PackageIds.Motion.Bus);
            await this.AddPackageVersionFilesAsync(rootFolders, application);

            var busConfig = this.CreateBusXml();

            await this.AddVersionedXmlConfigFileAsync(
                busConfig,
                application,
                @"Config\Bus\Bus.xml",
                rootFolders);

            this.AddFile(
                @"Config\Bus\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            this.AddXmlConfigFile(this.CreateMediClientConfig(), @"Config\Bus\medi.config", rootFolders);
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

            var config = this.CreateProcessConfig(@"Progs\Bus\Bus.exe", AppName);
            config.WindowMode = null;
            yield return config;
        }

        private BusConfig CreateBusXml()
        {
            BusConfig busConfig;
            var config = this.busPart.ViewModel.Editor.Config;
            var serializer = new XmlSerializer(typeof(BusConfig));
            using (var memoryStream = new MemoryStream(config.Document.Length))
            {
                using (var sw = new StreamWriter(memoryStream))
                {
                    sw.Write(config.Document);
                    sw.Flush();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    busConfig = (BusConfig)serializer.Deserialize(memoryStream);
                }
            }

            return busConfig;
        }

        private void UpdateSoftwarePackages()
        {
            if (this.busPart.ViewModel.IsVisible)
            {
                this.AddSoftwarePackageId(PackageIds.Motion.Bus);
            }
            else
            {
                this.RemoveSoftwarePackageId(PackageIds.Motion.Bus);
            }
        }
    }
}
