// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InitializationCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Init
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The configuration initialization category controller.
    /// </summary>
    public class InitializationCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationCategoryController"/> class.
        /// </summary>
        public InitializationCategoryController()
            : base(UnitConfigKeys.Initialization.Category)
        {
        }

        /// <summary>
        /// Loads the unit config data into this category controller and all its children.
        /// </summary>
        /// <param name="data">
        /// The configuration data for this category.
        /// </param>
        public override void Load(UnitConfigCategory data)
        {
            base.Load(data);
            this.ViewModel.CanBeVisible = true;
        }

        /// <summary>
        /// Creates all part controllers.
        /// </summary>
        /// <returns>
        /// An enumeration of the part controllers of this category.
        /// </returns>
        protected override IEnumerable<PartControllerBase> CreatePartControllers()
        {
            yield return new LoadDataPartController(this);
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Init;
        }
    }
}
