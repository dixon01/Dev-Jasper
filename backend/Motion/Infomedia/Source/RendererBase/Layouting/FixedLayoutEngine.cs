// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedLayoutEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FixedLayoutEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Layouting
{
    /// <summary>
    /// Layout engine that doesn't change the size or position of the items,
    /// but just places them next to each other (taking into account line breaks).
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of <see cref="ILayoutItem"/> that is being laid out.
    /// </typeparam>
    internal class FixedLayoutEngine<TItem> : LayoutEngineBase<TItem>
        where TItem : class, ILayoutItem, ISplittable<TItem>
    {
    }
}