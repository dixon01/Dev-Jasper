// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediSlaveUpdatePartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediSlaveUpdatePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Update
{
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The Medi slave update part controller.
    /// </summary>
    public class MediSlaveUpdatePartController : UpdatePartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediSlaveUpdatePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public MediSlaveUpdatePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Update.MediSlave, parent)
        {
        }

        /// <summary>
        /// Updates the visibility of the view model of this part controller.
        /// </summary>
        protected override void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.MethodsController.HasMediSlaveChecked;
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Update_MediSlave;
            viewModel.Description = AdminStrings.UnitConfig_Update_MediSlave_Description;
            return viewModel;
        }
    }
}