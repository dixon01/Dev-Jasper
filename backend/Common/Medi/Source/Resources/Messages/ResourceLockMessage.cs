// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceLockMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceLockMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Message representing <see cref="ResourceServiceBase.AcquireLock"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public class ResourceLockMessage
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is to be locked
        /// (true) or unlocked (false).
        /// </summary>
        public bool Lock { get; set; }
    }
}