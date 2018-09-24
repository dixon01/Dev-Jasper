// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser7BitTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for Parser7BitTest and is intended
//   to contain all Parser7BitTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Parsers
{
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for Parser7BitTest and is intended
    /// to contain all Parser7BitTest Unit Tests
    /// </summary>
    [TestClass]
    public class Parser7BitTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

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
        //
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #endregion

        /// <summary>
        /// A test for IsChecksumCorrect
        /// </summary>
        [TestMethod]
        public void IsChecksumCorrectTest()
        {
            var target = new Parser7Bit(true, new List<TelegramConfig>());
            var serialPort7 = new TelegramProvider7Bit();
            while (serialPort7.IsSomethingAvailable)
            {
                byte[] buffer = serialPort7.ReadAvailableBytes();
                Assert.IsTrue(target.IsChecksumCorrect(buffer));
            }

            serialPort7 = new TelegramProvider7Bit();
            while (serialPort7.IsSomethingAvailable)
            {
                byte[] buffer = serialPort7.ReadAvailableBytes();
                buffer[buffer.Length / 2]++; // simulate a bit error
                Assert.IsFalse(target.IsChecksumCorrect(buffer));
            }
        }

        /// <summary>
        /// A test for ReadFrom
        /// </summary>
        [TestMethod]
        public void ReadFromTest()
        {
            byte[] telegram = null;
            var target = new Parser7Bit(true, new List<TelegramConfig>());
            target.TelegramDataReceived += (s, e) => telegram = e.Data;

            var provider = new TelegramProvider7Bit();
            while (provider.IsSomethingAvailable)
            {
                var bytes = provider.ReadAvailableBytes();
                var stream = new MemoryStream(bytes, false);
                var read = target.ReadFrom(stream);
                Assert.IsTrue(read);
                Assert.IsNotNull(telegram);
                CollectionAssert.AreEqual(bytes, telegram);
                telegram = null;

                read = target.ReadFrom(stream);
                Assert.IsFalse(read);
                Assert.IsNull(telegram);
                telegram = null;
            }
        }
    }
}
