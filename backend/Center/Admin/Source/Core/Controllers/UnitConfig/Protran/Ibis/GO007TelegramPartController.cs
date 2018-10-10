// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO007TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO007TelegramPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using System.Linq;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// The GO007 telegram part controller.
    /// </summary>
    public class GO007TelegramPartController : SimpleTelegramPartController<GO007Config>
    {
        private GenericUsageEditorViewModel usedForTransfers;

        private GenericUsageEditorViewModel usedForDestination;

        private GenericUsageEditorViewModel usedForDestinationTransfers;

        private GenericUsageEditorViewModel usedForLineNumber;

        private SelectionEditorViewModel lastStopMode;

        private NumberEditorViewModel hideDestinationBelow;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO007TelegramPartController"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The telegram name.
        /// </param>
        /// <param name="defaultUsage">
        /// The default usage.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public GO007TelegramPartController(
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
        protected override void PrepareTelegram(GO007Config telegram)
        {
            base.PrepareTelegram(telegram);

            telegram.UsedForTransfers = this.usedForTransfers.GenericUsage;

            telegram.UsedForDestination = this.usedForDestination.GenericUsage;
            telegram.UsedForDestinationTransfers = this.usedForDestinationTransfers.GenericUsage;

            telegram.UsedForLineNumber = this.usedForLineNumber.GenericUsage;

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
        protected override void LoadFrom(GO007Config telegram)
        {
            base.LoadFrom(telegram);

            this.usedForTransfers.GenericUsage = telegram.UsedForTransfers;

            this.usedForDestination.GenericUsage = telegram.UsedForDestination;
            this.usedForDestinationTransfers.GenericUsage = telegram.UsedForDestinationTransfers;

            this.usedForLineNumber.GenericUsage = telegram.UsedForLineNumber;

            if (telegram.HideLastStop)
            {
                this.lastStopMode.SelectValue(LastStopMode.HideLastStop);
            }
            else if (telegram.HideDestinationBelow <= 0)
            {
                this.lastStopMode.SelectValue(LastStopMode.ShowAll);
            }
            else
            {
                this.lastStopMode.SelectValue(LastStopMode.HideDestination);
            }

            this.hideDestinationBelow.Value = telegram.HideDestinationBelow;

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

            this.usedForTransfers = new GenericUsageEditorViewModel();
            this.usedForTransfers.Label = AdminStrings.UnitConfig_Ibis_DS021Base_UsedForTransfers;
            this.usedForTransfers.ShouldShowRow = false;
            this.usedForTransfers.IsNullable = true;
            viewModel.Editors.Add(this.usedForTransfers);

            this.usedForDestination = new GenericUsageEditorViewModel();
            this.usedForDestination.Label = AdminStrings.UnitConfig_Ibis_DS021Base_UsedForDestination;
            this.usedForDestination.ShouldShowRow = true;
            this.usedForDestination.IsNullable = true;
            viewModel.Editors.Add(this.usedForDestination);

            this.usedForDestinationTransfers = new GenericUsageEditorViewModel();
            this.usedForDestinationTransfers.Label =
                AdminStrings.UnitConfig_Ibis_DS021Base_UsedForDestinationTransfers;
            this.usedForDestinationTransfers.ShouldShowRow = true;
            this.usedForDestinationTransfers.IsNullable = true;
            viewModel.Editors.Add(this.usedForDestinationTransfers);

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

            this.usedForLineNumber = new GenericUsageEditorViewModel();
            this.usedForLineNumber.Label = AdminStrings.UnitConfig_Ibis_GO007_UsedForLineNumber;
            this.usedForLineNumber.ShouldShowRow = true;
            this.usedForLineNumber.IsNullable = true;
            var index =
                viewModel.Editors.IndexOf(viewModel.Editors.OfType<GenericUsageEditorViewModel>().LastOrDefault());
            viewModel.Editors.Insert(index + 1, this.usedForLineNumber);

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