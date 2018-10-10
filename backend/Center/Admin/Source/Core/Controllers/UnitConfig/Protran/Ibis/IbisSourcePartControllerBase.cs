// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisSourcePartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisSourcePartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Ibis;

    /// <summary>
    /// The base class for all IBIS source part controllers.
    /// </summary>
    public abstract class IbisSourcePartControllerBase : FilteredPartControllerBase
    {
        private readonly IbisSourceType sourceType;

        private bool parentVisible;

        private IbisGeneralPartController general;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisSourcePartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="sourceType">
        /// The source type of this controller.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected IbisSourcePartControllerBase(string key, IbisSourceType sourceType, CategoryControllerBase parent)
            : base(key, parent)
        {
            this.sourceType = sourceType;
        }

        /// <summary>
        /// Update the visibility of this part.
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if the view model of this part should be visible.
        /// </param>
        public override void UpdateVisibility(bool visible)
        {
            this.parentVisible = visible;
            this.UpdateVisibility();
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.general = this.GetPart<IbisGeneralPartController>();
            this.general.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.parentVisible && this.general.SourceType == this.sourceType;
        }
    }
}