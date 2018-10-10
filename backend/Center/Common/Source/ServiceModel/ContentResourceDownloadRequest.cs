// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceDownloadRequest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines a request to download a ContentResource.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.ServiceModel;

    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Defines a request to download a ContentResource.
    /// </summary>
    [MessageContract]
    public class ContentResourceDownloadRequest
    {
        /// <summary>
        /// Gets or sets the hash of the content resource to download.
        /// </summary>
        [MessageHeader]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the hash type used to create the hash.
        /// </summary>
        [MessageHeader]
        public HashAlgorithmTypes HashType { get; set; }
    }
}
