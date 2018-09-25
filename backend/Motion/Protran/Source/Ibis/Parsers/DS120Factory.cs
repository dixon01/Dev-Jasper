// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS120Factory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Factory for DS120 answers.
    /// </summary>
    public class DS120Factory
    {
        private readonly DS120Config answerConfig;

        private readonly ByteInfo byteInfo;

        private readonly byte[] answer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS120Factory"/> class.
        /// </summary>
        /// <param name="answerConfig">
        /// The answer configuration.
        /// </param>
        /// <param name="byteInfo">
        /// The byte information.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// if the <see cref="byteInfo"/> has an invalid bit count.
        /// </exception>
        public DS120Factory(DS120Config answerConfig, ByteInfo byteInfo)
        {
            this.answerConfig = answerConfig;
            this.byteInfo = byteInfo;

            // create a default answer
            switch (this.byteInfo.BitCount)
            {
                case 7:
                    this.answer = new byte[] { 0x61, 0x30, 0x0D, 0x23 };
                    break;
                case 8:
                    this.answer = new byte[] { 0x61, 0x30, 0x0D, 0xA3 };
                    break;
                case 16:
                    this.answer = new byte[] { 0x00, 0x61, 0x00, 0x30, 0x00, 0x0D, 0xFF, 0xA3 };
                    break;
                default:
                    throw new NotSupportedException("Unsupported bit count: " + this.byteInfo.BitCount);
            }
        }

        /// <summary>
        /// Creates the IBIS telegram as answer for the incoming telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <param name="status">
        /// The status with which make the answer.
        /// </param>
        /// <returns>
        /// The telegram that is the answer for the incoming telegram.
        /// </returns>
        public byte[] CreateAnswer(byte[] telegram, Status status)
        {
            int responseValue = this.answerConfig.DefaultResponse;
            foreach (var response in this.answerConfig.Responses)
            {
                if (response.Status == status)
                {
                    responseValue = response.Value;
                    break;
                }
            }

            this.answer[(this.byteInfo.ByteSize * 2) - 1] = (byte)('0' + responseValue);
            return this.answer;
        }
    }
}
