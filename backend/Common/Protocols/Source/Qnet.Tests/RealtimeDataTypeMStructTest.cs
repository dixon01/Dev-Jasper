// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeDataTypeMStructTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Protocols.Qnet.Tests
{
    using System;

    using Gorba.Common.Protocols.Qnet.Structures;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for RealtimeDataTypeMStructTest and is intended
    /// to contain all RealtimeDataTypeMStructTest Unit Tests
    /// </summary>
    [TestClass]
    public class RealtimeDataTypeMStructTest
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
        /// A test for RealtimeDepartureTimeDataStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDepartureTimeDataStructTest()
        {
            const int MaxEntries = 4;
            const string TestText = "TEXT_";
            var target = new RealtimeDataTypeMStruct();

            var departureTime = new RealtimeDepartureTimeDataStruct[MaxEntries];
            for (var i = 0; i < MaxEntries; i++)
            {
                departureTime[i].Font = (sbyte)i;
                departureTime[i].Attributes = (byte)i;
                departureTime[i].Text = TestText + i.ToString();
            }

            target.DepartureTime = departureTime;

            var actual = target.DepartureTime;

            for (var i = 0; i < MaxEntries; i++)
            {
                var expectedText = TestText + i.ToString();
                Assert.AreEqual((sbyte)i, actual[i].Font);
                Assert.AreEqual((byte)i, actual[i].Attributes);
                Assert.AreEqual(expectedText, actual[i].Text);
            }
        }

        /// <summary>
        /// A test for RealtimeDepartureTimeDataStruct for string that contains more than the max length of the byte array.
        /// This true for Departure time, destination and line
        /// </summary>
        [TestMethod]
        public void DataStructTest_TextLengthOver8Characters_ExpectedTextTruncated()
        {
            // ARRANGE
            const int MaxEntries = 4;
            const string TestText = "TEXT MAXIMUN";
            const string TestDestinationText = "TEXT MAXTEXT MAXTEXT MAXTEXT MAXIMUM";
            const string ExpectedText = "TEXT MAX";
            const string ExpectedDestinationText = "TEXT MAXTEXT MAXTEXT MAXTEXT MAX";
            var target = new RealtimeDataTypeMStruct();

            var departureTime = new RealtimeDepartureTimeDataStruct[MaxEntries];
            var destinations = new RealtimeDestinationDataStruct[MaxEntries];
            var lines = new RealtimeLineDataStruct[MaxEntries];
            for (var i = 0; i < MaxEntries; i++)
            {
                departureTime[i].Text = TestText + i.ToString();
                destinations[i].Text = TestDestinationText + i.ToString();
                lines[i].Text = TestText + i.ToString();
            }

            // ACT
            target.DepartureTime = departureTime;
            target.Destination = destinations;
            target.Line = lines;

            // ASSERT
            var actualTime = target.DepartureTime;
            var actualDestinations = target.Destination;
            var actualLines = target.Line;
            for (var i = 0; i < MaxEntries; i++)
            {
                Assert.AreEqual(ExpectedText, actualTime[i].Text, "Departure time");
                Assert.AreEqual(ExpectedDestinationText, actualDestinations[i].Text, "Destinations");
                Assert.AreEqual(ExpectedText, actualLines[i].Text, "Lines");
            }
        }

        /// <summary>
        /// A test for RealtimeDepartureTimeDataStructTest with a bad entries number. Expected an ArgumentOutOfRangeException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RealtimeDepartureTimeDataStructTest_ExceedEntriesNumber_ExpectedException()
        {            
            // ARRANGE
            const int MaxEntries = 4;
            var target = new RealtimeDataTypeMStruct();
            var departureTime = new RealtimeDepartureTimeDataStruct[MaxEntries + 1];

            // ACT
            target.DepartureTime = departureTime;

            // ASSERT
            // Exepcted execption
        }      

        /// <summary>
        /// A test for RealtimeDestinationDataStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDestinationDataStructTest()
        {
            const int MaxEntries = 4;
            const string TestText = "TEXT_";
            var target = new RealtimeDataTypeMStruct();

            var destinations = new RealtimeDestinationDataStruct[MaxEntries];
            for (var i = 0; i < MaxEntries; i++)
            {
                destinations[i].Font = (sbyte)i;
                destinations[i].Attributes = (byte)i;
                destinations[i].Text = TestText + i.ToString();
            }

            target.Destination = destinations;

            var actual = target.Destination;

            for (var i = 0; i < MaxEntries; i++)
            {
                var expectedText = TestText + i.ToString();
                Assert.AreEqual((sbyte)i, actual[i].Font);
                Assert.AreEqual((byte)i, actual[i].Attributes);
                Assert.AreEqual(expectedText, actual[i].Text);
            }
        }

        /// <summary>
        /// A test for RealtimeDestinationDataStruct with a bad entries number. Expected an ArgumentOutOfRangeException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RealtimeDestinationDataStructTest_ExceedEntriesNumber_ExpectedException()
        {
            // ARRANGE
            const int MaxEntries = 4;
            var target = new RealtimeDataTypeMStruct();
            var destinations = new RealtimeDestinationDataStruct[MaxEntries + 1];

            // ACT
            target.Destination = destinations;

            // ASSERT
            // Exepcted execption
        }

        /// <summary>
        /// A test for RealtimeLineDataStruct with a bad entries number. Expected an ArgumentOutOfRangeException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RealtimeLineDataStructTest_ExceedEntriesNumber_ExpectedException()
        {
            // ARRANGE
            const int MaxEntries = 4;
            var target = new RealtimeDataTypeMStruct();
            var lines = new RealtimeLineDataStruct[MaxEntries + 1];

            // ACT
            target.Line = lines;

            // ASSERT
            // Exepcted execption
        }

        /// <summary>
        /// A test for RealtimeLineDataStruct
        /// </summary>
        [TestMethod]
        public void RealtimeLineDataStructTest()
        {
            const int MaxEntries = 4;
            const string TestText = "TEXT_";
            var target = new RealtimeDataTypeMStruct();

            var lines = new RealtimeLineDataStruct[MaxEntries];
            for (var i = 0; i < MaxEntries; i++)
            {
                lines[i].Font = (sbyte)i;
                lines[i].Attributes = (byte)i;
                lines[i].Text = TestText + i.ToString();
            }

            target.Line = lines;

            var actual = target.Line;

            for (var i = 0; i < MaxEntries; i++)
            {
                var expectedText = TestText + i.ToString();
                Assert.AreEqual((sbyte)i, actual[i].Font);
                Assert.AreEqual((byte)i, actual[i].Attributes);
                Assert.AreEqual(expectedText, actual[i].Text);
            }
        }

        /// <summary>
        /// A test for RowsNumber
        /// </summary>
        [TestMethod]
        public void RowsNumberTest()
        {
            const int MaxEntries = 4;
            var target = new RealtimeDataTypeMStruct();
            byte[] expected = { 1, 2, 3, 4 };
            target.RowsNumber = expected;
            byte[] actual = target.RowsNumber;

            for (var i = 0; i < MaxEntries; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        /// <summary>
        /// A test for RowsNumber with a bad entries number. Expected an ArgumentOutOfRangeException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RowsNumberTest_ExceedEntriesNumber_ExpectedException()
        {
            // ARRANGE
            var target = new RealtimeDataTypeMStruct();
            byte[] expected = { 1, 2, 3, 4, 5 };

            // ACT
            target.RowsNumber = expected;

            // ASSERT
            // Exepcted execption
        }

        // ReSharper restore InconsistentNaming
        #endregion
    }
}