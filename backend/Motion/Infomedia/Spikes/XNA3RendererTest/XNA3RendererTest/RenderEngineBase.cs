// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderEngineBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// </summary>
    /// <typeparam name="TItem">
    /// </typeparam>
    /// <typeparam name="TEngine">
    /// </typeparam>
    /// <typeparam name="TManager">
    /// </typeparam>
    public abstract class RenderEngineBase<TItem, TEngine, TManager> : IRenderEngine<IXnaRenderContext>
        where TItem : DrawableItemBase
        where TManager : DrawableRenderManagerBase<TItem, IXnaRenderContext, TEngine>
        where TEngine : class, IRenderEngine<IXnaRenderContext>
    {
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

        protected abstract void Release();

        protected virtual void OnCreateDevice()
        {
        }
 
        protected virtual void OnResetDevice()
        {
        }

        private void GraphicsOnDeviceDisposing(object sender, EventArgs e)
        {
            this.Release();
        }

        private void GraphicsOnDeviceReset(object sender, EventArgs e)
        {
            this.OnResetDevice();
        }

        private void GraphicsOnDeviceCreated(object sender, EventArgs e)
        {
            this.OnCreateDevice();
        }
    }
}
