// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpAddress.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpAddress type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp
{
    using System;

    /// <summary>
    /// The UDCP address, which is actually the same as the first MAC address found in the system.
    /// It is important to note that on a multi-interface system, the UDCP address is not necessarily
    /// the same as the MAC address of the interface sending or receiving the datagram.
    /// </summary>
    public class UdcpAddress : IEquatable<UdcpAddress>
    {
        /// <summary>
        /// The broadcast UDCP address (FF-FF-FF-FF-FF-FF).
        /// </summary>
        public static readonly UdcpAddress BroadcastAddress =
            new UdcpAddress(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        private const int SizeOf = 6;

        private readonly byte[] bytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpAddress"/> class.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        public UdcpAddress(byte[] bytes)
        {
            if (bytes.Length != SizeOf)
            {
                throw new ArgumentException("UDCP address must be " + SizeOf + " bytes");
            }

            this.bytes = (byte[])bytes.Clone();
        }

        /// <summary>
        /// Gets a copy of the address bytes.
        /// </summary>
        /// <returns>
        /// An array with 6 bytes.
        /// </returns>
        public byte[] GetAddressBytes()
        {
            return (byte[])this.bytes.Clone();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(UdcpAddress other)
        {
            if (other == null)
            {
                return false;
            }

            for (int i = 0; i < SizeOf; i++)
            {
                if (this.bytes[i] != other.bytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="object"/> to compare with the current <see cref="object"/>.
        /// </param>
        public override bool Equals(object obj)
        {
            var other = obj as UdcpAddress;
            return this.Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return BitConverter.ToInt32(this.bytes, SizeOf - sizeof(int));
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return BitConverter.ToString(this.bytes);
        }
    }
}