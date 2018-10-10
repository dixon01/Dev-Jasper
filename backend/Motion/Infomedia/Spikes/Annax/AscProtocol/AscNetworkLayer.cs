namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;
    using System.Collections.Generic;

    using NLog;

    public class AscNetworkLayer : INetworkLayer
    {
        private const NetworkServiceType NetworkServiceTypeAnswer = (NetworkServiceType)0x10;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<byte, PacketInfo> lastPacketInfo = new Dictionary<byte, PacketInfo>();

        private IPhysicalLayer physicalLayer;
        private IApplicationLayer applicationLayer;

        private int readOffset;
        private byte[] readBuffer;

        private byte nextSendTransactionId;
        private byte[] writeBuffer;

        public byte ProtocolId
        {
            get
            {
                return 0;
            }
        }

        public void SetPhysicalLayer(IPhysicalLayer phy)
        {
            if (this.physicalLayer == phy)
            {
                return;
            }

            this.physicalLayer = phy;
            this.physicalLayer.SetNetworkLayer(this);
            this.writeBuffer = new byte[phy.MaxPayloadSize];
        }

        public void SetApplicationLayer(IApplicationLayer app)
        {
            if (this.applicationLayer == app)
            {
                return;
            }

            this.applicationLayer = app;
            this.applicationLayer.SetNetworkLayer(this);
        }

        public void ReadPacket(byte sourceAddr, NetworkServiceType serviceType, FrameReader reader)
        {
            var packetNumber = reader.ReadByte();
            var transactionId = reader.ReadByte();

            if ((serviceType & NetworkServiceTypeAnswer) != 0)
            {
                var errorCode = reader.ReadByte();
                Logger.Info(
                    "Received {0} #{1:X2} tid={2:X2} error={3:X2}",
                    serviceType,
                    packetNumber,
                    transactionId,
                    errorCode);
                return;
            }

            Logger.Info("Received {0} #{1:X2} tid={2:X2}", serviceType, packetNumber, transactionId);

            PacketInfo packetInfo;
            if (!this.lastPacketInfo.TryGetValue(sourceAddr, out packetInfo))
            {
                packetInfo = new PacketInfo();
                this.lastPacketInfo.Add(sourceAddr, packetInfo);
            }

            if (packetInfo.LastTransactionId == transactionId)
            {
                if (packetNumber <= packetInfo.LastPacketNumber)
                {
                    Logger.Warn("Ignoring duplicate packet number for the same transaction");
                    return;
                }

                if (packetNumber != packetInfo.LastPacketNumber + 1)
                {
                    Logger.Warn(
                        "Ignoring wrong packet number: expected {0} but got {1}",
                        packetInfo.LastPacketNumber + 1,
                        packetNumber);
                    return;
                }

                packetInfo.LastPacketNumber = packetNumber;
                if (this.AppendToDataBuffer(reader))
                {
                    var bufferReader = new FrameReader(this.readBuffer, 0, this.readBuffer.Length);
                    this.applicationLayer.ReadData(serviceType, bufferReader);
                }

                return;
            }

            packetInfo.LastPacketNumber = packetNumber;
            packetInfo.LastTransactionId = transactionId;

            var totalLength = reader.ReadUInt16();

            if (totalLength > reader.Available)
            {
                this.readOffset = 0;
                this.readBuffer = new byte[totalLength];
                if (!this.AppendToDataBuffer(reader))
                {
                    return;
                }
            }

            this.applicationLayer.ReadData(serviceType, reader);
        }

        public int WritePacket(NetworkServiceType serviceType, FrameWriter writer)
        {
            var tid = this.nextSendTransactionId++;

            var totalLength = writer.Length;
            if (totalLength > this.writeBuffer.Length)
            {
                throw new NotSupportedException("Only small packets supported for now");
            }

            this.writeBuffer[0] = 0;
            this.writeBuffer[1] = tid;
            this.writeBuffer[2] = (byte)((totalLength >> 8) & 0xFF);
            this.writeBuffer[3] = (byte)(totalLength & 0xFF);

            writer.CopyTo(0, this.writeBuffer, 4, totalLength);
            this.physicalLayer.Encode(this, serviceType, this.writeBuffer, 0, totalLength + 4);
            return tid;
        }

        private bool AppendToDataBuffer(FrameReader reader)
        {
            var data = reader.ReadRemainingBytes();
            Array.Copy(data, 0, this.readBuffer, this.readOffset, data.Length);
            this.readOffset += data.Length;
            if (this.readOffset != this.readBuffer.Length)
            {
                Logger.Debug(
                    "Got partial data stream {0} of {1} data bytes received",
                    this.readOffset,
                    this.readBuffer.Length);
                return false;
            }

            Logger.Debug("Got entire data stream ({0} bytes)", this.readBuffer.Length);
            return true;
        }

        private class PacketInfo
        {
            public PacketInfo()
            {
                this.LastTransactionId = -1;
                this.LastPacketNumber = -1;
            }

            public int LastPacketNumber { get; set; }

            public int LastTransactionId { get; set; }
        }
    }
}