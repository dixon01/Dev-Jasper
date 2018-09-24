// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DaySecTimeTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Protocols.Qnet.Tests
{
    using System;

    using Gorba.Common.Protocols.Qnet;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for DaySecTimeTest and is intended
    /// to contain all DaySecTimeTest Unit Tests
    /// </summary>
    [TestClass]
    public class DaySecTimeTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods and Operators
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// A test for DaySecFromDateTime
        /// </summary>
        [TestMethod]
        public void DaySecFromDateTimeTest_RSpecifTime()
        {
            var dateTime = new DateTime(2012, 07, 25, 18, 22, 35);

            // 18:22:35 => (18 * 3600) + (22*60) + 35 = 66155
            const uint ExpectedDeaySecs = 66155;
            uint actual = DaySecTime.DaySecFromDateTime(dateTime);
            Assert.AreEqual(ExpectedDeaySecs, actual);
        }

        /// <summary>
        /// A test for DaySecFromDateTime
        /// </summary>
        [TestMethod]
        public void DaySecFromDateTimeTest_Midnight_Returns0()
        {
            var dateTime = new DateTime(2012, 07, 25, 0, 0, 0);
            const uint ExpectedDeaySecs = 0;
            uint actual = DaySecTime.DaySecFromDateTime(dateTime);
            Assert.AreEqual(ExpectedDeaySecs, actual);
        }

        /// <summary>
        /// A test for DaySecFromDateTime
        /// </summary>
        [TestMethod]
        public void DaySecFromDateTimeTest_OneSecondBeforeNextDay_Returns86399()
        {
            var dateTime = new DateTime(2012, 07, 24, 23, 59, 59);

            // 23:59:59 => (23 * 3600) + (59*60) + 59 = 86399
            const uint ExpectedDeaySecs = 86399;
            uint actual = DaySecTime.DaySecFromDateTime(dateTime);
            Assert.AreEqual(ExpectedDeaySecs, actual);
        }
        // ReSharper restore InconsistentNaming
        #endregion
    }
}