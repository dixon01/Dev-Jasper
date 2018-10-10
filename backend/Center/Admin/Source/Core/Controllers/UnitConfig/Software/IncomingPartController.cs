// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncomingPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IncomingPartController type.
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
    /// The part controller for incoming data selection.
    /// </summary>
    public class IncomingPartController : MultiSelectPartControllerBase
    {
        private OutgoingPartController outgoingController;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public IncomingPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Software.Incoming, parent)
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
            var incomingData = partData.GetEnumValue<IncomingData>(0);
            foreach (var option in this.ViewModel.Editor.Options)
            {
                var value = (IncomingData)option.Value;
                option.IsChecked = (incomingData & value) == value;
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
            var incomingData = this.GetSelectedData();
            partData.SetEnumValue(incomingData);
        }

        /// <summary>
        /// Gets the incoming data selected in this editor.
        /// </summary>
        /// <returns>
        /// The <see cref="IncomingData"/>.
        /// </returns>
        public IncomingData GetSelectedData()
        {
            var incomingData = this.ViewModel.Editor.GetCheckedValues()
                .Cast<IncomingData>()
                .Aggregate((IncomingData)0, (a, b) => a | b);
            return incomingData;
        }

        /// <summary>
        /// Checks if the given <paramref name="incomingData"/> is selected in this editor.
        /// </summary>
        /// <param name="incomingData">
        /// The <see cref="IncomingData"/>.
        /// </param>
        /// <returns>
        /// True if it is selected, otherwise false.
        /// </returns>
        public bool HasSelected(IncomingData incomingData)
        {
            return this.ViewModel.Editor.GetCheckedValues().Contains(incomingData);
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.outgoingController = this.GetPart<OutgoingPartController>();
            this.outgoingController.ViewModelUpdated += this.OutgoingControllerOnViewModelUpdated;
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
                DisplayName = AdminStrings.UnitConfig_Software_Incoming,
                Description = AdminStrings.UnitConfig_Software_Incoming_Description
            };
#if !__UseLuminatorTftDisplay
            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Ibis, IncomingData.Ibis));

            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Vdv301, IncomingData.Vdv301));

            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Ximple, IncomingData.Ximple));

            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Ximple, IncomingData.Ximple));
#endif

            //  SNTP option
            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Sntp, IncomingData.Sntp));

            // Medi option
            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Medi, IncomingData.Medi));

#if __UseLuminatorTftDisplay
            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Rss, IncomingData.Ximple));

            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_LamXimple, IncomingData.LamXimple));

            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_AudioPeripheral, IncomingData.AudioPeripheral));

            viewModel.Editor.Options.Add(new CheckableOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_AdHoc, IncomingData.AdHoc));
#endif

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

        private void OutgoingControllerOnViewModelUpdated(object sender, EventArgs e)
        {
            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            if (this.ViewModel == null || this.outgoingController == null || this.outgoingController.ViewModel == null)
            {
                return;
            }

            var errorState = ErrorState.Ok;
            if (!this.ViewModel.Editor.GetCheckedOptions().Any()
                     && !this.outgoingController.ViewModel.Editor.GetCheckedOptions().Any())
            {
                errorState = !this.ViewModel.WasVisited || !this.outgoingController.ViewModel.WasVisited
                    ? ErrorState.Missing
                    : ErrorState.Warning;

            }

            this.ViewModel.Editor.SetError("Options", errorState, AdminStrings.Errors_NoIncomingOutgoing);

            //if (errorState == ErrorState.Ok)
            //{
            //    if (this.HasSelected(IncomingData.LamXimple) && !this.HasSelected(IncomingData.AudioPeripheral))
            //    {
            //        errorState = ErrorState.Error;
            //    }

            //    this.ViewModel.Editor.SetError("Options", errorState, AdminStrings.Errors_LamButNoAudio);
            //}

            //if (errorState == ErrorState.Ok)
            //{
            //    if (this.HasSelected(IncomingData.AudioPeripheral) && !this.HasSelected(IncomingData.LamXimple))
            //    {
            //        errorState = ErrorState.Error;
            //    }

            //    this.ViewModel.Editor.SetError("Options", errorState, AdminStrings.Errors_AudioButNoLam);
            //}

            var mediErrorState = ErrorState.Ok;
            if (errorState == ErrorState.Ok && this.HasSelected(IncomingData.Medi)
                && this.outgoingController.HasSelected(OutgoingData.Medi))
            {
                mediErrorState = ErrorState.Warning;
            }

            this.ViewModel.Editor.SetError("Options", mediErrorState, AdminStrings.Errors_MediMasterAndSlave);
        }
    }
}