// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Wgs84.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Utility.Core
{
    /// <summary>
    /// WGS 84 coordinate system helper methods.
    /// </summary>
    public static class Wgs84
    {
        /// <summary>
        /// The earth radius.
        /// </summary>
        public static readonly int EarthRadius = 6378137;

        /// <summary>
        /// Gets the distance in meters between two WGS 84 coordinates.
        /// </summary>
        /// <param name="lat1">
        /// The latitude of the first coordinate.
        /// </param>
        /// <param name="lon1">
        /// The longitude of the first coordinate.
        /// </param>
        /// <param name="lat2">
        /// The latitude of the second coordinate.
        /// </param>
        /// <param name="lon2">
        /// The longitude of the second coordinate.
        /// </param>
        /// <returns>
        /// The distance in meters.
        /// </returns>
        public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (lat1 == 0 || lon1 == 0 || lat2 == 0 || lon2 == 0)
            {
                return double.MaxValue;
            }

            // ReSharper restore CompareOfFloatsByEqualityOperator
            const double D2R = System.Math.PI / 180;

            var cosAlpha = System.Math.Cos(lon1 * D2R);
            var smallRadius = EarthRadius * cosAlpha;
            var deltaLon = (lon2 - lon1) * D2R * smallRadius;
            var deltaLat = (lat2 - lat1) * D2R * EarthRadius;
            return System.Math.Sqrt((deltaLon * deltaLon) + (deltaLat * deltaLat));
        }
    }
}
