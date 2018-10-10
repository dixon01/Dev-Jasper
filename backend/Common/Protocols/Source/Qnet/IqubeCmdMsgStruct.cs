// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IqubeCmdMsgStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//      Defines all structures needed to send an iqube Cmd message to execute a command on iqube.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.InteropServices;

    using Gorba.Common.Protocols.Qnet.Structures;

    /// <summary>
    /// Structure that defines fields for Iqube Cmd message that enables to execute command on iqube.
    /// <remarks>Implements only task for momment</remarks>
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct IqubeCmdMsgStruct
    {
        /// <summary>
        /// IQUBE command code; code of the task
        /// </summary>
        [FieldOffset(0)]
        public byte CommandCode;

        /// <summary>
        /// command specific parameter
        /// </summary>
        [FieldOffset(1)]
        public byte SpecificParameter;

        /// <summary>
        /// Contains the data of the task in case of the command is a "task".
        /// Len = 186
        /// </summary>
        [FieldOffset(2)]
        public ActivityStruct Task;

        /// <summary>
        /// Union part : Time synchronization command message
        /// </summary>
        [FieldOffset(2)]
        public QntpMsgStruct TimeSync;

        /// <summary>
        /// Union part : ActivityIds command message
        /// len = 193
        /// </summary>
        [FieldOffset(2)]
        public ActivityIdsStruct ActivityIds;
        /*
         * union
  {
    BOOL      boolParam;
    tQNTPmsg  timeSync;
    tUpgrade  upgrade;
    tTask     task;
    tMyTime   clock;
  } u;
         * */
    }

    /// <summary>
    /// Condition for the task start 
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ConditionStartStruct
    {
        /// <summary>
        /// Condition start time of the task as Dos time 
        /// </summary>
        [FieldOffset(0)]
        public DOSTimeStruct StartTime;

        /// <summary>
        /// Condition start time of the task as day seconds
        /// </summary>
        [FieldOffset(0)]
        public uint StartDaySeconds;
    }

    /// <summary>
    /// Contains data for condition count
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ConditionCountStruct
    {
        /// <summary>
        /// max. count
        /// </summary>
        public ushort MaxTimes;

        /// <summary>
        /// current count: internally used in iqube software
        /// Set always with 0
        /// </summary>
        public ushort Current;
    }

    /// <summary>
    /// Condition for the task stop. It's implemented like an union structure.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ConditionStopStruct
    {
        /// <summary>
        /// absolute time/date
        /// </summary>
        [FieldOffset(0)]
        public DOSTimeStruct Time;

        /// <summary>
        /// day-seconds (0..86400)
        /// </summary>
        [FieldOffset(0)]
        public uint DaySecs;

        /// <summary>
        /// Contains the data of the condition count
        /// </summary>
        [FieldOffset(0)]
        public ConditionCountStruct Count;
    }

    /// <summary>
    /// Contains data for the condition of the execution of a task
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ExeConditionStruct
    {
        /// <summary>
        /// Start event
        /// </summary>
        public sbyte StartEvent; // INT8 ??? sbyte ou byte

        /// <summary>
        /// Stop event
        /// </summary>
        public sbyte StopEvent;

        /// <summary>
        /// Conditional start execution of the task
        /// </summary>
        public ConditionStartStruct Start;

        /// <summary>
        /// Conditional stop execution of the task
        /// </summary>
        public ConditionStopStruct Stop;
    }

    /// <summary>
    /// Contains data for the header of the task structure. 
    /// Lenght = 15 bytes
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IqubeTaskHdrStruct
    {
        /// <summary>
        /// Code of the task, see <see cref="TaskCode"/> for list of codes.
        /// </summary>
        public sbyte TaskCode;

        /// <summary>
        /// task/dispo identifier
        /// </summary>
        public uint Id;

        /// <summary>
        /// Condition(s) for execution of the task
        /// </summary>
        public ExeConditionStruct ExeCond;
    }

    /// <summary>
    /// Structure InfoParam for special info line text
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct InfoParamStruct
    {
        /// <summary>
        /// Indicates the font used to display the message.
        /// </summary>
        public sbyte Font; // INT8

        /// <summary>
        /// Indicates the aligment of the text: Left, Center, Right
        /// </summary>
        public byte Align; // BYTE

        /// <summary>
        /// Indicates if the text has to blink or not.
        /// </summary>
        public byte Blink;

        /// <summary>
        /// Indicates if the text has to scroll or not.
        /// </summary>
        public byte Scroll;

        /// <summary>
        /// Info line flags : See enum <see cref="InfoLineFlags"/> for available values.
        /// </summary>
        public sbyte Flags; 

        /// <summary>
        /// ??? Find the right field name 
        /// </summary>
        public sbyte ReservedAsBoolean; 

        /// <summary>
        /// ??? Find the right field name 
        /// </summary>
        public ushort ReservedAsWord; // WORD ???
    }

    /// <summary>
    /// Contains data of the info line data staructure, and especially, the parameters <see cref="InfoParamStruct"/> and the 
    /// text to display.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InfoTextDataStruct
    {
        /// <summary>
        /// Parameters for special info line text
        /// </summary>
        public InfoParamStruct Param;

        /// <summary>
        /// Text to be displayed. Max length = 160 characters
        /// </summary>
        private fixed byte internalText[QnetConstantes.MaxInfoLineTextSize];

        /// <summary>
        /// Gets or sets the text corresponding to the lane.
        /// </summary>
        public string Text
        {
            get
            {
                fixed (byte* ptr = this.internalText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(
                        ptr, QnetConstantes.MaxInfoLineTextSize);
                }
            }

            set
            {
                fixed (byte* ptr = this.internalText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(
                        value, ptr, QnetConstantes.MaxInfoLineTextSize);
                }
            }
        }
    }

    /// <summary>
    /// Contains the data of the special info line text structure.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct InfoLineDataStruct
    {
        /// <summary>
        /// File name, used only for type BITMAP|SEQFILE
        /// </summary>
        [FieldOffset(0)]
        public fixed byte FileName[QnetConstantes.QfsMaxFileNameSize + 4];

        /// <summary>
        /// Contains the info text values : text, scroll, alignment, and so on.
        /// </summary>
        [FieldOffset(0)]
        public InfoTextDataStruct Values;
    }

    /// <summary>
    /// Contains the data of the special info line text task. (Legacy code = tInfoLine)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct InfoLineStruct
    {
        /// <summary>
        /// Identification of InfoLine row
        /// </summary>
        public byte InfoLineRowId;

        /// <summary>
        /// Type of included data (NONE|DATA|BITMAP|SEQFILE)
        /// </summary>
        public sbyte DataType;

        /// <summary>
        /// Contains the data of the special info line text.
        /// </summary>
        public InfoLineDataStruct Data;
    }    
}
