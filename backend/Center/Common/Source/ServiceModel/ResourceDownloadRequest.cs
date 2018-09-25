// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDownloadRequest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceDownloadRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.ServiceModel;

    /// <summary>
    /// Defines a request to download a resource.
    /// </summary>
    [MessageContract]
    public class ResourceDownloadRequest
    {
        /// <summary>
        /// Gets or sets the hash of the resource to download.
        /// </summary>
        [MessageHeader]
        public string Hash { get; set; }
    }
}