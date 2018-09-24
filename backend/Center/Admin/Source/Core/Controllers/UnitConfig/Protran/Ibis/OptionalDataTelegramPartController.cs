// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionalDataTelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OptionalDataTelegramPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Telegram configuration part controller for simple telegrams that have an optional generic usage.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="TelegramConfig"/> that is created by this class.
    /// </typeparam>
    public class OptionalDataTelegramPartController<T> : SimpleTelegramPartController<T>
        where T : TelegramConfig, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalDataTelegramPartController{T}"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The telegram name.
        /// </param>
        /// <param name="defaultUsage">
        /// The default usage.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public OptionalDataTelegramPartController(
            string telegramName, GenericUsage defaultUsage, CategoryControllerBase parent)
            : base(telegramName, defaultUsage, parent)
        {
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
            this.GenericUsageEditor.IsNullable = true;
            return viewModel;
        }
    }
}