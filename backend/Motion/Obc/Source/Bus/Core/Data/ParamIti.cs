// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParamIti.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ParamIti type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Data
{
    using System;

    /// <summary>
    /// The <c>param.iti</c> file contents (parameters about the route search).
    /// </summary>
    public class ParamIti
    {
        /// <summary>
        /// Gets or sets the line number for which these parameters are valid.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the number of points to be timed out for a route to be regarded as outside schedule.
        /// </summary>
        public int ItiOut { get; set; }

        /// <summary>
        /// Gets or sets the in.
        /// </summary>
        public int ItiIn { get; set; }

        /// <summary>
        /// Gets or sets the delete.
        /// </summary>
        public int ItiDelete { get; set; }

        /// <summary>
        /// Gets or sets the time by which a bus can be delayed at a point
        /// for it to still be considered for route discovery (used with line only algorithm).
        /// </summary>
        public TimeSpan TimePlusLine { get; set; }

        /// <summary>
        /// Gets or sets the time by which a bus can be in advance at a point
        /// for it to still be considered for route discovery (used with line only algorithm).
        /// </summary>
        public TimeSpan TimeMinusLine { get; set; }

        /// <summary>
        /// Gets or sets the time by which a bus can be delayed at a point
        /// for it to still be considered for route discovery (used with line/service algorithm).
        /// </summary>
        public TimeSpan TimePlusService { get; set; }

        /// <summary>
        /// Gets or sets the time by which a bus can be in advance at a point
        /// for it to still be considered for route discovery (used with line/service algorithm).
        /// </summary>
        public TimeSpan TimeMinusService { get; set; }

        /// <summary>
        /// Gets or sets the time by which a bus can be delayed at a point
        /// before the entire trip is cancelled.
        /// </summary>
        public TimeSpan TimePlusValidated { get; set; }

        /// <summary>
        /// Gets or sets the time by which a bus can be in advance at a point
        /// before the entire trip is cancelled.
        /// </summary>
        public TimeSpan TimeMinusValidated { get; set; }

        /// <summary>
        /// Gets or sets the minimum distance between two points for an announcement to be played [m].
        /// </summary>
        public int MinDistanceAnnouncement { get; set; }

        /// <summary>
        /// Gets or sets the buffer size around an announcement point [m].
        /// </summary>
        public int BufferAnnouncement { get; set; }
    }
}