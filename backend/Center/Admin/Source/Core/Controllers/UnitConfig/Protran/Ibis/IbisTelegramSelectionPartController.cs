// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTelegramSelectionPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisTelegramSelectionPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The IBIS telegram selection part controller.
    /// </summary>
    public class IbisTelegramSelectionPartController : MultiSelectPartControllerBase, IFilteredPartController
    {
        // read: "<Key> requires <Value>"
        private static readonly KeyValuePair<string, string>[] TelegramDependencies =
            {
                new KeyValuePair<string, string>("DS021a", "DS010b"),
                new KeyValuePair<string, string>("DS021c", "DS010j"),
                new KeyValuePair<string, string>("GO001", "DS010b"),
                new KeyValuePair<string, string>("GO005", "DS010"),
                new KeyValuePair<string, string>("GO007", "DS009")
            };

        private static readonly KeyValuePair<string, string>[] TelegramExclusives =
            {
                new KeyValuePair<string, string>("DS003", "GO006"),
                new KeyValuePair<string, string>("DS010", "DS010j"),
                new KeyValuePair<string, string>("DS021", "GO005"),
                new KeyValuePair<string, string>("DS021", "GO007"),
                new KeyValuePair<string, string>("GO005", "GO007")
            };

        private readonly IbisProtocolCategoryController parent;

        private SoftwareVersionsPartController softwareVersions;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisTelegramSelectionPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public IbisTelegramSelectionPartController(IbisProtocolCategoryController parent)
            : base(UnitConfigKeys.IbisProtocol.TelegramSelection, parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the list of selected telegram names.
        /// The names are taken from <see cref="IbisTelegramPartControllerBase.TelegramName"/>.
        /// </summary>
        /// <returns>
        /// The list of selected telegram names.
        /// </returns>
        public IEnumerable<string> GetSelectedTelegrams()
        {
            return
                this.ViewModel.Editor.GetCheckedValues()
                    .Cast<IbisTelegramPartControllerBase>()
                    .Select(c => c.TelegramName);
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            foreach (var option in this.ViewModel.Editor.Options)
            {
                var controller = (IbisTelegramPartControllerBase)option.Value;
                option.IsChecked = partData.GetValue(false, controller.TelegramName);
            }

            this.UpdateErrors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            foreach (var option in this.ViewModel.Editor.Options)
            {
                var controller = (IbisTelegramPartControllerBase)option.Value;
                partData.SetValue(option.IsChecked, controller.TelegramName);
            }
        }

        /// <summary>
        /// Update the visibility of this part.
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if the view model of this part should be visible.
        /// </param>
        public void UpdateVisibility(bool visible)
        {
            this.ViewModel.IsVisible = visible;
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            foreach (var controller in this.parent.GetTelegramControllers())
            {
                this.ViewModel.Editor.Options.Add(new CheckableOptionViewModel(controller.TelegramName, controller));
            }

            this.softwareVersions = this.GetPart<SoftwareVersionsPartController>();
            this.softwareVersions.ViewModelUpdated += (s, e) => this.UpdateErrors();

            this.UpdateErrors();
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiSelectPartViewModel"/>.
        /// </returns>
        protected override MultiSelectPartViewModel CreateViewModel()
        {
            var viewModel = new MultiSelectPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Ibis_TelegramSelection;
            viewModel.Description = AdminStrings.UnitConfig_Ibis_TelegramSelection_Description;

            return viewModel;
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);
            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            var selectedTelegrams = this.GetSelectedTelegrams().ToList();
            var errorState = selectedTelegrams.Count > 0
                                 ? ErrorState.Ok
                                 : (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing);
            this.ViewModel.Editor.SetError("Options", errorState, AdminStrings.Errors_SelectOneAtLeast);

            foreach (var telegramExclusive in TelegramExclusives)
            {
                errorState = selectedTelegrams.Contains(telegramExclusive.Key)
                             && selectedTelegrams.Contains(telegramExclusive.Value)
                                 ? ErrorState.Error
                                 : ErrorState.Ok;
                var message = string.Format(
                    AdminStrings.UnitConfig_Ibis_TelegramSelection_XexcludesY_Format,
                    telegramExclusive.Key,
                    telegramExclusive.Value);
                this.ViewModel.Editor.SetError("Options", errorState, message);
            }

            foreach (var telegramDependency in TelegramDependencies)
            {
                errorState = selectedTelegrams.Contains(telegramDependency.Key)
                             && !selectedTelegrams.Contains(telegramDependency.Value)
                                 ? ErrorState.Error
                                 : ErrorState.Ok;
                var message = string.Format(
                    AdminStrings.UnitConfig_Ibis_TelegramSelection_XrequiresY_Format,
                    telegramDependency.Key,
                    telegramDependency.Value);
                this.ViewModel.Editor.SetError("Options", errorState, message);
            }

            this.SetTelegramVersionError("GO007", SoftwareVersions.Protran.SupportsGO007);
        }

        private void SetTelegramVersionError(string telegramName, Version minimumProtranVersion)
        {
            if (this.softwareVersions == null)
            {
                return;
            }

            PackageVersionReadableModel version;
            var errorState = ErrorState.Ok;
            if (this.softwareVersions.GetSelectedPackageVersions().TryGetValue(PackageIds.Motion.Protran, out version)
                && new Version(version.SoftwareVersion) < minimumProtranVersion
                && this.GetSelectedTelegrams().Any(t => t == telegramName))
            {
                errorState = ErrorState.Warning;
            }

            var message = string.Format(
                AdminStrings.UnitConfig_SettingValidator_UnsupportedValueFormat,
                telegramName,
                string.Empty,
                version != null ? version.Package.ProductName : string.Empty,
                minimumProtranVersion);
            this.ViewModel.Editor.SetError("Options", errorState, message);
        }
    }
}