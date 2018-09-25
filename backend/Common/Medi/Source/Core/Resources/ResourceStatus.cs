// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStatus.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    /// <summary>
    /// Result of the <see cref="IResourceServiceImpl.EndGetResourceStatus"/>
    /// method, this is only used within resource management.
    /// </summary>
    public class ResourceStatus
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is already available completely.
        /// </summary>
        public bool IsAvailableCompletely { get; set; }

        /// <summary>
        /// Gets or sets the number of available bytes.
        /// </summary>
        public long AvailableBytes { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(
                "ResourceStatus[{0}, {1} ({2})]",
                this.Id,
                this.AvailableBytes,
                this.IsAvailableCompletely ? "complete" : "incomplete");
        }
    }
}