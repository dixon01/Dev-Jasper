// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceUploadRequest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceUploadRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.IO;
    using System.ServiceModel;

    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Message used to stream a resource together with its metadata information (<see cref="Resource"/>).
    /// </summary>
    [MessageContract]
    public class ResourceUploadRequest
    {
        /// <summary>
        /// Gets or sets the metadata information relative to this resource.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public Resource Resource { get; set; }

        /// <summary>
        /// Gets or sets the content of the resource.
        /// </summary>
        [MessageBodyMember]
        public Stream Content { get; set; }
    }
}