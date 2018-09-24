// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfomediaConfigDataViewModelTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests dirty handling and export.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.DataViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests the infomedia config including export.
    /// </summary>
    [TestClass]
    public class InfomediaConfigDataViewModelTest
    {
        private static readonly DateTime UtcNow = new DateTime(2014, 2, 25, 13, 34, 12, DateTimeKind.Utc);

        private MediaProjectDataViewModel project;

        /// <summary>
        /// Initializes this test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var timeProviderMock = new Mock<TimeProvider>();
            timeProviderMock.SetupGet(provider => provider.UtcNow).Returns(UtcNow);
            TimeProvider.Current = timeProviderMock.Object;
            ResetServiceLocator();
            var applicationState = new Mock<IMediaApplicationState>();
            this.project = new MediaProjectDataViewModel();
            applicationState.Setup(state => state.CurrentProject).Returns(this.project);

            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(applicationState.Object);
        }

        /// <summary>
        /// Cleanups this test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            TimeProvider.ResetToDefault();
            ResetServiceLocator();
            this.project = null;
        }

        /// <summary>
        /// Verifies the handling of the <see cref="InfomediaConfigDataViewModel.IsDirty"/> flag.
        /// </summary>
        [TestMethod]
        public void IsDirtyTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var infomediaConfigDataViewModel = new InfomediaConfigDataViewModel(shellMock.Object);
            var layoutConfigDataViewModels = new ExtendedObservableCollection<LayoutConfigDataViewModel>();
            infomediaConfigDataViewModel.Layouts = layoutConfigDataViewModels;
            Assert.IsFalse(infomediaConfigDataViewModel.IsDirty);
            layoutConfigDataViewModels.Add(new LayoutConfigDataViewModel(shellMock.Object));
            Assert.IsTrue(infomediaConfigDataViewModel.IsDirty);
        }

        /// <summary>
        /// Simple test to export a data view model.
        /// </summary>
        [TestMethod]
        public void ExportTest()
        {
            var config = Helpers.CreateDefaultInfomediaConfig();
            var converted = config.Export();
            Assert.AreEqual(UtcNow, converted.CreationDate);
            Assert.AreEqual(2, converted.Version.Major);

            Assert.AreEqual(1, converted.PhysicalScreens.Count);
            var tftScreen = converted.PhysicalScreens.SingleOrDefault(screenConfig => screenConfig.Name == "TftScreen");
            Assert.IsNotNull(tftScreen);
            Assert.AreEqual(PhysicalScreenType.TFT, tftScreen.Type);

            Assert.AreEqual(1, converted.VirtualDisplays.Count);
            var tftDisplay =
                converted.VirtualDisplays.SingleOrDefault(displayConfig => displayConfig.Name == "TftDisplay");
            Assert.IsNotNull(tftDisplay);
            Assert.AreEqual("TftPackage", tftDisplay.CyclePackage);

            Assert.AreEqual(1, converted.Cycles.StandardCycles.Count);
            Assert.AreEqual(1, converted.MasterPresentation.MasterCycles.Count);
            var masterCycle = converted.MasterPresentation.MasterCycles.SingleOrDefault(c => c.Name == "Master cycle");
            Assert.IsNotNull(masterCycle);

            var masterSectionFound =
                masterCycle.Sections.SingleOrDefault(@base => @base.Layout == "StandardMasterLayout");
            Assert.IsNotNull(masterSectionFound);

            var masterLayout =
                converted.MasterPresentation.MasterLayouts.SingleOrDefault(
                    model => model.Name == "StandardMasterLayout");
            Assert.IsNotNull(masterLayout);

            var physicalScreenRefFound =
                masterLayout.PhysicalScreens.SingleOrDefault(refConfig => refConfig.Reference == "TftScreen");
            Assert.IsNotNull(physicalScreenRefFound);

            var virtualDisplayRefFound =
                physicalScreenRefFound.VirtualDisplays
                .SingleOrDefault(refConfig => refConfig.Reference == "TftDisplay");
            Assert.IsNotNull(virtualDisplayRefFound);
            Assert.AreEqual(0, virtualDisplayRefFound.X);
            Assert.AreEqual(0, virtualDisplayRefFound.Y);

            var testCycleFound =
                converted.Cycles.StandardCycles.SingleOrDefault(cycleConfig => cycleConfig.Name == "Test cycle");
            Assert.IsNotNull(testCycleFound);
            Assert.IsTrue(testCycleFound.Enabled);

            var testCycleLayoutFound =
                converted.Layouts.SingleOrDefault(layoutConfig => layoutConfig.Name == "Test cycle");
            Assert.IsNotNull(testCycleLayoutFound);

            var resolutionConfigFound =
                testCycleLayoutFound.Resolutions.SingleOrDefault(c => c.Height == 768 && c.Width == 1368);
            Assert.IsNotNull(resolutionConfigFound);

            Assert.AreEqual(1, resolutionConfigFound.Elements.Count);
            var staticTextElement = resolutionConfigFound.Elements.OfType<TextElement>().SingleOrDefault();
            Assert.IsNotNull(staticTextElement);
            Assert.AreEqual("Static text", staticTextElement.Value);
        }

        /// <summary>
        /// Tests that the exported class contains the subclasses of a list declared with the base class.
        /// </summary>
        [TestMethod]
        public void ExportClassWithListOfBaseClasses()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var masterSectionViewModel = new MasterSectionConfigDataViewModel(mediaShellMock.Object);
            masterSectionViewModel.Duration.Value = TimeSpan.FromMinutes(1);
            var sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase>
                               {
                                   masterSectionViewModel
                               };
            var masterCycleDataViewModel = new MasterCycleConfigDataViewModel(mediaShellMock.Object)
                                               {
                                                   Sections =
                                                       sections,
                                               };
            masterCycleDataViewModel.Name.Value = "MasterCycle";
            var entity = masterCycleDataViewModel.Export();
            Assert.AreEqual("MasterCycle", entity.Name);
            Assert.IsNotNull(entity.Sections);
            Assert.AreEqual(1, entity.Sections.Count);
            var sectionEntity = entity.Sections.First() as MasterSectionConfig;
            Assert.IsNotNull(sectionEntity);
            Assert.AreEqual(TimeSpan.FromMinutes(1), sectionEntity.Duration);
        }

        /// <summary>
        /// Tests that the exported class contains the evaluation part of a dynamic property.
        /// </summary>
        [TestMethod]
        public void ExportClassWithEvaluationTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object);
            var evaluation = new GenericEvalDataViewModel(mediaShellMock.Object)
                                 {
                                     Column = { Value = 1 },
                                     Table = { Value = 10 },
                                     Language = { Value = 2 },
                                     Row = { Value = 3 }
                                 };

            textViewModel.Value.Formula = evaluation;

            var converted = (TextElement)textViewModel.Export();
            Assert.IsNotNull(converted);
            Assert.IsNotNull(converted.ValueProperty);
            var eval = converted.ValueProperty.Evaluation as GenericEval;
            Assert.IsNotNull(eval);
            Assert.AreEqual(10, eval.Table);
            Assert.AreEqual(3, eval.Row);
            Assert.AreEqual(2, eval.Language);
            Assert.AreEqual(1, eval.Column);
        }

        /// <summary>
        /// Tests that an invalid formula is not exported.
        /// </summary>
        [TestMethod]
        public void ExportClassWithInvalidEvaluationTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object)
                                    {
                                        Value = { Value = "FallBackValue" }
                                    };

            var evaluation = new IfEvalDataViewModel(mediaShellMock.Object)
                                 {
                                     Condition = new ConstantEvalDataViewModel(mediaShellMock.Object)
                                             {
                                                 Value = { Value = "Test" }
                                             },
                                     Then = null
                                 };

            textViewModel.Value.Formula = evaluation;

            var converted = (TextElement)textViewModel.Export();
            Assert.IsNotNull(converted);
            Assert.IsNull(converted.ValueProperty);
            Assert.AreEqual(converted.Value, textViewModel.Value.Value);
        }

        /// <summary>
        /// Tests that the export including a pool.
        /// </summary>
        [TestMethod]
        public void ExportWithPoolTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var infomediaConfig = Helpers.CreateDefaultInfomediaConfig();
            const string BaseDirectory = @"D:\Pools\MediaPool\";
            var pool = new PoolConfigDataViewModel(mediaShellMock.Object)
                           {
                               Name = { Value = "MediaPool" },
                               BaseDirectory = { Value = BaseDirectory }
                           };
            infomediaConfig.Pools.Add(pool);
            var config = infomediaConfig.Export();
            Assert.AreEqual(1, config.Pools.Count);
            var infoPool = config.Pools.First();
            Assert.AreEqual("MediaPool", infoPool.Name);
            Assert.AreEqual(BaseDirectory, infoPool.BaseDirectory);
        }

        /// <summary>
        /// Tests the export including an animation but without an Evaluation.
        /// </summary>
        [TestMethod]
        public void ExportWithAnimationWithoutEvaluationTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object)
            {
                Value = { Value = "FallBackValue" }
            };

            var animation = new AnimationDataViewModel(mediaShellMock.Object)
                                {
                                    Type =
                                        {
                                            Value =
                                                PropertyChangeAnimationType
                                                .Linear
                                        },
                                    Duration =
                                        {
                                            Value =
                                                TimeSpan
                                                .FromSeconds(10)
                                        }
                                };

            textViewModel.Value.Animation = animation;

            var converted = (TextElement)textViewModel.Export();
            Assert.IsNotNull(converted);
            Assert.IsNull(converted.ValueProperty);
            var serializer = new XmlSerializer(typeof(TextElement));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, converted);
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    using (var xmlReader = XmlReader.Create(streamReader))
                    {
                        var result = (TextElement)serializer.Deserialize(xmlReader);
                        Assert.IsNotNull(result);
                        Assert.IsNull(result.ValueProperty);
                    }
                }
            }
        }

        /// <summary>
        /// Tests the export including an animation and an evaluation in the same property.
        /// </summary>
        [TestMethod]
        public void ExportWithAnimationAndEvaluationTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object)
            {
                Value = { Value = "FallBackValue" }
            };

            var animation = new AnimationDataViewModel(mediaShellMock.Object)
            {
                Type =
                {
                    Value =
                        PropertyChangeAnimationType
                        .Linear
                },
                Duration =
                {
                    Value =
                        TimeSpan
                        .FromSeconds(10)
                }
            };
            var evaluation = new GenericEvalDataViewModel(mediaShellMock.Object)
            {
                Column = { Value = 1 },
                Table = { Value = 10 },
                Language = { Value = 2 },
                Row = { Value = 3 }
            };

            textViewModel.Value.Formula = evaluation;
            textViewModel.Value.Animation = animation;

            var converted = (TextElement)textViewModel.Export();

            Assert.IsNotNull(converted);
            Assert.IsNotNull(converted.ValueProperty);
            var eval = converted.ValueProperty.Evaluation as GenericEval;
            Assert.IsNotNull(eval);
            Assert.AreEqual(10, eval.Table);
            Assert.AreEqual(3, eval.Row);
            Assert.AreEqual(2, eval.Language);
            Assert.AreEqual(1, eval.Column);
            Assert.IsNotNull(converted.ValueProperty.Animation);
            Assert.AreEqual(TimeSpan.FromSeconds(10), converted.ValueProperty.Animation.Duration);
            Assert.AreEqual(PropertyChangeAnimationType.Linear, converted.ValueProperty.Animation.Type);
        }

        /// <summary>
        /// Test the export of an ImageList with a valid formula assigned to the values property.
        /// </summary>
        [TestMethod]
        public void ExportImageListTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var generic = new GenericEvalDataViewModel(mediaShellMock.Object)
                              {
                                  Table = { Value = 10 },
                                  Column = { Value = 1 },
                                  Language = { Value = 2 },
                                  Row = { Value = 3 }
                              };
            var imageList = new ImageListElementDataViewModel(mediaShellMock.Object)
                                {
                                    Align = { Value = HorizontalAlignment.Right },
                                    Delimiter = { Value = "Delimiter" },
                                    FilePatterns = { Value = ".jpg" },
                                    Direction = { Value = TextDirection.RTL },
                                    Overflow = { Value = TextOverflow.Scroll },
                                    HorizontalImageGap = { Value = 5 },
                                    VerticalImageGap = { Value = 10 },
                                    FallbackImage = { Value = "FallBack" },
                                    ImageHeight = { Value = 20 },
                                    ImageWidth = { Value = 30 },
                                    Values = { Formula = generic }
                                };
            var exported = (ImageListElement)imageList.Export();
            Assert.AreEqual(HorizontalAlignment.Right, exported.Align);
            Assert.AreEqual("Delimiter", exported.Delimiter);
            Assert.AreEqual(".jpg", exported.FilePatterns);
            Assert.AreEqual(TextDirection.RTL, exported.Direction);
            Assert.AreEqual(TextOverflow.Scroll, exported.Overflow);
            Assert.AreEqual(5, exported.HorizontalImageGap);
            Assert.AreEqual(10, exported.VerticalImageGap);
            Assert.AreEqual("FallBack", exported.FallbackImage);
            Assert.AreEqual(20, exported.ImageHeight);
            Assert.AreEqual(30, exported.ImageWidth);
            var exportedGeneric = exported.ValuesProperty.Evaluation as GenericEval;
            Assert.IsNotNull(exportedGeneric);
            Assert.AreEqual(10, exportedGeneric.Table);
            Assert.AreEqual(1, exportedGeneric.Column);
            Assert.AreEqual(2, exportedGeneric.Language);
            Assert.AreEqual(3, exportedGeneric.Row);
        }

        /// <summary>
        /// Tests the export of an empty audio layout and deserializes it again.
        /// </summary>
        [TestMethod]
        public void ExportEmptyAudioLayoutAndDeserializeExportedTest()
        {
            var resolution = new ResolutionConfigDataViewModel(null) { Width = { Value = 0 }, Height = { Value = 0 } };
            resolution.Elements.Add(new AudioOutputElementDataViewModel(null));
            var audioLayout = new LayoutConfigDataViewModel(null)
                                  {
                                      Resolutions = { resolution },
                                      Name = { Value = "Audio" }
                                  };
            var exported = audioLayout.Export();
             var serializer = new XmlSerializer(typeof(LayoutConfig));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, exported);
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    using (var xmlReader = XmlReader.Create(streamReader))
                    {
                        var result = (LayoutConfig)serializer.Deserialize(xmlReader);
                        Assert.AreEqual(1, result.Resolutions.Count);
                        var resolutionResult = result.Resolutions.First();
                        Assert.AreEqual(1, resolutionResult.Elements.Count);
                    }
                }
            }
        }

        /// <summary>
        /// Tests the export of an audio layout including all audio elements and deserializes it again.
        /// </summary>
        [TestMethod]
        public void ExportAudioLayoutAndDeserializeExportedWithElementsTest()
        {
            var mediaShellMock = this.InitializeApplicationController();

            var resolution = new ResolutionConfigDataViewModel(null) { Width = { Value = 0 }, Height = { Value = 0 } };
            resolution.Elements.Add(new AudioOutputElementDataViewModel(null));
            var audioLayout = new LayoutConfigDataViewModel(null)
            {
                Resolutions = { resolution },
                Name = { Value = "Audio" }
            };

            // add elements
            var audioFileElement = new AudioFileElementDataViewModel(mediaShellMock.Object)
                                   {
                                       Filename = new DynamicDataValue<string>("Test.mp3")
                                   };

            var audioPauseElement = new AudioPauseElementDataViewModel(mediaShellMock.Object)
                                    {
                                        Duration = new DataValue<TimeSpan>(new TimeSpan(1, 2, 3, 4))
                                    };

            var audioTextToSpeechElement = new TextToSpeechElementDataViewModel(mediaShellMock.Object)
                                           {
                                               Value = new DynamicDataValue<string>("The text to read.")
                                           };
            var generic = new GenericEvalDataViewModel(mediaShellMock.Object)
                              {
                                  Table = { Value = 10 },
                                  Column = { Value = 2 },
                                  Row = { Value = 0 },
                                  Language = { Value = 0 }
                              };
            var dynamicValue = new DynamicDataValue<string> { Formula = generic };
            var audioDynamicTtsElement = new DynamicTtsElementDataViewModel(mediaShellMock.Object)
                                         {
                                             Value = dynamicValue
                                         };

            var audioOutputElement = (AudioOutputElementDataViewModel)audioLayout.Resolutions[0].Elements[0];
            audioOutputElement.Elements.Add(audioFileElement);
            audioOutputElement.Elements.Add(audioPauseElement);
            audioOutputElement.Elements.Add(audioTextToSpeechElement);
            audioOutputElement.Elements.Add(audioDynamicTtsElement);

            var exported = audioLayout.Export();
            var serializer = new XmlSerializer(typeof(LayoutConfig));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, exported);
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    using (var xmlReader = XmlReader.Create(streamReader))
                    {
                        var result = (LayoutConfig)serializer.Deserialize(xmlReader);
                        Assert.AreEqual(1, result.Resolutions.Count);
                        var resolutionResult = result.Resolutions.First();
                        Assert.AreEqual(1, resolutionResult.Elements.Count);

                        // verify elements
                        var audioOutputElements = ((AudioOutputElementDataViewModel)resolution.Elements[0]).Elements;
                        VerifyAudioElements(audioOutputElements);
                    }
                }
            }
        }

        /// <summary>
        /// Tests the exported fonts of a configuration that contains more resources than should be exported.
        /// </summary>
        [TestMethod]
        public void ExportSomeFontsTest()
        {
            var mediaShellMock = Helpers.CreateMediaShell();
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            mediaShellMock.Setup(shell => shell.MediaApplicationState).Returns(state);
            var font1 = new ResourceInfoDataViewModel
                            {
                                Facename = "TestFont1",
                                Filename = @"c:\TestFont1.cux",
                                Type = ResourceType.Font
                            };
            var font3 = new ResourceInfoDataViewModel
                            {
                                Facename = "TestFont3",
                                Filename = @"c:\TestFont3.cux",
                                Type = ResourceType.Font
                            };
            var image = new ResourceInfoDataViewModel
                             {
                                 Type = ResourceType.Image,
                                 Filename = @"c:\TestImage.png"
                             };
            var resources = new ExtendedObservableCollection<ResourceInfoDataViewModel> { font1, image, font3 };
            var infomediaConfig = Helpers.CreateDefaultInfomediaConfig(mediaShellMock.Object);
            var layout = infomediaConfig.Layouts.First();
            var textElement = new StaticTextElementDataViewModel(mediaShellMock.Object);
            layout.Resolutions.First().Elements.Add(textElement);

            infomediaConfig.Cycles.StandardCycles.First().Sections.First().Layout = layout;
            layout.CycleSectionReferences.Add(
                new LayoutCycleSectionRefDataViewModel(
                    infomediaConfig.Cycles.StandardCycles.First(),
                    infomediaConfig.Cycles.StandardCycles.First().Sections.First()));
            state.CurrentProject.Resources = resources;
            state.CurrentProject.InfomediaConfig = infomediaConfig;
            font1.SetReference(textElement);
            font3.SetReference(textElement);
            image.SetReference(textElement);
            infomediaConfig.Fonts.Add(
                new FontConfigDataViewModel(mediaShellMock.Object) { Path = { Value = @"Fonts\TestFont1.cux" } });
            infomediaConfig.Fonts.Add(
                new FontConfigDataViewModel(mediaShellMock.Object) { Path = { Value = @"Fonts\TestFont2.cux" } });
            infomediaConfig.Fonts.Add(
                new FontConfigDataViewModel(mediaShellMock.Object) { Path = { Value = @"Fonts\TestFont3.cux" } });
            font1.UpdateIsUsedVisible();
            font3.UpdateIsUsedVisible();
            image.UpdateIsUsedVisible();

            Assert.AreEqual(3, infomediaConfig.Fonts.Count);
            var config = infomediaConfig.Export();
            Assert.AreEqual(2, config.Fonts.Count);
            var fonts = config.Fonts;
            Assert.IsTrue(fonts.Any(f => f.Path.Contains("TestFont1.cux")));
            Assert.IsTrue(fonts.Any(f => f.Path.Contains("TestFont3.cux")));
        }

        /// <summary>
        /// Tests the conversion from DataViewModel to DataModel.
        /// </summary>
        [TestMethod]
        public void ConvertToDataModelTest()
        {
            var config = Helpers.CreateDefaultInfomediaConfig();
            var converted = config.ToDataModel();

            Assert.AreEqual(UtcNow, converted.CreationDate);
            Assert.AreEqual(2, converted.Version.Major);

            Assert.AreEqual(1, converted.PhysicalScreens.Count);
            var tftScreen = converted.PhysicalScreens.SingleOrDefault(screenConfig => screenConfig.Name == "TftScreen");
            Assert.IsNotNull(tftScreen);
            Assert.AreEqual(PhysicalScreenType.TFT, tftScreen.Type);

            Assert.AreEqual(1, converted.VirtualDisplays.Count);
            var tftDisplay =
                converted.VirtualDisplays.SingleOrDefault(displayConfig => displayConfig.Name == "TftDisplay");
            Assert.IsNotNull(tftDisplay);
            Assert.AreEqual("TftPackage", tftDisplay.CyclePackage);

            Assert.AreEqual(1, converted.Cycles.StandardCycles.Count);
            Assert.AreEqual(1, converted.MasterPresentation.MasterCycles.Count);
            var masterCycle = converted.MasterPresentation.MasterCycles.SingleOrDefault(c => c.Name == "Master cycle");
            Assert.IsNotNull(masterCycle);

            var masterSectionFound =
                masterCycle.Sections.SingleOrDefault(@base => @base.Layout == "StandardMasterLayout");
            Assert.IsNotNull(masterSectionFound);

            var masterLayout =
                converted.MasterPresentation.MasterLayouts.SingleOrDefault(
                    model => model.Name == "StandardMasterLayout");
            Assert.IsNotNull(masterLayout);

            var physicalScreenRefFound =
                masterLayout.PhysicalScreens.SingleOrDefault(refConfig => refConfig.Reference == "TftScreen");
            Assert.IsNotNull(physicalScreenRefFound);

            var virtualDisplayRefFound =
                physicalScreenRefFound.VirtualDisplays
                .SingleOrDefault(refConfig => refConfig.Reference == "TftDisplay");
            Assert.IsNotNull(virtualDisplayRefFound);
            Assert.AreEqual(0, virtualDisplayRefFound.X);
            Assert.AreEqual(0, virtualDisplayRefFound.Y);

            var testCycleFound =
                converted.Cycles.StandardCycles.SingleOrDefault(cycleConfig => cycleConfig.Name == "Test cycle");
            Assert.IsNotNull(testCycleFound);
            Assert.IsTrue(testCycleFound.Enabled);

            var testCycleLayoutFound =
                converted.Layouts.SingleOrDefault(layoutConfig => layoutConfig.Name == "Test cycle");
            Assert.IsNotNull(testCycleLayoutFound);

            var resolutionConfigFound =
                testCycleLayoutFound.Resolutions.SingleOrDefault(c => c.Height == 768 && c.Width == 1368);
            Assert.IsNotNull(resolutionConfigFound);

            Assert.AreEqual(1, resolutionConfigFound.Elements.Count);
            var staticTextElement = resolutionConfigFound.Elements.OfType<TextElementDataModel>().SingleOrDefault();
            Assert.IsNotNull(staticTextElement);
            Assert.AreEqual("Static text", staticTextElement.Value);
        }

        /// <summary>
        /// Tests the export of an element with a nested code conversion formula for an update group
        /// which doesn't support that feature.
        /// </summary>
        [TestMethod]
        public void ExportForIncompatibleNestedCodeConversionTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object)
            {
                Value = { Value = "FallBackValue" }
            };

            var evaluation = new IfEvalDataViewModel(mediaShellMock.Object)
            {
                Condition = new ConstantEvalDataViewModel(mediaShellMock.Object)
                {
                    Value = { Value = "Test" }
                },
                Then = new CodeConversionEvalDataViewModel(mediaShellMock.Object)
                           {
                               UseImage = { Value = true }
                           }
            };

            textViewModel.Value.Formula = evaluation;

            var exportParams = new ExportCompatibilityParameters { CsvMappingCompatibilityRequired = true };

            var converted = (TextElement)textViewModel.Export(exportParams);
            Assert.IsNotNull(converted);
            Assert.IsNotNull(converted.ValueProperty);
            var formula = converted.ValueProperty.Evaluation as IfEval;
            Assert.IsNotNull(formula);
            Assert.IsNotNull(formula.Then);
            VerifyConvertedCodeConversion(formula.Then.Evaluation, true);
        }

        /// <summary>
        /// Tests the export of an element with a nested code conversion formula for an update group
        /// which doesn't support that feature.
        /// </summary>
        [TestMethod]
        public void ExportIncompatibleNestedCodeConversionInSwitchTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object)
            {
                Value = { Value = "FallBackValue" }
            };

            var evaluation = new SwitchEvalDataViewModel(mediaShellMock.Object)
            {
                Default = new CodeConversionEvalDataViewModel(mediaShellMock.Object)
                {
                    UseImage = { Value = true }
                },
                Value = new GenericEvalDataViewModel(mediaShellMock.Object)
                            {
                                Column = { Value = 0 },
                                Row = { Value = 0 },
                                Table = { Value = 10 },
                                Language = { Value = 0 }
                            },
            };
            var case1 = new CaseDynamicPropertyDataViewModel(mediaShellMock.Object)
                            {
                                Evaluation = new CodeConversionEvalDataViewModel(mediaShellMock.Object),
                                Value = { Value = "CaseValue" }
                            };
            evaluation.Cases.Add(case1);
            var case2 = new CaseDynamicPropertyDataViewModel(mediaShellMock.Object)
            {
                Evaluation = new CodeConversionEvalDataViewModel(mediaShellMock.Object)
                                 {
                                     UseImage = { Value = true }
                                 },
                Value = { Value = "CaseValue2" }
            };
            evaluation.Cases.Add(case2);
            textViewModel.Value.Formula = evaluation;

            var exportParams = new ExportCompatibilityParameters { CsvMappingCompatibilityRequired = true };

            var converted = (TextElement)textViewModel.Export(exportParams);
            Assert.IsNotNull(converted);
            Assert.IsNotNull(converted.ValueProperty);
            var formula = converted.ValueProperty.Evaluation as SwitchEval;
            Assert.IsNotNull(formula);
            Assert.IsNotNull(formula.Value);
            Assert.IsNotNull(formula.Default);
            Assert.IsNotNull(formula.Cases);
            VerifyConvertedCodeConversion(formula.Default.Evaluation, true);
            var convertedCase1 = formula.Cases.First(c => c.Value == "CaseValue");
            VerifyConvertedCodeConversion(convertedCase1.Evaluation, false);
            var convertedCase2 = formula.Cases.First(c => c.Value == "CaseValue2");
            VerifyConvertedCodeConversion(convertedCase2.Evaluation, true);
        }

        /// <summary>
        /// Tests the export of an element with a nested code conversion formula for an update group
        /// which doesn't support that feature.
        /// </summary>
        [TestMethod]
        public void ExportIncompatibleNestedCodeConversionInCsvMappingTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object)
            {
                Value = { Value = "FallBackValue" }
            };

            var evaluation = new CsvMappingEvalDataViewModel(mediaShellMock.Object)
            {
                DefaultValue = new CodeConversionEvalDataViewModel(mediaShellMock.Object)
                {
                    UseImage = { Value = true }
                },
                FileName = { Value = "Mapping" },
                OutputFormat = { Value = "{3}" }
            };
            var match1 = new MatchDynamicPropertyDataViewModel(mediaShellMock.Object)
            {
                Evaluation = new CodeConversionEvalDataViewModel(mediaShellMock.Object),
                Column = { Value = 0 }
            };
            evaluation.Matches.Add(match1);
            var match2 = new MatchDynamicPropertyDataViewModel(mediaShellMock.Object)
            {
                Evaluation = new CodeConversionEvalDataViewModel(mediaShellMock.Object)
                                 {
                                     UseImage = { Value = true }
                                 },
                Column = { Value = 1 }
            };
            evaluation.Matches.Add(match2);
            textViewModel.Value.Formula = evaluation;

            var exportParams = new ExportCompatibilityParameters { CsvMappingCompatibilityRequired = true };

            var converted = (TextElement)textViewModel.Export(exportParams);
            Assert.IsNotNull(converted);
            Assert.IsNotNull(converted.ValueProperty);
            var formula = converted.ValueProperty.Evaluation as CsvMappingEval;
            Assert.IsNotNull(formula);
            Assert.IsNotNull(formula.DefaultValue);
            Assert.IsNotNull(formula.Matches);
            VerifyConvertedCodeConversion(formula.DefaultValue.Evaluation, true);
            var convertedCase1 = formula.Matches.First(c => c.Column == 0);
            VerifyConvertedCodeConversion(convertedCase1.Evaluation, false);
            var convertedCase2 = formula.Matches.First(c => c.Column == 1);
            VerifyConvertedCodeConversion(convertedCase2.Evaluation, true);
        }

        /// <summary>
        /// Tests the export of an element with a nested code conversion formula for an update group which
        /// supports that feature.
        /// </summary>
        [TestMethod]
        public void ExportForCompatibleNestedCodeConversionTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object)
            {
                Value = { Value = "FallBackValue" }
            };

            var evaluation = new IfEvalDataViewModel(mediaShellMock.Object)
            {
                Condition = new ConstantEvalDataViewModel(mediaShellMock.Object)
                {
                    Value = { Value = "Test" }
                },
                Then = new CodeConversionEvalDataViewModel(mediaShellMock.Object)
                {
                    UseImage = { Value = true }
                }
            };

            textViewModel.Value.Formula = evaluation;

            var exportParams = new ExportCompatibilityParameters { CsvMappingCompatibilityRequired = false };

            var converted = (TextElement)textViewModel.Export(exportParams);
            Assert.IsNotNull(converted);
            Assert.IsNotNull(converted.ValueProperty);
            var formula = converted.ValueProperty.Evaluation as IfEval;
            Assert.IsNotNull(formula);
            Assert.IsNotNull(formula.Then);
            var codeConversion = formula.Then.Evaluation as CodeConversionEval;
            Assert.IsNotNull(codeConversion, "CodeConversion formula should not be converted to CsvMapping");
        }

        /// <summary>
        /// Tests the export of an element with a code conversion formula for an update group which supports that
        /// feature.
        /// </summary>
        [TestMethod]
        public void ExportIncompatibleCodeConversionTest()
        {
            var mediaShellMock = this.InitializeApplicationController();
            var textViewModel = new TextElementDataViewModel(mediaShellMock.Object)
            {
                Value = { Value = "FallBackValue" }
            };

            var evaluation = new CodeConversionEvalDataViewModel(mediaShellMock.Object)
                {
                    UseImage = { Value = true }
                };

            textViewModel.Value.Formula = evaluation;

            var exportParams = new ExportCompatibilityParameters { CsvMappingCompatibilityRequired = true };

            var converted = (TextElement)textViewModel.Export(exportParams);
            Assert.IsNotNull(converted);
            Assert.IsNotNull(converted.ValueProperty);
            VerifyConvertedCodeConversion(converted.ValueProperty.Evaluation, true);
        }

        private static void ResetServiceLocator()
        {
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }

        private static void VerifyAudioElements(
          ExtendedObservableCollection<PlaybackElementDataViewModelBase> audioOutputElements)
        {
            Assert.AreEqual(4, audioOutputElements.Count);

            var audioFileElementDataViewModel = audioOutputElements[0] as AudioFileElementDataViewModel;
            Assert.IsNotNull(audioFileElementDataViewModel);
            Assert.AreEqual(audioFileElementDataViewModel.Filename.Value, "Test.mp3");

            var audioPauseElementDataViewModel = audioOutputElements[1] as AudioPauseElementDataViewModel;
            Assert.IsNotNull(audioPauseElementDataViewModel);
            Assert.AreEqual(audioPauseElementDataViewModel.Duration.Value, new TimeSpan(1, 2, 3, 4));

            var textToSpeechElementDataViewModel = audioOutputElements[2] as TextToSpeechElementDataViewModel;
            Assert.IsNotNull(textToSpeechElementDataViewModel);
            Assert.AreEqual(textToSpeechElementDataViewModel.Value.Value, "The text to read.");

            var dynamicTtsElementDataViewModel = audioOutputElements[3] as DynamicTtsElementDataViewModel;
            Assert.IsNotNull(dynamicTtsElementDataViewModel);
            Assert.AreEqual(string.Empty, dynamicTtsElementDataViewModel.Value.Value);
            Assert.IsNotNull(dynamicTtsElementDataViewModel.Value.Formula);
            var genericResult = dynamicTtsElementDataViewModel.Value.Formula as GenericEvalDataViewModel;
            Assert.IsNotNull(genericResult);
            Assert.AreEqual(10, genericResult.Table.Value);
            Assert.AreEqual(2, genericResult.Column.Value);
            Assert.AreEqual(0, genericResult.Language.Value);
            Assert.AreEqual(0, genericResult.Row.Value);
        }

        private static void VerifyConvertedCodeConversion(EvalBase eval, bool useImage)
        {
            Assert.IsNotNull(eval, "Evaluation can't be null");
            var csvMappingEval = eval as CsvMappingEval;
            Assert.IsNotNull(csvMappingEval, "Evaluation must be a CsvMappingEval");
            Assert.AreEqual(useImage ? "{2}" : "{3}", csvMappingEval.OutputFormat, "UseImage not right.");
            Assert.AreEqual("codeconversion.csv", csvMappingEval.FileName, "Filename must be codeconversion.csv");
            Assert.AreEqual(2, csvMappingEval.Matches.Count, "The formula must have 2 matches");

            var generic = csvMappingEval.Matches.First(m => m.Column == 0).Evaluation as GenericEval;
            Assert.IsNotNull(generic, "Match for column 0 must be a generic eval");
            Assert.AreEqual(1, generic.Column, "Generic column of 'match 0' must be 1");
            Assert.AreEqual(10, generic.Table, "Generic table of 'match 0' must be 10");
            Assert.AreEqual(0, generic.Row, "Generic row of 'match 0' must be 0");
            Assert.AreEqual(0, generic.Language, "Generic language of 'match 0' must be 0");

            generic = csvMappingEval.Matches.First(m => m.Column == 1).Evaluation as GenericEval;
            Assert.IsNotNull(generic, "Match for column 1 must be a generic eval");
            Assert.AreEqual(0, generic.Column, "Generic column of 'match 1' must be 0");
            Assert.AreEqual(10, generic.Table, "Generic table of 'match 1' must be 10");
            Assert.AreEqual(0, generic.Row, "Generic row of 'match 1' must be 0");
            Assert.AreEqual(0, generic.Language, "Generic language of 'match 1' must be 0");
        }

        private Mock<IMediaShell> InitializeApplicationController()
        {
            var commandRegistyMock = new Mock<ICommandRegistry>();
            var dict = new Dictionary();
            dict.Languages.Add(new Language { Index = 2, Name = "Default" });
            dict.Languages.Add(new Language { Index = 0, Name = "English" });
            dict.Version = new Version(2, 0);
            var columns = new List<Column>
                              {
                                  new Column { Index = 1, Name = "Column1" },
                                  new Column { Index = 0, Name = "Column0" }
                              };
            dict.Tables.Add(new Table { Index = 10, Name = "Table10", Columns = columns });
            var dictionary = new DictionaryDataViewModel(dict);
            var mediaShellMock = new Mock<IMediaShell>();
            var mediaShellControllerMock = new Mock<IMediaShellController>();
            mediaShellMock.Setup(shell => shell.Dictionary).Returns(dictionary);
            mediaShellMock.Setup(shell => shell.MediaApplicationState.CurrentProject.Resources)
                .Returns(new ExtendedObservableCollection<ResourceInfoDataViewModel>());
            var applicationControllerMock = new Mock<IMediaApplicationController>();
            applicationControllerMock.Setup(controller => controller.ShellController.FormulaController)
                                     .Returns(
                                         new FormulaController(
                                             mediaShellControllerMock.Object,
                                             mediaShellMock.Object,
                                             commandRegistyMock.Object));
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(applicationControllerMock.Object);
            return mediaShellMock;
        }
    }
}