// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceDownloadResult.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Message used to stream a content resource together with its metadata information
//   (<see cref="ContentResource" />).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.IO;
    using System.ServiceModel;

    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Message used to stream a content resource together with its metadata information
    /// (<see cref="ContentResource"/>).
    /// </summary>
    [MessageContract]
    public class ContentResourceDownloadResult
    {
        /// <summary>
        /// Gets or sets the metadata information relative to this resource.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public ContentResource Resource { get; set; }

        /// <summary>
        /// Gets or sets the content of the resource.
        /// </summary>
        [MessageBodyMember]
        public Stream Content { get; set; }
    }
}
