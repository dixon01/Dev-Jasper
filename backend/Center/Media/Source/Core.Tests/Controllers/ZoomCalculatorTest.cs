// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomCalculatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ZoomCalculatorTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers
{
    using System.Windows;

    using Gorba.Center.Media.Core.Controllers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Defines tests for the <see cref="ZoomCalculatorTest"/>;
    /// </summary>
    [TestClass]
    public class ZoomCalculatorTest
    {
        /// <summary>
        /// Initializes the current test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
        }

        /// <summary>
        /// Cleans up the current test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
        }

        /// <summary>
        /// Tests the simple centered zoom
        /// </summary>
        [TestMethod]
        public void SimpleZoomTest()
        {
            const int InitialZoom = 100;
            var layoutPosition = new Point(0, 0);
            const int ScreenWidth = 1368;
            const int ScreenHeight = 768;
            const int EditorWidth = 1169;
            const int EditorHeight = 760;
            var zoomPosition = new Point(ScreenWidth / 2, ScreenHeight / 2);

            var zoomCalculator = new ZoomCalculator(
                InitialZoom,
                layoutPosition,
                ScreenWidth,
                ScreenHeight,
                EditorWidth,
                EditorHeight,
                0,
                1000);

            zoomCalculator.ZoomInAt(zoomPosition, 10);
            var newLayoutPosition = zoomCalculator.GetLayoutPosition();
            var newZoom = zoomCalculator.GetZoom();

            Assert.AreEqual(new Point(0, 0), newLayoutPosition, "the new LayoutPosition is not as expected");
            Assert.AreEqual(110, newZoom, "the new zoom is not as expected");
        }

        /// <summary>
        /// Tests the simple zoom left of center
        /// </summary>
        [TestMethod]
        public void SimpleZoomInTest()
        {
            const int InitialZoom = 100;
            var layoutPosition = new Point(0, 0);
            const int ScreenWidth = 1368;
            const int ScreenHeight = 768;
            const int EditorWidth = 1169;
            const int EditorHeight = 760;
            const double HorizontalZoomOffset = 100;
            var zoomPosition = new Point((ScreenWidth / 2) - HorizontalZoomOffset, ScreenHeight / 2);

            var zoomCalculator = new ZoomCalculator(
                InitialZoom,
                layoutPosition,
                ScreenWidth,
                ScreenHeight,
                EditorWidth,
                EditorHeight,
                0,
                1000);

            zoomCalculator.ZoomInAt(zoomPosition, 10);
            var newLayoutPosition = zoomCalculator.GetLayoutPosition();
            var newZoom = zoomCalculator.GetZoom();

            var expectedPoint = new Point(HorizontalZoomOffset * 0.1, 0);

            Assert.AreEqual(expectedPoint, newLayoutPosition, "the new LayoutPosition is not as expected");
            Assert.AreEqual(110, newZoom, "the new zoom is not as expected");
        }

        /// <summary>
        /// Tests the full screen rectangle zoom
        /// </summary>
        [TestMethod]
        public void RectangleZoomTest()
        {
            const int InitialZoom = 72;
            var layoutPosition = new Point(0, 0);
            var zoomRectangle = new Rect(99.5, 4, 1169, 760);
            const int ScreenWidth = 1368;
            const int ScreenHeight = 768;
            const int EditorWidth = 1169;
            const int EditorHeight = 760;

            var zoomCalculator = new ZoomCalculator(
                InitialZoom,
                layoutPosition,
                ScreenWidth,
                ScreenHeight,
                EditorWidth,
                EditorHeight,
                0,
                1000);

            zoomCalculator.SetRectangleZoom(zoomRectangle);
            var newLayoutPosition = zoomCalculator.GetLayoutPosition();
            var newZoom = zoomCalculator.GetZoom();

            Assert.AreEqual(new Point(0, 0), newLayoutPosition, "the new LayoutPosition is not as expected");
            Assert.AreEqual(100, newZoom, "the new zoom is not as expected");
        }

        /// <summary>
        /// Tests the full screen rectangle zoom
        /// </summary>
        [TestMethod]
        public void RectangleZoomTest2()
        {
            const int InitialZoom = 72;
            var layoutPosition = new Point(0, 0);
            const int ScreenWidth = 1368;
            const int ScreenHeight = 768;
            const int EditorWidth = 1169;
            const int EditorHeight = 760;
            var zoomRectangle = new Rect(EditorWidth / 2, EditorHeight / 2, EditorWidth, EditorHeight);

            var zoomCalculator = new ZoomCalculator(
                InitialZoom,
                layoutPosition,
                ScreenWidth,
                ScreenHeight,
                EditorWidth,
                EditorHeight,
                10,
                1000);

            zoomCalculator.SetRectangleZoom(zoomRectangle);
            var newLayoutPosition = zoomCalculator.GetLayoutPosition();
            var newZoom = zoomCalculator.GetZoom();

            Assert.AreEqual(
                new Point(-(EditorWidth / 2) + 99.5, -(EditorHeight / 2) + 4),
                newLayoutPosition,
                "the new LayoutPosition is not as expected");
            Assert.AreEqual(100, newZoom, "the new zoom is not as expected");
        }

        /// <summary>
        /// Tests the full screen rectangle zoom
        /// </summary>
        [TestMethod]
        public void RectangleZoomTest3()
        {
            const int InitialZoom = 72;
            var layoutPosition = new Point(0, 0);
            const int ScreenWidth = 100;
            const int ScreenHeight = 100;
            const int EditorWidth = 100;
            const int EditorHeight = 100;
            const int ZoomRectSize = 10;

            var zoomRectangle = new Rect(
                (EditorWidth / 2) - (ZoomRectSize / 2),
                (EditorHeight / 2) - (ZoomRectSize / 2),
                ZoomRectSize,
                ZoomRectSize);

            var zoomCalculator = new ZoomCalculator(
                InitialZoom,
                layoutPosition,
                ScreenWidth,
                ScreenHeight,
                EditorWidth,
                EditorHeight,
                10,
                1000);

            zoomCalculator.SetRectangleZoom(zoomRectangle);
            var newLayoutPosition = zoomCalculator.GetLayoutPosition();
            var newZoom = zoomCalculator.GetZoom();

            Assert.AreEqual(new Point(0, 0), newLayoutPosition, "the new LayoutPosition is not as expected");
            Assert.AreEqual(1000, newZoom, "the new zoom is not as expected");
        }

        /// <summary>
        /// Tests the half screen rectangle zoom
        /// </summary>
        [TestMethod]
        public void HalfScreenRectangleZoomTest()
        {
            const int InitialZoom = 72;
            var layoutPosition = new Point(99.5, 4);
            var zoomRectangle = new Rect(0, 0, 584.5, 760);
            const int ScreenWidth = 1368;
            const int ScreenHeight = 768;
            const int EditorWidth = 1169;
            const int EditorHeight = 760;

            var zoomCalculator = new ZoomCalculator(
                InitialZoom,
                layoutPosition,
                ScreenWidth,
                ScreenHeight,
                EditorWidth,
                EditorHeight,
                0,
                1000);

            zoomCalculator.SetRectangleZoom(zoomRectangle);
            var newLayoutPosition = zoomCalculator.GetLayoutPosition();
            var newZoom = zoomCalculator.GetZoom();

            Assert.AreEqual(new Point(391.75, 4), newLayoutPosition, "the new LayoutPosition is not as expected");
            Assert.AreEqual(100, newZoom, "the new zoom is not as expected");
        }
    }
}