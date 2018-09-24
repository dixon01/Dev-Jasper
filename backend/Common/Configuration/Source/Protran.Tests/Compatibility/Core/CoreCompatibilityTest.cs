// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CoreCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Tests.Compatibility.Core
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The Protran configuration file compatibility test.
    /// </summary>
    [TestClass]
    public class CoreCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>protran.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Core\protran_v2.2.xml")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<ProtranConfig>();
            configManager.FileName = "protran_v2.2.xml";
            configManager.XmlSchema = ProtranConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.AreEqual(3, config.Protocols.Count);

            var protocol = config.Protocols[0];
            Assert.AreEqual("IbisProtocol", protocol.Name);
            Assert.IsTrue(protocol.Enabled);

            protocol = config.Protocols[1];
            Assert.AreEqual("IOProtocol", protocol.Name);
            Assert.IsFalse(protocol.Enabled);

            protocol = config.Protocols[2];
            Assert.AreEqual("VDV301", protocol.Name);
            Assert.IsFalse(protocol.Enabled);

            Assert.IsNotNull(config.Persistence);
            Assert.IsFalse(config.Persistence.IsEnabled);
            Assert.AreEqual("persistence.xml", config.Persistence.PersistenceFile);
            Assert.AreEqual(TimeSpan.FromMinutes(10), config.Persistence.DefaultValidity);
        }

        /// <summary>
        /// Tests <c>protran.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Core\protran_v2.4.xml")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<ProtranConfig>();
            configManager.FileName = "protran_v2.4.xml";
            configManager.XmlSchema = ProtranConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.AreEqual(3, config.Protocols.Count);

            var protocol = config.Protocols[0];
            Assert.AreEqual("IbisProtocol", protocol.Name);
            Assert.IsTrue(protocol.Enabled);

            protocol = config.Protocols[1];
            Assert.AreEqual("IOProtocol", protocol.Name);
            Assert.IsFalse(protocol.Enabled);

            protocol = config.Protocols[2];
            Assert.AreEqual("VDV301", protocol.Name);
            Assert.IsFalse(protocol.Enabled);

            Assert.IsNotNull(config.Persistence);
            Assert.IsFalse(config.Persistence.IsEnabled);
            Assert.AreEqual("persistence.xml", config.Persistence.PersistenceFile);
            Assert.AreEqual(TimeSpan.FromMinutes(10), config.Persistence.DefaultValidity);
        }

        /// <summary>
        /// Tests that <c>protran.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Core\protran_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<ProtranConfig>();
            configManager.FileName = "protran_v2.4.xml";
            configManager.XmlSchema = ProtranConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            var memory = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memory, config);
            Assert.IsTrue(memory.Position > 0);
        }

        /// <summary>
        /// Tests that <c>protran.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Core\protran_v2.5.xml")]
        public void TestV2_5()
        {
            var configManager = new ConfigManager<ProtranConfig>();
            configManager.FileName = "protran_v2.5.xml";
            configManager.XmlSchema = ProtranConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            var memory = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memory, config);
            Assert.IsTrue(memory.Position > 0);

            var protocol = config.Protocols.FirstOrDefault(m => m.Name == "XimpleProtocol");
            Assert.IsNotNull(protocol);
            Assert.AreEqual("XimpleProtocol", protocol.Name);
            Assert.IsTrue(protocol.Enabled);
        }

        // ReSharper restore InconsistentNaming
    }
}
