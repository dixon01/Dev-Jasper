// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The <see cref="IImageRenderEngine{TContext}"/> implementation for AHDLC.
    /// </summary>
    public class ImageRenderEngine :
        RenderEngineBase<ImageItem,
            IImageRenderEngine<IAhdlcRenderContext>,
            ImageRenderManager<IAhdlcRenderContext>>,
        IImageRenderEngine<IAhdlcRenderContext>
    {
        private ImageComponent lastImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public ImageRenderEngine(ImageRenderManager<IAhdlcRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.DisposeLastImage();

            base.Dispose();
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected override void DoRender(IAhdlcRenderContext context)
        {
            var image = this.CreateComponent<ImageComponent>();
            image.Filename = this.Manager.Filename.NewValue;

            if (image.Equals(this.lastImage))
            {
                // add the last image component, so the cached bitmap is preserved
                context.AddItem(this.lastImage, false);
                return;
            }

            if (this.Manager.Scaling != ElementScaling.Fixed)
            {
                this.Logger.Warn(
                    "Image rendering doesn't support scaling {0}, will show in fixed size", this.Manager.Scaling);
            }

            this.DisposeLastImage();
            this.lastImage = image;
            context.AddItem(image, true);
        }

        private void DisposeLastImage()
        {
            if (this.lastImage == null)
            {
                return;
            }

            this.lastImage.Dispose();
            this.lastImage = null;
        }
    }
}