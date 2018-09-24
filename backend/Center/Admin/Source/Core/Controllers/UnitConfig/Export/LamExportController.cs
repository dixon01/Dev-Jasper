// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LamExportController.cs" company="Luminator LTG">
//   Copyright © 2016 Luminator LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The export controller that exports everything related to a LAM.
    /// </summary>
    public class LamExportController : ExportControllerBase
    {
        /// <summary>
        /// The application name as shown on the System Manager splash screen.
        /// </summary>
        public static readonly string AppName = "LAM";

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public override void Initialize(UnitConfiguratorController parentController)
        {
            base.Initialize(parentController);
            AddSoftwarePackageId(PackageIds.Luminator.Lam);
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

            var lamApplication = GetSoftwareVersion(PackageIds.Luminator.Lam);
            await AddPackageVersionFilesAsync(rootFolders, lamApplication);

            //var lamConfig = await CreateLamXmlAsync(rootFolders);
            //await this.AddVersionedXmlConfigFileAsync(lamConfig, lamApplication, @"Config\Lam\Lam.xml", rootFolders);
        }

        /// <summary>
        /// Creates the application configurations required for LAM.
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

            var config = this.CreateProcessConfig(@"\Lam\GTFS\goodle_transit.zip", AppName);
            config.WindowMode = null;
            yield return config;
        }

        //private async Task<LamConfig> CreateLamXmlAsync(List<ExportFolder> rootFolders)
        //{
        //    var lamConfig = new LamConfig();

        //    return lamConfig;
        //}

        private void UpdateSoftwarePackages()
        {
            // TODO: Will want to check an export parameter to know whether or not to include
            // this software package ID.
            if (true)
            {
                AddSoftwarePackageId(PackageIds.Luminator.Lam);
            }
            else
            {
                RemoveSoftwarePackageId(PackageIds.Luminator.Lam);
            }
        }
    }
}

