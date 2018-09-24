// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainUnitConfigTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace EPaper.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.EPaper.MainUnit;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The main unit config test.
    /// </summary>
    [TestClass]
    public class MainUnitConfigTest
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Tests the deserialization of a <see cref="MainUnitConfig"/> without LCD.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Configurations/MainUnit_v1.0.xml")]
        public void TestDeserializationNoLcd()
        {
            var config = this.GetConfig("MainUnit_v1.0.xml");
            Assert.AreEqual("1acdd83ae662c3fa", config.FirmwareHash);
            Assert.AreEqual(1, config.DisplayUnits.Count);
            var displayUnitConfig = config.DisplayUnits.First();
            Assert.AreEqual("2bc88d4f2c8a3eff", displayUnitConfig.FirmwareHash);
            Assert.IsNull(config.LcdConfig);
        }

        /// <summary>
        /// Tests the deserialization of a <see cref="MainUnitConfig"/> with LCD.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Configurations/MainUnit_v1.0_LCD.xml")]
        public void TestDeserializationWithLcd()
        {
            var config = this.GetConfig("MainUnit_v1.0_LCD.xml");
            Assert.AreEqual("1acdd83ae662c3fa", config.FirmwareHash);
            Assert.IsNotNull(config.LcdConfig);
            Assert.AreEqual(TimeSpan.FromMinutes(3), config.LcdConfig.RefreshInterval);
            var displayUnitConfig = config.DisplayUnits.First();
            Assert.AreEqual("2bc88d4f2c8a3eff", displayUnitConfig.FirmwareHash);
        }

        /// <summary>
        /// Tests the deserialization of a <see cref="MainUnitConfig"/> with LCD.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Configurations/MainUnit_v1.0_XsdError.xml")]
        [ExpectedException(typeof(ConfiguratorException))]
        public void TestDeserializationXsdError()
        {
            this.GetConfig("MainUnit_v1.0_XsdError.xml");
        }

        private MainUnitConfig GetConfig(string fileName)
        {
            var config = new ConfigManager<MainUnitConfig>();
            config.FileName = Path.Combine(this.TestContext.DeploymentDirectory, fileName);
            config.XmlSchema = MainUnitConfig.Schema;
            return config.Config;
        }
    }
}