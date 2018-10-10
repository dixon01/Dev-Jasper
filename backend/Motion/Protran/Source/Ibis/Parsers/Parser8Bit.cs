// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser8Bit.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Parser8Bit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Object tasked to parse IBIS telegrams where each
    /// information is made with 8 bits.
    /// </summary>
    public class Parser8Bit : ParserOneByte
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parser8Bit"/> class.
        /// </summary>
        /// <param name="checkChecksum">
        /// True to enable the CRC check, else false.
        /// </param>
        /// <param name="configs">
        /// The telegram configurations.
        /// </param>
        public Parser8Bit(bool checkChecksum, IEnumerable<TelegramConfig> configs)
            : base(ByteType.Hengartner8, checkChecksum, configs)
        {
        }
    }
}