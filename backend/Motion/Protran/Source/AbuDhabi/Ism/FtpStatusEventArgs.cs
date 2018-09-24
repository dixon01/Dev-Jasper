// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpStatusEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Ism
{
    using System;

    /// <summary>
    /// Container of the information about an FTP download's status.
    /// </summary>
    public class FtpStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public DownloadStatus DownloadStatus { get; set; }
    }
}
