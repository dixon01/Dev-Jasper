// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetEnums.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    #region alias declaration

    // These aliases enable to make the // with qnet definitions. 
    // This section should be duplicated into each file that needs these aliases !!!
    // see Gorba.Common.Protocols.Qnet.QnetEnums.cs

    // typedef WORD tQNETaddr;       
        // Iqube network address
    using System;
    using System.Runtime.InteropServices;

    using _QNETaddr = System.UInt16;
    using _QNETport = System.Byte;

    // typedef BYTE tQNETport;       
        // Iqube protocol port (mapped to taskId)    

    #endregion

    /// <summary>
    /// The address class enumeration
    /// </summary>
    internal enum QnetClass : int
    {
        /// <summary>
        /// Not a valid Address Class
        /// </summary>
        QNETClassNone = -1,

        /// <summary>
        /// Iqube Network Address Class A
        /// </summary>
        QNETClassA = 0,

        /// <summary>
        /// Iqube Network Address Class B
        /// </summary>
        QNETClassB = 1,

        /// <summary>
        /// Iqube Network Address Class C
        /// </summary>
        QNETClassC = 2,

        /// <summary>
        /// Iqube Network Address Class D
        /// </summary>
        QNETClassD = 3,

        /// <summary>
        /// Iqube Network Address Class E
        /// </summary>
        QNETClassE = 4,

        /// <summary>
        /// Iqube Network Address Class F
        /// </summary>
        QNETClassF = 5,

        /// <summary>
        /// Just to have a max class 
        /// </summary>
        QNETClassMax
    }

    /// <summary>
    /// tSOCKtype enumeration
    /// </summary>
    public enum tSOCKtype // socket type
    {
        /// socket type none (not a valid type)
        SOCK_TYP_NONE,

        /// socket type datagram 
        SOCK_TYP_DGRAM,

        /// socket type stream
        SOCK_TYP_STREAM,

        ///  socket type max. value
        SOCK_TYP_MAX
    }

    /// <summary>
    /// The tSOCKaddr struct. Is defined in the QnetDll.dll source from csa
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct tSOCKaddr
    {
        /// <summary>
        /// The port number
        /// </summary>
        public _QNETport port;

        /// <summary>
        /// First part of the address
        /// </summary>
        public _QNETport adr1;

        /// <summary>
        /// second part of the address
        /// </summary>
        public _QNETport adr2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct tQNETippHdr // Iqube QNET IP tunneling protocol header
    {
        public Byte ver_typ; // version/type field

        public IIPP_MSGTYPE msg_typ; // message type

        public IIPP_DEVTYPE src_dev; // source iqube device type

        public IIPP_DEVTYPE dst_dev; // destination iqube device type

        public UInt16 src_lan; // source LAN number

        public UInt16 dst_lan; // destination LAN number

        public _QNETaddr srcAddr; // network address of source

        public _QNETaddr dstAddr; // network address of destination
    }

    /*
                [MarshalAs(
                UnmanagedType.ByValArray,
                SizeConst = 128)]
     * */


    /// <summary>
    /// Definition of some qnet constantes
    /// </summary>
    public class QnetConstantes
    {
        /// <summary>
        /// Maximum number of caracters for special info line text 160 + NULL
        /// </summary>
        public const int MaxInfoLineTextSize = 161;

        /// <summary>
        /// Number of maximum caracters for the name of the file
        /// </summary>
        public const byte QfsMaxFileNameSize = 12;

        /// <summary>
        /// Lenght of the string that represents a bus line.
        /// </summary>
        public const int LineTextLenght = 8;

        // Definition of special network addresses:

        /// <summary>
        /// The CommS has a static qnet address : A:0.7.1 (= 57 (ushort) 
        /// </summary>
        public const string FixedQnetAddressForCommS = "A:0.7.1"; 

        /// <summary>
        /// not a valid network address
        /// </summary>
        public const ushort QnetAddrNone = 0xFFFF;

        /// <summary>
        /// any network address        
        /// </summary>
        public const ushort QnetAddrAny = 0x0000;

        /// <summary>
        /// Broadcast <CLASS D>
        /// </summary>
        public static ushort QNET_ADDR_BROADCAST = 0xE000;

        /// <summary>
        /// Broadcast MASK <CLASS D addresses>
        /// </summary>
        public static ushort QNET_ADDR_BRC_MASK = 0xE000;

        public static ushort QNET_ADDR_BRC_MASKALL = 0xEFFF; // Broadcast MASK ALL <CLASS D>

        public static ushort QNET_ADDR_LOOPBACK = 0xC001; // Loopback <CLASS C>

        // Definition of max. values for CLASS_A address parts
        public static byte QNET_MAX_SUBNET = 0x0F;

        public static byte QNET_MAX_STATION = 0xFF;

        public static byte QNET_MAX_IQUBE = 0x07;

        public static byte QNET_XOR_VALUE = 0x20;

        // Definition for Qnet messages
        public const int MAX_TEXTLEN_SHORT = 3;

        public const int MAX_TEXTLEN_MEDIUM = 32; //orig 32

        public const int MAX_TEXTLEN_LONG = 210; //in vos.h definiert

        public const int LINE_STATE_LENGTH = 5;

        // lengths
        public static int TXBUF_BUF_LEN = 520;

        public static int RX_MIN_LENGTH = Marshal.SizeOf(new tQNETippHdr());

        // Maximum lenght of QNET frame
        public static ushort QNET_MAX_FRAME_LEN = 0xFF;

        public static ushort QNET_HEADER_LEN = 0x0C; // 12 octets

        public static ushort QNET_IPP_HDR_LEN = (UInt16)Marshal.SizeOf(new tQNETippHdr());

        public static ushort QNET_DLEN = (UInt16)(QnetConstantes.QNET_MAX_FRAME_LEN - QnetConstantes.QNET_IPP_HDR_LEN);

        public static ushort FIXED_QNET_ADDRESS_COMMS = 57; // 57 = A:0.7.1
    }
    
    /// <summary>
    /// Definition of bit shift values for addresses
    /// </summary>
    public class QnetShift
    {
        // Definition of bit shift values for CLASS_A address parts
        public static Byte CLASS_A_SUBNET_SHIFT = (Byte)0xB;

        public static Byte CLASS_A_STATION_SHIFT = (Byte)0x3;

        public static Byte CLASS_A_IQUBE_SHIFT = (Byte)0x0;

        // Definition of bit shift values for CLASS_C address parts
        public static Byte CLASS_C_SUBNET_SHIFT = (Byte)0xB;

        public static Byte CLASS_C_STATION_SHIFT = (Byte)0x3;

        public static Byte CLASS_C_IQUBE_SHIFT = (Byte)0x0;

        // Definition of bit shift values for CLASS_D address parts
        public static Byte CLASS_D_SUBNET_SHIFT = (Byte)0x8;

        public static Byte CLASS_D_STATION_SHIFT = (Byte)0x0;

        //#define CLASS_D_IQUBE_SHIFT   0x0
    }

    /// <summary>
    /// Definition of bit masks for addresses
    /// </summary>
    public class QnetMask
    {
        // Definition of bit masks for CLASS_A address parts
        public static UInt16 CLASS_A_SUBNET_MASK = (UInt16)0x7800;

        public static UInt16 CLASS_A_STATION_MASK = (UInt16)0x07F8;

        public static UInt16 CLASS_A_IQUBE_MASK = (UInt16)0x0007;

        // Definition of bit masks for CLASS_C address parts
        public static UInt16 CLASS_C_SUBNET_MASK = (UInt16)0x1800;

        public static UInt16 CLASS_C_STATION_MASK = (UInt16)0x07F8;

        public static UInt16 CLASS_C_IQUBE_MASK = (UInt16)0x0007;


        // Definition of bit masks for CLASS_D address parts
        public static UInt16 CLASS_D_SUBNET_MASK = (UInt16)0x0F00;

        public static UInt16 CLASS_D_STATION_MASK = (UInt16)0x00FF;

        //#define CLASS_D_IQUBE_MASK= 0x0


        // Definition of bit masks for CLASS_F address parts
        public static UInt16 CLASS_F_NODE_MASK = 0x00FF;

        // Qnet version Mask
        public static Byte QNET_VERSION_MASK = 0x0F;

        // Qnet type Mask
        public static Byte QNET_TYPE_MASK = 0xF0;
    }

    /// <summary>
    /// Definition of Layer 2 protocol version
    /// </summary>
    public enum QnetVersion : byte
    {
        QNET_VERSION_ASE = 0x00, // Ascom/Fahel ASE-protocol
        QNET_VERSION_STD = 0x01, // Iqube standard protocol
        QNET_VERSION_EXT = 0x01, // Iqube extended protocol
        QNET_VERSION_IPP = 0x01 // Iqube IP tunneling protocol
    }

    // Definition of Layer 2 protocol types
    public enum QnetType : byte
    {
        QNET_TYPE_ASE = 0x00, // Ascom/Fahel ASE-protocol
        QNET_TYPE_STD = 0x10, // Iqube standard protocol
        QNET_TYPE_EXT = 0x20, // Iqube extended protocol
        QNET_TYPE_IPP = 0x30 // Iqube IP tunneling protocol        
    }

    public class QnpConstantes
    {
        public static UInt16 QNP_HEADER_LEN = (UInt16)Marshal.SizeOf(new tQNPhdr());

        public static Byte QNP_VERSION = 1; // QNP Version number

        public static UInt16 QNP_MAX_DLEN = (UInt16)(QnetConstantes.QNET_DLEN - QnpConstantes.QNP_HEADER_LEN);
                             // QNP max. payload length in bytes

        public static Byte QNP_HOPLIMIT = 5; // QNP default hop limit

        public static Byte QNP_PRIORITY = 8; // QNP default datagram priority

        // Definition of protocol numbers
        public static Byte QNP_PT_QCP = 1; // Iqube Control Protocol

        public static Byte QNP_PT_QDP = 2; // Iqube Datagram Protocol

        public static Byte QNP_PT_QTP = 3; // Iqube Transport Protocol

    }

    public enum QnpProtocol : byte
    {
        QNP_PT_QCP = 1, // Iqube Control Protocol
        QNP_PT_QDP = 2, // Iqube Datagram Protocol
        QNP_PT_QTP = 3 // Iqube Transport Protocol
    }

    public class QdpConstantes
    {
        public static Byte QDP_HEADER_LEN = (Byte)Marshal.SizeOf(new tQDPhdr());
    }
    
    /// <summary>
    /// Network Interface number definition
    /// </summary>
    public enum QNETifcNum : sbyte
    {
        QNET_IFC_NONE = -1, // <-1> invalid interface number
        QNET_IFC_0 = 0,

        QNET_IFC_LOCAL = 0, // < 0> local host (pseudo) interface
        QNET_IFC_1 = 1,

        QNET_IFC_PRIMARY = 1, // < 1> primary interface
        QNET_IFC_2 = 2,

        QNET_IFC_SECONDARY = 2, // < 2> secondary interface
        QNET_IFC_3,

        QNET_IFC_4,
        //#ifdef BUILDING_THE_DLL
#if __WIN32__
  QNET_IFC_5,                 // IFC_5..8: Pseudo devices!
  QNET_IFC_6,
  QNET_IFC_7,
  QNET_IFC_8,
#endif
        // BUILDING_THE_DLL
        QNET_IFC_MAX
    }


    public struct tQNETrawHdr
    {
        public Byte ver_typ; // version/type field

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public Byte[] res;
    }


    /// <summary>
    /// Iqube QNET standard protocol header
    /// </summary>
    public struct tQNETstdHdr
    {
        public Byte ver_typ; // version/type field

        public _QNETaddr srcAddr; // network address of source

        public _QNETaddr dstAddr; // network address of destination
    }

    /// <summary>
    /// Iqube QNET extended protocol header
    /// </summary>
    public struct tQNETextHdr
    {
        public Byte ver_typ; // version/type field

        public _QNETaddr srcAddr; // network address of source

        public _QNETaddr dstAddr; // network address of destination

        public Byte devPar1; // communication device parameter 1

        public Byte devPar2; // communication device parameter 2
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct tQNEThdr // QNET protocol type specific header
    {
        [FieldOffset(0)]
        public tQNETrawHdr raw; // QNET raw header

        [FieldOffset(0)]
        public tQNETstdHdr std; // QNET standard protocol header

        [FieldOffset(0)]
        public tQNETextHdr ext; // QNET extended protocol header

        [FieldOffset(0)]
        public tQNETippHdr ipp; // QNET IP tunneling protocol header
    };

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct tQNET // QNET frame structure
    {
        public UInt32 reserved1; // reserved - used for debug memory pool

        public UInt32 reserved2; // reserved

        public _QNETaddr nextHop; // next hop network address

        public QNETifcNum ifcNum; // originating interface number

        public UInt16 dtaLen; // data length

        public Byte devPar1; // communication device parameter 1

        public Byte devPar2; // communication device parameter 2

        public tQNEThdr hdr; // protocol type specific header fop ip tunneling

        public Byte[] dta; // data

        //QnetConst.QNET_DLEN
        public tQNET(UInt16 len)
        {
            dta = new Byte[QnetConstantes.QNET_DLEN];
            reserved1 = 0; // reserved - used for debug memory pool
            reserved2 = 0; // reserved
            nextHop = 0;
            ifcNum = QNETifcNum.QNET_IFC_NONE; // originating interface number
            dtaLen = 0; // data length
            devPar1 = 0; // communication device parameter 1
            devPar2 = 0; // communication device parameter 2
            hdr = new tQNEThdr(); // protocol type specific header fop ip tunneling
        }
    }

    /// <summary>
    /// Definition of QDP header
    /// </summary>
    public struct tQDPhdr // QDP header
    {
        public _QNETport srcPort; // QDP source port

        public _QNETport dstPort; // QDP destination port
    };

    /// <summary>
    /// Definition of QNP header
    /// </summary>
    public struct tQNPhdr // QNP header
    {
        public Byte verprio; // QNP version and datagram priority

        public Byte payload; // QNP payload length

        public Byte nxtheader; // QNP next header value

        public Byte hoplimit; // QNP number of hops (time to live)

        public _QNETaddr srcAddr; // QNP network address of source

        public _QNETaddr dstAddr; // QNP network address of destination
    }

    /// <summary>
    /// Definition of IPP message type
    /// </summary>
    public enum IIPP_MSGTYPE : byte
    {
        IIPP_MSGTYP_NOTIFY = 0x1,

        IIPP_MSGTYP_REQUEST = 0x2,

        IIPP_MSGTYP_RESPONSE = 0x3,

        IIPP_MSGTYP_DATA = 0x4
    }

    /// <summary>
    /// Definition of IPP device type
    /// </summary>
    [FlagsAttribute]
    public enum IIPP_DEVTYPE : byte
    {
        IIPP_DEVTYP_IQUBE = 0x01,

        IIPP_DEVTYP_DETECTOR = 0x02,

        IIPP_DEVTYP_LSA = 0x04,

        IIPP_DEVTYP_VCU = 0x08,

        IIPP_DEVTYP_OMC = 0x10,

        IIPP_DEVTYP_OMC_MOB = 0x20,
        //#define IIPP_DEVTYP_RES1   = 0x40,
        //#define IIPP_DEVTYP_RES2   = 0x80,
        IIPP_DEVTYP_ALL = 0xFF
    }

    public enum InfoLineFlags : byte
    {
        /// <summary>
        /// None = 0
        /// </summary>
        None = 0,

        /// <summary>
        /// Alternate = 1
        /// </summary>
        Alternate = 1
    }
}
