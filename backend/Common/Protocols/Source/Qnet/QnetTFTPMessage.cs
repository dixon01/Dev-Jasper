// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetTFTPMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines QnetSNMPMessage class. Inherites from QnetMessageBase.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Text;

    using Gorba.Common.Protocols.Core;

    using NLog;

    /// <summary>
    /// Defines QnetSNMPMessage class. Inherits from QnetMessageBase.
    /// </summary>
    public class QnetTFTPMessage : QnetMessageBase
    {
        /// <summary>
        /// TFTP underlying structure
        /// </summary>
        private TftpStruct tftp;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetTFTPMessage"/> class.
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
        public QnetTFTPMessage(ushort sourceAddress, ushort destAddress, ushort gatewayAddress)
            : base(sourceAddress, destAddress, gatewayAddress)
        {
            this.tftp = new TftpStruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetTFTPMessage"/> class.
        /// </summary>
        public QnetTFTPMessage()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Gets access to the TFTP structure.
        /// </summary>
        public TftpStruct Tftp
        {
            get
            {
                return this.tftp;
            }
        }

        /// <summary>
        /// Convert a datagram (array of bytes) into a QnetTFTPMessage object.
        /// </summary>
        /// <param name="sourceAddr">
        /// The source address.
        /// </param>
        /// <param name="destAddr">
        /// The destination address.
        /// </param>
        /// <param name="data">
        /// Data that represents a TFTP datagram.
        /// </param>
        /// <param name="gatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns a QnetTFTPMessage object from the datagram. Returns null in case of a conversion error.
        /// Actually, just converts <see cref="QnetEventMessage"/> message.
        /// </returns>
        public static QnetMessageBase ConvertDataToQnetMessage(
            ushort sourceAddr, ushort destAddr, byte[] data, ushort gatewayAddress)
        {
            try
            {
                var tftp = ProtocolPacket.ByteArrayToStruct<TftpStruct>(data);
                var operationCode = (TftpOperationCode)tftp.OperationCode;
                var destinationQnetPort = tftp.TftpMail.DstPort;

                var datalen = tftp.TftpMail.DtaLen;
                if (datalen <= 0)
                {
                    return null;
                }

                if (datalen >= MessageConstantes.TftpMaxDataLength)
                {
                    throw new Exception("The length of the data contained into the mail is too long.");
                }

                var messageData = new byte[tftp.TftpMail.DtaLen];
                unsafe
                {
                    for (int i = 0; i < tftp.TftpMail.DtaLen; i++)
                    {
                        messageData[i] = tftp.TftpMail.Data[i];
                    }
                }

                switch (operationCode)
                {
                    case TftpOperationCode.MailRequest:
                        switch (destinationQnetPort)
                        {
                            case QnetFixedPort.QnetPortAlarm:
                                var fileName = new byte[MessageConstantes.TftpMaxFNameSize];
                                unsafe
                                {
                                    for (int i = 0; i < MessageConstantes.TftpMaxFNameSize; i++)
                                    {
                                        fileName[i] = tftp.TftpMail.NameStr[i];
                                    }
                                }

                                return QnetMessageEvent(messageData, sourceAddr, destAddr, fileName, gatewayAddress);

                            case QnetFixedPort.QnetPortDispo:
                                return QnetMessageActivityIds(messageData, sourceAddr, destAddr, gatewayAddress);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger()
                          .Error("QnetTFTPMessage::ConvertDataToQnetMessage exception occured: {0}", ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Create a qnet TFTP message to acknowledge a request
        /// </summary>
        /// <param name="blockNumber">
        /// Some requests need to acknowledge a specific block number or a specific acknowledge number.
        /// </param>
        public void SetAckMessage(ushort blockNumber)
        {
            this.tftp.TftpAck.BlockNumber = blockNumber;
            this.tftp.OperationCode = (byte)TftpOperationCode.Acknowledge;
        }

        /// <summary>
        /// Pack the specified qnet message into the QMAIL message.
        /// Copy the qnet datagram into the QMAIL data.
        /// </summary>
        /// <param name="sourceAddr">
        /// The source address.
        /// </param>
        /// <param name="destAddr">
        /// The destination address.
        /// </param>
        /// <param name="qnetMessage">
        /// The qnet message.
        /// </param>
        /// <param name="qmailDataLen">
        /// Length in byte of the data contains into the QMAIL data.
        /// </param>
        /// <param name="mailName">
        /// Unique QMAIL name.
        /// </param>
        public void SetQnetMessage(
            ushort sourceAddr, ushort destAddr, QnetMessageStruct qnetMessage, int qmailDataLen, string mailName)
        {
            this.tftp.OperationCode = (byte)TftpOperationCode.MailRequest;

            this.tftp.TftpMail.DstPort = QnetFixedPort.QNET_PORT_QMAIL;

            var buffer = Encoding.ASCII.GetBytes(mailName);
            var len = Math.Min(mailName.Length, MessageConstantes.QmailMaxFNameSize);
            unsafe
            {
                fixed (byte* ptr = this.tftp.TftpMail.NameStr)
                {
                    byte* ps = ptr;
                    for (int i = 0; i < len; i++)
                    {
                        *ps = buffer[i];
                        ps++;
                    }
                }
            }

            this.tftp.TftpMail.SrcAddr = sourceAddr;
            this.tftp.TftpMail.DstAddr = destAddr;

            var qmailData = ProtocolPacket.StructureToByteArray(qnetMessage);

            if (qmailDataLen >= MessageConstantes.TftpMaxDataLength)
            {
                throw new Exception("The length of the data contained into the mail is too long.");
            }

            this.tftp.TftpMail.DtaLen = (ushort)qmailDataLen;

            unsafe
            {
                fixed (byte* ptr = this.tftp.TftpMail.Data)
                {
                    byte* ps = ptr;
                    for (int i = 0; i < qmailDataLen; i++)
                    {
                        *ps = qmailData[i];
                        ps++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the length of the data of the message.
        /// </summary>
        /// <returns>
        /// Returns the length in bytes of the data of the message according to the <see cref="TftpOperationCode"/>.
        /// </returns>
        protected override byte GetDataLenght()
        {
            // minumum = 1 corresponding to the OperationCode
            byte len = 1;
            switch ((TftpOperationCode)this.tftp.OperationCode)
            {
                case TftpOperationCode.Acknowledge:
                    len += MessageConstantes.TftpAckLength;
                    break;
                case TftpOperationCode.MailRequest:
                    len += MessageConstantes.TftpQmailLength;
                    break;
            }

            return len;
        }

        private static unsafe QnetMessageBase QnetMessageActivityIds(
            byte[] messageData, ushort sourceAddr, ushort destAddr, ushort gatewayAddress)
        {
            var message = ProtocolPacket.ByteArrayToStruct<QnetMessageStruct>(messageData);
            switch ((IqubeCommandCode)message.Dta.IqubeCmdMsg.CommandCode)
            {
                case IqubeCommandCode.GetActivityIdsCommand:
                    var mgs = new ActivityIdsMessage(sourceAddr, destAddr, gatewayAddress);
                    int activitiesCount = message.Dta.IqubeCmdMsg.ActivityIds.Count;
                    for (int i = 0; i < activitiesCount; i++)
                    {
                        mgs.ActivityIdsList.Add(message.Dta.IqubeCmdMsg.ActivityIds.Ids[i]);
                    }

                    break;
            }

            return null;
        }

        private static QnetMessageBase QnetMessageEvent(
            byte[] messageData, ushort sourceAddr, ushort destAddr, byte[] fileName, ushort gatewayAddress)
        {
            var message = ProtocolPacket.ByteArrayToStruct<QnetMessageStruct>(messageData);
            var qnetMessageEvent = new QnetEventMessage(sourceAddr, destAddr, gatewayAddress)
                {
                    EventCode = (EventCode)message.Dta.Event.EventId,
                    AlarmClass = (AlarmClass)message.Dta.Event.AlarmClass,
                    Attribute = message.Dta.Event.Attribute,
                    EventStamp = DosDateTime.DosDateTimeToDateTime(message.Dta.Event.Time),
                    Param = message.Dta.Event.Param,
                    Param1 = message.Dta.Event.Param1,
                    Param2 = message.Dta.Event.Param2,
                    Param3 = message.Dta.Event.Param3
                };

            qnetMessageEvent.FileName = Encoding.ASCII.GetString(fileName);

            return qnetMessageEvent;
        }
    }
}