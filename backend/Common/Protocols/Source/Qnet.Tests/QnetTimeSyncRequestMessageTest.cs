// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetTimeSyncRequestMessageTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Protocols.Qnet;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for QnetTimeSyncRequestMessageTest and is intended
    /// to contain all QnetTimeSyncRequestMessageTest Unit Tests
    /// </summary>
    [TestClass]
    public class QnetTimeSyncRequestMessageTest
    {
        #region Constants and Fields

        /// <summary>
        /// The basis modul has a static address = 7 (A:0.0.7)
        /// </summary>
        private const ushort BasisModulStaticAddress = 7;

        /// <summary>
        /// The qnet address for A:2.1.1 = 4105
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", 
            Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable InconsistentNaming
            private const ushort QnetAddressFor_A_2_1_1 = 4105;

        #endregion

        // ReSharper restore InconsistentNaming
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

        /// <summary>
        /// A test for QnetTimeSyncRequestMessage Constructor
        /// </summary>
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void QnetTimeSyncRequestMessage_ConstructorTest()
        {
            // ReSharper restore InconsistentNaming
            DateTime originateTime = DateTime.Now;
            var target = new QnetTimeSyncRequestMessage(BasisModulStaticAddress, QnetAddressFor_A_2_1_1, originateTime, 0);
            Assert.AreEqual(target.DottedSourceAddress, "A:0.0.7", "The dotted qnet source address was not calculated.");
            Assert.AreEqual(target.OriginateTime, originateTime, "The originateTime was not assigned.");
        }

        #endregion
    }
}