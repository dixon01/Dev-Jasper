// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExitRequestEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExitRequestEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer
{
    using System;

    /// <summary>
    /// The event arguments used when an exit is requested.
    /// </summary>
    public class ExitRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitRequestEventArgs"/> class.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public ExitRequestEventArgs(string reason)
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the reason why the exit is requested.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event was handled.
        /// Subscribers should set this flag to true if they are exiting the application on their own.
        /// </summary>
        public bool Handled { get; set; }
    }
}
