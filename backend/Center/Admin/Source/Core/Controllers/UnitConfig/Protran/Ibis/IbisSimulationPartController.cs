// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisSimulationPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisSimulationPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using System;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Export;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Ibis;

    /// <summary>
    /// The IBIS simulation part controller.
    /// </summary>
    public class IbisSimulationPartController : IbisSourcePartControllerBase
    {
        private const string InitialDelayKey = "InitialDelay";
        private const string IntervalBetweenKey = "IntervalBetween";
        private const string TimesToRepeatKey = "TimesToRepeat";

        private TimeSpanEditorViewModel initialDelay;

        private TimeSpanEditorViewModel intervalBetween;

        private NumberEditorViewModel timesToRepeat;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisSimulationPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public IbisSimulationPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.IbisProtocol.Simulation, IbisSourceType.Simulation, parent)
        {
        }

        /// <summary>
        /// Gets the initial delay.
        /// </summary>
        public TimeSpan InitialDelay
        {
            get
            {
                // ReSharper disable once PossibleInvalidOperationException
                return this.initialDelay.Value.Value;
            }
        }

        /// <summary>
        /// Gets the interval between telegrams.
        /// </summary>
        public TimeSpan? IntervalBetweenTelegrams
        {
            get
            {
                return this.intervalBetween.Value;
            }
        }

        /// <summary>
        /// Gets the number of times to repeat.
        /// </summary>
        public int TimesToRepeat
        {
            get
            {
                return (int)this.timesToRepeat.Value;
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
            this.initialDelay.Value = partData.GetValue(TimeSpan.FromSeconds(0), InitialDelayKey);
            this.intervalBetween.Value = partData.GetValue((TimeSpan?)null, IntervalBetweenKey);
            this.timesToRepeat.Value = partData.GetValue(1, TimesToRepeatKey);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.InitialDelay, InitialDelayKey);
            partData.SetValue(this.IntervalBetweenTelegrams, IntervalBetweenKey);
            partData.SetValue(this.TimesToRepeat, TimesToRepeatKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Ibis_Simulation;
            viewModel.Description = string.Format(
                AdminStrings.UnitConfig_Ibis_Simulation_DescriptionFormat,
                ProtranExportController.SimulationFileLocation);

            this.initialDelay = new TimeSpanEditorViewModel();
            this.initialDelay.Label = AdminStrings.UnitConfig_Ibis_Simulation_InitialDelay;
            this.initialDelay.IsNullable = false;
            viewModel.Editors.Add(this.initialDelay);

            this.intervalBetween = new TimeSpanEditorViewModel();
            this.intervalBetween.Label = AdminStrings.UnitConfig_Ibis_Simulation_IntervalBetween;
            this.intervalBetween.IsNullable = true;
            viewModel.Editors.Add(this.intervalBetween);

            this.timesToRepeat = new NumberEditorViewModel();
            this.timesToRepeat.Label = AdminStrings.UnitConfig_Ibis_Simulation_TimesToRepeat;
            this.timesToRepeat.IsInteger = true;
            this.timesToRepeat.MinValue = 1;
            this.timesToRepeat.MaxValue = int.MaxValue;
            viewModel.Editors.Add(this.timesToRepeat);

            return viewModel;
        }
    }
}