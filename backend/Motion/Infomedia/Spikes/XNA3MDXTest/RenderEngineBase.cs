// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3MDXTest
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This is the RenderEngineBase.
    /// </summary>
    /// <typeparam name="TItem">The item to be rendered.</typeparam>
    /// <typeparam name="TEngine">The renderer.</typeparam>
    /// <typeparam name="TManager">The manager.</typeparam>
    public abstract class RenderEngineBase<TItem, TEngine, TManager> : IRenderEngine<IXnaRenderContext>
        where TItem : DrawableItemBase
        where TManager : DrawableRenderManagerBase<TItem, IXnaRenderContext, TEngine>
        where TEngine : class, IRenderEngine<IXnaRenderContext>
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
        protected GraphicsDeviceManager Graphics { get; private set; }

        /// <summary>
        /// Gets the device.
        /// </summary>
        protected GraphicsDevice Device
        {
            get
            {
                return this.Graphics.GraphicsDevice;
            }
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public abstract void Render(double alpha, IXnaRenderContext context);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Release();

            if (this.Graphics == null)
            {
                return;
            }

            this.Graphics.DeviceCreated -= this.GraphicsOnDeviceCreated;
            this.Graphics.DeviceReset -= this.GraphicsOnDeviceReset;
            this.Graphics.DeviceDisposing -= this.GraphicsOnDeviceDisposing;
        }

        /// <summary>
        /// Prepares the graphics device manager.
        /// </summary>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        public virtual void Prepare(GraphicsDeviceManager graphics)
        {
            this.Graphics = graphics;
            this.Graphics.DeviceCreated += this.GraphicsOnDeviceCreated;
            this.Graphics.DeviceReset += this.GraphicsOnDeviceReset;
            this.Graphics.DeviceDisposing += this.GraphicsOnDeviceDisposing;
        }

        /// <summary>
        /// The release.
        /// </summary>
        protected abstract void Release();

        /// <summary>
        /// The on create device.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected virtual void OnCreateDevice(GraphicsDevice device)
        {
        }

        /// <summary>
        /// The on reset device.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected virtual void OnResetDevice(GraphicsDevice device)
        {
        }

        private void GraphicsOnDeviceDisposing(object sender, EventArgs e)
        {
            this.Release();
        }

        private void GraphicsOnDeviceReset(object sender, EventArgs e)
        {
            this.OnResetDevice(this.Graphics.GraphicsDevice);
        }

        private void GraphicsOnDeviceCreated(object sender, EventArgs e)
        {
            this.OnCreateDevice(this.Graphics.GraphicsDevice);
        }
    }
}
