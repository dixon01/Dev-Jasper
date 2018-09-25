// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS081TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS081TelegramPartController type.
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
    /// The DS081 telegram part controller.
    /// </summary>
    public class DS081TelegramPartController : SimpleTelegramPartController<DS081Config>
    {
        private TextEditorViewModel value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS081TelegramPartController"/> class.
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
        public DS081TelegramPartController(
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
        protected override void PrepareTelegram(DS081Config telegram)
        {
            base.PrepareTelegram(telegram);

            telegram.Value = this.value.Text;
        }

        /// <summary>
        /// Loads the configuration from the given telegram into this part's editors.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to load the data from.
        /// </param>
        protected override void LoadFrom(DS081Config telegram)
        {
            base.LoadFrom(telegram);

            this.value.Text = telegram.Value ?? "0";
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

            this.value = new TextEditorViewModel();
            this.value.Label = AdminStrings.UnitConfig_Ibis_DS081_Value;
            viewModel.Editors.Add(this.value);

            return viewModel;
        }
    }
}