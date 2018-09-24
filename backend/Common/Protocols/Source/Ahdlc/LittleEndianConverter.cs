// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LittleEndianConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LittleEndianConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    /// <summary>
    /// Converter helper class for little endian byte arrays.
    /// </summary>
    internal static class LittleEndianConverter
    {
        /// <summary>
        /// Converts the 2 bytes at the given <see cref="startIndex"/> to an <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <returns>
        /// The calculated <see cref="ushort"/>.
        /// </returns>
        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            return (ushort)((value[startIndex] << 8) | value[startIndex + 1]);
        }

        /// <summary>
        /// Sets a int 16 (short) value in the given array.
        /// </summary>
        /// <param name="data">
        /// The data array.
        /// </param>
        /// <param name="startIndex">
        /// The offset into the <see cref="data"/> where to start setting the bytes.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        public static void SetInt16(byte[] data, int startIndex, short value)
        {
            data[startIndex] = (byte)((value >> 8) & 0xFF);
            data[startIndex + 1] = (byte)(value & 0xFF);
        }
    }
}
