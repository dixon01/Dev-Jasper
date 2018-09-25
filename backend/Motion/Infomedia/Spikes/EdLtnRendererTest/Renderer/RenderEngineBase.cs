namespace Gorba.Motion.Infomedia.EdLtnRendererTest.Renderer
{
    using Gorba.Motion.Infomedia.EdLtnRendererTest.Protocol;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    public abstract class RenderEngineBase<TItem, TEngine, TManager> : IRenderEngine<ILtnRenderContext>
        where TItem : DrawableItemBase
        where TManager : DrawableRenderManagerBase<TItem, ILtnRenderContext, TEngine>
        where TEngine : class, IRenderEngine<ILtnRenderContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderEngineBase{TItem,TEngine,TManager}"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        protected RenderEngineBase(TManager manager)
        {
            this.Manager = manager;
        }

        /// <summary>
        /// Gets the manager.
        /// </summary>
        protected TManager Manager { get; private set; }

        protected LtnSerialPort SerialPort { get; private set; }

        public void Prepare(LtnSerialPort serialPort)
        {
            this.SerialPort = serialPort;
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public abstract void Render(double alpha, ILtnRenderContext context);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }
    }
}
