// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftwareCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SoftwareCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Software
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The controller for the "Software" category.
    /// </summary>
    public class SoftwareCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoftwareCategoryController"/> class.
        /// </summary>
        public SoftwareCategoryController()
            : base(UnitConfigKeys.Software.Category)
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
            yield return new IncomingPartController(this);
            yield return new OutgoingPartController(this);
            yield return new MediSlavePartController(this);
            yield return new BackgroundSystemConnectionPartController(this);
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Software;
        }
    }
}
