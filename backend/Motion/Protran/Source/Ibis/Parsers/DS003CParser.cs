// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003CParser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS003CParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Telegram parser for <see cref="DS003C"/> that also removes the length character.
    /// </summary>
    public class DS003CParser : SimpleHeaderTelegramParser<DS003C>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS003CParser"/> class.
        /// </summary>
        public DS003CParser()
            : base("zI", new HexDigit())
        {
            // uppercase I
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header including the length character.
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