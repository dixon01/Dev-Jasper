// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetFixedPort.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Fixed port numbers definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Fixed port numbers definition
    /// </summary>
    public class QnetFixedPort
    {
        public const byte QNET_PORT_ANY = 0x00; // any port

        public const byte QNET_PORT_MAIN = 0x01;

        public const byte QNET_PORT_EVENT = 0x01;

        public const byte QNET_PORT_TIMER = 0x02;

        public const byte QNET_PORT_QNP = 0x03;

        public const byte QNET_PORT_QTP_INPUT = 0x04;

        public const byte QNET_PORT_QTP_OUTPUT = 0x05;

        public const byte QNET_PORT_QTP_TIMER = 0x06;

        public const byte QNET_PORT_QSNMP_SERVER = 0x07;

        public const byte QNET_PORT_GSMCMD = 0x0A;

        public const byte QNET_PORT_LSA_RX = 0x0B;

        public const byte QNET_PORT_QDPECHO = 0x10; // QDP echo server port

        public const byte QNET_PORT_QTPECHO = 0x11; // QTP echo server port

        public const byte QNET_PORT_QFTP = 0x14; // QFTP server port

        public const byte QNET_PORT_QCON = 0x15; // QCON server port

        public const byte QNET_PORT_TFTP_SERVER = 0x16; // TFTP server port

        public const byte QNET_PORT_QMAIL = 0x17; // QMAIL server port

        public const byte QNET_PORT_TFTP_CLIENT = 0x18; // TFTP client port

        public const byte QNET_PORT_TEST = 0x20; // test application port

        /// <summary>
        /// Iqube application ports (=> decimal 33)
        /// </summary>
        public const byte QnetPortDispo = 0x21;

        public const byte QNET_PORT_DISPMSG = 0x22;

        public const byte QNET_PORT_VCUAPPL = 0x23;

        public const byte QNET_PORT_IBIS = 0x24;

        /// <summary>
        /// Qnet port for alarms (events, => decimal 37)
        /// </summary>
        public const byte QnetPortAlarm = 0x25;

        public const byte QNET_PORT_DISPCTRL = 0x26;

        public const byte QNET_PORT_DISPTAB = 0x27;

        public const byte QNET_PORT_DCM = 0x28;

        public const byte QNET_PORT_SLCS = 0x2f; // Speise- & Ladegeraet Controller(-Task)

        public const byte QNET_PORT_MAX_RESERVED = 0x30; // max. reserved port number

        //public static byte QNET_PORT_MAX_RESERVED  0x7F    // max. reserved port number
        public const byte QNET_PORT_MIN_AVAIL = 0x80; // min. free available port number

        public const byte QNET_PORT_MAX = 0xFE; // max. port number

        public const byte QNET_PORT_NONE = 0xFF; // not a valid port! 
    }
}
