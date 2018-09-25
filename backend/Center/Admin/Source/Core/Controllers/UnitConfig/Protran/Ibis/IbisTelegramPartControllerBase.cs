// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTelegramPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisTelegramPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using System;
    using System.Linq;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// The base class for all part controllers responsible for IBIS telegrams.
    /// </summary>
    public abstract class IbisTelegramPartControllerBase : FilteredPartControllerBase
    {
        private bool parentVisible;

        private IbisTelegramSelectionPartController telegramSelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisTelegramPartControllerBase"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The name of the telegram (e.g. "DS001").
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected IbisTelegramPartControllerBase(string telegramName, CategoryControllerBase parent)
            : base(string.Format(UnitConfigKeys.IbisProtocol.TelegramFormat, telegramName), parent)
        {
            this.TelegramName = telegramName;
            this.TelegramType = TelegramType.String;
        }

        /// <summary>
        /// Gets the telegram name.
        /// </summary>
        public string TelegramName { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the configured telegram is an integer telegram.
        /// </summary>
        public TelegramType TelegramType { get; set; }

        /// <summary>
        /// Creates the telegram config for this part.
        /// </summary>
        /// <returns>
        /// The <see cref="TelegramConfig"/>.
        /// </returns>
        public abstract TelegramConfig CreateTelegramConfig();

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
            this.telegramSelection = this.GetPart<IbisTelegramSelectionPartController>();
            this.telegramSelection.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();
            this.UpdateErrors();
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
            viewModel.DisplayName = this.TelegramName;
            viewModel.Description = string.Format(
                AdminStrings.UnitConfig_Ibis_Telegram_DescriptionFormat,
                this.TelegramName);

            return viewModel;
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);
            this.UpdateErrors();
        }

        /// <summary>
        /// Updates the errors on the editors.
        /// </summary>
        protected virtual void UpdateErrors()
        {
        }

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.parentVisible
                                       && this.telegramSelection.GetSelectedTelegrams().Contains(this.TelegramName);
        }
    }
}