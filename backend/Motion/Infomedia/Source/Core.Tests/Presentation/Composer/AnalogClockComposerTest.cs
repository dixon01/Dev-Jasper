// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockComposerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockComposerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Composer
{
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="AnalogClockComposer"/>.
    /// </summary>
    [TestClass]
    public class AnalogClockComposerTest
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
        /// Tests the constructor.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Composer\dummy.txt")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void TestConstructor()
        {
            // Note: we have to use dummy.txt as image file name because the
            // composer checks if the file exists and returns null hands if not
            var context = new PresentationContextMock();
            var element = new AnalogClockElement
            {
                X = 10,
                Y = 20,
                Width = 200,
                Height = 220,
                ZIndex = 5,
                Hour =
                    new AnalogClockHandElement
                    {
                        CenterX = 2,
                        CenterY = 3,
                        Filename = "dummy.txt",
                        X = 15,
                        Y = 25,
                        Width = 5,
                        Height = 40,
                        Mode = AnalogClockHandMode.Smooth
                    },
                Minute =
                    new AnalogClockHandElement
                    {
                        CenterX = 4,
                        CenterY = 6,
                        Filename = "dummy.txt",
                        X = 35,
                        Y = 55,
                        Width = 7,
                        Height = 43,
                        Mode = AnalogClockHandMode.Jump
                    },
                Seconds =
                    new AnalogClockHandElement
                    {
                        CenterX = 8,
                        CenterY = 9,
                        Filename = "dummy.txt",
                        X = 45,
                        Y = 65,
                        Width = 11,
                        Height = 13,
                        Mode = AnalogClockHandMode.CatchUp
                    }
            };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var clock = target.Item as AnalogClockItem;
                Assert.IsNotNull(clock);
                Assert.AreEqual(10, clock.X);
                Assert.AreEqual(20, clock.Y);
                Assert.AreEqual(200, clock.Width);
                Assert.AreEqual(220, clock.Height);
                Assert.AreEqual(5, clock.ZIndex);

                Assert.IsNotNull(clock.Hour);
                Assert.AreEqual(15, clock.Hour.X);
                Assert.AreEqual(25, clock.Hour.Y);
                Assert.AreEqual(5, clock.Hour.Width);
                Assert.AreEqual(40, clock.Hour.Height);
                Assert.AreEqual("dummy.txt", clock.Hour.Filename);
                Assert.AreEqual(2, clock.Hour.CenterX);
                Assert.AreEqual(3, clock.Hour.CenterY);
                Assert.AreEqual(AnalogClockHandMode.Smooth, clock.Hour.Mode);

                Assert.IsNotNull(clock.Minute);
                Assert.AreEqual(35, clock.Minute.X);
                Assert.AreEqual(55, clock.Minute.Y);
                Assert.AreEqual(7, clock.Minute.Width);
                Assert.AreEqual(43, clock.Minute.Height);
                Assert.AreEqual("dummy.txt", clock.Minute.Filename);
                Assert.AreEqual(4, clock.Minute.CenterX);
                Assert.AreEqual(6, clock.Minute.CenterY);
                Assert.AreEqual(AnalogClockHandMode.Jump, clock.Minute.Mode);

                Assert.IsNotNull(clock.Seconds);
                Assert.AreEqual(45, clock.Seconds.X);
                Assert.AreEqual(65, clock.Seconds.Y);
                Assert.AreEqual(11, clock.Seconds.Width);
                Assert.AreEqual(13, clock.Seconds.Height);
                Assert.AreEqual("dummy.txt", clock.Seconds.Filename);
                Assert.AreEqual(8, clock.Seconds.CenterX);
                Assert.AreEqual(9, clock.Seconds.CenterY);
                Assert.AreEqual(AnalogClockHandMode.CatchUp, clock.Seconds.Mode);
            }
        }

        /// <summary>
        /// Tests that the composer works fine if an image file is not found.
        /// </summary>
        [TestMethod]
        public void TestHandImageNotFound()
        {
            var context = new PresentationContextMock();
            var element = new AnalogClockElement
            {
                X = 10,
                Y = 20,
                Width = 200,
                Height = 220,
                ZIndex = 5,
                Hour =
                    new AnalogClockHandElement
                    {
                        CenterX = 2,
                        CenterY = 3,
                        Filename = "test.png",
                        X = 15,
                        Y = 25,
                        Width = 5,
                        Height = 40,
                        Mode = AnalogClockHandMode.Smooth
                    }
            };

            var target = ComposerFactory.CreateComposer(context, null, element) as IPresentableComposer;
            Assert.IsNotNull(target);

            using (target)
            {
                var clock = target.Item as AnalogClockItem;
                Assert.IsNotNull(clock);
                Assert.IsNull(clock.Hour);
                Assert.IsNull(clock.Minute);
                Assert.IsNull(clock.Seconds);
            }
        }
    }
}
