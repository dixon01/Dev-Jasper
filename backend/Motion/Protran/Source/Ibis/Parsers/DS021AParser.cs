// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021AParser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Special parser for DS021a which has an additional 2-character header field.
    /// </summary>
    public class DS021AParser : AnswerWithDS120Parser<DS021A>
    {
        private const string TelegramHeader = "aL";

        private bool telegramOnceArrived;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021AParser"/> class.
        /// </summary>
        public DS021AParser()
            : base(TelegramHeader)
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
        /// Configures this object.
        /// </summary>
        /// <param name="byteInfo">
        /// The byte information.
        /// </param>
        /// <param name="config">
        /// The config of the telegram.
        /// </param>
        public override void Configure(ByteInfo byteInfo, TelegramConfig config)
        {
            base.Configure(byteInfo, config);

            if (byteInfo == ByteInfo.Hengartner8)
            {
                // the rest of the telegram is not verifiable for CU5 (Abu Dhabi)
                this.Verifier = new TelegramVerifier(new TelegramRule[]
                    {
                        new Constant(TelegramHeader)
                    });
            }
            else
            {
                this.Verifier = new TelegramVerifier(new TelegramRule[]
                    {
                        new Constant(TelegramHeader),
                        new HexDigit(1), // address
                        new HexDigit(2), // length
                        new Constant("\x03"), // <03>
                        new CharRange("AS0123456789"), // 2-3 digit number or Ax or Sx
                        new Digit(1, 2)
                    });
            }
        }

        /// <summary>
        /// Creates the IBIS telegram as answer for the incoming telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <param name="status">The status with which make the answer.</param>
        /// <returns>The telegram that is the answer for the incoming telegram.</returns>
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
            if (this.ByteInfo == ByteInfo.Hengartner8)
            {
                // we ignore CU5 (Abu Dhabi) special telegrams and assume they are always OK
                return Status.Ok;
            }

            var chars = this.ByteInfo.Encoding.GetChars(telegram, 0, telegram.Length - this.ByteInfo.ByteSize);
            if (chars.Length < 9)
            {
                return Status.IncorrectRecord;
            }

            // chars[0] is always 'a', otherwise this parser doesn't get called
            // chars[1] is always 'L', otherwise this parser doesn't get called

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

            // two or three digit stop index
            int index = 6;
            if (chars[index] == 'A' || chars[index] == 'S')
            {
                // we allow 'A' or 'S' as the first character of the stop index
                // for special cases "Connection" and "Text message"
                index++;
            }

            while (index < 9 && char.IsDigit(chars[index]))
            {
                index++;
            }

            if (index < 8)
            {
                // we got less than 2 digits stop index
                return Status.IncorrectRecord;
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
