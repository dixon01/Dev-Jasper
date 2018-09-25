// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsConnectionType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.HardwareManager.Gps
{
    /// <summary>
    ///     The GPS connection type.
    /// </summary>
    public enum GpsConnectionType
    {
        /// <summary>
        ///     The GPS pilot used for ADT simulations.
        /// </summary>
        GpsPilot,

        /// <summary>
        ///     Used for a GPS receiver connected to a serial port.
        /// </summary>
        GpsSerial
    }
}