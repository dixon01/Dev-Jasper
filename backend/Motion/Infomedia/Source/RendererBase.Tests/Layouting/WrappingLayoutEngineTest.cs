// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrappingLayoutEngineTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrappingLayoutEngineTest type.
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
    /// Unit tests for <see cref="WrappingLayoutEngine{TItem}"/>.
    /// </summary>
    [TestClass]
    public class WrappingLayoutEngineTest
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable RedundantArgumentDefaultValue

        /// <summary>
        /// Verifies that the wrapping works as defined when there is only one item that can't be split.
        /// </summary>
        [TestMethod]
        public void SingleItem_Unsplittable()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(new[] { new WrappableLayoutItem(new TestableLayoutItem(100, 10)) });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Wrap, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the wrapping works as defined when there is only one item that can be split
        /// and the first item(s) fit into the given bounds.
        /// </summary>
        [TestMethod]
        public void SingleItem_Splittable_SmallFirst()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(
                            new TestableLayoutItem(23, 10),
                            new TestableLayoutItem(21, 10),
                            new TestableLayoutItem(29, 10),
                            new TestableLayoutItem(13, 10))
                    });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Wrap, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(2, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(44, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[1];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(42, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the wrapping works as defined when there is only one item that can be split
        /// and the first item doesn't fit into the given bounds.
        /// </summary>
        [TestMethod]
        public void SingleItem_Splittable_LargeFirst()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(
                            new TestableLayoutItem(57, 10),
                            new TestableLayoutItem(22, 10),
                            new TestableLayoutItem(29, 10),
                            new TestableLayoutItem(13, 10))
                    });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Wrap, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(3, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(57, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[1];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(51, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[2];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(13, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the wrapping works as defined when there are multiple items that can't be split.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void MultiItem_Unsplittable()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(60, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(30, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(20, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(20, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(30, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(65, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(40, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(30, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(65, 10))
                    });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Wrap, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(9, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(60, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[1];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[2];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[3];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[4];
            Assert.AreEqual(37, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[5];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(43, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[6];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(53, item.Y);
            Assert.AreEqual(40, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[7];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(63, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[8];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(73, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the wrapping works as defined when there are multiple items that can be split.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void MultiItem_Splittable()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(
                            new TestableLayoutItem("A", 23, 10),
                            new TestableLayoutItem("B", 37, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("C", 10, 10),
                            new TestableLayoutItem("D", 17, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem("E", 20, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem("F", 21, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("G", 15, 10),
                            new TestableLayoutItem("H", 13, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("I", 44, 10),
                            new TestableLayoutItem("J", 22, 10),
                            new TestableLayoutItem("K", 9, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem("L", 23, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("M", 24, 10),
                            new TestableLayoutItem("N", 11, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("O", 12, 10),
                            new TestableLayoutItem("P", 23, 10),
                            new TestableLayoutItem("Q", 8, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem("R", 22, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("S", 14, 10),
                            new TestableLayoutItem("T", 12, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("U", 54, 10),
                            new TestableLayoutItem("V", 22, 10),
                            new TestableLayoutItem("W", 9, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(
                            new TestableLayoutItem("X", 15, 10),
                            new TestableLayoutItem("Y", 40, 10),
                            new TestableLayoutItem("Z", 65, 10))
                    });
            var bounds = new Rectangle(17, 13, 51, 183);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Wrap, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(21, items.Count);

            var item = items[0];
            Assert.AreEqual("A", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(23, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[1];
            Assert.AreEqual("B", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(37, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[2];
            Assert.AreEqual("C", item.Name);
            Assert.AreEqual(54, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(10, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[3];
            Assert.AreEqual("D", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(17, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[4];
            Assert.AreEqual("E", item.Name);
            Assert.AreEqual(34, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[5];
            Assert.AreEqual("F", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(43, item.Y);
            Assert.AreEqual(21, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[6];
            Assert.AreEqual("GH", item.Name);
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(43, item.Y);
            Assert.AreEqual(28, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[7];
            Assert.AreEqual("I", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(53, item.Y);
            Assert.AreEqual(44, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[8];
            Assert.AreEqual("JK", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(63, item.Y);
            Assert.AreEqual(31, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[9];
            Assert.AreEqual("L", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(73, item.Y);
            Assert.AreEqual(23, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[10];
            Assert.AreEqual("M", item.Name);
            Assert.AreEqual(40, item.X);
            Assert.AreEqual(73, item.Y);
            Assert.AreEqual(24, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[11];
            Assert.AreEqual("N", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(83, item.Y);
            Assert.AreEqual(11, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[12];
            Assert.AreEqual("OP", item.Name);
            Assert.AreEqual(28, item.X);
            Assert.AreEqual(83, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[13];
            Assert.AreEqual("Q", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(93, item.Y);
            Assert.AreEqual(8, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[14];
            Assert.AreEqual("R", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(103, item.Y);
            Assert.AreEqual(22, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[15];
            Assert.AreEqual("ST", item.Name);
            Assert.AreEqual(39, item.X);
            Assert.AreEqual(103, item.Y);
            Assert.AreEqual(26, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[16];
            Assert.AreEqual("U", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(113, item.Y);
            Assert.AreEqual(54, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[17];
            Assert.AreEqual("VW", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(123, item.Y);
            Assert.AreEqual(31, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[18];
            Assert.AreEqual("X", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(133, item.Y);
            Assert.AreEqual(15, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[19];
            Assert.AreEqual("Y", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(143, item.Y);
            Assert.AreEqual(40, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[20];
            Assert.AreEqual("Z", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(153, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the wrapping with scaling works as defined when there are multiple items that can't be split.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void MultiItem_Scale_Unsplittable()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(24, 15)),
                        new WrappableLayoutItem(new TestableLayoutItem(21, 15)),
                        new WrappableLayoutItem(new TestableLayoutItem(30, 15))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(21, 15)),
                        new WrappableLayoutItem(new TestableLayoutItem(27, 15)),
                        new WrappableLayoutItem(new TestableLayoutItem(24, 15))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(12, 15)),
                        new WrappableLayoutItem(new TestableLayoutItem(18, 15)),
                        new WrappableLayoutItem(new TestableLayoutItem(45, 15))
                    });
            var bounds = new Rectangle(17, 13, 50, 30);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.WrapScale, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(9, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(16, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[1];
            Assert.AreEqual(33, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(14, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[2];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[3];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(14, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[4];
            Assert.AreEqual(31, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(18, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[5];
            Assert.AreEqual(49, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(16, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[6];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(8, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[7];
            Assert.AreEqual(25, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(12, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[8];
            Assert.AreEqual(37, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the wrapping with scaling works as defined when there are multiple items that can't be split
        /// and the items don't have to be split.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void MultiItem_NoScale_Unsplittable()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(60, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(30, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(20, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(20, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(30, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(65, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem(40, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(30, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem(65, 10))
                    });
            var bounds = new Rectangle(17, 13, 65, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.WrapScale, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(9, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(60, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[1];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[2];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(23, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[3];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[4];
            Assert.AreEqual(37, item.X);
            Assert.AreEqual(33, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[5];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(43, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[6];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(53, item.Y);
            Assert.AreEqual(40, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[7];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(63, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);

            item = items[8];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(73, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the wrapping and scaling works as defined when there is only one item that can be split
        /// and the first item(s) fit into the given bounds.
        /// </summary>
        [TestMethod]
        public void SingleItem_Scale_Splittable_SmallFirst()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(
                            new TestableLayoutItem(23, 10),
                            new TestableLayoutItem(21, 10),
                            new TestableLayoutItem(8, 10),
                            new TestableLayoutItem(13, 10))
                    });
            var bounds = new Rectangle(17, 13, 51, 17);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.WrapScale, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(2, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(45, item.Width);
            Assert.AreEqual(8, item.Height);
            Assert.AreEqual(8, item.Ascent);

            item = items[1];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(21, item.Y);
            Assert.AreEqual(11, item.Width);
            Assert.AreEqual(8, item.Height);
            Assert.AreEqual(8, item.Ascent);
        }

        /// <summary>
        /// Verifies that the scaling works as defined when there is only one item that can't be split
        /// and it doesn't fit horizontally into the given bounds (thus has to be scaled).
        /// </summary>
        [TestMethod]
        public void SingleItem_ScaleX()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(new[] { new WrappableLayoutItem(new TestableLayoutItem(102, 10)) });
            var bounds = new Rectangle(17, 13, 51, 17);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.WrapScale, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(51, item.Width);
            Assert.AreEqual(5, item.Height);
            Assert.AreEqual(5, item.Ascent);
        }

        /// <summary>
        /// Verifies that the scaling works as defined when there is only one item that can't be split
        /// and it doesn't fit vertically into the given bounds (thus has to be scaled).
        /// </summary>
        [TestMethod]
        public void SingleItem_ScaleY()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(new[] { new WrappableLayoutItem(new TestableLayoutItem(40, 34)) });
            var bounds = new Rectangle(17, 13, 51, 17);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.WrapScale, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(20, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(17, item.Ascent);
        }

        /// <summary>
        /// Verifies that the wrapping and scaling works as defined when there are multiple items that can be split.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void MultiItem_Scale_Splittable()
        {
            var manager = new LayoutManager<WrappableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(
                            new TestableLayoutItem("A", 23, 10),
                            new TestableLayoutItem("B", 37, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("C", 10, 10),
                            new TestableLayoutItem("D", 17, 10)),
                        new WrappableLayoutItem(new TestableLayoutItem("E", 20, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem("F", 21, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("G", 15, 10),
                            new TestableLayoutItem("H", 13, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("I", 44, 10),
                            new TestableLayoutItem("J", 22, 10),
                            new TestableLayoutItem("K", 9, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem("L", 23, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("M", 24, 10),
                            new TestableLayoutItem("N", 11, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("O", 12, 10),
                            new TestableLayoutItem("P", 23, 10),
                            new TestableLayoutItem("Q", 8, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(new TestableLayoutItem("R", 22, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("S", 14, 10),
                            new TestableLayoutItem("T", 12, 10)),
                        new WrappableLayoutItem(
                            new TestableLayoutItem("U", 54, 10),
                            new TestableLayoutItem("V", 22, 10),
                            new TestableLayoutItem("W", 9, 10))
                    });
            manager.AddLine(
                new[]
                    {
                        new WrappableLayoutItem(
                            new TestableLayoutItem("X", 15, 10),
                            new TestableLayoutItem("Y", 40, 10),
                            new TestableLayoutItem("Z", 65, 10))
                    });
            var bounds = new Rectangle(17, 13, 51, 83);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.WrapScale, false);

            var items = new List<WrappableLayoutItem>(manager.Items);
            Assert.AreEqual(17, items.Count);

            var item = items[0];
            Assert.AreEqual("AB", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(41, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[1];
            Assert.AreEqual("C", item.Name);
            Assert.AreEqual(58, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(7, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[2];
            Assert.AreEqual("D", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(20, item.Y);
            Assert.AreEqual(11, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[3];
            Assert.AreEqual("E", item.Name);
            Assert.AreEqual(28, item.X);
            Assert.AreEqual(20, item.Y);
            Assert.AreEqual(14, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[4];
            Assert.AreEqual("F", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(27, item.Y);
            Assert.AreEqual(14, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[5];
            Assert.AreEqual("GH", item.Name);
            Assert.AreEqual(31, item.X);
            Assert.AreEqual(27, item.Y);
            Assert.AreEqual(19, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[6];
            Assert.AreEqual("IJK", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(34, item.Y);
            Assert.AreEqual(51, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[7];
            Assert.AreEqual("L", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(41, item.Y);
            Assert.AreEqual(16, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[8];
            Assert.AreEqual("MN", item.Name);
            Assert.AreEqual(33, item.X);
            Assert.AreEqual(41, item.Y);
            Assert.AreEqual(23, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[9];
            Assert.AreEqual("O", item.Name);
            Assert.AreEqual(56, item.X);
            Assert.AreEqual(41, item.Y);
            Assert.AreEqual(8, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[10];
            Assert.AreEqual("PQ", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(48, item.Y);
            Assert.AreEqual(21, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[11];
            Assert.AreEqual("R", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(55, item.Y);
            Assert.AreEqual(15, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[12];
            Assert.AreEqual("ST", item.Name);
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(55, item.Y);
            Assert.AreEqual(17, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[13];
            Assert.AreEqual("U", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(62, item.Y);
            Assert.AreEqual(37, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[14];
            Assert.AreEqual("VW", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(69, item.Y);
            Assert.AreEqual(21, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[15];
            Assert.AreEqual("XY", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(76, item.Y);
            Assert.AreEqual(38, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);

            item = items[16];
            Assert.AreEqual("Z", item.Name);
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(83, item.Y);
            Assert.AreEqual(45, item.Width);
            Assert.AreEqual(7, item.Height);
            Assert.AreEqual(7, item.Ascent);
        }

        // ReSharper restore RedundantArgumentDefaultValue
        // ReSharper restore InconsistentNaming
    }
}