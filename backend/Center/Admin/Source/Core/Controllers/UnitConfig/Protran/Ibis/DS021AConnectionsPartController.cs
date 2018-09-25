// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021AConnectionsPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AConnectionsPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// The part controller for DS0021a connections ('A' variant).
    /// </summary>
    public class DS021AConnectionsPartController : FilteredPartControllerBase
    {
        private bool parentVisible;

        private DS021ATelegramPartController partDs021A;

        private SelectionEditorViewModel showForNextStopOnly;

        private GenericUsageEditorViewModel usedFor;

        private GenericUsageEditorViewModel usedForStopName;

        private GenericUsageEditorViewModel usedForLineNumber;

        private GenericUsageEditorViewModel usedForDepartureTime;

        private TextEditorViewModel lineNumberFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021AConnectionsPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DS021AConnectionsPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.IbisProtocol.DS021AConnections, parent)
        {
        }

        /// <summary>
        /// Creates the <see cref="ConnectionConfig"/> edited by this part.
        /// </summary>
        /// <returns>
        /// The <see cref="ConnectionConfig"/>.
        /// </returns>
        public ConnectionConfig GetConfig()
        {
            return new ConnectionConfig
                       {
                           Enabled = this.ViewModel.IsVisible,
                           ShowForNextStopOnly = (bool)this.showForNextStopOnly.SelectedValue,
                           UsedFor = this.usedFor.GenericUsage,
                           UsedForStopName = this.usedForStopName.GenericUsage,
                           UsedForDepartureTime = this.usedForDepartureTime.GenericUsage,
                           LineNumberFormat = this.lineNumberFormat.Text
                       };
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            var config = partData.GetXmlValue<ConnectionConfig>();
            this.showForNextStopOnly.SelectValue(config.ShowForNextStopOnly);
            this.usedFor.GenericUsage = config.UsedFor;
            this.usedForStopName.GenericUsage = config.UsedForStopName;
            this.usedForLineNumber.GenericUsage = config.UsedForLineNumber;
            this.usedForDepartureTime.GenericUsage = config.UsedForDepartureTime;
            this.lineNumberFormat.Text = config.LineNumberFormat;
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetXmlValue(this.GetConfig());
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
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.partDs021A = this.GetPart<DS021ATelegramPartController>();
            this.partDs021A.ViewModelUpdated += (s, e) => this.UpdateVisibility();
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Ibis_DS021aConnections;
            viewModel.Description = AdminStrings.UnitConfig_Ibis_DS021aConnections_Description;

            this.showForNextStopOnly = new SelectionEditorViewModel();
            this.showForNextStopOnly.Label = AdminStrings.UnitConfig_Ibis_DS021aConnections_ShowForStop;
            this.showForNextStopOnly.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ibis_DS021aConnections_ShowForStop_Any, false));
            this.showForNextStopOnly.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ibis_DS021aConnections_ShowForStop_Next, true));
            viewModel.Editors.Add(this.showForNextStopOnly);

            this.usedForStopName = new GenericUsageEditorViewModel();
            this.usedForStopName.Label = AdminStrings.UnitConfig_Ibis_DS021aConnections_UsedForStopName;
            this.usedForStopName.ShouldShowRow = false;
            this.usedForStopName.IsNullable = true;
            viewModel.Editors.Add(this.usedForStopName);

            this.usedFor = new GenericUsageEditorViewModel();
            this.usedFor.Label = AdminStrings.UnitConfig_Ibis_DS021aConnections_UsedFor;
            this.usedFor.ShouldShowRow = false;
            this.usedFor.IsNullable = true;
            viewModel.Editors.Add(this.usedFor);

            this.usedForDepartureTime = new GenericUsageEditorViewModel();
            this.usedForDepartureTime.Label = AdminStrings.UnitConfig_Ibis_DS021aConnections_UsedForDepartureTime;
            this.usedForDepartureTime.ShouldShowRow = false;
            this.usedForDepartureTime.IsNullable = true;
            viewModel.Editors.Add(this.usedForDepartureTime);

            this.usedForLineNumber = new GenericUsageEditorViewModel();
            this.usedForLineNumber.Label = AdminStrings.UnitConfig_Ibis_DS021aConnections_UsedForLineNumber;
            this.usedForLineNumber.ShouldShowRow = false;
            this.usedForLineNumber.IsNullable = true;
            viewModel.Editors.Add(this.usedForLineNumber);

            this.lineNumberFormat = new TextEditorViewModel();
            this.lineNumberFormat.Label = AdminStrings.UnitConfig_Ibis_DS021aConnections_LineNumberFormat;
            viewModel.Editors.Add(this.lineNumberFormat);

            return viewModel;
        }

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.parentVisible
                                       && this.partDs021A.ViewModel.IsVisible
                                       && this.partDs021A.ConnectionsEnabled;
        }
    }
}