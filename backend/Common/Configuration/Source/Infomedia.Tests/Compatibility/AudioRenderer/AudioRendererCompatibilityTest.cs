// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioRendererCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioRendererCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Tests.Compatibility.AudioRenderer
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.AudioRenderer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The Audio Renderer file compatibility test.
    /// </summary>
    [TestClass]
    public class AudioRendererCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>AudioRenderer.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\AudioRenderer\AudioRenderer_v2.2.xml")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<AudioRendererConfig>();
            configManager.FileName = "AudioRenderer_v2.2.xml";
            configManager.XmlSchema = AudioRendererConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.IO);
            Assert.IsNotNull(config.IO.VolumePort);
            Assert.IsNull(config.IO.VolumePort.Unit);
            Assert.IsNull(config.IO.VolumePort.Application);
            Assert.AreEqual("Volume", config.IO.VolumePort.PortName);

            // SpeakerPort is missing, this is an incompatibility between 2.2 and 2.4!
            Assert.IsNotNull(config.AudioChannels);

            // no audio channels are created, this is an incompatibility between 2.2 and 2.4!
            Assert.AreEqual(0, config.AudioChannels.Count);

            Assert.IsNotNull(config.TextToSpeech);
            Assert.AreEqual(TextToSpeechApi.Acapela, config.TextToSpeech.Api);
            Assert.AreEqual(@"D:\Progs\Acapela", config.TextToSpeech.HintPath);
        }

        /// <summary>
        /// Tests <c>AudioRenderer.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\AudioRenderer\AudioRenderer_v2.4.xml")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<AudioRendererConfig>();
            configManager.FileName = "AudioRenderer_v2.4.xml";
            configManager.XmlSchema = AudioRendererConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.IO);
            Assert.IsNotNull(config.IO.VolumePort);
            Assert.IsNull(config.IO.VolumePort.Unit);
            Assert.IsNull(config.IO.VolumePort.Application);
            Assert.AreEqual("SystemVolume", config.IO.VolumePort.PortName);

            Assert.IsNotNull(config.AudioChannels);
            Assert.AreEqual(2, config.AudioChannels.Count);

            var channel = config.AudioChannels[0];
            Assert.AreEqual("1", channel.Id);
            Assert.IsNotNull(channel.SpeakerPorts);
            Assert.AreEqual(2, channel.SpeakerPorts.Count);

            var port = channel.SpeakerPorts[0];
            Assert.IsNull(port.Unit);
            Assert.IsNull(port.Application);
            Assert.AreEqual("Speaker", port.PortName);

            port = channel.SpeakerPorts[1];
            Assert.IsNull(port.Unit);
            Assert.IsNull(port.Application);
            Assert.AreEqual("Speaker1", port.PortName);

            channel = config.AudioChannels[1];
            Assert.AreEqual("2", channel.Id);
            Assert.IsNotNull(channel.SpeakerPorts);
            Assert.AreEqual(1, channel.SpeakerPorts.Count);

            port = channel.SpeakerPorts[0];
            Assert.IsNull(port.Unit);
            Assert.IsNull(port.Application);
            Assert.AreEqual("Speaker1", port.PortName);

            Assert.IsNotNull(config.TextToSpeech);
            Assert.AreEqual(TextToSpeechApi.Acapela, config.TextToSpeech.Api);
            Assert.AreEqual(@"D:\Progs\Acapela", config.TextToSpeech.HintPath);
        }

        /// <summary>
        /// Tests that <c>AhdlcRenderer.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\AudioRenderer\AudioRenderer_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<AudioRendererConfig>();
            configManager.FileName = "AudioRenderer_v2.4.xml";
            configManager.XmlSchema = AudioRendererConfig.Schema;
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
