// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialConfigTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InitialConfigTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Tests.Init
{
    using System.IO;

    using Gorba.Common.Protocols.ScreenGate.Init;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    /// <summary>
    /// Unit tests for <see cref="InitialConfig"/>.
    /// </summary>
    [TestClass]
    public class InitialConfigTest
    {
        /// <summary>
        /// Tests the de-serialization of the initial configuration.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Init\\config.json")]
        public void DeserializeConfig()
        {
            var config = JsonConvert.DeserializeObject<InitialConfig>(File.ReadAllText("config.json"));
            Assert.AreEqual(
                "https://sg-players.s3.amazonaws.com/players/2ddc007ca65363d1fc069383b0d2ca25.xml",
                config.XmlUrl);
        }
    }
}
