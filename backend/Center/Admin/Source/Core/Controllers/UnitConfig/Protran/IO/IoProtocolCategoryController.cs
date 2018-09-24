// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoProtocolCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IoProtocolCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.IO
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The Protran I/O protocol category controller.
    /// </summary>
    public class IoProtocolCategoryController : CategoryControllerBase
    {
        /// <summary>
        /// The maximum inputs count (16).
        /// </summary>
        public const int MaxInputsCount = 16;

        private IncomingPartController incoming;

        private IoProtocolGeneralPartController general;

        /// <summary>
        /// Initializes a new instance of the <see cref="IoProtocolCategoryController"/> class.
        /// </summary>
        public IoProtocolCategoryController()
            : base(UnitConfigKeys.IoProtocol.Category)
        {
        }

        /// <summary>
        /// Gets the input handler part controllers within this category.
        /// </summary>
        public List<InputHandlerPartController> InputHandlerPartControllers { get; private set; }

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
            yield return this.general = new IoProtocolGeneralPartController(this);
            this.general.ViewModelUpdated += (s, e) => this.UpdateVisibility();

            this.InputHandlerPartControllers = new List<InputHandlerPartController>(MaxInputsCount);
            for (int i = 1; i <= MaxInputsCount; i++)
            {
                var controller = new InputHandlerPartController(i, this);
                this.InputHandlerPartControllers.Add(controller);
                yield return controller;
            }
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_IoProtocol;
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
            var inputsCount = this.general.InputsCount;
            for (int i = 0; i < this.InputHandlerPartControllers.Count; i++)
            {
                this.InputHandlerPartControllers[i].ViewModel.IsVisible = isVisible && i < inputsCount;
            }
        }
    }
}
