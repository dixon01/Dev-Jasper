// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetDownloadStateMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SetDownloadStateMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Message representing <see cref="ResourceServiceBase.SetDownloadState"/>.
    /// Do not use this class outside this namespace!
    /// </summary>
    public class SetDownloadStateMessage
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets the number of downloaded bytes.
        /// </summary>
        public long DownloadedBytes { get; set; }

        /// <summary>
        /// Gets or sets the temporary filename.
        /// </summary>
        public string TempFilename { get; set; }
    }
}
