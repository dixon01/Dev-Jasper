// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSourcePartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeSourcePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync
{
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The time source part controller.
    /// </summary>
    public class TimeSourcePartController : MultiEditorPartControllerBase
    {
        private const string SourceKey = "Source";
        private const string BroadcastKey = "Broadcast";

        private const IncomingData NoTimeSync = 0;

        private SelectionEditorViewModel sourceSelection;

        private CheckableEditorViewModel broadcastCheck;

        private IncomingPartController incomingPart;

        private SelectionOptionViewModel ibisSourceOption;

        private SelectionOptionViewModel vdv301SourceOption;

        private SelectionOptionViewModel sntpSourceOption;

        private SelectionOptionViewModel mediSourceOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSourcePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public TimeSourcePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.TimeSync.TimeSource, parent)
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
            this.sourceSelection.SelectValue(partData.GetEnumValue(NoTimeSync, SourceKey));
            this.broadcastCheck.IsChecked = partData.GetValue(false, BroadcastKey);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetEnumValue(this.GetSelectedSource(), SourceKey);
            partData.SetValue(this.HasBroadcastEnabled(), BroadcastKey);
        }

        /// <summary>
        /// Gets the selected source.
        /// Only the following values are possible (and no combination of them):
        /// - <see cref="IncomingData.Ibis"/>
        /// - <see cref="IncomingData.Vdv301"/>
        /// - <see cref="IncomingData.Sntp"/>
        /// or '0', meaning no time sync should be done
        /// </summary>
        /// <returns>
        /// The <see cref="IncomingData"/>.
        /// </returns>
        public IncomingData GetSelectedSource()
        {
            if (this.sourceSelection.SelectedOption == null)
            {
                return NoTimeSync;
            }

            return (IncomingData)this.sourceSelection.SelectedValue;
        }

        /// <summary>
        /// Gets a flag indicating if the broadcast of time changes to other sources is enabled.
        /// </summary>
        /// <returns>
        /// True if enabled, otherwise false.
        /// </returns>
        public bool HasBroadcastEnabled()
        {
            return this.broadcastCheck.IsChecked.HasValue && this.broadcastCheck.IsChecked.Value;
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel
                                {
                                    DisplayName = AdminStrings.UnitConfig_TimeSync_Source,
                                    Description = AdminStrings.UnitConfig_TimeSync_Source_Description
                                };

            this.sourceSelection = new SelectionEditorViewModel();
            this.sourceSelection.Label = AdminStrings.UnitConfig_TimeSync_Source_Selection;
            this.sourceSelection.PropertyChanged += this.SourceSelectionOnPropertyChanged;
            viewModel.Editors.Add(this.sourceSelection);
            this.sourceSelection.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_TimeSync_Source_None, NoTimeSync));

            this.ibisSourceOption = new SelectionOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Ibis,
                IncomingData.Ibis);

            this.vdv301SourceOption = new SelectionOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Vdv301,
                IncomingData.Vdv301);

            this.sntpSourceOption = new SelectionOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Sntp,
                IncomingData.Sntp);

            this.mediSourceOption = new SelectionOptionViewModel(
                AdminStrings.UnitConfig_Software_Incoming_Medi,
                IncomingData.Medi);

            this.broadcastCheck = new CheckableEditorViewModel();
            this.broadcastCheck.Label = AdminStrings.UnitConfig_TimeSync_Broadcast;
            this.broadcastCheck.IsThreeState = false;
            this.broadcastCheck.PropertyChanged += (s, e) => this.UpdateErrors();
            viewModel.Editors.Add(this.broadcastCheck);

            return viewModel;
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.incomingPart = this.GetPart<IncomingPartController>();
            this.incomingPart.ViewModelUpdated += (s, e) => this.UpdateFromIncoming();

            this.UpdateFromIncoming();
        }

        private void UpdateFromIncoming()
        {
            this.UpdateOption(this.ibisSourceOption);
            this.UpdateOption(this.vdv301SourceOption);
            this.UpdateOption(this.sntpSourceOption);
            this.UpdateOption(this.mediSourceOption);

            this.ViewModel.IsVisible = this.sourceSelection.Options.Count > 1;
            this.UpdateBroadcastEnabled();

            this.UpdateErrors();
        }

        private void UpdateOption(SelectionOptionViewModel option)
        {
            var incomingData = (IncomingData)option.Value;
            var hasSelected = this.incomingPart.HasSelected(incomingData);
            var containsOption = this.sourceSelection.Options.Contains(option);
            if (hasSelected == containsOption)
            {
                return;
            }

            if (hasSelected)
            {
                this.sourceSelection.Options.Add(option);
                return;
            }

            if (incomingData.Equals(this.sourceSelection.SelectedValue))
            {
                // reset the selected option
                this.sourceSelection.SelectedOption = null;
            }

            this.sourceSelection.Options.Remove(option);
        }

        private void UpdateErrors()
        {
            var state = this.GetSelectedSource() == IncomingData.Medi && !this.HasBroadcastEnabled()
                            ? ErrorState.Warning
                            : ErrorState.Ok;
            this.broadcastCheck.SetError("IsChecked", state, AdminStrings.UnitConfig_TimeSync_Broadcast_MediWarning);
        }

        private void SourceSelectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedOption")
            {
                return;
            }

            this.sourceSelection.SetError(
                "SelectedOption",
                this.sourceSelection.SelectedOption == null ? ErrorState.Error : ErrorState.Ok,
                AdminStrings.Errors_NoItemSelected);
            this.UpdateBroadcastEnabled();
            this.UpdateErrors();
        }

        private void UpdateBroadcastEnabled()
        {
            this.broadcastCheck.IsEnabled = this.sourceSelection.SelectedValue != null
                                            && (IncomingData)this.sourceSelection.SelectedValue != NoTimeSync;
        }
    }
}