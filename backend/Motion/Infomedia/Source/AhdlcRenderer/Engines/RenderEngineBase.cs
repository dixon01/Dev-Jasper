// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderEngineBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using NLog;

    /// <summary>
    /// Base class for AHDLC render engines.
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
    public abstract class RenderEngineBase<TItem, TEngine, TManager> : IRenderEngine<IAhdlcRenderContext>
        where TItem : DrawableItemBase
        where TManager : DrawableRenderManagerBase<TItem, IAhdlcRenderContext, TEngine>
        where TEngine : class, IRenderEngine<IAhdlcRenderContext>
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
            this.Logger = LogHelper.GetLogger(this.GetType());
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
        public void Render(double alpha, IAhdlcRenderContext context)
        {
            if (this.Manager.Visible > 0)
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
        protected abstract void DoRender(IAhdlcRenderContext context);

        /// <summary>
        /// Creates a component of the given type and populates its common properties.
        /// </summary>
        /// <typeparam name="T">
        /// The type of component to create.
        /// </typeparam>
        /// <returns>
        /// The new component.
        /// </returns>
        protected T CreateComponent<T>() where T : ComponentBase, new()
        {
            return new T
                       {
                           X = this.Manager.X,
                           Y = this.Manager.Y,
                           Width = this.Manager.Width,
                           Height = this.Manager.Height,
                           ZIndex = this.Manager.ZIndex,
                           Visible = this.Manager.Visible >= 1.0
                       };
        }
    }
}