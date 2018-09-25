// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainUnitCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.MainUnit
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.DataViewModels.Units;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The main unit category controller.
    /// </summary>
    public class MainUnitCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// The unit name ending when two display units are present units.
        /// </summary>
        protected const string UnitNameEndingWithTwoUnits = "-2";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainUnitCategoryController"/> class.
        /// </summary>
        public MainUnitCategoryController()
            : base(UnitConfigKeys.MainUnit.Category)
        {
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_MainUnit;
        }

        /// <summary>
        /// Creates all part controllers.
        /// </summary>
        /// <returns>
        /// An enumeration of the part controllers of this category.
        /// </returns>
        protected override IEnumerable<PartControllerBase> CreatePartControllers()
        {
            yield return new MainUnitConfigPartController(this);

            var displayUnitCount =
                HardwareDescriptors.PowerUnit.GetDisplayUnitCount(this.Parent.UnitConfiguration.ProductType.Name);

            for (var i = 0; i < displayUnitCount; i++)
            {
                yield return new DisplayUnitPartController(this, i + 1);
            }
        }
    }
}
