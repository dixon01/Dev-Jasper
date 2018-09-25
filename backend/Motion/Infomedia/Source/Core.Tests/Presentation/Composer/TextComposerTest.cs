// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextComposerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextComposerTest type.
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
    /// Test for <see cref="TextComposer"/>.
    /// </summary>
    [TestClass]
    public class TextComposerTest
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
        public void ConstructorTest()
        {
            var context = new PresentationContextMock();
            var element = new TextElement
                              {
                                  X = 17,
                                  Y = 25,
                                  Width = 100,
                                  Height = 120,
                                  ZIndex = 13,
                                  Align = HorizontalAlignment.Center,
                                  Font = new Font
                                             {
                                                 Color = "red",
                                                 Face = "Verdana",
                                                 Height = 19,
                                                 Italic = true,
                                                 Weight = 500,
                                                 CharSpacing = 4,
                                                 OutlineColor = "White"
                                             },
                                  Overflow = TextOverflow.Scroll,
                                  Rotation = 45,
                                  ScrollSpeed = 23,
                                  VAlign = VerticalAlignment.Middle,
                                  Value = "Hello world!",
                                  Visible = true
                              };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as TextItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(25, item.Y);
                Assert.AreEqual(100, item.Width);
                Assert.AreEqual(120, item.Height);
                Assert.AreEqual(HorizontalAlignment.Center, item.Align);
                Assert.IsNotNull(item.Font);
                Assert.AreEqual("red", item.Font.Color);
                Assert.AreEqual("Verdana", item.Font.Face);
                Assert.AreEqual(19, item.Font.Height);
                Assert.IsTrue(item.Font.Italic);
                Assert.AreEqual(500, item.Font.Weight);
                Assert.AreEqual(4, item.Font.CharSpacing);
                Assert.AreEqual("White", item.Font.OutlineColor);
                Assert.AreEqual(TextOverflow.Scroll, item.Overflow);
                Assert.AreEqual(45, item.Rotation);
                Assert.AreEqual(23, item.ScrollSpeed);
                Assert.AreEqual(VerticalAlignment.Middle, item.VAlign);
                Assert.AreEqual("Hello world!", item.Text);
                Assert.IsTrue(item.Visible);
            }
        }

        /// <summary>
        /// Test that the text can change through generic values.
        /// </summary>
        [TestMethod]
        public void ValueChangedTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "This");
            var element = new TextElement
                              {
                                  X = 17,
                                  Y = 25,
                                  Width = 100,
                                  Height = 120,
                                  ZIndex = 13,
                                  Font = new Font
                                             {
                                                 Color = "red",
                                                 Face = "Verdana",
                                                 Height = 19,
                                                 Italic = true,
                                                 Weight = 500
                                             },
                                  ValueProperty = new AnimatedDynamicProperty
                                                      {
                                                          Evaluation = new GenericEval(0, 1, 2, 3)
                                                      },
                                  Visible = true
                              };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as TextItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(25, item.Y);
                Assert.AreEqual(100, item.Width);
                Assert.AreEqual(120, item.Height);
                Assert.AreEqual("This", item.Text);
                Assert.IsTrue(item.Visible);

                var changes = new List<AnimatedPropertyChangedEventArgs>();
                item.PropertyValueChanged += (sender, args) => changes.Add(args);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "that");

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("that", changes[0].Value);
                Assert.IsNull(changes[0].Animation);
                Assert.AreEqual("that", item.Text);
                Assert.IsTrue(item.Visible);
            }
        }
    }
}