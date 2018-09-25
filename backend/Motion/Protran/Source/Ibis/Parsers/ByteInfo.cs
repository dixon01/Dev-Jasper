// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ByteInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;
    using System.Text;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Ibis;

    /// <summary>
    /// Class that provides byte information for parsers.
    /// </summary>
    public class ByteInfo
    {
        /// <summary>
        /// Byte information: 7 bit, 1 byte, ASCII.
        /// </summary>
        public static readonly ByteInfo Ascii7 = new ByteInfo { BitCount = 7, ByteSize = 1, Encoding = Encoding.ASCII };

        /// <summary>
        /// Byte information: 8 bit, 1 byte, big endian Unicode.
        /// </summary>
        public static readonly ByteInfo Hengartner8 = new ByteInfo
                                                          {
                                                              BitCount = 8,
                                                              ByteSize = 1,
                                                              Encoding = Encoding.BigEndianUnicode
                                                          };

        /// <summary>
        /// Byte information: 16 bit, 2 byte, big endian Unicode.
        /// </summary>
        public static readonly ByteInfo UnicodeBigEndian = new ByteInfo
                                                               {
                                                                   BitCount = 16,
                                                                   ByteSize = 2,
                                                                   Encoding = Encoding.BigEndianUnicode
                                                               };

        private ByteInfo()
        {
        }

        /// <summary>
        /// Gets the number of bits in each character.
        /// </summary>
        [XmlIgnore]
        public int BitCount { get; private set; }

        /// <summary>
        /// Gets the number of bytes for each character.
        /// </summary>
        [XmlIgnore]
        public int ByteSize { get; private set; }

        /// <summary>
        /// Gets the default encoding for this byte type.
        /// </summary>
        [XmlIgnore]
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Returns the byte information for a given byte type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// One of <see cref="Ascii7"/>, <see cref="Hengartner8"/>
        /// or <see cref="UnicodeBigEndian"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// if the byte type is not supported.
        /// </exception>
        public static ByteInfo For(ByteType type)
        {
            switch (type)
            {
                case ByteType.Ascii7:
                    return Ascii7;
                case ByteType.Hengartner8:
                    return Hengartner8;
                case ByteType.UnicodeBigEndian:
                    return UnicodeBigEndian;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
