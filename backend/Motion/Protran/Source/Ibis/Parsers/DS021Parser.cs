// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021Parser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021Parser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Telegram parser for <see cref="DS021"/> that also removes the length character.
    /// </summary>
    public class DS021Parser : AnswerWithDS120Parser<DS021>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS021Parser"/> class.
        /// </summary>
        public DS021Parser()
            : base("aA", new HexDigit(2))
        {
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header including the address and length characters.
        /// </summary>
        public override int HeaderSize
        {
            get
            {
                return base.HeaderSize + this.ByteInfo.ByteSize;
            }
        }
    }
}