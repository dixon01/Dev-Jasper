// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetMessageTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using Gorba.Common.Protocols.Qnet;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for QnetMessageTest and is intended
    /// to contain all QnetMessageTest Unit Tests
    /// </summary>
    [TestClass]
    public class QnetMessageTest
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

        /// <summary>
        /// A test for QnetMessage Constructor
        /// </summary>
        [TestMethod]
        public void QnetMessageConstructorTest()
        {
            ushort sourceAddress = 0; // TODO: Initialize to an appropriate value
            ushort destAddress = 0; // TODO: Initialize to an appropriate value
            ushort gatewayAddress = 0; // TODO: Initialize to an appropriate value
            var target = new QnetMessage(sourceAddress, destAddress, gatewayAddress);
            Assert.AreEqual(
                target.SourceAddress, sourceAddress, "Source address is not assigned with the source address parameter");
            Assert.AreEqual(
                target.DestAddress, 
                destAddress, 
                "Destination address is not assigned with the destination address parameter");
        }

        #endregion
    }
}