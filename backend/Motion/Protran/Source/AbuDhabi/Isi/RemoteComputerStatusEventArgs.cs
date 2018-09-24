// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteComputerStatusEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;

    /// <summary>
    /// EventArgs containing a telegram.
    /// </summary>
    public class RemoteComputerStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteComputerStatusEventArgs"/> class.
        /// </summary>
        /// <param name="status">The current status,</param>
        public RemoteComputerStatusEventArgs(RemoteComputerStatus status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Gets or sets the current computer status.
        /// </summary>
        public RemoteComputerStatus Status { get; set; }
    }
}
