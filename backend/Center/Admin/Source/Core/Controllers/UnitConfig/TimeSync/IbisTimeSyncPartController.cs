// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTimeSyncPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisTimeSyncPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Ibis;

    /// <summary>
    /// The IBIS time sync part controller.
    /// </summary>
    public class IbisTimeSyncPartController : MultiEditorPartControllerBase
    {
        private const string UseDs006AKey = "UseDs006a";
        private const string TimeSyncDataKey = "TimeSyncData";

        private TimeSourcePartController timeSource;

        private SelectionEditorViewModel useDs006ASelection;

        private TimeSpanEditorViewModel initialDelay;

        private NumberEditorViewModel waitTelegrams;

        private TimeSpanEditorViewModel tolerance;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisTimeSyncPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public IbisTimeSyncPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.TimeSync.Ibis, parent)
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
            this.useDs006ASelection.SelectValue(partData.GetValue(false, UseDs006AKey));
            var config = partData.GetXmlValue<TimeSyncConfig>(TimeSyncDataKey);
            this.initialDelay.Value = config.InitialDelay;
            this.tolerance.Value = config.Tolerance;
            this.waitTelegrams.Value = config.WaitTelegrams;
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.ShouldUseDs006A(), UseDs006AKey);
            partData.SetXmlValue(this.GetTimeSyncConfig(), TimeSyncDataKey);
        }

        /// <summary>
        /// The get time sync configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeSyncConfig"/> representing this editor's values.
        /// </returns>
        public TimeSyncConfig GetTimeSyncConfig()
        {
            var config = new TimeSyncConfig();
            config.Enabled = this.ViewModel.IsVisible;

            if (this.initialDelay.Value.HasValue)
            {
                config.InitialDelay = this.initialDelay.Value.Value;
            }

            if (this.tolerance.Value.HasValue)
            {
                config.Tolerance = this.tolerance.Value.Value;
            }

            config.WaitTelegrams = (int)this.waitTelegrams.Value;
            return config;
        }

        /// <summary>
        /// Gets a flag indicating if DS006a should be used (instead of DS005/DS006).
        /// </summary>
        /// <returns>
        /// True if DS006a should be used, otherwise false.
        /// </returns>
        public bool ShouldUseDs006A()
        {
            return (bool)this.useDs006ASelection.SelectedValue;
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
                                    DisplayName = AdminStrings.UnitConfig_TimeSync_Ibis,
                                    Description = AdminStrings.UnitConfig_TimeSync_Ibis_Description
                                };

            this.useDs006ASelection = new SelectionEditorViewModel();
            this.useDs006ASelection.Label = AdminStrings.UnitConfig_TimeSync_Ibis_Records;
            this.useDs006ASelection.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_TimeSync_Ibis_Ds005_Ds006, false));
            this.useDs006ASelection.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_TimeSync_Ibis_Ds006a, true));
            viewModel.Editors.Add(this.useDs006ASelection);

            this.initialDelay = new TimeSpanEditorViewModel();
            this.initialDelay.Label = AdminStrings.UnitConfig_TimeSync_Ibis_InitialDelay;
            viewModel.Editors.Add(this.initialDelay);

            this.waitTelegrams = new NumberEditorViewModel();
            this.waitTelegrams.Label = AdminStrings.UnitConfig_TimeSync_Ibis_WaitTelegrams;
            this.waitTelegrams.IsInteger = true;
            this.waitTelegrams.MinValue = 0;
            this.waitTelegrams.MaxValue = 100;
            viewModel.Editors.Add(this.waitTelegrams);

            this.tolerance = new TimeSpanEditorViewModel();
            this.tolerance.Label = AdminStrings.UnitConfig_TimeSync_Ibis_Tolerance;
            viewModel.Editors.Add(this.tolerance);

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
            this.timeSource = this.GetPart<TimeSourcePartController>();
            this.timeSource.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.timeSource.GetSelectedSource() == IncomingData.Ibis;
        }
    }
}