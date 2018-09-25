// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IqubeCsvDataAccess.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Obc.Bus.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Csv;
    using Gorba.Motion.Obc.Bus.Core.Route;

    using NLog;

    /// <summary>
    /// Data access using the old .csv files from the iqube / VM.c/r world.
    /// </summary>
    public class IqubeCsvDataAccess : IDataAccess
    {
        private static readonly Logger Logger = LogHelper.GetLogger<IqubeCsvDataAccess>();

        private static readonly Encoding Encoding = Encoding.GetEncoding("ISO-8859-1");

        /// <summary>
        /// Gets the day type for the given date.
        /// </summary>
        /// <param name="date">
        /// The date (the time will be ignored).
        /// </param>
        /// <returns>
        /// The day type; this can be 0 if it was not found.
        /// </returns>
        public int GetDayType(DateTime date)
        {
            // see ReadDayType() and getIQube_DayType() in the old code
            var found = 0;
            var dateString = date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            ReadDatabaseFileLines(
                "calendar.ini",
                false,
                line =>
                    {
                        int dayType;
                        if (line.Length < 2 || line[0].StartsWith("#") || !ParserUtil.TryParse(line[1], out dayType))
                        {
                            return;
                        }

                        bool match;
                        switch (line[0])
                        {
                            case "DEF":
                                match = true;
                                break;
                            case "MON":
                                match = date.DayOfWeek == DayOfWeek.Monday;
                                break;
                            case "DIE":
                                match = date.DayOfWeek == DayOfWeek.Tuesday;
                                break;
                            case "MIT":
                                match = date.DayOfWeek == DayOfWeek.Wednesday;
                                break;
                            case "DON":
                                match = date.DayOfWeek == DayOfWeek.Thursday;
                                break;
                            case "FRE":
                                match = date.DayOfWeek == DayOfWeek.Friday;
                                break;
                            case "SAM":
                                match = date.DayOfWeek == DayOfWeek.Saturday;
                                break;
                            case "SON":
                                match = date.DayOfWeek == DayOfWeek.Sunday;
                                break;
                            default:
                                match = line[0] == dateString;
                                break;
                        }

                        if (match)
                        {
                            found = dayType;
                        }
                    });
            return found;
        }

        /// <summary>
        /// Loads the simple stop list.
        /// </summary>
        /// <returns>
        /// The list of simple stops.
        /// </returns>
        public IEnumerable<SimpleStop> LoadSimpleStopList()
        {
            var stopList = new List<SimpleStop>();
            ReadDatabaseFileLines(
                "poi.csv",
                true,
                line =>
                    {
                        // 19 fields in v6
                        if (line.Length != 19 || line[0].StartsWith("#"))
                        {
                            return;
                        }

                        var id = SafeParseInt(line[0]);
                        if (stopList.Find(s => s.Id == id) != null)
                        {
                            return;
                        }

                        stopList.Add(
                            new SimpleStop
                                {
                                    Id = id,
                                    OrtOrigin = SafeParseInt(line[1]),
                                    Longitude = float.Parse(line[3]),
                                    Latitude = float.Parse(line[4]),
                                    Buffer = SafeParseInt(line[5]),
                                    Zone = SafeParseInt(line[15])
                                });
                    });

            return stopList;
        }

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
        public IEnumerable<TripInfo> LoadTrips(int dayType, int lineNumber, int serviceNumber, int tripNumber)
        {
            var trips = new List<TripInfo>();
            ReadDatabaseFileLines(
                "ser.csv",
                true,
                line =>
                    {
                        if (line.Length < 2 || line[0].StartsWith("#"))
                        {
                            return;
                        }

                        var foundLineNumber = SafeParseInt(line[0]);
                        var foundServiceNumber = SafeParseInt(line[1]);
                        if ((foundLineNumber == lineNumber || lineNumber == 0)
                            && (foundServiceNumber == serviceNumber || serviceNumber == 0))
                        {
                            LoadTrips(dayType, foundLineNumber, foundServiceNumber, tripNumber, trips);
                        }
                    });

            return trips;
        }

        /// <summary>
        /// Loads timetable parameters.
        /// </summary>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <returns>
        /// The <see cref="ParamIti"/> object containing all parameters.
        /// </returns>
        public ParamIti LoadParamIti(int lineNumber)
        {
            // see IQube_LoadParam() and readParamLine() in the old code
            ParamIti paramIti = null;
            ReadDatabaseFileLines(
                "paramiti.csv",
                true,
                line =>
                    {
                        if (line.Length < 10 || line[0].StartsWith("#"))
                        {
                            return;
                        }

                        var readLineNumber = line[0].StartsWith("D") ? 0 : SafeParseInt(line[0]);
                        if (readLineNumber != 0 && readLineNumber != lineNumber)
                        {
                            return;
                        }

                        paramIti = new ParamIti
                                       {
                                           Line = readLineNumber,
                                           ItiOut = SafeParseInt(line[1]),
                                           ItiIn = SafeParseInt(line[2]),
                                           ItiDelete = SafeParseInt(line[3]),
                                           TimePlusLine = TimeSpan.FromMinutes(SafeParseInt(line[4])),
                                           TimeMinusLine = TimeSpan.FromMinutes(SafeParseInt(line[5])),
                                           TimePlusService = TimeSpan.FromMinutes(SafeParseInt(line[6])),
                                           TimeMinusService = TimeSpan.FromMinutes(SafeParseInt(line[7])),
                                           TimePlusValidated = TimeSpan.FromMinutes(SafeParseInt(line[8])),
                                           TimeMinusValidated = TimeSpan.FromMinutes(SafeParseInt(line[9]))
                                       };
                        if (line.Length >= 15)
                        {
                            paramIti.MinDistanceAnnouncement = SafeParseInt(line[13]);
                            paramIti.BufferAnnouncement = SafeParseInt(line[14]);
                        }
                    });
            return paramIti;
        }

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
        public IEnumerable<DriverTripInfo> LoadDriverTrips(string name, int dayType)
        {
            var driverTrips = new List<DriverTripInfo>();
            ReadDatabaseFileLines(
                "ser_agent.csv",
                false,
                line =>
                    {
                        if (line.Length < 8 || line[0].StartsWith("#"))
                        {
                            return;
                        }

                        var foundName = line[0];
                        var foundDayKind = SafeParseInt(line[1]);
                        var foundBlock = SafeParseInt(line[2]);
                        var foundReleveIndex = SafeParseInt(line[4]);
                        var foundReleveTime = SafeParseInt(line[5]);
                        var foundTrips = line[7];
                        if (foundDayKind == dayType && foundName == name)
                        {
                            var trips = foundTrips.Split(',');
                            for (var i = 0; i < trips.Length; i++)
                            {
                                driverTrips.Add(
                                    new DriverTripInfo
                                        {
                                            Trip = SafeParseInt(trips[i]),
                                            Block = foundBlock,
                                            ReliefTime = TimeSpan.FromSeconds(foundReleveTime),
                                            Relief1Index = i == 0 ? foundReleveIndex : -1
                                        });
                            }
                        }
                    });

            return driverTrips;
        }

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
        public RouteInfo LoadRoute(int lineNumber, int routeId, int timeGroup)
        {
            // was IQube_LoadItineraire() in old code
            const int MaxZt = 150;

            if (timeGroup > MaxZt)
            {
                Logger.Error("Unable to read FZT>{0}", MaxZt);
                return null;
            }

            if (timeGroup < 1)
            {
                Logger.Error("Unable to read FZT<1");
                return null;
            }

            var route = new RouteInfo { RouteId = routeId };
            var totalPoints = 0;
            var fileName = string.Format("{0:D3}_rou.csv", lineNumber);
            var theoTime = TimeSpan.Zero;
            ReadDatabaseFileLines(
                fileName,
                true,
                line =>
                    {
                        if (line.Length < 7 || line[0].StartsWith("#"))
                        {
                            return;
                        }

                        totalPoints++;
                        var routeName = line[0];
                        if (Array.IndexOf(routeName.Split(','), routeId.ToString()) < 0)
                        {
                            // see isIQube_ItineraireInItineraires
                            return;
                        }

                        var id = SafeParseInt(line[1]);
                        var times = new List<PointTimes>();
                        for (int i = 4; i + 2 < line.Length; i += 3)
                        {
                            times.Add(
                                new PointTimes
                                    {
                                        AverageTo = SafeParseInt(line[i]),
                                        MinimumTo = SafeParseInt(line[i + 1]),
                                        AverageAt = SafeParseInt(line[i + 2]),
                                    });
                        }

                        if (timeGroup > times.Count)
                        {
                            throw new IndexOutOfRangeException("Unable to read FZT " + timeGroup);
                        }

                        var time = times[timeGroup - 1];
                        theoTime += TimeSpan.FromSeconds(time.AverageTo + time.AverageAt);
                        route.Points.Add(new PointInfo { Id = id, TheoreticalPassageTime = theoTime });
                    });

            Logger.Debug(
                "{0} points read - {1} points found for line: {2} and route: {3}",
                totalPoints,
                route.Points.Count,
                lineNumber,
                routeId);
            return route;
        }

        /// <summary>
        /// Loads the via information into the given route.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="lineNumber">
        /// The line number of the route.
        /// </param>
        public void LoadVia(RouteInfo route, int lineNumber)
        {
            // was IQube_LoadVia() in old code
            if (route == null)
            {
                return;
            }

            var matchPoints = 0;
            var totalPoints = 0;
            ReadDatabaseFileLines(
                "via.csv",
                true,
                line =>
                {
                    // 19 fields in v6
                    if (line.Length != 19 || line[0].StartsWith("#"))
                    {
                        return;
                    }

                    var foundLineNumber = SafeParseInt(line[0]);
                    var foundRouteId = SafeParseInt(line[2]);
                    totalPoints++;
                    if (foundLineNumber != lineNumber || foundRouteId != route.RouteId)
                    {
                        return;
                    }

                    var id = SafeParseInt(line[3]);
                    if (route.Points[matchPoints].Id != id)
                    {
                        route.Points.Insert(matchPoints, new PointInfo { Id = id });
                    }

                    var point = route.Points[matchPoints];
                    matchPoints++;
                    point.LineName = line[1];
                    point.Type = (PointType)SafeParseInt(line[4]);
                    point.SubType = SafeParseInt(line[5]);
                    point.Buffer = SafeParseInt(line[7]);
                    point.SignCode = SafeParseInt(line[11]);
                    point.ExteriorAnnouncementMp3 = SafeParseInt(line[12]);
                    point.ExteriorAnnouncement = line[13];
                    point.ExteriorAnnouncementTts = line[14];
                    point.Zone = SafeParseInt(line[15]);
                    point.Direction = SafeParseInt(line[17]);
                    point.Data = SafeParseInt(line[18]);
                    switch (line[8])
                    {
                        case "via":
                            point.InteriorSoundMode = InteriorSoundMode.Via;
                            point.InteriorAnnouncementMp3 = SafeParseInt(line[9]);
                            break;
                        case "mute":
                            point.InteriorSoundMode = InteriorSoundMode.Mute;
                            break;
                        default:
                            point.InteriorSoundMode = InteriorSoundMode.Poi;
                            break;
                    }
                });
            Logger.Debug("{0} points read - {1} points updated", totalPoints, matchPoints);
        }

        /// <summary>
        /// Loads all point information into the route.
        /// </summary>
        /// <param name="route">
        /// The route for which to load the points.
        /// </param>
        public void LoadPoints(RouteInfo route)
        {
            // was IQube_LoadPoints() in old code
            if (route == null)
            {
                return;
            }

            var totalPoints = 0;
            var matchPoints = 0;
            ReadDatabaseFileLines(
                "poi.csv",
                true,
                line =>
                {
                    // 19 fields in v6
                    if (line.Length != 19 || line[0].StartsWith("#"))
                    {
                        return;
                    }

                    var id = SafeParseInt(line[0]);
                    var found = false;
                    foreach (var point in route.Points)
                    {
                        if (point.Id != id)
                        {
                            continue;
                        }

                        point.OrtOrigin = SafeParseInt(line[1]);
                        point.Longitude = float.Parse(line[3]);
                        point.Latitude = float.Parse(line[4]);
                        point.Name1 = line[7];
                        point.Name1Tts = line[8];
                        point.Name2 = line[10];
                        point.Name2Tts = line[11];
                        point.Didok = SafeParseInt(line[17]);
                        point.DruckName = line[18];

                        if (!found)
                        {
                            found = true;
                            point.VoiceType = SafeParseInt(line[12]);
                            point.Type = (PointType)SafeParseInt(line[13]);
                            point.SubType = SafeParseInt(line[14]);
                            point.Zone = SafeParseInt(line[15]);
                            if (point.Buffer == 0)
                            {
                                point.Buffer = SafeParseInt(line[5]);
                            }

                            if (point.InteriorSoundMode == InteriorSoundMode.Poi)
                            {
                                point.ExteriorAnnouncementMp3 = SafeParseInt(line[6]);
                            }
                        }
                    }

                    totalPoints++;
                    if (found)
                    {
                        matchPoints++;
                    }
                });
            Logger.Debug("{0} points read - {1} points updated", totalPoints, matchPoints);
        }

        private static int SafeParseInt(string input)
        {
            int value;
            return ParserUtil.TryParse(input.Trim(), out value) ? value : 0;
        }

        private static void ReadDatabaseFileLines(string fileName, bool skipFirstLine, Action<string[]> lineHandler)
        {
            try
            {
                using (var reader = new CsvReader(PathManager.Instance.GetPath(FileType.Database, fileName), Encoding))
                {
                    if (skipFirstLine)
                    {
                        reader.GetCsvLine();
                    }

                    string[] line;
                    while ((line = reader.GetCsvLine()) != null)
                    {
                        try
                        {
                            lineHandler(line);
                        }
                        catch (Exception ex)
                        {
                            Logger.DebugException("Coudn't read " + fileName + " line: " + string.Join(";", line), ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't read " + fileName, ex);
            }
        }

        private static void LoadTrips(
            int dayType,
            int lineNumber,
            int serviceNumber,
            int tripNumber,
            ICollection<TripInfo> trips)
        {
            var fileName = string.Format("{0:D3}_frt.csv", lineNumber);
            var total = 0;
            var matches = 0;
            ReadDatabaseFileLines(
                fileName,
                true,
                line =>
                    {
                        // v5 has 11 fields
                        if (line.Length != 11 || line[0].StartsWith("#"))
                        {
                            return;
                        }

                        var foundDayType = SafeParseInt(line[2]);
                        var foundTrip = SafeParseInt(line[4]);
                        var foundService = SafeParseInt(line[5]);
                        total++;
                        if (foundService == serviceNumber && foundDayType == dayType
                            && (tripNumber == 0 || tripNumber == foundTrip))
                        {
                            trips.Add(
                                new TripInfo
                                    {
                                        StartTime = TimeSpan.FromSeconds(SafeParseInt(line[0])),
                                        TripId = foundTrip,
                                        CustomerTripId = SafeParseInt(line[9]),
                                        RouteId = SafeParseInt(line[1]),
                                        LineNumber = lineNumber,
                                        ServiceNumber = foundService,
                                        TimeGroup = SafeParseInt(line[3]),
                                        FirstStopIndex = 0
                                    });
                            matches++;
                        }
                    });
            Logger.Debug("{0} trips read - {1} trips found for block {2}", total, matches, serviceNumber);
        }

        private class PointTimes
        {
            // Temps de parcours moyen depuis le point précedent
            public int AverageTo { get; set; }

            // Temps de parcours minimum depuis le point précedent
            public int MinimumTo { get; set; }

            // Temps d'arret moyen au point
            public int AverageAt { get; set; }
        }
    }
}