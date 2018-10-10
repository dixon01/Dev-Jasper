// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrappableLayoutItem.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrappableLayoutItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Tests.Layouting
{
    using System;
    using System.Linq;
    using System.Text;

    using Gorba.Motion.Infomedia.RendererBase.Layouting;

    /// <summary>
    /// Implementation of <see cref="ILayoutItem"/> that contains multiple <see cref="TestableLayoutItem"/>.
    /// </summary>
    public class WrappableLayoutItem : ILayoutItem, ISplittable<WrappableLayoutItem>
    {
        private readonly TestableLayoutItem[] items;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappableLayoutItem"/> class.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public WrappableLayoutItem(params TestableLayoutItem[] items)
        {
            this.items = items;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                var name = new StringBuilder(this.items.Length);
                foreach (var item in this.items)
                {
                    name.Append(item.Name);
                }

                return name.ToString();
            }
        }

        /// <summary>
        /// Gets the X coordinate of the item.
        /// </summary>
        public int X
        {
            get
            {
                return this.items[0].X;
            }
        }

        /// <summary>
        /// Gets the Y coordinate of the item.
        /// </summary>
        public int Y
        {
            get
            {
                return this.items[0].Y;
            }
        }

        /// <summary>
        /// Gets the width of the item.
        /// </summary>
        public int Width
        {
            get
            {
                return this.items.Sum(item => item.Width);
            }
        }

        /// <summary>
        /// Gets the height of the item.
        /// </summary>
        public int Height
        {
            get
            {
                return this.items.Max(item => item.Height);
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
                return this.items.Max(item => item.Ascent);
            }
        }

        /// <summary>
        /// Gets the horizontal gap after this item if it is adjacent to another item.
        /// This gap is not taken into account when there is no item coming after the current one,
        /// but it is added horizontally if there is a next item on the same line.
        /// </summary>
        public int HorizontalGapAfter
        {
            get
            {
                return this.items.Select(i => i.HorizontalGapAfter).LastOrDefault();
            }
        }
        
        /// <summary>
        /// Moves this item to the given location.
        /// </summary>
        /// <param name="x">
        /// The new X coordinate of the item.
        /// </param>
        /// <param name="y">
        /// The new Y coordinate of the item.
        /// </param>
        public void MoveTo(int x, int y)
        {
            var dx = x - this.X;
            var dy = y - this.Y;

            foreach (ILayoutItem item in this.items)
            {
                item.MoveTo(item.X + dx, item.Y + dy);
            }
        }

        /// <summary>
        /// Sets the scaling factor of this item.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        public void SetScaling(double factor)
        {
            var x = 0;
            foreach (ILayoutItem item in this.items)
            {
                item.MoveTo(x, item.Y);
                item.SetScaling(factor);
                x += item.Width;
            }
        }

        /// <summary>
        /// Tries to split the item into two parts at the given offset.
        /// The last possible split point in this item has to be found (meaning where the width of
        /// the returned <see cref="left"/> item is less than or equal to the given <see cref="offset"/>).
        /// If this item can't be split, the method must return false and <see cref="right"/> must be null.
        /// If the first possible split point is past <see cref="offset"/>, this method should split
        /// at that point and return true.
        /// </summary>
        /// <param name="offset">
        /// The offset at which this item should be split.
        /// </param>
        /// <param name="left">
        /// The left item of the split operation. This is never null.
        /// If the item couldn't be split, this return parameter might be the object this method was called on.
        /// </param>
        /// <param name="right">
        /// The right item of the split operation. This is null if the method returns false.
        /// </param>
        /// <returns>
        /// A flag indicating if the split operation was successful.
        /// </returns>
        public bool Split(int offset, out WrappableLayoutItem left, out WrappableLayoutItem right)
        {
            var x = 0;
            for (int i = 0; i < this.items.Length; i++)
            {
                var item = this.items[i];
                x += item.Width;
                if (x <= offset)
                {
                    continue;
                }

                if (i == 0)
                {
                    // can't split if the first item is already too big
                    if (this.items.Length == 1)
                    {
                        break;
                    }

                    i = 1;
                }

                var first = new TestableLayoutItem[i];
                Array.Copy(this.items, first, i);
                var second = new TestableLayoutItem[this.items.Length - i];
                Array.Copy(this.items, i, second, 0, second.Length);

                left = new WrappableLayoutItem(first);
                right = new WrappableLayoutItem(second);
                return true;
            }

            left = this;
            right = null;
            return false;
        }
    }
}
