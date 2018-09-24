using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace Gorba.Common.Protocols.Qnet
{
    #region alias declaration
    // These aliases enable to make the // with qnet definitions. 
    // This section should be duplicated into each file that needs these aliases !!!
    // see Gorba.Common.Protocols.Qnet.QnetEnums.cs
    //using tSOCKET = System.Int16;

    using NLog;

    using _QNETport = System.Byte;

    // typedef WORD tQNETaddr;       
    // Iqube network address
    using _QNETaddr = System.UInt16;

    // typedef BYTE tQNETport;       
    // Iqube protocol port (mapped to taskId)    
    using _QnetPort = System.Byte;
    #endregion

    /// <summary>
    /// this class represent a Qnet Address.
    /// To get create a connection, the connectionManager needs a QnetAddress object.
    /// </summary>
    public class QnetAddress
    {
        private static readonly Logger Logger = LogManager.GetLogger("GlobalLog");

        private UInt16 m_Subnet;
        public UInt16 Subnet
        {
            get { return GetSubnet(); }
        }
        private UInt16 m_Station;
        public UInt16 Station
        {
            get { return GetStation(); }
        }
        private UInt16 m_Iqube;
        public UInt16 Iqube
        {
            get { return GetIqube(); }
        }

        private UInt16 m_Node;
        /// <summary>
        /// Represents the Node part of a Class F address.
        /// </summary>
        public UInt16 Node
        {
            get { return GetNode(); }
        }

        private QnetClass m_QnetAddressClass;


        private tSOCKaddr m_SOCKAddr;
        public tSOCKaddr SOCKAddr
        {
            get { return m_SOCKAddr; }
        }

        /// <summary>
        /// Represents the qnet address of the remote iqube as formatted string like 2.3.1
        /// </summary>
        private string m_DottedAddress;

        public string DottedAddress
        {
            get { return m_DottedAddress; }
            set { SetDottedAddress(value); }
        }


        private _QNETaddr m_QNETAddr;

        public _QNETaddr QNETAddr
        {
            get { return m_QNETAddr; }
            set { SetQNETAddr(value); }
        }


        /// <summary>
        /// Converts the string representation of a qnet addree to its QnetAddress equivalent. 
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">Type: System.String
        /// A string containing a number to convert. </param>
        /// <param name="result">Type: QnetAddress
        /// When this method returns, contains the QnetAddress value equivalent to the number contained in s, 
        /// if the conversion succeeded, or null if the conversion failed. 
        /// The conversion fails if the s parameter is null, is not of the correct format.This parameter is passed null.</param>
        /// <returns>Type: System.Boolean
        /// True if s was converted successfully; otherwise, false.</returns>
        static public bool TryParse(string s, out QnetAddress result)
        {
            try {
                result = new QnetAddress(s);
            } catch(Exception exception) {
                Logger.ErrorException(string.Format("Can't parse the address '{0}", s), exception);
                result = null;
                return false;
            } // try

            return true;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public QnetAddress()
        {
            m_QNETAddr = QnetConstantes.QnetAddrNone;
            m_QnetAddressClass = QnetClass.QNETClassNone;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="addr">Type: _QNETaddr = ushort
        /// <para>The qnet address as a ushort. The dotted address is set according to the given 'addr' parameter.</para></param>
        public QnetAddress(_QNETaddr addr)
        {
            SetQNETAddr(addr);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="remoteQnetAddress">Type: System.String
        /// <para>The qnet Address as string. Format of qnet address is like : D:0.0, F:0 or A:2.3.1</para></param>
        public QnetAddress(string qnetAddress)
        {
            m_QNETAddr = QnetConstantes.QnetAddrNone;

            if (string.IsNullOrEmpty(qnetAddress.Trim())) {
                throw new Exception("QnetAddress constructor : The remote qnet address must be assigned and not empty.");
            } // if

            if (qnetAddress.Length < 3) {
                throw new Exception("QnetAddress constructor : The remote qnet address is to short: " + qnetAddress);
            } // if


            /*
             * this.rPort = remotePort;
            this.lport = localPort;
            this.type = type;
             * */
            // string Addr = address;

            //Console.WriteLine("Remote Port: " + getRemotePort());
            //this.remoteAddress.port = getRemotePort();
            SetDottedAddress(qnetAddress);
        }      

        /// <summary>
        /// Give the Address as Human readable String back
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_DottedAddress;
        }

        /// <summary>
        /// Returns the Subnet part from the full address
        /// Example: C:X.Y.Z
        /// Subnet is X value
        /// </summary>
        /// <returns></returns>
        private UInt16 GetSubnet() { return m_Subnet; }
        /// <summary>
        /// Returns the station part from the full address
        /// Example: C:X.Y.Z
        /// Station is Y value
        /// </summary>
        /// <returns></returns>
        private UInt16 GetStation() { return m_Station; }
        /// <summary>
        /// Returns the iqube part part from the full address
        /// Example: C:X.Y.Z
        /// Iqube is Z value
        /// </summary>
        /// <returns></returns>
        private UInt16 GetIqube() { return m_Iqube; }
        /// <summary>
        /// Returns the net class part from the full address
        /// Example: C:X.Y.Z
        /// Class is C value
        /// </summary>
        /// <returns></returns>
        private char GetClass() 
        {
            switch (m_QnetAddressClass) {
                case QnetClass.QNETClassA:
                    return 'A';
                case QnetClass.QNETClassB:
                    return 'B';
                case QnetClass.QNETClassC:
                    return 'C';
                case QnetClass.QNETClassD:
                    return 'D';
                case QnetClass.QNETClassE:
                    return 'E';
                case QnetClass.QNETClassF:
                    return 'F';
                default:
                    return '?';
            } // switch     
        }

        /// <summary>
        /// Gets Node. only for F qnet class
        /// </summary>
        /// <returns></returns>
        public UInt16 GetNode() 
        {
            if (m_QnetAddressClass != QnetClass.QNETClassF) {
                throw new Exception("Node is a property only available for 'F' Qnet address class ");
            }  // if
            return m_Node;  
        }

        /// <summary>
        /// Set m_QnetAddressClass and m_QnetAddressClassLetter according to qnetAddressClassLetter parameter.
        /// </summary>
        /// <param name="qnetAddressClassLetter">Letter that represents the class of qnet address.</param>
        private void SetQnetAddressClass(char qnetAddressClassLetter)
        {
            m_QnetAddressClass = QnetClass.QNETClassNone;

            switch (qnetAddressClassLetter) {
                case 'A':
                    m_QnetAddressClass = QnetClass.QNETClassA;
                    break;
                case 'B':
                    m_QnetAddressClass = QnetClass.QNETClassB;
                    break;
                case 'C':
                    m_QnetAddressClass = QnetClass.QNETClassC;
                    break;
                case 'D':
                    m_QnetAddressClass = QnetClass.QNETClassD;
                    break;
                case 'E':
                    m_QnetAddressClass = QnetClass.QNETClassE;
                    break;
                case 'F':
                    m_QnetAddressClass = QnetClass.QNETClassF;
                    break;
                default:
                    throw new Exception("Invalid or not supported QNET_CLASS in this constructor: " + m_DottedAddress);
            } // switch
        }



        /// <summary>
        /// Set the m_DottedAddress field (format like A:2.1.0). 
        /// </summary>
        /// <param name="remoteQnetAddress"></param>
        private void SetDottedAddress(string remoteQnetAddress)
        {
            m_DottedAddress = remoteQnetAddress;
            SetQnetAddressClass(remoteQnetAddress[0]);            
            string[] address2 = remoteQnetAddress.Substring(2).Split('.');
            if (address2.Length < 2 || address2.Length > 3) {
                throw new Exception("Invalid format address :" + remoteQnetAddress);
            } // if
            
            // SUBNET
            this.m_Subnet = UInt16.Parse(address2[0]);
            // STATION
            if (address2.Length >= 2) {
                this.m_Station = UInt16.Parse(address2[1]);
            } else {
                this.m_Station = 0;    
            } // else
            // IQUBE            
            if (address2.Length == 3) {
                this.m_Iqube = UInt16.Parse(address2[2]);
            } else {
                this.m_Iqube = 0;    
            } // else

            UpdateQNETAddr();
            UpdateSOCKAddr();
        }

        /// <summary>
        /// Update  the m_QNETaddr from the Subnet, station and Iqube values. 
        /// The QnetAddr is the representation of the remote qnet address in WORD format.
        /// </summary>
        /// <returns></returns>
        private void UpdateQNETAddr()
        {
            if ((m_Subnet <= QnetConstantes.QNET_MAX_SUBNET) && (m_Station <= QnetConstantes.QNET_MAX_STATION) &&
               ((m_Iqube <= QnetConstantes.QNET_MAX_IQUBE) || (m_QnetAddressClass == QnetClass.QNETClassD))) {
                switch (m_QnetAddressClass) {
                    case QnetClass.QNETClassA:
                        m_QNETAddr = (UInt16)(((m_Subnet << QnetShift.CLASS_A_SUBNET_SHIFT) & QnetMask.CLASS_A_SUBNET_MASK) |
                            (UInt16)((m_Station << QnetShift.CLASS_A_STATION_SHIFT) & QnetMask.CLASS_A_STATION_MASK) |
                            (UInt16)((m_Iqube << QnetShift.CLASS_A_IQUBE_SHIFT) & QnetMask.CLASS_A_IQUBE_MASK));
                        break;

                    //      case QNET_CLASS_B:  // currently not supported!
                    //        break;

                    case QnetClass.QNETClassC:
                        m_QNETAddr = (UInt16)(((m_Subnet << QnetShift.CLASS_C_SUBNET_SHIFT) & QnetMask.CLASS_C_SUBNET_MASK) |
                               ((m_Station << QnetShift.CLASS_C_STATION_SHIFT) & QnetMask.CLASS_C_STATION_MASK) |
                               ((m_Iqube << QnetShift.CLASS_C_IQUBE_SHIFT) & QnetMask.CLASS_C_IQUBE_MASK) | 0xC000);
                        break;

                    case QnetClass.QNETClassD:
                        m_QNETAddr = (UInt16)(((m_Subnet << QnetShift.CLASS_D_SUBNET_SHIFT) & QnetMask.CLASS_D_SUBNET_MASK) |
                               ((m_Station << QnetShift.CLASS_D_STATION_SHIFT) & QnetMask.CLASS_D_STATION_MASK) | 0xE000);
                        break;

                    //      case QNET_CLASS_E:  // currently not supported!
                    //        break;

                    default:
                        m_QNETAddr = QnetConstantes.QnetAddrNone;
                        m_QnetAddressClass = QnetClass.QNETClassNone;
                        break;
                } // switch
            } else {
                m_QNETAddr = QnetConstantes.QnetAddrNone;
                m_QnetAddressClass = QnetClass.QNETClassNone;
            } // else
        }


        /// <summary>
        /// Update m_SOCKAddr from m_QnetAddr
        /// </summary>
        private void UpdateSOCKAddr()
        {
            m_SOCKAddr = new tSOCKaddr();
            m_SOCKAddr.adr1 = (byte)(m_QNETAddr & (ushort)0xff);
            m_SOCKAddr.adr2 = (byte)((m_QNETAddr & (ushort)0xff00) >> 8);
        }


        /// <summary>
        /// Set the QNETAddr and update QnetClass and class letter, Subnet, iqube and node from it.
        /// </summary>
        /// <param name="addr">Remote iqube address.</param>
        private void SetQNETAddr(_QNETaddr addr)
        {
            m_QNETAddr = addr;
            UpdateQnetClassFromQNEtAddr();
            UpdateSubnetFromQNEtAddr();
            UpdateStationFromQNEtAddr();
            UpdateIqubeFromQNEtAddr();
            UpdateNodeFromQNEtAddr();
            UpdateDottedAddressFromQNEtAddr();            
        }

        /// <summary>
        /// Update the Dotted address property from m_QNETAddr field.
        /// </summary>
        private void UpdateDottedAddressFromQNEtAddr()
        {
            switch (m_QnetAddressClass) {                   
                case  QnetClass.QNETClassA:
                case QnetClass.QNETClassC:
                    m_DottedAddress = string.Format("{0}:{1}.{2}.{3}", GetClass(), m_Subnet, m_Station, m_Iqube);
                    break;
                case QnetClass.QNETClassD:
                    m_DottedAddress = string.Format("{0}:{1}.{2}", GetClass(), m_Subnet, m_Station);
                    break;
                case QnetClass.QNETClassF:
                    m_DottedAddress = string.Format("{0}:{1}", GetClass(), m_Node);
                    break;
                default:
                    m_DottedAddress = "CLASS UNKNOWN";
                    break;
            } // switch
        }


        /// <summary>
        /// Update the QnetAddressClass property from m_QNETAddr.
        /// </summary>
        private void UpdateQnetClassFromQNEtAddr()
        {
            if (m_QNETAddr == QnetConstantes.QnetAddrNone) {
                m_QnetAddressClass = QnetClass.QNETClassNone;
            } else
                /* QNET Class A address */
                if (((m_QNETAddr) & 0x8000) == 0x0000) {
                    m_QnetAddressClass = QnetClass.QNETClassA;
                } else
                    /* QNET Class C address */
                    if (((m_QNETAddr) & 0xE000) == 0xC000) {
                        m_QnetAddressClass = QnetClass.QNETClassC;
                    } else
                        /* QNET Class D address */
                        if (((m_QNETAddr) & 0xF000) == 0xE000) {
                            m_QnetAddressClass = QnetClass.QNETClassD;
                        } else
                            /* QNET Class F address */
                            if (((m_QNETAddr) & 0xFF00) == 0xF000) {
                                m_QnetAddressClass = QnetClass.QNETClassF;
                            } else {
                                m_QnetAddressClass = QnetClass.QNETClassNone;
                            } // else
        }

        /// <summary>
        /// Update the m_Subnet property from m_QNETAddr according to the qnet class of qnet address
        /// </summary>
        private void UpdateSubnetFromQNEtAddr()
        {
            m_Subnet = QnetConstantes.QnetAddrAny;
            switch (m_QnetAddressClass) {
                case QnetClass.QNETClassA:
                    m_Subnet = (_QNETaddr)((m_QNETAddr & QnetMask.CLASS_A_SUBNET_MASK) >> QnetShift.CLASS_A_SUBNET_SHIFT);
                    break;
                //  case QnetClass.QNET_CLASS_B: m_Subnet = (tQNETaddr)((m_QNETAddr & QnetMask.CLASS_B_SUBNET_MASK) >> QnetShift.CLASS_B_SUBNET_SHIFT); break; // currently not supported!
                case QnetClass.QNETClassC:
                    m_Subnet = (_QNETaddr)((m_QNETAddr & QnetMask.CLASS_C_SUBNET_MASK) >> QnetShift.CLASS_C_SUBNET_SHIFT);
                    break;
                case QnetClass.QNETClassD:
                    m_Subnet = (_QNETaddr)((m_QNETAddr & QnetMask.CLASS_D_SUBNET_MASK) >> QnetShift.CLASS_D_SUBNET_SHIFT);
                    break;
                //  case QnetClass.QNET_CLASS_E: m_Subnet = (tQNETaddr)((m_QNETAddr & QnetMask.CLASS_E_SUBNET_MASK) >> QnetShift.CLASS_E_SUBNET_SHIFT); break; // currently not supported!
                case QnetClass.QNETClassF:
                    m_Subnet = 0; // special case -> needed for routing!!!
                    break;
                default:
                    break;
            } // switch
        }


        /// <summary>
        /// Update the m_Station field from m_QNETAddr according to the qnet class of qnet address
        /// </summary>
        private void UpdateStationFromQNEtAddr()
        {
            m_Station = QnetConstantes.QnetAddrAny;

            switch (m_QnetAddressClass) {
                case QnetClass.QNETClassA: m_Station = (_QNETaddr)((m_QNETAddr & QnetMask.CLASS_A_STATION_MASK) >> QnetShift.CLASS_A_STATION_SHIFT); break;
                //  case QnetClass.QNET_CLASS_B: m_Station = (_QNETaddr)((m_QNETAddr & QnetMask.QnetMask.CLASS_B_STATION_MASK) >> QnetShift.CLASS_B_STATION_SHIFT); break; // currently not supported!
                case QnetClass.QNETClassC: m_Station = (_QNETaddr)((m_QNETAddr & QnetMask.CLASS_C_STATION_MASK) >> QnetShift.CLASS_C_STATION_SHIFT); break;
                case QnetClass.QNETClassD: m_Station = (_QNETaddr)((m_QNETAddr & QnetMask.CLASS_D_STATION_MASK) >> QnetShift.CLASS_D_STATION_SHIFT); break;
                //  case QnetClass.QNET_CLASS_E: m_Station = (_QNETaddr)((addr & QnetMask.CLASS_E_STATION_MASK) >> QnetShift..CLASS_E_STATION_SHIFT); break; // currently not supported!
                default:
                    break;
            }
        }

        /// <summary>
        /// Update the m_Iqube field from m_QNETAddr according to the qnet class of qnet address
        /// </summary>
        private void UpdateIqubeFromQNEtAddr()
        {
            m_Iqube = QnetConstantes.QnetAddrAny;
            switch (m_QnetAddressClass) {
                case QnetClass.QNETClassA: m_Iqube = (_QNETaddr)(m_QNETAddr & QnetMask.CLASS_A_IQUBE_MASK); break;
                //  case QnetClass.QNET_CLASS_B: m_Iqube = (tQNETaddr)(m_QNETAddr & QnetMask.CLASS_B_IQUBE_MASK); break; // currently not supported!
                case QnetClass.QNETClassC: m_Iqube = (_QNETaddr)(m_QNETAddr & QnetMask.CLASS_C_IQUBE_MASK); break;
                //    case QNET_CLASS_D:  // NO iqube part in class D addresses!
                //  case QNET_CLASS_E:  // currently not supported!
                default:
                    break;
            }
        }

        /// <summary>
        /// Update the m_Iqube field from m_QNETAddr according to the qnet class of qnet address
        /// </summary>
        private void UpdateNodeFromQNEtAddr()
        {
            m_Node = QnetConstantes.QnetAddrAny;
            switch (m_QnetAddressClass) {
                case QnetClass.QNETClassF: 
                    m_Node = (_QNETaddr)(m_QNETAddr & QnetMask.CLASS_F_NODE_MASK); break;
                default:
                    break;
            } // switch
        }
    }
}