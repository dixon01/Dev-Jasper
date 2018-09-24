// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSectionTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Cycles.Package
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="ImageSection"/>.
    /// </summary>
    [TestClass]
    public class ImageSectionTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
        }

        /// <summary>
        /// A test for Activate showing the image in full screen
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Cycles\Package\dummy.txt")]
        public void Activate_FullScreen_Test()
        {
            var config = new ImageSectionConfig
                             {
                                 Duration = TimeSpan.FromSeconds(10),
                                 Enabled = true,
                                 Layout = "Layout",
                                 Filename = "dummy.txt",
                                 Frame = 0
                             };

            var context = new PresentationContextMock(CreateLayoutConfig());
            var target = new ImageSection(config, context);

            using (target)
            {
                bool actual = target.Activate();

                Assert.IsTrue(actual);

                var page = target.CurrentObject;
                Assert.IsNotNull(page);
                Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
                Assert.AreEqual(0, page.PageIndex);
                Assert.AreEqual(1, page.TotalPages);
                Assert.AreEqual(0, page.RowOffset);

                var layout = page.Layout as FrameLayout;
                Assert.IsNotNull(layout);

                var elements = new List<ElementBase>(layout.LoadLayoutElements(1440, 900));
                Assert.AreEqual(1, elements.Count);

                var image = elements[0] as ImageElement;
                Assert.IsNotNull(image);
                Assert.AreEqual(0, image.X);
                Assert.AreEqual(0, image.Y);
                Assert.AreEqual(1440, image.Width);
                Assert.AreEqual(900, image.Height);
                Assert.AreEqual(ElementScaling.Scale, image.Scaling);
                Assert.AreEqual("dummy.txt", image.Filename);
            }
        }

        /// <summary>
        /// A test for Activate showing the image in a frame
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Cycles\Package\dummy.txt")]
        public void Activate_Frame_Test()
        {
            var config = new ImageSectionConfig
            {
                Duration = TimeSpan.FromSeconds(10),
                Enabled = true,
                Layout = "Layout",
                Filename = "dummy.txt",
                Frame = 1
            };

            var context = new PresentationContextMock(CreateLayoutConfig());
            var target = new ImageSection(config, context);

            using (target)
            {
                bool actual = target.Activate();

                Assert.IsTrue(actual);

                var page = target.CurrentObject;
                Assert.IsNotNull(page);
                Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
                Assert.AreEqual(0, page.PageIndex);
                Assert.AreEqual(1, page.TotalPages);
                Assert.AreEqual(0, page.RowOffset);

                var layout = page.Layout as FrameLayout;
                Assert.IsNotNull(layout);

                var elements = new List<ElementBase>(layout.LoadLayoutElements(1440, 900));
                Assert.AreEqual(1, elements.Count);

                var image = elements[0] as ImageElement;
                Assert.IsNotNull(image);
                Assert.AreEqual(15, image.X);
                Assert.AreEqual(19, image.Y);
                Assert.AreEqual(122, image.Width);
                Assert.AreEqual(315, image.Height);
                Assert.AreEqual(ElementScaling.Scale, image.Scaling);
                Assert.AreEqual("dummy.txt", image.Filename);
            }
        }

        /// <summary>
        /// A test for ShowNextObject
        /// </summary>
        [TestMethod]
        public void ShowNextPageTest()
        {
            var config = new ImageSectionConfig
                             {
                                 Duration = TimeSpan.FromSeconds(10),
                                 Enabled = true,
                                 Layout = "Layout",
                                 Filename = "dummy.txt",
                                 Frame = 0
                             };

            var context = new PresentationContextMock(CreateLayoutConfig());
            var target = new ImageSection(config, context);

            using (target)
            {
                bool actual = target.Activate();
                Assert.IsTrue(actual);

                actual = target.ShowNextObject();
                Assert.IsFalse(actual);
            }
        }

        private static LayoutConfig CreateLayoutConfig()
        {
            return new LayoutConfig
            {
                Name = "Layout",
                Resolutions =
                {
                    new ResolutionConfig
                        {
                            Width = 1440,
                            Height = 900,
                            Elements =
                                {
                                    new FrameElement
                                        {
                                            X = 15,
                                            Y = 19,
                                            Width = 122,
                                            Height = 315,
                                            FrameId = 1
                                        }
                                }
                        }
                }
            };
        }

        // ReSharper restore InconsistentNaming
    }
}