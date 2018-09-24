// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A route, also known as <c>intineraire</c> in French.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Obc.Bus.Core.Data;

    using Math = System.Math;

    /// <summary>
    /// A route, also known as <c>intineraire</c> in French.
    /// </summary>
    public class RouteInfo : StopListBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteInfo"/> class.
        /// </summary>
        public RouteInfo()
        {
            this.Points = new List<PointInfo>();
        }

        /// <summary>
        /// Gets or sets the route id.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Gets the list of points.
        /// </summary>
        public List<PointInfo> Points { get; private set; }

        /// <summary>
        /// Gets the stop index at the given point index.
        /// These two can be different because there are points that are not stops.
        /// </summary>
        /// <param name="pointIndex">
        /// The point index.
        /// </param>
        /// <returns>
        /// The stop index or -1 if it wasn't a stop.
        /// </returns>
        public int GetStopIndex(int pointIndex)
        {
            var ret = -1;

            if (this.Points[pointIndex].Type != PointType.Stop)
            {
                return ret;
            }

            for (int i = 0; i <= pointIndex; i++)
            {
                var point = this.Points[i];
                if (point.Type == PointType.Stop)
                {
                    ret++;
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the next point by its type and sub-type.
        /// </summary>
        /// <param name="pointIndex">
        /// The point index.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="subType">
        /// The sub type (see <see cref="PointInfo.SubType"/>).
        /// </param>
        /// <param name="data">
        /// The data expected at the given point.
        /// </param>
        /// <returns>
        /// The point index or -1 if not found.
        /// </returns>
        public int GetNextBufferByType(int pointIndex, PointType type, int subType, int data)
        {
            if (pointIndex < 0)
            {
                return -1;
            }

            for (int i = pointIndex + 1; i < this.Points.Count; i++)
            {
                var point = this.Points[i];
                if (point.Type == type && point.SubType == subType && point.Data == data)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the next point by its type.
        /// </summary>
        /// <param name="pointIndex">
        /// The point index.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The point index or -1 if not found.
        /// </returns>
        public int GetNextBufferByType(int pointIndex, PointType type)
        {
            if (pointIndex < 0)
            {
                return -1;
            }

            for (int i = pointIndex + 1; i < this.Points.Count; i++)
            {
                var point = this.Points[i];
                if (point.Type == type)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Calculates the time spent at stops between two points.
        /// </summary>
        /// <param name="index1">
        /// The first index.
        /// </param>
        /// <param name="index2">
        /// The second index.
        /// </param>
        /// <returns>
        /// The total time spent.
        /// </returns>
        public TimeSpan CalculateTimeAtStops(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0)
            {
                return TimeSpan.Zero;
            }

            var totalTime = TimeSpan.Zero;
            for (int i = index1; i < index2; i++)
            {
                if (this.Points[i].Type == PointType.Stop)
                {
                    totalTime += TimeSpan.FromSeconds(40);
                }
            }

            return totalTime;
        }

        /// <summary>
        /// Calculates the time between two points.
        /// </summary>
        /// <param name="index1">
        /// The first index.
        /// </param>
        /// <param name="index2">
        /// The second index.
        /// </param>
        /// <param name="speed">
        /// The speed in m/s.
        /// </param>
        /// <returns>
        /// The estimated travel time.
        /// </returns>
        public TimeSpan CalculateTimeBetweenPoints(int index1, int index2, float speed)
        {
            double dist;
            var currentIndex = index1;
            var currentSpeed = speed;
            var totalTime = TimeSpan.Zero;

            var speedLimitIndex = this.GetNextBufferByType(currentIndex, PointType.SpeedLimit);

            while (speedLimitIndex != -1 && speedLimitIndex < index2)
            {
                // On a trouvé un panneau de limite de vitesse
                // On calcule uniquement cette partie
                dist = this.CalculateDistanceBetweenPoints(currentIndex, speedLimitIndex);
                totalTime += CalculateTime(dist, currentSpeed);

                // On met a jour la vitesse
                // attention, la vitesse est dans sstype et pas dans data comme on pourrait s'y attendre
                var speedLimit = this.Points[speedLimitIndex].SubType / 3.6f;
                currentSpeed = Math.Min(speedLimit, speed);

                // et l'index courant
                currentIndex = speedLimitIndex;
                speedLimitIndex = this.GetNextBufferByType(currentIndex, PointType.SpeedLimit);
            }

            // Ensuite on calcule le "solde"
            dist = this.CalculateDistanceBetweenPoints(currentIndex, index2);
            totalTime += CalculateTime(dist, currentSpeed);

            // On ajoute le temps aux arrets
            totalTime += this.CalculateTimeAtStops(index1, index2);

            return totalTime;
        }

        /// <summary>
        /// Gets the first point that was not yet touched but we are close to it.
        /// </summary>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        /// <returns>
        /// The index of the point or -1 if not found.
        /// </returns>
        public int GetBusFirstTouchedPoint(float longitude, float latitude)
        {
            for (int i = 0; i < this.Points.Count; i++)
            {
                if (this.Points[i].Status == PointStatus.NotTouched
                    || this.Points[i].Status == PointStatus.OutsideSchedule)
                {
                    if (this.IsBusAtPoint(i, longitude, latitude))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the next theoretical point that should be on the route.
        /// </summary>
        /// <returns>
        /// The index of the point or -1 if not found.
        /// </returns>
        public int GetBusNextTheoreticalPoint()
        {
            var foundIndex = -1;
            for (var i = this.Points.Count - 1; i >= 0; i--)
            {
                if (this.Points[i].Status == PointStatus.NotTouched
                    || this.Points[i].Status == PointStatus.OutsideSchedule)
                {
                    foundIndex = i;
                }
                else
                {
                    return foundIndex;
                }
            }

            return foundIndex;
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
        protected override PointBase GetPoint(int index)
        {
            return this.Points[index];
        }

        /// <summary>
        /// Gets the number of points in this list.
        /// </summary>
        /// <returns>
        /// The number of points in this list.
        /// </returns>
        protected override int GetPointCount()
        {
            return this.Points.Count;
        }

        /// <summary>
        /// Calculates the time it takes to travel the given distance with the given speed.
        /// </summary>
        /// <param name="distance">the distance in meters.</param>
        /// <param name="speed">the speed in m/s.</param>
        /// <returns>The time it takes to travel the given distance with the given speed.</returns>
        private static TimeSpan CalculateTime(double distance, double speed)
        {
            const double MinSpeed = 30 / 3.6; // 30 km/h
            const double MaxSpeed = 70 / 3.6; // 70 km/h
            speed = Math.Min(Math.Max(MinSpeed, speed), MaxSpeed);
            return TimeSpan.FromSeconds(distance / speed);
        }
    }
}