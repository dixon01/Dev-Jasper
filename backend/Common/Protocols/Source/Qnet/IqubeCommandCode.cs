// -----------------------------------------------------------------------
// <copyright file="IqubeCommandCode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using Gorba.Common.Protocols.Qnet.Structures;

    /// <summary>
    /// Enumerates the list of the available iqube command codes.
    /// </summary>
    public enum IqubeCommandCode : sbyte
    {
        /// <summary>
        /// None
        /// </summary>
        IQU_CMD_NONE = -1,

        /// <summary>
        /// Reserved 
        /// </summary>
        IQU_CMD_RESERVED = 0,

        /// <summary>
        /// Command to test; command #1
        /// </summary>
        IQU_CMD_TEST,

        /// <summary>
        /// Command to restart the iqube; command #2
        /// </summary>
        IQU_CMD_RESTART,

        /// <summary>
        /// Command to synchronize the time of the unit; command #3
        /// Uses [tQNTPmsg]
        /// </summary>
        TimeSyncCommand,

        /// <summary>
        /// Upgrade command; command #4 
        /// Uses [tUpgrade]
        /// </summary>
        IQU_CMD_UPGRADE,

        /// <summary>
        /// Set the display on; command #5
        /// </summary>
        IQU_CMD_DISP_ON,

        /// <summary>
        /// set display off; command #6
        /// </summary>
        IQU_CMD_DISP_OFF,

        /// <summary>
        /// Enables to execute a specific task; command #7 (legacy code = IQU_CMD_DISPOSITION)
        /// See <see cref="ActivityStruct"/>
        /// </summary>
        ActivityCommand,

        /// <summary>
        /// Delete all registered activities
        /// </summary>
        IQU_CMD_DISPO_DEL_ALL,

        /// <summary>
        /// Get ids of all registered activities (legacy code = IQU_CMD_DISPO_GET_IDS); command #9
        /// </summary>
        GetActivityIdsCommand,

        /// <summary>
        /// clock synchronization; command #8
        /// [tMyTime]
        /// </summary>
        IQU_CMD_CLOCK_SYNC,

        /// <summary>
        /// set line on (display); command #9
        /// </summary>
        IQU_CMD_LINE_ON,

        /// <summary>
        /// Set line off (display); command #10
        /// </summary>
        IQU_CMD_LINE_OFF,

        /// <summary>
        /// New installation specific parameter 1; command #11
        /// </summary>
        IQU_CMD_NEW_INST_PARAM1
    }
}
