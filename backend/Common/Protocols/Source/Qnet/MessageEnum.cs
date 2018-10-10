// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEnum.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumetation of all available remote command (used in DirectRemoteControl mode)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Runtime.InteropServices;

    using Gorba.Common.Protocols.Qnet.Structures;

        #region alias declaration

        // These aliases enable to make the // with qnet definitions. 
        // This section should be duplicated into each file that needs these aliases !!!
        // see Gorba.Common.Protocols.Qnet.QnetEnums.cs
    using QNETaddrAlias = System.UInt16;

    #endregion

    /// <summary>
    /// Enumetation of all available remote command (used in DirectRemoteControl mode)
    /// </summary>
    public enum RemoteCommand : ushort
    {
        REMOTE_CMD_DISPLAY_VALUE = 1,

        // ---------------------- 2
        REMOTE_CMD_DISPLAY_TEXT = 3,

        // ---------------------- 4
        REMOTE_CMD_DISPLAY_JAM = 5,

        /// <summary>
        /// Command used to diplay bus line windows in direct remote control mode
        /// </summary>
        DisplayBusLine = 6,

        REMOTE_CMD_DISPLAY_INFO = 7,

        REMOTE_CMD_DISP_ON_OFF = 8,

        REMOTE_CMD_LINE_ON_OFF = 9,

        REMOTE_CMD_CLEAR_LINE = 10,

        REMOTE_CMD_CLEAR_MULTILINE = 11,

        REMOTE_CMD_CLEAR_INFOLINE = 12,

        REMOTE_CMD_SET_DISP_MAX = 13,

        REMOTE_CMD_RESTART = 14,

        REMOTE_CMD_POWER_RESET = 15,

        REMOTE_CMD_SET_SYSTIME = 16,

        REMOTE_CMD_SET_INFOLINE_CLOCK = 17,

        REMOTE_CMD_ACK = 18,

        REMOTE_CMD_STATUS_REQUEST = 19
    }

    /// <summary> 
    /// Enumerates all allowed qnet message types
    /// </summary>
    public enum MSGtyp
    {
        /// <summary>
        /// None type
        /// </summary>
        MSG_TYP_NONE = -1, // -1

        /// <summary>
        /// 0: System Message     [tSysMsg]
        /// </summary>
        MSG_TYP_SYSTEM = 0, // 0

        /// <summary>
        /// 1: Error Message      [tError]
        /// </summary>
        MSG_TYP_ERROR, // 1

        /// <summary>
        /// 2: Test Message
        /// </summary>
        MSG_TYP_TEST_1, // 2

        /// <summary>
        /// 3: Test Message
        /// </summary>
        MSG_TYP_TEST_2, // 3

        /// <summary>
        /// 4: Bus Message        [tBusMsg]
        /// </summary>
        MsgTypBus, // 4

        /// <summary>
        /// 5: IBIS Message       [tIbis]
        /// </summary>
        MSG_TYP_IBIS, // 5 

        /// <summary>
        /// 6: Event Message      [tEVENT] legacy code = MSG_TYP_EVENT
        /// </summary>
        MsgTypEvent, // 6

        /// <summary>
        /// 7: Alarm Message      [tALARMdata]
        /// </summary>
        MSG_TYP_ALARM, // 7 

        /// <summary>
        /// 8: Ort Message        [tOrtMsg]
        /// </summary>
        MSG_TYP_ORT, // 8

        /// <summary>
        /// 9: Trip Message       [tTripMsg]
        /// </summary>
        MSG_TYP_TRIP, // 9

        /// <summary>
        /// 10: VCU verification   [tVcuVerif]
        /// </summary>
        MSG_TYP_VCU_VERIF, // 10

        /// <summary>
        /// 11: VCU command oIqubeMsg.   [tVcuCmd]
        /// </summary>
        MSG_TYP_VCU_CMD, // 11

        /// <summary>
        ///  12: Display Data oIqubeMsg.  [tDispData]
        /// </summary>
        MsgTypDispData, // 12

        /// <summary>
        /// 13: Coach Message      [tCoachMsg]
        /// </summary>
        MSG_TYP_COACH, // 13

        /// <summary>
        /// 14: IQUBE command oIqubeMsg. [tIquCmd] = disposition in german
        /// </summary>
        MsgTypIquCmd,

        /// <summary>
        /// 15: LSA message        [tLsaMsg]
        /// </summary>
        MSG_TYP_LSA, // 15

        /// <summary>
        /// 16: SMS message        [tSMSdata]
        /// </summary>
        MSG_TYP_SMS, // 16

        /// <summary>
        /// 17: Display table data [tDispTabData]
        /// </summary>
        MSG_TYP_DISPTAB_DATA, // 17

        /// <summary>
        /// 18: LSA R09 message    [tLsa_R09]
        /// </summary>
        MSG_TYP_LSA_R09, // 18

        /// <summary>
        /// 19: RBL R06 message    [tRbl_R06]
        /// </summary>
        MSG_TYP_RBL_R06, // 19

        /// <summary>
        /// 20: VDV AZB Nachricht  [VdvMsgStruct]
        /// </summary>
        MsgTypVdv, // 20

        /// <summary>
        /// 23: Realtime Monitoring [tRTMonMsg] (Legacy code = MSG_TYP_RTMON)
        /// </summary>
        MsgTypeRealtimeMonitor = 23,              

        /// <summary>
        /// 25 : Remote message type
        /// </summary>
        MsgTypRemoteBus = 25,

        /// <summary>
        /// 101: Connection Status  [ConStaStruct]
        /// </summary>
        MsgTypConSta = 101, // 101

        /// <summary>
        /// Just to have a max limit for message type
        /// </summary>
        MsgTypMax
    }

    public enum TftpFlag
    {
        /// <summary>
        /// definition of flags for File Write Request [TFTP_WRITE_RQS]. See <see cref="TftpOperationCode"/>
        /// </summary>
        TFTP_FLAG_NONE = 0x0,

        /// <summary>
        /// 
        /// </summary>
        TFTP_FLAG_REPLACE = 0x1,

        /// <summary>
        /// definition of flags for File Status Request [TFTP_FSTA_RQS]. See <see cref="TftpOperationCode"/>
        /// </summary>
        TFTP_FLAG_FSTA_LEN = 0x1,

        /// <summary>
        /// 
        /// </summary>
        TFTP_FLAG_FSTA_CRC = 0x2
    }

    #region Structs bus message

    /// <summary>
    /// Contains data for connection status structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ConStaStruct
    {
        /// <summary>
        /// Qnet address
        /// </summary>
        public QNETaddrAlias QnetAddress;

        /// <summary>
        ///  0: disconnected, 1: connected
        /// </summary>
        public ushort Attribute;

        /// <summary>
        /// Count for connection status ???? 
        /// </summary>
        public ushort Count;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tRemoteCmdMsg
    {
        public UInt16 msgNumber;

        public UInt16 cmd;

        public UInt16 cmdInfo;

        public tRemoteCmdUnion union;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct tRemoteCmdUnion
    {
        [FieldOffset(0)]
        public UInt16 onOff;

        [FieldOffset(0)]
        public tSysState sysState;

        [FieldOffset(0)]
        public tSysTime sysTime;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tSysState
    {
        public Int16 sysState;

        public UInt16 powerState;

        public UInt16 lineState0;

        public UInt16 lineState1;

        public UInt16 lineState2;

        public UInt16 lineState3;

        public UInt16 lineState4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tSysTime
    {
        public tMyTime mytime;

        public tMyDate myDate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tMyTime
    {
        public Int16 ti_hour;

        public Int16 ti_min;

        public Int16 ti_sec;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tMyDate
    {
        public Int16 da_day;

        public Int16 da_mon;

        public Int16 da_year;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tRemoteMsg
    {
        public UInt16 msgNumber;

        public UInt16 cmd;

        public UInt16 lineIndex;

        public UInt16 infoFlag;

        public tLineUnion tUnion;

        // public tInfoLineWindow infoLineWindow;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct tLineUnion
    {
        [FieldOffset(0)]
        public tBusLineWindow busLineWindow;

        [FieldOffset(0)]
        public tInfoLineWindow infoLineWindow;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tInfoLineWindow
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = QnetConstantes.MAX_TEXTLEN_LONG)]
        public byte[] infoLineText;

        public Int16 infoLineTextInfo;
    }

    // from qnet wrapper by Kasimir
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tBusLineWindow
    {
        public Int16 dispVal;

        public UInt16 dispValInfo;

        public UInt16 lineId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = QnetConstantes.MAX_TEXTLEN_SHORT + 1)]
        public byte[] lineText;

        public UInt16 lineTextInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = QnetConstantes.MAX_TEXTLEN_MEDIUM + 1)]
        public byte[] destText;

        public UInt16 destTextInfo;

        public UInt16 dock;
    }

    #endregion

    #region QSNMP message

    internal enum QSNMPCmd
    {
        QSNMP_CMD_GETDISP,

        QSNMP_CMD_GETTIME,

        QSNMP_CMD_RADIOSTAT,

        /// <summary>
        /// (legacy code = QSNMP_CMD_RESTART)
        /// </summary>
        Restart,

        QSNMP_CMD_GETDEPARTURE,

        /// <summary>
        /// Command to synchronize the time (legacy code = QSNMP_CMD_TIME_SYNC)
        /// </summary>
        TimeSynchronization,

        /// <summary>
        /// (legacy code = QSNMP_CMD_DISP_ON)
        /// </summary>
        DisplaySwitchOn,

        /// <summary>
        /// (legacy code = QSNMP_CMD_DISP_OFF)
        /// </summary>
        DisplaySwitchOff,

        QSNMP_CMD_DISP_UPGRADE,

        QSNMP_CMD_SYSTEM_DATA,

        QSNMP_CMD_DISP_MAX
    }

    internal enum QSNMPTyp
    {
        QSNMP_TYP_SINGLE,

        QSNMP_TYP_MULTI
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tQSNMPHdr
    {
        public UInt16 msglen;

        public Int16 cmd;

        public Byte typ;

        public Byte par;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tQSNMPDispRqs
    {
        public UInt16 lineId;

        public Byte flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tQSNMPDispStat
    {
        public UInt16 value;

        public Byte tripstate;

        public Int16 corr;

        public Byte flags; // Bit 1 Disp On

        public Int16 lineId;

        public Int16 umlaufNr;

        public Int16 tripId;

        public Int16 busNr;

        public Byte busPos; // INT8
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct tQSNMPSystemData
    {
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public fixed byte SWVersion [12];

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public fixed byte SWRelDate [12];

        public UInt16 HWVersion;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public fixed Byte HWSerialNbr [5];

        public DOSTimeStruct lastRestart;

        public DOSTimeStruct lastUpgrade;

        public DOSTimeStruct lastDaylightSav;

        public DOSTimeStruct LastTimeStructSync;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tQSNMPRadioStat
    {
        public UInt32 timeStamp; // longWrod

        public UInt16 ortsnumber;

        public Int32 count;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tQSNMPDeparture
    {
        public Int16 code; // Response Code

        public UInt32 departTime; // Departure Time

        public UInt16 tripID; // trip ID

        public Int16 tripState; // trip State
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tQSNMPtimeSync
    {
        public QntpMsgStruct qntp;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct tQSNMPUpgrade
    {
        // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public fixed byte filename [15];

        public UInt16 param;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    // Transform the structure into class to handle better 
    public unsafe struct tQSNMPMsg
    {
        [FieldOffset(0)]
        public tQSNMPDispRqs dispRqs;

        [FieldOffset(0)]
        public tQSNMPtimeSync timeSync;

        [FieldOffset(0)]
        public tQSNMPDispStat svalue;

        //[FieldOffset(0)]
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 120)] // 15*8
        //public fixed tQSNMPDispStat mvalues[8];

        [FieldOffset(0)]
        public tQSNMPRadioStat radioStat;

        [FieldOffset(0)]
        public tQSNMPDeparture departure;

        [FieldOffset(0)]
        public tQSNMPUpgrade upgrade;

        [FieldOffset(0)]
        public tQSNMPSystemData systemData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct QsnmpStruct
    {
        public tQSNMPHdr hdr;

        public tQSNMPMsg msg;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IqubeMessageStruct
    {
        public tQSNMPHdr hdr;

        public tQSNMPMsg msg;
    }

    #endregion

    /// <summary>
    /// Constantes for messages
    /// </summary>
    public class MessageConstantes
    {
        public static readonly int MAX_IQUBES = 5;

        public static readonly int MAX_STATIONS = 15;

        public static readonly int CONFIG_MAX_LINES = 8;

        /// <summary>
        /// (legacy code = MESSAGE_MAX_LINES)
        /// </summary>
        public const int MaxMessageLines = 8;

        /// <summary>
        /// (legacy code = MAX_DISPO_ID)
        /// </summary>
        public const int MaxActivitiesId = 48;

        public static readonly int MAX_DISPLAY_TEXT = 117; // 116 + NUL

        public static readonly int MAX_ADD_DISPLAY_TEXT = 32;

        /// <summary>
        /// max. file name size (legacy code VFM_MAX_FNAME_SIZE)
        /// </summary>
        public const byte VfmMaxFnameSize = 12;

        /// <summary>
        /// max. file name size (legacy code FSD_MAX_FNAME_SIZE)
        /// </summary>
        public const byte FsdMaxFNameSize = VfmMaxFnameSize;

        /// <summary>
        ///  (legacy code QFTP_MAX_DATA)
        /// </summary>
        public const byte QftpMaxData = 128;

        /// <summary>
        /// TFTP max. file name size (legacy code TFTP_MAX_FPART_SIZE)
        /// </summary>
        public const byte TftpMaxFPartSize = 3;

        /// <summary>
        /// Maximum size of the FileName for tftp mail message (legacy code TFTP_MAX_FNAME_SIZE)
        /// len = 15 bytes
        /// </summary>
        public const byte TftpMaxFNameSize = FsdMaxFNameSize + TftpMaxFPartSize;

        /// <summary>
        /// (legacy code QMAIL_MAX_FNAME_SIZE)
        /// </summary>
        public const byte QmailMaxFNameSize = TftpMaxFNameSize;

        /// <summary>
        /// current protocol version number
        /// </summary>
        public static readonly Byte QNTP_VERSION = 0x1;

        /// <summary>
        /// request from client to server
        /// </summary>
        public static readonly Byte QNTP_MODE_REQUEST = 0x1;

        /// <summary>
        /// response from server to client
        /// </summary>
        public static readonly Byte QNTP_MODE_RESPONSE = 0x2;

        /// <summary>
        /// broadcast from server
        /// </summary>
        public static readonly Byte QNTP_MODE_BROADCAST = 0x3;

        public static readonly Byte QNTP_LEVEL_NONE = 0x0;

        /// <summary>
        /// primary time reference (e.g. ibis)
        /// </summary>
        public static readonly Byte QNTP_LEVEL_PRIMARY = 0x1;

        /// <summary>
        /// secondary time reference (via QNTP)
        /// </summary>
        public static readonly Byte QNTP_LEVEL_SECONDARY = 0x2;

        public static readonly Byte QNTP_IDENT_NONE = 0x0;

        /// <summary>
        /// generic radio service (DCF77)
        /// </summary>
        public static readonly Byte QNTP_IDENT_RADIO = 0x1;

        /// <summary>
        /// generic satellite service (GPS)
        /// </summary>
        public static readonly Byte QNTP_IDENT_SAT = 0x2;

        /// <summary>
        /// time sync. via IBIS (VCU)
        /// </summary>
        public static readonly Byte QNTP_IDENT_IBIS = 0x3;

        /// <summary>
        /// time sync. via Trip-Rsp.Message (VCU)
        /// </summary>
        public static readonly Byte QNTP_IDENT_TRIP_RSP = 0x4;

        /// <summary>
        /// time sync. via this message's timestamp
        /// </summary>
        public static readonly Byte QNTP_IDENT_THIS = 0x10;

        /// <summary>
        /// time sync. via other time server
        /// </summary>
        public static readonly Byte QNTP_IDENT_OTHER = 0x11;

        public static readonly UInt16 QSNMP_HEADER_LEN = (UInt16)Marshal.SizeOf(new tQSNMPHdr());

        public static readonly UInt16 QSNMP_TIMESYNC_LEN = (UInt16)Marshal.SizeOf(new tQSNMPtimeSync());

        /// <summary>
        /// Length in bytes of the qnet message header
        /// </summary>
        public static readonly byte QnetMessageHeaderLength = (byte)Marshal.SizeOf(new QnetMsgHdr());

        /// <summary>
        /// Length in bytes of the bus message
        /// </summary>
        public static readonly byte BusMessageLength =
            (byte)(Marshal.SizeOf(new BusMessageStruct()) + QnetMessageHeaderLength);

        /// <summary>
        /// Length in bytes of the Connection status message
        /// </summary>
        public static readonly byte ConStaMessageLength =
            (byte)(Marshal.SizeOf(new ConStaStruct()) + QnetMessageHeaderLength);

        /// <summary>
        /// Length in bytes of the vdv message
        /// </summary>
        public static readonly byte VdvMessageLength =
            (byte)(Marshal.SizeOf(new VdvMsgStruct()) + QnetMessageHeaderLength);

        /// <summary>
        /// Length in bytes of the iqube command message 
        /// </summary>
        public static readonly byte IqubeCommandMessageLength =
            (byte)(Marshal.SizeOf(new IqubeCmdMsgStruct()) + QnetMessageHeaderLength);

        /// <summary>
        /// Length in bytes of the TftpAck message
        /// </summary>
        public static readonly byte TftpAckLength = (byte)Marshal.SizeOf(new TftpAckStruct());

        /// <summary>
        /// Length in bytes of the TftpMail message
        /// </summary>
        public static readonly byte TftpQmailLength = (byte)Marshal.SizeOf(new TftpMailStruct());

        /// <summary>
        /// Length in bytes of the RealtimeMonitoring message
        /// </summary>
        public static readonly byte RealtimeMonitoringMessageLenght = 
            (byte)(Marshal.SizeOf(new RealtimeMonitoringStruct()) + QnetMessageHeaderLength);

        /// <summary>
        /// Length in bytes of the iqube event data
        /// </summary>
        public const byte EventDataLength = 16;

        /// <summary>
        /// TFTP max. data message buffer size for tftp message
        /// </summary>
        public const byte TftpMaxDataLength = 209;

        /// <summary>
        /// Maximum number of chars contained into voice text (160 +1). The last one must be filled with 0.
        /// </summary>
        public const byte MaxVoiceTextLength = 161;

        /// <summary>
        /// Maximum number of chars contained into info line text (160 +1). The last one must be filled with 0.
        /// </summary>
        public const byte MaxInfoLineTextLenght = 161;

        /// <summary>
        /// Mx. number of rows in RTMon data message for led countdown display (type C) (legacy code : RTMON_MAX_ROWS_TYP_C)
        /// </summary>
        public const byte MaxDisplayedRowsTypeC = 8;

        /// <summary>
        /// max. number of rows in RTMon data message for led matrix display (type M) 
        /// - without Lane (legacy code : RTMON_MAX_ROWS_TYP_M)
        /// </summary>
        public const byte MaxDisplayedRowsTypeM = 4;

        /// <summary>
        /// max. number of rows in RTMon data message for led matrix display (type L) - with Lane information.
        /// ( Legacy code : RTMON_MAX_ROWS_TYP_L)
        /// </summary>
        public const byte MaxDisplayedRowsTypeL = 3;

        /// <summary>
        /// Lenght in bytes of DmcDisplayData Structure
        /// </summary>
        public static readonly byte DmcDisplayDataLength = (byte)Marshal.SizeOf(new DmcDisplayDataStruct());

        /// <summary>
        /// Maximum number of chars contained into led matrix display for line (type M and L). 
        /// </summary>
        public const byte MaxLedMatrixDisplayLineTextLenght = 8;

        /// <summary>
        /// Maximum number of chars contained into led matrix display for destination. (type M and L)
        /// </summary>
        public const byte MaxLedMatrixDisplayDestinationTextLenght = 32;

        /// <summary>
        /// Maximum number of chars contained into led matrix display for time. (type M and L)
        /// </summary>
        public const byte MaxLedMatrixDisplayTimeTextLenght = 8;

        /// <summary>
        /// Maximum number of chars contained into led matrix display for lane. (type M and L)
        /// </summary>
        public const byte MaxLedMatrixDisplayLaneTextLenght = 8;
    }

}
