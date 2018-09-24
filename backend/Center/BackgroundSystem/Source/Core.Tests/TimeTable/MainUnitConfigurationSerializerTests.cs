// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainUnitConfigurationSerializerTests.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.TimeTable
{
    using System;
    using System.IO;

    using Gorba.Center.BackgroundSystem.Core.Qube.Configuration;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Utils;
    using Gorba.Common.Configuration.EPaper.MainUnit;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The main unit configuration serializer tests.
    /// </summary>
    [TestClass]
    public class MainUnitConfigurationSerializerTests
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// The test serialization.
        /// </summary>
        [TestMethod]
        [DeploymentItem("TimeTable/test_config.bin")]
        public void TestSerialization()
        {
            var serializer = new MainUnitConfigurationSerializer();
            var config = new MainUnitConfig
                             {
                                 FirmwareHash = "0123456789abcdef",
                                 DisplayUnits =
                                     {
                                         new DisplayUnitConfig
                                             {
                                                 ContentHash =
                                                     "d754c26638d49e9d"
                                             },
                                         new DisplayUnitConfig
                                             {
                                                 ContentHash =
                                                     "c0851098390a3a40"
                                             }
                                     }
                             };
            var result = serializer.SerializeAsync(config).Result;
            var outputPath = Path.Combine(this.TestContext.DeploymentDirectory, "output_config.bin");
            using (var fileStream = File.OpenWrite(outputPath))
            {
                result.CopyTo(fileStream);
            }

            var referencePath = Path.Combine(this.TestContext.DeploymentDirectory, "test_config.bin");
            var compare = Compare(referencePath, outputPath);
            Assert.IsTrue(compare);
        }

        private static bool Compare(string path1, string path2)
        {
            var hash1 = ContentResourceHash.Create(path1, HashAlgorithmTypes.xxHash64);
            var hash2 = ContentResourceHash.Create(path2, HashAlgorithmTypes.xxHash64);
            return string.Equals(hash1, hash2, StringComparison.OrdinalIgnoreCase);
        }
    }
}