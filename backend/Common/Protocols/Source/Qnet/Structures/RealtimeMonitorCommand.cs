// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeMonitorCommand.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the enumeration of all available Realtime monitioring Command
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    /// <summary>
    /// Defines the enumeration of all available Realtime monitioring Command
    /// </summary>
    internal enum RealtimeMonitorCommand : short
    {
        /// <summary>
        /// Unknown command (legacy code = RTMON_CMD_NONE)
        /// </summary>
        None = -1,

        /// <summary>
        /// 0: Start Realtime Monitoring     [tRTMonStart] (legacy code = RTMON_CMD_START)
        /// </summary>
        Start = 0,              

        /// <summary>
        /// 1: Stop Realtime Monitoring      [-] (legacy code = RTMON_CMD_STOP)
        /// </summary>
        Stop,                   

        /// <summary>
        /// 2: Get Realtime Monitoring Data  [-] (legacy code = RTMON_CMD_GET_DATA)
        /// </summary>
        GetData,               

        /// <summary>
        /// 3: Realtime Monitoring Data      [tRTMonData] (legacy code = RTMON_CMD_DATA)
        /// </summary>
        Data,                  

        /// <summary>
        /// Max commdn (legacy code = RTMON_CMD_MAX)
        /// </summary>
        Max 
    }
}
