// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TerminalExportController.cs" company="Gorba AG">
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
    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The export controller that exports everything related to Terminal.
    /// </summary>
    public class TerminalExportController : ExportControllerBase
    {
        /// <summary>
        /// The application name as shown on the System Manager splash screen.
        /// </summary>
        public static readonly string AppName = "Terminal";

        private TerminalPartController terminalPart;

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
            this.terminalPart = this.Parent.GetPart<TerminalPartController>();
            this.terminalPart.ViewModelUpdated += (s, e) => this.UpdateSoftwarePackages();
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

            var application = this.GetSoftwareVersion(PackageIds.Motion.Terminal);
            await this.AddPackageVersionFilesAsync(rootFolders, application);

            var terminalConfig = this.CreateTerminalXml();

            await this.AddVersionedXmlConfigFileAsync(
                terminalConfig,
                application,
                @"Config\Terminal\Terminal.xml",
                rootFolders);

            this.AddFile(
                @"Config\Terminal\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            this.AddXmlConfigFile(this.CreateMediClientConfig(), @"Config\Terminal\medi.config", rootFolders);
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

            var config = this.CreateProcessConfig(@"Progs\Terminal\Terminal.exe", AppName);
            config.WindowMode = null;
            yield return config;
        }

        private TerminalConfig CreateTerminalXml()
        {
            TerminalConfig terminalConfig;
            var config = this.terminalPart.ViewModel.Editor.Config;
            var serializer = new XmlSerializer(typeof(TerminalConfig));
            using (var memoryStream = new MemoryStream(config.Document.Length))
            {
                using (var sw = new StreamWriter(memoryStream))
                {
                    sw.Write(config.Document);
                    sw.Flush();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    terminalConfig = (TerminalConfig)serializer.Deserialize(memoryStream);
                }
            }

            return terminalConfig;
        }

        private void UpdateSoftwarePackages()
        {
            if (this.terminalPart.ViewModel.IsVisible)
            {
                this.AddSoftwarePackageId(PackageIds.Motion.Terminal);
            }
            else
            {
                this.RemoveSoftwarePackageId(PackageIds.Motion.Terminal);
            }
        }
    }
}
