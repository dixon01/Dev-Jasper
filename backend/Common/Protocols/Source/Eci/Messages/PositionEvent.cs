// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PositionEvent.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The type of a position message.
    /// </summary>
    public enum PositionEvent
    {
        /// <summary>
        /// GPS position update.
        /// </summary>
        Gps = 0,

        /// <summary>
        /// The bus left a stop point.
        /// </summary>
        StopLeft = 1,

        //// NextStop = 2
        //// ApproachingStop = 3

        /// <summary>
        /// The bus entered a stop point.
        /// </summary>
        StopEntry = 4,

        /// <summary>
        /// The bus arrived at the terminus.
        /// </summary>
        AtTerminus = 5,

        /// <summary>
        /// The bus arrived at the departure.
        /// </summary>
        AtDeparture = 7
    }
}
