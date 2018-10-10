// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebmediaPoolTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WebmediaPoolTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Webmedia
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Webmedia;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Webmedia;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="WebmediaPool"/>.
    /// </summary>
    [TestClass]
    public class WebmediaPoolTest
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
        /// Test for <see cref="WebmediaPool.MoveNext"/>.
        /// </summary>
        [TestMethod]
        public void TestMoveNext()
        {
            var context = new PresentationContextMock();
            var config = new WebmediaConfig
                {
                    Cycles =
                        {
                            new WebmediaCycleConfig
                                {
                                    Name = "CriteriaCycle",
                                    EnabledProperty = new DynamicProperty(new GenericEval(1, 2, 3, 4)),
                                    Elements =
                                        {
                                            new ImageWebmediaElement
                                                {
                                                    Filename = "criteria1.jpg",
                                                    Duration = TimeSpan.FromSeconds(5)
                                                },
                                            new ImageWebmediaElement
                                                {
                                                    Filename = "criteria2.jpg",
                                                    Duration = TimeSpan.FromSeconds(10),
                                                    EnabledProperty = new DynamicProperty(new GenericEval(2, 3, 4, 5))
                                                }
                                        }
                                },
                            new WebmediaCycleConfig
                                {
                                    Name = "MainCycle",
                                    Elements =
                                        {
                                            new ImageWebmediaElement
                                                {
                                                    Filename = "image.jpg",
                                                    Duration = TimeSpan.FromSeconds(15)
                                                },
                                            new VideoWebmediaElement
                                                {
                                                    VideoUri = "video.mpg",
                                                    Duration = TimeSpan.FromSeconds(20),
                                                    EnabledProperty = new DynamicProperty(new GenericEval(3, 4, 5, 6))
                                                },
                                            new VideoWebmediaElement
                                                {
                                                    VideoUri = "video2.mpg",
                                                    Duration = TimeSpan.FromSeconds(25)
                                                }
                                        }
                                }
                        }
                };
            var target = new WebmediaPool(config, context);

            Assert.IsNull(target.CurrentItem);

            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                var image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("image.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(15), image.Duration);

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                var video = target.CurrentItem as VideoWebmediaElement;
                Assert.IsNotNull(video);
                Assert.AreEqual("video2.mpg", video.VideoUri);
                Assert.AreEqual(TimeSpan.FromSeconds(25), video.Duration);

                context.SetCellValue(new GenericCoordinate(3, 4, 5, 6), "true");

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("image.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(15), image.Duration);

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                video = target.CurrentItem as VideoWebmediaElement;
                Assert.IsNotNull(video);
                Assert.AreEqual("video.mpg", video.VideoUri);
                Assert.AreEqual(TimeSpan.FromSeconds(20), video.Duration);

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                video = target.CurrentItem as VideoWebmediaElement;
                Assert.IsNotNull(video);
                Assert.AreEqual("video2.mpg", video.VideoUri);
                Assert.AreEqual(TimeSpan.FromSeconds(25), video.Duration);

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("image.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(15), image.Duration);

                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "true");

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("criteria1.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(5), image.Duration);

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("criteria1.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(5), image.Duration);

                context.SetCellValue(new GenericCoordinate(2, 3, 4, 5), "true");

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("criteria2.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(10), image.Duration);

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("criteria1.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(5), image.Duration);

                // this shouldn't have any effect on the next page being shown
                context.SetCellValue(new GenericCoordinate(3, 4, 5, 6), "false");

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("criteria2.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(10), image.Duration);

                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "false");

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                image = target.CurrentItem as ImageWebmediaElement;
                Assert.IsNotNull(image);
                Assert.AreEqual("image.jpg", image.Filename);
                Assert.AreEqual(TimeSpan.FromSeconds(15), image.Duration);

                Assert.IsTrue(target.MoveNext(true));

                Assert.IsNotNull(target.CurrentItem);
                video = target.CurrentItem as VideoWebmediaElement;
                Assert.IsNotNull(video);
                Assert.AreEqual("video2.mpg", video.VideoUri);
                Assert.AreEqual(TimeSpan.FromSeconds(25), video.Duration);

                context.SetCellValue(new GenericCoordinate(2, 3, 4, 5), "false");
            }
        }
    }
}
