// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDriveInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDriveInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// Interface containing drive information.
    /// </summary>
    public interface IDriveInfo
    {
        /// <summary>
        /// Gets the block number.
        /// </summary>
        int BlockNumber { get; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        int LineNumber { get; }

        /// <summary>
        /// Gets the run number.
        /// </summary>
        int RunNumber { get; }

        /// <summary>
        /// Gets the route path number.
        /// </summary>
        int RoutePathNumber { get; }

        /// <summary>
        /// Gets the zone number.
        /// </summary>
        int ZoneNumber { get; }

        /// <summary>
        /// Gets the destination number.
        /// </summary>
        int DestinationNumber { get; }

        /// <summary>
        /// Gets the driver number.
        /// </summary>
        int DriverNumber { get; }

        /// <summary>
        /// Gets the block number string (used for display).
        /// </summary>
        string SBlockNumber { get; }

        /// <summary>
        /// Gets the line name string (used for display).
        /// </summary>
        string SLineName { get; }

        /// <summary>
        /// Gets the run number string (used for display).
        /// </summary>
        string SRunNumber { get; }

        /// <summary>
        /// Gets the route path number string (used for display).
        /// </summary>
        string SRoutePathNumber { get; }

        /// <summary>
        /// Gets the zone number string (used for display).
        /// </summary>
        string SZoneNumber { get; }

        /// <summary>
        /// Gets the destination number string (used for display).
        /// </summary>
        string SDestinationNumber { get; }

        /// <summary>
        /// Gets the driver number string (used for display).
        /// </summary>
        string SDriverNumber { get; }
    }
}