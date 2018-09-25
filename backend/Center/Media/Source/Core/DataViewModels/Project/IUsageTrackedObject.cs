// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUsageTrackedObject.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUsageTrackedObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Project
{
    /// <summary>
    /// Defines an object whose usage is being tracked.
    /// </summary>
    public interface IUsageTrackedObject
    {
        /// <summary>
        /// Gets a value indicating whether this resource is referenced by any entity of the system.
        /// </summary>
        /// <value>
        /// <c>true</c> if this resource is referenced by any entity of the system; otherwise, <c>false</c>.
        /// </value>
        bool IsUsed { get; }

        /// <summary>
        /// Gets or sets the references count.
        /// </summary>
        /// <value>
        /// The references count.
        /// </value>
        int ReferencesCount { get; set; }
    }
}