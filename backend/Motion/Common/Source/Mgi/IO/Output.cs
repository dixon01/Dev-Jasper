// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Output.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Output type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    using Kontron.Jida32;

    /// <summary>
    /// An output on the MGI topbox.
    /// </summary>
    public sealed class Output : IOBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Output"/> class.
        /// </summary>
        /// <param name="address">
        /// The I2C address of the Bus Expander containing this output.
        /// </param>
        /// <param name="pin">
        /// The index of the pin (0-based) on the Bus Expander.
        /// </param>
        /// <param name="manager">
        /// The manager.
        /// </param>
        internal Output(byte address, byte pin, InputOutputManagerBase manager)
            : base(address, pin, manager)
        {
        }

        /// <summary>
        /// Writes a value to this output.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="JidaException">
        /// if the I/O port couldn't be written.
        /// </exception>
        public void Write(bool value)
        {
            this.Manager.Write(this, value);
        }
    }
}