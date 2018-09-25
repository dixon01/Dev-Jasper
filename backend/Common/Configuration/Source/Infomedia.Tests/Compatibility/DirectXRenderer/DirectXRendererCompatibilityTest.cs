// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXRendererCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXRendererCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Tests.Compatibility.DirectXRenderer
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The Audio Renderer file compatibility test.
    /// </summary>
    [TestClass]
    public class DirectXRendererCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>DirectXRenderer.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\DirectXRenderer\DirectXRenderer_v2.2.xml")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<RendererConfig>();
            configManager.FileName = "DirectXRenderer_v2.2.xml";
            configManager.XmlSchema = RendererConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.AreEqual(WindowMode.Windowed, config.WindowMode);
            Assert.AreEqual(TimeSpan.FromSeconds(30), config.FallbackTimeout);

            Assert.IsNotNull(config.Screens);
            Assert.AreEqual(2, config.Screens.Count);

            var screen = config.Screens[0];
            Assert.AreEqual(0, screen.Adapter);
            Assert.IsNull(screen.Id);
            Assert.AreEqual(1920, screen.Width);
            Assert.AreEqual(1080, screen.Height);
            Assert.AreEqual(@"D:\Presentation\Images\Fallback.jpg", screen.FallbackImage);
            Assert.IsNotNull(screen.VisibleRegion);
            Assert.AreEqual(0, screen.VisibleRegion.X);
            Assert.AreEqual(0, screen.VisibleRegion.Y);
            Assert.AreEqual(1368, screen.VisibleRegion.Width);
            Assert.AreEqual(768, screen.VisibleRegion.Height);

            screen = config.Screens[1];
            Assert.AreEqual(1, screen.Adapter);
            Assert.IsNull(screen.Id);
            Assert.AreEqual(1440, screen.Width);
            Assert.AreEqual(900, screen.Height);
            Assert.AreEqual(string.Empty, screen.FallbackImage);
            Assert.IsNull(screen.VisibleRegion);

            Assert.IsNotNull(config.Device);
            Assert.AreEqual(MultiSampleTypes.None, config.Device.MultiSample);
            Assert.AreEqual(0, config.Device.MultiSampleQuality);
            Assert.AreEqual(true, config.Device.MultiThreaded);
            Assert.AreEqual(PresentFlags.None, config.Device.PresentFlag);
            Assert.AreEqual(PresentIntervals.One, config.Device.PresentationInterval);
            Assert.AreEqual(SwapEffects.Discard, config.Device.SwapEffect);

            Assert.IsNotNull(config.Text);
            Assert.AreEqual(TimeSpan.FromSeconds(3), config.Text.AlternationInterval);
            Assert.AreEqual(TimeSpan.FromSeconds(0.5), config.Text.BlinkInterval);
            Assert.AreEqual(FontQualities.AntiAliased, config.Text.FontQuality);
            Assert.AreEqual(TextMode.FontSprite, config.Text.TextMode);

            Assert.IsNotNull(config.Image);
            Assert.AreEqual(TimeSpan.FromSeconds(3600), config.Image.BitmapCacheTimeout);
            Assert.AreEqual(52428800, config.Image.MaxBitmapCacheBytes);
            Assert.AreEqual(1000000, config.Image.MaxCacheBytesPerBitmap);

            Assert.IsNotNull(config.Video);
            Assert.AreEqual(VideoMode.DirectXWindow, config.Video.VideoMode);
        }

        /// <summary>
        /// Tests <c>DirectXRenderer.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\DirectXRenderer\DirectXRenderer_v2.4.xml")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<RendererConfig>();
            configManager.FileName = "DirectXRenderer_v2.4.xml";
            configManager.XmlSchema = RendererConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.AreEqual(WindowMode.Windowed, config.WindowMode);
            Assert.AreEqual(TimeSpan.FromSeconds(30), config.FallbackTimeout);

            Assert.IsNotNull(config.Screens);
            Assert.AreEqual(2, config.Screens.Count);

            var screen = config.Screens[0];
            Assert.AreEqual(0, screen.Adapter);
            Assert.IsNull(screen.Id);
            Assert.AreEqual(1920, screen.Width);
            Assert.AreEqual(1080, screen.Height);
            Assert.AreEqual(@"D:\Presentation\Images\Fallback.jpg", screen.FallbackImage);
            Assert.IsNotNull(screen.VisibleRegion);
            Assert.AreEqual(0, screen.VisibleRegion.X);
            Assert.AreEqual(0, screen.VisibleRegion.Y);
            Assert.AreEqual(1368, screen.VisibleRegion.Width);
            Assert.AreEqual(768, screen.VisibleRegion.Height);

            screen = config.Screens[1];
            Assert.AreEqual(1, screen.Adapter);
            Assert.IsNull(screen.Id);
            Assert.AreEqual(1440, screen.Width);
            Assert.AreEqual(900, screen.Height);
            Assert.AreEqual(string.Empty, screen.FallbackImage);
            Assert.IsNull(screen.VisibleRegion);

            Assert.IsNotNull(config.Device);
            Assert.AreEqual(MultiSampleTypes.None, config.Device.MultiSample);
            Assert.AreEqual(0, config.Device.MultiSampleQuality);
            Assert.AreEqual(true, config.Device.MultiThreaded);
            Assert.AreEqual(PresentFlags.None, config.Device.PresentFlag);
            Assert.AreEqual(PresentIntervals.One, config.Device.PresentationInterval);
            Assert.AreEqual(SwapEffects.Discard, config.Device.SwapEffect);

            Assert.IsNotNull(config.Text);
            Assert.AreEqual(TimeSpan.FromSeconds(3), config.Text.AlternationInterval);
            Assert.AreEqual(TimeSpan.FromSeconds(0.5), config.Text.BlinkInterval);
            Assert.AreEqual(FontQualities.AntiAliased, config.Text.FontQuality);
            Assert.AreEqual(TextMode.FontSprite, config.Text.TextMode);

            Assert.IsNotNull(config.Image);
            Assert.AreEqual(TimeSpan.FromSeconds(3600), config.Image.BitmapCacheTimeout);
            Assert.AreEqual(52428800, config.Image.MaxBitmapCacheBytes);
            Assert.AreEqual(1000000, config.Image.MaxCacheBytesPerBitmap);

            Assert.IsNotNull(config.Video);
            Assert.AreEqual(VideoMode.DirectXWindow, config.Video.VideoMode);
        }

        /// <summary>
        /// Tests that <c>AhdlcRenderer.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\DirectXRenderer\DirectXRenderer_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<RendererConfig>();
            configManager.FileName = "DirectXRenderer_v2.4.xml";
            configManager.XmlSchema = RendererConfig.Schema;
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
