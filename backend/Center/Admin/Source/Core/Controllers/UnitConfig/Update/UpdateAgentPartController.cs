// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateAgentPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateAgentPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Update
{
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The update agent part controller.
    /// </summary>
    public class UpdateAgentPartController : UpdatePartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAgentPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public UpdateAgentPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Update.Agent, parent)
        {
        }

        /// <summary>
        /// Updates the visibility of the view model of this part controller.
        /// </summary>
        protected override void UpdateVisibility()
        {
            this.ViewModel.IsVisible = true;
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Update_Agent;
            viewModel.Description = AdminStrings.UnitConfig_Update_Agent_Description;
            return viewModel;
        }
    }
}