// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebmediaSerializationTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WebmediaSerializationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Tests.Webmedia
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Webmedia;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The web.media serialization and deserialization test.
    /// </summary>
    [TestClass]
    public class WebmediaSerializationTest
    {
        private const string ConfigDirectory = @"Webmedia\";
        private const string Webmedia1 = @"webmedia1.wm2";

        /// <summary>
        /// Simple test for serialization.
        /// </summary>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2100:CodeLineMustNotBeLongerThan",
            Justification = "Readability is better like that.")]
        [TestMethod]
        public void TestSimpleSerialization()
        {
            var config = new WebmediaConfig();

            config.CreationDate = new DateTime(2013, 2, 14, 13, 22, 55);

            var cycle = new WebmediaCycleConfig();
            config.Cycles.Add(cycle);
            cycle.Name = "Main Cycle";

            WebmediaElementBase element = new ImageWebmediaElement
            {
                Name = "My Image",
                Duration = TimeSpan.FromSeconds(15),
                Filename = "_content_123_.jpg",
                Frame = 1,
                Scaling = ElementScaling.Fixed
            };
            cycle.Elements.Add(element);

            element = new VideoWebmediaElement
            {
                Name = "My Video",
                Duration = TimeSpan.FromSeconds(20),
                VideoUri = "_content_124_.mpg",
                Frame = 1,
                Scaling = ElementScaling.Scale
            };
            cycle.Elements.Add(element);

            var serializer = new XmlSerializer(config.GetType());

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var memory = new MemoryStream();
            using (var writer = new XmlTextWriter(memory, Encoding.UTF8) { Formatting = Formatting.None })
            {
                serializer.Serialize(writer, config, namespaces);
                writer.Close();
            }

            var xml = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual(
                "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                + "<WebMedia Version=\"1.2\" Created=\"2013-02-14 13:22:55\">"
                + "<Cycles>"
                + "<Cycle Name=\"Main Cycle\">"
                + "<Image Name=\"My Image\" Frame=\"1\" Duration=\"PT15S\" Filename=\"_content_123_.jpg\" Scaling=\"Fixed\" />"
                + "<Video Name=\"My Video\" Frame=\"1\" Duration=\"PT20S\" VideoUri=\"_content_124_.mpg\" Scaling=\"Scale\" />"
                + "</Cycle></Cycles></WebMedia>",
                xml);
        }

        /// <summary>
        /// Extended test for serialization.
        /// </summary>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2100:CodeLineMustNotBeLongerThan",
            Justification = "Better readability.")]
        [TestMethod]
        public void TestExtendedSerialization()
        {
            var config = new WebmediaConfig();

            config.CreationDate = new DateTime(2013, 2, 14, 13, 22, 55);

            var cycle = new WebmediaCycleConfig();
            config.Cycles.Add(cycle);
            cycle.Name = "Main Cycle";
            cycle.EnabledProperty =
                new DynamicProperty(
                    new StringCompareEval
                        {
                            Evaluation = new GenericEval { Language = 0, Table = 10, Row = 0, Column = 5 },
                            Value = "Test"
                        });

            WebmediaElementBase element = new ImageWebmediaElement
            {
                Name = "My Image",
                Duration = TimeSpan.FromSeconds(15),
                Filename = "_content_123_.jpg",
                Scaling = ElementScaling.Fixed,
                Frame = 1,
                EnabledProperty =
                    new DynamicProperty(
                        new IntegerCompareEval
                            {
                                Evaluation = new GenericEval { Language = 0, Table = 11, Row = 1, Column = 2 },
                                Begin = 17,
                                End = 33
                            })
            };
            cycle.Elements.Add(element);

            var serializer = new XmlSerializer(config.GetType());

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var memory = new MemoryStream();
            using (var writer = new XmlTextWriter(memory, Encoding.UTF8) { Formatting = Formatting.None })
            {
                serializer.Serialize(writer, config, namespaces);
                writer.Close();
            }

            var xml = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual(
                "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                + "<WebMedia Version=\"1.2\" Created=\"2013-02-14 13:22:55\">"
                + "<Cycles><Cycle Name=\"Main Cycle\"><Enabled><StringCompare Value=\"Test\">"
                + "<Generic Lang=\"0\" Table=\"10\" Column=\"5\" Row=\"0\" /></StringCompare></Enabled>"
                + "<Image Name=\"My Image\" Frame=\"1\" Duration=\"PT15S\" Filename=\"_content_123_.jpg\" Scaling=\"Fixed\">"
                + "<Enabled><IntegerCompare Begin=\"17\" End=\"33\">"
                + "<Generic Lang=\"0\" Table=\"11\" Column=\"2\" Row=\"1\" /></IntegerCompare></Enabled></Image>"
                + "</Cycle></Cycles></WebMedia>",
                xml);
        }

        /// <summary>
        /// A simple test for de-serialization.
        /// </summary>
        [TestMethod]
        [DeploymentItem(ConfigDirectory + Webmedia1)]
        public void TestSimpleDeserialization()
        {
            var configMgr = new ConfigManager<WebmediaConfig> { FileName = Webmedia1 };

            var config = configMgr.Config;
            Assert.IsNotNull(config);

            Assert.AreEqual(new Version(1, 2), config.Version);
            Assert.AreEqual(new DateTime(2013, 3, 17, 15, 42, 18), config.CreationDate);
            Assert.IsNotNull(config.Cycles);
            Assert.AreEqual(2, config.Cycles.Count);

            var cycle = config.Cycles[0];
            Assert.IsTrue(cycle.Enabled);
            Assert.AreEqual("Conditional", cycle.Name);
            Assert.IsNotNull(cycle.EnabledProperty);

            var stringCompare = cycle.EnabledProperty.Evaluation as StringCompareEval;
            Assert.IsNotNull(stringCompare);
            Assert.AreEqual("Hello World", stringCompare.Value);

            var generic = stringCompare.Evaluation as GenericEval;
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(12, generic.Table);
            Assert.AreEqual(4, generic.Column);
            Assert.AreEqual(0, generic.Row);

            Assert.AreEqual(2, cycle.Elements.Count);

            var image = cycle.Elements[0] as ImageWebmediaElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("An image", image.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), image.Duration);
            Assert.AreEqual(1, image.Frame);
            Assert.AreEqual("_content_321_.jpg", image.Filename);

            var video = cycle.Elements[1] as VideoWebmediaElement;
            Assert.IsNotNull(video);
            Assert.AreEqual("A video", video.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(19), video.Duration);
            Assert.AreEqual(0, video.Frame);
            Assert.AreEqual("_content_322_.wmv", video.VideoUri);

            cycle = config.Cycles[1];
            Assert.IsTrue(cycle.Enabled);
            Assert.AreEqual("Main Cycle", cycle.Name);
            Assert.IsNull(cycle.EnabledProperty);

            Assert.AreEqual(2, cycle.Elements.Count);

            image = cycle.Elements[0] as ImageWebmediaElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("Another image", image.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), image.Duration);
            Assert.AreEqual(0, image.Frame);
            Assert.AreEqual("_content_324_.jpg", image.Filename);

            var layout = cycle.Elements[1] as LayoutWebmediaElement;
            Assert.IsNotNull(layout);
            Assert.AreEqual("Full Layout", layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(25), layout.Duration);
            Assert.AreEqual(0, layout.Frame);
            Assert.AreEqual(2, layout.Elements.Count);
            Assert.IsInstanceOfType(layout.Elements[0], typeof(ImageElement));
            Assert.IsInstanceOfType(layout.Elements[1], typeof(VideoElement));
        }
    }
}
