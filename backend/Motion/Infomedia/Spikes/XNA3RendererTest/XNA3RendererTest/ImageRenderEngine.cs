namespace XNA3RendererTest
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Xna.Framework.Graphics;

    public class ImageRenderEngine
        : RenderEngineBase<ImageItem, IImageRenderEngine<IXnaRenderContext>, ImageRenderManager<IXnaRenderContext>>,
          IImageRenderEngine<IXnaRenderContext>
    {
        private ImageSprite oldSprite;
        private ImageSprite newSprite;

        public ImageRenderEngine(ImageRenderManager<IXnaRenderContext> manager)
            : base(manager)
        {
        }

        public override void Render(double alpha, IXnaRenderContext context)
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
        /// </summary>
        protected override void OnCreateDevice()
        {
            this.Release();
        }

        /// <summary>
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

        private class ImageSprite : ImageSprite<ImageItem, IImageRenderEngine<IXnaRenderContext>>
        {
            private readonly ImageRenderManager<IXnaRenderContext> manager;

            public ImageSprite(ImageRenderManager<IXnaRenderContext> manager, GraphicsDevice device)
                : base(manager, device)
            {
                this.manager = manager;
            }

            public override void Render(int alpha, IXnaRenderContext context)
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
