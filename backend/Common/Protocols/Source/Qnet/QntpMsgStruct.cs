// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QntpMsgStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QntpMsgStruct type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines the QNTPMsg structure used in the qnet protocol
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct QntpMsgStruct
    {
        /// <summary>
        /// time protocol version
        /// </summary>
        public byte Version;

        /// <summary>
        /// time protocol mode
        /// </summary>
        public byte Mode;

        /// <summary>
        /// primary / secondary time reference
        /// </summary>
        public byte ReferenceLevel;

        /// <summary>
        /// particular source of time reference
        /// </summary>
        public byte ReferenceIdent;

        /// <summary>
        /// optional time server address
        /// </summary>
        public ushort TimeServer;

        /// <summary>
        /// time request sent by client
        /// </summary>
        public TimeStruct OriginateTime;

        /// <summary>
        /// time request received by server
        /// </summary>
        public TimeStruct ReceiveTime;

        /// <summary>
        /// time message sent by server
        /// </summary>
        public TimeStruct TransmitTime;

        /// <summary>
        /// date message sent by server
        /// </summary>
        public DateStruct TransmitDate;
    }
}
