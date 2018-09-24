// --------------------------------------------------------------------------------------------------------------------
// <copyright file="I2CType.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the I2CType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32.I2C
{
    /// <summary>
    /// The type of I2C bus.
    /// </summary>
    public enum I2CType
    {
        /// <summary>
        /// unknown or special purposes
        /// </summary>
        Unknown,

        /// <summary>
        /// primary I2C bus
        /// </summary>
        Primary,

        /// <summary>
        /// system management bus
        /// </summary>
        Smb,

        /// <summary>
        /// JILI interface
        /// </summary>
        Jili
    }
}