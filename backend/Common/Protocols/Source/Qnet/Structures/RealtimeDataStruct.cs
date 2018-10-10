// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeDataStruct.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Realtime monitoring start structure used into <see cref="RealtimeMonitorStruct" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Realtime monitoring data structure used into <see cref="RealtimeMonitoringStruct"/>
    /// </summary>
    /// <remarks>Len = 222</remarks>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 166)]
    public struct RealtimeDataStruct
    {
        /// <summary>
        /// Indicates the type of the display that defines data content.
        /// </summary>
        [FieldOffset(0)]
        public ushort DisplayType;

        /// <summary>
        /// Contains info line data. (Legacy code = infoData)
        /// </summary>
        /// <remarks>Len = 164 bytes</remarks>
        [FieldOffset(2)]
        public RealtimeInfoLineStruct RealtimeInfoLine;

        /// <summary>
        /// Display Type S data (Legacy code = dataTypeS)
        /// </summary>
        /// <remarks>Len = 3 bytes</remarks>
        [FieldOffset(2)]
        public RealtimeDataTypeSStruct RealtimeDataTypeS;

       /// <summary>
       /// Display Type C (Legacy code = dataTypeC)
       /// </summary>
        /// <remarks>Len = 32 bytes</remarks>
       [FieldOffset(2)]
       public RealtimeDataTypeCStruct RealtimeDataTypeC;

      /// <summary>
      /// Display Type M (without Lane information) (Legacy code = dataTypeM)
      /// </summary>
      /// <remarks>Len = 220 bytes</remarks>
      [FieldOffset(2)]
      public RealtimeDataTypeMStruct RealtimeDataTypeM;

      /// <summary>
      /// Display Type L (with Lane information) (Legacy code = dataTypeL)
      /// </summary>
      /// <remarks>Len = 195 bytes</remarks>
      [FieldOffset(2)]
      public RealtimeDataTypeLStruct RealtimeDataTypeL;
    }
}