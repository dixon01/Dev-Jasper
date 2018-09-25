// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// This class should not be used outside this namespace.
    /// State of a resource.
    /// </summary>
    public enum ResourceState
    {
        /// <summary>
        /// The resource has been announced by a <see cref="ResourceAnnouncement"/>,
        /// but has not yet arrived in this ResourceService.
        /// </summary>
        Announced,

        /// <summary>
        /// The resource is being downloaded, but not all bytes are available yet.
        /// </summary>
        Downloading,

        /// <summary>
        /// The resource is available locally, and it is stored in the resources directory
        /// of the local service.
        /// </summary>
        Available,

        /// <summary>
        /// The resource is awaiting deletion and should no longer be used.
        /// </summary>
        Deleting
    }
}