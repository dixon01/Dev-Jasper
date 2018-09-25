// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeadlineTimer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDeadlineTimer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    /// <summary>
    /// Interface of a deadline timer created by the <see cref="TimerFactory"/>.
    /// A deadline timer elapses exactly at a given date and time (instead of after a given timespan).
    /// </summary>
    public interface IDeadlineTimer : IDisposable
    {
        /// <summary>
        /// Event that is fired whenever the timer elapses.
        /// </summary>
        event EventHandler Elapsed;

        /// <summary>
        /// Gets the name of this timer used when creating it.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer should be triggered
        /// (<see cref="Elapsed"/> is fired) when the system UTC time changes past the
        /// <see cref="UtcDeadline"/>.
        /// If true, this timer will raise the <see cref="Elapsed"/> event if the
        /// system UTC time changes and the new system UTC time is greater than
        /// <see cref="UtcDeadline"/>.
        /// </summary>
        /// <example>
        /// - Timer is set to elapse on 14.03.2014 at 15:52 UTC
        /// - on 14.03.2014 at 15:00 UTC, the system time changes to 14.03.2014 16:00 UTC
        /// - if this property is set to true, the <see cref="Elapsed"/> event is fired, otherwise not
        /// </example>
        bool TriggerIfPassed { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time at which the <see cref="Elapsed"/>
        /// event is fired.
        /// </summary>
        DateTime UtcDeadline { get; set; }
    }
}