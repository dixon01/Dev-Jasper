// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramParserFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Factory for <see cref="ITelegramParser"/> implementations.
    /// </summary>
    public class TelegramParserFactory
    {
        /// <summary>
        /// Creates a parser for the given config.
        /// </summary>
        /// <param name="byteInfo">
        /// The byte information.
        /// </param>
        /// <param name="config">
        /// The telegram config.
        /// </param>
        /// <returns>
        /// A new parser for the given config.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// If <see cref="TelegramConfig.Name"/> is not known to this factory.
        /// </exception>
        public ITelegramParser CreateParser(ByteInfo byteInfo, TelegramConfig config)
        {
            var parser = this.CreateParser(config.Name);
            parser.Configure(byteInfo, config);
            return parser;
        }

        private ITelegramParser CreateParser(string name)
        {
            switch (name.ToUpper())
            {
                case "DS001":
                    // Attention: this is an L in lowercase
                    return new SimpleHeaderTelegramParser<DS001>("l", new Digit(3, 6), new EndOfTelegram());
                case "DS001A":
                    // Attention: this is an L in lowercase
                    return new SimpleHeaderTelegramParser<DS001A>("lE", new Digit(2, 4), new EndOfTelegram());
                case "DS002":
                    return new SimpleHeaderTelegramParser<DS002>("k", new Digit(2, 3), new EndOfTelegram());
                case "DS003":
                    return new SimpleHeaderTelegramParser<DS003>("z", new Digit(3, 4), new EndOfTelegram());
                case "DS003A":
                    return new SimpleHeaderTelegramParser<DS003A>("zA", new HexDigit());
                case "DS003C":
                    return new DS003CParser();
                case "DS005":
                    return new SimpleHeaderTelegramParser<DS005>("u", new Digit(4), new EndOfTelegram());
                case "DS006":
                    return new SimpleHeaderTelegramParser<DS006>("d", new Digit(5, 6), new EndOfTelegram());
                case "DS006A":
                    return new SimpleHeaderTelegramParser<DS006A>("dU", new Digit(14), new EndOfTelegram());
                case "DS008":
                    return new SimpleHeaderTelegramParser<DS008>("n", new HexDigit(3), new EndOfTelegram());
                case "DS009":
                    return new SimpleHeaderTelegramParser<DS009>("v", new Any(16, 24), new EndOfTelegram());
                case "DS010":
                    return new SimpleHeaderTelegramParser<DS010>("x", new Digit(4, 5), new EndOfTelegram());
                case "DS010B":
                    // Attention: this is an I in uppercase
                    return new SimpleHeaderTelegramParser<DS010B>("xI", new Digit(2, 3), new EndOfTelegram());
                case "DS010J":
                    return new SimpleHeaderTelegramParser<DS010J>("x", new Digit(4, 5), new EndOfTelegram());
                case "DS020":
                    return new AnswerWithDS120Parser<DS020>("a", new HexDigit(), new EndOfTelegram());
                case "DS021":
                    return new DS021Parser();
                case "DS021A":
                    return new DS021AParser();
                case "DS021C":
                    return new DS021CParser();
                case "DS030":
                    return new AnswerWithDS130Parser<DS030>("hS", new EndOfTelegram());
                case "DS036":
                    return new DS036Parser();
                case "DS080":
                    return new SimpleHeaderTelegramParser<DS080>("bT", new EndOfTelegram());
                case "DS081":
                    return new SimpleHeaderTelegramParser<DS081>("bM", new EndOfTelegram());
                case "DS120":
                    return new SimpleHeaderTelegramParser<DS120>("a", new Digit(), new EndOfTelegram());
                case "DS130":
                    return new SimpleHeaderTelegramParser<DS130>("h", new Digit(), new EndOfTelegram());
                case "GO001":
                case "EVENTTELEGRAM": // old name
                    return new SimpleHeaderTelegramParser<GO001>("xE", new HexDigit(), new Digit(2));
                case "GO002":
                    return new GO002Parser();
                case "GO003":
                    return new GO003Parser();
                case "GO004":
                    return new GO004Parser();
                case "GO005":
                    return new GO005Parser();
                case "GO006":
                    return new SimpleHeaderTelegramParser<GO006>(
                        "z", new Constant("0"), new Any(3, 5), new EndOfTelegram());
                case "GO007":
                    return new GO007Parser();
                case "HPW074":
                    return new AnswerWithDS120Parser<HPW074>(
                        "sN", new HexDigit(), new Digit(1, 3), new EndOfTelegram());
            }

            throw new NotSupportedException("Telegram type not supported: " + name);
        }
    }
}
