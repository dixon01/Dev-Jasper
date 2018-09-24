// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITimer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    /// <summary>
    /// Interface of a timer created by the <see cref="TimerFactory"/>.
    /// </summary>
    public interface ITimer : IDisposable
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
        /// Gets or sets a value indicating whether the timer is automatically 
        /// restarted when it elapsed.
        /// </summary>
        bool AutoReset { get; set; }

        /// <summary>
        /// Gets or sets the interval at which the <see cref="Elapsed"/>
        /// event is fired.
        /// </summary>
        TimeSpan Interval { get; set; }
    }
}