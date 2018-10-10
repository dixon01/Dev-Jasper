// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO004TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO004TelegramPartController type.
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
    /// The GO004 telegram part controller.
    /// </summary>
    public class GO004TelegramPartController : AnswerWithDS120TelegramPartController<GO004Config>
    {
        private GenericUsageEditorViewModel usedForTitle;

        private GenericUsageEditorViewModel usedForType;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO004TelegramPartController"/> class.
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
        public GO004TelegramPartController(
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
        protected override void PrepareTelegram(GO004Config telegram)
        {
            base.PrepareTelegram(telegram);

            telegram.UsedForTitle = this.usedForTitle.GenericUsage;
            telegram.UsedForType = this.usedForType.GenericUsage;
        }

        /// <summary>
        /// Loads the configuration from the given telegram into this part's editors.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to load the data from.
        /// </param>
        protected override void LoadFrom(GO004Config telegram)
        {
            base.LoadFrom(telegram);

            this.usedForTitle.GenericUsage = telegram.UsedForTitle;
            this.usedForType.GenericUsage = telegram.UsedForType;
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
            this.GenericUsageEditor.ShouldShowRow = false;
            this.GenericUsageEditor.IsNullable = true;

            this.usedForTitle = new GenericUsageEditorViewModel();
            this.usedForTitle.Label = AdminStrings.UnitConfig_Ibis_GO004_UsedForTitle;
            this.usedForTitle.ShouldShowRow = false;
            this.usedForTitle.IsNullable = true;
            viewModel.Editors.Add(this.usedForTitle);

            this.usedForType = new GenericUsageEditorViewModel();
            this.usedForType.Label = AdminStrings.UnitConfig_Ibis_GO004_UsedForType;
            this.usedForType.ShouldShowRow = false;
            this.usedForType.IsNullable = true;
            viewModel.Editors.Add(this.usedForType);

            return viewModel;
        }
    }
}