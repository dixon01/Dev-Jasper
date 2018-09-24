// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestableLayoutItem.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestableLayoutItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Tests.Layouting
{
    using System;

    using Gorba.Motion.Infomedia.RendererBase.Layouting;

    /// <summary>
    /// Implementation of <see cref="ILayoutItem"/> used for unit testing.
    /// </summary>
    public class TestableLayoutItem : ILayoutItem, ISplittable<TestableLayoutItem>
    {
        private readonly int originalWidth;

        private readonly int originalHeight;

        private readonly int originalAscent;

        private double scaling = 1.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestableLayoutItem"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="ascent">
        /// The ascent.
        /// </param>
        /// <param name="horizontalGapAfter">
        /// The horizontal gap after this item.
        /// </param>
        public TestableLayoutItem(int width, int height, int ascent = -1, int horizontalGapAfter = 0)
            : this(string.Empty, width, height, ascent, horizontalGapAfter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestableLayoutItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="ascent">
        /// The ascent.
        /// </param>
        /// <param name="horizontalGapAfter">
        /// The horizontal gap after this item.
        /// </param>
        public TestableLayoutItem(string name, int width, int height, int ascent = -1, int horizontalGapAfter = 0)
        {
            this.Name = name;
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width");
            }

            if (height < 0)
            {
                throw new ArgumentOutOfRangeException("height");
            }

            if (ascent < -1 || ascent > height)
            {
                throw new ArgumentOutOfRangeException("ascent");
            }

            this.originalWidth = width;
            this.originalHeight = height;
            this.originalAscent = ascent == -1 ? height : ascent;
            this.HorizontalGapAfter = horizontalGapAfter;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the X coordinate of the item.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y coordinate of the item.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Gets the width of the item.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)(this.originalWidth * this.scaling);
            }
        }

        /// <summary>
        /// Gets the height of the item.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)(this.originalHeight * this.scaling);
            }
        }

        /// <summary>
        /// Gets the ascent of the item.
        /// The ascent spans the distance between the baseline and
        /// the top of the glyph that reaches farthest from the baseline.
        /// </summary>
        /// <seealso cref="http://en.wikipedia.org/wiki/Metric-compatible#Font_metrics"/>
        public int Ascent
        {
            get
            {
                return (int)(this.originalAscent * this.scaling);
            }
        }

        /// <summary>
        /// Gets the horizontal gap after this item if it is adjacent to another item.
        /// This gap is not taken into account when there is no item coming after the current one,
        /// but it is added horizontally if there is a next item on the same line.
        /// </summary>
        public int HorizontalGapAfter { get; private set; }
        
        void ILayoutItem.MoveTo(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        void ILayoutItem.SetScaling(double factor)
        {
            this.scaling = factor;
        }

        bool ISplittable<TestableLayoutItem>.Split(
            int offset, out TestableLayoutItem left, out TestableLayoutItem right)
        {
            left = this;
            right = null;
            return false;
        }
    }
}
