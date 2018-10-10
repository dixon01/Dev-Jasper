// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartUpSplashScreenPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StartUpSplashScreenPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SplashScreens
{
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The start-up splash screen part controller.
    /// </summary>
    public class StartUpSplashScreenPartController : SplashScreenPartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartUpSplashScreenPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public StartUpSplashScreenPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.SplashScreens.StartUp, parent)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this splash screen can be disabled.
        /// </summary>
        protected override bool CanDisable
        {
            get
            {
                return false;
            }
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
            viewModel.DisplayName = AdminStrings.UnitConfig_SplashScreens_StartUp;
            viewModel.Description = AdminStrings.UnitConfig_SplashScreens_StartUp_Description;
            return viewModel;
        }
    }
}