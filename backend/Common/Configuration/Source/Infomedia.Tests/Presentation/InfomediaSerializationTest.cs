// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfomediaSerializationTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfomediaSerializationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Tests.Presentation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test to verify that <see cref="InfomediaConfig"/> is serializable.
    /// </summary>
    [TestClass]
    public class InfomediaSerializationTest
    {
        private const string ConfigDirectory = @"Presentation\";
        private const string Infomedia1 = @"infomedia1.im2";

        /// <summary>
        /// A simple test for de-serialization.
        /// </summary>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        [TestMethod]
        [DeploymentItem(ConfigDirectory + Infomedia1)]
        public void TestSimpleDeserialization()
        {
            var configMgr = new ConfigManager<InfomediaConfig> { FileName = Infomedia1 };

            var config = configMgr.Config;
            Assert.IsNotNull(config);

            Assert.IsNotNull(config.Version);
            Assert.AreEqual(new Version(2, 0), config.Version);
            Assert.AreEqual(new DateTime(2013, 4, 15, 15, 42, 18), config.CreationDate);

            Assert.IsNotNull(config.PhysicalScreens);
            Assert.AreEqual(3, config.PhysicalScreens.Count);

            var screen = config.PhysicalScreens[0];
            Assert.AreEqual("TftFront", screen.Name);
            Assert.AreEqual(PhysicalScreenType.TFT, screen.Type);
            Assert.AreEqual("0", screen.Identifier);
            Assert.AreEqual(1440, screen.Width);
            Assert.AreEqual(900, screen.Height);

            screen = config.PhysicalScreens[1];
            Assert.AreEqual("TftBack", screen.Name);
            Assert.AreEqual(PhysicalScreenType.TFT, screen.Type);
            Assert.AreEqual("1", screen.Identifier);
            Assert.AreEqual(1440, screen.Width);
            Assert.AreEqual(900, screen.Height);

            screen = config.PhysicalScreens[2];
            Assert.AreEqual("TftSide", screen.Name);
            Assert.AreEqual(PhysicalScreenType.TFT, screen.Type);
            Assert.AreEqual("2", screen.Identifier);
            Assert.AreEqual(1920, screen.Width);
            Assert.AreEqual(540, screen.Height);

            Assert.IsNotNull(config.VirtualDisplays);
            Assert.AreEqual(2, config.VirtualDisplays.Count);

            var display = config.VirtualDisplays[0];
            Assert.AreEqual("TftStandard", display.Name);
            Assert.AreEqual("TftPackage", display.CyclePackage);
            Assert.AreEqual(1440, display.Width);
            Assert.AreEqual(900, display.Height);

            display = config.VirtualDisplays[1];
            Assert.AreEqual("TftWidescreen", display.Name);
            Assert.AreEqual("TftPackage", display.CyclePackage);
            Assert.AreEqual(1920, display.Width);
            Assert.AreEqual(540, display.Height);

            var master = config.MasterPresentation;
            Assert.IsNotNull(master);
            Assert.IsNotNull(master.MasterCycles);
            Assert.AreEqual(1, master.MasterCycles.Count);

            var masterCycle = master.MasterCycles[0];
            Assert.AreEqual("MainCycle", masterCycle.Name);
            Assert.IsNull(masterCycle.EnabledProperty);
            Assert.IsNotNull(masterCycle.Sections);
            Assert.AreEqual(1, masterCycle.Sections.Count);
            var masterSection = masterCycle.Sections[0] as MasterSectionConfig;
            Assert.IsNotNull(masterSection);
            Assert.AreEqual(TimeSpan.FromSeconds(10), masterSection.Duration);
            Assert.AreEqual("StandardMasterLayout", masterSection.Layout);
            Assert.IsTrue(masterSection.Enabled);
            Assert.IsNull(masterSection.EnabledProperty);

            Assert.IsNotNull(master.MasterEventCycles);
            Assert.AreEqual(0, master.MasterEventCycles.Count);

            Assert.IsNotNull(master.MasterLayouts);
            Assert.AreEqual(1, master.MasterLayouts.Count);

            var masterLayout = master.MasterLayouts[0];
            Assert.AreEqual("StandardMasterLayout", masterLayout.Name);
            Assert.AreEqual(3, masterLayout.PhysicalScreens.Count);

            var physicalScreenRef = masterLayout.PhysicalScreens[0];
            Assert.AreEqual("TftFront", physicalScreenRef.Reference);
            Assert.IsNotNull(physicalScreenRef.VirtualDisplays);
            Assert.AreEqual(1, physicalScreenRef.VirtualDisplays.Count);
            var virtualDisplayRef = physicalScreenRef.VirtualDisplays[0];
            Assert.AreEqual("TftStandard", virtualDisplayRef.Reference);
            Assert.AreEqual(0, virtualDisplayRef.X);
            Assert.AreEqual(0, virtualDisplayRef.Y);
            Assert.AreEqual(-1, virtualDisplayRef.Width);
            Assert.AreEqual(-1, virtualDisplayRef.Height);
            Assert.AreEqual(0, virtualDisplayRef.ZIndex);
            Assert.IsNotNull(virtualDisplayRef.VisibleProperty);
            var intCompare = virtualDisplayRef.VisibleProperty.Evaluation as IntegerCompareEval;
            Assert.IsNotNull(intCompare);
            Assert.AreEqual(1, intCompare.Begin);
            Assert.AreEqual(1, intCompare.End);
            var generic = intCompare.Evaluation as GenericEval;
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(0, generic.Table);
            Assert.AreEqual(2, generic.Column);
            Assert.AreEqual(0, generic.Row);

            physicalScreenRef = masterLayout.PhysicalScreens[1];
            Assert.AreEqual("TftBack", physicalScreenRef.Reference);
            Assert.IsNotNull(physicalScreenRef.VirtualDisplays);
            Assert.AreEqual(1, physicalScreenRef.VirtualDisplays.Count);
            virtualDisplayRef = physicalScreenRef.VirtualDisplays[0];
            Assert.AreEqual("TftStandard", virtualDisplayRef.Reference);
            Assert.AreEqual(0, virtualDisplayRef.X);
            Assert.AreEqual(0, virtualDisplayRef.Y);
            Assert.AreEqual(-1, virtualDisplayRef.Width);
            Assert.AreEqual(-1, virtualDisplayRef.Height);
            Assert.AreEqual(0, virtualDisplayRef.ZIndex);
            Assert.IsNull(virtualDisplayRef.VisibleProperty);

            physicalScreenRef = masterLayout.PhysicalScreens[2];
            Assert.AreEqual("TftSide", physicalScreenRef.Reference);
            Assert.IsNotNull(physicalScreenRef.VirtualDisplays);
            Assert.AreEqual(1, physicalScreenRef.VirtualDisplays.Count);
            virtualDisplayRef = physicalScreenRef.VirtualDisplays[0];
            Assert.AreEqual("TftWidescreen", virtualDisplayRef.Reference);
            Assert.AreEqual(0, virtualDisplayRef.X);
            Assert.AreEqual(0, virtualDisplayRef.Y);
            Assert.AreEqual(-1, virtualDisplayRef.Width);
            Assert.AreEqual(-1, virtualDisplayRef.Height);
            Assert.AreEqual(0, virtualDisplayRef.ZIndex);
            Assert.IsNull(virtualDisplayRef.VisibleProperty);

            Assert.IsNotNull(config.CyclePackages);
            Assert.AreEqual(1, config.CyclePackages.Count);

            var cyclePackage = config.CyclePackages[0];
            Assert.AreEqual("TftPackage", cyclePackage.Name);
            Assert.IsNotNull(cyclePackage.StandardCycles);

            Assert.AreEqual(1, cyclePackage.StandardCycles.Count);
            var standardCycleRef = cyclePackage.StandardCycles[0];
            var standardCycle = config.Cycles.StandardCycles.Find(c => c.Name == standardCycleRef.Reference);
            Assert.IsNotNull(standardCycle);
            Assert.AreEqual("PerlschnurCycle", standardCycle.Name);
            Assert.IsTrue(standardCycle.Enabled);
            Assert.IsNull(standardCycle.EnabledProperty);
            Assert.IsNotNull(standardCycle.Sections);
            Assert.AreEqual(2, standardCycle.Sections.Count);

            var standardSection = standardCycle.Sections[0] as StandardSectionConfig;
            Assert.IsNotNull(standardSection);
            Assert.AreEqual(TimeSpan.FromSeconds(10), standardSection.Duration);
            Assert.IsTrue(standardSection.Enabled);
            Assert.IsNull(standardSection.EnabledProperty);
            Assert.AreEqual("PerlschnurLayout", standardSection.Layout);

            var webmediaSection = standardCycle.Sections[1] as WebmediaSectionConfig;
            Assert.IsNotNull(webmediaSection);
            Assert.IsTrue(webmediaSection.Enabled);
            Assert.IsNull(webmediaSection.EnabledProperty);
            Assert.AreEqual("WebLayout", webmediaSection.Layout);

            Assert.AreEqual(1, cyclePackage.EventCycles.Count);
            var eventCycleRef = cyclePackage.EventCycles[0];
            var eventCycle = config.Cycles.EventCycles.Find(c => c.Name == eventCycleRef.Reference);
            Assert.IsNotNull(eventCycle);
            Assert.AreEqual("MessageCycle", eventCycle.Name);
            Assert.IsNotNull(eventCycle.EnabledProperty);
            intCompare = eventCycle.EnabledProperty.Evaluation as IntegerCompareEval;
            Assert.IsNotNull(intCompare);
            Assert.AreEqual(1, intCompare.Begin);
            Assert.AreEqual(2, intCompare.End);
            generic = intCompare.Evaluation as GenericEval;
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(20, generic.Table);
            Assert.AreEqual(2, generic.Column);
            Assert.AreEqual(0, generic.Row);
            Assert.IsNotNull(eventCycle.Trigger);
            Assert.IsNotNull(eventCycle.Trigger.Coordinates);
            Assert.AreEqual(1, eventCycle.Trigger.Coordinates.Count);
            generic = eventCycle.Trigger.Coordinates[0];
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(20, generic.Table);
            Assert.AreEqual(1, generic.Column);
            Assert.AreEqual(0, generic.Row);

            Assert.IsNotNull(eventCycle.Sections);
            Assert.AreEqual(1, eventCycle.Sections.Count);

            standardSection = eventCycle.Sections[0] as StandardSectionConfig;
            Assert.IsNotNull(standardSection);
            Assert.AreEqual(TimeSpan.FromSeconds(20), standardSection.Duration);
            Assert.IsTrue(standardSection.Enabled);
            Assert.IsNull(standardSection.EnabledProperty);
            Assert.AreEqual("MessageLayout", standardSection.Layout);

            Assert.IsNotNull(config.Layouts);
            Assert.AreEqual(3, config.Layouts.Count);

            var layout = config.Layouts[0];
            Assert.AreEqual("PerlschnurLayout", layout.Name);
            Assert.IsNull(layout.BaseLayoutName);
            Assert.IsNotNull(layout.Resolutions);
            Assert.AreEqual(2, layout.Resolutions.Count);
            var resolution = layout.Resolutions[0];
            Assert.AreEqual(1440, resolution.Width);
            Assert.AreEqual(900, resolution.Height);
            Assert.IsNotNull(resolution.Elements);
            Assert.AreEqual(0, resolution.Elements.Count);
            resolution = layout.Resolutions[1];
            Assert.AreEqual(1920, resolution.Width);
            Assert.AreEqual(540, resolution.Height);
            Assert.IsNotNull(resolution.Elements);
            Assert.AreEqual(0, resolution.Elements.Count);

            layout = config.Layouts[1];
            Assert.AreEqual("WebLayout", layout.Name);
            Assert.IsNull(layout.BaseLayoutName);
            Assert.IsNotNull(layout.Resolutions);
            Assert.AreEqual(2, layout.Resolutions.Count);
            resolution = layout.Resolutions[0];
            Assert.AreEqual(1440, resolution.Width);
            Assert.AreEqual(900, resolution.Height);
            Assert.IsNotNull(resolution.Elements);
            Assert.AreEqual(0, resolution.Elements.Count);
            resolution = layout.Resolutions[1];
            Assert.AreEqual(1920, resolution.Width);
            Assert.AreEqual(540, resolution.Height);
            Assert.IsNotNull(resolution.Elements);
            Assert.AreEqual(0, resolution.Elements.Count);

            layout = config.Layouts[2];
            Assert.AreEqual("MessageLayout", layout.Name);
            Assert.IsNull(layout.BaseLayoutName);
            Assert.IsNotNull(layout.Resolutions);
            Assert.AreEqual(2, layout.Resolutions.Count);
            resolution = layout.Resolutions[0];
            Assert.AreEqual(1440, resolution.Width);
            Assert.AreEqual(900, resolution.Height);
            Assert.IsNotNull(resolution.Elements);
            Assert.AreEqual(0, resolution.Elements.Count);
            resolution = layout.Resolutions[1];
            Assert.AreEqual(1920, resolution.Width);
            Assert.AreEqual(540, resolution.Height);
            Assert.IsNotNull(resolution.Elements);
            Assert.AreEqual(0, resolution.Elements.Count);
        }

        /// <summary>
        /// Tests that a dynamic property without evaluation is properly serialized and deserialized.
        /// </summary>
        [TestMethod]
        public void TestEmptyDynamicProperty()
        {
            var input = new AudioOutputElement();
            input.Elements.Add(
                new AudioFileElement
                    {
                        EnabledProperty = new DynamicProperty(),
                        FilenameProperty = new DynamicProperty()
                    });
            input.Elements.Add(new AudioFileElement { EnabledProperty = new DynamicProperty(), Filename = "file.mp3" });

            var output = SerializeDeserialize(input);
            Assert.AreEqual(2, output.Elements.Count);

            var audioFile = output.Elements[0] as AudioFileElement;
            Assert.IsNotNull(audioFile);
            Assert.IsNotNull(audioFile.EnabledProperty);
            Assert.IsNull(audioFile.EnabledProperty.Evaluation);
            Assert.IsNotNull(audioFile.FilenameProperty);
            Assert.IsNull(audioFile.FilenameProperty.Evaluation);
            Assert.IsNull(audioFile.Filename);

            audioFile = output.Elements[1] as AudioFileElement;
            Assert.IsNotNull(audioFile);
            Assert.IsNotNull(audioFile.EnabledProperty);
            Assert.IsNull(audioFile.EnabledProperty.Evaluation);
            Assert.IsNull(audioFile.FilenameProperty);
            Assert.AreEqual("file.mp3", audioFile.Filename);
        }

        /// <summary>
        /// Tests that an animated dynamic property without evaluation is properly serialized and deserialized.
        /// </summary>
        [TestMethod]
        public void TestEmptyAnimatedDynamicProperty()
        {
            var input = new GroupElement();
            input.Elements.Add(
                new VideoElement
                    {
                        VisibleProperty =
                            new AnimatedDynamicProperty
                                {
                                    Animation =
                                        new PropertyChangeAnimation
                                            {
                                                Duration = TimeSpan.FromSeconds(1),
                                                Type = PropertyChangeAnimationType.Linear
                                            }
                                },
                        VideoUriProperty =
                            new AnimatedDynamicProperty
                                {
                                    Animation =
                                        new PropertyChangeAnimation
                                            {
                                                Duration = TimeSpan.FromSeconds(2),
                                                Type = PropertyChangeAnimationType.FadeThroughNothing
                                            }
                                },
                        Scaling = ElementScaling.Fixed
                    });
            input.Elements.Add(new TextElement { Visible = true, Value = "Hello" });

            var output = SerializeDeserialize(input);
            Assert.AreEqual(2, output.Elements.Count);

            var video = output.Elements[0] as VideoElement;
            Assert.IsNotNull(video);
            Assert.IsNotNull(video.VisibleProperty);
            Assert.IsNull(video.VisibleProperty.Evaluation);
            Assert.IsNotNull(video.VisibleProperty.Animation);
            Assert.AreEqual(TimeSpan.FromSeconds(1), video.VisibleProperty.Animation.Duration);
            Assert.AreEqual(PropertyChangeAnimationType.Linear, video.VisibleProperty.Animation.Type);
            Assert.IsNotNull(video.VideoUriProperty);
            Assert.IsNull(video.VideoUriProperty.Evaluation);
            Assert.IsNotNull(video.VideoUriProperty.Animation);
            Assert.AreEqual(TimeSpan.FromSeconds(2), video.VideoUriProperty.Animation.Duration);
            Assert.AreEqual(PropertyChangeAnimationType.FadeThroughNothing, video.VideoUriProperty.Animation.Type);
        }

        /// <summary>
        /// Tests that a collection <see cref="CollectionEvalBase"/> without elements
        /// is properly serialized and deserialized.
        /// </summary>
        [TestMethod]
        public void TestEmptyCollectionEval()
        {
            var input = new OrEval();
            input.Conditions.Add(new AndEval { Conditions = { new GenericEval(0, 0, 3, 0) } });
            input.Conditions.Add(new OrEval());
            input.Conditions.Add(new OrEval { Conditions = { new GenericEval(0, 0, 5, 0) } });

            var output = SerializeDeserialize(input);
            Assert.AreEqual(3, output.Conditions.Count);

            var and = output.Conditions[0] as AndEval;
            Assert.IsNotNull(and);
            Assert.AreEqual(1, and.Conditions.Count);
            var generic = and.Conditions[0] as GenericEval;
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(0, generic.Table);
            Assert.AreEqual(3, generic.Column);
            Assert.AreEqual(0, generic.Row);

            var or = output.Conditions[1] as OrEval;
            Assert.IsNotNull(or);
            Assert.AreEqual(0, or.Conditions.Count);

            or = output.Conditions[2] as OrEval;
            Assert.IsNotNull(or);
            Assert.AreEqual(1, or.Conditions.Count);
            generic = or.Conditions[0] as GenericEval;
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(0, generic.Table);
            Assert.AreEqual(5, generic.Column);
            Assert.AreEqual(0, generic.Row);
        }

        /// <summary>
        /// Tests that a collection <see cref="ResolutionConfig"/> without elements
        /// is properly serialized and deserialized.
        /// </summary>
        [TestMethod]
        public void TestEmptyResolution()
        {
            var input = new LayoutConfig();
            input.Resolutions.Add(
                new ResolutionConfig
                    {
                        Width = 100,
                        Height = 200,
                        Elements =
                            {
                                new TextElement { Visible = true, Value = "Hello" },
                                new TextElement { Visible = true, Value = "World" }
                            }
                    });
            input.Resolutions.Add(new ResolutionConfig { Width = 200, Height = 300 });
            input.Resolutions.Add(
                new ResolutionConfig
                    {
                        Width = 300,
                        Height = 400,
                        Elements =
                            {
                                new TextElement { Visible = true, Value = "Foo" },
                                new TextElement { Visible = true, Value = "Bar" }
                            }
                    });

            var output = SerializeDeserialize(input);
            Assert.AreEqual(3, output.Resolutions.Count);

            var resolution = output.Resolutions[0];
            Assert.AreEqual(100, resolution.Width);
            Assert.AreEqual(200, resolution.Height);
            Assert.AreEqual(2, resolution.Elements.Count);

            var text = resolution.Elements[0] as TextElement;
            Assert.IsNotNull(text);
            Assert.IsTrue(text.Visible);
            Assert.AreEqual("Hello", text.Value);

            text = resolution.Elements[1] as TextElement;
            Assert.IsNotNull(text);
            Assert.IsTrue(text.Visible);
            Assert.AreEqual("World", text.Value);

            resolution = output.Resolutions[1];
            Assert.AreEqual(200, resolution.Width);
            Assert.AreEqual(300, resolution.Height);
            Assert.AreEqual(0, resolution.Elements.Count);

            resolution = output.Resolutions[2];
            Assert.AreEqual(300, resolution.Width);
            Assert.AreEqual(400, resolution.Height);
            Assert.AreEqual(2, resolution.Elements.Count);

            text = resolution.Elements[0] as TextElement;
            Assert.IsNotNull(text);
            Assert.IsTrue(text.Visible);
            Assert.AreEqual("Foo", text.Value);

            text = resolution.Elements[1] as TextElement;
            Assert.IsNotNull(text);
            Assert.IsTrue(text.Visible);
            Assert.AreEqual("Bar", text.Value);
        }

        private static T SerializeDeserialize<T>(T input)
        {
            var memory = new MemoryStream();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(memory, input);

            memory.Seek(0, SeekOrigin.Begin);
            return (T)serializer.Deserialize(memory);
        }
    }
}
