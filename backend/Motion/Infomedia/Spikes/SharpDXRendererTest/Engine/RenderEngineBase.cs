// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderEngineBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine
{
    using System;
    using System.Drawing;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using SharpDX.Direct3D9;

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
    public abstract class RenderEngineBase<TItem, TEngine, TManager> : IRenderEngine<IDxRenderContext>
        where TItem : DrawableItemBase
        where TManager : DrawableRenderManagerBase<TItem, IDxRenderContext, TEngine>
        where TEngine : class, IRenderEngine<IDxRenderContext>
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

        /// <summary>
        /// Gets the device.
        /// </summary>
        protected Device Device { get; private set; }

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
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public void Render(double alpha, IDxRenderContext context)
        {
            if (this.ShouldRender())
            {
                this.DoRender(alpha, context);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Release();

            if (this.Device == null)
            {
                return;
            }

            //this.Device.DeviceReset -= this.DeviceOnDeviceReset;
            //this.Device.DeviceLost -= this.DeviceOnDeviceLost;
            this.Device.Disposing -= this.DeviceOnDisposing;
        }

        /// <summary>
        /// Prepares this renderer to be able to render with the given framework.
        /// Usually this creates all necessary DirectX resources.
        /// </summary>
        /// <param name="device">
        /// the device.
        /// </param>
        /// <param name="viewport">
        /// the viewport that is to be rendered to the device.
        /// </param>
        public virtual void Prepare(Device device, Rectangle viewport)
        {
            this.Device = device;
            //this.Device.DeviceReset += this.DeviceOnDeviceReset;
            //this.Device.DeviceLost += this.DeviceOnDeviceLost;
            this.Device.Disposing += this.DeviceOnDisposing;

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
            return bounds.Right >= this.Viewport.Left
                && bounds.Bottom >= this.Viewport.Top
                && bounds.Left <= this.Viewport.Right
                && bounds.Top <= this.Viewport.Bottom;
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
        protected abstract void DoRender(double alpha, IDxRenderContext context);

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