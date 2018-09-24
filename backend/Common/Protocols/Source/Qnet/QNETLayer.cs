namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using Gorba.Common.Protocols.Core;

    #region alias declaration
    // These aliases enable to make the // with qnet definitions. 
    // This section should be duplicated into each file that needs these aliases !!!
    // see Gorba.Common.Protocols.Qnet.QnetEnums.cs

    using _QNETport = System.Byte;

    // typedef WORD tQNETaddr;       
    // Iqube network address
    using _QNETaddr = System.UInt16;

    // typedef BYTE tQNETport;       
    // Iqube protocol port (mapped to taskId)    
    using _QnetPort = System.Byte;
    #endregion


    /// <summary>
    /// Implementation of IQUBE Datagram Protocol (Data link layer - layer 2).
    /// Puts data in frames
    /// </summary>
    public class QNETLayer: ProtocolLayer
    {
        #region Qnet fields
        
        public tQNETippHdr QnetIppHdr;

        private _QNETaddr m_NextHop;        // next hop network address

        public _QNETaddr NextHop
        {
            get { return m_NextHop; }
            set { m_NextHop = value; }
        }

        private UInt32 m_Reserved1;

        public UInt32 Reserved1
        {
            get { return m_Reserved1; }
            set { m_Reserved1 = value; }
        }
        private UInt32 m_Reserved2;

        public UInt32 Reserved2
        {
            get { return m_Reserved2; }
            set { m_Reserved2 = value; }
        }
        /// <summary>
        /// communication device parameter 1
        /// </summary>
        private Byte m_DevPar1;

        public Byte DevPar1
        {
            get { return m_DevPar1; }
            set { m_DevPar1 = value; }
        }

        /// <summary>
        /// communication device parameter 2
        /// </summary>
        private Byte m_DevPar2;

        public Byte DevPar2
        {
            get { return m_DevPar2; }
            set { m_DevPar2 = value; }
        }

        QNETifcNum m_IfcNum;

        public QNETifcNum IfcNum
        {
            get { return m_IfcNum; }
            set { m_IfcNum = value; }
        }

        private UInt16 m_DtaLen;

        public UInt16 DtaLen
        {
            get { return m_DtaLen; }
            set { m_DtaLen = value; }
        }

        private QnetType m_QnetType;

        public QnetType QnetType
        {
            get { return m_QnetType; }
        }
        #endregion



        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="qnetProtocolStack"></param>
        public QNETLayer()
        {
            QnetIppHdr = new tQNETippHdr();
        }

        /// <summary>
        /// Handles currently only IPP datagramm
        /// This layer send the qnet datagramm via the QNA object. First, the header is filled with qnet header information 
        /// </summary>
        /// <param name="packet"></param>
        public override void Transmit(ref ProtocolPacket packet)
        {
            switch (QnetIppHdr.msg_typ) {
                case IIPP_MSGTYPE.IIPP_MSGTYP_DATA:
                    // Send data without change anything
                    break;
                default:
                    QnetIppHdr.ver_typ = (_QNETport)QnetType.QNET_TYPE_IPP + (_QNETport)QnetVersion.QNET_VERSION_IPP;
                    //QnetIppHdr.msg_typ = m_IppMsgType;
                    QnetIppHdr.src_dev = 0;
                    QnetIppHdr.dst_dev = 0;
                    QnetIppHdr.src_lan = 0;
                    QnetIppHdr.dst_lan = 0;
                    QnetIppHdr.srcAddr = QnetConstantes.QnetAddrAny;
                    QnetIppHdr.dstAddr = m_NextHop;

                    break;
            } // switch message type          
            
            packet.AddHeader(QnetConstantes.QNET_HEADER_LEN);
            packet.SetHeader(ProtocolPacket.StructureToByteArray(QnetIppHdr));          

            // Transmit if necessary 
            if (LowerLayer != null) {                             
                LowerLayer.Transmit(ref packet);
            } // if
        }

        public override void HandleReceive(ref ProtocolPacket packet)
        {
            packet.ExtractHeader(QnetConstantes.QNET_HEADER_LEN);
            //packet.ExtractHeader(Marshal.SizeOf(m_tQNET.Hdr));
            // Get data from byte array
            byte[] body = packet.GetBody();
            byte[] header = packet.GetHeader();

            m_QnetType = (QnetType)((byte)(header[0] & QnetMask.QNET_TYPE_MASK));
            switch (m_QnetType) {
                case QnetType.QNET_TYPE_ASE:
                    // to do
                    UpperLayer.HandleReceive(ref packet);
                    break;
                case QnetType.QNET_TYPE_EXT:
                    // to do
                    UpperLayer.HandleReceive(ref packet);
                    break;
                case QnetType.QNET_TYPE_IPP:

                    m_DevPar1 = 0;
                    m_DevPar2 = 0;
                    // See qnet header
                    QnetIppHdr = ProtocolPacket.ByteArrayToStruct<tQNETippHdr>(header);

                    m_NextHop = QnetIppHdr.srcAddr;
                    m_DtaLen = (ushort)body.Length;

                    //m_tQNET.ifcNum = 0;
                    switch (QnetIppHdr.msg_typ) {
                        case IIPP_MSGTYPE.IIPP_MSGTYP_NOTIFY:
                        case IIPP_MSGTYPE.IIPP_MSGTYP_REQUEST:
                        case IIPP_MSGTYPE.IIPP_MSGTYP_RESPONSE:
                            //QnetProcessIppMessage(packet);
                            break;
                       case IIPP_MSGTYPE.IIPP_MSGTYP_DATA:
                            UpperLayer.HandleReceive(ref packet);
                            //QnetProcessIppMessage(ref packet);
                            break;
                        default:
                            // no upper layr for other cases
                            break;
                    } // switch message type
                    
                    break;

                case QnetType.QNET_TYPE_STD:
                    break;
                default:
                    // Error ERROR_QNA_INVALID_HEADER_TYPE
                    break;
            } // switch
        }

        /// <summary>
        /// To delete 
        /// </summary>
        private void QnetProcessIppMessage(ProtocolPacket packet)
        {
            switch (QnetIppHdr.msg_typ) {
                case IIPP_MSGTYPE.IIPP_MSGTYP_NOTIFY:

                    break;

                case IIPP_MSGTYPE.IIPP_MSGTYP_REQUEST:
                    m_NextHop = QnetIppHdr.srcAddr;
                    QnetIppHdr.msg_typ = IIPP_MSGTYPE.IIPP_MSGTYP_RESPONSE;
                    //SendQnetPacket(packet);
                    Transmit(ref packet);
                    break;

                case IIPP_MSGTYPE.IIPP_MSGTYP_RESPONSE:

                    break;
            } // switch message type
        }
    }
}
