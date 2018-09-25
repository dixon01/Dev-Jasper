// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotKeySplashScreenPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HotKeySplashScreenPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SplashScreens
{
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The hot key splash screen part controller.
    /// </summary>
    public class HotKeySplashScreenPartController : SplashScreenPartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeySplashScreenPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public HotKeySplashScreenPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.SplashScreens.HotKey, parent)
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
            viewModel.DisplayName = AdminStrings.UnitConfig_SplashScreens_HotKey;
            viewModel.Description = AdminStrings.UnitConfig_SplashScreens_HotKey_Description;
            return viewModel;
        }
    }
}