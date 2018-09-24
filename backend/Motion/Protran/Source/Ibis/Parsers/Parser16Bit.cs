// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser16Bit.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Core.Buffers;

    /// <summary>
    /// Object tasked to parse IBIS telegrams where each
    /// information is made with 16 bits.
    /// </summary>
    public class Parser16Bit : Parser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parser16Bit"/> class
        /// with a specific value for the CRC check.
        /// </summary>
        /// <param name="checkChecksum">
        /// True to enable the CRC check, else false.
        /// </param>
        /// <param name="configs">
        /// The telegram configurations.
        /// </param>
        public Parser16Bit(bool checkChecksum, IEnumerable<TelegramConfig> configs)
            : base(ByteType.UnicodeBigEndian, checkChecksum, configs)
        {
        }

        /// <summary>
        /// Updates the last two bytes of the given telegram to be a correct CRC.
        /// </summary>
        /// <param name="telegram">the telegram to update</param>
        public override void UpdateChecksum(byte[] telegram)
        {
            if (telegram == null || telegram.Length < 2 || telegram.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid telegram");
            }

            var crc = CalculateChecksum(telegram);
            telegram[telegram.Length - 2] = (byte)(crc >> 8);
            telegram[telegram.Length - 1] = (byte)(crc & 0xFF);
        }

        /// <summary>
        /// Checks if the incoming telegram has a valid CRC value.
        /// </summary>
        /// <param name="telegram">The telegram that has to be checked.</param>
        /// <returns>True if the telegram has a valid CRC value, else false.</returns>
        public override bool IsChecksumCorrect(byte[] telegram)
        {
            if (!this.CheckChecksum)
            {
                // this parser doesn't have to check the CRC.
                // so, for me the telegram is surely corrected.
                return true;
            }

            if (telegram == null || telegram.Length == 0 || telegram.Length % 2 != 0)
            {
                // invalid telegram.
                // I cannot parse it.
                return false;
            }

            var calculatedValue = CalculateChecksum(telegram);
            var originalValue = telegram[telegram.Length - 2] << 8 | telegram[telegram.Length - 1];
            return calculatedValue == originalValue;
        }

        /// <summary>
        /// Finds the index of the first marker inside the circular buffer,
        /// making a scan between the actual values of the head and the tail.
        /// </summary>
        /// <param name="circularBuffer">The buffer in which search for the IBIS marker.</param>
        /// <returns>The marker's index or -1 if the marker is not found.</returns>
        protected override int FindMarker(CircularBuffer circularBuffer)
        {
            int dist = circularBuffer.CurrentLength;
            for (int i = 0; i + 1 < dist; i++)
            {
                // Attention:
                // the circular buffer has overriden the operator []
                // in a circular fashion.
                // so, circularBuffer[ i ] <==> circularBuffer[ i + circularBuffer.Head % circularBuffer.Buffer.Length ]
                var value = (short)((circularBuffer[i] << 8) | circularBuffer[i + 1]);
                if (value == this.Marker)
                {
                    return (circularBuffer.Head + i) % circularBuffer.Buffer.Length;
                }
            }

            return -1;
        }

        /// <summary>
        /// Checks the circular buffer if it starts with a telegram header.
        /// </summary>
        /// <param name="circularBuffer">
        /// The circular buffer.
        /// </param>
        /// <returns>
        /// True if the first character in the buffer is a telegram header character.
        /// </returns>
        protected override bool IsTelegramHeader(CircularBuffer circularBuffer)
        {
            return circularBuffer[1] >= 'A' && circularBuffer[1] <= 'z';
        }

        private static int CalculateChecksum(byte[] telegram)
        {
            int calculatedValue = 0xFFFF;
            for (int i = 0; i < telegram.Length - 2; i += 2)
            {
                byte high = telegram[i];
                byte low = telegram[i + 1];
                var tmp = (high << 8) | low;
                calculatedValue ^= tmp;
            }

            return calculatedValue;
        }
    }
}
