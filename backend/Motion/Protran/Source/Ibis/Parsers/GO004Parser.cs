// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO004Parser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO004Parser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;

    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Special parser for GO004.
    /// </summary>
    public class GO004Parser : AnswerWithDS120Parser<GO004>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GO004Parser"/> class.
        /// </summary>
        public GO004Parser()
            : base("aM", new HexDigit(), new Digit(2))
        {
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header including the address character.
        /// </summary>
        public override int HeaderSize
        {
            get
            {
                return base.HeaderSize + (2 * this.ByteInfo.ByteSize);
            }
        }

        /// <summary>
        /// Parses the given byte array into a telegram.
        /// </summary>
        /// <param name="data">The telegram including header, marker and checksum.</param>
        /// <returns>The telegram object containing the payload of the given telegram.</returns>
        protected override GO004 Parse(byte[] data)
        {
            var telegram = base.Parse(data);

            // important: use the base header size since we are now parsing our part of the header
            var offset = base.HeaderSize;
            int byteSize = this.ByteInfo.ByteSize;

            telegram.MessageIndex = this.ParseInteger(data, offset, 2);
            offset += 2 * byteSize;
            telegram.MessageType = this.ParseInteger(data, offset, 2);
            offset += 2 * byteSize;
            telegram.TimeRange = this.ParseInteger(data, offset, 8);
            offset += 8 * byteSize;

            int payloadSize = data.Length - offset - (2 * byteSize);
            if (payloadSize > 0)
            {
                telegram.Payload = new byte[payloadSize];
                Array.Copy(data, offset, telegram.Payload, 0, payloadSize);
            }
            else if (telegram.Payload.Length > 0)
            {
                telegram.Payload = new byte[0];
            }

            return telegram;
        }

        private int ParseInteger(byte[] data, int offset, int length)
        {
            int byteSize = this.ByteInfo.ByteSize;
            length *= byteSize;
            if (data.Length - (2 * byteSize) < offset + length)
            {
                return -1;
            }

            var text = this.ByteInfo.Encoding.GetString(data, offset, length);
            return int.Parse(text);
        }
    }
}