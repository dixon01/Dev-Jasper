// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    using Kontron.Jida32;

    /// <summary>
    /// Base class for a single I/O pin: <see cref="Input"/> or <see cref="Output"/>.
    /// </summary>
    public abstract class IOBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IOBase"/> class.
        /// </summary>
        /// <param name="address">
        /// The I2C address of the Bus Expander containing this I/O.
        /// </param>
        /// <param name="pin">
        /// The index of the pin (0-based) on the Bus Expander.
        /// </param>
        /// <param name="manager">
        /// The manager.
        /// </param>
        internal IOBase(byte address, byte pin, InputOutputManagerBase manager)
        {
            this.Address = address;
            this.Pin = pin;
            this.Manager = manager;
        }

        /// <summary>
        /// Gets the I2C address of the Bus Expander containing this I/O.
        /// </summary>
        internal byte Address { get; private set; }

        /// <summary>
        /// Gets the index of the pin (0-based) on the Bus Expander.
        /// </summary>
        internal byte Pin { get; private set; }

        /// <summary>
        /// Gets the manager that created this I/O.
        /// </summary>
        internal InputOutputManagerBase Manager { get; private set; }

        /// <summary>
        /// Reads the value of this I/O.
        /// </summary>
        /// <returns>
        /// True if the I/O is set, otherwise false.
        /// </returns>
        /// <exception cref="JidaException">
        /// if the I/O port couldn't be read.
        /// </exception>
        public bool Read()
        {
            return this.Manager.Read(this);
        }
    }
}