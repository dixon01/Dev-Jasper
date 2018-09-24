// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComposerCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Tests.Compatibility.Composer
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.Composer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The Audio Renderer file compatibility test.
    /// </summary>
    [TestClass]
    public class ComposerCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>Composer.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Composer\Composer_v2.2.xml")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<ComposerConfig>();
            configManager.FileName = "Composer_v2.2.xml";
            configManager.XmlSchema = ComposerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.XimpleInactivity);
            Assert.AreEqual(TimeSpan.FromSeconds(60), config.XimpleInactivity.Timeout);
            Assert.IsTrue(config.XimpleInactivity.AtStartup);
        }

        /// <summary>
        /// Tests <c>Composer.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Composer\Composer_v2.4.xml")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<ComposerConfig>();
            configManager.FileName = "Composer_v2.4.xml";
            configManager.XmlSchema = ComposerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.XimpleInactivity);
            Assert.AreEqual(TimeSpan.FromSeconds(60), config.XimpleInactivity.Timeout);
            Assert.IsTrue(config.XimpleInactivity.AtStartup);
        }

        /// <summary>
        /// Tests that <c>AhdlcRenderer.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Composer\Composer_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<ComposerConfig>();
            configManager.FileName = "Composer_v2.4.xml";
            configManager.XmlSchema = ComposerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            var memory = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memory, config);
            Assert.IsTrue(memory.Position > 0);
        }

        // ReSharper restore InconsistentNaming
    }
}
