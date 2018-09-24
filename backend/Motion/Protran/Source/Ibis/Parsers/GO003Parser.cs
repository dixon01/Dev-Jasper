// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO003Parser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO003Parser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Special parser for GO003 which has an additional header field.
    /// </summary>
    public class GO003Parser : AnswerWithDS120Parser<GO003>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GO003Parser"/> class.
        /// </summary>
        public GO003Parser()
            : base("aB")
        {
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header including the additional character.
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
