// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="Extensions.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Models;

    /// <summary>The extensions.</summary>
    public static class Extensions
    {
        #region Public Methods and Operators

        /// <summary>The calculate check sum.</summary>
        /// <param name="model">The model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="byte"/>.</returns>
        public static byte CalculateCheckSum<T>(this T model) where T : class, IPeripheralBaseMessage
        {
            var bytes = model.ToBytes();
            model.Checksum = CheckSumUtil.CheckSum(bytes);
            return model.Checksum;
        }

        /// <summary>The from bytes.</summary>
        /// <param name="byteArray">The byte array.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        /// <exception cref="MissingMethodException"></exception>
        public static T FromBytes<T>(this byte[] byteArray) where T : class
        {
            if (typeof(T).IsInterface)
            {
                throw new MissingMethodException("FromBytes<T>() Cannot create an instance of an interface for Type " + typeof(T));
            }

            if (typeof(T).IsAbstract)
            {
                throw new MissingMethodException("FromBytes<T>() Cannot create an instance of an abstract class for Type " + typeof(T));
            }

            int size = byteArray.Length;
            T obj = default(T);
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(byteArray, 0, ptr, size);
                obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
                return obj;
            }
            finally
            {
                // Release unmanaged memory.
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>The host to network byte order.</summary>
        /// <param name="header">The header.</param>
        /// <returns>The <see cref="PeripheralHeader"/>.</returns>
        public static PeripheralHeader HostToNetworkByteOrder(this PeripheralHeader header)
        {
            header.Length = HostToNetworkOrder(header.Length);
            header.Address = HostToNetworkOrder(header.Address);
            return header;
        }

        /// <summary>The network byte order to host.</summary>
        /// <param name="header">The header.</param>
        /// <returns>The <see cref="PeripheralHeader"/>.</returns>
        public static PeripheralHeader NetworkByteOrderToHost(this PeripheralHeader header)
        {
            header.Length = NetworkToHostByteOrder(header.Length);
            header.Address = NetworkToHostByteOrder(header.Address);
            return header;
        }

        /// <summary>The remove framing bytes.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] RemoveFramingBytes(this byte[] bytes)
        {
            return bytes.SkipWhile(m => m.Equals(DimmerConstants.PeripheralFramingByte)).ToArray();
        }

        /// <summary>The to bytes.</summary>
        /// <param name="obj">The obj.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] ToBytes<T>(this T obj) where T : class
        {
            var size = Marshal.SizeOf(obj);

            // Both managed and unmanaged buffers required.
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                // Copy object byte-to-byte to unmanaged memory.
                Marshal.StructureToPtr(obj, ptr, false);

                // Copy data from unmanaged memory to managed buffer.
                Marshal.Copy(ptr, bytes, 0, size);
                return bytes;
            }
            finally
            {
                // Release unmanaged memory.
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>The to peripheral header.</summary>
        /// <param name="bytesArray">The bytes array.</param>
        /// <param name="useNetworkByteOrder">The use network byte order.</param>
        /// <returns>The <see cref="PeripheralHeader"/>.</returns>
        public static PeripheralHeader ToPeripheralHeader(this byte[] bytesArray, bool useNetworkByteOrder = false)
        {
            PeripheralHeader header = null;
            if (bytesArray != null)
            {
                var bytesWithOutFrame = bytesArray.RemoveFramingBytes().Take(PeripheralHeader.HeaderSize).ToArray();
                header = new PeripheralHeader(bytesWithOutFrame, useNetworkByteOrder);
            }

            return header;
        }

        #endregion

        #region Methods

        private static ushort HostToNetworkOrder(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        private static ushort NetworkToHostByteOrder(ushort value)
        {
            return (ushort)((value & 0xFF00U) >> 8 | (value & 0xFFU) << 8);
        }

        #endregion
    }
}