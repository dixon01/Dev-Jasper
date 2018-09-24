// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO005TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO005TelegramPartController type.
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
    /// The GO005 telegram part controller.
    /// </summary>
    public class GO005TelegramPartController : DS021TelegramPartControllerBase<GO005Config>
    {
        private CheckableEditorViewModel bufferNextRoute;

        private NumberEditorViewModel hideNextStopForIndex;

        private GenericUsageEditorViewModel asciiLineNumberUsedFor;

        private CheckableEditorViewModel deleteRoute;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO005TelegramPartController"/> class.
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
        public GO005TelegramPartController(
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
        protected override void PrepareTelegram(GO005Config telegram)
        {
            base.PrepareTelegram(telegram);

            // ReSharper disable PossibleInvalidOperationException
            telegram.BufferNextRoute = this.bufferNextRoute.IsChecked.Value;
            telegram.HideNextStopForIndex = (int)this.hideNextStopForIndex.Value;
            telegram.AsciiLineNumberUsedFor = this.asciiLineNumberUsedFor.GenericUsage;
            telegram.DeleteRoute = this.deleteRoute.IsChecked.Value;

            // ReSharper restore PossibleInvalidOperationException
        }

        /// <summary>
        /// Loads the configuration from the given telegram into this part's editors.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to load the data from.
        /// </param>
        protected override void LoadFrom(GO005Config telegram)
        {
            base.LoadFrom(telegram);

            this.bufferNextRoute.IsChecked = telegram.BufferNextRoute;
            this.hideNextStopForIndex.Value = telegram.HideNextStopForIndex;
            this.asciiLineNumberUsedFor.GenericUsage = telegram.AsciiLineNumberUsedFor;
            this.deleteRoute.IsChecked = telegram.DeleteRoute;
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

            this.bufferNextRoute = new CheckableEditorViewModel();
            this.bufferNextRoute.Label = AdminStrings.UnitConfig_Ibis_GO005_BufferNextRoute;
            this.bufferNextRoute.IsThreeState = false;
            viewModel.Editors.Add(this.bufferNextRoute);

            this.hideNextStopForIndex = new NumberEditorViewModel();
            this.hideNextStopForIndex.Label = AdminStrings.UnitConfig_Ibis_GO005_HideNextStopForIndex;
            this.hideNextStopForIndex.IsInteger = true;
            this.hideNextStopForIndex.MinValue = 0;
            this.hideNextStopForIndex.MaxValue = 9999;
            viewModel.Editors.Add(this.hideNextStopForIndex);

            this.asciiLineNumberUsedFor = new GenericUsageEditorViewModel();
            this.asciiLineNumberUsedFor.Label = AdminStrings.UnitConfig_Ibis_DS021c_AsciiLineNumberUsedFor;
            this.asciiLineNumberUsedFor.ShouldShowRow = true;
            this.asciiLineNumberUsedFor.IsNullable = true;
            var index =
                viewModel.Editors.IndexOf(viewModel.Editors.OfType<GenericUsageEditorViewModel>().LastOrDefault());
            viewModel.Editors.Insert(index + 1, this.asciiLineNumberUsedFor);

            this.deleteRoute = new CheckableEditorViewModel();
            this.deleteRoute.Label = AdminStrings.UnitConfig_Ibis_GO005_DeleteRoute;
            this.deleteRoute.IsThreeState = false;
            viewModel.Editors.Add(this.deleteRoute);

            return viewModel;
        }
    }
}