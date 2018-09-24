// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceUploadResult.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceUploadResult type.
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
    public class ResourceUploadResult
    {
        /// <summary>
        /// Gets or sets the resource relative to the uploaded file.
        /// </summary>
        [MessageHeader]
        public Resource Resource { get; set; }
    }
}