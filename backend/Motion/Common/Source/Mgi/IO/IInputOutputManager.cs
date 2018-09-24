// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInputOutputManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IInputOutputManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    using System;

    /// <summary>
    /// Interface to access the inputs and outputs of an MGI device.
    /// </summary>
    public interface IInputOutputManager : IDisposable
    {
        /// <summary>
        /// Gets the general button input.
        /// </summary>
        Input Button { get; }

        /// <summary>
        /// Gets the update LED output.
        /// </summary>
        Output UpdateLed { get; }

        /// <summary>
        /// Gets the antenna short circuit input.
        /// </summary>
        Input GpsShortCircuit { get; }

        /// <summary>
        /// Gets the antenna current detection input.
        /// </summary>
        Input GpsCurrentDetection { get; }

        /// <summary>
        /// Gets the number of general programmable inputs and outputs in the system.
        /// </summary>
        int GpioCount { get; }

        /// <summary>
        /// Gets the given general programmable I/O.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="IOBase"/>.
        /// </returns>
        IOBase GetGpio(int index);

        /// <summary>
        /// Reads all general programmable I/O's and returns them in an array of flags.
        /// This method is considerably faster than querying every single I/O for its value
        /// using <see cref="IOBase.Read"/>.
        /// A single call to this method does the same amount of I/O operations as
        /// one call to <see cref="IOBase.Read"/>.
        /// </summary>
        /// <returns>
        /// An array with <see cref="GpioCount"/> values. The values correspond to the
        /// I/O returned by <see cref="GetGpio"/> with the given index.
        /// </returns>
        bool[] ReadAllGpios();

        /// <summary>
        /// Gets the graphic control output.
        /// </summary>
        /// <param name="index">
        /// The index (from 0).
        /// </param>
        /// <param name="pin">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Output"/>.
        /// </returns>
        Output GetGraphicControlOutput(int index, GraphicControlPin pin);

        /// <summary>
        /// Gets the multiprotocol transceiver port.
        /// </summary>
        /// <param name="index">
        ///     The index (from 0).
        /// </param>
        /// <param name="pin">
        ///     The type.
        /// </param>
        /// <returns>
        /// The <see cref="IOBase"/>.
        /// </returns>
        Output GetMultiprotocolTransceiverOutput(int index, MultiprotocolTransceiverPin pin);
    }
}