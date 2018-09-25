// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayoutItem.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILayoutItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Layouting
{
    /// <summary>
    /// Interface to be implemented by every class that is to be laid out using the <see cref="LayoutManager{TItem}"/>.
    /// All classes implementing this interface also have to implement <see cref="ISplittable{TItem}"/>.
    /// </summary>
    public interface ILayoutItem
    {
        /// <summary>
        /// Gets the X coordinate of the item.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the Y coordinate of the item.
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Gets the width of the item.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the item.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the ascent of the item.
        /// The ascent spans the distance between the baseline and
        /// the top of the glyph that reaches farthest from the baseline.
        /// </summary>
        /// <seealso cref="http://en.wikipedia.org/wiki/Metric-compatible#Font_metrics"/>
        int Ascent { get; }

        /// <summary>
        /// Gets the horizontal gap after this item if it is adjacent to another item.
        /// This gap is not taken into account when there is no item coming after the current one,
        /// but it is added horizontally if there is a next item on the same line.
        /// </summary>
        int HorizontalGapAfter { get; }
        
        /// <summary>
        /// Moves this item to the given location.
        /// </summary>
        /// <param name="x">
        /// The new X coordinate of the item.
        /// </param>
        /// <param name="y">
        /// The new Y coordinate of the item.
        /// </param>
        void MoveTo(int x, int y);

        /// <summary>
        /// Sets the scaling factor of this item.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        void SetScaling(double factor);
    }
}