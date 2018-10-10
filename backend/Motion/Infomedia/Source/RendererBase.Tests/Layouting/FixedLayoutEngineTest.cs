// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedLayoutEngineTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FixedLayoutEngineTest type.
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
    /// Unit tests for <see cref="FixedLayoutEngine{TItem}"/>.
    /// </summary>
    [TestClass]
    public class FixedLayoutEngineTest
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable RedundantArgumentDefaultValue
        #region Single Item
        #region VALIGN=TOP

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Left_Top_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Center_Top_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(67, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Right_Top_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Left_Top_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Center_Top_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(-33, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Right_Top_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        #endregion

        #region VALIGN=MIDDLE

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Left_Middle_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(58, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Center_Middle_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(67, item.X);
            Assert.AreEqual(58, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Right_Middle_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(58, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Left_Middle_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(8, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Center_Middle_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(-33, item.X);
            Assert.AreEqual(8, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Right_Middle_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(8, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        #endregion

        #region VALIGN=BOTTOM

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Left_Bottom_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(103, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Center_Bottom_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(67, item.X);
            Assert.AreEqual(103, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Right_Bottom_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(103, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Left_Bottom_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(3, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Center_Bottom_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(-33, item.X);
            Assert.AreEqual(3, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Right_Bottom_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(3, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(10, item.Ascent);
        }

        #endregion

        #region VALIGN=BASELINE

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Left_Baseline_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10, 7) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(7, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Center_Baseline_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10, 7) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(67, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(7, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Right_Baseline_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10, 7) });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(7, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Left_Baseline_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10, 7) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(6, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(7, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Center_Baseline_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10, 7) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(-33, item.X);
            Assert.AreEqual(6, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(7, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void SingleItem_Right_Baseline_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(new[] { new TestableLayoutItem(100, 10, 7) });
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(1, items.Count);

            var item = items[0];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(6, item.Y);
            Assert.AreEqual(100, item.Width);
            Assert.AreEqual(10, item.Height);
            Assert.AreEqual(7, item.Ascent);
        }

        #endregion
        #endregion

        #region Multiple Items
        #region VALIGN=TOP (base align=false)

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Top_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Top_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(15, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(45, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(120, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(155, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(11, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Top_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(12, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(152, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Top_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Top_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-85, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-55, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(20, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(55, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-89, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-62, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(22, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Top_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-188, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-158, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(-48, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-195, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-168, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(-84, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        #endregion

        #region VALIGN=TOP (base align=true)

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Top_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Top_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Top, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(15, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(45, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(120, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(155, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(11, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Top_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Top, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(12, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(152, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Top_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Top_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Top, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-85, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-55, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(20, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(55, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-89, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-62, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(22, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Top
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Top_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Top, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-188, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-158, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(-48, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-195, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-168, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(-84, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        #endregion

        #region VALIGN=MIDDLE (base align=false)

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Middle_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(46, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(48, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(46, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(64, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(63, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Middle_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(15, item.X);
            Assert.AreEqual(46, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(45, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(120, item.X);
            Assert.AreEqual(48, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(155, item.X);
            Assert.AreEqual(46, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(11, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(64, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(63, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Middle_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(12, item.X);
            Assert.AreEqual(46, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(48, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(152, item.X);
            Assert.AreEqual(46, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(64, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(63, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Middle_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(-3, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(-2, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(14, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Middle_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-85, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-55, item.X);
            Assert.AreEqual(-3, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(20, item.X);
            Assert.AreEqual(-2, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(55, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-89, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-62, item.X);
            Assert.AreEqual(14, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(22, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Middle_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Middle, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-188, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-158, item.X);
            Assert.AreEqual(-3, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(-2, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(-48, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-195, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-168, item.X);
            Assert.AreEqual(14, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(-84, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        #endregion

        #region VALIGN=MIDDLE (base align=true)

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Middle_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Middle, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(45, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(45, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(43, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(62, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Middle_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Middle, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(15, item.X);
            Assert.AreEqual(45, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(45, item.X);
            Assert.AreEqual(45, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(120, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(155, item.X);
            Assert.AreEqual(43, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(11, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(62, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Middle_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Middle, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(12, item.X);
            Assert.AreEqual(45, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(45, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(152, item.X);
            Assert.AreEqual(43, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(62, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(65, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Middle_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Middle, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(-2, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(-6, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(16, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(16, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Middle_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Middle, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-85, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-55, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(20, item.X);
            Assert.AreEqual(-2, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(55, item.X);
            Assert.AreEqual(-6, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-89, item.X);
            Assert.AreEqual(16, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-62, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(22, item.X);
            Assert.AreEqual(16, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Middle
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Middle_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Middle, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-188, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-158, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(-2, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(-48, item.X);
            Assert.AreEqual(-6, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-195, item.X);
            Assert.AreEqual(16, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-168, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(-84, item.X);
            Assert.AreEqual(16, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        #endregion

        #region VALIGN=BOTTOM (base align=false)

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Bottom_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(79, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(81, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(83, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(79, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(101, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(99, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Bottom_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(15, item.X);
            Assert.AreEqual(79, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(45, item.X);
            Assert.AreEqual(81, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(120, item.X);
            Assert.AreEqual(83, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(155, item.X);
            Assert.AreEqual(79, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(11, item.X);
            Assert.AreEqual(101, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(99, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Bottom_Box()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(12, item.X);
            Assert.AreEqual(79, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(81, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(83, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(152, item.X);
            Assert.AreEqual(79, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(101, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(99, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Bottom_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(-21, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(-19, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(-17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(-21, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(1, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(-1, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Bottom_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-85, item.X);
            Assert.AreEqual(-21, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-55, item.X);
            Assert.AreEqual(-19, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(20, item.X);
            Assert.AreEqual(-17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(55, item.X);
            Assert.AreEqual(-21, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-89, item.X);
            Assert.AreEqual(1, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-62, item.X);
            Assert.AreEqual(-1, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(22, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Bottom_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Bottom, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-188, item.X);
            Assert.AreEqual(-21, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-158, item.X);
            Assert.AreEqual(-19, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(-17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(-48, item.X);
            Assert.AreEqual(-21, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-195, item.X);
            Assert.AreEqual(1, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-168, item.X);
            Assert.AreEqual(-1, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(-84, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        #endregion

        #region VALIGN=BOTTOM (base align=true)

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Bottom_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Bottom, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(76, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(76, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(78, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(74, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(93, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Bottom_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Bottom, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(15, item.X);
            Assert.AreEqual(76, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(45, item.X);
            Assert.AreEqual(76, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(120, item.X);
            Assert.AreEqual(78, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(155, item.X);
            Assert.AreEqual(74, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(11, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(93, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Bottom_Box_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Bottom, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(12, item.X);
            Assert.AreEqual(76, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(76, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(78, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(152, item.X);
            Assert.AreEqual(74, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(93, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(96, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Bottom_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Bottom, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(-24, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(-24, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(-22, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(-26, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(-7, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Bottom_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Bottom, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-85, item.X);
            Assert.AreEqual(-24, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-55, item.X);
            Assert.AreEqual(-24, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(20, item.X);
            Assert.AreEqual(-22, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(55, item.X);
            Assert.AreEqual(-26, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-89, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-62, item.X);
            Assert.AreEqual(-7, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(22, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Bottom
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Bottom_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Bottom, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-188, item.X);
            Assert.AreEqual(-24, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-158, item.X);
            Assert.AreEqual(-24, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(-22, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(-48, item.X);
            Assert.AreEqual(-26, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-195, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-168, item.X);
            Assert.AreEqual(-7, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(-84, item.X);
            Assert.AreEqual(-4, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        #endregion

        #region VALIGN=BASELINE (base align=false)

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Baseline_Box()
        {
            // special case: this will actually align on top but using the baseline within the lines
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Baseline_Box()
        {
            // special case: this will actually align on top but using the baseline within the lines
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(15, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(45, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(120, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(155, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(11, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Baseline_Box()
        {
            // special case: this will actually align on top but using the baseline within the lines
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(12, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(152, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Baseline_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(4, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(0, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(19, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Baseline_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-85, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-55, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(20, item.X);
            Assert.AreEqual(4, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(55, item.X);
            Assert.AreEqual(0, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-89, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-62, item.X);
            Assert.AreEqual(19, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(22, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: false
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Baseline_Point()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Baseline, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-188, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-158, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(4, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(-48, item.X);
            Assert.AreEqual(0, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-195, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-168, item.X);
            Assert.AreEqual(19, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(-84, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }
        #endregion

        #region VALIGN=BASELINE (base align=true)

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Left
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Baseline_Box_BaseAlign()
        {
            // special case: this will actually align on top but using the baseline within the lines
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Baseline, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Center
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Baseline_Box_BaseAlign()
        {
            // special case: this will actually align on top but using the baseline within the lines
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Baseline, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(15, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(45, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(120, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(155, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(11, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(38, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a rectangle
        /// Horizontal alignment: Right
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Baseline_Box_BaseAlign()
        {
            // special case: this will actually align on top but using the baseline within the lines
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Baseline, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(12, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(15, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(117, item.X);
            Assert.AreEqual(17, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(152, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(32, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(35, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Left
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Left_Baseline_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Baseline, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(47, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(122, item.X);
            Assert.AreEqual(4, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(157, item.X);
            Assert.AreEqual(0, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(44, item.X);
            Assert.AreEqual(19, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(128, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Center
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Center_Baseline_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Center, VerticalAlignment.Baseline, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-85, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-55, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(20, item.X);
            Assert.AreEqual(4, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(55, item.X);
            Assert.AreEqual(0, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-89, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-62, item.X);
            Assert.AreEqual(19, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(22, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }

        /// <summary>
        /// Verifies that the layout works as defined for the following parameters:
        /// Bounds define a point
        /// Horizontal alignment: Right
        /// Vertical alignment: Baseline
        /// Text overflow: Extend
        /// Base alignment: true
        /// </summary>
        [TestMethod]
        public void MultiItems_Right_Baseline_Point_BaseAlign()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            AddMultipleItems(manager);
            var bounds = new Rectangle(17, 13, 0, 0);
            manager.Layout(bounds, HorizontalAlignment.Right, VerticalAlignment.Baseline, TextOverflow.Extend, true);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(7, items.Count);

            var item = items[0];
            Assert.AreEqual(-188, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(-158, item.X);
            Assert.AreEqual(2, item.Y);
            Assert.AreEqual(75, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(-83, item.X);
            Assert.AreEqual(4, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(-48, item.X);
            Assert.AreEqual(0, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(-195, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(-168, item.X);
            Assert.AreEqual(19, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(-84, item.X);
            Assert.AreEqual(22, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);
        }
        #endregion

        #endregion

        #region Special Tests

        /// <summary>
        /// Tests that multiple lines with different alignments are properly aligned with default alignment left.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        [TestMethod]
        public void MultiItemsMultiAlign_Left()
        {
            var manager = new LayoutManager<TestableLayoutItem>();
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(30, 17, 11),
                        new TestableLayoutItem(66, 15, 11),
                        new TestableLayoutItem(35, 13, 9),
                        new TestableLayoutItem(65, 17, 13)
                    },
                HorizontalAlignment.Center);
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(27, 12, 11),
                        new TestableLayoutItem(84, 14, 14),
                        new TestableLayoutItem(101, 17, 11)
                    },
                HorizontalAlignment.Right);
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(25, 12, 11),
                        new TestableLayoutItem(33, 13, 13),
                        new TestableLayoutItem(87, 14, 9)
                    });
            var bounds = new Rectangle(17, 13, 200, 100);
            manager.Layout(bounds, HorizontalAlignment.Left, VerticalAlignment.Top, TextOverflow.Extend, false);

            var items = new List<TestableLayoutItem>(manager.Items);
            Assert.AreEqual(10, items.Count);

            var item = items[0];
            Assert.AreEqual(19, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(30, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[1];
            Assert.AreEqual(49, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(66, item.Width);
            Assert.AreEqual(15, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[2];
            Assert.AreEqual(115, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(35, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(9, item.Ascent);

            item = items[3];
            Assert.AreEqual(150, item.X);
            Assert.AreEqual(13, item.Y);
            Assert.AreEqual(65, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[4];
            Assert.AreEqual(5, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(27, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[5];
            Assert.AreEqual(32, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(84, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(14, item.Ascent);

            item = items[6];
            Assert.AreEqual(116, item.X);
            Assert.AreEqual(30, item.Y);
            Assert.AreEqual(101, item.Width);
            Assert.AreEqual(17, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[7];
            Assert.AreEqual(17, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(25, item.Width);
            Assert.AreEqual(12, item.Height);
            Assert.AreEqual(11, item.Ascent);

            item = items[8];
            Assert.AreEqual(42, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(33, item.Width);
            Assert.AreEqual(13, item.Height);
            Assert.AreEqual(13, item.Ascent);

            item = items[9];
            Assert.AreEqual(75, item.X);
            Assert.AreEqual(47, item.Y);
            Assert.AreEqual(87, item.Width);
            Assert.AreEqual(14, item.Height);
            Assert.AreEqual(9, item.Ascent);
        }
        #endregion

        private static void AddMultipleItems(LayoutManager<TestableLayoutItem> manager)
        {
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(30, 17, 11),
                        new TestableLayoutItem(75, 15, 11),
                        new TestableLayoutItem(35, 13, 9),
                        new TestableLayoutItem(65, 17, 13)
                    });
            manager.AddLine(
                new[]
                    {
                        new TestableLayoutItem(27, 12, 11),
                        new TestableLayoutItem(84, 14, 14),
                        new TestableLayoutItem(101, 17, 11)
                    });
        }

        // ReSharper restore RedundantArgumentDefaultValue
        // ReSharper restore InconsistentNaming
    }
}