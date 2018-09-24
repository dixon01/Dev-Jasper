// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetProtocolStack.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Protocol stack to handle QNET protocol.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Diagnostics.Contracts;

    using Gorba.Common.Protocols.Core;

    using NLog;

    /// <summary>
    /// Protocol stack to handle QNET protocol.
    /// </summary>
    public class QnetProtocolStack : ProtocolStack
    {
        #region Private fields

        private readonly Logger globalLogger;

        /// <summary>
        /// Fixed qnet address of the bus message.
        /// </summary>
        private readonly QnetAddress busMsgSourceAddress;

        /// <summary>
        /// Fixed qnet address of the gateway for the bus message.
        /// </summary>
        private readonly QnetAddress busMsgGatewayAddress;

        /// <summary>
        /// Represents the internal QNET layer. See <see cref="QNETLayer"/>.
        /// See also <see cref="ProtocolLayer"/> for information about protocol layers.
        /// </summary>
        private readonly QNETLayer internalQnetLayer;

        /// <summary>
        /// Represents the internal QDP layer. See <see cref="QDPLayer"/>
        /// See also <see cref="ProtocolLayer"/> for information about protocol layers.
        /// </summary>
        private readonly QDPLayer internalQdpLayer;

        /// <summary>
        /// Represents the internal QNP layer. See <see cref="QNPLayer"/>
        /// See also <see cref="ProtocolLayer"/> for information about protocol layers.
        /// </summary>
        private readonly QNPLayer internalQnpLayer;

        /// <summary>
        /// Lower level to decode and encode the qnet datagram. See <see cref="Qna"/>.
        /// </summary>
        private readonly Qna qna;

        /// <summary>
        /// Stores the qnet datagram as a byte array.
        /// </summary>
        private byte[] qnetDatagram;

        #endregion Private fields

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetProtocolStack"/> class.
        /// </summary>
        public QnetProtocolStack()
        {
            this.globalLogger = LogManager.GetLogger("GlobalLog");

            // set a default qmail identification provider
            this.QmailIdentifierProvider = new QmailIdentifierProvider();

            this.busMsgSourceAddress = new QnetAddress("A:9.1.1");
            this.busMsgGatewayAddress = new QnetAddress("A:9.1.1");
            this.QnetSrcAddress = new QnetAddress();
            this.QnetDstAddress = new QnetAddress();
            this.qnetDatagram = null;
            this.qna = new Qna();

            // layer 2 : Data link layer
            this.internalQnetLayer = new QNETLayer();

            // Layer 3 : Network layer
            this.internalQnpLayer = new QNPLayer();

            // Layer 4 : Transport layer
            this.internalQdpLayer = new QDPLayer();

            // Add each layer on top in reverse order to build the protocol stack
            this.AddLayer(this.internalQnetLayer);
            this.AddLayer(this.internalQnpLayer);
            this.AddLayer(this.internalQdpLayer);
        }

        /// <summary>
        /// Gets or sets the QMAIL Identifier Provider. It's a dependency injection.
        /// </summary>
        public IQmailIdentifierProvider QmailIdentifierProvider { get; set; }

        /// <summary>
        /// Gets or sets the event occurs OnReceivedQnetMessage.
        /// This event is fired when socket handler receives data from client socket connection.
        /// </summary>
        public EventHandler<ReceivedQnetMessageArgs> ReceivedQnetMessage { get; set; }

        /// <summary>
        /// Gets QnetSrcAddress.
        /// </summary>
        public QnetAddress QnetSrcAddress { get; private set; }

        /// <summary>
        /// Gets the qnet destination address.
        /// </summary>
        public QnetAddress QnetDstAddress { get; private set; }

        /// <summary>
        /// Gets QnetDatagram as a byte array.
        /// </summary>
        public virtual byte[] QnetDatagram
        {
            get
            {
                return this.qnetDatagram;
            }
        }

        /// <summary>
        /// Gets type of the qnet datagram. See <see cref="QnetType"/>
        /// </summary>
        public QnetType QnetType { get; private set; }

        /// <summary>
        /// Returns the len of the datagram if a qnet datagram is decoded
        /// </summary>
        /// <param name="receivedByte">Number of bytes to treat to try to decode a qnet datagram.</param>
        /// <returns>
        /// Returns the length of the decoded qnet datagram
        /// </returns>
        public virtual int HandleReceiveData(byte receivedByte)
        {
            var ret = this.qna.HandleReceiveData(receivedByte);
            if (ret > 0)
            {
                // We have received a qnet datagram
                this.qnetDatagram = new byte[this.qna.DatagramLength];
                if (this.qna.Hdr != null)
                {
                    this.qna.Hdr.CopyTo(this.qnetDatagram, 0);
                }

                if (this.qna.Dta != null)
                {
                    var header = this.qna.Hdr;
                    if (header != null)
                    {
                        this.qna.Dta.CopyTo(this.qnetDatagram, header.Length);
                    }
                }

                var packet = new ProtocolPacket(this.qna.DatagramLength, 0, this.qna.DatagramLength);
                packet.SetBody(this.qnetDatagram);
                this.HandleReceive(ref packet);
                this.QnetSrcAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.srcAddr;
                this.QnetDstAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.dstAddr;
                this.QnetType = this.internalQnetLayer.QnetType;

                this.ConvertQnetDataToQnetMessage();
            }
            else
            {
                if (ret < 0)
                {
                    this.globalLogger.Warn(
                        "HandleReceiveData: Returns qna error {0} - {1}", ret, QnaErrors.ConvertErrorCode(ret));
                }
            }

            return ret;
        }

        /// <summary>
        /// Reinitialize the underlying protocol decoder(QNA layer).
        /// </summary>
        public void Reinit()
        {
            this.qna.QnetInit();
        }

        /// <summary>
        /// Send a position bus message with QDP layer.
        /// </summary>
        /// <param name="context">Contains fields to fill bus message datagram</param>
        /// <returns>Returns an array of bytes that contains qnet datagram with PositionBusMessage data.</returns>
        public byte[] BuildPositionBusMessageDatagram(QnetBusMessageContext context)
        {
            // Set bus message
            var busMessage = new QnetBusMessage();
            busMessage.FillPositionBusMessage(
                (ushort)context.LineId,
                (ushort)context.BlockId,
                (ushort)context.TripId,
                (ushort)context.StopId,
                (ushort)context.BusId,
                (ushort)context.Distance,
                context.VehicleType,
                context.Attribute,
                context.Timestamp);

            this.FillQnetHeaderForDisplayMsg(
                this.busMsgSourceAddress.QNETAddr,
                QnetConstantes.QNET_ADDR_BROADCAST,
                busMessage.DataLength,
                this.busMsgGatewayAddress.QNETAddr);

            // buils the packet to send
            var packet = new ProtocolPacket(
                busMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(busMessage.BusMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Returns the qnet datagram that enables to restart an iqube unit
        /// </summary>
        /// <param name="qnetAddress">
        /// qnet address of the iqube to be rebooted.
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        /// <returns>
        /// Returns a array of byte containing the qnet raw datagram to restart the iqube
        /// </returns>
        public byte[] BuildRestartDatagram(ushort qnetAddress, ushort qnetGatewayAddress)
        {
            var qnetSnmpMessage = new QnetSNMPMessage();

            qnetSnmpMessage.SetRestartCommand();

            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_DISPMSG,
                QnetFixedPort.QNET_PORT_QSNMP_SERVER,
                qnetSnmpMessage.DataLength,
                QnetConstantes.QnetAddrNone,
                qnetAddress,
                qnetGatewayAddress);

            var packet = new ProtocolPacket(
                qnetSnmpMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(qnetSnmpMessage.QSNMP));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Returns a qnet datagram to send a request to an iqube.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet Destination Address.
        /// </param>
        /// <returns>
        /// Returns an array of bytes containing the raw datagram to send a request to an iqube.
        /// </returns>
        public byte[] BuildRequestMessageDatagram(ushort qnetDestAddr)
        {
            // fill IPP header
            this.internalQnetLayer.QnetIppHdr.msg_typ = IIPP_MSGTYPE.IIPP_MSGTYP_REQUEST;

            // fill QNET message parameter
            this.internalQnetLayer.NextHop = qnetDestAddr; // next hop network address

            var packet = new ProtocolPacket(0, QnetConstantes.QNET_HEADER_LEN, QnetConstantes.QNET_HEADER_LEN);

            this.internalQnetLayer.Transmit(ref packet);
            this.qna.PrepareQnetDatagramm(packet.GetHeader(), packet.GetBody());
            this.QnetSrcAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.srcAddr;
            this.QnetDstAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.dstAddr;
            return this.qna.RawDatagramm;
        }

        /// <summary>
        /// Returns a qnet datagram to send a response to an iqube that sent a request.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <returns>
        /// Returns an array of bytes containing the raw datagram to send a response to an iqube.
        /// </returns>
        public byte[] BuildResponseMessageDatagram(ushort qnetDestAddr)
        {
            // fill IPP header
            this.internalQnetLayer.QnetIppHdr.msg_typ = IIPP_MSGTYPE.IIPP_MSGTYP_RESPONSE;

            // fill QNET message parameter
            this.internalQnetLayer.NextHop = qnetDestAddr; // next hop network address

            var packet = new ProtocolPacket(0, QnetConstantes.QNET_HEADER_LEN, QnetConstantes.QNET_HEADER_LEN);

            this.internalQnetLayer.Transmit(ref packet);
            this.qna.PrepareQnetDatagramm(packet.GetHeader(), packet.GetBody());
            this.QnetSrcAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.srcAddr;
            this.QnetDstAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.dstAddr;
            return this.qna.RawDatagramm;
        }

        /// <summary>
        /// Returns the qnet datagram that enables to synchronize date/time on an iqube unit.
        /// </summary>
        /// <param name="qnetAddress">
        /// Qnet address of the iqube that asked to be synchronized.
        /// </param>
        /// <param name="receiveTime">
        /// The receive Time.
        /// </param>
        /// <param name="originalTime">
        /// The original Time.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns a array of byte containing the qnet raw datagram to synchronize the date/time on the iqube.
        /// </returns>
        public byte[] BuildRespondTimeSyncDatagram(
            ushort qnetAddress, DateTime receiveTime, DateTime originalTime, ushort qnetGatewayAddress)
        {
            var msg = new QnetSNMPMessage();

            msg.RespondTimeSync(receiveTime, originalTime);

            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_DISPMSG,
                QnetFixedPort.QNET_PORT_QSNMP_SERVER,
                msg.DataLength,
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetAddress,
                qnetGatewayAddress);
            var packet = new ProtocolPacket(
                msg.DataLength, QDPLayer.BodyOffet, this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(msg.QSNMP));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Returns the qnet datagram that enables to synchronize date/time on an iqube unit with server date/time
        /// </summary>
        /// <param name="qnetAddress">
        /// Qnet address of the iqube that will be synchronized
        /// </param>
        /// <param name="syncTime">
        /// DateTime to be synchronized in the unit.
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        /// <returns>
        /// Returns a array of byte containing the qnet raw datagram to synchronize the date/time on the iqube.
        /// </returns>
        public byte[] BuildTimeSyncDatagram(ushort qnetAddress, DateTime syncTime, ushort qnetGatewayAddress)
        {
            var msg = new QnetSNMPMessage();

            msg.NotifyTimeSync(syncTime);

            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_DISPMSG,
                QnetFixedPort.QNET_PORT_QSNMP_SERVER,
                msg.DataLength,
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetAddress,
                qnetGatewayAddress);
            var packet = new ProtocolPacket(
                msg.DataLength, QDPLayer.BodyOffet, this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(msg.QSNMP));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Builds and returns the final qnet datagram ready to be sent to units.
        /// </summary>
        /// <param name="qnetData">
        /// The qnet data.
        /// </param>
        /// <returns>
        /// Array of bytes containing the final qnet datagram ready to send to units.
        /// </returns>
        public byte[] BuildQnetDatagram(byte[] qnetData)
        {
            var packet = new ProtocolPacket(qnetData.Length, 0, qnetData.Length);
            packet.SetBody(qnetData);

            this.qna.PrepareQnetDatagramm(packet.GetHeader(), packet.GetBody());
            this.QnetSrcAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.srcAddr;
            this.QnetDstAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.dstAddr;
            return this.qna.RawDatagramm;
        }

        /// <summary>
        /// Build the qnet datagram from the context fields for VDV in order to delete a trip.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="context">
        /// The context that contains data to build the datagram.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns an array of bytes containing the qnet datagram for VDV in order to delete a trip.
        /// </returns>
        public byte[] GetDeleteTripAsQnetDatagram(
            ushort qnetDestAddr, QnetVdvMessageContext context, ushort qnetGatewayAddress)
        {
            // Set vdv message
            var vdvMessage = new QnetVdvMessage();
            vdvMessage.SetDeleteTripMessage(
                context.ITCSIdentifier,
                context.LineId,
                context.DestinationId,
                context.TripId,
                context.StopSequenceCounter);

            // simple qnet message

            // Fill the qnet message
            this.FillQnetHeaderForDisplayMsg(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS, qnetDestAddr, vdvMessage.DataLength, qnetGatewayAddress);

            // buils the packet to send
            var packet = new ProtocolPacket(
                vdvMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(vdvMessage.VdvMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Builds the qnet datagram from the context fields for VDV in order to add reference text.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Type : byte array
        /// Returns an array of bytes containing the qnet datagram for VDV in order to add a reference text.
        /// </returns>
        public byte[] BuildReferenceTextMessageDatagram(
            ushort qnetDestAddr, QnetVdvMessageContext context, ushort qnetGatewayAddress)
        {
            // Set bus message
            var vdvMessage = new QnetVdvMessage();
            vdvMessage.SetReferenceTextMessage(
                context.ReferenceTextId,
                context.FontNumber,
                (byte)context.ReferenceType,
                context.DisplayText,
                context.TTSText);

            // Fill the qnet message
            this.FillQnetHeaderForDisplayMsg(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS, qnetDestAddr, vdvMessage.DataLength, qnetGatewayAddress);

            // buils the packet to send
            var packet = new ProtocolPacket(
                vdvMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(vdvMessage.VdvMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Build the VDV qnet datagram from the context for info line message
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Byte array that contains the data of the InfoLine qnet message
        /// </returns>
        public byte[] BuildSpecialLineTextMessageDatagram(
            ushort qnetDestAddr, QnetVdvMessageContext context, ushort qnetGatewayAddress)
        {
            // Set bus message
            var vdvMessage = new QnetVdvMessage();
            vdvMessage.SetSpecialLineText(
                context.ITCSIdentifier,
                context.LineNumber,
                context.DestinationId,
                context.InfoLineExpirationTime,
                context.DisplayText);

            // Fill the qnet message
            this.FillQnetHeaderForDisplayMsg(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS, qnetDestAddr, vdvMessage.DataLength, qnetGatewayAddress);

            // buils the packet to send
            var packet = new ProtocolPacket(
                vdvMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(vdvMessage.VdvMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Builds the VDV qnet datagram from the context for info line deletion message
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Byte array that contains the data of the InfoLine qnet message
        /// </returns>
        public byte[] BuildDeleteInfoLineTextMessageDatagram(
            ushort qnetDestAddr, QnetVdvMessageContext context, ushort qnetGatewayAddress)
        {
            // Set bus message
            var vdvMessage = new QnetVdvMessage();
            vdvMessage.SetSpecialLineTextDeletion(context.ITCSIdentifier, context.LineNumber, context.DestinationId);

            // Fill the qnet message
            this.FillQnetHeaderForDisplayMsg(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS, qnetDestAddr, vdvMessage.DataLength, qnetGatewayAddress);

            // buils the packet to send
            var packet = new ProtocolPacket(
                vdvMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(vdvMessage.VdvMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Builds the info line text qnet datagram from the context for info line text message
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="context">
        /// The <see cref="QnetIqubeCmdMessageContext"/> context to give all needed parameters.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Type : byte[]
        /// Bytes array that contains the data of the qnet iqube command with info line text task inside.
        /// </returns>
        public byte[] BuildInfoLineTextMessageDatagram(
            ushort qnetDestAddr, QnetIqubeCmdMessageContext context, ushort qnetGatewayAddress)
        {
            var qnetMessage = new QnetIqubeActivityMessage();

            qnetMessage.SetInfoLineTextActivity(
                context.TaskId,
                context.RowId,
                context.Text,
                context.Blink,
                context.Align,
                context.Flags,
                context.Font,
                context.Scroll,
                context.Side,
                context.StartDate,
                context.StopDate,
                context.IsScheduledDaily);

            // Fill the qnet message
            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QnetPortDispo,
                QnetFixedPort.QnetPortDispo,
                qnetMessage.DataLength,
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetDestAddr,
                qnetGatewayAddress);

            // buils the packet to send
            var packet = new ProtocolPacket(
                qnetMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(qnetMessage.IqubeActivityMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Builds the revoke qnet datagram with the task id.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="taskId">
        /// The id of the qnet task to revoke.
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        /// <returns>
        /// Type : byte[]
        /// Bytes array that contains the data of the qnet iqube command with revoke task inside.
        /// </returns>
        public byte[] BuildRevokeMessageDatagram(ushort qnetDestAddr, uint taskId, ushort qnetGatewayAddress)
        {
            var qnetMessage = new QnetIqubeActivityMessage();

            qnetMessage.SetRevokeTask(taskId);

            // Fill the qnet message
            this.FillQnetHeaderForDispositionMsg(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS, qnetDestAddr, qnetMessage.DataLength, qnetGatewayAddress);

            // buils the packet to send
            var packet = new ProtocolPacket(
                qnetMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(qnetMessage.IqubeActivityMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Returns the qnet datagram that enables to set the display on/off for an iqube unit.
        /// <remarks>
        /// This datagram could be sent with QMAIL protocol to handle acknowledgement.
        /// </remarks>
        /// </summary>
        /// <param name="qnetDestAddr">
        ///     qnet destination address of the iqube
        /// </param>
        /// <param name="taskId">
        ///     The id of the qnet task to revoke.
        /// </param>
        /// <param name="displayOn">
        ///     Flag indicating whether the display should be turn of or on.
        /// </param>
        /// <param name="startDate">
        ///     The optional start date. If <b>null</b>, the task should start immediately.
        /// </param>
        /// <param name="stopDate">
        ///     The optional stop date. If <b>null</b>, the task is valid until explicit revoke.
        /// </param>
        /// <param name="isScheduledDaily">Flag to indicate whether the task should be scheduled daily.</param>
        /// <param name="displayMode">The display Mode. See <see cref="DisplayMode"/>.</param>
        /// <param name="specialText">
        /// Text that replaces the destination if DisplayMode equals
        /// <see cref="DisplayMode.ReplaceDestWithSpecialText"/>.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <param name="lineId">The line Id for which one the trips are hidden or displayed.</param>
        /// <param name="sendOverQmail">Flag indicating that the the message is sends over QMAIL.</param>
        /// <returns>
        /// Returns an array of byte containing the qnet raw datagram to restart the iqube.
        /// </returns>
        public byte[] GetDisplayOnOffDatagram(
            ushort qnetDestAddr,
            uint taskId,
            bool displayOn,
            DateTime? startDate,
            DateTime? stopDate,
            bool isScheduledDaily,
            DisplayMode displayMode,
            string specialText,
            ushort qnetGatewayAddress,
            ushort lineId = 0,
            bool sendOverQmail = false)
        {
            var qnetMessage = new QnetIqubeActivityMessage();

            // prepare the message to send
            qnetMessage.SetDisplayOnOffActivity(
                taskId, displayOn, specialText, startDate, stopDate, isScheduledDaily, displayMode, lineId);

            // over simple qnet
            if (!sendOverQmail)
            {
                // Fill the qnet message
                this.FillQnetHeaderForDispositionMsg(
                    QnetConstantes.FIXED_QNET_ADDRESS_COMMS, qnetDestAddr, qnetMessage.DataLength, qnetGatewayAddress);

                // buils the packet to send
                var packet = new ProtocolPacket(
                    qnetMessage.DataLength,
                    QDPLayer.BodyOffet,
                    this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
                packet.SetBody(ProtocolPacket.StructureToByteArray(qnetMessage.IqubeActivityMessage));
                return this.GetQnetDatagramFromPacket(packet);
            }

            // Over qmail message:
            return this.GetQnetMessageWrappedIntoQmailDatagram(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetDestAddr,
                qnetMessage.IqubeActivityMessage,
                qnetMessage.DataLength,
                qnetGatewayAddress);
        }

        /// <summary>
        /// Returns the qnet datagram that enables to send an announcement to an iqube unit.
        /// <remarks>This datagram could be sent with QMAIl protocol to handle acknowledgement.</remarks>
        /// </summary>
        /// <param name="qnetDestAddr">
        ///     The qnet destination address.
        /// </param>
        /// <param name="activityId">
        ///     The task identifier.
        /// </param>
        /// <param name="voiceText">
        ///     The voice text to announce.
        /// </param>
        /// <param name="interval">
        ///     Interval in seconds between two announcements. Could be set with 0.
        /// </param>
        /// <param name="executionSchedule">
        ///     The execution schedule context.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <param name="sendOverQmail">
        ///     Flag indicating if the message must be sent over QMAIL.
        /// </param>
        /// <returns>
        /// Returns an array of byte containing the qnet raw datagram to send an announcement.
        /// </returns>
        public virtual byte[] GetVoiceTextDatagram(
            ushort qnetDestAddr,
            uint activityId,
            string voiceText,
            ushort interval,
            ExecutionScheduleContext executionSchedule,
            ushort qnetGatewayAddress,
            bool sendOverQmail = false)
        {
            Contract.Requires(executionSchedule != null);

            var qnetMessage = new QnetIqubeActivityMessage();

            // prepare the message to send
            qnetMessage.SetVoiceTextActivity(activityId, voiceText, interval, executionSchedule);

            // over simple qnet
            if (!sendOverQmail)
            {
                // Fill the qnet message
                this.FillQnetHeaderForDispositionMsg(
                    QnetConstantes.FIXED_QNET_ADDRESS_COMMS, qnetDestAddr, qnetMessage.DataLength, qnetGatewayAddress);

                // buils the packet to send
                var packet = new ProtocolPacket(
                    qnetMessage.DataLength,
                    QDPLayer.BodyOffet,
                    this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
                packet.SetBody(ProtocolPacket.StructureToByteArray(qnetMessage.IqubeActivityMessage));
                return this.GetQnetDatagramFromPacket(packet);
            }

            // Over qmail message:
            return this.GetQnetMessageWrappedIntoQmailDatagram(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetDestAddr,
                qnetMessage.IqubeActivityMessage,
                qnetMessage.DataLength,
                qnetGatewayAddress);
        }

        /// <summary>
        /// Returns the qnet datagram that enables to delete trip(s) from iqube unit.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet address.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="itcsProviderId">
        /// ITCS provider identifier. if set to 0, this parameter won't be evaluated during the deletion process on
        /// iqube.
        /// </param>
        /// <param name="lineId">
        /// Line identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </param>
        /// <param name="directionId">
        /// Direction identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </param>
        /// <param name="tripId">
        /// Trip identifier. if set to 0, this parameter won't be evaluated during the deletion process on iqube.
        /// </param>
        /// <param name="executionSchedule">
        /// The exec schedule.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns an array of byte containing the qnet raw datagram to send the deleteTrip message.
        /// </returns>
        public byte[] GetDeleteTripActivityMessageDatagram(
            ushort qnetDestAddr,
            uint activityId,
            ushort itcsProviderId,
            uint lineId,
            ushort directionId,
            uint tripId,
            ExecutionScheduleContext executionSchedule,
            ushort qnetGatewayAddress)
        {
            Contract.Requires(executionSchedule != null);

            var qnetMessage = new QnetIqubeActivityMessage();

            // prepare the message to send
            qnetMessage.SetDeleteTripActicity(
                activityId, itcsProviderId, lineId, directionId, tripId, executionSchedule);

            // Over qmail message:
            return this.GetQnetMessageWrappedIntoQmailDatagram(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetDestAddr,
                qnetMessage.IqubeActivityMessage,
                qnetMessage.DataLength,
                qnetGatewayAddress);
        }

        /// <summary>
        /// Build a qnet datagram from the QnetVDVMessageContext argument properties.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="context">
        /// The context that contains data to build the qnet VDV message.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns a array of bytes containing the VDV Scheduled timetable data message.
        /// </returns>
        public byte[] GetScheduledTimetableDataAsQnetDatagram(
            ushort qnetDestAddr, QnetVdvMessageContext context, ushort qnetGatewayAddress)
        {
            // Set bus message
            var vdvMessage = new QnetVdvMessage();
            vdvMessage.SetScheduledTimetableDataMessage(
                context.ITCSIdentifier,
                context.LineId,
                context.DestinationId,
                context.TripId,
                context.StopSequenceCounter,
                context.ScheduledArrivalTime,
                context.ScheduledDepartureTime,
                context.LineTextId,
                context.Destination1TextId,
                context.Destination2TextId,
                context.PlatformTextId);

            // Fills the qnet message
            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_DISPMSG,
                QnetFixedPort.QNET_PORT_DISPMSG,
                vdvMessage.DataLength,
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetDestAddr,
                qnetGatewayAddress);

            // Builds and sends the packet
            var packet = new ProtocolPacket(
                vdvMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(vdvMessage.VdvMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Build a qnet datagram from the specified QnetVDVMessageContext parameter for real time data.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="context">
        /// The context that contains all necessary fields to build the qnet VDV message.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns an array of bytes containing the raw datagram to send a RealtimeData message to an iqube
        /// </returns>
        public byte[] GetRealtimeDataAsQnetDatagram(
            ushort qnetDestAddr, QnetVdvMessageContext context, ushort qnetGatewayAddress)
        {
            // Set bus message
            var vdvMessage = new QnetVdvMessage();
            vdvMessage.SetRealtimeDataMessage(
                context.ITCSIdentifier,
                context.LineId,
                context.DestinationId,
                context.TripId,
                context.StopSequenceCounter,
                (ushort)(context.ContainsRealetime ? 1 : 0),
                context.AnAbmeldeId,
                context.ScheduledArrivalTime,
                context.EstimatedArrivalTime,
                context.ScheduledDepartureTime,
                context.EstimatedDepartureTime,
                (ushort)(context.IsAtStation ? 1 : 0),
                (ushort)context.VdvTrafficJamIndicator,
                context.LineTextId,
                context.Destination1TextId,
                context.Destination2TextId,
                context.PlatformTextId);

            // Fills the qnet message
            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_DISPMSG,
                QnetFixedPort.QNET_PORT_DISPMSG,
                vdvMessage.DataLength,
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetDestAddr,
                qnetGatewayAddress);

            // Builds and sends the packet
            var packet = new ProtocolPacket(
                vdvMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(vdvMessage.VdvMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Build a qnet datagram (QnetTFTPMessage) to send an acknowledge to a TFT mail request.
        /// </summary>
        /// <param name="qnetSrcAddr">
        /// The qnet source address.
        /// </param>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        /// <returns>
        /// Returns an array of bytes containing the raw datagram to send a TFTP mail acknowledge to a requester iqube.
        /// </returns>
        public byte[] GetTftpMailAckAsQnetDatagram(ushort qnetSrcAddr, ushort qnetDestAddr, ushort qnetGatewayAddress)
        {
            return this.GetTftpAckAsQnetDatagram(qnetSrcAddr, qnetDestAddr, 0, qnetGatewayAddress);
        }

        /// <summary>
        /// Build a qnet datagram (QnetTFTPMessage) to send a acknowledge to a TFT mail request.
        /// </summary>
        /// <param name="qnetSrcAddr">
        /// The qnet source address.
        /// </param>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="blockNumber">
        /// The block Number to acknowledge.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns an array of bytes containing the raw datagram to send a TFTP mail acknowledge to a requester iqube.
        /// </returns>
        public byte[] GetTftpAckAsQnetDatagram(
            ushort qnetSrcAddr, ushort qnetDestAddr, ushort blockNumber, ushort qnetGatewayAddress)
        {
            var tftpAck = new QnetTFTPMessage(qnetSrcAddr, qnetDestAddr, qnetGatewayAddress);
            tftpAck.SetAckMessage(blockNumber);

            // Fills the qnet message
            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_TFTP_SERVER,
                QnetFixedPort.QNET_PORT_QMAIL,
                tftpAck.DataLength,
                qnetSrcAddr,
                qnetDestAddr,
                qnetGatewayAddress);

            // Builds and returns the datagram
            var packet = new ProtocolPacket(
                tftpAck.DataLength, QDPLayer.BodyOffet, this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(tftpAck.Tftp));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Build a qnet datagram (QnetTFTPMessage) to send an acknowledge to a TFT mail request.
        /// </summary>
        /// <param name="qnetSrcAddr">
        /// The qnet source address.
        /// </param>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="qnetMessage">
        /// The qnet message to wrap into QMAIL message
        /// </param>
        /// <param name="messageLen">
        /// Length in bytes of the qnet message.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns an array of bytes containing the raw datagram to send a TFTP mail acknowledge to a requester iqube.
        /// </returns>
        public byte[] GetQnetMessageWrappedIntoQmailDatagram(
            ushort qnetSrcAddr,
            ushort qnetDestAddr,
            QnetMessageStruct qnetMessage,
            int messageLen,
            ushort qnetGatewayAddress)
        {
            if (this.QmailIdentifierProvider == null)
            {
                throw new QnetProtocolStackException("The QmailIdentifierProvider must be not null");
            }

            var tftpMessage = new QnetTFTPMessage(qnetSrcAddr, qnetDestAddr, qnetGatewayAddress);

            var qmailName = this.QmailIdentifierProvider.GetUniqueMailIdentifier();
            tftpMessage.SetQnetMessage(qnetSrcAddr, qnetDestAddr, qnetMessage, messageLen, qmailName);

            // Fills the qnet message
            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_NONE,
                QnetFixedPort.QNET_PORT_TFTP_SERVER,
                tftpMessage.DataLength,
                qnetSrcAddr,
                qnetDestAddr,
                qnetGatewayAddress);

            // Builds and returns the datagram
            var packet = new ProtocolPacket(
                tftpMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(tftpMessage.Tftp));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Returns the qnet datagram that enables to restart an iqube unit
        /// </summary>
        /// <param name="qnetAddress">qnet address of the iqube to be rebooted</param>
        /// <param name="startMode">
        /// Starting mode : See <see cref="RealtimeStartMode"/> for more details.
        /// </param>
        /// <param name="interval">
        /// Interval of data request from Realtime monitoring server in seconds.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        /// <returns>
        /// Returns an array of bytes containing the qnet raw datagram to start the monitoring session on iqube.
        /// </returns>
        public byte[] GetRealtimeMonitorStartDatagram(
            ushort qnetAddress, RealtimeStartMode startMode, int interval, ushort qnetGatewayAddress)
        {
            var realtimeMessage = new QnetRealtimeMessage();

            realtimeMessage.SetStartMessage((ushort)startMode, (ushort)interval);

            return this.GetRealtimeDatagram(qnetAddress, realtimeMessage, qnetGatewayAddress);
        }

        /// <summary>
        /// Returns the qnet datagram that enables to restart an iqube unit
        /// </summary>
        /// <param name="qnetAddress">
        /// qnet address of the iqube to be rebooted
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        /// <returns>
        /// Returns an array of bytes containing the qnet raw datagram to start the monitoring session on iqube.
        /// </returns>
        public byte[] GetRealtimeMonitorDataRequestDatagram(ushort qnetAddress, ushort qnetGatewayAddress)
        {
            var realtimeMessage = new QnetRealtimeMessage();

            realtimeMessage.SetDataRequestMessage();
            return this.GetRealtimeDatagram(qnetAddress, realtimeMessage, qnetGatewayAddress);
        }

        /// <summary>
        /// Create the qnet datagram that enables to request for active activity ids on iqube
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        /// <returns>
        /// Returns an array of bytes containing the qnet raw datagram to request for active activity ids on iqube.
        /// </returns>
        public byte[] GetActivityIdsRequestDatagram(ushort qnetDestAddr, ushort qnetGatewayAddress)
        {
            var qnetMessage = new QnetIqubeActivityMessage();

            // prepare the message to send
            qnetMessage.SetGetActivityIdsCmd();

            // Over qmail message:
            return this.GetQnetMessageWrappedIntoQmailDatagram(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetDestAddr,
                qnetMessage.IqubeActivityMessage,
                qnetMessage.DataLength,
                qnetGatewayAddress);
        }

        /// <summary>
        /// Returns the qnet datagram that enables to restart an iqube unit
        /// </summary>
        /// <param name="qnetAddress">
        /// qnet address of the iqube to be rebooted
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        /// <returns>
        /// Returns a array of byte containing the qnet raw datagram to start the monitoring session on iqube.
        /// </returns>
        public byte[] GetRealtimeMonitorStopDatagram(ushort qnetAddress, ushort qnetGatewayAddress)
        {
            var realtimeMessage = new QnetRealtimeMessage();

            realtimeMessage.SetStopMessage();
            return this.GetRealtimeDatagram(qnetAddress, realtimeMessage, qnetGatewayAddress);
        }

        /// <summary>
        /// The get The VDV alive datagram.
        /// </summary>
        /// <param name="qnetDestAddr">
        /// The qnet destination address.
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet gateway address.
        /// </param>
        /// <returns>
        /// The datagram.
        /// </returns>
        public byte[] GetVdvAliveDatagram(ushort qnetDestAddr, ushort qnetGatewayAddress)
        {
            // Set vdv message
            var vdvMessage = new QnetVdvMessage();
            vdvMessage.SetAlive();

            // Fill the qnet message
            this.FillQnetHeaderForDisplayMsg(
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS, qnetDestAddr, vdvMessage.DataLength, qnetGatewayAddress);

            // buils the packet to send
            var packet = new ProtocolPacket(
                vdvMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(vdvMessage.VdvMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        private byte[] GetRealtimeDatagram(
            ushort qnetAddress, QnetRealtimeMessage realtimeMessage, ushort qnetGatewayAddress)
        {
            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_ANY,
                QnetFixedPort.QNET_PORT_DISPCTRL,
                realtimeMessage.DataLength,
                QnetConstantes.FIXED_QNET_ADDRESS_COMMS,
                qnetAddress,
                qnetGatewayAddress);

            var packet = new ProtocolPacket(
                realtimeMessage.DataLength,
                QDPLayer.BodyOffet,
                this.internalQnetLayer.DtaLen + QnetConstantes.QNET_HEADER_LEN);
            packet.SetBody(ProtocolPacket.StructureToByteArray(realtimeMessage.QnetMessage));

            return this.GetQnetDatagramFromPacket(packet);
        }

        /// <summary>
        /// Fills the qnet header on port <see cref="QnetFixedPort.QNET_PORT_DISPMSG"/>.
        /// </summary>
        /// <param name="srcAddr">
        /// The source Address.
        /// </param>
        /// <param name="destAddr">
        /// The destination address.
        /// </param>
        /// <param name="len">
        /// The len.
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        private void FillQnetHeaderForDisplayMsg(ushort srcAddr, ushort destAddr, byte len, ushort qnetGatewayAddress)
        {
            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QNET_PORT_DISPMSG,
                QnetFixedPort.QNET_PORT_DISPMSG,
                len,
                srcAddr,
                destAddr,
                qnetGatewayAddress);
        }

        /// <summary>
        /// Fills the qnet header on port <see cref="QnetFixedPort.QnetPortDispo"/>.
        /// </summary>
        /// <param name="srcAddr">
        /// The source Address.
        /// </param>
        /// <param name="destAddr">
        /// The destination Address.
        /// </param>
        /// <param name="len">
        /// The length
        /// </param>
        /// <param name="qnetGatewayAddress">
        /// The qnet Gateway Address.
        /// </param>
        private void FillQnetHeaderForDispositionMsg(
            ushort srcAddr, ushort destAddr, byte len, ushort qnetGatewayAddress)
        {
            this.FillQnetHeadersForIppDataType(
                QnetFixedPort.QnetPortDispo, QnetFixedPort.QnetPortDispo, len, srcAddr, destAddr, qnetGatewayAddress);
        }

        /// <summary>
        /// Fills QDP header with the given source and destination ports
        /// </summary>
        /// <param name="srcPort">Source port number</param>
        /// <param name="desPort">Destination port number</param>
        private void FillQdpHeader(byte srcPort, byte desPort)
        {
            this.internalQdpLayer.QDPHeader.srcPort = srcPort;
            this.internalQdpLayer.QDPHeader.dstPort = desPort;
        }

        /// <summary>
        /// Fills the datagram header of the QNP layer
        /// </summary>
        /// <param name="srcPort">
        /// The src port.
        /// </param>
        /// <param name="desPort">
        /// The des port.
        /// </param>
        /// <param name="msgDataLength">
        /// The message data length.
        /// </param>
        /// <param name="qnetAddrSrc">
        /// The qnet address src.
        /// </param>
        /// <param name="qnetAddrDst">
        /// The qnet address destination.
        /// </param>
        private void FillQnpHeader(
            byte srcPort, byte desPort, byte msgDataLength, ushort qnetAddrSrc, ushort qnetAddrDst)
        {
            // fill QDP header
            this.FillQdpHeader(srcPort, desPort);

            this.internalQnpLayer.QNPHeader.verprio =
                (byte)((QnpConstantes.QNP_VERSION << 4) | QnpConstantes.QNP_PRIORITY);
            this.internalQnpLayer.QNPHeader.payload = (byte)(msgDataLength + QdpConstantes.QDP_HEADER_LEN);
            this.internalQnpLayer.QNPHeader.nxtheader = QnpConstantes.QNP_PT_QDP;
            this.internalQnpLayer.QNPHeader.hoplimit = QnpConstantes.QNP_HOPLIMIT;
            this.internalQnpLayer.QNPHeader.srcAddr = qnetAddrSrc;
            this.internalQnpLayer.QNPHeader.dstAddr = qnetAddrDst;
        }

        /// <summary>
        /// Fills the header for the IPP data type for the QNP layer
        /// </summary>
        /// <param name="srcPort">
        /// The src port.
        /// </param>
        /// <param name="desPort">
        /// The des port.
        /// </param>
        /// <param name="msgDataLength">
        /// The message data length.
        /// </param>
        /// <param name="qnetAddrSrc">
        /// The qnet address src.
        /// </param>
        /// <param name="qnetAddrDst">
        /// The qnet address destination.
        /// </param>
        /// <param name="qnetGatewayAddress">The gateway address.</param>
        private void FillQnetHeadersForIppDataType(
            byte srcPort,
            byte desPort,
            byte msgDataLength,
            ushort qnetAddrSrc,
            ushort qnetAddrDst,
            ushort qnetGatewayAddress)
        {
            // fill QNP header
            this.FillQnpHeader(srcPort, desPort, msgDataLength, qnetAddrSrc, qnetAddrDst);

            // fill IPP header
            this.internalQnetLayer.QnetIppHdr.ver_typ = (byte)QnetType.QNET_TYPE_IPP
                                                        + (byte)QnetVersion.QNET_VERSION_IPP;
            this.internalQnetLayer.QnetIppHdr.msg_typ = IIPP_MSGTYPE.IIPP_MSGTYP_DATA;

            // QnetIppHdr.src_dev is hard coded because it is not used anymore
            this.internalQnetLayer.QnetIppHdr.src_dev = IIPP_DEVTYPE.IIPP_DEVTYP_ALL;

            // QnetIppHdr.dst_dev is hard coded because it is not used anymore
            this.internalQnetLayer.QnetIppHdr.dst_dev = IIPP_DEVTYPE.IIPP_DEVTYP_ALL;

            // QnetIppHdr.src_lan not used
            this.internalQnetLayer.QnetIppHdr.src_lan = 0;

            // QnetIppHdr.dst_lan not used
            this.internalQnetLayer.QnetIppHdr.dst_lan = 0;

            this.internalQnetLayer.QnetIppHdr.srcAddr = qnetAddrSrc;
            this.internalQnetLayer.QnetIppHdr.dstAddr = qnetGatewayAddress;

            // fill QNET message parameter
            this.internalQnetLayer.Reserved1 = 0; // reserved
            this.internalQnetLayer.Reserved2 = 0; // reserved
            this.internalQnetLayer.NextHop = qnetGatewayAddress; // next hop network address
            this.internalQnetLayer.IfcNum = QNETifcNum.QNET_IFC_LOCAL; // originating interface number
            this.internalQnetLayer.DtaLen =
                (ushort)(this.internalQnpLayer.QNPHeader.payload + QnpConstantes.QNP_HEADER_LEN); // data length
            this.internalQnetLayer.DevPar1 = 0; // communication device parameter 1
            this.internalQnetLayer.DevPar2 = 0; // communication device parameter 2
        }

        /// <summary>
        /// Get the qnet datagram from the given data packet
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// type : byte array
        /// Returns an array of bytes containing the qnet datagram packet that will be sent to the unit.
        /// </returns>
        private byte[] GetQnetDatagramFromPacket(ProtocolPacket packet)
        {
            // 1 - Transmit the packet to the protocol stack to fill it with data layers
            this.HandleTransmit(ref packet);

            // 2 - Create the final qnet datagramm to send
            this.qna.PrepareQnetDatagramm(packet.GetHeader(), packet.GetBody());
            this.QnetSrcAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.srcAddr;
            this.QnetDstAddress.QNETAddr = this.internalQnetLayer.QnetIppHdr.dstAddr;

            return this.qna.RawDatagramm;
        }

        /// <summary>
        /// Convert the received qnet datagram into a internal qnet message.
        /// </summary>
        private void ConvertQnetDataToQnetMessage()
        {
            QnetMessageBase lastQnetDecodedMessage = null;
            switch (this.internalQnetLayer.QnetType)
            {
                case QnetType.QNET_TYPE_IPP:
                    switch (this.internalQnetLayer.QnetIppHdr.msg_typ)
                    {
                        case IIPP_MSGTYPE.IIPP_MSGTYP_REQUEST:
                            lastQnetDecodedMessage = new QnetRequestMessage(
                                this.internalQnpLayer.QNPHeader.srcAddr,
                                this.internalQnpLayer.QNPHeader.dstAddr,
                                this.internalQnetLayer.NextHop);
                            break;
                        case IIPP_MSGTYPE.IIPP_MSGTYP_DATA:
                            switch (this.internalQnpLayer.QnpProtocol)
                            {
                                case QnpProtocol.QNP_PT_QCP: // Control prot (used for ping)
                                    break;
                                case QnpProtocol.QNP_PT_QDP: // Datagram protocol (= to UDP)
                                    switch (this.internalQdpLayer.QDPHeader.dstPort)
                                    {
                                        case QnetFixedPort.QNET_PORT_QSNMP_SERVER:
                                            lastQnetDecodedMessage =
                                                QnetSNMPMessage.ConvertDataToQnetMessage(
                                                    this.internalQnpLayer.QNPHeader.srcAddr,
                                                    this.internalQnpLayer.QNPHeader.dstAddr,
                                                    this.internalQdpLayer.QDPData,
                                                    this.internalQnetLayer.NextHop);
                                            break;
                                        case QnetFixedPort.QNET_PORT_EVENT:
                                        case QnetFixedPort.QNET_PORT_TEST:
                                        case QnetFixedPort.QNET_PORT_TFTP_SERVER:
                                        case QnetFixedPort.QNET_PORT_QMAIL:
                                        case QnetFixedPort.QnetPortAlarm:
                                            lastQnetDecodedMessage =
                                                QnetTFTPMessage.ConvertDataToQnetMessage(
                                                    this.internalQnpLayer.QNPHeader.srcAddr,
                                                    this.internalQnpLayer.QNPHeader.dstAddr,
                                                    this.internalQdpLayer.QDPData,
                                                    this.internalQnetLayer.NextHop);
                                            break;
                                        case QnetFixedPort.QNET_PORT_DISPCTRL:
                                            lastQnetDecodedMessage =
                                                QnetRealtimeMessage.ConvertDataToQnetMessage(
                                                    this.internalQnpLayer.QNPHeader.srcAddr,
                                                    this.internalQnpLayer.QNPHeader.dstAddr,
                                                    this.internalQdpLayer.QDPData,
                                                    this.internalQnetLayer.NextHop);
                                            break;

                                            // case QnetFixedPort.QNET_PORT_QNP:
                                        default:
                                            lastQnetDecodedMessage =
                                                QnetMessage.ConvertDataToQnetMessage(
                                                    this.internalQnpLayer.QNPHeader.srcAddr,
                                                    this.internalQnpLayer.QNPHeader.dstAddr,
                                                    this.internalQdpLayer.QDPData,
                                                    this.internalQnetLayer.NextHop);
                                            break;
                                    }

                                    break;
                                case QnpProtocol.QNP_PT_QTP: // Transport protocol (= to TCP)
                                    break;
                            }

                            break;
                    }

                    break;
            }

            if (lastQnetDecodedMessage != null)
            {
                this.NotifyReceivedQnetMessage(lastQnetDecodedMessage);
            }
        }

        private void NotifyReceivedQnetMessage(QnetMessageBase lastQnetDecodedMessage)
        {
            var receivedQnetMessage = this.ReceivedQnetMessage;

            if (receivedQnetMessage != null)
            {
                receivedQnetMessage(this, new ReceivedQnetMessageArgs(lastQnetDecodedMessage));
            }
        }
    }
}