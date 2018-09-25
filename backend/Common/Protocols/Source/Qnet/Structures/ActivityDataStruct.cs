// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityDataStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines ActivityDataStruct used with <see cref="DisplayStruct" />.
//   Len = 38 bytes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines ActivityDataStruct used with <see cref="DisplayStruct"/>.
    /// Len = 38 bytes
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ActivityDataStruct
    {
        /// <summary>
        /// Bus Line identifier / Line number
        /// </summary>
        public ushort LineId; 

        /// <summary>
        /// 
        /// </summary>
        public ushort Umlauf;

        /// <summary>
        /// 
        /// </summary>
        public ushort TripId;

        public ushort TripId2;

        public ushort WagonId;

        public ushort OrtsNbr;

        public int Time;

        public byte Tagesart;


        public byte byteParam1;

        public byte byteParam2;

        public byte byteParam3;

        /// <summary>
        /// For Display struct, represent the <see cref="DisplayMode"/>.
        /// </summary>
        public ushort ushortParam1;

        public ushort ushortParam2;

        public ushort ushortParam3;

        public uint uintParam1;

        public uint uintParam2;

        public uint uintParam3;
    }
}
