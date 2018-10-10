// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigurationReaderTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigurationReader test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers
{
    using System;

    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Defines tests for the <see cref="ZoomCalculatorTest"/>;
    /// </summary>
    [TestClass]
    public class UnitConfigurationReaderTest
    {
        /// <summary>
        /// The read unit configuration.
        /// </summary>
        [TestMethod]
        public void ReadUnitConfiguration()
        {
            const string TestXmlUnitConfiguration =
                    @" <test>
                         <Category Key=""AudioRenderer"">
                           <Part Key=""AudioRenderer.Acapela"">
                             <Value Key=""Voices"" Value="""" />
                           </Part>
                         </Category>
                         <Category Key=""Conclusion"">
                           <Value Key=""Gorba.Motion.WrongThing"" Value=""2.4.1445.8470"" />
                           <Part Key=""Conclusion.SoftwareVersions"">
                             <Value Key=""Gorba.Motion.Infomedia.AhdlcRenderer"" Value=""2.4.1111.8482"" />
                             <Value Key=""Gorba.Motion.Infomedia.AudioRenderer"" Value=""2.4.2222.8482"" />
                             <Value Key=""Gorba.Motion.Infomedia.Composer"" Value=""2.4.3333.8482"" />
                             <Value Key=""Gorba.Motion.Infomedia.DirectXRenderer"" Value=""2.4.4444.8482"" />
                             <Value Key=""Gorba.Motion.HardwareManager"" Value=""1.4.5555.8467"" />
                             <Value Key=""Gorba.Motion.Protran"" Value=""2.4.6666.8476"" />
                             <Value Key=""Gorba.Motion.SystemManager"" Value=""2.4.7777.8464"" />
                             <Value Key=""Gorba.Motion.Update"" Value=""2.4.8888.8470"" />
                           </Part>
                           <Part Key=""Conclusion.ExportPreparation"" />
                           <Value Key=""Gorba.Motion.WrongThing"" Value=""2.4.1445.8470"" />
                         </Category>
                       </test>";

            var result = UnitConfigurationReader.GetComponentVersions(TestXmlUnitConfiguration);

            Assert.IsTrue(result.Count == 8);

            Assert.IsTrue(result[0].Component == FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer);
            Assert.IsTrue(result[0].Version == new SoftwareComponentVersion("2.4.1111.8482"));

            Assert.IsTrue(result[1].Component == FeatureComponentRequirements.SoftwareComponent.AudioRenderer);
            Assert.IsTrue(result[1].Version == new SoftwareComponentVersion("2.4.2222.8482"));

            Assert.IsTrue(result[2].Component == FeatureComponentRequirements.SoftwareComponent.Composer);
            Assert.IsTrue(result[2].Version == new SoftwareComponentVersion("2.4.3333.8482"));

            Assert.IsTrue(result[3].Component == FeatureComponentRequirements.SoftwareComponent.DxRenderer);
            Assert.IsTrue(result[3].Version == new SoftwareComponentVersion("2.4.4444.8482"));

            Assert.IsTrue(result[4].Component == FeatureComponentRequirements.SoftwareComponent.HardwareManager);
            Assert.IsTrue(result[4].Version == new SoftwareComponentVersion("1.4.5555.8467"));

            Assert.IsTrue(result[5].Component == FeatureComponentRequirements.SoftwareComponent.Protran);
            Assert.IsTrue(result[5].Version == new SoftwareComponentVersion("2.4.6666.8476"));

            Assert.IsTrue(result[6].Component == FeatureComponentRequirements.SoftwareComponent.SystemManager);
            Assert.IsTrue(result[6].Version == new SoftwareComponentVersion("2.4.7777.8464"));

            Assert.IsTrue(result[7].Component == FeatureComponentRequirements.SoftwareComponent.Update);
            Assert.IsTrue(result[7].Version == new SoftwareComponentVersion("2.4.8888.8470"));
        }

        /// <summary>
        /// The read unit configuration not found.
        /// </summary>
        [TestMethod]
        public void ReadUnitConfigurationUnknownComponent()
        {
            const string TestXmlUnitConfiguration =
                    @" <test>
                         <Category Key=""AudioRenderer"">
                           <Part Key=""AudioRenderer.Acapela"">
                             <Value Key=""Voices"" Value="""" />
                           </Part>
                         </Category>
                         <Category Key=""Conclusion"">
                           <Part Key=""Conclusion.SoftwareVersions"">
                             <Value Key=""Gorba.Motion.Infomedia.UnKnown"" Value=""7.4.1234.8482"" />
                             <Value Key=""Gorba.Motion.Infomedia.Composer"" Value=""2.6.1536.10200"" />
                           </Part>
                           <Part Key=""Conclusion.ExportPreparation"" />
                         </Category>
                       </test>";

            var result = UnitConfigurationReader.GetComponentVersions(TestXmlUnitConfiguration);

            Assert.AreEqual(1, result.Count);
        }

        /// <summary>
        /// The read unit configuration not found.
        /// </summary>
        [TestMethod]
        public void ReadUnitConfigurationNotFound()
        {
            const string TestXmlUnitConfiguration =
                    @" <test>
                         <Category Key=""AudioRenderer"">
                           <Part Key=""AudioRenderer.Acapela"">
                             <Value Key=""Voices"" Value="""" />
                           </Part>
                         </Category>
                         <Category Key=""Conclusion"">
                           <Part Key=""Conclusion.ExportPreparation"" />
                           <Value Key=""Gorba.Motion.WrongThing"" Value=""2.4.1445.8470"" />
                         </Category>
                       </test>";

            var result = UnitConfigurationReader.GetComponentVersions(TestXmlUnitConfiguration);

            Assert.IsTrue(result.Count == 0);
        }

        /// <summary>
        /// The read unit configuration empty string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(System.Xml.XmlException))]
        public void ReadUnitConfigurationEmptyString()
        {
            var testXmlUnitConfiguration = string.Empty;

            var result = UnitConfigurationReader.GetComponentVersions(testXmlUnitConfiguration);

            Assert.IsTrue(result.Count == 0);
        }
    }
}