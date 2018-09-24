// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetUploadStateMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SetUploadStateMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Message representing <see cref="ResourceServiceBase.SetUploading"/> and
    /// <see cref="ResourceServiceBase.ClearUploading"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public class SetUploadStateMessage
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets the source address.
        /// </summary>
        public MediAddress Source { get; set; }

        /// <summary>
        /// Gets or sets the destination address.
        /// </summary>
        public MediAddress Destination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transfer is temporary.
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// This is only used for temporary file transfers.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the upload is running (true) or not (false).
        /// </summary>
        public bool Uploading { get; set; }
    }
}
