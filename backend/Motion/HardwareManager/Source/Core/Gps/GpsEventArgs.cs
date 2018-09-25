// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GpsEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Gps
{
    using System;
    using Gorba.Motion.Common.Entities.Gps;

    /// <summary>
    /// Defines GPS event containing EventArgs.
    /// </summary>
    public class GpsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpsEventArgs"/> class.
        /// </summary>
        /// <param name="gpsData">
        /// The GPS data.
        /// </param>
        public GpsEventArgs(GpsData gpsData)
        {
            this.GpsData = gpsData;
        }

        /// <summary>
        /// Gets or sets the GPS data.
        /// </summary>
        public GpsData GpsData { get; set; }
    }
}