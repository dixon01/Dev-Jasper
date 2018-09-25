// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcRendererCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcRendererCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.AhdlcRenderer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The AHDLC Renderer category controller.
    /// </summary>
    public class AhdlcRendererCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// The maximum AHDLC address (15).
        /// </summary>
        public const int MaxAddress = 15;

        private List<FilteredPartControllerBase> controllers;

        private OutgoingPartController outgoing;

        /// <summary>
        /// Initializes a new instance of the <see cref="AhdlcRendererCategoryController"/> class.
        /// </summary>
        public AhdlcRendererCategoryController()
            : base(UnitConfigKeys.AhdlcRenderer.Category)
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
            this.outgoing = this.Parent.GetPart<OutgoingPartController>();
            this.outgoing.ViewModelUpdated += (s, e) => this.UpdateVisibility();
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
            this.controllers = new List<FilteredPartControllerBase> { new AhdlcGeneralPartController(this) };

            for (int i = 1; i <= MaxAddress; i++)
            {
                this.controllers.Add(new AhdlcSignPartController(i, this));
            }

            return this.controllers;
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Ahdlc;
        }

        private void UpdateVisibility()
        {
            var isVisible = this.outgoing.HasSelected(OutgoingData.Ahdlc);
            foreach (var controller in this.controllers)
            {
                controller.UpdateVisibility(isVisible);
            }
        }
    }
}
