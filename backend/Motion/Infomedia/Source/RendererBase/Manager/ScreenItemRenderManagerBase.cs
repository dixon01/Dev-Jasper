// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenItemRenderManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenItemRenderManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;

    /// <summary>
    /// Base class for all render managers for subclasses of
    /// <see cref="ScreenItemBase"/>.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of the <see cref="ScreenItemBase"/> subclass that is
    /// managed by this manager.
    /// </typeparam>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    /// <typeparam name="TEngine">
    /// The type of <see cref="IRenderEngine{TContext}"/> that is used by this manager.
    /// </typeparam>
    public abstract class ScreenItemRenderManagerBase<TItem, TContext, TEngine>
        : RenderManagerBase<TItem, TContext>, IScreenItemRenderManager<TContext>
        where TItem : ScreenItemBase
        where TContext : IRenderContext
        where TEngine : class, IRenderEngine<TContext>
    {
        private TEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenItemRenderManagerBase{TItem,TContext,TEngine}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item to be rendered.
        /// </param>
        /// <param name="factory">
        /// The factory creating this manager.
        /// </param>
        internal ScreenItemRenderManagerBase(TItem item, RenderManagerFactoryBase<TContext> factory)
            : base(item)
        {
            this.Factory = factory;
        }

        /// <summary>
        /// Gets the the index by which render managers are sorted before rendering.
        /// </summary>
        public abstract int SortIndex { get; }

        /// <summary>
        /// Gets the factory that was used to create this manager.
        /// </summary>
        protected RenderManagerFactoryBase<TContext> Factory { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Render"/> method should do anything
        /// even if the video is invisible.
        /// </summary>
        protected bool RenderIfInvisible { get; set; }

        /// <summary>
        /// Renders the contents using the engine provided with <see cref="Connect"/>.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public override void Render(double alpha, TContext context)
        {
            if (this.engine != null && (alpha > 0 || this.RenderIfInvisible))
            {
                this.engine.Render(alpha, context);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();

            if (this.engine != null)
            {
                this.engine.Dispose();
            }
        }

        RootRenderManager<TContext> IScreenItemRenderManager<TContext>.FindRoot(int id)
        {
            return this.FindRoot(id);
        }

        void IScreenItemRenderManager<TContext>.UpdateItems(ScreenUpdate update)
        {
            this.UpdateItems(update);
        }

        /// <summary>
        /// Connects this manager to the given engine.
        /// </summary>
        /// <param name="renderEngine">
        /// The render engine.
        /// </param>
        internal void Connect(TEngine renderEngine)
        {
            if (this.engine != null)
            {
                throw new NotSupportedException("Can't connect to another engine");
            }

            this.engine = renderEngine;
        }
    }
}