// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PointBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Data
{
    /// <summary>
    /// The base class for all points in the database.
    /// </summary>
    public abstract class PointBase
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ort origin.
        /// </summary>
        public int OrtOrigin { get; set; }

        /// <summary>
        /// Gets or sets the longitude (used to be X in the old code).
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets the latitude (used to be Y in the old code).
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets the buffer size in meters around the point (used to be Rayon in the old code).
        /// </summary>
        public int Buffer { get; set; }

        /// <summary>
        /// Gets or sets the zone to which the point belongs.
        /// </summary>
        public int Zone { get; set; }
    }
}