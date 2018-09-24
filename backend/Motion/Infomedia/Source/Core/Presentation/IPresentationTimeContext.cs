// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPresentationTimeContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPresentationTimeContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;

    /// <summary>
    /// The time context that contains methods to query the "current" time
    /// and to register handlers to be called after some time ("timers").
    /// </summary>
    public interface IPresentationTimeContext
    {
        /// <summary>
        /// Gets the current presentation time in UTC.
        /// Use <see cref="DateTime.ToLocalTime()"/> to get the local time.
        /// This time is updated every time a timer expires, so it should only
        /// be used in the context of a handler registered to this class.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Adds a handler for a given date and time.
        /// The handler will be called on or after the given time and the handler
        /// will automatically be removed from this context.
        /// </summary>
        /// <param name="time">
        /// The time at which the <see cref="action"/> should be called.
        /// </param>
        /// <param name="action">
        /// The action to perform when the time is reached.
        /// The <see cref="DateTime"/> argument will be the "current" time
        /// (i.e. the one from <see cref="UtcNow"/>) at the moment the action
        /// is called; it is either equal to or greater than <see cref="time"/>.
        /// </param>
        void AddTimeReachedHandler(DateTime time, Action<DateTime> action);

        /// <summary>
        /// Removes a handler previously added with <see cref="AddTimeReachedHandler"/>.
        /// </summary>
        /// <param name="time">
        /// The same time provided to <see cref="AddTimeReachedHandler"/>.
        /// </param>
        /// <param name="action">
        /// The same action provided to <see cref="AddTimeReachedHandler"/>.
        /// </param>
        void RemoveTimeReachedHandler(DateTime time, Action<DateTime> action);

        /// <summary>
        /// Adds a handler for a given timespan.
        /// The handler will be called on or after the given timespan has elapsed
        /// and the handler will automatically be removed from this context.
        /// </summary>
        /// <param name="timeSpan">
        /// The timespan after which the <see cref="action"/> should be called.
        /// </param>
        /// <param name="action">
        /// The action to perform when the timespan has elapsed.
        /// The <see cref="DateTime"/> argument will be the "current" time
        /// (i.e. the one from <see cref="UtcNow"/>) at the moment the action
        /// is called; it is either equal to or greater than the "current" time
        /// at the moment this method was called plus the given <see cref="timeSpan"/>.
        /// </param>
        void AddTimeElapsedHandler(TimeSpan timeSpan, Action<DateTime> action);

        /// <summary>
        /// Removes a handler previously added with <see cref="AddTimeElapsedHandler"/>.
        /// </summary>
        /// <param name="timeSpan">
        /// The same timespan provided to <see cref="AddTimeElapsedHandler"/>.
        /// </param>
        /// <param name="action">
        /// The same action provided to <see cref="AddTimeElapsedHandler"/>.
        /// </param>
        void RemoveTimeElapsedHandler(TimeSpan timeSpan, Action<DateTime> action);
    }
}