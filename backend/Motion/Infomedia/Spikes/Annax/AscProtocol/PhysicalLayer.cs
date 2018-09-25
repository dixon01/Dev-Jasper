namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;
    using System.Collections.Generic;

    using NLog;

    public class PhysicalLayer : IPhysicalLayer
    {
        private const byte StartOfHeader = 0x1;
        private const byte Enquiry = 0x05;
        private const byte DataLinkEscape = 0x10;

        private const byte EscapeXor = 0x20;

        private const int FcsSize = sizeof(ushort);

        private const MacServiceType MacServiceTypeMask = (MacServiceType)0xC0;
        private const NetworkServiceType NetworkServiceTypeMask = (NetworkServiceType)0x3F;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<byte, INetworkLayer> networkLayers = new Dictionary<byte, INetworkLayer>();

        private readonly byte[] decodeBuffer = new byte[260];
        private readonly byte[] sendBuffer = new byte[520];

        private readonly Fcs16 fcs16 = new Fcs16(0xa001);

        public event EventHandler<DataEventArgs> DataReady;

        public int MaxPayloadSize
        {
            get
            {
                return 251;
            }
        }

        public void SetNetworkLayer(INetworkLayer net)
        {
            INetworkLayer networkLayer;
            if (this.networkLayers.TryGetValue(net.ProtocolId, out networkLayer) && networkLayer == net)
            {
                return;
            }

            this.networkLayers[net.ProtocolId] = net;
            net.SetPhysicalLayer(this);
        }

        public int Decode(byte[] buffer, int offset, int count)
        {
            var startIndex = Array.IndexOf(buffer, StartOfHeader, offset, count);
            if (startIndex < 0 || startIndex + 1 >= offset + count)
            {
                // no SOF or only SOF but no length byte yet (and also no escaped length)
                return 0;
            }

            var index = startIndex + 1;
            var length = ReadByte(buffer, offset + count, ref index);

            if (index + length + FcsSize >= offset + count)
            {
                return 0;
            }

            for (int i = 0; i < length + FcsSize; i++)
            {
                var read = ReadByte(buffer, offset + count, ref index);
                if (read < 0)
                {
                    return 0;
                }

                this.decodeBuffer[i] = (byte)read;
            }

            if (startIndex > 0)
            {
                Logger.Warn("Skipping unknown data:");
                for (int i = 0; i < startIndex; i += 8)
                {
                    Logger.Warn("    {0}", BitConverter.ToString(buffer, i, Math.Min(8, startIndex - i)));
                }
            }

            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("Raw frame:");
                for (int i = startIndex; i < index; i += 8)
                {
                    Logger.Trace("    {0}", BitConverter.ToString(buffer, i, Math.Min(8, index - i)));
                }
            }

            ushort fcs = 0;
            for (int i = 0; i < length; i++)
            {
                this.fcs16.Update(ref fcs, this.decodeBuffer[i]);
            }

            var receivedFcs = BitConverter.ToUInt16(this.decodeBuffer, length);
            if (fcs != receivedFcs)
            {
                Logger.Warn("Frame has wrong FCS {0:X4} received, {1:X4} calculated", receivedFcs, fcs);
                return index;
            }

            var protocolType = this.decodeBuffer[0];
            var reader = new FrameReader(this.decodeBuffer, 1, length - 1);
            if (!this.HandleFrame(protocolType, reader))
            {
                return index;
            }

            if (reader.Available > 0)
            {
                Logger.Warn("Didn't read entire frame: {0} bytes left", reader.Available);
            }

            return index;
        }

        public void Encode(
            INetworkLayer networkLayer, NetworkServiceType networkServiceType, byte[] buffer, int offset, int count)
        {
            if (count > this.MaxPayloadSize)
            {
                throw new ArgumentOutOfRangeException(
                    "count", count, "Can't be more than " + this.MaxPayloadSize + " bytes");
            }

            var protocolId = networkLayer.ProtocolId;
            var sourceAddr = (byte)0xFE;
            var destAddr = (byte)0x01;
            var serviceType = (byte)((int)networkServiceType | (int)MacServiceType.AnsweredFrame);

            var index = 3;
            ushort fcs = 0;

            this.fcs16.Update(ref fcs, protocolId);
            WriteByte(this.sendBuffer, protocolId, ref index);

            this.fcs16.Update(ref fcs, sourceAddr);
            WriteByte(this.sendBuffer, sourceAddr, ref index);

            this.fcs16.Update(ref fcs, destAddr);
            WriteByte(this.sendBuffer, destAddr, ref index);

            this.fcs16.Update(ref fcs, serviceType);
            WriteByte(this.sendBuffer, serviceType, ref index);

            for (int i = offset; i < offset + count; i++)
            {
                var b = buffer[i];
                this.fcs16.Update(ref fcs, b);
                WriteByte(this.sendBuffer, b, ref index);
            }

            // write the length at the end (we don't know in the beginning), escaping if necessary
            var length = count + 4;
            int startOffset;
            if (length == StartOfHeader || length == Enquiry || length == DataLinkEscape)
            {
                this.sendBuffer[0] = StartOfHeader;
                this.sendBuffer[1] = DataLinkEscape;
                this.sendBuffer[2] = (byte)(length ^ EscapeXor);
                startOffset = 0;
            }
            else
            {
                this.sendBuffer[1] = StartOfHeader;
                this.sendBuffer[2] = (byte)length;
                startOffset = 1;
            }

            WriteByte(this.sendBuffer, (byte)(fcs & 0xFF), ref index);
            WriteByte(this.sendBuffer, (byte)((fcs >> 8) & 0xFF), ref index);

            this.RaiseDataReady(new DataEventArgs(this.sendBuffer, startOffset, index - startOffset));
        }

        protected virtual void RaiseDataReady(DataEventArgs e)
        {
            var handler = this.DataReady;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static int ReadByte(byte[] buffer, int maxIndex, ref int offset)
        {
            if (offset == maxIndex)
            {
                return -1;
            }

            var read = buffer[offset++];
            if (read == DataLinkEscape)
            {
                if (offset == maxIndex)
                {
                    return -1;
                }

                read = (byte)(buffer[offset++] ^ EscapeXor);
            }

            return read;
        }

        private static void WriteByte(byte[] buffer, byte value, ref int index)
        {
            if (value == StartOfHeader || value == Enquiry || value == DataLinkEscape)
            {
                buffer[index++] = DataLinkEscape;
                value ^= EscapeXor;
            }

            buffer[index++] = value;
        }

        private bool HandleFrame(byte protocolType, FrameReader reader)
        {
            INetworkLayer networkLayer;
            if (!this.networkLayers.TryGetValue(protocolType, out networkLayer))
            {
                Logger.Warn("Frame protocol {0:X2} unknown, skipping frame", protocolType);
                return false;
            }

            var sourceAddr = reader.ReadByte();
            var destAddr = reader.ReadByte();
            var serviceType = reader.ReadByte();

            Logger.Info(
                "Received frame: src={0:X2} dst={1:X2} srv={2:X2} ({3})",
                sourceAddr,
                destAddr,
                serviceType,
                (MacServiceType)serviceType & MacServiceTypeMask);

            networkLayer.ReadPacket(sourceAddr, (NetworkServiceType)serviceType & NetworkServiceTypeMask, reader);
            return true;
        }
    }
}
