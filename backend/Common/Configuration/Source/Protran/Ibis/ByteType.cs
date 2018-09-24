// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ByteType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    /// <summary>
    /// Supported byte types for IBIS protocol.
    /// </summary>
    public enum ByteType
    {
        /// <summary>
        /// 7-bit ASCII encoding using one byte for each 7 bit character
        /// </summary>
        Ascii7,

        /// <summary>
        /// 8-bit encoding sometimes using 8 bits ASCII, sometimes 16 bits as
        /// both little and big endian Unicode.
        /// </summary>
        Hengartner8,

        /// <summary>
        /// 16-bit big endian Unicode using two bytes for each 16 bit character
        /// </summary>
        UnicodeBigEndian
    }
}
