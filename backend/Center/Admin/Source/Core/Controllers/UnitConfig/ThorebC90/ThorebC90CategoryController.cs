// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThorebC90CategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.ThorebC90
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The bus category controller.
    /// </summary>
    public class ThorebC90CategoryController : CategoryControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThorebC90CategoryController"/> class.
        /// </summary>
        public ThorebC90CategoryController()
            : base(UnitConfigKeys.ThorebC90C74.Category)
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
            yield return new BusPartController(this);
            yield return new TerminalPartController(this);
            yield return new IbisControlPartController(this);
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_ThorebC90;
        }
    }
}
