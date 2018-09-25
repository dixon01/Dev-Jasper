// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RxState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   States used to decode datagram with a state machine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// States of transmit used to decode datagram with a state machine.
    /// </summary>
    internal enum RxState
    {
        /// <summary>
        /// Pausing state
        /// </summary>
        RxPause = 0,

        /// <summary>
        /// Getting the lenght of the frame
        /// </summary>
        RxLength,

        /// <summary>
        /// Getting the version type of the frame 
        /// </summary>
        RxVerTyp,

        /// <summary>
        /// Getting the the header of the frame 
        /// </summary>
        RxHeader,

        /// <summary>
        /// Getting the the data of the frame
        /// </summary>
        RxData,

        /// <summary>
        /// Current state checking the in Crc low part
        /// </summary>
        RxCrcLow,

        /// <summary>
        /// Current state checking Crc high part
        /// </summary>
        RxCrcHigh,

        /// <summary>
        /// Reinitialization of decoding
        /// </summary>
        RxReinit
    }
}
