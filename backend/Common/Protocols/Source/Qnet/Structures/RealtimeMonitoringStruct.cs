// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeMonitoringStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Structure that defines fields for realtime monitoring. (legacy name = tRTMonMsg)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Structure that defines fields for realtime monitoring. (legacy name = tRTMonMsg)    
    /// </summary>
    /// <remarks>Len = 224 bytes</remarks>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct RealtimeMonitoringStruct
    {
        /// <summary>
        /// Realtime monitoring command. See <see cref="RealtimeMonitorCommand"/> for more details.
        /// </summary>
        /// <remarks>Len = 2 bytes</remarks>
        [FieldOffset(0)]
        public short Command;

        /// <summary>
        /// Field containing the start information to request the realtime data to the iqube. 
        /// <para>See <see cref="RealtimeStartStruct"/> for details.</para>
        /// </summary>
        /// <remarks>        
        /// <para>The start message is not mandatory. 
        /// If an iqube receives a <see cref="RealtimeMonitoringStruct.Command"/> with <see cref="RealtimeMonitorCommand.GetData"/>
        ///  value, it will always send back a <see cref="RealtimeMonitoringStruct.Data"/> message.
        /// </para>
        /// <para>Len = 4 bytes</para>
        /// </remarks>
        [FieldOffset(2)]
        public RealtimeStartStruct Start;

        /// <summary>
        /// Field containing the data with the realtime data displayed by the iqube.
        /// <para>See <see cref="RealtimeDataStruct"/></para> for details.
        /// </summary>
        /// <remarks>Len = bytes</remarks>
        [FieldOffset(2)]
        public RealtimeDataStruct Data;
    }
}
