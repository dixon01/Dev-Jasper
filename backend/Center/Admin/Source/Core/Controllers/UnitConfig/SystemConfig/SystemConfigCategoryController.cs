// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemConfigCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemConfigCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SystemConfig
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The system config category controller.
    /// </summary>
    public class SystemConfigCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// The maximum number of I/O specific settings editable by the user.
        /// </summary>
        public static readonly int MaxMultiConfigCount = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfigCategoryController"/> class.
        /// </summary>
        public SystemConfigCategoryController()
            : base(UnitConfigKeys.SystemConfig.Category)
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
            yield return new ConfigModePartController(this);
            yield return new SingleSystemConfigPartController(this);
            yield return new GlobalSystemConfigPartController(this);

            for (int i = 0; i < MaxMultiConfigCount; i++)
            {
                yield return new IOSystemConfigPartController(i, this);
            }
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_SystemConfig;
        }
    }
}
