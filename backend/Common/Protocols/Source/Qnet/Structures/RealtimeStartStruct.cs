// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeStartStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Realtime monitoring start structure used into <see cref="RealtimeMonitorStruct" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    /// <summary>
    /// Realtime monitoring start structure used into <see cref="RealtimeMonitoringStruct"/>
    /// Lenght = 4 bytes
    /// </summary>
    public struct RealtimeStartStruct
    {
        /// <summary>
        /// Start mode : 
        /// <para>0 Indicates to the iqube to send data only on request.</para>
        /// <para>1 Indicates to the iqube to send data and request but also when something change.</para>
        /// </summary>
        public ushort Mode;

        /// <summary>
        /// Interval of data request from Realtime monitoring server in seconds
        /// this value in the start message is used by the iqube to check if the background system is still alive. 
        /// If the realtime monitoring was started (with a start message) but there was no GetData message within three 
        /// times the interval, the iqube will stop realtime monitoring itself. 
        /// The interval can be set to 0 to disable this function.
        /// </summary>
        public ushort Interval;
    }
}
