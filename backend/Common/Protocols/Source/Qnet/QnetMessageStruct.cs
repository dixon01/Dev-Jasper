// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetMessageStruct.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Structure to handle all Qnet messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    using Gorba.Common.Protocols.Qnet.Structures;

    /// <summary>
    /// Structure to handle all Qnet messages.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct QnetMessageStruct
    {
        /// <summary>
        /// Header of the qnet message.
        /// Len : 6 bytes
        /// </summary>
        public QnetMsgHdr Hdr;

        /// <summary>
        /// Data of the qnet message.
        /// Len : Depends of the underlying type of message contains into the data
        /// </summary>
        public QnetMsgData Dta;
    }

    /// <summary>
    /// Header of the qnet message. It enables to know which kind of data are contained into the qnet datagram.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct QnetMsgHdr
    {
        /// <summary>
        /// Type of the message contained into the data of the qnet datagram.
        /// </summary>
        public byte Type;

        /// <summary>
        /// Subtype of the message contained into the data of the qnet datagram. The subtype is linked to the type.
        /// </summary>
        public byte SubTyp;

        /// <summary>
        /// Time in dos format. <see cref="DOSTimeStruct"/>
        /// </summary>
        public DOSTimeStruct TimeStruct;
    }

    /// <summary>
    /// Contains the data of the qnet message sent through the qnet protocol.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct QnetMsgData
    {
        /// <summary>
        /// Represent the bus message data
        /// </summary>
        [FieldOffset(0)]
        public BusMessageStruct BusMsg;

        /// <summary>
        /// Represents the connection status data.
        /// </summary>
        [FieldOffset(0)]
        public ConStaStruct ConStaStruct;

        /// <summary>
        /// Represents the structure that enables to send a vdv message to a iqube through the qnet protocol.
        /// </summary>
        [FieldOffset(0)]
        public VdvMsgStruct VdvMsg;

        /// <summary>
        /// Represents the structure to send a command (like disposition or task) to an iqube with qnet protocol.
        /// </summary>
        [FieldOffset(0)]
        public IqubeCmdMsgStruct IqubeCmdMsg;

        /// <summary>
        /// For remote message (DirectRemoteControl).
        /// </summary>
        [FieldOffset(0)]
        public RemoteBusStruct RemoteBus;

        /// <summary>
        /// For event message, used also to handle alarms.
        /// </summary>
        [FieldOffset(0)]
        public QnetEventStruct Event;

        /// <summary>
        /// Realtime Monitoring message (Legacy code = tRTMonMsg).
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [FieldOffset(0)]
        public RealtimeMonitoringStruct RealtimeMonitoring;
    }
}
