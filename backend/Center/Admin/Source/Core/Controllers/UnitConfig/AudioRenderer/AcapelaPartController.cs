// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcapelaPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AcapelaPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.AudioRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Software;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The Acapela part controller.
    /// </summary>
    public class AcapelaPartController : FilteredPartControllerBase
    {
        private const string VoicesKey = "Voices";

        private static readonly Regex AcapelaPackageRegex =
            new Regex(
                "^" + string.Format(PackageIds.Acapela.VoiceFormat.Replace(".", @"\."), @"[^\.]+", @"[^\.]+") + "$");

        private MultiSelectEditorViewModel voices;

        private bool parentVisible;

        private AudioGeneralPartController generalPart;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcapelaPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public AcapelaPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.AudioRenderer.Acapela, parent)
        {
        }

        /// <summary>
        /// The get selected voices.
        /// </summary>
        /// <returns>
        /// The list of selected software packages with the voices.
        /// </returns>
        public IEnumerable<PackageReadOnlyDataViewModel> GetSelectedVoices()
        {
            return this.voices.GetCheckedValues().Cast<PackageReadOnlyDataViewModel>();
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
            this.generalPart = this.GetPart<AudioGeneralPartController>();
            this.generalPart.ViewModelUpdated += (s, e) => this.UpdateVisibility();

            var dataController = this.Parent.Parent.DataController;
            await dataController.Package.AwaitAllDataAsync();
            foreach (var package in dataController.Package.All.Where(p => AcapelaPackageRegex.IsMatch(p.PackageId)))
            {
                this.voices.Options.Add(new CheckableOptionViewModel(package.ProductName, package));
            }
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            var voiceList = partData.GetValue(string.Empty, VoicesKey).Split(';');
            foreach (var option in this.voices.Options)
            {
                var package = (PackageReadOnlyDataViewModel)option.Value;
                option.IsChecked = voiceList.Contains(package.PackageId);
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
            partData.SetValue(string.Join(";", this.GetSelectedVoices().Select(p => p.PackageId)), VoicesKey);
        }

        /// <summary>
        /// Update the visibility of this part.
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if the view model of this part should be visible.
        /// </param>
        public override void UpdateVisibility(bool visible)
        {
            this.parentVisible = visible;
            this.UpdateVisibility();
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Audio_Acapela;
            viewModel.Description = AdminStrings.UnitConfig_Audio_Acapela_Description;

            this.voices = new MultiSelectEditorViewModel();
            this.voices.Label = AdminStrings.UnitConfig_Audio_Acapela_Voices;
            viewModel.Editors.Add(this.voices);

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

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.parentVisible && this.generalPart.UseAcapela;
        }

        private void UpdateErrors()
        {
            var errorState = this.voices.CheckedOptionsCount > 0
                                 ? ErrorState.Ok
                                 : (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing);
            this.voices.SetError("Options", errorState, AdminStrings.Errors_SelectOneAtLeast);
        }
    }
}