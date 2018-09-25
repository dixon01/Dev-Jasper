// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftpMailStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Structure of TFTP mail . (Legacy C code = tTFTPmsg)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Structure of TFTP mail . (Legacy C code = tTFTPmail)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct TftpMailStruct
    {
        /// <summary>
        /// Source iqbue address
        /// </summary>
        public ushort SrcAddr;

        /// <summary>
        /// Destination iqbue address 
        /// </summary>
        public ushort DstAddr;

        /// <summary>
        /// Destination port number
        /// </summary>
        public byte DstPort;

        /// <summary>
        /// Data length
        /// </summary>
        public ushort DtaLen;

        /// <summary>
        /// Filename of attached file (or NUL)
        /// </summary>
        public fixed byte NameStr[MessageConstantes.QmailMaxFNameSize]; 

        /// <summary>
        /// Must be zero
        /// </summary>
        public byte Zero;

        /// <summary>
        /// Data contains into the mail
        /// </summary>
        public fixed byte Data[MessageConstantes.TftpMaxDataLength];
    }
}
