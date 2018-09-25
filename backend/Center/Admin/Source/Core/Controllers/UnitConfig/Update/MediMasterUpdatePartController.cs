// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediMasterUpdatePartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediMasterUpdatePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Update
{
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The Medi master update part controller.
    /// </summary>
    public class MediMasterUpdatePartController : UpdatePartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediMasterUpdatePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public MediMasterUpdatePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Update.MediMaster, parent)
        {
        }

        /// <summary>
        /// Updates the visibility of the view model of this part controller.
        /// </summary>
        protected override void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.MethodsController.HasMediMasterChecked;
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Update_MediMaster;
            viewModel.Description = AdminStrings.UnitConfig_Update_MediMaster_Description;
            return viewModel;
        }
    }
}