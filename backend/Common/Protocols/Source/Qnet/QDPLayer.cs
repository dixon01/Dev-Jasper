namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using Gorba.Common.Protocols.Core;

    /// <summary>
    /// Implementation of IQUBE Datagram Protocol (Transport Layer).
    /// </summary>
    public class QDPLayer: ProtocolLayer
    {
        public static int BodyOffet = QnetConstantes.QNET_HEADER_LEN + QnpConstantes.QNP_HEADER_LEN + QdpConstantes.QDP_HEADER_LEN;

        public tQDPhdr QDPHeader;
        public byte[] QDPData;

        public QDPLayer()
        {
            QDPHeader = new tQDPhdr();
        }

        public override void Transmit(ref ProtocolPacket packet)
        {
            packet.AddHeader(QdpConstantes.QDP_HEADER_LEN);
            packet.SetHeader(ProtocolPacket.StructureToByteArray(QDPHeader));
            if (LowerLayer != null) {
                LowerLayer.Transmit(ref packet);
            } // if
        }

        public override void HandleReceive(ref ProtocolPacket packet)
        {
            packet.ExtractHeader(QdpConstantes.QDP_HEADER_LEN);
            // Get data from byte array
            byte[] body = packet.GetBody();
            byte[] header = packet.GetHeader();

            // See QDP header
            QDPHeader = ProtocolPacket.ByteArrayToStruct<tQDPhdr>(header);
            if (body.Length > 0) {
                QDPData = new byte[body.Length];
                Array.Copy(body, 0, QDPData, 0, body.Length);
            } else {
                QDPData = null;
            } // else

            if (UpperLayer != null) {
                UpperLayer.HandleReceive(ref packet);
            } // if            
        }
    }
}
