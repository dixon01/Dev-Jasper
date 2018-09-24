// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using System;

    /// <summary>
    /// The information about a service.
    /// </summary>
    [Serializable]
    public class ServiceInfo
    {
        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the service number.
        /// </summary>
        public int ServiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the trip number.
        /// </summary>
        public int TripNumber { get; set; }

        /// <summary>
        /// Gets or sets the index of the first stop.
        /// </summary>
        public int FirstStopIndex { get; set; }
    }
}