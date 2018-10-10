// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Telegram.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Telegram type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers;

    /// <summary>
    /// IBIS telegram wrapper with some simple factory methods.
    /// </summary>
    public class Telegram
    {
        private static readonly Parser Parser7Bit = new Parser7Bit(true, new TelegramConfig[0]);
        private static readonly Parser Parser8Bit = new Parser8Bit(true, new TelegramConfig[0]);
        private static readonly Parser Parser16Bit = new Parser16Bit(true, new TelegramConfig[0]);

        /// <summary>
        /// Initializes a new instance of the <see cref="Telegram"/> class.
        /// </summary>
        /// <param name="data">
        /// The telegram data including CR and checksum.
        /// </param>
        public Telegram(byte[] data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets the telegram data including CR and checksum.
        /// </summary>
        internal byte[] Data { get; private set; }

        /// <summary>
        /// Create a telegram from escaped ASCII format.
        /// </summary>
        /// <param name="ascii">
        /// The ascii string containing escapes either with &lt;CR&gt;,
        /// &lt;LF&gt; or &lt;xx&gt; where xx is a 2-digit hex number.
        /// </param>
        /// <returns>
        /// the telegram with the bytes representing the given telegram string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <see cref="ascii"/> contains invalid characters or bad escapes.
        /// </exception>
        public static Telegram FromEscapedAscii(string ascii)
        {
            var data = new List<byte>(ascii.Length);
            for (int i = 0; i < ascii.Length; i++)
            {
                char c = ascii[i];
                if (c == '<')
                {
                    int pos = ascii.IndexOf('>', i + 1);
                    if (i + 3 != pos)
                    {
                        throw new ArgumentException("Expected escapes in form <xx> but got " + ascii.Substring(i, 4));
                    }

                    var escape = ascii.Substring(i + 1, 2);
                    if (escape == "CR")
                    {
                        data.Add((byte)'\r');
                    }
                    else if (escape == "LF")
                    {
                        data.Add((byte)'\n');
                    }
                    else
                    {
                        data.Add(byte.Parse(escape, NumberStyles.HexNumber));
                    }

                    i = pos;
                }
                else if (c > 0x7F)
                {
                    throw new ArgumentException("Non-ASCII characters have to be escaped but got char code " + (int)c);
                }
                else
                {
                    data.Add((byte)c);
                }
            }

            return new Telegram(data.ToArray());
        }

        /// <summary>
        /// Converts a hex string to a telegram.
        /// </summary>
        /// <param name="hex">
        /// The hexadecimal string representing the telegram.
        /// This string has to contain only pairs of hexadecimal characters.
        /// </param>
        /// <returns>
        /// the telegram with the bytes representing the given telegram string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the hex string length is not a multiple of 2.
        /// </exception>
        public static Telegram FromHex(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException("Hex requires the string length to be a multiple of 2");
            }

            var data = new byte[hex.Length / 2];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            return new Telegram(data);
        }

        /// <summary>
        /// Updates the last byte of this telegram with the
        /// correct IBIS checksum.
        /// </summary>
        /// <param name="bitCount">
        /// The number of bits per character used for checksum calculation.
        /// Must be one of: 7, 8, 16
        /// </param>
        /// <exception cref="ArgumentException">
        /// If bitCount is not one of the expected values.
        /// </exception>
        public void UpdateChecksum(int bitCount)
        {
            Parser parser;
            switch (bitCount)
            {
                case 7:
                    parser = Parser7Bit;
                    break;
                case 8:
                    parser = Parser8Bit;
                    break;
                case 16:
                    parser = Parser16Bit;
                    break;
                default:
                    throw new ArgumentException("Expected 7, 8 or 16 as bitCount");
            }

            parser.UpdateChecksum(this.Data);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return BitConverter.ToString(this.Data);
        }
    }
}
