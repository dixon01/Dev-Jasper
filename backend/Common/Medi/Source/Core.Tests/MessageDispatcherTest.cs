// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDispatcherTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for MessageDispatcherTest and is intended
//   to contain all MessageDispatcherTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests
{
    using Gorba.Common.Medi.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NLog;
    using NLog.Config;

    /// <summary>
    /// This is a test class for MessageDispatcherTest and is intended
    ///  to contain all MessageDispatcherTest Unit Tests
    /// </summary>
    [TestClass]
    public class MessageDispatcherTest
    {
        #region Public Properties

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
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
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods

        /// <summary>
        /// This is run before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            SimpleConfigurator.ConfigureForConsoleLogging(LogLevel.Trace);
        }

        /// <summary>
        /// A test for Instance
        /// </summary>
        [TestMethod]
        public void InstanceTest()
        {
            Assert.IsNotNull(MessageDispatcher.Instance);
            Assert.AreSame(MessageDispatcher.Instance, MessageDispatcher.Instance);
        }

        #endregion
    }
}