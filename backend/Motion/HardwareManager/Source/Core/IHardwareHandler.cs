// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHardwareHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core
{
    /// <summary>
    /// The hardware handler interface
    /// </summary>
    public interface IHardwareHandler
    {
        /// <summary>
        /// Gets the name of this handler.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the serial number of the underlying hardware.
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// Starts the hardware handler
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the hardware handler
        /// </summary>
        void Stop();
    }
}
