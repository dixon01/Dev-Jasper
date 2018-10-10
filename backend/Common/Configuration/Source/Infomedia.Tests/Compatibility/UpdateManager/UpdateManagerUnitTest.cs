// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="UpdateManagerUnitTest.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Infomedia.Tests.Compatibility.UpdateManager
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Medi.Core.Config;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>The update manager unit test.</summary>
    [TestClass]
    public class UpdateManagerUnitTest
    {
        #region Static Fields

        private static TestContext testContext;

        #endregion

        #region Public Methods and Operators

        /// <summary>The assembly init.</summary>
        /// <param name="context">The context.</param>
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            testContext = context;
        }

        /// <summary>The test update config serilization_1_0.</summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\UpdateManager\Update.1.0.xml")]
        public void TestUpdateConfigSerilization_1_0()
        {
            try
            {
                var configManager = new ConfigManager<UpdateConfig> { FileName = "Update.1.0.xml", XmlSchema = UpdateConfig.Schema };
                var config = configManager.Config;

                Assert.IsNotNull(config);              
                Assert.IsNotNull(config.Clients);
                Assert.AreEqual(4, config.Clients.Count);
                Assert.IsTrue(config.Clients.All(m => m.ShowVisualization));
            }
            catch (XmlException exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        /// <exception cref="ArgumentException"><paramref name="path" /> is an empty string (""), contains only white space, or contains one or more invalid characters. -or-<paramref name="path" /> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in an NTFS environment.</exception>
        [TestMethod]
        [DeploymentItem(@"Compatibility\UpdateManager\Update.1.2.xml")]
        public void WriteUpdateManagerXml()
        {
            var configManager = new ConfigManager<UpdateConfig>("Update.1.2.xml") { XmlSchema = UpdateConfig.Schema };
            var config = configManager.Config;

            var file = Path.Combine(testContext.TestRunDirectory, "UpdateTest.xml");
            Debug.WriteLine("Creating file " + file);
            using (var fs = new FileStream(file, FileMode.CreateNew))
            {
                var serializer = new XmlSerializer(typeof(UpdateConfig));
                serializer.Serialize(fs, config);
            }
            Assert.IsTrue(File.Exists(file));

            // read it back in
            var configManagerNew = new ConfigManager<UpdateConfig>(file) { XmlSchema = UpdateConfig.Schema };
            var configNew = configManagerNew.Config;
        }



        [TestMethod]
        [DeploymentItem(@"Compatibility\Composer\Medi.config")]
        public void TestReadingComposerMediConfig()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(MediConfig));
                using (var reader = File.OpenText("Medi.config"))
                {
                    var cfg = serializer.Deserialize(reader) as MediConfig;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.Fail(ex.ToString());
            }
        }

        #endregion
    }
}