// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortReopen.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortReopen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.MatrixRenderer
{
    using System.IO.Ports;

    /// <summary>
    /// Supported serial port reopen cases
    /// </summary>
    public enum SerialPortReopen
    {
        /// <summary>
        /// For none of the serial port errors
        /// </summary>
        None,

        /// <summary>
        /// For frame only serial port error
        /// </summary>
        FrameOnly = SerialError.Frame,

        /// <summary>
        /// For all serial port errors
        /// </summary>
        All = SerialError.Frame + SerialError.Overrun + SerialError.RXOver + SerialError.RXParity + SerialError.TXFull
    }
}