// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramParsedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramParsedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using Gorba.Common.Protocols.Vdv300;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis;
    using Gorba.Motion.Protran.Ibis.Parsers;

    /// <summary>
    /// Event argument for a parsed telegram including its
    /// original data, its parser, the resulting telegram and the created answer.
    /// </summary>
    public class TelegramParsedEventArgs : TelegramEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramParsedEventArgs"/> class.
        /// </summary>
        /// <param name="data">
        /// The raw telegram data.
        /// </param>
        /// <param name="parser">
        /// The parser used for this telegram.
        /// </param>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <param name="answer">
        /// The answer telegram.
        /// </param>
        public TelegramParsedEventArgs(byte[] data, ITelegramParser parser, Telegram telegram, Telegram answer)
            : base(telegram)
        {
            this.Data = data;
            this.Parser = parser;
            this.Answer = answer;
        }

        /// <summary>
        /// Gets the raw telegram data.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Gets the parser used for this telegram.
        /// </summary>
        public ITelegramParser Parser { get; private set; }

        /// <summary>
        /// Gets the answer telegram.
        /// </summary>
        public Telegram Answer { get; private set; }
    }
}