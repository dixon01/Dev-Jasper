// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceSentInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Message to notify a peer that a resource is being sent.
//   This class is necessary to be sure the (internal)
//   <see cref="ResourceAnnouncement" /> was sent before asking
//   for a resource on the receiving side.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// Message to notify a peer that a resource is being sent.
    /// This class is necessary to be sure the (internal)
    /// <see cref="ResourceAnnouncement"/> was sent before asking
    /// for a resource on the receiving side.
    /// </summary>
    public class ResourceSentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSentInfo"/> class.
        /// </summary>
        public ResourceSentInfo()
        {                
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSentInfo"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public ResourceSentInfo(ResourceId id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public ResourceId Id { get; set; } 
    }
}