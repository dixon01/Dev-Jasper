// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompactRs485Switch.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompactRs485Switch type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Mgi
{
    /// <summary>
    /// The MGI RS485 interface switch state.
    /// </summary>
    public enum CompactRs485Switch
    {
        /// <summary>
        /// Interface connected to CPU.
        /// </summary>
        Cpu = 0,

        /// <summary>
        /// Interface connected to at91 (default)
        /// </summary>
        At91 = 1
    }
}