// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawableRenderManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DrawableRenderManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;

    /// <summary>
    /// Base class for all render managers for subclasses of
    /// <see cref="DrawableItemBase"/>.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of the <see cref="ItemBase"/> subclass that is
    /// managed by this manager.
    /// </typeparam>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="RenderManagerBase{TItem,TContext}.Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    /// <typeparam name="TEngine">
    /// The type of <see cref="IRenderEngine{TContext}"/> that is used by this manager.
    /// </typeparam>
    public partial class DrawableRenderManagerBase<TItem, TContext, TEngine>
    {
        /// <summary>
        /// Gets the the index by which render managers are sorted before rendering.
        /// </summary>
        public override int SortIndex
        {
            get
            {
                return this.ZIndex;
            }
        }
    }
}