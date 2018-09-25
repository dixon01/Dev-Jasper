// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParserUtil.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ParserUtil type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Net;

    /// <summary>
    /// Utility class for parsing strings.
    /// This class provides methods that are not available in some frameworks.
    /// </summary>
    public static partial class ParserUtil
    {
        /// <summary>
        /// Tries to parse the given <see cref="value"/> into a byte.
        /// </summary>
        /// <param name="value">
        /// The value string.
        /// </param>
        /// <param name="output">
        /// The output byte.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParse(string value, out byte output)
        {
            return byte.TryParse(value, out output);
        }

        /// <summary>
        /// Tries to parse the given <see cref="value"/> into a byte.
        /// </summary>
        /// <param name="value">
        /// The value string.
        /// </param>
        /// <param name="style">
        /// The number style.
        /// </param>
        /// <param name="provider">
        /// The format provider.
        /// </param>
        /// <param name="output">
        /// The output byte.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out byte output)
        {
            return byte.TryParse(value, style, provider, out output);
        }

        /// <summary>
        /// Tries to parse the given <see cref="value"/> into an integer.
        /// </summary>
        /// <param name="value">
        /// The value string.
        /// </param>
        /// <param name="output">
        /// The output integer.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParse(string value, out int output)
        {
            return int.TryParse(value, out output);
        }

        /// <summary>
        /// Tries to parse the given <see cref="value"/> into an integer.
        /// </summary>
        /// <param name="value">
        /// The value string.
        /// </param>
        /// <param name="style">
        /// The number style.
        /// </param>
        /// <param name="provider">
        /// The format provider.
        /// </param>
        /// <param name="output">
        /// The output integer.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out int output)
        {
            return int.TryParse(value, style, provider, out output);
        }

        /// <summary>
        /// Tries to parse the given <see cref="value"/> into an unsigned integer.
        /// </summary>
        /// <param name="value">
        /// The value string.
        /// </param>
        /// <param name="output">
        /// The output unsigned integer.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParse(string value, out uint output)
        {
            return uint.TryParse(value, out output);
        }

        /// <summary>
        /// Tries to parse the given <see cref="value"/> into an unsigned integer.
        /// </summary>
        /// <param name="value">
        /// The value string.
        /// </param>
        /// <param name="style">
        /// The number style.
        /// </param>
        /// <param name="provider">
        /// The format provider.
        /// </param>
        /// <param name="output">
        /// The output unsigned integer.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out uint output)
        {
            return uint.TryParse(value, style, provider, out output);
        }

        /// <summary>
        /// Tries to parse the given <see cref="value"/> into an IP address.
        /// </summary>
        /// <param name="value">
        /// The value string.
        /// </param>
        /// <param name="address">
        /// The output IP address.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParse(string value, out IPAddress address)
        {
            return IPAddress.TryParse(value, out address);
        }

        /// <summary>
        /// Tries to parse the given <see cref="value"/> into a date.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="format">
        /// The date/time format.
        /// </param>
        /// <param name="provider">
        /// The format provider.
        /// </param>
        /// <param name="style">
        /// The date/time style.
        /// </param>
        /// <param name="result">
        /// The resulting date time.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParseExact(
            string value, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
        {
            return DateTime.TryParseExact(value, format, provider, style, out result);
        }

        /// <summary>
        /// Tries to parse the given <see cref="color"/> string into a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">
        /// The color name or color value (#RRGGBB or #AARRGGBB).
        /// </param>
        /// <param name="output">
        /// The output color.
        /// </param>
        /// <returns>
        /// True if the <see cref="color"/> was successfully parsed.
        /// </returns>
        public static bool TryParseColor(string color, out Color output)
        {
            if (color.StartsWith("#"))
            {
                uint value;
                if (!TryParse(color.Substring(1), NumberStyles.HexNumber, null, out value))
                {
                    output = default(Color);
                    return false;
                }

                if (color.Length <= 7)
                {
                    value |= 0xFF000000;
                }

                output = Color.FromArgb((int)value);
                return true;
            }

            try
            {
                output = Color.FromName(color);
                return true;
            }
            catch (Exception)
            {
                output = default(Color);
                return false;
            }
        }

        /// <summary>
        /// Tries to parse the given <see cref="value"/> into a <see cref="Version"/>.
        /// </summary>
        /// <param name="value">
        /// The value string.
        /// </param>
        /// <param name="output">
        /// The output version.
        /// </param>
        /// <returns>
        /// True if the <see cref="value"/> was successfully parsed.
        /// </returns>
        public static bool TryParse(string value, out Version output)
        {
            try
            {
                output = new Version(value);
                return true;
            }
            catch (Exception)
            {
                output = null;
                return false;
                throw;
            }
        }
    }
}
