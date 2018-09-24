// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QmailStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines QmailStruct
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines the qmail message structure (tQMAIL from legacy code)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct QmailStruct
    {
        /// <summary>
        /// Qmail header
        /// </summary>
        public QmailHeader Hdr;

        /// <summary>
        /// Qmail data
        /// </summary>
        public fixed byte Dta[MessageConstantes.TftpMaxDataLength];
    }

    /// <summary>
    /// Header of the Qmail message (legacy code tQMAILhdr)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct QmailHeader
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
        /// Originating time of mail message
        /// </summary>
        public DOSTimeStruct OriginateTme;

        /// <summary>
        /// Filename of attached file (or NUL)
        /// </summary>
        public fixed byte NameStr[MessageConstantes.QmailMaxFNameSize]; 
        
        /// <summary>
        /// Must be zero
        /// </summary>
        public char Zero; 
    }
}
