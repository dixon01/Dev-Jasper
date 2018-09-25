// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISplittable.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISplittable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Layouting
{
    /// <summary>
    /// Additional interface to be implemented by all classes that
    /// implement <see cref="ILayoutItem"/>.
    /// </summary>
    /// <typeparam name="TItem">
    /// The return type of the split items, this is always the type of the class
    /// implementing the interface itself.
    /// </typeparam>
    public interface ISplittable<TItem>
    {
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
        bool Split(int offset, out TItem left, out TItem right);
    }
}