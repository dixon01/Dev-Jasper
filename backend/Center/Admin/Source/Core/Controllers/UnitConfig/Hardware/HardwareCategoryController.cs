// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The hardware category controller.
    /// </summary>
    public class HardwareCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareCategoryController"/> class.
        /// </summary>
        public HardwareCategoryController()
            : base(UnitConfigKeys.Hardware.Category)
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
            yield return new ScreenResolutionsPartController(this);
            yield return new DisplayOrientationPartController(this);
            yield return new MultiScreenPartController(this);
            yield return new DisplayBrightnessPartController(this);
            yield return new DviLevelShiftersPartController(this);
            yield return new InputsPartController(this);
            yield return new OutputsPartController(this);
            yield return new Rs485ModePartController(this);
            yield return new TransceiversPartController(this);
            yield return new RebootPartController(this);
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Hardware;
        }
    }
}