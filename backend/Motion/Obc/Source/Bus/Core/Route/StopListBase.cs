// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopListBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StopListBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Bus.Core.Data;

    /// <summary>
    /// The base class for stop lists.
    /// </summary>
    public abstract class StopListBase
    {
        /// <summary>
        /// Gets the index of the last point in the list.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetTerminusIndex()
        {
            return this.GetPointCount() - 1;
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        /// <param name="index1">
        /// The first index.
        /// </param>
        /// <param name="index2">
        /// The second index.
        /// </param>
        /// <returns>
        /// The distance in meters.
        /// </returns>
        public double CalculateDistanceBetweenPoints(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0)
            {
                return -1;
            }

            var point1 = this.GetPoint(index1);
            var point2 = this.GetPoint(index2);
            return Wgs84.GetDistance(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude);
        }

        /// <summary>
        /// Gets a value indicating if the bus is within the buffer of the given point.
        /// </summary>
        /// <param name="idx">
        /// The index of the point.
        /// </param>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        /// <returns>
        /// True if the bus is within the buffer of the given point.
        /// </returns>
        public bool IsBusAtPoint(int idx, float longitude, float latitude)
        {
            if (idx >= this.GetPointCount())
            {
                return false;
            }

            var point = this.GetPoint(idx);
            return Wgs84.GetDistance(point.Latitude, point.Longitude, latitude, longitude) <= point.Buffer;
        }

        /// <summary>
        /// Gets the point at the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The point.
        /// </returns>
        protected abstract PointBase GetPoint(int index);

        /// <summary>
        /// Gets the number of points in this list.
        /// </summary>
        /// <returns>
        /// The number of points in this list.
        /// </returns>
        protected abstract int GetPointCount();
    }
}