// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualDisplayComposerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VirtualDisplayComposerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Composer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="VirtualDisplayComposer"/>.
    /// </summary>
    [TestClass]
    public class VirtualDisplayComposerTest
    {
        /// <summary>
        /// Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
        }

        /// <summary>
        /// Tests the standard constructor.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Composer\dummy.txt")]
        public void ConstructorTest()
        {
            var context = CreateContext();
            var element = new VirtualDisplayRefConfig
            {
                Reference = "VD1",
                X = 17,
                Y = 19,
                Width = 1279,
                Height = 723,
                Visible = true
            };

            var target = new VirtualDisplayComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as IncludeItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(19, item.Y);
                Assert.AreEqual(1279, item.Width);
                Assert.AreEqual(723, item.Height);
                Assert.IsTrue(item.Visible);

                Assert.IsNotNull(item.Include);

                Assert.AreEqual(315, item.Include.Width);
                Assert.AreEqual(577, item.Include.Height);

                Assert.IsNotNull(item.Include.Items);
                Assert.AreEqual(2, item.Include.Items.Count);

                var text = item.Include.Items[0] as TextItem;
                Assert.IsNotNull(text);
                Assert.AreEqual(22, text.X);
                Assert.AreEqual(26, text.Y);
                Assert.AreEqual(201, text.Width);
                Assert.AreEqual(21, text.Height);
                Assert.AreEqual(string.Empty, text.Text);

                var image = item.Include.Items[1] as ImageItem;
                Assert.IsNotNull(image);
                Assert.AreEqual(48, image.X);
                Assert.AreEqual(30, image.Y);
                Assert.AreEqual(23, image.Width);
                Assert.AreEqual(17, image.Height);
                Assert.IsTrue(image.Visible);
                Assert.AreEqual("dummy.txt", image.Filename);
            }
        }

        /// <summary>
        /// Tests that inner values are changed properly.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Composer\dummy.txt")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void ValueChangedTest()
        {
            var context = CreateContext();
            var element = new VirtualDisplayRefConfig
            {
                Reference = "VD1",
                X = 17,
                Y = 19,
                Width = 1279,
                Height = 723,
                Visible = true
            };

            var target = new VirtualDisplayComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as IncludeItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(19, item.Y);
                Assert.AreEqual(1279, item.Width);
                Assert.AreEqual(723, item.Height);
                Assert.IsTrue(item.Visible);

                var changes = new List<AnimatedPropertyChangedEventArgs>();
                item.PropertyValueChanged += (sender, args) => changes.Add(args);

                Assert.IsNotNull(item.Include);

                Assert.AreEqual(315, item.Include.Width);
                Assert.AreEqual(577, item.Include.Height);

                Assert.IsNotNull(item.Include.Items);
                Assert.AreEqual(2, item.Include.Items.Count);

                var text = item.Include.Items[0] as TextItem;
                Assert.IsNotNull(text);
                Assert.AreEqual(22, text.X);
                Assert.AreEqual(26, text.Y);
                Assert.AreEqual(201, text.Width);
                Assert.AreEqual(21, text.Height);
                Assert.AreEqual(string.Empty, text.Text);

                var image = item.Include.Items[1] as ImageItem;
                Assert.IsNotNull(image);
                Assert.AreEqual(48, image.X);
                Assert.AreEqual(30, image.Y);
                Assert.AreEqual(23, image.Width);
                Assert.AreEqual(17, image.Height);
                Assert.IsTrue(image.Visible);
                Assert.AreEqual("dummy.txt", image.Filename);

                context.RaiseUpdating(EventArgs.Empty);
                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");
                var updated = new PresentationUpdatedEventArgs();
                context.RaiseUpdated(updated);

                Assert.AreEqual(0, changes.Count);
                Assert.AreEqual(1, updated.Updates.Count);
                Assert.AreEqual(1, updated.Updates[0].Updates.Count);
                var update = updated.Updates[0].Updates[0];
                Assert.AreEqual("Text", update.Property);
                Assert.AreEqual(text.Id, update.ScreenItemId);
                Assert.AreEqual("Hello", update.Value);
                Assert.AreEqual("Hello", text.Text);

                context.RaiseUpdating(EventArgs.Empty);
                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "1");
                updated = new PresentationUpdatedEventArgs();
                context.RaiseUpdated(updated);

                Assert.AreEqual(0, updated.Updates.Count);
                Assert.AreEqual(1, changes.Count);
                var change = changes[0];
                Assert.IsNull(change.Animation);
                Assert.AreEqual("Include", change.PropertyName);

                var root = change.Value as RootItem;
                Assert.IsNotNull(root);

                Assert.AreEqual(item.Include, root);

                Assert.AreEqual(315, item.Include.Width);
                Assert.AreEqual(577, item.Include.Height);

                Assert.IsNotNull(item.Include.Items);
                Assert.AreEqual(1, item.Include.Items.Count);

                text = item.Include.Items[0] as TextItem;
                Assert.IsNotNull(text);
                Assert.AreEqual(22, text.X);
                Assert.AreEqual(26, text.Y);
                Assert.AreEqual(201, text.Width);
                Assert.AreEqual(21, text.Height);
                Assert.AreEqual("Event", text.Text);
            }
        }

        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "config setup for test")]
        private static PresentationContextMock CreateContext()
        {
            return new PresentationContextMock(
                new InfomediaConfig
                    {
                        VirtualDisplays =
                            {
                                new VirtualDisplayConfig
                                    {
                                        Name = "VD1",
                                        Width = 315,
                                        Height = 577,
                                        CyclePackage = "CP1"
                                    }
                            },
                        CyclePackages =
                            {
                                new CyclePackageConfig
                                    {
                                        Name = "CP1",
                                        StandardCycles =
                                            {
                                                new StandardCycleRefConfig { Reference = "SC1" }
                                            },
                                        EventCycles =
                                            {
                                                new EventCycleRefConfig { Reference = "EC1" }
                                            }
                                    }
                            },
                        Cycles = new CyclesConfig
                            {
                                StandardCycles =
                                    {
                                        new StandardCycleConfig
                                            {
                                                Name = "SC1",
                                                Enabled = true,
                                                Sections =
                                                    {
                                                        new StandardSectionConfig
                                                            {
                                                                Duration = TimeSpan.FromSeconds(60),
                                                                Layout = "MainLayout"
                                                            }
                                                    }
                                            }
                                    },
                                EventCycles =
                                    {
                                        new EventCycleConfig
                                            {
                                                Name = "EC1",
                                                Enabled = true,
                                                Trigger = new GenericTriggerConfig(new GenericEval(1, 2, 3, 4)),
                                                Sections =
                                                    {
                                                        new StandardSectionConfig
                                                            {
                                                                Duration = TimeSpan.FromSeconds(60),
                                                                Layout = "EventLayout"
                                                            }
                                                    }
                                            }
                                    }
                            },
                        Layouts =
                            {
                                new LayoutConfig
                                    {
                                        Name = "MainLayout",
                                        Resolutions =
                                            {
                                                new ResolutionConfig
                                                    {
                                                        Width = 315,
                                                        Height = 577,
                                                        Elements =
                                                            {
                                                                new TextElement
                                                                    {
                                                                        X = 5,
                                                                        Y = 7,
                                                                        Width = 201,
                                                                        Height = 21,
                                                                        Font = new Font
                                                                            {
                                                                                Face = "Arial",
                                                                                Color = "Black"
                                                                            },
                                                                        ValueProperty = new AnimatedDynamicProperty
                                                                            {
                                                                                Evaluation = new GenericEval(0, 1, 2, 3)
                                                                            }
                                                                    },
                                                                    new ImageElement
                                                                        {
                                                                            X = 31,
                                                                            Y = 11,
                                                                            Width = 23,
                                                                            Height = 17,
                                                                            Filename = "dummy.txt"
                                                                        }
                                                            }
                                                    }
                                            }
                                    },
                                new LayoutConfig
                                    {
                                        Name = "EventLayout",
                                        Resolutions =
                                            {
                                                new ResolutionConfig
                                                    {
                                                        Width = 315,
                                                        Height = 577,
                                                        Elements =
                                                            {
                                                                new TextElement
                                                                    {
                                                                        X = 5,
                                                                        Y = 7,
                                                                        Width = 201,
                                                                        Height = 21,
                                                                        Font = new Font
                                                                            {
                                                                                Face = "Arial",
                                                                                Color = "Black"
                                                                            },
                                                                        Value = "Event"
                                                                    }
                                                            }
                                                    }
                                            }
                                    }
                            }
                    });
        }
    }
}
