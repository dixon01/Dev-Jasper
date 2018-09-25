// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021TelegramPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021TelegramPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Telegram configuration part controller for DS021 based telegrams.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="TelegramConfig"/> that is created by this class.
    /// </typeparam>
    public class DS021TelegramPartControllerBase<T> : SimpleTelegramPartController<T>
        where T : DS021ConfigBase, new()
    {
        private GenericUsageDS021BaseEditorViewModel usedForTransfers;

        private GenericUsageDS021BaseEditorViewModel usedForTransferSymbols;

        private GenericUsageEditorViewModel usedForDestination;

        private GenericUsageEditorViewModel usedForDestinationTransfers;

        private GenericUsageEditorViewModel usedForDestinationTransferSymbols;

        private NumberEditorViewModel flushNumberOfStations;

        private TimeSpanEditorViewModel flushTimeout;

        private SelectionEditorViewModel lastStopMode;

        private NumberEditorViewModel hideDestinationBelow;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021TelegramPartControllerBase{T}"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The telegram name.
        /// </param>
        /// <param name="defaultUsage">
        /// The default generic usage of the telegram.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DS021TelegramPartControllerBase(
            string telegramName, GenericUsage defaultUsage, CategoryControllerBase parent)
            : base(telegramName, defaultUsage, parent)
        {
        }

        /// <summary>
        /// Prepares the given telegram with the data configured in this part.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be filled with data.
        /// </param>
        protected override void PrepareTelegram(T telegram)
        {
            base.PrepareTelegram(telegram);

            telegram.UsedForTransfers = this.usedForTransfers.GenericUsageDS021Base;
            telegram.UsedForTransferSymbols = this.usedForTransferSymbols.GenericUsageDS021Base;

            telegram.UsedForDestination = this.usedForDestination.GenericUsage;
            telegram.UsedForDestinationTransfers = this.usedForDestinationTransfers.GenericUsage;
            telegram.UsedForDestinationTransferSymbols = this.usedForDestinationTransferSymbols.GenericUsage;

            telegram.FlushNumberOfStations = (int)this.flushNumberOfStations.Value;

            // ReSharper disable once PossibleInvalidOperationException
            telegram.FlushTimeout = this.flushTimeout.Value.Value;

            var mode = this.GetLastStopMode();
            telegram.HideLastStop = mode == LastStopMode.HideLastStop;
            telegram.HideDestinationBelow = mode != LastStopMode.HideDestination
                                                ? 0
                                                : (int)this.hideDestinationBelow.Value;

            var answer = new DS120Config
                             {
                                 Enabled = true,
                                 Name = "DS120",
                                 DefaultResponse = 0,
                                 Responses = { new Response { Status = Status.IncorrectRecord, Value = 6 } }
                             };
            telegram.Answer = new Answer { Telegram = answer };
        }

        /// <summary>
        /// Loads the configuration from the given telegram into this part's editors.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to load the data from.
        /// </param>
        protected override void LoadFrom(T telegram)
        {
            base.LoadFrom(telegram);

            this.usedForTransfers.GenericUsageDS021Base = telegram.UsedForTransfers;
            this.usedForTransferSymbols.GenericUsageDS021Base = telegram.UsedForTransferSymbols;

            this.usedForDestination.GenericUsage = telegram.UsedForDestination;
            this.usedForDestinationTransfers.GenericUsage = telegram.UsedForDestinationTransfers;
            this.usedForDestinationTransferSymbols.GenericUsage = telegram.UsedForDestinationTransferSymbols;

            this.flushNumberOfStations.Value = telegram.FlushNumberOfStations;
            this.flushTimeout.Value = telegram.FlushTimeout;

            if (telegram.HideLastStop)
            {
                this.lastStopMode.SelectValue(LastStopMode.HideLastStop);
                this.hideDestinationBelow.Value = telegram.FlushNumberOfStations + 1;
            }
            else if (telegram.HideDestinationBelow <= 0)
            {
                this.lastStopMode.SelectValue(LastStopMode.ShowAll);
                this.hideDestinationBelow.Value = telegram.FlushNumberOfStations + 1;
            }
            else
            {
                this.lastStopMode.SelectValue(LastStopMode.HideDestination);
                this.hideDestinationBelow.Value = telegram.HideDestinationBelow;
            }

            this.UpdateEnabled();
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = base.CreateViewModel();

            this.GenericUsageEditor.Label = AdminStrings.UnitConfig_Ibis_DS021Base_UsedFor;
            this.GenericUsageEditor.ShouldShowRow = false;
            this.GenericUsageEditor.IsNullable = true;

            this.usedForTransfers = new GenericUsageDS021BaseEditorViewModel();
            this.usedForTransfers.Label = AdminStrings.UnitConfig_Ibis_DS021Base_UsedForTransfers;
            this.usedForTransfers.ShouldShowRow = false;
            this.usedForTransfers.IsNullable = true;
            viewModel.Editors.Add(this.usedForTransfers);

            this.usedForTransferSymbols = new GenericUsageDS021BaseEditorViewModel();
            this.usedForTransferSymbols.Label = AdminStrings.UnitConfig_Ibis_DS021Base_UsedForTransferSymbols;
            this.usedForTransferSymbols.ShouldShowRow = false;
            this.usedForTransferSymbols.IsNullable = true;
            viewModel.Editors.Add(this.usedForTransferSymbols);

            this.usedForDestination = new GenericUsageEditorViewModel();
            this.usedForDestination.Label = AdminStrings.UnitConfig_Ibis_DS021Base_UsedForDestination;
            this.usedForDestination.ShouldShowRow = true;
            this.usedForDestination.IsNullable = true;
            viewModel.Editors.Add(this.usedForDestination);

            this.usedForDestinationTransfers = new GenericUsageEditorViewModel();
            this.usedForDestinationTransfers.Label = AdminStrings.UnitConfig_Ibis_DS021Base_UsedForDestinationTransfers;
            this.usedForDestinationTransfers.ShouldShowRow = true;
            this.usedForDestinationTransfers.IsNullable = true;
            viewModel.Editors.Add(this.usedForDestinationTransfers);

            this.usedForDestinationTransferSymbols = new GenericUsageEditorViewModel();
            this.usedForDestinationTransferSymbols.Label =
                AdminStrings.UnitConfig_Ibis_DS021Base_UsedForDestinationTransferSymbols;
            this.usedForDestinationTransferSymbols.ShouldShowRow = true;
            this.usedForDestinationTransferSymbols.IsNullable = true;
            viewModel.Editors.Add(this.usedForDestinationTransferSymbols);

            this.flushNumberOfStations = new NumberEditorViewModel();
            this.flushNumberOfStations.Label = AdminStrings.UnitConfig_Ibis_DS021Base_FlushNumberOfStations;
            this.flushNumberOfStations.IsInteger = true;
            this.flushNumberOfStations.MinValue = 1;
            this.flushNumberOfStations.MaxValue = 1000;
            viewModel.Editors.Add(this.flushNumberOfStations);

            this.flushTimeout = new TimeSpanEditorViewModel();
            this.flushTimeout.Label = AdminStrings.UnitConfig_Ibis_DS021Base_FlushTimeout;
            this.flushTimeout.IsNullable = false;
            viewModel.Editors.Add(this.flushTimeout);

            this.lastStopMode = new SelectionEditorViewModel();
            this.lastStopMode.Label = AdminStrings.UnitConfig_Ibis_DS021Base_LastStopMode;
            this.lastStopMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_DS021Base_LastStopMode_ShowAll,
                    LastStopMode.ShowAll));
            this.lastStopMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_DS021Base_LastStopMode_HideLastStop,
                    LastStopMode.HideLastStop));
            this.lastStopMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_DS021Base_LastStopMode_HideDestination,
                    LastStopMode.HideDestination));
            this.lastStopMode.PropertyChanged += (s, e) => this.UpdateEnabled();
            viewModel.Editors.Add(this.lastStopMode);

            this.hideDestinationBelow = new NumberEditorViewModel();
            this.hideDestinationBelow.Label = AdminStrings.UnitConfig_Ibis_DS021Base_HideDestinationBelow;
            this.hideDestinationBelow.IsInteger = true;
            this.hideDestinationBelow.MinValue = 1;
            this.hideDestinationBelow.MaxValue = 1000;
            viewModel.Editors.Add(this.hideDestinationBelow);

            return viewModel;
        }

        private void UpdateEnabled()
        {
            this.hideDestinationBelow.IsEnabled = this.GetLastStopMode() == LastStopMode.HideDestination;
        }

        private LastStopMode GetLastStopMode()
        {
            return (LastStopMode)this.lastStopMode.SelectedValue;
        }
    }
}