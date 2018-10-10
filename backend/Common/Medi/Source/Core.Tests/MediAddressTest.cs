// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediAddressTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for MediAddressTest and is intended
//   to contain all MediAddressTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests
{
    using Gorba.Common.Medi.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for MediAddressTest and is intended
    ///   to contain all MediAddressTest Unit Tests
    /// </summary>
    [TestClass]
    public class MediAddressTest
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
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
        #region Public Methods

        /// <summary>
        /// A test for MediAddress Constructor
        /// </summary>
        [TestMethod]
        public void MediAddressConstructorTest()
        {
            var target = new MediAddress();
            Assert.AreEqual(target.Application, "*");
            Assert.AreEqual(target.Unit, "*");
        }

        /// <summary>
        /// A test for ToString
        /// </summary>
        [TestMethod]
        public void ToStringTest()
        {
            var target = new MediAddress();
            string actual = target.ToString();
            Assert.AreEqual("*:*", actual);

            target = new MediAddress { Application = "App", Unit = "Unit" };
            actual = target.ToString();
            Assert.AreEqual("Unit:App", actual);
        }

        #endregion
    }
}