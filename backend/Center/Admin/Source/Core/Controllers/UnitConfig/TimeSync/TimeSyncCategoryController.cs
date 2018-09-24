// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSyncCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeSyncCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The time sync category controller.
    /// </summary>
    public class TimeSyncCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSyncCategoryController"/> class.
        /// </summary>
        public TimeSyncCategoryController()
            : base(UnitConfigKeys.TimeSync.Category)
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
            yield return new TimeSourcePartController(this);
            yield return new IbisTimeSyncPartController(this);
            yield return new Vdv301TimeSyncPartController(this);
            yield return new SntpTimeSyncPartController(this);
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_TimeSync;
        }
    }
}
