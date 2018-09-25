// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortFactory.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Serial
{
    using Gorba.Common.Configuration.Protran.IO;

    /// <summary>
    /// Factory class that creates a serial port controller for a given configuration.
    /// </summary>
    public partial class SerialPortFactory
    {
        /// <summary>
        /// This creates a serial port controller based on configuration.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <returns>
        /// The <see cref="SerialPortController"/>.
        /// </returns>
        public virtual SerialPortController CreateSerialPortController(SerialPortConfig port)
        {
                if (!port.Enabled)
                {
                    return null;
                }

            return new SerialPortController(port);
        }
    }
}
