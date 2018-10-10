// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationsCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationsCategoryController type.
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
    /// The transformations category controller.
    /// </summary>
    public class TransformationsCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// The maximum transformations count (50).
        /// </summary>
        public const int MaxTransformationsCount = 50;

        private IncomingPartController incoming;

        private TransformationsGeneralPartController general;

        private List<TransformationPartController> transformationPartControllers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationsCategoryController"/> class.
        /// </summary>
        public TransformationsCategoryController()
            : base(UnitConfigKeys.Transformations.Category)
        {
        }

        /// <summary>
        /// Gets the transformation part controllers within this category.
        /// </summary>
        public IEnumerable<TransformationPartController> TransformationPartControllers
        {
            get
            {
                return this.transformationPartControllers;
            }
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
            yield return this.general = new TransformationsGeneralPartController(this);
            this.general.ViewModelUpdated += (s, e) => this.UpdateVisibility();

            this.transformationPartControllers = new List<TransformationPartController>(MaxTransformationsCount);
            for (int i = 1; i <= MaxTransformationsCount; i++)
            {
                var controller = new TransformationPartController(i, this);
                this.transformationPartControllers.Add(controller);
                yield return controller;
            }
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Transformations;
        }

        private void UpdateVisibility()
        {
            if (this.incoming == null || this.general == null)
            {
                return;
            }

            var isVisible = this.incoming.HasSelected(IncomingData.Ibis)
                            || this.incoming.HasSelected(IncomingData.Vdv301);
            this.general.ViewModel.IsVisible = isVisible;
            var transformationsCount = this.general.TransformationsCount;
            for (int i = 0; i < this.transformationPartControllers.Count; i++)
            {
                this.transformationPartControllers[i].ViewModel.IsVisible = isVisible && i < transformationsCount;
            }
        }
    }
}
