// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataAccess.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Data
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Obc.Bus.Core.Route;

    /// <summary>
    /// Interface providing access to trip and parameter information.
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// Gets the day type for the given date.
        /// </summary>
        /// <param name="date">
        /// The date (the time will be ignored).
        /// </param>
        /// <returns>
        /// The day type; this can be 0 if it was not found.
        /// </returns>
        int GetDayType(DateTime date);

        /// <summary>
        /// Loads the simple stop list.
        /// </summary>
        /// <returns>
        /// The list of simple stops.
        /// </returns>
        IEnumerable<SimpleStop> LoadSimpleStopList();

        /// <summary>
        /// Loads timetable parameters.
        /// </summary>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <returns>
        /// The <see cref="ParamIti"/> object containing all parameters.
        /// </returns>
        ParamIti LoadParamIti(int lineNumber);

        /// <summary>
        /// Loads all trips matching the given day type, line, service and trip numbers.
        /// </summary>
        /// <param name="dayType">
        /// The day type.
        /// </param>
        /// <param name="lineNumber">
        /// The line number, if this is 0, all lines will be matched.
        /// </param>
        /// <param name="serviceNumber">
        /// The service number, if this is 0, all services will be matched.
        /// </param>
        /// <param name="tripNumber">
        /// The trip number, if this is 0, all trips will be matched.
        /// </param>
        /// <returns>
        /// The list of found trips.
        /// </returns>
        IEnumerable<TripInfo> LoadTrips(int dayType, int lineNumber, int serviceNumber, int tripNumber);

        /// <summary>
        /// Loads all trips for a given driver on a given day type.
        /// </summary>
        /// <param name="name">
        /// The name of the driver.
        /// </param>
        /// <param name="dayType">
        /// The day type.
        /// </param>
        /// <returns>
        /// The list of all driver trips.
        /// </returns>
        IEnumerable<DriverTripInfo> LoadDriverTrips(string name, int dayType);

        /// <summary>
        /// Loads the route with the given line and route number and loads the travel times for the given time group.
        /// </summary>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="routeId">
        /// The route id.
        /// </param>
        /// <param name="timeGroup">
        /// The time group.
        /// </param>
        /// <returns>
        /// The <see cref="RouteInfo"/> or null if not found.
        /// </returns>
        RouteInfo LoadRoute(int lineNumber, int routeId, int timeGroup);

        /// <summary>
        /// Loads the via information into the given route.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="lineNumber">
        /// The line number of the route.
        /// </param>
        void LoadVia(RouteInfo route, int lineNumber);

        /// <summary>
        /// Loads all point information into the route.
        /// </summary>
        /// <param name="route">
        /// The route for which to load the points.
        /// </param>
        void LoadPoints(RouteInfo route);
    }
}
