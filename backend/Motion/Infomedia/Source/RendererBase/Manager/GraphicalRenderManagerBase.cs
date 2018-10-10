// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalRenderManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GraphicalRenderManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;

    /// <summary>
    /// Base class for all render managers for subclasses of
    /// <see cref="GraphicalItemBase"/>.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of the <see cref="GraphicalItemBase"/> subclass that is
    /// managed by this manager.
    /// </typeparam>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    /// <typeparam name="TEngine">
    /// The type of <see cref="IRenderEngine{TContext}"/> that is used by this manager.
    /// </typeparam>
    public partial class GraphicalRenderManagerBase<TItem, TContext, TEngine>
    {
        /// <summary>
        /// Gets the bounds of this item.
        /// This method is a shortcut for calling all bounds properties one after the other.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(this.X, this.Y, this.Width, this.Height);
            }
        }

        /// <summary>
        /// Renders the contents using the engine provided with
        /// <see cref="ScreenItemRenderManagerBase{TItem,TContext,TEngine}.Connect"/>.
        /// This method can't be overridden anymore, please override <see cref="DoRender"/> instead.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public sealed override void Render(double alpha, TContext context)
        {
            var innerAlpha = alpha * this.Visible;
            base.Render(innerAlpha, context);

            this.DoRender(innerAlpha, context);
        }

        /// <summary>
        /// Specific rendering implementation for this manager.
        /// In most cases, this doesn't need to be overridden.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected virtual void DoRender(double alpha, TContext context)
        {
        }
    }
}