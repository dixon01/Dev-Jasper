// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadStatusCode.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   All valid status code for a download.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Responses
{
    /// <summary>
    /// All valid status code for a download.
    /// </summary>
    public enum DownloadStatusCode
    {
        /// <summary>
        /// The download of a file has succedeed.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The download of a file is enqueued but not yet started.
        /// </summary>
        Queued = 1,

        /// <summary>
        /// The download of a file is started.
        /// </summary>
        Downloading = 2,

        /// <summary>
        /// The download of a file is finished due to a general error.
        /// </summary>
        GeneralError = -1,

        /// <summary>
        /// The download of a file is finished due to a memory error.
        /// </summary>
        LowMemory = -2,

        /// <summary>
        /// The download of a file is finished due to a connection error.
        /// </summary>
        ConnectionError = -3,

        /// <summary>
        /// The file download has a wrong CRC.
        /// </summary>
        BadCrc = -4
    }
}