// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomCalculator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ZoomCalculator.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System.Windows;

    /// <summary>
    /// The ZoomController.
    /// </summary>
    public class ZoomCalculator
    {
        private readonly int screenWidth;

        private readonly int screenHeight;

        private readonly double editorWidth;

        private readonly double editorHeight;

        private readonly double minZoom;

        private readonly double maxZoom;

        private double zoom;

        private Point layoutPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomCalculator"/> class.
        /// </summary>
        /// <param name="zoom">the previous zoom value</param>
        /// <param name="layoutPosition">the previous layout position</param>
        /// <param name="screenWidth">the previous screen width</param>
        /// <param name="screenHeight">the previous screen height</param>
        /// <param name="editorWidth">the previous editor width</param>
        /// <param name="editorHeight">the previous editor height</param>
        /// <param name="minZoom">the minimum zoom factor</param>
        /// <param name="maxZoom">the maximum zoom factor</param>
        public ZoomCalculator(
            double zoom,
            Point layoutPosition,
            int screenWidth,
            int screenHeight,
            double editorWidth,
            double editorHeight,
            double minZoom,
            double maxZoom)
        {
            this.zoom = zoom;
            this.layoutPosition = layoutPosition;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.editorWidth = editorWidth;
            this.editorHeight = editorHeight;
            this.minZoom = minZoom;
            this.maxZoom = maxZoom;
        }

        /// <summary>
        /// Calculate the zoom for a rectangle
        /// </summary>
        /// <param name="rectangle">the rectangle</param>
        public void SetRectangleZoom(Rect rectangle)
        {
            var widthRatio = this.editorWidth / rectangle.Width;
            var heightRatio = this.editorHeight / rectangle.Height;
            if (widthRatio < heightRatio)
            {
                this.zoom = widthRatio * 100;
            }
            else
            {
                this.zoom = heightRatio * 100;
            }

            var center = rectangle.TopLeft + new Vector(
                rectangle.Width / 2,
                rectangle.Height / 2);

            if (this.zoom < this.minZoom)
            {
                this.zoom = this.minZoom;
            }
            else if (this.zoom > this.maxZoom)
            {
                this.zoom = this.maxZoom;
            }

            this.SetNewZoomCenter(center);
        }

        /// <summary>
        /// Returns the current calculated Layout Position
        /// </summary>
        /// <returns>the layout position</returns>
        public Point GetLayoutPosition()
        {
            return this.layoutPosition;
        }

        /// <summary>
        /// Returns the current calculated zoom
        /// </summary>
        /// <returns>the zoom</returns>
        public double GetZoom()
        {
            return this.zoom;
        }

        /// <summary>
        /// Zooms the view in at a certain point
        /// </summary>
        /// <param name="point">the reference point</param>
        /// <param name="zoomStep">the zoom increment</param>
        public void ZoomInAt(Point point, double zoomStep)
        {
            var interactionVector = new Vector(point.X, point.Y);
            var screenCenterOffset = new Vector(this.screenWidth / 2.0, this.screenHeight / 2.0);
            var interactionToCenterOffset = screenCenterOffset - interactionVector;

            this.zoom += zoomStep;

            if (this.zoom < this.minZoom)
            {
                this.zoom = this.minZoom;
            }
            else if (this.zoom > this.maxZoom)
            {
                this.zoom = this.maxZoom;
            }
            else
            {
                this.layoutPosition += interactionToCenterOffset * (zoomStep / 100);

                this.CheckLayoutBounds();
            }
        }

        /// <summary>
        /// Zooms the view out at a certain point
        /// </summary>
        /// <param name="point">the reference point</param>
        /// <param name="zoomStep">the zoom increment</param>
        public void ZoomOutAt(Point point, double zoomStep)
        {
            var screenCenter = new Vector(this.screenWidth / 2.0, this.screenHeight / 2.0);
            var interactionVector = new Vector(point.X, point.Y);
            var interactionToCenterOffset = screenCenter - interactionVector;

            this.zoom -= zoomStep;

            if (this.zoom < this.minZoom)
            {
                this.zoom = this.minZoom;
            }
            else if (this.zoom > this.maxZoom)
            {
                this.zoom = this.maxZoom;
            }
            else
            {
                this.layoutPosition -= interactionToCenterOffset * (zoomStep / 100);

                this.CheckLayoutBounds();
            }
        }

        /// <summary>
        /// Zooms the view in
        /// </summary>
        /// <param name="zoomStep">the zoom increment</param>
        public void ZoomIn(double zoomStep)
        {
            if (this.zoom + zoomStep < this.maxZoom)
            {
                this.zoom += zoomStep;
            }

            this.CheckLayoutBounds();
        }

        /// <summary>
        /// Zooms the view out
        /// </summary>
        /// <param name="zoomStep">the zoom increment</param>
        public void ZoomOut(double zoomStep)
        {
            if (this.zoom - zoomStep > this.minZoom)
            {
                this.zoom -= zoomStep;
            }

            this.CheckLayoutBounds();
        }

        private void SetNewZoomCenter(Point center)
        {
            var screenCenter = new Point(this.screenWidth / 2.0, this.screenHeight / 2.0);

            var newPosition = -(center - screenCenter);
            newPosition *= this.zoom / 100;
            this.layoutPosition = new Point(newPosition.X, newPosition.Y);
            this.CheckLayoutBounds();
        }

        private void CheckLayoutBounds()
        {
            if (this.layoutPosition.X < -this.editorWidth * this.zoom)
            {
                this.layoutPosition.X = -this.editorWidth * this.zoom;
            }
            else if (this.layoutPosition.X > this.editorWidth * this.zoom)
            {
                this.layoutPosition.X = this.editorWidth * this.zoom;
            }

            if (this.layoutPosition.Y < -this.editorHeight * this.zoom)
            {
                this.layoutPosition.Y = -this.editorHeight * this.zoom;
            }
            else if (this.layoutPosition.Y > this.editorHeight * this.zoom)
            {
                this.layoutPosition.Y = this.editorHeight * this.zoom;
            }
        }
    }
}