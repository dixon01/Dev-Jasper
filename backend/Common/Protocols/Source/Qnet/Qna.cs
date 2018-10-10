// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Qna.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Linq;

    /// <summary>
    /// Class used to code and decode qnet datagram 
    /// Implementation of IQUBE Network Access Protocol (Layer 2).
    /// </summary>
    internal class Qna
    {
        /*
          To calculate the CRC-16 checksum, the following table is used.
        */
        /*
            CRC - 16 : x^16 + x^15 + x^2 + 1
        */
        #region Constants and Fields

        private readonly ushort[] crcTable = new ushort[]
            {
                0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241, 0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 
                0xC5C1, 0xC481, 0x0440, 0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40, 0x0A00, 0xCAC1, 
                0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841, 0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 
                0x1A40, 0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41, 0x1400, 0xD4C1, 0xD581, 0x1540, 
                0xD701, 0x17C0, 0x1680, 0xD641, 0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040, 0xF001, 
                0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240, 0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 
                0x3480, 0xF441, 0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41, 0xFA01, 0x3AC0, 0x3B80, 
                0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840, 0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41, 
                0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40, 0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 
                0xE7C1, 0xE681, 0x2640, 0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041, 0xA001, 0x60C0, 
                0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240, 0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 
                0xA441, 0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41, 0xAA01, 0x6AC0, 0x6B80, 0xAB41, 
                0x6900, 0xA9C1, 0xA881, 0x6840, 0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41, 0xBE01, 
                0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40, 0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 
                0xB681, 0x7640, 0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041, 0x5000, 0x90C1, 0x9181, 
                0x5140, 0x9301, 0x53C0, 0x5280, 0x9241, 0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440, 
                0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40, 0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 
                0x59C0, 0x5880, 0x9841, 0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40, 0x4E00, 0x8EC1, 
                0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41, 0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 
                0x8641, 0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
            };

        private readonly int maxLen = QnetConstantes.QNET_DLEN;

        private ushort crc16;

        private byte lastChar;

        /// <summary>
        /// Data lenght = total - header
        /// </summary>
        private int receivDtaLen;

        /// <summary>
        /// Header length
        /// </summary>
        private int receivHdrLen = QnetConstantes.QNET_IPP_HDR_LEN;

        private int receivIndex;

        /// <summary>
        /// // total length
        /// </summary>
        private int receivLength;

        private RxState receivState = RxState.RxPause;

        private byte receivTyp;

        private byte receivVer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Qna"/> class.
        /// </summary>
        public Qna()
        {
            this.QnetInit();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the datagram length.
        /// </summary>
        public int DatagramLength
        {
            get
            {
                return this.GetDatagramLength();
            }
        }

        /// <summary>
        /// Gets the data of the datagram.
        /// </summary>
        public byte[] Dta { get; private set; }

        /// <summary>
        /// Gets the header of the datagram.
        /// </summary>
        public byte[] Hdr { get; private set; }

        /// <summary>
        /// Gets the raw datagramm.
        /// </summary>
        public byte[] RawDatagramm { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle receive data.
        /// </summary>
        /// <param name="receivedData">
        /// The received data.
        /// </param>
        /// <returns>
        /// The handled receive data.
        /// </returns>
        public short HandleReceiveData(byte receivedData)
        {
            short ret = 0;

            if (this.receivState == RxState.RxReinit)
            {
                this.QnetInit();
            }

            // Decode datagram
            if (receivedData == ReservedCharacters.SOH)
            {
                this.lastChar = 0;
                this.receivState = RxState.RxLength;
                return 0;
            }

            if (receivedData == ReservedCharacters.DLE)
            {
                this.lastChar = ReservedCharacters.DLE;
                return 0;
            }

            if (this.lastChar == ReservedCharacters.DLE)
            {
                this.lastChar = 0;
                if (this.IsSpecialChar(receivedData))
                {
#if QNET_PROTOCOL_DEBUG
                            Console.WriteLine("QNA_recvFrame(): Unexpected char after DLE <0x{0:X}>", m_LastChar);
#endif
                    this.receivState = RxState.RxPause;
                    return 0;
                }

                receivedData = (byte)(receivedData ^ QnetConstantes.QNET_XOR_VALUE);
            }

            switch (this.receivState)
            {
                case RxState.RxPause:
                    break;

                case RxState.RxLength:
                    this.receivLength = receivedData & 0xFF;
                    if (this.receivLength < QnetConstantes.RX_MIN_LENGTH)
                    {
#if QNET_PROTOCOL_DEBUG
            Console.WriteLine("QNA_recvFrame(): RxLength <{0}> < minLength", m_RxLength);
#endif
                        ret = QnaErrors.ErrorQnaRxMinLength;
                    }
                    else
                    {
#if QNET_PROTOCOL_DEBUG

                        // Console.WriteLine("QNA_recvFrame(): RxLength=%hu\n",RxLength);
#endif
                        this.receivState = RxState.RxVerTyp;
                    }

                        // else
                    break;

                case RxState.RxVerTyp:
                    this.receivVer = (byte)(receivedData & QnetMask.QNET_VERSION_MASK);
                    this.receivTyp = (byte)(receivedData & QnetMask.QNET_TYPE_MASK);

                    // We handle only IPP type
                    if (this.receivTyp == (byte)QnetType.QNET_TYPE_IPP)
                    {
                        if (this.receivVer != (byte)QnetVersion.QNET_VERSION_IPP)
                        {
                            ret = QnaErrors.ErrorQnaRxVersion;
                        }
                    }
                    else
                    {
                        ret = QnaErrors.ErrorQnaRxVersion;
                    }

                    if (ret >= 0)
                    {
                        this.receivDtaLen = this.receivLength - this.receivHdrLen;
                        if (this.receivDtaLen > this.maxLen)
                        {
#if QNET_PROTOCOL_DEBUG
                            Console.WriteLine("QNA_recvFrame(): RxDtaLen <{0}> exceeds maxLen <{1}>", m_RxDtaLen, m_MaxLen);
#endif
                            ret = QnaErrors.ErrorQnaRxMaxDataLength;
                        }
                        else
                        {
#if QNET_PROTOCOL_DEBUG
                            Console.WriteLine("QNA_recvFrame(): RxVerTyp=0x{0:X}  RxHdrLen={1}  RxDtaLen={2}", m_RxVer + m_RxTyp, m_RxHdrLen, m_RxDtaLen);
#endif
                            this.CrcStep(receivedData);
                            this.receivIndex = 0;
                            this.Hdr[this.receivIndex++] = receivedData;
                            this.receivState = RxState.RxHeader;
                        }
                    }

                    break;

                case RxState.RxHeader:
                    this.CrcStep(receivedData);
                    this.Hdr[this.receivIndex] = receivedData;
                    if (++this.receivIndex >= this.receivHdrLen)
                    {
                        this.receivIndex = 0;
                        if (this.receivDtaLen > 0)
                        {
                            this.Dta = new byte[this.receivDtaLen];
                            this.receivState = RxState.RxData;
                        }
                        else
                        {
                            this.receivState = RxState.RxCrcLow;
                        }
                    }

                    break;

                case RxState.RxData:
                    this.CrcStep(receivedData);
                    this.Dta[this.receivIndex++] = receivedData;
                    if (this.receivIndex >= this.receivDtaLen)
                    {
                        this.receivState = RxState.RxCrcLow;
                    }

                    break;

                case RxState.RxCrcLow:
                    this.CrcStep(receivedData);
                    this.receivState = RxState.RxCrcHigh;
                    break;

                case RxState.RxCrcHigh:
                    this.CrcStep(receivedData);
                    if (this.crc16 == 0x0)
                    {
#if QNET_PROTOCOL_DEBUG
                        Console.WriteLine("QNA_recvFrame(): Frame received RxVerTyp={0}  RxLength={1}", m_RxVer + m_RxTyp, m_RxLength);
#endif
                        ret = (short)this.receivLength;

                        // initialize state for next char handling
                        this.receivState = RxState.RxReinit;
                    }
                    else
                    {
#if QNET_PROTOCOL_DEBUG
                        Console.WriteLine("QNA_recvFrame(): Wrong CRC <0x{0:X}>", m_CRC16);
#endif
                        ret = QnaErrors.ErrorQnaRxCrc;
                    }

                    break;

                default:
#if QNET_PROTOCOL_DEBUG
                    Console.WriteLine("QNA_recvFrame(): Unknown State <{0}>", m_RxState);
#endif
                    ret = QnaErrors.ErrorQnaRxState;
                    break;
            }

            if (ret < 0)
            {
                this.QnetInit();
            }

            return ret;
        }

        /// <summary>
        /// Prepare Frame and call the OnSendFrame event if it is assigned.
        /// </summary>
        /// <param name="qnetHdr">
        /// The qnet Hdr.
        /// </param>
        /// <param name="qnetData">
        /// The qnet Data.
        /// </param>
        /// <returns>
        /// The prepare qnet datagramm.
        /// </returns>
        public short PrepareQnetDatagramm(byte[] qnetHdr, byte[] qnetData)
        {
            var transmitBuf = new byte[QnetConstantes.TXBUF_BUF_LEN];

            int totLen = qnetHdr.Length + qnetData.Length;
            int transmitIndex = 0;
            ushort crc = 0;

            this.RawDatagramm = null;

            if (totLen > QnetConstantes.QNET_MAX_FRAME_LEN)
            {
#if QNET_PROTOCOL_DEBUG
                Console.WriteLine("QNA_sendFrame(): totLen <%hu> exceeds max. frame length <%i>\n", totLen, QnetConstantes.QNET_MAX_FRAME_LEN);
#endif
                return QnaErrors.ErrorQnaFrameLength;
            }

            // --- Start Of Header
            transmitBuf[transmitIndex++] = ReservedCharacters.SOH;

            // --- frame length
            if (this.IsSpecialChar((byte)totLen))
            {
                transmitBuf[transmitIndex++] = ReservedCharacters.DLE; // insert Data Link Escape
                transmitBuf[transmitIndex++] = (byte)(totLen ^ QnetConstantes.QNET_XOR_VALUE);
            }
            else
            {
                transmitBuf[transmitIndex++] = (byte)totLen;
            }

                // else

            // --- frame header
            crc = this.CrcRegion(crc, qnetHdr);
            for (int idx = 0; idx < qnetHdr.Length; idx++)
            {
                // printf("QNA_sendFrame(): HEADER 0x%hx\n",(WORD)*pData);
                if (this.IsSpecialChar(qnetHdr[idx]))
                {
                    transmitBuf[transmitIndex++] = ReservedCharacters.DLE; // insert Data Link Escape
                    transmitBuf[transmitIndex++] = (byte)(qnetHdr[idx] ^ QnetConstantes.QNET_XOR_VALUE);
                }
                else
                {
                    transmitBuf[transmitIndex++] = qnetHdr[idx];
                }

                    // else
            }

            // --- frame data
            if (qnetData.Length > 0)
            {
                crc = this.CrcRegion(crc, qnetData);
                for (int idx = 0; idx < qnetData.Length; idx++)
                {
                    // printf("QNA_sendFrame(): DATA 0x%hx\n",(WORD)*pData);
                    if (this.IsSpecialChar(qnetData[idx]))
                    {
                        transmitBuf[transmitIndex++] = ReservedCharacters.DLE; // insert Data Link Escape
                        transmitBuf[transmitIndex++] = (byte)(qnetData[idx] ^ QnetConstantes.QNET_XOR_VALUE);
                    }
                    else
                    {
                        transmitBuf[transmitIndex++] = qnetData[idx];
                    }

                        // else
                }
            }

            // --- crc low byte
            if (this.IsSpecialChar(ByteAccess.LoByte(crc)))
            {
                transmitBuf[transmitIndex++] = ReservedCharacters.DLE; // insert Data Link Escape
                transmitBuf[transmitIndex++] = (byte)(ByteAccess.LoByte(crc) ^ QnetConstantes.QNET_XOR_VALUE);
            }
            else
            {
                transmitBuf[transmitIndex++] = ByteAccess.LoByte(crc);
            }

            // --- crc high byte
            // printf("QNA_sendFrame(): CRC_HIGH 0x%hx\n",(WORD)HIBYTE(crc16));
            if (this.IsSpecialChar(ByteAccess.HiByte(crc)))
            {
                transmitBuf[transmitIndex++] = ReservedCharacters.DLE; // insert Data Link Escape
                transmitBuf[transmitIndex++] = (byte)(ByteAccess.HiByte(crc) ^ QnetConstantes.QNET_XOR_VALUE);
            }
            else
            {
                transmitBuf[transmitIndex++] = ByteAccess.HiByte(crc);
            }

            this.RawDatagramm = new byte[transmitIndex];
            Array.Copy(transmitBuf, this.RawDatagramm,  transmitIndex);
            return (short)transmitIndex;
        }

        /// <summary>
        /// Initialize qnet informations that enables to decode qnet datagramm
        /// </summary>
        public void QnetInit()
        {
            this.lastChar = 0;
            this.receivState = RxState.RxPause;
            this.receivVer = 0;
            this.receivTyp = 0;
            this.receivLength = 0;

            // Hard coding because currently we handle only IPP type !
            this.receivHdrLen = QnetConstantes.QNET_IPP_HDR_LEN;

            this.Hdr = new byte[QnetConstantes.QNET_IPP_HDR_LEN];
            this.receivDtaLen = 0;
            this.receivIndex = 0;
            this.crc16 = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculate the new CRC16 according to the new byte
        /// </summary>
        /// <param name="newByte">
        /// Specified byte to calculate the new crc 16
        /// </param>
        private void CrcStep(byte newByte)
        {
            this.crc16 =
                (ushort)(((ushort)(this.crc16 / 0x100)) ^ this.crcTable[newByte ^ ((byte)(this.crc16 % 0x100))]);
        }

        private ushort CrcRegion(ushort lastResult, byte[] data)
        {
            return data.Aggregate(
                lastResult, (current, t) => (ushort)((current / 0x100) ^ this.crcTable[t ^ (current % 0x100)]));
        }

        private int GetDatagramLength()
        {
            int ret = 0;
            if (this.Hdr != null)
            {
                ret += this.Hdr.Length;
            }

            if (this.Dta != null)
            {
                ret += this.Dta.Length;
            }

            return ret;
        }

        /// <summary>
        /// Check for special (reserved) characters.
        /// </summary>
        /// <param name="ch">
        /// Character to check.
        /// </param>
        /// <returns>
        /// True / false
        /// </returns>
        private bool IsSpecialChar(byte ch)
        {
            return (ch == ReservedCharacters.SOH) || (ch == ReservedCharacters.ENQ) || (ch == ReservedCharacters.DLE);
        }

        #endregion
    }
}