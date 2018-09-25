// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteredPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FilteredPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    /// <summary>
    /// The base class for part controllers that can be filtered by the parent category controller.
    /// </summary>
    public abstract class FilteredPartControllerBase : MultiEditorPartControllerBase, IFilteredPartController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredPartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected FilteredPartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Update the visibility of this part.
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if the view model of this part should be visible.
        /// </param>
        public virtual void UpdateVisibility(bool visible)
        {
            this.ViewModel.IsVisible = visible;
        }
    }
}