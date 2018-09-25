// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderEngineBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Engine
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using NLog;

    /// <summary>
    /// Base class for render engines.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of the item to be rendered.
    /// </typeparam>
    /// <typeparam name="TEngine">
    /// The type of the engine.
    /// </typeparam>
    /// <typeparam name="TManager">
    /// The type of the manager.
    /// </typeparam>
    public abstract class RenderEngineBase<TItem, TEngine, TManager> : IRenderEngine<IAudioRenderContext>
        where TItem : AudioItemBase
        where TManager : AudioRenderManagerBase<TItem, IAudioRenderContext, TEngine>
        where TEngine : class, IRenderEngine<IAudioRenderContext>
    {
        /// <summary>
        /// The logger to be used by subclasses.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderEngineBase{TItem,TEngine,TManager}"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        protected RenderEngineBase(TManager manager)
        {
            this.Manager = manager;
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the manager.
        /// </summary>
        protected TManager Manager { get; private set; }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public void Render(double alpha, IAudioRenderContext context)
        {
            if (this.Manager.Enabled)
            {
                this.DoRender(context);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected abstract void DoRender(IAudioRenderContext context);
    }
}