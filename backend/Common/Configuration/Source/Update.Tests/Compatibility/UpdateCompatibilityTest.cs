// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Tests.Compatibility
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Configuration.Update.Clients;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The System Manager configuration file compatibility test.
    /// </summary>
    [TestClass]
    public class UpdateCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>SystemManager.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Update_v2.2.xml")]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<UpdateConfig>();
            configManager.FileName = "Update_v2.2.xml";
            configManager.XmlSchema = UpdateConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.Agent.Enabled);
            Assert.AreEqual(0, config.Agent.RestartApplications.Dependencies.Count);

            Assert.AreEqual(1, config.Clients.Count);
            var usbClient = config.Clients[0] as UsbUpdateClientConfig;
            Assert.IsNotNull(usbClient);
            Assert.AreEqual("USB_E", usbClient.Name);
            Assert.AreEqual(@"E:\Gorba\Update", usbClient.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromSeconds(3), usbClient.UsbDetectionTimeOut);
            Assert.AreEqual(TimeSpan.FromSeconds(30), usbClient.PollInterval);

            Assert.AreEqual(0, config.Providers.Count);

            Assert.AreEqual(TimeSpan.FromSeconds(30), config.Visualization.HideTimeout);
            Assert.IsTrue(config.Visualization.SplashScreen.Enabled);
            Assert.IsFalse(config.Visualization.Led.Enabled);
            Assert.AreEqual(1.25, config.Visualization.Led.DefaultFrequency);
            Assert.AreEqual(5, config.Visualization.Led.ErrorFrequency);
        }

        /// <summary>
        /// Tests <c>SystemManager.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Update_v2.4.xml")]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<UpdateConfig>();
            configManager.FileName = "Update_v2.4.xml";
            configManager.XmlSchema = UpdateConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.Agent.Enabled);
            Assert.AreEqual(0, config.Agent.RestartApplications.Dependencies.Count);

            Assert.AreEqual(1, config.Clients.Count);
            var usbClient = config.Clients[0] as UsbUpdateClientConfig;
            Assert.IsNotNull(usbClient);
            Assert.AreEqual("USB_E", usbClient.Name);
            Assert.AreEqual(@"E:\Gorba\Update", usbClient.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromSeconds(3), usbClient.UsbDetectionTimeOut);
            Assert.AreEqual(TimeSpan.FromSeconds(30), usbClient.PollInterval);

            Assert.AreEqual(0, config.Providers.Count);

            Assert.AreEqual(TimeSpan.FromSeconds(30), config.Visualization.HideTimeout);
            Assert.IsTrue(config.Visualization.SplashScreen.Enabled);
            Assert.IsFalse(config.Visualization.Led.Enabled);
            Assert.AreEqual(1.25, config.Visualization.Led.DefaultFrequency);
            Assert.AreEqual(5, config.Visualization.Led.ErrorFrequency);
        }

        /// <summary>
        /// Tests that <c>SystemManager.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Update_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<UpdateConfig>();
            configManager.FileName = "Update_v2.4.xml";
            configManager.XmlSchema = UpdateConfig.Schema;
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
