// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaConfigurationSerializationTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaConfigurationSerializationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///  Defines tests for the serialization / deserialization of the <see cref="MediaConfiguration"/> object.
    /// </summary>
    [TestClass]
    public class MediaConfigurationSerializationTest
    {
        /// <summary>
        /// Tests the deserialization of a valid configuration.
        /// </summary>
        [TestMethod]
        public void DeserializeValidConfigurationTest()
        {
            var resourceSettings = new ResourceSettings
                                       {
                                           MaxUsedDiskSpace = 100,
                                           MinRemainingDiskSpace = 1000,
                                           RemoveLocalResourceAfter = TimeSpan.FromMinutes(1),
                                       };
            var physicalScreenSettings = CreatePhysicalScreenSettings();
            var mediaConfiguration = new MediaConfiguration
                                         {
                                             ResourceSettings = resourceSettings,
                                             PhysicalScreenSettings = physicalScreenSettings
                                         };
            var serializer = new XmlSerializer(typeof(MediaConfiguration));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, mediaConfiguration);
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    using (var xmlReader = XmlReader.Create(streamReader, CreateXmlReaderSettings()))
                    {
                        var result = (MediaConfiguration)serializer.Deserialize(xmlReader);
                        Assert.IsNotNull(result.ResourceSettings);
                        Assert.AreEqual(resourceSettings.MaxUsedDiskSpace, result.ResourceSettings.MaxUsedDiskSpace);
                        Assert.AreEqual(
                            resourceSettings.MinRemainingDiskSpace, result.ResourceSettings.MinRemainingDiskSpace);
                        Assert.AreEqual(
                            resourceSettings.RemoveLocalResourceAfter,
                            result.ResourceSettings.RemoveLocalResourceAfter);
                        Assert.IsNull(result.ResourceSettings.LocalResourcePath);
                        var screenSettings = result.PhysicalScreenSettings;
                        Assert.IsNotNull(screenSettings);
                        Assert.AreEqual(3, screenSettings.PhysicalScreenTypes.Count);
                        var screenTypeTft = screenSettings.PhysicalScreenTypes.FirstOrDefault(s => s.Name == "TFT");
                        Assert.IsNotNull(screenTypeTft);
                        Assert.AreEqual(2, screenTypeTft.AvailableResolutions.Count);
                        var resolutionWithLayouts =
                            screenTypeTft.AvailableResolutions.FirstOrDefault(r => r.MasterLayouts.Count > 0);
                        Assert.IsNotNull(resolutionWithLayouts);
                        Assert.AreEqual(1920, resolutionWithLayouts.Width);
                        Assert.AreEqual(630, resolutionWithLayouts.Height);
                        Assert.AreEqual(1, resolutionWithLayouts.MasterLayouts.Count);
                        var layout = resolutionWithLayouts.MasterLayouts[0];
                        Assert.IsNotNull(layout);

                        var screenTypeAudio = screenSettings.PhysicalScreenTypes.FirstOrDefault(s => s.Name == "Audio");
                        Assert.IsNotNull(screenTypeAudio);
                        Assert.AreEqual(0, screenTypeAudio.AvailableResolutions.Count);
                    }
                }
            }
        }

        /// <summary>
        /// Tests the deserialization of a valid directX renderer part of the configuration.
        /// </summary>
        [TestMethod]
        public void DeserializeValidDirectRendererPartTest()
        {
            var resourceSettings = new ResourceSettings
            {
                MaxUsedDiskSpace = 100,
                MinRemainingDiskSpace = 1000,
                RemoveLocalResourceAfter = TimeSpan.FromMinutes(1),
            };
            var physicalScreenSettings = CreatePhysicalScreenSettings();
            var rendererSettings = this.CreateDirectXRendererSettings();
            var mediaConfiguration = new MediaConfiguration
            {
                ResourceSettings = resourceSettings,
                PhysicalScreenSettings = physicalScreenSettings,
                DirectXRendererConfig = rendererSettings
            };
            var serializer = new XmlSerializer(typeof(MediaConfiguration));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, mediaConfiguration);
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    using (var xmlReader = XmlReader.Create(streamReader, CreateXmlReaderSettings()))
                    {
                        var result = (MediaConfiguration)serializer.Deserialize(xmlReader);
                        Assert.IsNotNull(result.DirectXRendererConfig);
                        Assert.AreEqual(VideoMode.VlcWindow, result.DirectXRendererConfig.Video.VideoMode);
                        Assert.AreEqual(TextMode.Gdi, result.DirectXRendererConfig.Text.TextMode);
                        Assert.AreEqual(FontQualities.Draft, result.DirectXRendererConfig.Text.FontQuality);
                    }
                }
            }
        }

        /// <summary>
        /// Tests the deserialization of an invalid configuration.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XmlSchemaValidationException))]
        public void DeserializeInvalidConfigurationTest()
        {
            var mediaConfiguration = new MediaConfiguration();
            var serializer = new XmlSerializer(typeof(MediaConfiguration));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, mediaConfiguration);
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    using (var xmlReader = XmlReader.Create(streamReader, CreateXmlReaderSettings()))
                    {
                        try
                        {
                            // ReSharper disable UnusedVariable
                            var configuration = (MediaConfiguration)serializer.Deserialize(xmlReader);
                            // ReSharper restore UnusedVariable
                        }
                        catch (InvalidOperationException operationException)
                        {
                            throw operationException.InnerException;
                        }
                    }
                }
            }
        }

        private static XmlReaderSettings CreateXmlReaderSettings()
        {
            var schema = typeof(MediaConfiguration).Assembly.GetManifestResourceStream(
                    "Gorba.Center.Media.Core.Configuration.MediaConfiguration.xsd");
            Assert.IsNotNull(schema);
            XmlSchema xmlSchema;
            XmlSchema directXmlSchema;
            using (schema)
            {
                xmlSchema = XmlSchema.Read(schema, (sender, args) => Assert.Fail("Schema not found."));
            }

            var directXSchema =
                typeof(MediaConfiguration).Assembly.GetManifestResourceStream(
                    "Gorba.Center.Media.Core.Configuration.DirectXRenderer.xsd");
            Assert.IsNotNull(directXSchema);
            using (directXSchema)
            {
                directXmlSchema = XmlSchema.Read(
                    directXSchema,
                    (sender, args) => Assert.Fail("DirectXSchema not found."));
            }

            var schemas = new XmlSchemaSet();
            schemas.Add(xmlSchema);
            schemas.Add(directXmlSchema);
            var xmlSettings = new XmlReaderSettings
                                  {
                                      Schemas = schemas,
                                      ValidationType = ValidationType.Schema,
                                      ValidationFlags =
                                          XmlSchemaValidationFlags.ProcessIdentityConstraints
                                          | XmlSchemaValidationFlags.ReportValidationWarnings
                                  };
            xmlSettings.ValidationEventHandler += (sender, args) =>
                {
                    if (args.Severity == XmlSeverityType.Error)
                    {
                        throw args.Exception;
                    }
                };
            return xmlSettings;
        }

        private static PhysicalScreenSettings CreatePhysicalScreenSettings()
        {
            var physicalScreenSettings = new PhysicalScreenSettings();
            var physicalScreenTypeLed = new PhysicalScreenTypeConfig
            {
                Name = "LED",
                AvailableResolutions =
                    new List<ResolutionConfiguration>
                                             {
                                                 new ResolutionConfiguration
                                                     {
                                                         Height = 16,
                                                         Width = 112,
                                                     }
                                             }
            };
            var physicalScreenTypeAudio = new PhysicalScreenTypeConfig { Name = "Audio" };
            var masterLayout = new MasterLayout
            {
                Name = "Split",
                Columns = "*.*",
                Rows = "*",
                HorizontalGaps = "10",
                VerticalGaps = "0",
            };
            var masterLayouts = new List<MasterLayout> { masterLayout };
            var physicalScreenTypeTft = new PhysicalScreenTypeConfig
            {
                Name = "TFT",
                AvailableResolutions =
                    new List<ResolutionConfiguration>
                                             {
                                                 new ResolutionConfiguration
                                                     {
                                                         Height = 600,
                                                         Width = 800,
                                                     },
                                                 new ResolutionConfiguration
                                                     {
                                                         Height = 630,
                                                         Width = 1920,
                                                         MasterLayouts = masterLayouts
                                                     }
                                             }
            };

            physicalScreenSettings.PhysicalScreenTypes = new List<PhysicalScreenTypeConfig>
                                                             {
                                                                 physicalScreenTypeTft,
                                                                 physicalScreenTypeAudio,
                                                                 physicalScreenTypeLed
                                                             };

            return physicalScreenSettings;
        }

        private RendererConfig CreateDirectXRendererSettings()
        {
            var config = new RendererConfig();
            config.Video.VideoMode = VideoMode.VlcWindow;
            config.Text.TextMode = TextMode.Gdi;
            config.Text.FontQuality = FontQualities.Draft;
            return config;
        }
    }
}
