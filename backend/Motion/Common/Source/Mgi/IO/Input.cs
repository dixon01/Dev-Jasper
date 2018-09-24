// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Input.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Input type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    /// <summary>
    /// An input on the MGI topbox.
    /// </summary>
    public sealed class Input : IOBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Input"/> class.
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
        internal Input(byte address, byte pin, InputOutputManagerBase manager)
            : base(address, pin, manager)
        {
        }
    }
}