// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioRendererCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioRendererCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.AudioRenderer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The Audio Renderer category controller.
    /// </summary>
    public class AudioRendererCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// The maximum audio channel count (3).
        /// </summary>
        public const int MaxChannelCount = 3;

        private List<FilteredPartControllerBase> controllers;

        private OutgoingPartController outgoing;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRendererCategoryController"/> class.
        /// </summary>
        public AudioRendererCategoryController()
            : base(UnitConfigKeys.AudioRenderer.Category)
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
            this.controllers = new List<FilteredPartControllerBase> { new AudioGeneralPartController(this) };
            for (int i = 1; i <= MaxChannelCount; i++)
            {
                this.controllers.Add(new AudioChannelPartController(i, this));
            }

            this.controllers.Add(new AcapelaPartController(this));

            return this.controllers;
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Audio;
        }

        private void UpdateVisibility()
        {
            var isVisible = this.outgoing.HasSelected(OutgoingData.Audio);
            foreach (var controller in this.controllers)
            {
                controller.UpdateVisibility(isVisible);
            }
        }
    }
}
