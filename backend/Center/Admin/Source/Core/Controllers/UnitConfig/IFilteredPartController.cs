// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilteredPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFilteredPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    /// <summary>
    /// Interface for part controllers that can be filtered by the parent category controller.
    /// </summary>
    public interface IFilteredPartController
    {
        /// <summary>
        /// Update the visibility of this part.
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if the view model of this part should be visible.
        /// </param>
        void UpdateVisibility(bool visible);
    }
}