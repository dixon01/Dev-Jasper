// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureUpdatePartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureUpdatePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Update
{
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The Azure update part controller.
    /// </summary>
    public class AzureUpdatePartController : UpdatePartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureUpdatePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public AzureUpdatePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Update.Azure, parent)
        {
        }

        /// <summary>
        /// Updates the visibility of the view model of this part controller.
        /// </summary>
        protected override void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.MethodsController.HasAzureChecked;
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Update_Azure;
            viewModel.Description = AdminStrings.UnitConfig_Update_Azure_Description;

            return viewModel;
        }
    }
}