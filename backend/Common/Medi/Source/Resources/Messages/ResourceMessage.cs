// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// Message representing a remote call to <see cref="IResourceService"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public abstract class ResourceMessage
    {
        /// <summary>
        /// Gets or sets the unique request id.
        /// </summary>
        public int RequestId { get; set; }
    }
}