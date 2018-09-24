// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Tests.Compatibility.Presentation
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The Audio Renderer file compatibility test.
    /// </summary>
    [TestClass]
    public class PresentationCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests the deserialization of <c>main.im2</c> version 2.2.
        /// This test doesn't verify the contents of the file since this would be too much effort.
        /// Specific tests should be written for known possible compatibility issues.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Presentation\main_v2.2.im2")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<InfomediaConfig>();
            configManager.FileName = "main_v2.2.im2";
            var config = configManager.Config;

            Assert.IsNotNull(config);
        }

        /// <summary>
        /// Tests the deserialization of <c>main.im2</c> version 2.4.
        /// This test doesn't verify the contents of the file since this would be too much effort.
        /// Specific tests should be written for known possible compatibility issues.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Presentation\main_v2.4.im2")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<InfomediaConfig>();
            configManager.FileName = "main_v2.4.im2";
            var config = configManager.Config;

            Assert.IsNotNull(config);
        }

        /// <summary>
        /// Tests that <c>AhdlcRenderer.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Presentation\main_v2.4.im2")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<InfomediaConfig>();
            configManager.FileName = "main_v2.4.im2";
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
