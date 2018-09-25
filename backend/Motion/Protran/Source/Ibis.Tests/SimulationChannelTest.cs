// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimulationChannelTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests
{
    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Ibis.Channels;
    using Gorba.Motion.Protran.Ibis.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for SimulationTest and is intended
    /// to contain all SimulationTest Unit Tests
    /// </summary>
    [TestClass]
    public class SimulationChannelTest
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
        #region Public Methods

        /// <summary>
        /// A test for Simulation Constructor
        /// </summary>
        [TestMethod]
        public void SimulationChannelConstructorTest()
        {
            var config = new IbisConfig
                             {
                                 Sources =
                                     new IbisSourcesConfig
                                         {
                                             Simulation =
                                                 new SimulationConfig
                                                     {
                                                         SimulationFile = string.Empty
                                                     }
                                         }
                             };
            var dictionary = new Dictionary();
            var target = new SimulationChannel(new IbisConfigContextMock(dictionary, config));
            Assert.IsFalse(target.IsRunning);
            Assert.IsFalse(target.IsOpen);
        }

        #endregion
    }
}