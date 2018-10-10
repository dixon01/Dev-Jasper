// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;

    /// <summary>
    /// The time event args.
    /// </summary>
    public class TimeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeEventArgs"/> class.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        public TimeEventArgs(DateTime time)
        {
            this.Time = time;
        }

        /// <summary>
        /// Gets the at which the event was fired.
        /// </summary>
        public DateTime Time { get; private set; }
    }
}