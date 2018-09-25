// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetSNMPMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines QnetSNMPMessage class. Inherits from QnetMessageBase.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    using Gorba.Common.Protocols.Core;

    /// <summary>
    /// Defines QnetSNMPMessage class. Inherits from QnetMessageBase.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public class QnetSNMPMessage : QnetMessageBase
    // ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// QSNMP underlying structure
        /// </summary>
        private QsnmpStruct qsnmp;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetSNMPMessage"/> class.
        /// </summary>
        /// <param name="sourceAddress">
        /// The source address.
        /// </param>
        /// <param name="destAddress">
        /// The destination address.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public QnetSNMPMessage(ushort sourceAddress, ushort destAddress, ushort gatewayAddress)
            : base(sourceAddress, destAddress, gatewayAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetSNMPMessage"/> class.
        /// </summary>
        public QnetSNMPMessage()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Gets QSNMP.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public QsnmpStruct QSNMP
        // ReSharper restore InconsistentNaming
        {
            get { return this.qsnmp; }
        }

        /// <summary>
        /// Convert a datagram (array of bytes) into a QnetSNMPMessage object.
        /// </summary>
        /// <param name="sourceAddr">
        /// The source address.
        /// </param>
        /// <param name="destAddr">
        /// The destination address.
        /// </param>
        /// <param name="data">
        /// Data that represents a QSNMP datagram
        /// </param>
        /// <param name="gatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns a QnetSNMPMessage object from the datagram. Returns null in case of a conversion error.
        /// Actually, just converts <see cref="QSNMPCmd.TimeSynchronization"/> command.
        /// </returns>
        public static QnetMessageBase ConvertDataToQnetMessage(
            ushort sourceAddr, ushort destAddr, byte[] data, ushort gatewayAddress)
        {
            QnetMessageBase qnetMessage = null;
            try
            {
                var sntp = ProtocolPacket.ByteArrayToStruct<QsnmpStruct>(data);
                var cmd = (QSNMPCmd)sntp.hdr.cmd;
                switch (cmd)
                {
                    case QSNMPCmd.TimeSynchronization:
                        if (sntp.msg.timeSync.qntp.Version == MessageConstantes.QNTP_VERSION
                            && sntp.msg.timeSync.qntp.Mode == MessageConstantes.QNTP_MODE_REQUEST)
                        {
                            var originateTime = new DateTime(
                                1,
                                1,
                                1,
                                sntp.msg.timeSync.qntp.OriginateTime.Hour,
                                sntp.msg.timeSync.qntp.OriginateTime.Minute,
                                sntp.msg.timeSync.qntp.OriginateTime.Second);
                            qnetMessage = new QnetTimeSyncRequestMessage(
                                sourceAddr, destAddr, originateTime, gatewayAddress);
                        } // if

                        break;
                } // switch
            }
            catch
            {
                qnetMessage = null;
            } // try

            return qnetMessage;
        }

        /// <summary>
        /// Fill the structure for restart command using the QSNMP protocol
        /// </summary>
        public void SetRestartCommand()
        {
            this.SetQSNMPCommand(QSNMPCmd.Restart);
        }

        /// <summary>
        /// Set the fields for the disposition on/off  message and sets the command to
        /// <see cref="QSNMPCmd.DisplaySwitchOn"/> or <see cref="QSNMPCmd.DisplaySwitchOff"/>.
        /// </summary>
        /// <param name="dispOn">
        /// The disposition on.
        /// </param>
        public void SetDisplayCommand(bool dispOn)
        {
            this.SetQSNMPCommand(dispOn ? QSNMPCmd.DisplaySwitchOn : QSNMPCmd.DisplaySwitchOff);
        }

        /*
        public void TimeSync()
        {
            m_tQSNMP.msg.timeSync.qntp.Mode = MessageConstantes.QNTP_MODE_BROADCAST;
            m_tQSNMP.msg.timeSync.qntp.ReferenceLevel = MessageConstantes.QNTP_LEVEL_SECONDARY;
            m_tQSNMP.msg.timeSync.qntp.ReferenceIdent = MessageConstantes.QNTP_IDENT_THIS;
            m_tQSNMP.msg.timeSync.qntp.TimeServer = QnetConstantes.QNET_ADDR_NONE;

            SetQSNMPCommand(QSNMPCmd.QSNMP_CMD_TIME_SYNC);
        }
                   * */

        /// <summary>
        /// Send a time synchronization to the unit.
        /// </summary>
        /// <param name="syncTime">
        /// DateTime to be synchronized in the unit.
        /// </param>
        public void NotifyTimeSync(DateTime syncTime)
        {
            this.qsnmp.msg.timeSync.qntp.Mode = MessageConstantes.QNTP_MODE_BROADCAST;
            this.qsnmp.msg.timeSync.qntp.ReferenceLevel = MessageConstantes.QNTP_LEVEL_SECONDARY;
            this.qsnmp.msg.timeSync.qntp.ReferenceIdent = MessageConstantes.QNTP_IDENT_THIS;

            this.qsnmp.msg.timeSync.qntp.OriginateTime.Hour = 0;
            this.qsnmp.msg.timeSync.qntp.OriginateTime.Minute = 0;
            this.qsnmp.msg.timeSync.qntp.OriginateTime.Second = 0;

            this.qsnmp.msg.timeSync.qntp.ReceiveTime.Hour = 0;
            this.qsnmp.msg.timeSync.qntp.ReceiveTime.Minute = 0;
            this.qsnmp.msg.timeSync.qntp.ReceiveTime.Second = 0;

            this.qsnmp.msg.timeSync.qntp.TransmitTime.Hour = (short)syncTime.Hour;
            this.qsnmp.msg.timeSync.qntp.TransmitTime.Minute = (short)syncTime.Minute;
            this.qsnmp.msg.timeSync.qntp.TransmitTime.Second = (short)syncTime.Second;

            this.qsnmp.msg.timeSync.qntp.TransmitDate.Day = (short)syncTime.Day;
            this.qsnmp.msg.timeSync.qntp.TransmitDate.Month = (short)syncTime.Month;
            this.qsnmp.msg.timeSync.qntp.TransmitDate.Year = (short)syncTime.Year;

            this.SetQSNMPCommand(QSNMPCmd.TimeSynchronization);
        }

        /// <summary>
        /// Set the fields for time synchronization response message and sets the command to
        /// <see cref="QSNMPCmd.TimeSynchronization"/>.
        /// </summary>
        /// <param name="receiveTime">
        /// DateTime that corresponds to the date when the message is received.
        /// </param>
        /// <param name="originalTime">
        /// DateTime of the original time sync request message. Set by the requester.
        /// </param>
        public void RespondTimeSync(DateTime receiveTime, DateTime originalTime)
        {
            DateTime syncTime = DateTime.Now;

            this.qsnmp.msg.timeSync.qntp.Mode = MessageConstantes.QNTP_MODE_RESPONSE;
            this.qsnmp.msg.timeSync.qntp.ReferenceLevel = MessageConstantes.QNTP_LEVEL_SECONDARY;
            this.qsnmp.msg.timeSync.qntp.ReferenceIdent = MessageConstantes.QNTP_IDENT_THIS;

            this.qsnmp.msg.timeSync.qntp.OriginateTime.Hour = (short)originalTime.Hour;
            this.qsnmp.msg.timeSync.qntp.OriginateTime.Minute = (short)originalTime.Minute;
            this.qsnmp.msg.timeSync.qntp.OriginateTime.Second = (short)originalTime.Second;

            this.qsnmp.msg.timeSync.qntp.ReceiveTime.Hour = (short)receiveTime.Hour;
            this.qsnmp.msg.timeSync.qntp.ReceiveTime.Minute = (short)receiveTime.Minute;
            this.qsnmp.msg.timeSync.qntp.ReceiveTime.Second = (short)receiveTime.Second;

            this.qsnmp.msg.timeSync.qntp.TransmitTime.Hour = (short)syncTime.Hour;
            this.qsnmp.msg.timeSync.qntp.TransmitTime.Minute = (short)syncTime.Minute;
            this.qsnmp.msg.timeSync.qntp.TransmitTime.Second = (short)syncTime.Second;

            this.qsnmp.msg.timeSync.qntp.TransmitDate.Day = (short)syncTime.Day;
            this.qsnmp.msg.timeSync.qntp.TransmitDate.Month = (short)syncTime.Month;
            this.qsnmp.msg.timeSync.qntp.TransmitDate.Year = (short)syncTime.Year;

            this.SetQSNMPCommand(QSNMPCmd.TimeSynchronization);
        }

        /// <summary>
        /// Gets the length of the data of the message.
        /// </summary>
        /// <returns>
        /// Returns the length in bytes of the data of the message that is stored in the header message length field.
        /// </returns>
        protected override byte GetDataLenght()
        {
            return (byte)this.qsnmp.hdr.msglen;
        }

        /// <summary>
        /// Fill qsnmp header for the given command
        /// </summary>
        /// <param name="cmd">
        /// QSNMP command to set in the header of the message.
        /// Others fields are also set according to the command.
        /// </param>
        // ReSharper disable InconsistentNaming
        private void SetQSNMPCommand(QSNMPCmd cmd)
        // ReSharper restore InconsistentNaming
        {
            this.qsnmp.hdr.cmd = (short)cmd;
            this.qsnmp.hdr.typ = 0;
            this.qsnmp.hdr.par = 0;

            switch (cmd)
            {
                case QSNMPCmd.Restart:
                case QSNMPCmd.DisplaySwitchOn:
                case QSNMPCmd.DisplaySwitchOff:
                case QSNMPCmd.QSNMP_CMD_SYSTEM_DATA:
                    this.qsnmp.hdr.msglen = MessageConstantes.QSNMP_HEADER_LEN;
                    break;
                case QSNMPCmd.TimeSynchronization:
                    this.qsnmp.hdr.msglen =
                        (ushort)(MessageConstantes.QSNMP_HEADER_LEN + MessageConstantes.QSNMP_TIMESYNC_LEN);
                    this.qsnmp.msg.timeSync.qntp.Version = MessageConstantes.QNTP_VERSION;
                    break;
            } // switch
        }
    }
}