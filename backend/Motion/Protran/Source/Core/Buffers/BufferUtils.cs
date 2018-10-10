// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferUtils.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Buffers
{
    using System;
    using System.Globalization;
    using System.Text;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// A container of utility functions for buffers.
    /// </summary>
    public static class BufferUtils
    {
        /// <summary>
        /// Gets a string from the bytes contained inside the incoming buffer.
        /// The conversion uses the big endian UTF-16 format.
        /// </summary>
        /// <param name="buffer">The buffer containing the bytes to parse.</param>
        /// <returns>The big endian UTF-16 string representation of the incoming buffer,
        /// or an empty string in case of error.</returns>
        public static string FromBytesToString(byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                // invalid buffer.
                // I cannot parse nothing.
                return string.Empty;
            }

            string value;
            try
            {
                value = Encoding.BigEndianUnicode.GetString(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {
                // an error was occured parsing the string in UTF-16 format.
                return string.Empty;
            }

            // ok, it seems everithing ok.
            return value;
        }

        /// <summary>
        /// Converts an hexadecimal string (padded with zeroes) to a byte array.
        /// </summary>
        /// <param name="zeroPaddedHexString">The hexadecimal string (padded with zeroes)
        /// from which extract the buffer of bytes.</param>
        /// <returns>The representation of the incoming string as a buffer of bytes.</returns>
        public static byte[] FromHexStringToByteArray(string zeroPaddedHexString)
        {
            if (string.IsNullOrEmpty(zeroPaddedHexString))
            {
                // invalid string.
                return null;
            }

            if (zeroPaddedHexString.Length % 2 != 0)
            {
                // a zero-padded hexadecimal string has surely a length
                // as an even number of characters.
                return null;
            }

            // variable used to hold position in string
            int i = 0;

            // variable used to hold byte array element position
            int x = 0;

            // allocate byte array based on half of string length
            var bytes = new byte[zeroPaddedHexString.Length / 2];

            // loop through the string - 2 bytes at a time converting
            // it to decimal equivalent and store in byte array
            while (zeroPaddedHexString.Length > i + 1)
            {
                bool parsedOk;
                try
                {
                    string token = zeroPaddedHexString.Substring(i, 2);
                    parsedOk = ParserUtil.TryParse(token, NumberStyles.HexNumber, null, out bytes[x]);
                }
                catch (Exception)
                {
                    // an error was occured converting a piece of string
                    // to an intger.
                    // I cannot continue anymore with this function.
                    return null;
                }

                if (!parsedOk)
                {
                    // an error was occured on transforming a piece
                    // of string into a byte.
                    // I cannot continue anymore with this function.
                    return null;
                }

                // ok, I can update my variable for the next byte.
                i = i + 2;
                ++x;
            }

            // return the finished byte array of decimal values
            return bytes;
        }

        /// <summary>
        /// Converts a byte array to a string in hexadecimal form
        /// (padded eventually with zeroes).
        /// </summary>
        /// <param name="buffer">The buffer from which extract the hexadecimal string.</param>
        /// <returns>The hexadecimal representation of the incoming buffer (padded with zeroes).</returns>
        public static string FromByteArrayToHexString(byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                // invalid buffer.
                return string.Empty;
            }

            return FromByteArrayToHexString(buffer, 0, buffer.Length, false);
        }

        /// <summary>
        /// Creates an hexadecimal formatted string starting from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array from which construct the hexadecimal string.</param>
        /// <param name="startOffset">The start offset from which construct the hexadecimal string.</param>
        /// <param name="numBytes">The amount of bytes to be taken.</param>
        /// <param name="autoPrefix">True to have a string that starts with "0x", else false.</param>
        /// <returns>The byte array expressed with an hexadecimal string, or an empty string
        /// in case of invalid parameters.</returns>
        public static string FromByteArrayToHexString(byte[] buffer, int startOffset, int numBytes, bool autoPrefix)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                // invalid buffer.
                return string.Empty;
            }

            if (startOffset >= buffer.Length)
            {
                // invalid start offset
                return string.Empty;
            }

            if (numBytes == 0)
            {
                // no byte to take in consideration.
                return string.Empty;
            }

            if (startOffset + numBytes > buffer.Length)
            {
                // invalid dimension. I'll go out of buffer
                return string.Empty;
            }

            // ok, it seems that the parameters are good.
            var strBuilder = new StringBuilder((numBytes * 2) + 2);
            if (autoPrefix)
            {
                strBuilder.Append("0x");
            }

            for (int i = 0; i < numBytes; i++)
            {
                strBuilder.Append(buffer[startOffset + i].ToString("X2"));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Dumps a buffer in a string with hexadecimal conversion of the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to be dumped as an hexadecimal string.</param>
        /// <returns>A buffer in a string with hexadecimal conversion of the buffer.</returns>
        public static string DumpBufferAsHexString(byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                // nothing to parse.
                return string.Empty;
            }

            var stringBuilder = new StringBuilder((buffer.Length * 2) + 2);
            stringBuilder.Append("0x");
            for (int i = 0; i < buffer.Length; i++)
            {
                try
                {
                    stringBuilder.Append(buffer[i].ToString("X2"));
                }
                catch (Exception)
                {
                    // an error was occured appending the
                    // current byte in hexadecimal format.
                    // So, here I append a "token" that indicates an error.
                    stringBuilder.Append("BAADBEEF");
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the string representation of a UNIX 64 timestamp.
        /// The returned string has the format:
        /// YYYY/MM/DD HH:MM:SS
        /// </summary>
        /// <param name="buffer">The buffer containing the UNIX 64 timestamp (in big endian format).</param>
        /// <returns>The date representation of the incoming UNIX 64 timestamp, or an empty
        /// string
        /// <see cref="DateTime.MaxValue"/> in case of error.</returns>
        public static DateTime FromBytesToUnix64BitTimestamp(byte[] buffer)
        {
            if (buffer == null || buffer.Length != 8)
            {
                // invalid buffer.
                // I cannot parse nothing.
                return DateTime.MaxValue;
            }

            ulong tmp7 = buffer[0];
            tmp7 = tmp7 << 56;
            ulong tmp6 = buffer[1];
            tmp6 = tmp6 << 48;
            ulong tmp5 = buffer[2];
            tmp5 = tmp5 << 40;
            ulong tmp4 = buffer[3];
            tmp4 = tmp4 << 32;
            ulong tmp3 = buffer[4];
            tmp3 = tmp3 << 24;
            ulong tmp2 = buffer[5];
            tmp2 = tmp2 << 16;
            ulong tmp1 = buffer[6];
            tmp1 = tmp1 << 8;
            ulong tmp0 = buffer[7];
            ulong value = tmp7 | tmp6 | tmp5 | tmp4 | tmp3 | tmp2 | tmp1 | tmp0;

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
            try
            {
                return epoch.AddSeconds(value);
            }
            catch (Exception)
            {
                // an error was occured adding the seconds to the epoch time.
                return DateTime.MaxValue;
            }
        }
    }
}
