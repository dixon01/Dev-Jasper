// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301PartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301PartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    /// <summary>
    /// The base class for most VDV 301 part controllers.
    /// </summary>
    public abstract class Vdv301PartControllerBase : FilteredPartControllerBase
    {
        private bool shouldShow;

        private bool parentVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301PartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected Vdv301PartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this part should be shown.
        /// This flag is set depending on the selected data items in the tree.
        /// </summary>
        public bool ShouldShow
        {
            get
            {
                return this.shouldShow;
            }

            set
            {
                if (this.shouldShow == value)
                {
                    return;
                }

                this.shouldShow = value;
                this.UpdateVisibility();
            }
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

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.parentVisible && this.ShouldShow;
        }
    }
}