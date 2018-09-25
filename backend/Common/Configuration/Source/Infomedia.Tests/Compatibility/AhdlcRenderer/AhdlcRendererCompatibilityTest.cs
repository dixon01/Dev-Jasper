// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcRendererCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcRendererCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Tests.Compatibility.AhdlcRenderer
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The AHDLC Renderer file compatibility test.
    /// </summary>
    [TestClass]
    public class AhdlcRendererCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>AhdlcRenderer.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\AhdlcRenderer\AhdlcRenderer_v2.2.xml")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<AhdlcRendererConfig>();
            configManager.FileName = "AhdlcRenderer_v2.2.xml";
            configManager.XmlSchema = AhdlcRendererConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Channels);
            Assert.AreEqual(1, config.Channels.Count);

            var channel = config.Channels[0];
            Assert.IsNotNull(channel.SerialPort);
            Assert.AreEqual("COM20", channel.SerialPort.ComPort);
            Assert.AreEqual(38400, channel.SerialPort.BaudRate);
            Assert.AreEqual(8, channel.SerialPort.DataBits);
            Assert.AreEqual(StopBits.Two, channel.SerialPort.StopBits);
            Assert.AreEqual(Parity.None, channel.SerialPort.Parity);
            Assert.AreEqual(RtsMode.Default, channel.SerialPort.RtsMode);
            Assert.AreEqual(false, channel.SerialPort.IsHighSpeed);
            Assert.AreEqual(false, channel.SerialPort.IgnoreFrameStart);
            Assert.AreEqual(false, channel.SerialPort.IgnoreResponses);
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), channel.SerialPort.IgnoreResponseTime);

            Assert.IsNotNull(channel.Signs);
            Assert.AreEqual(3, channel.Signs.Count);

            var sign = channel.Signs[0];
            Assert.AreEqual(1, sign.Address);
            Assert.AreEqual(SignMode.Monochrome, sign.Mode);
            Assert.AreEqual(112, sign.Width);
            Assert.AreEqual(16, sign.Height);

            sign = channel.Signs[1];
            Assert.AreEqual(7, sign.Address);
            Assert.AreEqual(SignMode.Text, sign.Mode);
            Assert.AreEqual(96, sign.Width);
            Assert.AreEqual(8, sign.Height);

            sign = channel.Signs[2];
            Assert.AreEqual(13, sign.Address);
            Assert.AreEqual(SignMode.Color, sign.Mode);
            Assert.AreEqual(34, sign.Width);
            Assert.AreEqual(20, sign.Height);
        }

        /// <summary>
        /// Tests <c>AhdlcRenderer.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\AhdlcRenderer\AhdlcRenderer_v2.4.xml")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<AhdlcRendererConfig>();
            configManager.FileName = "AhdlcRenderer_v2.4.xml";
            configManager.XmlSchema = AhdlcRendererConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Channels);
            Assert.AreEqual(1, config.Channels.Count);

            var channel = config.Channels[0];
            Assert.IsNotNull(channel.SerialPort);
            Assert.AreEqual("COM20", channel.SerialPort.ComPort);
            Assert.AreEqual(38400, channel.SerialPort.BaudRate);
            Assert.AreEqual(8, channel.SerialPort.DataBits);
            Assert.AreEqual(StopBits.Two, channel.SerialPort.StopBits);
            Assert.AreEqual(Parity.None, channel.SerialPort.Parity);
            Assert.AreEqual(RtsMode.Default, channel.SerialPort.RtsMode);
            Assert.AreEqual(false, channel.SerialPort.IsHighSpeed);
            Assert.AreEqual(false, channel.SerialPort.IgnoreFrameStart);
            Assert.AreEqual(false, channel.SerialPort.IgnoreResponses);
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), channel.SerialPort.IgnoreResponseTime);

            Assert.IsNotNull(channel.Signs);
            Assert.AreEqual(3, channel.Signs.Count);

            var sign = channel.Signs[0];
            Assert.AreEqual(1, sign.Address);
            Assert.AreEqual(SignMode.Monochrome, sign.Mode);
            Assert.AreEqual(112, sign.Width);
            Assert.AreEqual(16, sign.Height);

            sign = channel.Signs[1];
            Assert.AreEqual(7, sign.Address);
            Assert.AreEqual(SignMode.Text, sign.Mode);
            Assert.AreEqual(96, sign.Width);
            Assert.AreEqual(8, sign.Height);

            sign = channel.Signs[2];
            Assert.AreEqual(13, sign.Address);
            Assert.AreEqual(SignMode.Color, sign.Mode);
            Assert.AreEqual(34, sign.Width);
            Assert.AreEqual(20, sign.Height);
        }

        /// <summary>
        /// Tests that <c>AhdlcRenderer.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\AhdlcRenderer\AhdlcRenderer_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<AhdlcRendererConfig>();
            configManager.FileName = "AhdlcRenderer_v2.4.xml";
            configManager.XmlSchema = AhdlcRendererConfig.Schema;
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
