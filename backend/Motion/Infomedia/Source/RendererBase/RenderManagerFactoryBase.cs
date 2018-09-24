// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerFactoryBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerFactoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// Base class for a render manager factory.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the
    /// <see cref="RenderManagerBase{TItem,TContext}.Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public abstract partial class RenderManagerFactoryBase<TContext>
        where TContext : IRenderContext
    {
        private delegate TEngine CreateEngineMethod<TItem, TManager, TEngine>(TManager manager)
            where TItem : ScreenItemBase
            where TManager : ScreenItemRenderManagerBase<TItem, TContext, TEngine>
            where TEngine : class, IRenderEngine<TContext>;

        /// <summary>
        /// Creates a screen render manager for the given screen root.
        /// </summary>
        /// <param name="screen">
        /// The screen root.
        /// </param>
        /// <returns>
        /// The screen render manager which contains all sub-renderers.
        /// </returns>
        public ScreenRootRenderManager<TContext> CreateRenderManager(ScreenRoot screen)
        {
            return new ScreenRootRenderManager<TContext>(screen, this);
        }

        /// <summary>
        /// Creates a root render manager for the given root item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The root render manager which contains all sub-renderers.
        /// </returns>
        internal RootRenderManager<TContext> CreateRenderManager(RootItem item)
        {
            return new RootRenderManager<TContext>(item, this);
        }

        /// <summary>
        /// Creates an <see cref="IIncludeRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected virtual IIncludeRenderEngine<TContext> CreateEngine(IncludeRenderManager<TContext> manager)
        {
            // special case: usually we don't need an engine for the include item,
            // therefore we have a default implementation here that returns null.
            return null;
        }

        private static IScreenItemRenderManager<TContext> Connect<TItem, TManager, TEngine>(
            TManager manager,
            CreateEngineMethod<TItem, TManager, TEngine> createEngine)
            where TManager : ScreenItemRenderManagerBase<TItem, TContext, TEngine>
            where TItem : ScreenItemBase
            where TEngine : class, IRenderEngine<TContext>
        {
            var engine = createEngine(manager);
            if (engine != null)
            {
                manager.Connect(engine);
            }

            return manager;
        }
    }
}
