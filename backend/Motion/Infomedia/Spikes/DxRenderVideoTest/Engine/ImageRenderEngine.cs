// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The image render engine.
    /// </summary>
    public class ImageRenderEngine
        : RenderEngineBase<ImageItem, IImageRenderEngine<IDxRenderContext>, ImageRenderManager<IDxRenderContext>>,
          IImageRenderEngine<IDxRenderContext>
    {
        private ImageSprite oldSprite;
        private ImageSprite newSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public ImageRenderEngine(ImageRenderManager<IDxRenderContext> manager)
            : base(manager)
        {
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
        public override void Render(double alpha, IDxRenderContext context)
        {
            if (this.oldSprite == null)
            {
                this.oldSprite = new ImageSprite(this.Manager, this.Device);
            }

            if (this.newSprite == null)
            {
                this.newSprite = new ImageSprite(this.Manager, this.Device);
            }

            var oldFilename = this.Manager.Filename.OldValue;
            var newFilename = this.Manager.Filename.NewValue;

            if (oldFilename != null)
            {
                if (this.oldSprite.CurrentFilename != oldFilename
                    && this.newSprite.CurrentFilename == oldFilename)
                {
                    // swap sprites
                    var sprite = this.oldSprite;
                    this.oldSprite = this.newSprite;
                    this.newSprite = sprite;
                }

                this.oldSprite.Setup(oldFilename);
                this.oldSprite.Render((int)(255 * alpha * this.Manager.Filename.OldAlpha), context);
            }

            if (newFilename != null)
            {
                this.newSprite.Setup(newFilename);
                this.newSprite.Render((int)(255 * alpha * this.Manager.Filename.NewAlpha), context);
            }
        }

        /// <summary>
        /// Override this method to be notified when a new device is created.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected override void OnCreateDevice(Device device)
        {
            this.Release();
        }

        /// <summary>
        /// Override this method to be notified when a device is reset.
        /// If you have any DirectX resources, make sure to call
        /// OnResetDevice() on them if available.
        /// </summary>
        /// <param name="dev">
        /// The device.
        /// </param>
        protected override void OnResetDevice(Device dev)
        {
            base.OnResetDevice(dev);
            if (this.oldSprite != null)
            {
                this.oldSprite.OnResetDevice(dev);
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnResetDevice(dev);
            }
        }

        /// <summary>
        /// Override this method to be notified when a device is lost.
        /// If you have any DirectX resources, make sure to call
        /// OnLostDevice() on them if available.
        /// </summary>
        protected override void OnLostDevice()
        {
            base.OnLostDevice();
            if (this.oldSprite != null)
            {
                this.oldSprite.OnLostDevice();
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnLostDevice();
            }
        }

        /// <summary>
        /// Releases all previously created DirectX resources.
        /// </summary>
        protected override void Release()
        {
            if (this.oldSprite != null)
            {
                this.oldSprite.Dispose();
                this.oldSprite = null;
            }

            if (this.newSprite != null)
            {
                this.newSprite.Dispose();
                this.newSprite = null;
            }
        }

        private class ImageSprite : ImageSprite<ImageItem, IImageRenderEngine<IDxRenderContext>>
        {
            private readonly ImageRenderManager<IDxRenderContext> manager;

            public ImageSprite(ImageRenderManager<IDxRenderContext> manager, Device device)
                : base(manager, device)
            {
                this.manager = manager;
            }

            public override void Render(int alpha, IDxRenderContext context)
            {
                if (this.manager.Blink && !context.BlinkOn)
                {
                    return;
                }

                base.Render(alpha, context);
            }
        }
    }
}