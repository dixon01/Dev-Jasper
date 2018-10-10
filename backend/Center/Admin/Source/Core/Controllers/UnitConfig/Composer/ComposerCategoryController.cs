// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComposerCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Composer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The composer category controller.
    /// </summary>
    public class ComposerCategoryController : CategoryControllerBase
    {
        private IncomingPartController incoming;

        private ComposerGeneralPartController general;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposerCategoryController"/> class.
        /// </summary>
        public ComposerCategoryController()
            : base(UnitConfigKeys.Composer.Category)
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
            yield return this.general = new ComposerGeneralPartController(this);
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Composer;
        }

        private void UpdateVisibility()
        {
            // handle the visibility of all children here, it's easier
            var isVisible = this.incoming.HasSelected(IncomingData.Ibis)
                            || this.incoming.HasSelected(IncomingData.Vdv301)
                            || this.incoming.HasSelected(IncomingData.Ximple);
            this.general.ViewModel.IsVisible = isVisible;
        }
    }
}
