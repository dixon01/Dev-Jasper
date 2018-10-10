// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceUploadResult.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the result of an upload operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.ServiceModel;

    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Defines the result of an upload operation.
    /// </summary>
    [MessageContract]
    public class ContentResourceUploadResult
    {
        /// <summary>
        /// Gets or sets the content resource relative to the uploaded file.
        /// </summary>
        [MessageHeader]
        public ContentResource Resource { get; set; }
    }
}
