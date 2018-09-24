// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutgoingPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OutgoingPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Software
{
    using System;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The part controller for outgoing data selection.
    /// </summary>
    public class OutgoingPartController : MultiSelectPartControllerBase
    {
        private IncomingPartController incomingController;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public OutgoingPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Software.Outgoing, parent)
        {
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            var outgoingData = partData.GetEnumValue<OutgoingData>(0);
            foreach (var option in this.ViewModel.Editor.Options)
            {
                var value = (OutgoingData)option.Value;
                option.IsChecked = (outgoingData & value) == value;
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
            var outgoingData = this.GetSelectedData();
            partData.SetEnumValue(outgoingData);
        }

        /// <summary>
        /// Gets the outgoing data selected in this editor.
        /// </summary>
        /// <returns>
        /// The <see cref="OutgoingData"/>.
        /// </returns>
        public OutgoingData GetSelectedData()
        {
            var outgoingData = this.ViewModel.Editor.GetCheckedValues()
                .Cast<OutgoingData>()
                .Aggregate((OutgoingData)0, (a, b) => a | b);
            return outgoingData;
        }

        /// <summary>
        /// Checks if the given <paramref name="outgoingData"/> is selected in this editor.
        /// </summary>
        /// <param name="outgoingData">
        /// The <see cref="OutgoingData"/>.
        /// </param>
        /// <returns>
        /// True if it is selected, otherwise false.
        /// </returns>
        public bool HasSelected(OutgoingData outgoingData)
        {
            return this.ViewModel.Editor.GetCheckedValues().Contains(outgoingData);
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.incomingController = this.GetPart<IncomingPartController>();
            this.incomingController.ViewModelUpdated += this.IncomingControllerOnViewModelUpdated;

            this.ViewModel.Editor.Options.Clear();
            this.ViewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Outgoing_DirectX, OutgoingData.DirectX));
            //this.ViewModel.Editor.Options.Add(new CheckableOptionViewModel(
            //    AdminStrings.UnitConfig_Software_Outgoing_Ahdlc, OutgoingData.Ahdlc));

            if (descriptor.OperatingSystem.Version != OperatingSystemVersion.WindowsXPe)
            {
                // Audio Renderer is only available for .NET 3.5 (i.e. > WinXP)
                this.ViewModel.Editor.Options.Add(new CheckableOptionViewModel(
                    AdminStrings.UnitConfig_Software_Outgoing_Audio, OutgoingData.Audio));
            }

            this.ViewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Outgoing_Medi, OutgoingData.Medi));

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
            var viewModel = new MultiSelectPartViewModel
                                {
                                    IsVisible =
                                        this.Parent.Parent.UnitConfiguration.ProductType.UnitType != UnitTypes.Obu,
                                    DisplayName = AdminStrings.UnitConfig_Software_Outgoing,
                                    Description = AdminStrings.UnitConfig_Software_Outgoing_Description
                                };

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

        private void IncomingControllerOnViewModelUpdated(object sender, EventArgs e)
        {
            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            if (this.ViewModel == null || this.incomingController == null || this.incomingController.ViewModel == null)
            {
                return;
            }

            var errorState = ErrorState.Ok;
            if (!this.ViewModel.Editor.GetCheckedOptions().Any()
                     && !this.incomingController.ViewModel.Editor.GetCheckedOptions().Any())
            {
                errorState = !this.ViewModel.WasVisited || !this.incomingController.ViewModel.WasVisited
                    ? ErrorState.Missing
                    : ErrorState.Warning;
            }

            this.ViewModel.Editor.SetError("Options", errorState, AdminStrings.Errors_NoIncomingOutgoing);

            var mediErrorState = ErrorState.Ok;
            if (errorState == ErrorState.Ok && this.HasSelected(OutgoingData.Medi)
                && this.incomingController.HasSelected(IncomingData.Medi))
            {
                mediErrorState = ErrorState.Warning;
            }

            this.ViewModel.Editor.SetError("Options", mediErrorState, AdminStrings.Errors_MediMasterAndSlave);
        }
    }
}