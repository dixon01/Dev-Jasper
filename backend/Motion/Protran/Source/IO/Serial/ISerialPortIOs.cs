// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerialPortIOs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISerialPortIOs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Serial
{
    using System;

    /// <summary>
    /// Interface to the inputs and outputs of a serial port.
    /// </summary>
    public interface ISerialPortIOs
    {
        /// <summary>
        /// Event that is risen when the <see cref="CtsHolding"/> changes.
        /// </summary>
        event EventHandler CtsChanged;

        /// <summary>
        /// Event that is risen when the <see cref="DsrHolding"/> changes.
        /// </summary>
        event EventHandler DsrChanged;

        /// <summary>
        /// Event that is risen when the <see cref="DtrEnable"/> changes.
        /// </summary>
        event EventHandler DtrChanged;

        /// <summary>
        /// Event that is risen when the <see cref="RtsEnable"/> changes.
        /// </summary>
        event EventHandler RtsChanged;

        /// <summary>
        /// Gets a value indicating whether the CTS line (Clear To Send) is held.
        /// </summary>
        bool CtsHolding { get; }

        /// <summary>
        /// Gets a value indicating whether DSR line (Data Set Ready) is held.
        /// </summary>
        bool DsrHolding { get; }

        /// <summary>
        /// Gets or sets a value indicating whether DTR line (Data Terminal Ready) is enabled.
        /// </summary>
        bool DtrEnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RTS line (Request To Send) is enabled.
        /// </summary>
        bool RtsEnable { get; set; }
    }
}