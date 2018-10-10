// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    /// <summary>
    /// The Atmel Controller IBIS state.
    /// </summary>
    public class IbisState : AtmelControlObject
    {
        /// <summary>
        /// Gets or sets the line number (DS001).
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? LineNo { get; set; }

        /// <summary>
        /// Gets or sets the course number (DS002).
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? CourseNo { get; set; }

        /// <summary>
        /// Gets or sets the route number (DS003).
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? RouteNo { get; set; }

        /// <summary>
        /// Gets or sets the stop number (DS004b).
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? StopNo { get; set; }

        /// <summary>
        /// Gets or sets the announcement number (DS036).
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? AnnounceNo { get; set; }
    }
}