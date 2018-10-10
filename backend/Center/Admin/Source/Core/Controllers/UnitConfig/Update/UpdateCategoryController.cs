// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Update
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The update category controller.
    /// </summary>
    public class UpdateCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCategoryController"/> class.
        /// </summary>
        public UpdateCategoryController()
            : base(UnitConfigKeys.Update.Category)
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
            yield return new UpdateAgentPartController(this);
            yield return new UpdateMethodsPartController(this);
            yield return new UsbUpdatePartController(this);
            yield return new FtpUpdatePartController(this);
            yield return new AzureUpdatePartController(this);
            yield return new MediMasterUpdatePartController(this);
            yield return new MediSlaveUpdatePartController(this);
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Update;
        }
    }
}
