// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConclusionController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ConclusionController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The ConclusionController.
    /// </summary>
    public class ConclusionController : CategoryControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConclusionController"/> class.
        /// </summary>
        public ConclusionController()
            : base(UnitConfigKeys.Conclusion.Category)
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
            var isEpaperUnit = this.IsEpaperUnit();

            yield return new SoftwareVersionsPartController(this);
            yield return new ExportPreparationPartController(this);

            if (!isEpaperUnit)
            {
                yield return new PreInstallationActionPartController(this);
                yield return new PostInstallationActionPartController(this);
            }

            yield return new ExportExecutionPartController(this, isEpaperUnit);

            if (!isEpaperUnit)
            {
                yield return new LocalDownloadPartController(this);
            }
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Conclusion;
        }

        private bool IsEpaperUnit()
        {
            var name = this.Parent.UnitConfiguration.ProductType.Name;

            return HardwareDescriptors.PowerUnit.PowerUnitNight1.Name.Equals(name)
                   || HardwareDescriptors.PowerUnit.PowerUnitNight2.Name.Equals(name)
                   || HardwareDescriptors.PowerUnit.PowerUnitSolar1.Name.Equals(name)
                   || HardwareDescriptors.PowerUnit.PowerUnitSolar2.Name.Equals(name);
        }
    }
}