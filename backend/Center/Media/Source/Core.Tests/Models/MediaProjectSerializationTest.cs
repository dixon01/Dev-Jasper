// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaProjectSerializationTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for MediaProjectTest and is intended
//   to contain all MediaProjectTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Common.Configuration.Infomedia.Common;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for MediaProjectTest and is intended
    /// to contain all MediaProjectTest Unit Tests
    /// </summary>
    [TestClass]
    public class MediaProjectSerializationTest
    {
        private readonly DateTime utcNow = new DateTime(2014, 03, 21, 10, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Tests if the serialization of a MediaProject is successful.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Unittest methods can be bigger."), TestMethod]
        public void TestSimpleSerialization()
        {
            var lastModified = this.utcNow.AddHours(2);
            var projectId = Guid.NewGuid();
            var pool1 = new PoolConfigDataModel
                            {
                                Name = "Pool1",
                                ResourceReferences =
                                    new List<ResourceReference> { new ResourceReference { Hash = "Hash" } }
                            };
            var pools = new List<PoolConfigDataModel> { pool1 };
            var project = new MediaProjectDataModel
                              {
                                  Authors = new List<string> { "TestUser" },
                                  DateCreated = this.utcNow,
                                  DateLastModified = lastModified,
                                  Resources =
                                      new List<ResourceInfo>
                                          {
                                              new ResourceInfo
                                                  {
                                                      Filename = "TestResource",
                                                      Hash = "Hash",
                                                      Type = ResourceType.Image
                                                  }
                                          },
                                  Pools = pools,
                                  ProjectId = projectId,
                                  InfomediaConfig = Helpers.CreateTestInfomediaConfigDataModel(),
                                  Replacements = new List<TextualReplacementDataModel>
                                                 {
                                                     new TextualReplacementDataModel
                                                     {
                                                         Code = "###",
                                                         Description = "TestDescription",
                                                         Filename = "TestFilename",
                                                         IsImageReplacement = false,
                                                         Number = 15,
                                                     }
                                                 },
                              };
            project.InfomediaConfig.Evaluations.Add(
                new EvaluationConfigDataModel
                    {
                        DisplayText = "TestDisplayText",
                        Evaluation =
                            new ConstantEvalDataModel
                                {
                                    DisplayText = "TestConstantDisplayText",
                                    Value = "25",
                                },
                        Name = "TestName",
                    });

            string output = string.Empty;
            try
            {
                using (var streamWriter = new StringWriter())
                {
                    var xmlSerializer = new XmlSerializer(typeof(MediaProjectDataModel));
                    using (var writer =
                        XmlWriter.Create(streamWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
                    {
                        xmlSerializer.Serialize(writer, project);
                        output = streamWriter.ToString();
                        Assert.IsFalse(string.IsNullOrWhiteSpace(output));
                    }
                }
            }
            catch (Exception exception)
            {
                var message = string.Format("Serialization threw an exception: {0}", exception);
                Assert.Fail(message);
            }

            var serializer = new XmlSerializer(typeof(MediaProjectDataModel));
            MediaProjectDataModel config;
            using (var reader = XmlReader.Create(new StringReader(output)))
            {
                config = (MediaProjectDataModel)serializer.Deserialize(reader);
            }

            ValidateMediaProject(config, lastModified, projectId);
        }

        /// <summary>
        /// Tests the serialization of a MediaProject with an empty predefined formula.
        /// </summary>
        [TestMethod]
        public void SerializationEmptyPredefinedFormulaTest()
        {
             var lastModified = this.utcNow.AddHours(2);
            var project = this.SetupMediaProjectBase(lastModified);
            project.InfomediaConfig.Evaluations.Add(
                new EvaluationConfigDataModel { Name = "Test formula", DisplayText = "Testformula" });
            string output = string.Empty;
            try
            {
                using (var streamWriter = new StringWriter())
                {
                    var xmlSerializer = new XmlSerializer(typeof(MediaProjectDataModel));
                    using (var writer =
                        XmlWriter.Create(streamWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
                    {
                        xmlSerializer.Serialize(writer, project);
                        output = streamWriter.ToString();
                        Assert.IsFalse(string.IsNullOrWhiteSpace(output));
                    }
                }
            }
            catch (Exception exception)
            {
                var message = string.Format("Serialization threw an exception: {0}", exception);
                Assert.Fail(message);
            }

            var serializer = new XmlSerializer(typeof(MediaProjectDataModel));
            MediaProjectDataModel config;
            using (var reader = XmlReader.Create(new StringReader(output)))
            {
                config = (MediaProjectDataModel)serializer.Deserialize(reader);
            }

            Assert.AreEqual(1, config.InfomediaConfig.Evaluations.Count);
            Assert.AreEqual("Test formula", config.InfomediaConfig.Evaluations[0].Name);
            Assert.AreEqual("Testformula", config.InfomediaConfig.Evaluations[0].DisplayText);
            Assert.IsNull(config.InfomediaConfig.Evaluations[0].Evaluation);
        }

        /// <summary>
        /// Tests the serialization of a MediaProject with an animation in a property.
        /// </summary>
        [TestMethod]
        public void SerializationAnimationTest()
        {
            var lastModified = this.utcNow.AddHours(2);
            var project = this.SetupMediaProjectBase(lastModified);
            var textElement =
                project.InfomediaConfig.Layouts[1].Resolutions[0].Elements.FirstOrDefault(
                    e => e is StaticTextElementDataModel) as TextElementDataModel;
            textElement.ValueProperty = new AnimatedDynamicPropertyDataModel
                                            {
                                                Animation = new PropertyChangeAnimationDataModel
                                                        {
                                                            Duration = TimeSpan.FromSeconds(10),
                                                            Type = PropertyChangeAnimationType.FadeThroughNothing
                                                        }
                                            };
            string output = string.Empty;
            try
            {
                using (var streamWriter = new StringWriter())
                {
                    var xmlSerializer = new XmlSerializer(typeof(MediaProjectDataModel));
                    using (
                        var writer =
                            XmlWriter.Create(streamWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
                    {
                        xmlSerializer.Serialize(writer, project);
                        output = streamWriter.ToString();
                        Assert.IsFalse(string.IsNullOrWhiteSpace(output));
                    }
                }
            }
            catch (Exception exception)
            {
                var message = string.Format("Serialization threw an exception: {0}", exception);
                Assert.Fail(message);
            }

            var serializer = new XmlSerializer(typeof(MediaProjectDataModel));
            MediaProjectDataModel config;
            using (var reader = XmlReader.Create(new StringReader(output)))
            {
                config = (MediaProjectDataModel)serializer.Deserialize(reader);
            }

            Assert.IsNotNull(config.InfomediaConfig.Layouts);
            Assert.AreEqual(2, config.InfomediaConfig.Layouts.Count);
            Assert.IsNotNull(config.InfomediaConfig.Layouts[1].Resolutions);
            var resolution = config.InfomediaConfig.Layouts[1].Resolutions[0];
            Assert.AreEqual(3, resolution.Elements.Count);
            var element = resolution.Elements[2] as TextElementDataModel;
            Assert.IsNotNull(element);
            Assert.IsNotNull(element.ValueProperty);
            Assert.IsNotNull(element.ValueProperty.Animation);
            Assert.AreEqual(TimeSpan.FromSeconds(10), element.ValueProperty.Animation.Duration);
            Assert.AreEqual(PropertyChangeAnimationType.FadeThroughNothing, element.ValueProperty.Animation.Type);
        }

        private static void ValidateMediaProject(MediaProjectDataModel config, DateTime lastModified, Guid projectId)
        {
            Assert.IsNotNull(config);

            Assert.IsNotNull(config.Authors);
            Assert.AreEqual(1, config.Authors.Count);
            Assert.AreEqual("TestUser", config.Authors[0]);

            Assert.AreEqual(lastModified, config.DateLastModified);

            Assert.AreEqual(projectId, config.ProjectId);

            Assert.IsNotNull(config.Pools);
            Assert.AreEqual(1, config.Pools.Count);
            Assert.AreEqual("Pool1", config.Pools[0].Name);

            Assert.IsNotNull(config.Pools[0].ResourceReferences);
            Assert.AreEqual(1, config.Pools[0].ResourceReferences.Count);
            Assert.AreEqual("Hash", config.Pools[0].ResourceReferences[0].Hash);

            Assert.IsNotNull(config.Resources);
            Assert.AreEqual(1, config.Resources.Count);
            Assert.AreEqual("TestResource", config.Resources[0].Filename);
            Assert.AreEqual(ResourceType.Image, config.Resources[0].Type);

            Assert.IsNotNull(config.Replacements);
            Assert.AreEqual(1, config.Replacements.Count);
            Assert.AreEqual("###", config.Replacements[0].Code);
            Assert.AreEqual("TestDescription", config.Replacements[0].Description);
            Assert.AreEqual("TestFilename", config.Replacements[0].Filename);
            Assert.AreEqual(false, config.Replacements[0].IsImageReplacement);
            Assert.AreEqual(15, config.Replacements[0].Number);

            Assert.IsNotNull(config.InfomediaConfig.Evaluations);
            Assert.AreEqual(1, config.InfomediaConfig.Evaluations.Count);

            Assert.AreEqual("TestDisplayText", config.InfomediaConfig.Evaluations[0].DisplayText);
            Assert.AreEqual("TestName", config.InfomediaConfig.Evaluations[0].Name);
            Assert.IsNotNull(config.InfomediaConfig.Evaluations[0].Evaluation);
            Assert.IsInstanceOfType(config.InfomediaConfig.Evaluations[0].Evaluation, typeof(ConstantEvalDataModel));
            Assert.AreEqual("TestConstantDisplayText", config.InfomediaConfig.Evaluations[0].Evaluation.DisplayText);
            Assert.AreEqual("25", ((ConstantEvalDataModel)config.InfomediaConfig.Evaluations[0].Evaluation).Value);

            ValidateInfomediaConfig(config);
        }

        private static void ValidateInfomediaConfig(MediaProjectDataModel config)
        {
            Assert.IsNotNull(config.InfomediaConfig);
            Assert.IsNotNull(config.InfomediaConfig.Layouts);
            Assert.AreEqual(2, config.InfomediaConfig.Layouts.Count);
            var layout = config.InfomediaConfig.Layouts.Find(l => l.Resolutions[0].Elements.Count > 1);
            Assert.IsNotNull(layout);
            var elements = layout.Resolutions[0].Elements;
            Assert.AreEqual(3, elements.Count);
            var dynamicText = elements.Find(e => e.ElementName == "DynamicText1") as DynamicTextElementDataModel;
            Assert.IsNotNull(dynamicText);
            Assert.AreEqual("TestText", dynamicText.TestData);
            Assert.IsNotNull(dynamicText.SelectedDictionaryValue);
            Assert.AreEqual("StopName", dynamicText.SelectedDictionaryValue.Column.Name);
            Assert.AreEqual("Stops", dynamicText.SelectedDictionaryValue.Table.Name);

            Assert.IsNotNull(config.InfomediaConfig.Cycles);
            Assert.IsNotNull(config.InfomediaConfig.Cycles.StandardCycles);
            Assert.AreEqual(1, config.InfomediaConfig.Cycles.StandardCycles.Count);
            Assert.IsNotNull(config.InfomediaConfig.Cycles.StandardCycles[0].Sections);
            Assert.AreEqual(1, config.InfomediaConfig.Cycles.StandardCycles[0].Sections.Count);
            Assert.AreEqual(
                TimeSpan.FromSeconds(10), config.InfomediaConfig.Cycles.StandardCycles[0].Sections[0].Duration);

            Assert.IsNotNull(config.InfomediaConfig.Cycles.EventCycles);
            Assert.AreEqual(1, config.InfomediaConfig.Cycles.EventCycles.Count);
            var eventCycle = config.InfomediaConfig.Cycles.EventCycles[0];
            Assert.IsNotNull(eventCycle.Trigger);
            Assert.AreEqual(1, eventCycle.Trigger.Coordinates.Count);
            Assert.IsTrue(eventCycle.Enabled);
            Assert.IsNotNull(config.InfomediaConfig.CyclePackages);
            Assert.AreEqual(1, config.InfomediaConfig.CyclePackages.Count);
            Assert.AreEqual("TftPackage", config.InfomediaConfig.CyclePackages[0].Name);
            Assert.IsNotNull(config.InfomediaConfig.CyclePackages[0].StandardCycles);
            Assert.AreEqual("Test cycle", config.InfomediaConfig.CyclePackages[0].StandardCycles[0].Reference);

            Assert.IsNotNull(config.InfomediaConfig.MasterPresentation);
            Assert.IsNotNull(config.InfomediaConfig.MasterPresentation.MasterCycles);
            Assert.AreEqual(1, config.InfomediaConfig.MasterPresentation.MasterCycles.Count);
            Assert.AreEqual("Master cycle", config.InfomediaConfig.MasterPresentation.MasterCycles[0].Name);
        }

        private MediaProjectDataModel SetupMediaProjectBase(DateTime lastModified)
        {
            var projectId = Guid.NewGuid();
            var pool1 = new PoolConfigDataModel
            {
                Name = "Pool1",
                ResourceReferences =
                    new List<ResourceReference> { new ResourceReference { Hash = "Hash" } }
            };
            var pools = new List<PoolConfigDataModel> { pool1 };
            var project = new MediaProjectDataModel
            {
                Authors = new List<string> { "TestUser" },
                DateCreated = this.utcNow,
                DateLastModified = lastModified,
                Resources =
                    new List<ResourceInfo>
                                          {
                                              new ResourceInfo
                                                  {
                                                      Filename = "TestResource",
                                                      Hash = "Hash",
                                                      Type = ResourceType.Image
                                                  }
                                          },
                Pools = pools,
                ProjectId = projectId,
                InfomediaConfig = Helpers.CreateTestInfomediaConfigDataModel(),
                Replacements = new List<TextualReplacementDataModel>
                                                 {
                                                     new TextualReplacementDataModel
                                                     {
                                                         Code = "###",
                                                         Description = "TestDescription",
                                                         Filename = "TestFilename",
                                                         IsImageReplacement = false,
                                                         Number = 15,
                                                     }
                                                 },
            };

            return project;
        }
    }
}
