// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021CTelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021CTelegramPartController type.
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
    /// The DS021c telegram part controller.
    /// </summary>
    public class DS021CTelegramPartController : DS021TelegramPartControllerBase<DS021CConfig>
    {
        private SelectionEditorViewModel firstStopIndexValue;

        private GenericUsageDS021BaseEditorViewModel asciiLineNumberUsedFor;

        private SelectionEditorViewModel takeDestinationFromLastStop;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021CTelegramPartController"/> class.
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
        public DS021CTelegramPartController(
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
        protected override void PrepareTelegram(DS021CConfig telegram)
        {
            base.PrepareTelegram(telegram);

            telegram.FirstStopIndexValue = (int)this.firstStopIndexValue.SelectedValue;
            telegram.AsciiLineNumberUsedFor = this.asciiLineNumberUsedFor.GenericUsageDS021Base;
            telegram.TakeDestinationFromLastStop = (bool)this.takeDestinationFromLastStop.SelectedValue;
        }

        /// <summary>
        /// Loads the configuration from the given telegram into this part's editors.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to load the data from.
        /// </param>
        protected override void LoadFrom(DS021CConfig telegram)
        {
            base.LoadFrom(telegram);

            this.firstStopIndexValue.SelectValue(telegram.FirstStopIndexValue);
            this.asciiLineNumberUsedFor.GenericUsageDS021Base = telegram.AsciiLineNumberUsedFor;
            this.takeDestinationFromLastStop.SelectValue(telegram.TakeDestinationFromLastStop);
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

            this.firstStopIndexValue = new SelectionEditorViewModel();
            this.firstStopIndexValue.Label = AdminStrings.UnitConfig_Ibis_DS021c_FirstStopIndexValue;
            this.firstStopIndexValue.Options.Add(new SelectionOptionViewModel("0", 0));
            this.firstStopIndexValue.Options.Add(new SelectionOptionViewModel("1", 1));
            viewModel.Editors.Add(this.firstStopIndexValue);

            this.asciiLineNumberUsedFor = new GenericUsageDS021BaseEditorViewModel();
            this.asciiLineNumberUsedFor.Label = AdminStrings.UnitConfig_Ibis_DS021c_AsciiLineNumberUsedFor;
            this.asciiLineNumberUsedFor.ShouldShowRow = true;
            this.asciiLineNumberUsedFor.IsNullable = true;
            var index =
                viewModel.Editors.IndexOf(viewModel.Editors.OfType<GenericUsageEditorViewModel>().LastOrDefault());
            viewModel.Editors.Insert(index + 1, this.asciiLineNumberUsedFor);

            this.takeDestinationFromLastStop = new SelectionEditorViewModel();
            this.takeDestinationFromLastStop.Label = AdminStrings.UnitConfig_Ibis_DS021c_DestinationFrom;
            this.takeDestinationFromLastStop.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ibis_DS021c_DestinationFrom_Index101, false));
            this.takeDestinationFromLastStop.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ibis_DS021c_DestinationFrom_LastStop, true));
            viewModel.Editors.Add(this.takeDestinationFromLastStop);

            return viewModel;
        }
    }
}