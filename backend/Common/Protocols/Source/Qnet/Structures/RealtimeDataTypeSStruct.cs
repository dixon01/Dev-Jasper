// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeDataTypeSStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Data for Realtime monitoring of displayed data for standard display (type S)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Data for Realtime monitoring of displayed data for standard display (type S)
    /// </summary>
    /// <remarks>Len = 3 bytes</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RealtimeDataTypeSStruct
    {
        /// <summary>
        /// Contains the value curretly displayed on the iqube
        /// Display value: -2: SHOW_DASHES [--]; -1: Black [  ]; 0-99: countdown [88]
        /// </summary>
        public short DisplayValue;

        /// <summary>
        /// Attributes: Bit 1: show leading zero On|Off, e.g.: [01] or [ 1]
        /// </summary>
        public byte Attributes;
    }
}
