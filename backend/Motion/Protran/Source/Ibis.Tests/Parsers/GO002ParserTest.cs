// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO002ParserTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Parsers
{
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Core.Buffers;
    using Gorba.Motion.Protran.Ibis.Parsers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for GO002ParserTest and is intended
    /// to contain all GO002ParserTest Unit Tests
    /// </summary>
    [TestClass]
    public class GO002ParserTest
    {
        private const string AheadValidExpression = @"^([\d]{0,3})' früher$";
        private const string DelayedValidExpression = @"^([\d]{0,3})' später$";
        private const string ExpressionOk = "ok";
        private const string PictogramExpression = @"^D:\\Infomedia\\Symbols\\Conn_([\d]{1}).png$";
        private const string LineNumberExpression = @"^D:\\Infomedia\\Symbols\\L([\d]{5}).png$";

        private static readonly GO002Config Config = new GO002Config
        {
            TransfRef = "ConnectionDest",
            CheckLength = true,
            StopIndexSize = 2,
            RowNumberSize = 1,
            PictogramSize = 1,
            LineNumberSize = 5,
            TrackNumberSize = 2,
            ScheduleDeviationSize = 4,
            FirstStopIndex = 1,
            FirstRowIndex = 1,
            PictogramFormat = "D:\\Infomedia\\Symbols\\Conn_{0}.png",
            LineNumberFormat = "D:\\Infomedia\\Symbols\\L{0}.png",
            ScheduleDeviation = new ScheduleDeviation { OnTime = "ok", Ahead = "{0}' früher", Delayed = "{0}' später" },
            ShowForNextStopOnly = false,
            UsedForPictogram = new GenericUsage { Column = "8", Table = "Connections", Row = "{0}" },
            UsedForLineNumber = new GenericUsage { Column = "1", Table = "Connections", Row = "{0}" },
            UsedForDepartureTime = new GenericUsage { Column = "3", Table = "Connections", Row = "{0}" },
            UsedForTrackNumber = new GenericUsage { Column = "7", Table = "Connections", Row = "{0}" },
            UsedForScheduleDeviation = new GenericUsage { Column = "9", Table = "Connections", Row = "{0}" },
            UsedFor = new GenericUsage { Column = "4", Table = "Connections", Row = "{0}" }
        };

        private readonly Regex aheadValidator = new Regex(AheadValidExpression);
        private readonly Regex delayedValidator = new Regex(DelayedValidExpression);
        private readonly Regex validatorinTime = new Regex(ExpressionOk);
        private readonly Regex validatorPictogram = new Regex(PictogramExpression);
        private readonly Regex validatorLineNumber = new Regex(LineNumberExpression);

        /// <summary>
        /// A test for GO002Parser data length verification
        /// </summary>
        [TestMethod]
        public void CheckLengthTest()
        {
            ByteInfo byteInfo = ByteInfo.For(ByteType.Ascii7);
            ITelegramParser parser = new GO002Parser();
            parser.Configure(byteInfo, Config);
            var serialPort7 = new TelegramProvider7Bit();
            while (serialPort7.IsSomethingAvailable)
            {
                byte[] buffer = serialPort7.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    if (Config.CheckLength)
                    {
                        Assert.AreEqual(32, telegram.DataLength);
                    }
                }
            }

            parser.Configure(ByteInfo.UnicodeBigEndian, Config);
            var serialPort16 = new TelegramProvider16Bit();
            while (serialPort16.IsSomethingAvailable)
            {
                byte[] buffer = serialPort16.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    if (Config.CheckLength)
                    {
                        Assert.AreEqual(69, telegram.DataLength);
                    }
                }
            }
        }

        /// <summary>
        /// A test for GO002Parser departure time
        /// </summary>
        [TestMethod]
        public void DepartureTimeTest()
        {
            ByteInfo byteInfo = ByteInfo.For(ByteType.Ascii7);
            ITelegramParser parser = new GO002Parser();
            parser.Configure(byteInfo, Config);
            var serialPort7 = new TelegramProvider7Bit();
            while (serialPort7.IsSomethingAvailable)
            {
                byte[] buffer = serialPort7.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    var regex = new Regex("^(\\d\\d):(\\d\\d)$");
                    var ok = regex.Match(telegram.DepartureTime).Success;
                    Assert.IsTrue(ok);
                }
            }

            parser.Configure(ByteInfo.UnicodeBigEndian, Config);
            var serialPort16 = new TelegramProvider16Bit();
            while (serialPort16.IsSomethingAvailable)
            {
                byte[] buffer = serialPort16.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    var regex = new Regex("^(\\d\\d):(\\d\\d)$");
                    var ok = regex.Match(telegram.DepartureTime).Success;
                    Assert.IsTrue(ok);
                }
            }
        }

        /// <summary>
        /// A test for GO002Parser schedule deviation
        /// </summary>
        [TestMethod]
        public void ScheduleDeviationTest()
        {
            ITelegramParser parser = new GO002Parser();
            parser.Configure(ByteInfo.Ascii7, Config);
            var serialPort7 = new TelegramProvider7Bit();
            while (serialPort7.IsSomethingAvailable)
            {
                byte[] buffer = serialPort7.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    var ok = this.validatorinTime.Match(telegram.Deviation).Success;
                    ok |= this.aheadValidator.Match(telegram.Deviation).Success;
                    ok |= this.delayedValidator.Match(telegram.Deviation).Success;
                    Assert.IsTrue(ok);
                }
            }

            parser.Configure(ByteInfo.UnicodeBigEndian, Config);
            var serialPort16 = new TelegramProvider16Bit();
            while (serialPort16.IsSomethingAvailable)
            {
                byte[] buffer = serialPort16.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    var ok = this.validatorinTime.Match(telegram.Deviation).Success;
                    ok |= this.aheadValidator.Match(telegram.Deviation).Success;
                    ok |= this.delayedValidator.Match(telegram.Deviation).Success;
                    Assert.IsTrue(ok);
                }
            }
        }

        /// <summary>
        /// A test for GO002Parser pictogram format
        /// </summary>
        [TestMethod]
        public void PictogramFormatTest()
        {
            ITelegramParser parser = new GO002Parser();
            parser.Configure(ByteInfo.Ascii7, Config);
            var serialPort7 = new TelegramProvider7Bit();
            while (serialPort7.IsSomethingAvailable)
            {
                byte[] buffer = serialPort7.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    var ok = this.validatorPictogram.Match(telegram.Pictogram).Success;
                    Assert.IsTrue(ok);
                }
            }

            parser.Configure(ByteInfo.UnicodeBigEndian, Config);
            var serialPort16 = new TelegramProvider16Bit();
            while (serialPort16.IsSomethingAvailable)
            {
                byte[] buffer = serialPort16.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    var ok = this.validatorPictogram.Match(telegram.Pictogram).Success;
                    Assert.IsTrue(ok);
                }
            }
        }

        /// <summary>
        /// A test for GO002Parser line number format
        /// </summary>
        [TestMethod]
        public void LineNumberFormatTest()
        {
            ITelegramParser parser = new GO002Parser();
            parser.Configure(ByteInfo.Ascii7, Config);
            var serialPort7 = new TelegramProvider7Bit();
            while (serialPort7.IsSomethingAvailable)
            {
                byte[] buffer = serialPort7.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    var ok = this.validatorLineNumber.Match(telegram.LineNumber).Success;
                    Assert.IsTrue(ok);
                }
            }

            parser.Configure(ByteInfo.UnicodeBigEndian, Config);
            var serialPort16 = new TelegramProvider16Bit();
            while (serialPort16.IsSomethingAvailable)
            {
                byte[] buffer = serialPort16.ReadAvailableBytes();
                if (parser.Accept(buffer))
                {
                    var telegram = (GO002)parser.Parse(buffer);
                    Assert.IsNotNull(telegram);
                    var ok = this.validatorLineNumber.Match(telegram.LineNumber).Success;
                    Assert.IsTrue(ok);
                }
            }
        }

        /// <summary>
        /// A test for GO002Parser destination name format
        /// </summary>
        [TestMethod]
        public void DestinationNameTest()
        {
            var loader = new IbisCfgLoader(true);
            var ibisConfig = loader.Load();
            var dictionary = new Dictionary();
            var channel = new MockChannel(new IbisConfigContextMock(dictionary, ibisConfig));
            var serialPort7 = new TelegramProvider7Bit();
            int[] eventCounter = { 0 };
            channel.TelegramReceived += (sender, args) => eventCounter[0]++;

            while (serialPort7.IsSomethingAvailable)
            {
                byte[] telegram = serialPort7.ReadAvailableBytes();
                eventCounter[0] = 0;
                channel.HandleData(telegram);
                Assert.AreEqual(
                    1, eventCounter[0], "Couldn't handle telegram {0}", BufferUtils.FromByteArrayToHexString(telegram));
            }

            loader = new IbisCfgLoader(false);
            ibisConfig = loader.Load();
            channel = new MockChannel(new IbisConfigContextMock(dictionary, ibisConfig));
            var serialPort16 = new TelegramProvider16Bit();
            eventCounter[0] = 0;
            channel.TelegramReceived += (sender, args) => eventCounter[0]++;

            while (serialPort16.IsSomethingAvailable)
            {
                byte[] telegram = serialPort16.ReadAvailableBytes();
                eventCounter[0] = 0;
                channel.HandleData(telegram);
                Assert.AreEqual(
                    1, eventCounter[0], "Couldn't handle telegram {0}", BufferUtils.FromByteArrayToHexString(telegram));
            }
        }
    }
}
