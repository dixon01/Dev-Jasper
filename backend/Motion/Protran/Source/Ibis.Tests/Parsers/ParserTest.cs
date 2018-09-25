// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParserTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ParserTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Parsers
{
    using System;
    using System.Linq;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ParserTest and is intended
    /// to contain all ParserTest Unit Tests
    /// </summary>
    [TestClass]
    public class ParserTest
    {
        /// <summary>
        /// Container of all the (actually) known telegrams.
        /// Actually the telegrams are:
        /// "DS001", "DS001A", "DS005", "DS006", "DS010B", "DS020", "DS021A", "DS030", "GO003", "GO007", HPW074".
        /// </summary>
        private readonly string[] tlgNames =
                                                 {
            "DS001", "DS001A", "DS005", "DS006", "DS010B", "DS020", "DS021A", "DS030", "GO003",
            "GO007", "HPW074"
                                                 };

        #region Public Methods
        /// <summary>
        /// Test for the Parser's constructor.
        /// </summary>
        [TestMethod]
        public void ParserConstructorTest()
        {
            // I want to test the parser's constructor
            // in order to see if is correct the creation
            // of all its "internal" handlers.
            // now I create my 2 parser and I give to them
            // the list with all the available telegrams.
            // inside their constructor, there is also the
            // constructor of all the required handlers.
            var parser7Bit = this.CreateParser(ByteType.Ascii7);
            var parser16Bit = this.CreateParser(ByteType.UnicodeBigEndian);

            // now I need to mock ( to "simulate" ) the external guy
            // that sends us the real telegrams.
            // so, lets' create the referring mock object.
            var serialPort7 = new TelegramProvider7Bit();
            var serialPort16 = new TelegramProvider16Bit();

            // to verify if the handlers creations was did right
            // in the parsers constructor, I'll invoke the parser's
            // function "GetTelegramHandler".
            while (serialPort7.IsSomethingAvailable)
            {
                byte[] telegram7 = serialPort7.ReadAvailableBytes();
                ITelegramParser handler7Bit = parser7Bit.GetTelegramParser(telegram7);
                Assert.IsNotNull(handler7Bit);
            }

            while (serialPort16.IsSomethingAvailable)
            {
                byte[] telegram16 = serialPort16.ReadAvailableBytes();
                ITelegramParser handler16Bit = parser16Bit.GetTelegramParser(telegram16);
                Assert.IsNotNull(handler16Bit);
            }
        }

        /// <summary>
        /// A test for ByteType
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void ByteTypeTest()
        {
            // I want to test that a parser
            // has really the right byte type.
            var parsers = new[] { this.CreateParser(ByteType.Ascii7), this.CreateParser(ByteType.UnicodeBigEndian) };
            foreach (var parser in parsers)
            {
                if (parser is Parser7Bit)
                {
                    Assert.AreEqual(ByteInfo.Ascii7, parser.ByteInfo);
                }
                else if (parser is Parser16Bit)
                {
                    Assert.AreEqual(ByteInfo.UnicodeBigEndian, parser.ByteInfo);
                }
                else
                {
                    Assert.Inconclusive("Verify the correctness of this test method.");
                }
            }
        }

        /// <summary>
        /// A test for GetTelegramHandler
        /// </summary>
        [TestMethod]
        public void GetTelegramParserTest()
        {
            var parser7Bit = this.CreateParser(ByteType.Ascii7);
            var parser16Bit = this.CreateParser(ByteType.UnicodeBigEndian);
            var serialPort7 = new TelegramProvider7Bit();
            var serialPort16 = new TelegramProvider16Bit();

            while (serialPort7.IsSomethingAvailable)
            {
                byte[] telegram7 = serialPort7.ReadAvailableBytes();
                ITelegramParser handler7Bit = parser7Bit.GetTelegramParser(telegram7);
                Assert.IsNotNull(handler7Bit);
            }

            while (serialPort16.IsSomethingAvailable)
            {
                byte[] telegram16 = serialPort16.ReadAvailableBytes();
                ITelegramParser handler16Bit = parser16Bit.GetTelegramParser(telegram16);
                Assert.IsNotNull(handler16Bit);
            }
        }

        /// <summary>
        /// A test for Marker
        /// </summary>
        [TestMethod]
        public void MarkerTest()
        {
            // I want to test that a parser
            // has really the right byte type.
            var random = new Random();
            var parsers = new[] { this.CreateParser(ByteType.Ascii7), this.CreateParser(ByteType.UnicodeBigEndian) };
            var markers = new[] { random.Next(), random.Next() };
            for (var i = 0; i < parsers.Length; i++)
            {
                // I give to the parser a marker.
                parsers[i].Marker = (byte)markers[i];
            }

            // now I want to see if each parser has really
            // the marker that I set to it before.
            for (var i = 0; i < parsers.Length; i++)
            {
                Assert.AreEqual(parsers[i].Marker, (byte)markers[i]);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a parser with a specific byte type
        /// and capable to recognize all the following telegrams:
        /// "DS001", "DS001A", "DS005", "DS006", "DS010B", "DS020", "DS021A", "DS030", "GO003", "GO007", "HPW074"
        /// </summary>
        /// <param name="byteType">The parser's byte type.</param>
        /// <returns>The parser created.</returns>
        private Parser CreateParser(ByteType byteType)
        {
            var telegramsList =
                this.tlgNames.Select(t => new SimpleTelegramConfig { Name = t, Enabled = true }).ToList();

            switch (byteType)
            {
                case ByteType.Ascii7:
                    return new Parser7Bit(true, telegramsList);
                case ByteType.UnicodeBigEndian:
                    return new Parser16Bit(true, telegramsList);
                default:
                    return null;
            }
        }
        #endregion
    }
}
