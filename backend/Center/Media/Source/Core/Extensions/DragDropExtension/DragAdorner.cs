// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DragAdorner.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DragAdorner.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// the drag adorner
    /// </summary>
    internal class DragAdorner : Adorner
    {
        private readonly AdornerLayer adornerLayer;
        private readonly UIElement adornment;
        private Point mousePosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">the adorned element</param>
        /// <param name="adornment">the adornment</param>
        public DragAdorner(UIElement adornedElement, UIElement adornment)
            : base(adornedElement)
        {
            this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.adornerLayer.Add(this);
            this.adornment = adornment;
            this.IsHitTestVisible = false;
        }

        /// <summary>
        /// Gets or sets the mouse position
        /// </summary>
        public Point MousePosition
        {
            get
            {
                return this.mousePosition;
            }

            set
            {
                if (this.mousePosition != value)
                {
                    this.mousePosition = value;
                    this.adornerLayer.Update(this.AdornedElement);
                }
            }
        }

        /// <summary>
        /// Gets the visual child count
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// The detach function
        /// </summary>
        public void Detatch()
        {
            this.adornerLayer.Remove(this);
        }

        /// <summary>
        /// Gets the desired transform
        /// </summary>
        /// <param name="transform">the transform</param>
        /// <returns>the general transform</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.MousePosition.X - 4, this.MousePosition.Y - 4));

            return result;
        }

        /// <summary>
        /// The arrange override
        /// </summary>
        /// <param name="finalSize">the final size</param>
        /// <returns>the size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.adornment.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// the visual child getter
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>the visual child</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.adornment;
        }

        /// <summary>
        /// measures the adornment
        /// </summary>
        /// <param name="constraint">the constraint</param>
        /// <returns>the measurement</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.adornment.Measure(constraint);
            return this.adornment.DesiredSize;
        }
    }
}