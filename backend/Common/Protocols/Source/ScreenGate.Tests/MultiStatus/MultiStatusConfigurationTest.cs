// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiStatusConfigurationTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiStatusConfigurationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Tests.MultiStatus
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.ScreenGate.MultiStatus;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="MultiStatusConfiguration"/>.
    /// </summary>
    [TestClass]
    public class MultiStatusConfigurationTest
    {
        /// <summary>
        /// Tests the deserialization of an entire multi-status XML document.
        /// </summary>
        [TestMethod]
        [DeploymentItem("MultiStatus\\2ddc007ca65363d1fc069383b0d2ca25.xml")]
        public void TestDeserialization()
        {
            var serializer = new XmlSerializer(typeof(MultiStatusConfiguration));
            MultiStatusConfiguration config;
            using (var input = File.OpenRead("2ddc007ca65363d1fc069383b0d2ca25.xml"))
            {
                config = (MultiStatusConfiguration)serializer.Deserialize(input);
            }

            Assert.AreEqual(3, config.MultiStatuses.Count);

            var multiStatus = config.MultiStatuses[0];
            Assert.AreEqual(
                "https://sg-players.s3.amazonaws.com/players/2ddc007ca65363d1fc069383b0d2ca25",
                multiStatus.Base);
            Assert.AreEqual(DateTimeKind.Utc, multiStatus.UpdatedAt.Kind);
            Assert.AreEqual("2015-02-09 10:09:18Z", multiStatus.UpdatedAt.ToString("u"));
            Assert.AreEqual(2, multiStatus.Responses.Count);
            Assert.AreEqual("/", multiStatus.Responses[0].HypertextReference);
            Assert.IsNull(multiStatus.Responses[0].PropStat);
            Assert.AreEqual("/data.json", multiStatus.Responses[1].HypertextReference);
            Assert.AreEqual("c6aa3a3436fbd3cac2fdd091b9d9e293", multiStatus.Responses[1].PropStat.Prop.GetETag);
        }
    }
}
