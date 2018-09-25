// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QmailStructTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using Gorba.Common.Protocols.Qnet.Structures;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for dosDateTimeTest and is intended
    /// to contain all dosDateTimeTest Unit Tests
    /// </summary>
    [TestClass]
    public class QmailStructTest : BaseStructTest
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
        /// A test for the size in bytes of IqubeCmdMsgStruct
        /// </summary>
        [TestMethod]
        public void QmailHeaderStructLength_SizeOf_Returns27()
        {
            const int Expectedlen = 27;
            this.TestStructLength<QmailHeader>(Expectedlen);
        }       
        
        // ReSharper restore InconsistentNaming
        #endregion
    }
}