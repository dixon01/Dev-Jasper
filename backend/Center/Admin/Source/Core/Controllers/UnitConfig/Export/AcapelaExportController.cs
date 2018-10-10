// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcapelaExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AcapelaExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.AudioRenderer;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.DataViewModels.Software;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The export controller that exports everything related to Acapela TTS.
    /// </summary>
    public class AcapelaExportController : ExportControllerBase
    {
        private readonly Dictionary<string, PackageReadOnlyDataViewModel> allAcapelaVoices =
            new Dictionary<string, PackageReadOnlyDataViewModel>();

        private AcapelaPartController acapela;

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

            this.acapela = this.Parent.GetPart<AcapelaPartController>();
            this.outgoing = this.Parent.GetPart<OutgoingPartController>();

            this.acapela.ViewModelUpdated += (s, e) => this.UpdateSoftwarePackages();
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
            foreach (var packageId in this.SoftwarePackageIds)
            {
                var voice = this.GetSoftwareVersion(packageId);
                await this.AddPackageVersionFilesAsync(rootFolders, voice);
            }
        }

        /// <summary>
        /// Creates the application configurations required for System Manager.
        /// </summary>
        /// <returns>
        /// The list of application configurations. The list can be empty, but never null.
        /// </returns>
        public override IEnumerable<ApplicationConfigBase> CreateApplicationConfigs()
        {
            // Acapela has no executable that needs to be started
            yield break;
        }

        private void UpdateSoftwarePackages()
        {
            this.allAcapelaVoices.Keys.ForEach(id => this.RemoveSoftwarePackageId(id));
            if (!this.outgoing.HasSelected(OutgoingData.Audio))
            {
                this.RemoveSoftwarePackageId(PackageIds.Acapela.Application);
                return;
            }

            this.AddSoftwarePackageId(PackageIds.Acapela.Application);
            foreach (var package in this.acapela.GetSelectedVoices())
            {
                this.AddSoftwarePackageId(package.PackageId);
                this.allAcapelaVoices[package.PackageId] = package;
            }
        }
    }
}