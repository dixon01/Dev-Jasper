namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using Gorba.Common.Protocols.Core;

    /// <summary>
    /// Implementation of IQUBE Datagram Protocol (Transport Layer, layer 3).
    /// </summary>
    public class QNPLayer: ProtocolLayer
    {

        public tQNPhdr QNPHeader;
        public byte[] QNPData;

        public QNPLayer()
        {
            QNPHeader = new tQNPhdr();
        }

        private QnpProtocol m_QnpProtocol;

        public QnpProtocol QnpProtocol
        {
            get { return m_QnpProtocol; }
        } 

        public override void Transmit(ref ProtocolPacket packet)
        {
            packet.AddHeader(QnpConstantes.QNP_HEADER_LEN);
            packet.SetHeader(ProtocolPacket.StructureToByteArray(QNPHeader));
            if (LowerLayer != null) {
                LowerLayer.Transmit(ref packet);
            } // if
        }

        public override void HandleReceive(ref ProtocolPacket packet)
        {
            // Do routing here

            packet.ExtractHeader(QnpConstantes.QNP_HEADER_LEN);
            // Get data from byte array
            byte[] body = packet.GetBody();
            byte[] header = packet.GetHeader();

            // See QNP header
            QNPHeader = ProtocolPacket.ByteArrayToStruct<tQNPhdr>(header);

            if (body.Length > 0) {
                QNPData = new byte[body.Length];
                Array.Copy(body, 0, QNPData, 0, body.Length);
            } else {
                QNPData = null;
            } // else

            m_QnpProtocol = (Qnet.QnpProtocol)QNPHeader.nxtheader;            

            if (UpperLayer != null) {
                UpperLayer.HandleReceive(ref packet);
            } // if     
       
        }
    }
}
