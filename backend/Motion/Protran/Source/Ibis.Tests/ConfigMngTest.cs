// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigMngTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ConfigMngTest and is intended
//   to contain all ConfigMngTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests
{
    using System.IO;

    using Gorba.Motion.Protran.Ibis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class contains all Unit Tests for <see cref="ConfigMng"/>.
    /// </summary>
    [TestClass]
    public class ConfigMngTest
    {
        /// <summary>
        /// A test for Load
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void LoadTestInvalidFileName()
        {
            var cfgManager = new ConfigMng();
            cfgManager.Load("Invalid file name");
        }

        /// <summary>
        /// A test for Load
        /// </summary>
        [TestMethod]
        [DeploymentItem("ibis.xml")]
        public void LoadTest()
        {
            var cfgManager = new ConfigMng();
            cfgManager.Load("ibis.xml");
        }
    }
}
