// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnswerWithDS130Parser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnswerWithDS130Parser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Parser for telegrams that require a DS130 answer.
    /// </summary>
    /// <typeparam name="T">
    /// The type of telegram this handler creates in its <see cref="TelegramParser{T}.Parse"/> method.
    /// </typeparam>
    public class AnswerWithDS130Parser<T> : SimpleHeaderTelegramParser<T>
        where T : Telegram, new()
    {
        private DS130Config answerConfig;
        private DS130Factory ds130Factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerWithDS130Parser{T}"/> class.
        /// </summary>
        /// <param name="header">
        /// The header characters.
        /// </param>
        /// <param name="rules">
        /// The telegram checks used to check if the incoming telegram is valid.
        /// </param>
        public AnswerWithDS130Parser(string header, params ITelegramRule[] rules)
            : base(header, rules)
        {
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
            this.answerConfig = config.Answer.Telegram as DS130Config;

            if (this.answerConfig != null)
            {
                this.ds130Factory = new DS130Factory(this.answerConfig, byteInfo);
            }

            base.Configure(byteInfo, config);
        }

        /// <summary>
        /// Checks whether the given telegram needs an answer to be sent.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// true if the IBIS channel has to send an answer for the given telegram
        /// </returns>
        public override bool RequiresAnswer(byte[] telegram)
        {
            return this.answerConfig != null && this.answerConfig.Enabled;
        }

        /// <summary>
        /// Creates the IBIS telegram as answer for the incoming telegram.
        /// </summary>
        /// <param name="telegram">The telegram including header, marker and checksum.</param>
        /// <param name="status">The status with which make the answer.</param>
        /// <returns>This method always returns null.</returns>
        public override byte[] CreateAnswer(byte[] telegram, Status status)
        {
            return this.ds130Factory.CreateAnswer(telegram, status);
        }
    }
}
