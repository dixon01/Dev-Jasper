// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoComposerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoComposerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Composer
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="VideoComposer"/>.
    /// </summary>
    [TestClass]
    public class VideoComposerTest
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
            var context = new PresentationContextMock();
            var element = new VideoElement
                              {
                                  X = 17,
                                  Y = 25,
                                  Width = 100,
                                  Height = 120,
                                  ZIndex = 13,
                                  VideoUri = "dummy.txt",
                                  Scaling = ElementScaling.Fixed,
                                  Replay = false,
                                  Visible = true
                              };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as VideoItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(25, item.Y);
                Assert.AreEqual(100, item.Width);
                Assert.AreEqual(120, item.Height);
                Assert.AreEqual(ElementScaling.Fixed, item.Scaling);
                Assert.IsFalse(item.Replay);
                Assert.AreEqual("dummy.txt", item.VideoUri);
                Assert.IsTrue(item.Visible);
            }
        }

        /// <summary>
        /// Test that a video URI can change through generic values.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Composer\dummy.txt")]
        [DeploymentItem(@"Presentation\Composer\other.txt")]
        public void FilenameChangedTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "dummy.txt");
            var element = new VideoElement
                              {
                                  X = 17,
                                  Y = 25,
                                  Width = 100,
                                  Height = 120,
                                  ZIndex = 13,
                                  VideoUriProperty = new AnimatedDynamicProperty
                                                         {
                                                             Evaluation = new GenericEval(0, 1, 2, 3)
                                                         },
                                  Visible = true
                              };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as VideoItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(25, item.Y);
                Assert.AreEqual(100, item.Width);
                Assert.AreEqual(120, item.Height);
                Assert.AreEqual("dummy.txt", item.VideoUri);
                Assert.IsTrue(item.Visible);

                var changes = new List<AnimatedPropertyChangedEventArgs>();
                item.PropertyValueChanged += (sender, args) => changes.Add(args);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "other.txt");

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("other.txt", changes[0].Value);
                Assert.IsNull(changes[0].Animation);
                Assert.AreEqual("other.txt", item.VideoUri);
                Assert.IsTrue(item.Visible);

                changes.Clear();

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "nowhere.txt");

                Assert.AreEqual(2, changes.Count);
                var visibleChanged = changes.Find(e => e.PropertyName == "Visible");
                Assert.IsNotNull(visibleChanged);
                Assert.AreEqual(false, visibleChanged.Value);

                var filenameChanged = changes.Find(e => e.PropertyName == "VideoUri");
                Assert.IsNotNull(filenameChanged);
                Assert.AreEqual("nowhere.txt", filenameChanged.Value);

                Assert.IsFalse(item.Visible);
                Assert.AreEqual("nowhere.txt", item.VideoUri);
            }
        }

        /// <summary>
        /// Tests that an empty video URI is properly handled.
        /// </summary>
        [TestMethod]
        public void EmptyVideoUriTest()
        {
            var context = new PresentationContextMock();
            var element = new VideoElement
                              {
                                  X = 17,
                                  Y = 25,
                                  Width = 100,
                                  Height = 120,
                                  ZIndex = 13,
                                  VideoUri = string.Empty,
                                  Visible = true
                              };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as VideoItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(25, item.Y);
                Assert.AreEqual(100, item.Width);
                Assert.AreEqual(120, item.Height);
                Assert.AreEqual(string.Empty, item.VideoUri);
                Assert.IsFalse(item.Visible);
            }
        }

        /// <summary>
        /// Tests that a null video URI is properly handled.
        /// </summary>
        [TestMethod]
        public void NullVideoUriTest()
        {
            var context = new PresentationContextMock();
            var element = new VideoElement
                              {
                                  X = 17,
                                  Y = 25,
                                  Width = 100,
                                  Height = 120,
                                  ZIndex = 13,
                                  VideoUri = null,
                                  Visible = true
                              };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as VideoItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(25, item.Y);
                Assert.AreEqual(100, item.Width);
                Assert.AreEqual(120, item.Height);
                Assert.AreEqual(string.Empty, item.VideoUri);
                Assert.IsFalse(item.Visible);
            }
        }
    }
}