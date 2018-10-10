// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemSettingsProviderTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemSettingsProviderTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Xml.Schema;

    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Common.Configuration.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Defines unit tests for the <see cref="HostingSettingsProvider"/> component.
    /// </summary>
    [TestClass]
    public class BackgroundSystemSettingsProviderTest
    {
        /// <summary>
        /// Gets or sets the <see cref="TestContext"/> for the test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes a test run.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Reset();
        }

        /// <summary>
        /// Cleans a test run up.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Reset();
        }

        /// <summary>
        /// Tests the loading of the BackgroundSystemSettings object using the BackgroundSystemSettings.xml file.
        /// </summary>
        [TestMethod]
        [DeploymentItem("BackgroundSystemSettings.xml")]
        public void TestValidBackgroundSystemSettings()
        {
            var config = HostingSettingsProvider.Current.GetSettings();
        }

        /// <summary>
        /// Tests the loading of the BackgroundSystemSettings object using a path to the file.
        /// </summary>
        [TestMethod]
        [DeploymentItem("BackgroundSystemSettings.xml", "TestWithPath")]
        public void TestValidBackgroundSystemSettingsWithPath()
        {
            var path = this.GetTestFilePath("BackgroundSystemSettings.xml");
            var config = HostingSettingsProvider.Current.GetSettings(path);
        }

        /// <summary>
        /// Tests the validation of a BackgroundSystemSettings file.
        /// </summary>
        [TestMethod]
        [DeploymentItem("BackgroundSystemSettingsWithErrors.xml", "TestWithPath")]
        [ExpectedException(typeof(ConfiguratorException))]
        public void TestBackgroundSystemSettingsWithErrors()
        {
            var path = this.GetTestFilePath("BackgroundSystemSettingsWithErrors.xml");
            var configManager = new ConfigManager<BackgroundSystemSettings>();
            configManager.FileName = path;
            configManager.XmlSchema = GetXmlSchema();
            var config = configManager.Config;
        }

        private static XmlSchema GetXmlSchema()
        {
            var backgroundSystemSettingsType = typeof(BackgroundSystemSettings);
            var resourceName = backgroundSystemSettingsType.Namespace + ".BackgroundSystemSettings.xsd";
            using (var stream = backgroundSystemSettingsType.Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception(
                        "Can't find the BackgroundSystemSettings xml schema. Ensure that the file is set as"
                        + "'EmbeddedResource' and it is in the same directory as this class.");
                }

                var exceptions = new List<XmlSchemaException>();
                var schema = XmlSchema.Read(stream, (sender, args) => exceptions.Add(args.Exception));
                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions);
                }

                return schema;
            }
        }

        private static void Reset()
        {
            BackgroundSystemConfigurationProvider.Reset();
        }

        private string GetTestFilePath(string fileName)
        {
            var path = Path.Combine(this.TestContext.DeploymentDirectory, "TestWithPath", fileName);
            if (!File.Exists(path))
            {
                throw new Exception("Test file not found");
            }

            return path;
        }
    }
}