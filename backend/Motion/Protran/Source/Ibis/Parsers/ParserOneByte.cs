// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParserOneByte.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ParserOneByte type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Core.Buffers;

    /// <summary>
    /// Base class for all parsers that parse single byte telegrams (7 or 8 bit).
    /// </summary>
    public abstract class ParserOneByte : Parser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserOneByte"/> class
        /// with a specific value for the CRC check.
        /// </summary>
        /// <param name="byteType">
        /// The byte of the parser.
        /// </param>
        /// <param name="checkChecksum">
        /// True to enable the CRC check, else false.
        /// </param>
        /// <param name="configs">
        /// The telegram configurations.
        /// </param>
        protected ParserOneByte(ByteType byteType, bool checkChecksum, IEnumerable<TelegramConfig> configs)
            : base(byteType, checkChecksum, configs)
        {
        }

        /// <summary>
        /// Updates the last byte of the given telegram to be a correct CRC.
        /// </summary>
        /// <param name="telegram">the telegram to update</param>
        public override void UpdateChecksum(byte[] telegram)
        {
            if (telegram == null || telegram.Length < 1)
            {
                throw new ArgumentException("Invalid telegram");
            }

            telegram[telegram.Length - 1] = this.CalculateChecksum(telegram);
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

            if (telegram == null || telegram.Length == 0)
            {
                // invalid telegram.
                // I cannot parse it.
                return false;
            }

            var calculatedValue = this.CalculateChecksum(telegram);
            return calculatedValue == telegram[telegram.Length - 1];
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
            for (int i = 0; i < dist; i++)
            {
                // Attention:
                // the circular buffer has overriden the operator []
                // in a circular fashion.
                // so, circularBuffer[ i ] <==> circularBuffer[ i + circularBuffer.Head % circularBuffer.Buffer.Length ]
                if (circularBuffer[i] == this.Marker)
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
            return circularBuffer[0] >= 'A' && circularBuffer[0] <= 'z';
        }

        /// <summary>
        /// Calculates the 8-bit checksum of the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <returns>
        /// the XOR checksum of the given telegram, excluding the last byte.
        /// </returns>
        protected virtual byte CalculateChecksum(byte[] telegram)
        {
            byte calculatedValue = 0xFF;
            for (int i = 0; i < telegram.Length - 1; i++)
            {
                calculatedValue ^= telegram[i];
            }

            return calculatedValue;
        }
    }
}