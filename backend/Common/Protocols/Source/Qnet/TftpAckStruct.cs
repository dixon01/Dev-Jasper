// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftpAckStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Structure of TftpAck for mail . (Legacy C code = tTFTPack)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Structure of TftpAck for mail . (Legacy C code = tTFTPack)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TftpAckStruct
    {
        /// <summary>
        /// BlockNumber to acknowledge (lagacy code = blockNum)
        /// </summary>
        public ushort BlockNumber;      
    }
}
