// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtranCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProtranCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The general Protran category controller.
    /// </summary>
    public class ProtranCategoryController : CategoryControllerBase
    {
        private List<FilteredPartControllerBase> controllers;

        private IncomingPartController incoming;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtranCategoryController"/> class.
        /// </summary>
        public ProtranCategoryController()
            : base(UnitConfigKeys.Protran.Category)
        {
        }

        /// <summary>
        /// Asynchronously prepares this category controller and all its children with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public override Task PrepareAsync(HardwareDescriptor descriptor)
        {
            this.incoming = this.Parent.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();

            return base.PrepareAsync(descriptor);
        }

        /// <summary>
        /// Creates all part controllers.
        /// </summary>
        /// <returns>
        /// An enumeration of the part controllers of this category.
        /// </returns>
        protected override IEnumerable<PartControllerBase> CreatePartControllers()
        {
            this.controllers = new List<FilteredPartControllerBase> { new PersistencePartController(this) };
            return this.controllers;
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Protran;
        }

        private void UpdateVisibility()
        {
            if (this.incoming == null)
            {
                return;
            }

            var visible = this.incoming.HasSelected(IncomingData.Ibis)
                || this.incoming.HasSelected(IncomingData.Vdv301)
                || this.incoming.HasSelected(IncomingData.LamXimple);

            foreach (var part in this.controllers)
            {
                part.UpdateVisibility(visible);
            }
        }
    }
}
