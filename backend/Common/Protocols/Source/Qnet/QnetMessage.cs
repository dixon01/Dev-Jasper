// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Class that enables mainly to tranlate the received qnet datagram to qnet message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    using Gorba.Common.Protocols.Core;
    using Gorba.Common.Protocols.Qnet.Structures;

    /// <summary>
    /// Class that enables mainly to translate the received qnet datagram to qnet message.
    /// </summary>
    public class QnetMessage : QnetMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QnetMessage"/> class.
        /// </summary>
        /// <param name="sourceAddress">
        /// The qnet source address.
        /// </param>
        /// <param name="destAddress">
        /// The qnet destination address.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public QnetMessage(ushort sourceAddress, ushort destAddress, ushort gatewayAddress)
            : base(sourceAddress, destAddress, gatewayAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetMessage"/> class.
        /// </summary>
        public QnetMessage()
            : this(QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone)
        {
        }

        /// <summary>
        /// Convert a datagram (array of bytes) into a derived type of QnetMessageBase object.
        /// </summary>
        /// <param name="sourceAddress">
        /// The source Address.
        /// </param>
        /// <param name="destAddress">
        /// The destination address.
        /// </param>
        /// <param name="data">
        /// Data that represents a qnet datagram as array of bytes.
        /// </param>
        /// <param name="gatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns a QnetMessageBase object from the received datagram (array of bytes).
        /// Returns null in case of a conversion error.
        /// </returns>
        public static QnetMessageBase ConvertDataToQnetMessage(
            ushort sourceAddress, ushort destAddress, byte[] data, ushort gatewayAddress)
        {
            QnetMessageBase qnetMessage = null;
            try
            {
                var tmpBuffer = new byte[MessageConstantes.QnetMessageHeaderLength];

                Array.Copy(data, 0, tmpBuffer, 0, MessageConstantes.QnetMessageHeaderLength);

                var messageHeader = ProtocolPacket.ByteArrayToStruct<QnetMsgHdr>(tmpBuffer);
                var msgType = (MSGtyp)messageHeader.Type;
                switch (msgType)
                {
                        // VDV MESSAGE
                    case MSGtyp.MsgTypVdv:
                        switch ((VdvSubType)messageHeader.SubTyp)
                        {
                            case VdvSubType.VdvSubtypScheduledTimetableDataRequest:
                                qnetMessage = new QnetScheduledTimetableRequestMessage(
                                    sourceAddress, destAddress, gatewayAddress);
                                break;

                            case VdvSubType.VdvSubtypRefTextRequest:
                                qnetMessage =
                                    new QnetRefDataRequestMessage(sourceAddress, destAddress, gatewayAddress);
                                break;
                        }

                        break;
                }
            }
            catch
            {
                qnetMessage = null;
            }

            return qnetMessage;
        }
    }
}