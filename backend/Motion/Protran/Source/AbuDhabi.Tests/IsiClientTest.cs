// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiClientTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for IsiClientTest and is intended
//   to contain all IsiClientTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Tests
{
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Protran.AbuDhabi;
    using Gorba.Motion.Protran.AbuDhabi.Isi;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for IsiClientTest and is intended
    /// to contain all IsiClientTest Unit Tests
    /// </summary>
    [TestClass]
    public class IsiClientTest
    {
        #region Additional test attributes
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }
        //
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #endregion

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            var serviceContainer = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => new ServiceContainerLocator(serviceContainer));
            Gorba.Motion.Protran.Core.Protran.SetupCoreServices();
        }

        /// <summary>
        /// A test for IsiClient Constructor
        /// </summary>
        [TestMethod]
        public void IsiClientConstructorTest()
        {
            var cfgMng = new ConfigMng();
            cfgMng.LoadContent(Constants.ConfigFileContent);

            Assert.IsTrue(cfgMng.InitOk);

            var isiClient = new IsiClient(new Dictionary(), cfgMng.AbuDhabiConfig);

            // I don't want any exception

            // this.isiClient.Start();
            // simulate 3 seconds of ISI communications...
            // Thread.Sleep(3000);
            // I don't want any excpetion.
            // this.isiClient.Stop();
        }
    }
}
