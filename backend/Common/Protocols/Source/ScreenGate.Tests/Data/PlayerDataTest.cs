// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerDataTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayerDataTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Tests.Data
{
    using System.IO;

    using Gorba.Common.Protocols.ScreenGate.Data;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    /// <summary>
    /// Unit tests for <see cref="PlayerData"/>.
    /// </summary>
    [TestClass]
    public class PlayerDataTest
    {
        /// <summary>
        /// Tests the deserialization of an entire data.json file.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Data\\data.json")]
        public void TestDeserialization()
        {
            var config = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText("data.json"));
            Assert.AreEqual("http://sg-prod.gatemedia.ch/widgets.json", config.Config.WidgetsUrl);
            Assert.AreEqual("https://sg-resources.s3.amazonaws.com/", config.Config.RemoteResourceUrl);
            Assert.AreEqual(
                "http://sg-prod.gatemedia.ch/gmp/players/872/ping.json?player_token=2ddc007ca65363d1fc069383b0d2ca25",
                config.Config.PingUrl);
            Assert.AreEqual(872, config.Config.PlayerId);
            Assert.AreEqual("2ddc007ca65363d1fc069383b0d2ca25", config.Config.PlayerToken);

            Assert.AreEqual(1, config.BroadcastLocations.Count);

            var broadcastLocation = config.BroadcastLocations[0];
            Assert.AreEqual(646, broadcastLocation.Id);
            Assert.AreEqual("Gorba SA", broadcastLocation.Name);
            Assert.AreEqual(3600, broadcastLocation.ReportInterval);
            Assert.AreEqual("47.1192499", broadcastLocation.Latitude);
            Assert.AreEqual("7.2610375", broadcastLocation.Longitude);
            Assert.AreEqual("CH", broadcastLocation.Country);
            Assert.AreEqual("35A Erlen Street", broadcastLocation.Street);
            Assert.AreEqual("Brügg", broadcastLocation.City);
            Assert.AreEqual("2555", broadcastLocation.Zip);
            Assert.AreEqual(1080, broadcastLocation.Height);
            Assert.AreEqual(1920, broadcastLocation.Width);
            Assert.AreEqual("#000000", broadcastLocation.BackgroundColor);
            Assert.AreEqual(1, broadcastLocation.Surfaces.Count);

            var surface = broadcastLocation.Surfaces[0];
            Assert.AreEqual(0, surface.X);
            Assert.AreEqual(0, surface.Y);
            Assert.AreEqual(1920, surface.Width);
            Assert.AreEqual(1080, surface.Height);
            Assert.AreEqual(0.0, surface.Volume);
            Assert.AreEqual(0, surface.Playlist.Tags.Count);
            Assert.AreEqual(11, surface.Playlist.Elements.Count);

            var element = surface.Playlist.Elements[2];
            Assert.AreEqual(86989, element.Id);
            Assert.AreEqual(212, element.Duration);
            Assert.AreEqual(11799, element.ContentId);
            Assert.AreEqual(1, element.BroadcastPlanning.Monday.Count);
            Assert.AreEqual(2, element.BroadcastPlanning.Monday[0].Length);
            Assert.AreEqual("00:00", element.BroadcastPlanning.Monday[0][0]);
            Assert.AreEqual("23:59", element.BroadcastPlanning.Monday[0][1]);
            Assert.AreEqual(1, element.BroadcastPlanning.Tuesday.Count);
            Assert.AreEqual(1, element.BroadcastPlanning.Wednesday.Count);
            Assert.AreEqual(3, element.BroadcastPlanning.Thursday.Count);
            Assert.AreEqual(2, element.BroadcastPlanning.Thursday[0].Length);
            Assert.AreEqual("19:30", element.BroadcastPlanning.Thursday[0][0]);
            Assert.AreEqual("22:00", element.BroadcastPlanning.Thursday[0][1]);
            Assert.AreEqual(2, element.BroadcastPlanning.Thursday[1].Length);
            Assert.AreEqual("16:00", element.BroadcastPlanning.Thursday[1][0]);
            Assert.AreEqual("19:30", element.BroadcastPlanning.Thursday[1][1]);
            Assert.AreEqual(2, element.BroadcastPlanning.Thursday[2].Length);
            Assert.AreEqual("04:30", element.BroadcastPlanning.Thursday[2][0]);
            Assert.AreEqual("09:00", element.BroadcastPlanning.Thursday[2][1]);
            Assert.IsNull(element.BroadcastPlanning.Friday);
            Assert.AreEqual(1, element.BroadcastPlanning.Saturday.Count);
            Assert.AreEqual(1, element.BroadcastPlanning.Sunday.Count);
        }
    }
}
