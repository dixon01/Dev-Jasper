// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderEngineBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX.Direct3D;
    using Microsoft.Samples.DirectX.UtilityToolkit;

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
        /// Gets the framework.
        /// </summary>
        protected Framework Framework { get; private set; }

        /// <summary>
        /// Gets the device.
        /// </summary>
        protected Device Device
        {
            get
            {
                return this.Framework.Device;
            }
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
        public abstract void Render(double alpha, IDxRenderContext context);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Release();

            if (this.Framework == null)
            {
                return;
            }

            this.Framework.DeviceCreated -= this.FrameworkOnDeviceCreated;
            this.Framework.DeviceReset -= this.FrameworkOnDeviceReset;
            this.Framework.DeviceLost -= this.FrameworkOnDeviceLost;
            this.Framework.Disposing -= this.FrameworkOnDisposing;
        }

        /// <summary>
        /// Prepares this renderer to be able to render with the given framework.
        /// Usually this creates all necessary DirectX resources.
        /// </summary>
        /// <param name="fw">
        /// the framework.
        /// </param>
        public virtual void Prepare(Framework fw)
        {
            this.Framework = fw;
            this.Framework.DeviceCreated += this.FrameworkOnDeviceCreated;
            this.Framework.DeviceReset += this.FrameworkOnDeviceReset;
            this.Framework.DeviceLost += this.FrameworkOnDeviceLost;
            this.Framework.Disposing += this.FrameworkOnDisposing;
        }

        /// <summary>
        /// Releases all previously created DirectX resources.
        /// </summary>
        protected abstract void Release();

        /// <summary>
        /// Override this method to be notified when a new device is created.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected virtual void OnCreateDevice(Device device)
        {
        }

        /// <summary>
        /// Override this method to be notified when a device is reset.
        /// If you have any DirectX resources, make sure to call
        /// OnResetDevice() on them if available.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected virtual void OnResetDevice(Device device)
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

        private void FrameworkOnDisposing(object sender, EventArgs e)
        {
            this.Release();
        }

        private void FrameworkOnDeviceLost(object sender, EventArgs e)
        {
            this.OnLostDevice();
        }

        private void FrameworkOnDeviceReset(object sender, DeviceEventArgs e)
        {
            this.OnResetDevice(e.Device);
        }

        private void FrameworkOnDeviceCreated(object sender, DeviceEventArgs e)
        {
            this.OnCreateDevice(e.Device);
        }
    }
}