// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleHeaderTelegramParserTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Summary description for SimpleHeaderTelegramParserTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Parsers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for SimpleTelegramParser
    /// </summary>
    [TestClass]
    public class SimpleHeaderTelegramParserTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Test HeaderSize property.
        /// </summary>
        [TestMethod]
        public void HeaderSizeTest()
        {
            var config = new SimpleTelegramConfig { Enabled = true, Name = "DS001" };
            var ds001 = new SimpleHeaderTelegramParser<DS001>("l");

            ds001.Configure(ByteInfo.Ascii7, config);
            Assert.AreEqual(1, ds001.HeaderSize);

            ds001.Configure(ByteInfo.UnicodeBigEndian, config);
            Assert.AreEqual(2, ds001.HeaderSize);

            var ds001A = new SimpleHeaderTelegramParser<DS001A>("lE");

            ds001A.Configure(ByteInfo.Ascii7, config);
            Assert.AreEqual(2, ds001A.HeaderSize);

            ds001A.Configure(ByteInfo.UnicodeBigEndian, config);
            Assert.AreEqual(4, ds001A.HeaderSize);
        }

        /// <summary>
        /// Test FooterSize property.
        /// </summary>
        [TestMethod]
        public void FooterSizeTest()
        {
            var config = new SimpleTelegramConfig { Enabled = true, Name = "DS001" };
            var ds001 = new SimpleHeaderTelegramParser<DS001>("l");

            ds001.Configure(ByteInfo.Ascii7, config);
            Assert.AreEqual(2, ds001.FooterSize);

            ds001.Configure(ByteInfo.UnicodeBigEndian, config);
            Assert.AreEqual(4, ds001.FooterSize);

            var ds001A = new SimpleHeaderTelegramParser<DS001A>("lE");

            ds001A.Configure(ByteInfo.Ascii7, config);
            Assert.AreEqual(2, ds001A.FooterSize);

            ds001A.Configure(ByteInfo.UnicodeBigEndian, config);
            Assert.AreEqual(4, ds001A.FooterSize);
        }

        /// <summary>
        /// Test Parse() method with DS001
        /// </summary>
        [TestMethod]
        public void ParseDS001Test()
        {
            var config = new SimpleTelegramConfig { Enabled = true, Name = "DS001" };
            ITelegramParser target = new SimpleHeaderTelegramParser<DS001>("l");

            target.Configure(ByteInfo.Ascii7, config);
            var telegram = target.Parse(new byte[] { 0x6C, 0x39, 0x35, 0x37, 0x0D, 0x3C });

            Assert.IsNotNull(telegram);
            Assert.IsNotNull(telegram.Payload);
            Assert.AreEqual(3, telegram.Payload.Length);

            target.Configure(ByteInfo.UnicodeBigEndian, config);
            telegram =
                target.Parse(new byte[] { 0x00, 0x6C, 0x00, 0x39, 0x00, 0x35, 0x00, 0x37, 0x00, 0x0D, 0xFF, 0x3C });

            Assert.IsNotNull(telegram);
            Assert.IsNotNull(telegram.Payload);
            Assert.AreEqual(6, telegram.Payload.Length);
        }
    }
}
