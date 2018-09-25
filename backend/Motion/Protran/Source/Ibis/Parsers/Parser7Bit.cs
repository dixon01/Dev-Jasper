// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser7Bit.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Object tasked to parse IBIS telegrams where each
    /// information is made with 7 bits.
    /// </summary>
    public class Parser7Bit : ParserOneByte
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parser7Bit"/> class
        /// with a specific value for the CRC check.
        /// </summary>
        /// <param name="checkChecksum">
        /// True to enable the CRC check, else false.
        /// </param>
        /// <param name="configs">
        /// The telegram configurations.
        /// </param>
        public Parser7Bit(bool checkChecksum, IEnumerable<TelegramConfig> configs)
            : base(ByteType.Ascii7, checkChecksum, configs)
        {
        }

        /// <summary>
        /// Calculates the 7-bit checksum of the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <returns>
        /// the XOR checksum of the given telegram, excluding the last byte.
        /// </returns>
        protected override byte CalculateChecksum(byte[] telegram)
        {
            // mask the highest bit since we only have 7 bits
            return (byte)(base.CalculateChecksum(telegram) & 0x7F);
        }
    }
}
