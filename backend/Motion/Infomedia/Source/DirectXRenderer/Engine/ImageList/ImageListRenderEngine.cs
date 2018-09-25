// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageListRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageListRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.ImageList
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The image list render engine.
    /// </summary>
    public class ImageListRenderEngine
        : RenderEngineBase<ImageListItem,
                           IImageListRenderEngine<IDxDeviceRenderContext>,
                           ImageListRenderManager<IDxDeviceRenderContext>>,
          IImageListRenderEngine<IDxDeviceRenderContext>
    {
        private ImageListSprite oldSprite;
        private ImageListSprite newSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public ImageListRenderEngine(ImageListRenderManager<IDxDeviceRenderContext> manager)
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
        protected override void DoRender(double alpha, IDxDeviceRenderContext context)
        {
            if (this.oldSprite == null)
            {
                this.oldSprite = new ImageListSprite(this.Manager, context);
            }

            if (this.newSprite == null)
            {
                this.newSprite = new ImageListSprite(this.Manager, context);
            }

            var bounds = this.Manager.Bounds;
            bounds.X += this.Viewport.X;
            bounds.Y += this.Viewport.Y;

            if (this.Manager.Images.OldValue != null)
            {
                this.oldSprite.Setup(this.Manager.Images.OldValue);
                this.oldSprite.Render(bounds, (int)(255 * alpha * this.Manager.Images.OldAlpha), context);
            }

            if (this.Manager.Images.NewValue != null)
            {
                this.newSprite.Setup(this.Manager.Images.NewValue);
                this.newSprite.Render(bounds, (int)(255 * alpha * this.Manager.Images.NewAlpha), context);
            }
        }

        /// <summary>
        /// Override this method to be notified when a device is reset.
        /// If you have any DirectX resources, make sure to call
        /// OnResetDevice() on them if available.
        /// </summary>
        protected override void OnResetDevice()
        {
            base.OnResetDevice();
            if (this.oldSprite != null)
            {
                this.oldSprite.OnResetDevice();
            }

            if (this.newSprite != null)
            {
                this.newSprite.OnResetDevice();
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
    }
}
