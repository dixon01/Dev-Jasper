// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeMonitoringStructTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for all RealtimeStruct
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using Gorba.Common.Protocols.Qnet.Structures;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for all RealtimeStruct 
    /// </summary>
    [TestClass]
    public class RealtimeMonitoringStructTest : BaseStructTest
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
        /// A test for the size in bytes of RealtimeStartStruct
        /// </summary>
        [TestMethod]
        public void RealtimeStartStructLength_SizeOf_Returns4()
        {
            const int Expectedlen = 4;
            this.TestStructLength<RealtimeStartStruct>(Expectedlen);
        }
        
        /// <summary>
        /// A test for the size in bytes of RealtimeInfoLineStruct
        /// </summary>
        [TestMethod]
        public void RealtimeInfoLineStructLength_SizeOf_Returns164()
        {
            const int Expectedlen = 164;
            this.TestStructLength<RealtimeInfoLineStruct>(Expectedlen);
        }

        /// <summary>
        /// A test for the size in bytes of RealtimeDataTypeSStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDataTypeSStructLength_SizeOf_Returns3()
        {
            const int Expectedlen = 3;
            this.TestStructLength<RealtimeDataTypeSStruct>(Expectedlen);
        }

        /// <summary>
        /// A test for the size in bytes of RealtimeDataTypeCStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDataTypeCStructLength_SizeOf_Returns32()
        {
            const int Expectedlen = 32;
            this.TestStructLength<RealtimeDataTypeCStruct>(Expectedlen);
        }
                        
        /// <summary>
        /// A test for the size in bytes of RealtimeLineDataStruct
        /// </summary>
        [TestMethod]
        public void RealtimeLineDataStructLength_SizeOf_Returns10()
        {
            const int Expectedlen = 10;
            this.TestStructLength<RealtimeLineDataStruct>(Expectedlen);
        }
        
        /// <summary>
        /// A test for the size in bytes of RealtimeDestinationDataStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDestinationDataStructLength_SizeOf_Returns34()
        {
            const int Expectedlen = 34;
            this.TestStructLength<RealtimeDestinationDataStruct>(Expectedlen);
        }
        
        /// <summary>
        /// A test for the size in bytes of RealtimeDepartureTimeDataStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDepartureTimeDataStructLength_SizeOf_Returns10()
        {
            const int Expectedlen = 10;
            this.TestStructLength<RealtimeDepartureTimeDataStruct>(Expectedlen);
        }

        /// <summary>
        /// A test for the size in bytes of RealtimeDataTypeMStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDataTypeMStructLength_SizeOf_Returns220()
        {
            const int Expectedlen = 220;
            this.TestStructLength<RealtimeDataTypeMStruct>(Expectedlen);
        }
                
        /// <summary>
        /// A test for the size in bytes of RealtimeLaneDataStruct
        /// </summary>
        [TestMethod]
        public void RealtimeLaneDataStructLength_SizeOf_Returns10()
        {
            const int Expectedlen = 10;
            this.TestStructLength<RealtimeLaneDataStruct>(Expectedlen);
        }

        /// <summary>
        /// A test for the size in bytes of RealtimeDataTypeLStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDataTypeLStructLength_SizeOf_Returns195()
        {
            const int Expectedlen = 195;
            this.TestStructLength<RealtimeDataTypeLStruct>(Expectedlen);
        }
        
        /// <summary>
        /// A test for the size in bytes of RealtimeDataStruct
        /// </summary>
        [TestMethod]
        public void RealtimeDataStructLength_SizeOf_Returns222()
        {
            const int Expectedlen = 222;
            this.TestStructLength<RealtimeDataStruct>(Expectedlen);
        }

        /// <summary>
        /// A test for the size in bytes of IqubeCmdMsgStruct
        /// </summary>
        [TestMethod]
        public void RealtimeMonitoringStructLength_SizeOf_Returns224()
        {
            const int Expectedlen = 224;
            this.TestStructLength<RealtimeMonitoringStruct>(Expectedlen);
        }
        
        // ReSharper restore InconsistentNaming
        #endregion
    }
}