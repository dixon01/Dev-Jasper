// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupComposerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GroupComposerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Composer
{
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
    /// Test for <see cref="GroupComposer"/>.
    /// </summary>
    [TestClass]
    public class GroupComposerTest
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
        /// Tests nested properties inheritance (coordinates and cropping of width and height).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Composer\dummy.txt")]
        public void NestedPropertiesTest()
        {
            var context = new PresentationContextMock();
            var groupElement = new GroupElement
                {
                    X = 10,
                    Y = 20,
                    Width = 200,
                    Height = 400,
                    Visible = true,
                    Elements =
                        {
                            new ImageElement
                                {
                                    X = 17,
                                    Y = 25,
                                    Width = 100,
                                    Height = 120,
                                    ZIndex = 13,
                                    Filename = "dummy.txt",
                                    Visible = true
                                },
                            new ImageElement
                                {
                                    X = 33,
                                    Y = 57,
                                    Width = 180,
                                    Height = 370,
                                    ZIndex = 7,
                                    Filename = "dummy.txt",
                                    Visible = true
                                }
                        }
                };

            var target = ComposerFactory.CreateComposer(context, null, groupElement);
            Assert.IsInstanceOfType(target, typeof(GroupComposer));

            using (target)
            {
                var composer =
                    ComposerFactory.CreateComposer(context, target, groupElement.Elements[0]) as IPresentableComposer;
                Assert.IsNotNull(composer);

                var item = composer.Item as ImageItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(27, item.X);
                Assert.AreEqual(45, item.Y);
                Assert.AreEqual(100, item.Width);
                Assert.AreEqual(120, item.Height);
                Assert.AreEqual("dummy.txt", item.Filename);
                Assert.IsTrue(item.Visible);

                composer = ComposerFactory.CreateComposer(context, target, groupElement.Elements[1]) as ImageComposer;
                Assert.IsNotNull(composer);

                // this item gets cropped
                item = composer.Item as ImageItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(43, item.X);
                Assert.AreEqual(77, item.Y);
                Assert.AreEqual(167, item.Width);
                Assert.AreEqual(343, item.Height);
                Assert.AreEqual("dummy.txt", item.Filename);
                Assert.IsTrue(item.Visible);
            }
        }

        /// <summary>
        /// Checks that visibility of nested elements changes when this element's visibility changes.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Composer\dummy.txt")]
        public void VisibleChangedTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "true");
            var groupElement = new GroupElement
                {
                    X = 10,
                    Y = 20,
                    Width = 200,
                    Height = 400,
                    VisibleProperty = new AnimatedDynamicProperty(new GenericEval(0, 1, 2, 3)),
                    Elements =
                        {
                            new ImageElement
                                {
                                    X = 17,
                                    Y = 25,
                                    Width = 100,
                                    Height = 120,
                                    ZIndex = 13,
                                    Filename = "dummy.txt",
                                    Visible = true
                                },
                            new ImageElement
                                {
                                    X = 33,
                                    Y = 57,
                                    Width = 180,
                                    Height = 370,
                                    ZIndex = 7,
                                    Filename = "dummy.txt",
                                    Visible = true
                                }
                        }
                };

            var target = ComposerFactory.CreateComposer(context, null, groupElement);
            Assert.IsInstanceOfType(target, typeof(GroupComposer));

            using (target)
            {
                var composerA =
                    ComposerFactory.CreateComposer(context, target, groupElement.Elements[0]) as IPresentableComposer;
                Assert.IsNotNull(composerA);

                var item = composerA.Item as ImageItem;
                Assert.IsNotNull(item);
                Assert.IsTrue(item.Visible);
                AnimatedPropertyChangedEventArgs itemAChange = null;
                item.PropertyValueChanged += (sender, args) => itemAChange = args;

                var composerB =
                    ComposerFactory.CreateComposer(context, target, groupElement.Elements[1]) as IPresentableComposer;
                Assert.IsNotNull(composerB);

                // this item gets cropped
                item = composerB.Item as ImageItem;
                Assert.IsNotNull(item);
                Assert.IsTrue(item.Visible);
                AnimatedPropertyChangedEventArgs itemBChange = null;
                item.PropertyValueChanged += (sender, args) => itemBChange = args;

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "false");

                Assert.IsNotNull(itemAChange);
                Assert.AreEqual(false, itemAChange.Value);
                Assert.IsNull(itemAChange.Animation);
                Assert.IsFalse(((ImageItem)composerA.Item).Visible);

                Assert.IsNotNull(itemBChange);
                Assert.AreEqual(false, itemBChange.Value);
                Assert.IsNull(itemBChange.Animation);
                Assert.IsFalse(((ImageItem)composerB.Item).Visible);
            }
        }
    }
}