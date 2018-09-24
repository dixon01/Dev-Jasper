// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigMngTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ConfigMngTest and is intended
//   to contain all ConfigMngTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Tests
{
    using System;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Protran.AbuDhabi;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for <see cref="ConfigMng"/> and is intended
    /// to contain all <see cref="ConfigMng"/> Unit Tests
    /// </summary>
    [TestClass]
    public class ConfigMngTest
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
        /// A test for Load
        /// </summary>
        [Ignore]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadCfgFileWithInvalidFileNameTest()
        {
            var target = new ConfigMng();
            string fileName = string.Empty;
            var path = PathManager.Instance.GetPath(FileType.Config, fileName);
            target.Load(path);
        }

        /// <summary>
        /// A test for Load
        /// </summary>
        [TestMethod]
        public void LoadContentCfgFileWithContent()
        {
            var target = new ConfigMng();
            target.LoadContent(Constants.ConfigFileContent);
            Assert.IsTrue(target.InitOk);
        }
    }
}
