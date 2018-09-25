// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PartBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Parts
{
    using System;

    using Gorba.Motion.Infomedia.RendererBase.Layouting;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    /// <summary>
    /// A part of a text, used for layout and format handling.
    /// </summary>
    public abstract class PartBase : ILayoutItem, ISplittable<PartBase>, IPart, IEquatable<PartBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartBase"/> class.
        /// </summary>
        protected PartBase()
        {
            this.Blink = false;
        }

        /// <summary>
        /// Gets the X coordinate of the item.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y coordinate of the item.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Gets or sets the width of the item.
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// Gets or sets the height of the item.
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// Gets or sets the ascent of the item.
        /// The ascent spans the distance between the baseline and
        /// the top of the glyph that reaches farthest from the baseline.
        /// </summary>
        /// <seealso cref="http://en.wikipedia.org/wiki/Metric-compatible#Font_metrics"/>
        public int Ascent { get; protected set; }

        /// <summary>
        /// Gets or sets the horizontal gap after this item if it is adjacent to another item.
        /// This gap is not taken into account when there is no item coming after the current one,
        /// but it is added horizontally if there is a next item on the same line.
        /// </summary>
        public int HorizontalGapAfter { get; protected set; }
        
        /// <summary>
        /// Gets a value indicating whether this part should blink.
        /// </summary>
        public bool Blink { get; private set; }

        /// <summary>
        /// Sets the scaling factor of this item.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        public abstract void SetScaling(double factor);

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
        public abstract bool Split(int offset, out PartBase left, out PartBase right);

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        public abstract IPart Duplicate();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public abstract bool Equals(PartBase other);

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
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="Object"/> to compare with the current <see cref="Object"/>.
        /// </param>
        public override bool Equals(object obj)
        {
            var other = obj as PartBase;

            return other != null && this.Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }
    }
}
