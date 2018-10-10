// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumMessage.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Messages
{
    using System;

    /// <summary>
    /// Message containing several enums.
    /// </summary>
    [Serializable]
    public class EnumMessage
    {
        /// <summary>
        /// Simple int enum.
        /// </summary>
        public enum MyEnum
        {
            /// <summary>
            /// First value
            /// </summary>
            A,

            /// <summary>
            /// Second value
            /// </summary>
            B,

            /// <summary>
            /// Third value
            /// </summary>
            C
        }

        /// <summary>
        /// Simple ulong enum.
        /// </summary>
        public enum MyUlongEnum : ulong
        {
            /// <summary>
            /// First value
            /// </summary>
            A = 5,

            /// <summary>
            /// Second value
            /// </summary>
            B,

            /// <summary>
            /// Third value
            /// </summary>
            C
        }

        /// <summary>
        /// Simple byte enum.
        /// </summary>
        public enum MyByteEnum : byte
        {
            /// <summary>
            /// First value
            /// </summary>
            A,

            /// <summary>
            /// Second value
            /// </summary>
            B,

            /// <summary>
            /// Third value
            /// </summary>
            C = 255
        }

        /// <summary>
        /// Gets or sets DateTimeKind.
        /// </summary>
        public DateTimeKind DateTimeKind { get; set; }

        /// <summary>
        /// Gets or sets Enum.
        /// </summary>
        public MyEnum Enum { get; set; }

        /// <summary>
        /// Gets or sets UlongEnum.
        /// </summary>
        public MyUlongEnum UlongEnum { get; set; }

        /// <summary>
        /// Gets or sets ByteEnum.
        /// </summary>
        public MyByteEnum ByteEnum { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param>
        public override bool Equals(object obj)
        {
            var other = obj as EnumMessage;
            if (other == null)
            {
                return false;
            }

            return this.DateTimeKind == other.DateTimeKind && this.ByteEnum == other.ByteEnum && this.Enum == other.Enum
                   && this.UlongEnum == other.UlongEnum;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.DateTimeKind.GetHashCode();
        }
    }
}
