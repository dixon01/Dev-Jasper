// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedResourceInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExtendedResourceInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// Extension of <see cref="ResourceInfo"/> that is used only
    /// in this namespace.
    /// </summary>
    public class ExtendedResourceInfo : ResourceInfo
    {
        /// <summary>
        /// Gets or sets the local resource path.
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or sets the current state of the resource.
        /// </summary>
        public ResourceState State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this resource is only temporary.
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// This is only used for temporary file transfers.
        /// </summary>
        public string OriginalName { get; set; }
    }
}
