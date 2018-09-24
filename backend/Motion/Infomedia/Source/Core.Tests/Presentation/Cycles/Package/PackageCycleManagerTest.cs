// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCycleManagerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackageCycleManagerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Cycles.Package
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="PackageCycleManager"/>.
    /// </summary>
    [TestClass]
    public class PackageCycleManagerTest
    {
        // ReSharper disable InconsistentNaming
        private const string ConfigFilePath = @"Presentation\Cycles\Package\";
        private const string WebmediaConfigFile = "PackageCycleManagerTest.wm2";

        /// <summary>
        /// Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
        }

        /// <summary>
        /// Test that makes sure if multiple web sections are defined to use the
        /// same file, they will also reuse the same pool.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code.")]
        [DeploymentItem(ConfigFilePath + WebmediaConfigFile)]
        public void TestMultipleWebSections()
        {
            var config = new InfomediaConfig();
            config.CyclePackages = new List<CyclePackageConfig>
                {
                    new CyclePackageConfig
                        {
                            StandardCycles =
                                {
                                    new StandardCycleRefConfig { Reference = "StandardCycle" }
                                }
                        }
                };
            config.Cycles = new CyclesConfig
            {
                StandardCycles =
                    {
                        new StandardCycleConfig
                            {
                                Name = "StandardCycle",
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout",
                                                Duration = TimeSpan.FromSeconds(10)
                                            },
                                        new WebmediaSectionConfig
                                            {
                                                Layout = "WebLayout",
                                                Filename = WebmediaConfigFile
                                            },
                                        new WebmediaSectionConfig
                                            {
                                                Layout = "WebLayout",
                                                Filename = WebmediaConfigFile
                                            },
                                        new WebmediaSectionConfig
                                            {
                                                Layout = "WebLayout",
                                                Filename = WebmediaConfigFile
                                            }
                                    }
                            }
                    }
            };
            config.Layouts = new List<LayoutConfig>
                                 {
                                     new LayoutConfig { Name = "StandardLayout" },
                                     new LayoutConfig { Name = "WebLayout" }
                                 };
            var context = new PresentationContextMock(config);
            var target = new PackageCycleManager(config.CyclePackages[0], context);

            Assert.IsNull(target.CurrentCycle);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            var page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);
            Assert.IsNotNull(page.Layout);
            var elements = new List<ElementBase>(page.Layout.LoadLayoutElements(1440, 900));
            Assert.AreEqual(1, elements.Count);
            var image = elements[0] as ImageElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("_content_324_.jpg", Path.GetFileName(image.Filename));

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);
            Assert.IsNotNull(page.Layout);
            elements = new List<ElementBase>(page.Layout.LoadLayoutElements(1440, 900));
            Assert.AreEqual(1, elements.Count);
            image = elements[0] as ImageElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("_content_325_.jpg", Path.GetFileName(image.Filename));

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.AreEqual(TimeSpan.FromSeconds(25), page.Duration);
            Assert.IsNotNull(page.Layout);
            elements = new List<ElementBase>(page.Layout.LoadLayoutElements(1440, 900));
            Assert.AreEqual(1, elements.Count);
            image = elements[0] as ImageElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("_content_326_.jpg", Path.GetFileName(image.Filename));

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
            Assert.IsNotNull(page.Layout);
            elements = new List<ElementBase>(page.Layout.LoadLayoutElements(1440, 900));
            Assert.AreEqual(1, elements.Count);
            image = elements[0] as ImageElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("_content_327_.jpg", Path.GetFileName(image.Filename));

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.AreEqual(TimeSpan.FromSeconds(17), page.Duration);
            Assert.IsNotNull(page.Layout);
            elements = new List<ElementBase>(page.Layout.LoadLayoutElements(1440, 900));
            Assert.AreEqual(1, elements.Count);
            image = elements[0] as ImageElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("_content_328_.jpg", Path.GetFileName(image.Filename));

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.AreEqual(TimeSpan.FromSeconds(18), page.Duration);
            Assert.IsNotNull(page.Layout);
            elements = new List<ElementBase>(page.Layout.LoadLayoutElements(1440, 900));
            Assert.AreEqual(1, elements.Count);
            var group = elements[0] as GroupElement;
            Assert.IsNotNull(group);
            Assert.AreEqual(2, group.Elements.Count);
            image = group.Elements[0] as ImageElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("_content_329_.png", Path.GetFileName(image.Filename));
            image = group.Elements[1] as ImageElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("_content_330_.png", Path.GetFileName(image.Filename));

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);
            Assert.IsNotNull(page.Layout);
            elements = new List<ElementBase>(page.Layout.LoadLayoutElements(1440, 900));
            Assert.AreEqual(1, elements.Count);
            image = elements[0] as ImageElement;
            Assert.IsNotNull(image);
            Assert.AreEqual("_content_324_.jpg", Path.GetFileName(image.Filename));
        }

        /// <summary>
        /// Test that makes sure if multiple multi sections are defined to use the
        /// same table and language, they will also reuse the same pool.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code.")]
        public void TestMultiplePooledMultiSections()
        {
            var config = new InfomediaConfig();
            config.CyclePackages = new List<CyclePackageConfig>
                {
                    new CyclePackageConfig
                        {
                            StandardCycles =
                                {
                                    new StandardCycleRefConfig { Reference = "StandardCycle" }
                                }
                        }
                };
            config.Cycles = new CyclesConfig
            {
                StandardCycles =
                    {
                        new StandardCycleConfig
                            {
                                Name = "StandardCycle",
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout1",
                                                Duration = TimeSpan.FromSeconds(10)
                                            },
                                        new MultiSectionConfig
                                            {
                                                Layout = "PageLayout1",
                                                Duration = TimeSpan.FromSeconds(15),
                                                Language = 0,
                                                Table = 20,
                                                Mode = PageMode.Pool
                                            },
                                        new MultiSectionConfig
                                            {
                                                Layout = "PageLayout1",
                                                Duration = TimeSpan.FromSeconds(15),
                                                Language = 0,
                                                Table = 20,
                                                Mode = PageMode.Pool
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout2",
                                                Duration = TimeSpan.FromSeconds(20)
                                            },
                                        new MultiSectionConfig
                                            {
                                                Layout = "PageLayout1",
                                                Duration = TimeSpan.FromSeconds(15),
                                                Language = 0,
                                                Table = 20,
                                                Mode = PageMode.Pool
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout2",
                                                Duration = TimeSpan.FromSeconds(20)
                                            },
                                        new MultiSectionConfig
                                            {
                                                Layout = "PageLayout2",
                                                Duration = TimeSpan.FromSeconds(25),
                                                Language = 1,
                                                Table = 20,
                                                Mode = PageMode.Pool
                                            },
                                    }
                            }
                    }
            };
            config.Layouts = new List<LayoutConfig>
            {
                new LayoutConfig { Name = "StandardLayout1" },
                new LayoutConfig { Name = "StandardLayout2" },
                new LayoutConfig { Name = "PageLayout1" },
                new LayoutConfig { Name = "PageLayout2" }
            };
            var context = new PresentationContextMock(config);

            context.SetCellValue(new GenericCoordinate(0, 20, 0, 0), "L0R0");
            context.SetCellValue(new GenericCoordinate(0, 20, 0, 1), "L0R1");
            context.SetCellValue(new GenericCoordinate(0, 20, 0, 2), "L0R2");
            context.SetCellValue(new GenericCoordinate(0, 20, 0, 3), "L0R3");
            context.SetCellValue(new GenericCoordinate(0, 20, 0, 4), "L0R4");

            context.SetCellValue(new GenericCoordinate(1, 20, 0, 0), "L1R0");
            context.SetCellValue(new GenericCoordinate(1, 20, 0, 1), "L1R1");
            context.SetCellValue(new GenericCoordinate(1, 20, 0, 2), "L1R2");

            var target = new PackageCycleManager(config.CyclePackages[0], context);

            Assert.IsNull(target.CurrentCycle);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            var page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(1, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(-1, page.LanguageIndex);
            Assert.AreEqual(-1, page.TableIndex);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(5, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(0, page.LanguageIndex);
            Assert.AreEqual(20, page.TableIndex);
            Assert.AreEqual("PageLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(1, page.PageIndex);
            Assert.AreEqual(5, page.TotalPages);
            Assert.AreEqual(1, page.RowOffset);
            Assert.AreEqual(0, page.LanguageIndex);
            Assert.AreEqual(20, page.TableIndex);
            Assert.AreEqual("PageLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(1, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(-1, page.LanguageIndex);
            Assert.AreEqual(-1, page.TableIndex);
            Assert.AreEqual("StandardLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(2, page.PageIndex);
            Assert.AreEqual(5, page.TotalPages);
            Assert.AreEqual(2, page.RowOffset);
            Assert.AreEqual(0, page.LanguageIndex);
            Assert.AreEqual(20, page.TableIndex);
            Assert.AreEqual("PageLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(1, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(-1, page.LanguageIndex);
            Assert.AreEqual(-1, page.TableIndex);
            Assert.AreEqual("StandardLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(3, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(1, page.LanguageIndex);
            Assert.AreEqual(20, page.TableIndex);
            Assert.AreEqual("PageLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(25), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(1, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(-1, page.LanguageIndex);
            Assert.AreEqual(-1, page.TableIndex);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(3, page.PageIndex);
            Assert.AreEqual(5, page.TotalPages);
            Assert.AreEqual(3, page.RowOffset);
            Assert.AreEqual(0, page.LanguageIndex);
            Assert.AreEqual(20, page.TableIndex);
            Assert.AreEqual("PageLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(4, page.PageIndex);
            Assert.AreEqual(5, page.TotalPages);
            Assert.AreEqual(4, page.RowOffset);
            Assert.AreEqual(0, page.LanguageIndex);
            Assert.AreEqual(20, page.TableIndex);
            Assert.AreEqual("PageLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(1, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(-1, page.LanguageIndex);
            Assert.AreEqual(-1, page.TableIndex);
            Assert.AreEqual("StandardLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(5, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(0, page.LanguageIndex);
            Assert.AreEqual(20, page.TableIndex);
            Assert.AreEqual("PageLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(1, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(-1, page.LanguageIndex);
            Assert.AreEqual(-1, page.TableIndex);
            Assert.AreEqual("StandardLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual(1, page.PageIndex);
            Assert.AreEqual(3, page.TotalPages);
            Assert.AreEqual(1, page.RowOffset);
            Assert.AreEqual(1, page.LanguageIndex);
            Assert.AreEqual(20, page.TableIndex);
            Assert.AreEqual("PageLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(25), page.Duration);

            target.ShowNextPage();
        }

        /// <summary>
        /// Tests the handling of event cycles that have an Enabled dynamic
        /// property (<see cref="EventCycleConfig"/>).
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code.")]
        public void TestEventCycleWithEnabled()
        {
            var config = new InfomediaConfig();
            config.CyclePackages = new List<CyclePackageConfig>
                {
                    new CyclePackageConfig
                        {
                            StandardCycles =
                                {
                                    new StandardCycleRefConfig
                                        {
                                            Reference = "StandardCycle"
                                        }
                                },

                                EventCycles =
                                {
                                    new EventCycleRefConfig
                                        {
                                            Reference = "EventCycle"
                                        }
                                }
                        }
                };
            config.Cycles = new CyclesConfig
            {
                StandardCycles =
                    {
                        new StandardCycleConfig
                            {
                                Name = "StandardCycle",
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout1",
                                                Duration = TimeSpan.FromSeconds(10)
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout2",
                                                Duration = TimeSpan.FromSeconds(15)
                                            }
                                    }
                            }
                    },
                EventCycles =
                    {
                        new EventCycleConfig
                            {
                                Name = "EventCycle",
                                EnabledProperty = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                                Trigger = new GenericTriggerConfig(new GenericEval(0, 1, 2, 3)),
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout1",
                                                Duration = TimeSpan.FromSeconds(20)
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout2",
                                                Duration = TimeSpan.FromSeconds(30)
                                            }
                                    }
                            }
                    }
            };
            config.Layouts = new List<LayoutConfig>
                                 {
                                     new LayoutConfig { Name = "StandardLayout1" },
                                     new LayoutConfig { Name = "StandardLayout2" },
                                     new LayoutConfig { Name = "EventLayout1" },
                                     new LayoutConfig { Name = "EventLayout2" }
                                 };
            var context = new PresentationContextMock(config);
            var target = new PackageCycleManager(config.CyclePackages[0], context);

            Assert.IsNull(target.CurrentCycle);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            var page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "true");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(30), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
        }

        /// <summary>
        /// Tests the handling of event cycles that don't have an Enabled dynamic
        /// property (<see cref="EventCycleConfig"/>).
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code.")]
        public void TestEventCycleWithoutEnabled()
        {
            var config = new InfomediaConfig();
            config.CyclePackages = new List<CyclePackageConfig>
                {
                    new CyclePackageConfig
                        {
                            StandardCycles =
                                {
                                    new StandardCycleRefConfig
                                        {
                                            Reference = "StandardCycle"
                                        }
                                },

                                EventCycles =
                                {
                                    new EventCycleRefConfig
                                        {
                                            Reference = "EventCycle"
                                        }
                                }
                        }
                };
            config.Cycles = new CyclesConfig
            {
                StandardCycles =
                    {
                        new StandardCycleConfig
                            {
                                Name = "StandardCycle",
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout1",
                                                Duration = TimeSpan.FromSeconds(10)
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout2",
                                                Duration = TimeSpan.FromSeconds(15)
                                            }
                                    }
                            }
                    },
                EventCycles =
                    {
                        new EventCycleConfig
                            {
                                Name = "EventCycle",
                                Trigger = new GenericTriggerConfig(new GenericEval(0, 1, 2, 3)),
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout1",
                                                Duration = TimeSpan.FromSeconds(20)
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout2",
                                                Duration = TimeSpan.FromSeconds(30)
                                            }
                                    }
                            }
                    }
            };
            config.Layouts = new List<LayoutConfig>
                                 {
                                     new LayoutConfig { Name = "StandardLayout1" },
                                     new LayoutConfig { Name = "StandardLayout2" },
                                     new LayoutConfig { Name = "EventLayout1" },
                                     new LayoutConfig { Name = "EventLayout2" }
                                 };
            var context = new PresentationContextMock(config);
            var target = new PackageCycleManager(config.CyclePackages[0], context);

            Assert.IsNull(target.CurrentCycle);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            var page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(30), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "2");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);
        }

        /// <summary>
        /// Tests the handling of event cycles that have an Enabled dynamic
        /// property (<see cref="EventCycleConfig"/>).
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code.")]
        public void TestMultiEventCycles()
        {
            var config = new InfomediaConfig();
            config.CyclePackages = new List<CyclePackageConfig>
                {
                    new CyclePackageConfig
                        {
                            StandardCycles =
                                {
                                    new StandardCycleRefConfig { Reference = "StandardCycle" }
                                },

                                EventCycles =
                                {
                                    new EventCycleRefConfig { Reference = "EventCycle1" },
                                    new EventCycleRefConfig { Reference = "EventCycle2" },
                                    new EventCycleRefConfig { Reference = "EventCycle3" }
                                }
                        }
                };
            config.Cycles = new CyclesConfig
            {
                StandardCycles =
                    {
                        new StandardCycleConfig
                            {
                                Name = "StandardCycle",
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout1",
                                                Duration = TimeSpan.FromSeconds(10)
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "StandardLayout2",
                                                Duration = TimeSpan.FromSeconds(15)
                                            }
                                    }
                            }
                    },
                EventCycles =
                    {
                        new EventCycleConfig
                            {
                                Name = "EventCycle1",
                                Trigger = new GenericTriggerConfig(new GenericEval(0, 1, 2, 3)),
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout1.1",
                                                Duration = TimeSpan.FromSeconds(20)
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout1.2",
                                                Duration = TimeSpan.FromSeconds(30)
                                            }
                                    }
                            },
                        new EventCycleConfig
                            {
                                Name = "EventCycle2",
                                Trigger = new GenericTriggerConfig(new GenericEval(1, 2, 3, 4)),
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout2.1",
                                                Duration = TimeSpan.FromSeconds(20)
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout2.2",
                                                Duration = TimeSpan.FromSeconds(30)
                                            },
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout2.3",
                                                Duration = TimeSpan.FromSeconds(30)
                                            }
                                    }
                            },
                        new EventCycleConfig
                            {
                                Name = "EventCycle3",
                                Trigger = new GenericTriggerConfig(new GenericEval(2, 3, 4, 5)),
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "EventLayout3.1",
                                                Duration = TimeSpan.FromSeconds(20)
                                            }
                                    }
                            }
                    }
            };
            config.Layouts = new List<LayoutConfig>
                                 {
                                     new LayoutConfig { Name = "StandardLayout1" },
                                     new LayoutConfig { Name = "StandardLayout2" },
                                     new LayoutConfig { Name = "EventLayout1.1" },
                                     new LayoutConfig { Name = "EventLayout1.2" },
                                     new LayoutConfig { Name = "EventLayout2.1" },
                                     new LayoutConfig { Name = "EventLayout2.2" },
                                     new LayoutConfig { Name = "EventLayout2.3" },
                                     new LayoutConfig { Name = "EventLayout3.1" }
                                 };
            var context = new PresentationContextMock(config);
            var target = new PackageCycleManager(config.CyclePackages[0], context);

            Assert.IsNull(target.CurrentCycle);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            var page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "1");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout2.1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout2.2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(30), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout1.1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout1.2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(30), page.Duration);

            target.ShowNextPage();

            // we show layout 2.2 again because it was previously interrupted
            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout2.2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(30), page.Duration);

            context.SetCellValue(new GenericCoordinate(2, 3, 4, 5), "1");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout2.2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(30), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout2.3", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(30), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout3.1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            // we show layout 1 again because it was previously interrupted
            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "2");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout1.1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(20), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("EventLayout1.2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(30), page.Duration);

            target.ShowNextPage();

            // we show layout 2 again since it was previously interrupted
            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout2", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(15), page.Duration);

            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("StandardLayout1", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
        }

        /// <summary>
        /// Test the switching between standard cycles.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code.")]
        public void TestConditionalCycleSwitching_Bug12124()
        {
            var config = new InfomediaConfig();
            config.CyclePackages = new List<CyclePackageConfig>
                {
                    new CyclePackageConfig
                        {
                            StandardCycles =
                                {
                                    new StandardCycleRefConfig { Reference = "Zero Line" },
                                    new StandardCycleRefConfig { Reference = "Main Cycle" }
                                }
                        }
                };
            config.Cycles = new CyclesConfig
            {
                StandardCycles =
                    {
                        new StandardCycleConfig
                            {
                                Name = "Zero Line",
                                EnabledProperty = new DynamicProperty(
                                    new IntegerCompareEval
                                        {
                                            Begin = 0,
                                            End = 0,
                                            Evaluation = new GenericEval
                                            {
                                                Language = 0,
                                                Table = 10,
                                                Column = 0,
                                                Row = 0
                                            }
                                         }),
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "Zero Line",
                                                Duration = TimeSpan.FromSeconds(10)
                                            }
                                    }
                            },
                        new StandardCycleConfig
                            {
                                Name = "Main Cycle",
                                Sections =
                                    {
                                        new StandardSectionConfig
                                            {
                                                Layout = "Main Cycle",
                                                Duration = TimeSpan.FromSeconds(10),
                                            }
                                    }
                            }
                    }
            };
            config.Layouts = new List<LayoutConfig>
                                 {
                                     new LayoutConfig { Name = "Main Cycle" },
                                     new LayoutConfig { Name = "Zero Line" }
                                 };
            var context = new PresentationContextMock(config);
            var target = new PackageCycleManager(config.CyclePackages[0], context);

            Assert.IsNull(target.CurrentCycle);
            target.ShowNextPage();

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            var page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("Main Cycle", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "123");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("Main Cycle", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "0");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("Zero Line", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "123");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("Main Cycle", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);

            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "0");

            Assert.IsNotNull(target.CurrentCycle);
            Assert.IsNotNull(target.CurrentCycle.CurrentSection);
            page = target.CurrentCycle.CurrentSection.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("Zero Line", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
        }

        // ReSharper restore InconsistentNaming
    }
}
