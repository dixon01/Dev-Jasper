// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusResponseEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusResponseEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Master
{
    using System;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Event arguments that contain a <see cref="StatusResponseFrame"/>.
    /// </summary>
    public class StatusResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusResponseEventArgs"/> class.
        /// </summary>
        /// <param name="status">
        /// The status response.
        /// </param>
        public StatusResponseEventArgs(StatusResponseFrame status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Gets the status response.
        /// </summary>
        public StatusResponseFrame Status { get; private set; }
    }
}