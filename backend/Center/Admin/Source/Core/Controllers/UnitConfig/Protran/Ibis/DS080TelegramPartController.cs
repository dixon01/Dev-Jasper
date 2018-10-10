// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS080TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS080TelegramPartController type.
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
    /// The DS080 telegram part controller.
    /// </summary>
    public class DS080TelegramPartController : SimpleTelegramPartController<DS080Config>
    {
        private TextEditorViewModel openValue;

        private TextEditorViewModel closeValue;

        private CheckableEditorViewModel resetWithDS010B;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS080TelegramPartController"/> class.
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
        public DS080TelegramPartController(
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
        protected override void PrepareTelegram(DS080Config telegram)
        {
            base.PrepareTelegram(telegram);

            telegram.OpenValue = this.openValue.Text;
            telegram.CloseValue = this.closeValue.Text;
            telegram.ResetWithDS010B = this.resetWithDS010B.IsChecked.HasValue && this.resetWithDS010B.IsChecked.Value;
        }

        /// <summary>
        /// Loads the configuration from the given telegram into this part's editors.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to load the data from.
        /// </param>
        protected override void LoadFrom(DS080Config telegram)
        {
            base.LoadFrom(telegram);

            this.openValue.Text = telegram.OpenValue ?? "1";
            this.closeValue.Text = telegram.CloseValue ?? "0";
            this.resetWithDS010B.IsChecked = telegram.ResetWithDS010B;
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

            this.openValue = new TextEditorViewModel();
            this.openValue.Label = AdminStrings.UnitConfig_Ibis_DS080_OpenValue;
            viewModel.Editors.Add(this.openValue);

            this.closeValue = new TextEditorViewModel();
            this.closeValue.Label = AdminStrings.UnitConfig_Ibis_DS080_CloseValue;
            viewModel.Editors.Add(this.closeValue);

            this.resetWithDS010B = new CheckableEditorViewModel();
            this.resetWithDS010B.Label = AdminStrings.UnitConfig_Ibis_DS080_ResetWithDS010B;
            this.resetWithDS010B.IsThreeState = false;
            viewModel.Editors.Add(this.resetWithDS010B);

            return viewModel;
        }
    }
}