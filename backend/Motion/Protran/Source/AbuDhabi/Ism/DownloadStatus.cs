// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadStatus.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enum containing the possible status for an FTP download.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Ism
{
    /// <summary>
    /// Enum containing the possible status for an FTP download.
    /// </summary>
    public enum DownloadStatus
    {
        /// <summary>
        /// An FTP download process is currently running.
        /// </summary>
        Running,

        /// <summary>
        /// No FTP download process is currently running.
        /// </summary>
        NotRunning
    }
}