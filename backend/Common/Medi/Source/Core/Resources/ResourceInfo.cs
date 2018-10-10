// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    /// <summary>
    /// Information about a local resource.
    /// Don't create instances yourself, but use
    /// <see cref="IResourceService.GetResource(Gorba.Common.Medi.Core.Resources.ResourceId)"/> instead.
    /// </summary>
    public class ResourceInfo
    {
        /// <summary>
        /// Gets or sets the unique resource id.
        /// Only use the getter!
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets the size of the local resource.
        /// Only use the getter!
        /// </summary>
        public long Size { get; set; }
    }
}