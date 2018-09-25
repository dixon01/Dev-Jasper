// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsAntennaState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The type of GPS device state.
    /// </summary>
    public enum GpsAntennaState
    {
        /// <summary>
        /// The antenna is OK.
        /// </summary>
        Ok = 0x00,

        /// <summary>
        /// The antenna short circuit.
        /// </summary>
        ShortCircuit = 0x04,

        /// <summary>
        /// The antenna is plugged but the consumption is unusual.
        /// </summary>
        UnusualConsumption = 0x08,

        /// <summary>
        /// No antenna.
        /// </summary>
        NotConnected = 0x0C
    }
}
