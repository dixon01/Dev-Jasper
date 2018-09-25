// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramFormatter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramFormatter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using System;
    using System.Text;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Motion.Protran.Core.Buffers;

    /// <summary>
    /// Static helper that allows us to format telegrams and parse them.
    /// </summary>
    public static class TelegramFormatter
    {
        /// <summary>
        /// Converts a byte array to a string in hexadecimal form
        /// (padded eventually with zeroes).
        /// </summary>
        /// <param name="data">The buffer from which extract the hexadecimal string.</param>
        /// <returns>The hexadecimal representation of the incoming buffer (padded with zeroes).</returns>
        public static string ToHexString(byte[] data)
        {
            return BufferUtils.FromByteArrayToHexString(data);
        }

        /// <summary>
        /// Converts an hexadecimal string (padded with zeroes) to a byte array.
        /// </summary>
        /// <param name="zeroPaddedHexString">The hexadecimal string (padded with zeroes)
        /// from which extract the buffer of bytes.</param>
        /// <returns>The representation of the incoming string as a buffer of bytes.</returns>
        public static byte[] FromHexString(string zeroPaddedHexString)
        {
            return BufferUtils.FromHexStringToByteArray(zeroPaddedHexString);
        }

        /// <summary>
        /// Converts a byte array to a string in ASCII form with all special characters
        /// escaped using &lt;xx&gt; hex format.
        /// </summary>
        /// <param name="data">
        /// The buffer from which extract the telegram string.
        /// </param>
        /// <param name="byteType">
        /// The byte type to be used for formatting (7 or 16 bit).
        /// </param>
        /// <returns>
        /// The telegram representation of the incoming buffer.
        /// </returns>
        public static string ToTelegramString(byte[] data, ByteType byteType)
        {
            char[] chars;
            if (byteType == ByteType.Hengartner8)
            {
                chars = Array.ConvertAll(data, b => (char)b);
            }
            else
            {
                var encoding = byteType == ByteType.UnicodeBigEndian ? Encoding.BigEndianUnicode : Encoding.ASCII;
                chars = encoding.GetChars(data);
            }

            var sb = new StringBuilder(data.Length * 2);
            foreach (var c in chars)
            {
                if (char.IsControl(c) || c >= 0x7F)
                {
                    sb.AppendFormat(byteType == ByteType.UnicodeBigEndian ? "<{0:X4}>" : "<{0:X2}>", (int)c);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Escapes control characters in a string using using &lt;xx&gt; hex format.
        /// </summary>
        /// <param name="value">
        /// The string that has to be escaped.
        /// </param>
        /// <returns>
        /// The telegram representation of the incoming buffer.
        /// </returns>
        public static string ToTelegramString(string value)
        {
            var sb = new StringBuilder(value.Length * 2);
            foreach (var c in value)
            {
                if (char.IsControl(c))
                {
                    sb.AppendFormat(c <= 0xFF ? "<{0:X2}>" : "<{0:X4}>", (int)c);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
