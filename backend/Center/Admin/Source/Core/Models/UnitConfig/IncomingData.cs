// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncomingData.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IncomingData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using System;

    /// <summary>
    /// The possible incoming data items.
    /// </summary>
    [Flags]
    public enum IncomingData : uint
    {
        /// <summary>
        /// IBIS (VDV 300) protocol.
        /// </summary>
        Ibis = 0x00000001,

        /// <summary>
        /// VDV 301 (IBIS-IP) protocol.
        /// </summary>
        Vdv301 = 0x00000002,

        /// <summary>
        /// Location Awareness Module Ximple protocol.
        /// </summary>
        AudioPeripheral = 0x00002000,

        /// <summary>
        /// Location Awareness Module Ximple protocol.
        /// </summary>
        LamXimple = 0x00004000,

        /// <summary>
        /// Ximple protocol.
        /// </summary>
        Ximple = 0x00008000,

        /// <summary>
        /// SNTP protocol.
        /// </summary>
        Sntp = 0x00010000,
        
        /// <summary>
        /// AdHoc protocol
        /// </summary>
        AdHoc = 0x00020000,

        /// <summary>
        /// Medi protocol.
        /// </summary>
        Medi = 0x80000000
    }
}
