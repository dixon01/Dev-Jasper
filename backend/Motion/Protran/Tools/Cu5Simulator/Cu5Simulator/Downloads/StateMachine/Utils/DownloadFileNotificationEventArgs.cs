// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadFileNotificationEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils
{
    using System;

    using Gorba.Common.Protocols.Ctu.Responses;

    /// <summary>
    /// Container of all the information about
    /// the download progress of a file.
    /// </summary>
    public class DownloadFileNotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="fileAbsName">The state name.</param>
        /// <param name="progress">The message.</param>
        /// <param name="status">The status.</param>
        public DownloadFileNotificationEventArgs(string fileAbsName, int progress, DownloadStatusCode status)
        {
            this.FileAbsName = fileAbsName;
            this.Progress = progress;
            this.Status = status;
        }

        /// <summary>
        /// Gets or sets FileAbsName.
        /// </summary>
        public string FileAbsName { get; set; }

        /// <summary>
        /// Gets or sets Progress.
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public DownloadStatusCode Status { get; set; }
    }
}
