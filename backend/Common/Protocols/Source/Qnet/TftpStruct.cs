// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftpStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TftpStruct type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// (Legacy C code = tTFTPmsg)
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TftpStruct
    {
        /// <summary>
        /// TFTP operation code. Enable to select betwen the different structures of the union.
        /// </summary>
        [FieldOffset(0)]
        public byte OperationCode;

        /// <summary>
        /// Structure that takes part of union for tftp mail operation
        /// </summary>
        [FieldOffset(1)]
        public TftpMailStruct TftpMail;

        /// <summary>
        /// Structure that takes part of union for acknowledge the frames of tftp protocol
        /// </summary>
        [FieldOffset(1)]
        public TftpAckStruct TftpAck;

        /*
   * union u
  {
    tTFTPrenameRqs  renameRqs;
    tTFTPclntRqs    clntRqs;
    tTFTPrqs        rqs;
    tTFTPdta        dta;
    tTFTPack        ack; // done
    tTFTPerr        err;
    tTFTPfsta       fsta;
    tTFTPmail       mail; // done
  }
   */
    }
}