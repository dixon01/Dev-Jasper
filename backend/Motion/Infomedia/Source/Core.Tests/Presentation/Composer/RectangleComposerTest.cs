// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleComposerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RectangleComposerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Composer
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="RectangleComposer"/>.
    /// </summary>
    [TestClass]
    public class RectangleComposerTest
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
            var element = new RectangleElement
                              {
                                  X = 17,
                                  Y = 25,
                                  Width = 100,
                                  Height = 120,
                                  ZIndex = 13,
                                  Color = "Blue",
                                  Visible = true
                              };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var item = target.Item as RectangleItem;
                Assert.IsNotNull(item);
                Assert.AreEqual(17, item.X);
                Assert.AreEqual(25, item.Y);
                Assert.AreEqual(100, item.Width);
                Assert.AreEqual(120, item.Height);
                Assert.AreEqual("Blue", item.Color);
                Assert.IsTrue(item.Visible);
            }
        }
    }
}