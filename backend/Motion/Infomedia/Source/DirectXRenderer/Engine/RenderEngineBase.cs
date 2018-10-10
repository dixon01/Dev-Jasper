// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderEngineBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine
{
    using System;
    using System.Drawing;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

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
    public abstract class RenderEngineBase<TItem, TEngine, TManager> : IRenderEngine<IDxDeviceRenderContext>
        where TItem : DrawableItemBase
        where TManager : GraphicalRenderManagerBase<TItem, IDxDeviceRenderContext, TEngine>
        where TEngine : class, IRenderEngine<IDxDeviceRenderContext>
    {
        private IDxDeviceRenderContext context;

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

        /// <summary>
        /// Gets the virtual viewport of the screen.
        /// </summary>
        protected Rectangle Viewport { get; private set; }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public void Render(double alpha, IDxDeviceRenderContext renderContext)
        {
            if (this.ShouldRender())
            {
                this.DoRender(alpha, renderContext);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Release();

            if (this.context == null)
            {
                return;
            }

            this.context.Device.DeviceReset -= this.DeviceOnDeviceReset;
            this.context.Device.DeviceLost -= this.DeviceOnDeviceLost;
            this.context.Device.Disposing -= this.DeviceOnDisposing;
        }

        /// <summary>
        /// Prepares this renderer to be able to render with the given framework.
        /// Usually this creates all necessary DirectX resources.
        /// </summary>
        /// <param name="renderContext">
        /// the render context.
        /// </param>
        /// <param name="viewport">
        /// the viewport that is to be rendered to the device.
        /// </param>
        public virtual void Prepare(IDxDeviceRenderContext renderContext, Rectangle viewport)
        {
            this.context = renderContext;
            this.context.Device.DeviceReset += this.DeviceOnDeviceReset;
            this.context.Device.DeviceLost += this.DeviceOnDeviceLost;
            this.context.Device.Disposing += this.DeviceOnDisposing;

            this.Viewport = viewport;
        }

        /// <summary>
        /// Test if this engine should render.
        /// Returns only true if this object is within the virtual
        /// viewport of the screen.
        /// </summary>
        /// <returns>
        /// True if this object is within the virtual viewport of the screen.
        /// </returns>
        protected virtual bool ShouldRender()
        {
            var bounds = this.Manager.Bounds;
            return bounds.Right >= 0
                && bounds.Bottom >= 0
                && bounds.Left <= this.Viewport.Width
                && bounds.Top <= this.Viewport.Height;
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
        protected abstract void DoRender(double alpha, IDxDeviceRenderContext context);

        /// <summary>
        /// Releases all previously created DirectX resources.
        /// </summary>
        protected abstract void Release();

        /// <summary>
        /// Override this method to be notified when a device is reset.
        /// If you have any DirectX resources, make sure to call
        /// OnResetDevice() on them if available.
        /// </summary>
        protected virtual void OnResetDevice()
        {
        }

        /// <summary>
        /// Override this method to be notified when a device is lost.
        /// If you have any DirectX resources, make sure to call
        /// OnLostDevice() on them if available.
        /// </summary>
        protected virtual void OnLostDevice()
        {
        }

        private void DeviceOnDisposing(object sender, EventArgs e)
        {
            this.Release();
        }

        private void DeviceOnDeviceLost(object sender, EventArgs e)
        {
            this.OnLostDevice();
        }

        private void DeviceOnDeviceReset(object sender, EventArgs eventArgs)
        {
            this.OnResetDevice();
        }
    }
}