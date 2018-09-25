// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiprotocolTransceiverPin.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiprotocolTransceiverPin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    /// <summary>
    /// The three possible multiprotocol transceiver pins.
    /// </summary>
    public enum MultiprotocolTransceiverPin
    {
        /// <summary>
        /// The type (RS485/RS232) pin (bits 0 and 3).
        /// </summary>
        Type = 0,

        /// <summary>
        /// The termination pin (bits 1 and 4).
        /// </summary>
        Termination = 1,

        /// <summary>
        /// The mode (full / half duplex) pin (bits 2 and 5).
        /// </summary>
        Mode = 2,
    }
}