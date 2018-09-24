// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021CParser.cs" company="Gorba AG">
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
    public class DS021CParser : AnswerWithDS120Parser<DS021C>
    {
        private Status currentDataStatus = Status.NoData;

        private bool telegramOnceArrived;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021CParser"/> class.
        /// </summary>
        public DS021CParser()
            : base(
                "aX",
                new HexDigit(),        // address
                new CharRange("012"),  // status (only 0, 1 and 2 are valid)
                new Constant("\x03"),  // <03>
                new Digit(3),          // stop index
                new Constant("\x04"))  // <04>
        {
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
            var chars = this.ByteInfo.Encoding.GetChars(telegram, 0, telegram.Length - this.ByteInfo.ByteSize);
            if (chars.Length < 8)
            {
                return Status.IncorrectRecord;
            }

            // chars[0] is always 'a', otherwise this parser doesn't get called
            // chars[1] is always 'X', otherwise this parser doesn't get called

            // chars[2]: address
            if (chars[2] < '0' || chars[2] > '?')
            {
                return Status.IncorrectRecord;
            }

            // chars[3]: status
            if (chars[3] < '0' || chars[3] > '2')
            {
                return Status.IncorrectRecord;
            }

            int status = chars[3] - '0';

            // chars[4]: delimiter has to be <03>
            if (chars[4] != 0x03)
            {
                return Status.IncorrectRecord;
            }

            // two or three digit stop index
            int offset = 5;
            int index = 0;
            while (offset < 8 && char.IsDigit(chars[offset]))
            {
                index = (index * 10) + (chars[offset] - '0');
                offset++;
            }

            if (offset < 7)
            {
                // we got less than 2 digits stop index
                return Status.IncorrectRecord;
            }

            if (chars[offset] != 0x0D && chars[offset] != 0x04)
            {
                // we expect a <0D> or <04> after the stop index
                return Status.IncorrectRecord;
            }

            if (index == 0)
            {
                return this.currentDataStatus;
            }

            if (index == 1)
            {
                this.currentDataStatus = Status.MissingData;
            }
            else if (this.currentDataStatus != Status.MissingData)
            {
                // we have an index > 1 and did never get index 1,
                // so we don't have any valid data
                this.currentDataStatus = Status.NoData;
            }
            else if (status == 2)
            {
                this.currentDataStatus = Status.Ok;
            }

            return this.currentDataStatus;
        }
    }
}
