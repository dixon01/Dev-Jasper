// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreensCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreensCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SplashScreens
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The splash screens category controller.
    /// </summary>
    public class SplashScreensCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreensCategoryController"/> class.
        /// </summary>
        public SplashScreensCategoryController()
            : base(UnitConfigKeys.SplashScreens.Category)
        {
        }

        /// <summary>
        /// Creates all part controllers.
        /// </summary>
        /// <returns>
        /// An enumeration of the part controllers of this category.
        /// </returns>
        protected override IEnumerable<PartControllerBase> CreatePartControllers()
        {
            yield return new StartUpSplashScreenPartController(this);
            yield return new HotKeySplashScreenPartController(this);
            yield return new ButtonSplashScreenPartController(this);
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_SplashScreens;
        }
    }
}
