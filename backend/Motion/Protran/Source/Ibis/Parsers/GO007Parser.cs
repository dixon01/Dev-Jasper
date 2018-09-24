// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO007Parser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO007Parser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Parser for GO007.
    /// </summary>
    public class GO007Parser : AnswerWithDS120Parser<GO007>
    {
        private bool telegramOnceArrived;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO007Parser"/> class.
        /// </summary>
        public GO007Parser()
            : base(
                "aA",
                new HexDigit(1), // address
                new HexDigit(2), // length
                new Constant("\x03"), // <03>
                new Any(4)) // line number
        {
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header including the two length characters.
        /// </summary>
        public override int HeaderSize
        {
            get
            {
                return base.HeaderSize + (2 * this.ByteInfo.ByteSize);
            }
        }

        /// <summary>
        /// Creates the IBIS telegram as answer for the incoming telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The telegram that is the answer for the incoming telegram.
        /// </returns>
        public override byte[] CreateAnswer(byte[] telegram, Status status)
        {
            this.telegramOnceArrived = true;

            // we have a special status here, not the system status
            return base.CreateAnswer(telegram, this.CheckTelegram(telegram));
        }

        /// <summary>
        /// To return a status if telegram had previously arrived
        /// </summary>
        /// <returns>
        /// if telegram has previously arrived
        /// </returns>
        public override bool TelegramArrivedBefore()
        {
            return this.telegramOnceArrived;
        }

        private Status CheckTelegram(byte[] telegram)
        {
            var chars = this.ByteInfo.Encoding.GetChars(telegram, 0, telegram.Length - this.ByteInfo.ByteSize);
            if (chars.Length < 11)
            {
                return Status.IncorrectRecord;
            }

            // chars[0] is always 'a', otherwise this parser doesn't get called
            // chars[1] is always 'A', otherwise this parser doesn't get called

            // chars[2], chars[3], chars[4]: address and data length have to be IBIS hex
            if (chars[2] < '0' || chars[2] > '?' || chars[3] < '0' || chars[3] > '?' || chars[4] < '0' ||
                chars[4] > '?')
            {
                return Status.IncorrectRecord;
            }

            // chars[5]: delimiter has to be <03>
            if (chars[5] != 0x03)
            {
                return Status.IncorrectRecord;
            }

            // four digit line number
            int index = 6;
            while (index < 10)
            {
                if (!char.IsLetterOrDigit(chars[index]) && !char.IsWhiteSpace(chars[index]))
                {
                    return Status.IncorrectRecord;
                }

                index++;
            }

            if (chars[index] != 0x0D && chars[index] != 0x04)
            {
                // we expect a <0D> or <04> after the stop index
                return Status.IncorrectRecord;
            }

            return Status.Ok;
        }
    }
}
