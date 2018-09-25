// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetTftpMessageTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for QnetSNMPMessageTest and is intended
//   to contain all QnetSNMPMessageTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using Gorba.Common.Protocols.Qnet;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for QnetSNMPMessageTest and is intended
    /// to contain all QnetSNMPMessageTest Unit Tests
    /// </summary>
    [TestClass]
    public class QnetTftpMessageTest
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
        /// A test for QnetTFTPMessage default Constructor
        /// </summary>
        [TestMethod]
        public void TestDefaultContrcutor()
        {
            var target = new QnetTFTPMessage();
            Assert.IsNotNull(target.Tftp, "The Tftp has to be instanciated into the constructor");
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// A test for QnetTFTPMessage Constructor with addresses
        /// </summary>
        [TestMethod]
        public void QnetTFTPMessageConstructor_WithSourcAndDest()
        {
            const ushort SourceAddress = 0; 
            const ushort DestAddress = 0;
            const ushort GatewayAddress = 0;
            var target = new QnetTFTPMessage(SourceAddress, DestAddress, GatewayAddress);
            Assert.IsNotNull(target.Tftp, "The Tftp has to be instanciated into the constructor");
        }

        #endregion
        // ReSharper restore InconsistentNaming
    }
}