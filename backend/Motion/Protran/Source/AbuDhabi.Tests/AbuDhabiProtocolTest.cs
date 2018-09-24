// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiProtocolTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for AbuDhabiProtocolTest and is intended
//   to contain all AbuDhabiProtocolTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Tests
{
    using System.IO;
    using System.Threading;

    using Gorba.Motion.Protran.AbuDhabi;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for AbuDhabiProtocolTest and is intended
    /// to contain all AbuDhabiProtocolTest Unit Tests
    /// </summary>
    [TestClass]
    public class AbuDhabiProtocolTest
    {
        private ConfigMng cfgManager;

        private bool ok;

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
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #endregion

        /// <summary>
        /// A test for Configure
        /// </summary>
        [TestMethod]
        [Ignore]
        public void ConfigureTest()
        {
            // todo [WES] rewrite this test to not use Thread.Sleep()
            this.cfgManager = new ConfigMng();
            this.cfgManager.LoadContent(Constants.ConfigFileContent);
            Assert.IsTrue(this.cfgManager.InitOk);
            AbuDhabiProtocol protocol = null;
            try
            {
                protocol = new AbuDhabiProtocol();
            }
            catch (FileNotFoundException)
            {
                // expected exception, because the file doesn't exist.
                // do nothing.
            }

            var starter = new Thread(this.SimulateProtocol);
            starter.Start(protocol);

            // simulation of 3 seconds of Protran's execution...
            Thread.Sleep(3000);

            if (protocol != null)
            {
                Assert.AreEqual("ABU DHABI", protocol.Name);

                // I don't want any excpetion
                protocol.Stop();
            }

            Assert.IsTrue(this.ok);
        }

        private void SimulateProtocol(object protocol)
        {
            var abuDhabiProtocol = protocol as AbuDhabiProtocol;
            if (abuDhabiProtocol != null)
            {
                abuDhabiProtocol.Configure(this.cfgManager);
                this.ok = true;
            }
            else
            {
                this.ok = false;
            }
        }
    }
}
