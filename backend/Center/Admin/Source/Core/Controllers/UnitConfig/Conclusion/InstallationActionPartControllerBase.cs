// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationActionPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationActionPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The software versions part controller.
    /// </summary>
    public class InstallationActionPartControllerBase : PartControllerBase<InstallationActionPartViewModel>
    {
        private readonly string displayName;

        private readonly string description;

        private ExportPreparationPartController exportPreparation;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationActionPartControllerBase"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="uniquePartName">
        /// Unique key to identify part.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public InstallationActionPartControllerBase(
            CategoryControllerBase parent,
            string uniquePartName,
            string displayName,
            string description)
            : base(uniquePartName, parent)
        {
            this.displayName = displayName;
            this.description = description;
        }

        /// <summary>
        /// The prepare async.
        /// </summary>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override Task PrepareAsync(HardwareDescriptor descriptor)
        {
            this.ViewModel.IsVisible = false;
            this.exportPreparation = this.GetPart<ExportPreparationPartController>();
            this.exportPreparation.ViewModelUpdated += this.ExportPreparationUpdated;
            return Task.FromResult(0);
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="partData">
        /// The part data.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="partData">
        /// The part data.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override InstallationActionPartViewModel CreateViewModel()
        {
            var viewModel = new InstallationActionPartViewModel
                            {
                                DisplayName = this.displayName,
                                Description = this.description,
                                IsVisible = true
                            };

            return viewModel;
        }

        private void ExportPreparationUpdated(object sender, EventArgs e)
        {
            this.ViewModel.IsVisible = this.exportPreparation != null
                                       && !this.exportPreparation.ViewModel.Editor.HasErrors
                                       && !this.exportPreparation.ViewModel.Editor.IsLoading
                                       && !this.exportPreparation.ViewModel.Editor.ShouldReload
                                       && this.exportPreparation.ViewModel.Editor.ExportFolders.Any();
        }
    }
}