// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScalingLayoutEngineTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScalingLayoutEngineTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Tests.Layouting
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.RendererBase.Layouting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="ScalingLayoutEngine{TItem}"/>.
    /// </summary>
    [TestClass]
    public class ScalingLayoutEngineTest
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable RedundantArgumentDefaultValue

        /// <summary>
        /// Verifies that the scaling works as defined when there is nothing to be scaled.
        /// </summary>
        [TestMethod]
        public void SingleItem_NoScaling()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(new[] { new WrappableLayoutItem(new TestableLayoutItem(50, 10)) });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Scale, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(50, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the scaling works as defined when the only item needs to be scaled by 50%.
        /// </summary>
        [TestMethod]
        public void SingleItem_Scaling_50()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(102, 20) });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Scale, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(51, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the scaling works as defined when only item needs to be scaled by a (random) scaling factor.
        /// </summary>
        [TestMethod]
        public void SingleItem_Scaling_Fraction()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(60, 20, 10) });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Scale, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(51, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(8, item.Ascent);
        }

        /// <summary>
        /// Verifies that the scaling works as defined when there is nothing to be scaled.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void MultiItem_NoScaling()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(15, 10),
                        new TestableLayoutItem(20, 10),
                        new TestableLayoutItem(13, 10)
                    });
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(11, 10),
                        new TestableLayoutItem(19, 10),
                        new TestableLayoutItem(21, 10)
                    });
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(18, 10),
                        new TestableLayoutItem(16, 10),
                        new TestableLayoutItem(14, 10)
                    });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Scale, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(9, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(15, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[1];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[2];
            Assert.AreEqual(52, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(13, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[3];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(11, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[4];
            Assert.AreEqual(28, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(19, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[5];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(21, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[6];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(18, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[7];
            Assert.AreEqual(35, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(16, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[8];
            Assert.AreEqual(51, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(14, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the scaling works as defined when the items need to be scaled by 50%.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void MultiItem_Scaling_50()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(30, 20),
                        new TestableLayoutItem(40, 20),
                        new TestableLayoutItem(26, 20)
                    });
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(22, 20),
                        new TestableLayoutItem(38, 20),
                        new TestableLayoutItem(42, 20)
                    });
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(36, 20),
                        new TestableLayoutItem(32, 20),
                        new TestableLayoutItem(28, 20)
                    });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Scale, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(9, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(15, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[1];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[2];
            Assert.AreEqual(52, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(13, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[3];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(11, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[4];
            Assert.AreEqual(28, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(19, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[5];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(21, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[6];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(18, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[7];
            Assert.AreEqual(35, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(16, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[8];
            Assert.AreEqual(51, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(14, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the scaling works as defined when the items need to be scaled by a (random) scaling factor.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void MultiItem_Scaling_Fraction()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(30, 20),
                        new TestableLayoutItem(40, 20),
                        new TestableLayoutItem(26, 20)
                    });
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(22, 20),
                        new TestableLayoutItem(38, 20),
                        new TestableLayoutItem(42, 20)
                    });
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(36, 20),
                        new TestableLayoutItem(32, 20),
                        new TestableLayoutItem(28, 20)
                    });
            var bounds = new Rectangle(17, 13, 89, 183);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Scale, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(9, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(26, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);

            item = items[1];
            Assert.AreEqual(43, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);

            item = items[2];
            Assert.AreEqual(78, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(22, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);

            item = items[3];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(19, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);

            item = items[4];
            Assert.AreEqual(36, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(33, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);

            item = items[5];
            Assert.AreEqual(69, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(36, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);

            item = items[6];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(31, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);

            item = items[7];
            Assert.AreEqual(48, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(28, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);

            item = items[8];
            Assert.AreEqual(76, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(24, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);
        }

        // ReSharper restore RedundantArgumentDefaultValue
        // ReSharper restore InconsistentNaming
    }
}
