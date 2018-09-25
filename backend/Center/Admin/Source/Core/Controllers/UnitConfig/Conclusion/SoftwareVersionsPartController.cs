// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftwareVersionsPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SoftwareVersionsPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The software versions part controller.
    /// </summary>
    public class SoftwareVersionsPartController : MultiEditorPartControllerBase
    {
        private readonly List<PackageVersionSelectionViewModel> versionSelections =
            new List<PackageVersionSelectionViewModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftwareVersionsPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public SoftwareVersionsPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Conclusion.SoftwareVersions, parent)
        {
        }

        /// <summary>
        /// Gets the selected software versions.
        /// </summary>
        /// <returns>
        /// A dictionary mapping the package id (string) to a <see cref="PackageVersionReadableModel"/>.
        /// The mapped version might be null if the user didn't select a version.
        /// </returns>
        public IDictionary<string, PackageVersionReadableModel> GetSelectedPackageVersions()
        {
            return this.versionSelections.ToDictionary(
                v => v.Package.PackageId,
                v => (PackageVersionReadableModel)v.SelectedValue);
        }

        /// <summary>
        /// Asynchronously prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public async override Task PrepareAsync(HardwareDescriptor descriptor)
        {
            var versions =
                await
                this.Parent.Parent.DataController.ConnectionController.PackageVersionChangeTrackingManager.QueryAsync(
                    PackageVersionQuery.Create().IncludePackage().IncludeStructure());
            foreach (var version in versions)
            {
                var versionSelection = this.versionSelections.FirstOrDefault(p => p.Package.Id == version.Package.Id);
                if (versionSelection == null)
                {
                    versionSelection = new PackageVersionSelectionViewModel(version.Package);
                    this.versionSelections.Add(versionSelection);
                }

                versionSelection.Options.Add(new SelectionOptionViewModel(version.SoftwareVersion, version));
            }

            this.versionSelections.Sort(
                (a, b) => string.Compare(a.Label, b.Label, StringComparison.CurrentCultureIgnoreCase));
            foreach (var versionSelection in this.versionSelections)
            {
                var packageVersions = versionSelection.Options.ToList();
                versionSelection.Options.Clear();
                packageVersions.Sort(
                    (a, b) =>
                    ((PackageVersionReadableModel)a.Value).Id.CompareTo(((PackageVersionReadableModel)b.Value).Id));
                packageVersions.ForEach(versionSelection.Options.Add);

                versionSelection.SelectValue(packageVersions.Select(o => o.Value).LastOrDefault());
            }

            this.Parent.Parent.ViewModel.PropertyChanged += this.UnitConfiguratorOnPropertyChanged;
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            foreach (var partValue in partData.Values)
            {
                var versionSelection = this.versionSelections.FirstOrDefault(p => p.Package.PackageId == partValue.Key);
                if (versionSelection != null)
                {
                    if (versionSelection.SelectValue(
                            versionSelection.Options.Select(o => (PackageVersionReadableModel)o.Value)
                                .FirstOrDefault(v => v.SoftwareVersion == partValue.Value)))
                    {
                        versionSelection.ClearDirty();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            foreach (var versionSelection in this.versionSelections.Where(p => p.SelectedValue != null))
            {
                partData.SetValue(
                    ((PackageVersionReadableModel)versionSelection.SelectedValue).SoftwareVersion,
                    versionSelection.Package.PackageId);
            }
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Conclusion_SoftwareVersions;
            viewModel.Description = AdminStrings.UnitConfig_Conclusion_SoftwareVersions_Description;
            viewModel.IsVisible = true;

            return viewModel;
        }

        private void UnitConfiguratorOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedItem")
            {
                return;
            }

            if (this.Parent.Parent.ViewModel.SelectedItem != this.ViewModel)
            {
                return;
            }

            this.ViewModel.Editors.Clear();
            var packageIds = this.Parent.Parent.GetExportControllers().SelectMany(c => c.SoftwarePackageIds).ToList();
            foreach (
                var versionSelection in this.versionSelections.Where(p => packageIds.Contains(p.Package.PackageId)))
            {
                this.ViewModel.Editors.Add(versionSelection);
            }
        }

        private class PackageVersionSelectionViewModel : SelectionEditorViewModel
        {
            public PackageVersionSelectionViewModel(PackageReadableModel package)
            {
                this.Package = package;
                this.Label = package.ProductName;
            }

            public PackageReadableModel Package { get; private set; }
        }
    }
}