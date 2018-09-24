// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ChannelTest and is intended
//   to contain all ChannelTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests
{
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Core.Buffers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ChannelTest and is intended
    /// to contain all ChannelTest Unit Tests
    /// </summary>
    [TestClass]
    public class ChannelTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #endregion

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            SetupHelper.SetupCoreServices();
        }

        /// <summary>
        /// A test for HandleData
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void HandleDataTest()
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
            channel.TelegramReceived += (sender, args) => eventCounter[0]++;
            var serialPort16 = new TelegramProvider16Bit();
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
